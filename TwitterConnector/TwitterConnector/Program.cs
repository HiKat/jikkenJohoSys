using System;
using System.Security.Cryptography;
using System.Configuration;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using TwitterOAuth;
using TwitterTweetJson;

namespace TwitterGetContent
{
	
	class Program
	{
		static void Main (string[] args)
		{
//			//リクエストトークン取得====================================================================
//			//ApiTestNoCallback
//			//callbackurl設定なしアカウント
//			const string CONSUMER_KEY = "8369eInu3C2YzkAgx3YrWbXMJ";
//			const string CONSUMER_SECRET = "8uWxX1iTTR1yxdA25RqlQxxn82tDipnsC5482UU3Etm3EpwQGn";
//
//			Auth auth = new Auth (CONSUMER_KEY, CONSUMER_SECRET);
//
//			// リクエストトークンを取得する
//			auth.GetRequestToken ();
//			Console.WriteLine ("RequestToken: " + auth.RequestToken);
//			Console.WriteLine ("RequestTokenSecret: " + auth.RequestTokenSecret);
//
//			//ユーザーにRequestTokenを認証してもらう
//			Console.WriteLine ("Authentication URL：");
//			Console.WriteLine ("https://api.twitter.com/oauth/authorize?oauth_token=" + auth.RequestToken);
//			Console.Write ("PIN：");
//			string pin = Console.ReadLine ().Trim ();
//			// アクセストークンを取得する
//			auth.GetAccessToken (pin);
//
//			// 結果を表示する
//			Console.WriteLine ("AccessToken: " + auth.AccessToken);
//			Console.WriteLine ("AccessTokenSecret: " + auth.AccessTokenSecret);
//			Console.WriteLine ("UserId: " + auth.UserId);
//			Console.WriteLine ("ScreenName: " + auth.ScreenName);
//			//=====================================================================================


			//デバッグ用================================================================================
			const string CONSUMER_KEY = "IKBVUl9gifsSDrqEOeGBYEv3c";
			const string CONSUMER_SECRET = "6qqHzsX24YiyRyKHfeOuV4vAdPobKkPvChxgid07QTQJsnfapu";
			const string ACCESS_TOKEN = "4452190160-YwOa1h9VQUNRRYhp2MMMXMDssSyeEO9DcsDgydQ";
			const string ACCESS_TOKEN_SECRET = "YYT4zmqKMd7w6W0k2SulA5yHPByQ2KCFAmRK1NFYetfIk";
			const string USER_ID = "4452190160";
			const string SCREEN_NAME = "apiTestJohoSys";
		
			Auth auth = new Auth (
				            CONSUMER_KEY,
				            CONSUMER_SECRET,
				            ACCESS_TOKEN,
				            ACCESS_TOKEN_SECRET,
				            USER_ID,
				            SCREEN_NAME
			            );
			//=======================================================================================





			//====================================================
			//debug
			Console.WriteLine ("debug start!");
			Console.ReadKey ();

			//auth.GetTimeLine ();

			//タイムライン取得例

			TwitterConnector tc = new TwitterConnector (
				                      auth.ConsumerKey, auth.ConsumerSecret, auth.AccessToken, 
				                      auth.AccessTokenSecret, auth.UserId, auth.ScreenName);
			//var usrTl = tc.GetUsrTimeline ("nanjolno");
			//var homeTl = tc.GetHomeTimeline ();
			//var mentions = tc.GetMentionsTimeline ();
			//var idTweet = tc.GetTweet (674435882917584896);

			//debug
			Console.WriteLine ("debug end!");
			Console.ReadKey ();
			//=====================================================



			//debug
			Console.ReadKey ();
		}
	}






	//* TwitterAPIの仕様変更に伴いTweet内容の取得時にも署名の作成が必要となったため、
	//認証クラスのAuthクラスを継承してTwitterConnectorクラスを作成しています.
	//* 署名作成等に必要なメソッド、認証情報の格納等のプロパティ及びコンストラクタは基底クラスである
	//Authクラスのものを呼び出します.
	public class TwitterConnector : Auth
	{
		//コンストラクタ
		public TwitterConnector (
			string consumerKey, string consumerSecret, string accessToken, 
			string accessTokenSecret, string userId, string screenName)
			: base (consumerKey, consumerSecret, accessToken, accessTokenSecret, userId, screenName)
		{
		}
		
		//最近の自身へのメンションを取得するメソッド
		//返り値：取得したメンションのリスト
		public List<Tweet> GetMentionsTimeline ()
		{
			string oauthNonce = GenNonce ();
			string timeStamp = GenTimestamp ();

			//署名作成=============================================================================================
			//パラメータ==================
			SortedDictionary<string, string> parameters = new SortedDictionary<string, string> ();
			parameters.Add ("oauth_consumer_key", ConsumerKey);
			parameters.Add ("oauth_signature_method", "HMAC-SHA1");
			parameters.Add ("oauth_timestamp", timeStamp);
			parameters.Add ("oauth_nonce", oauthNonce);
			parameters.Add ("oauth_version", "1.0");
			parameters.Add ("oauth_token", AccessToken);
			parameters.Add ("include_rts", "1");
			//==========================

			string signature = GenSignature ("GET", "https://api.twitter.com/1.1/statuses/mentions_timeline.json", parameters, ConsumerSecret, AccessTokenSecret);

			//===================================================================================================

			//ヘッダ作成===========================================================================================
			string authHeader = string.Format (
				                    "OAuth oauth_consumer_key=\"{0}\", " +
				                    "oauth_nonce=\"{1}\", " +
				                    "oauth_signature=\"{2}\", " +
				                    "oauth_signature_method=\"{3}\", " +
				                    "oauth_timestamp=\"{4}\", " +
				                    "oauth_token=\"{5}\", " +
				                    "oauth_version=\"{6}\""
				//APIKeyなども形式的に念のため全てURLエンコードする
				, Uri.EscapeDataString (ConsumerKey)
				, Uri.EscapeDataString (oauthNonce)
				, Uri.EscapeDataString (signature)
				, Uri.EscapeDataString ("HMAC-SHA1")
				, Uri.EscapeDataString (timeStamp)
				, Uri.EscapeDataString (AccessToken)
				, Uri.EscapeDataString ("1.0"));
			//===================================================================================================


			//get送信=======================================================
			//パラメータinclude_rts=1は推奨値
			string reqUrl = "https://api.twitter.com/1.1/statuses/mentions_timeline.json?&include_rts=1";
			ServicePointManager.Expect100Continue = false;
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create (reqUrl) as HttpWebRequest;
			req.Method = "GET";
			req.ContentType = "application/x-www-form-urlencoded";
			req.Host = "api.twitter.com";
			req.Headers.Add ("Authorization", authHeader);

			HttpWebResponse res = (HttpWebResponse)req.GetResponse ();
			Stream resStream = res.GetResponseStream ();
			StreamReader sr = new StreamReader (resStream);
			//JSONデータを取得
			string resultJson = sr.ReadToEnd ();
			resStream.Close ();
			sr.Close ();
			//JSONデータのパース
			var root = JsonConvert.DeserializeObject<List<RootObject>> (resultJson);
			List<Tweet> resultList = new List<Tweet> ();
			foreach (RootObject r in root) {

				//debug
				Console.WriteLine ("");
				Console.WriteLine (r.id);
				Console.WriteLine (r.text);
				Console.WriteLine (r.user.screen_name);
				Console.WriteLine (r.user.name);
				Console.WriteLine ("");

				User usr = new User (r.user.name, r.user.screen_name);
				Tweet tweet = new Tweet ((long)r.id, r.text, usr);
				resultList.Add (tweet);
			}	
			return resultList;
			//=============================================================
		}
		//===================================================================================================

		//最近の自身のホームタイムラインを取得するメソッド
		//返り値：取得したツイートのリスト
		public List<Tweet> GetHomeTimeline ()
		{
			string oauthNonce = GenNonce ();
			string timeStamp = GenTimestamp ();

			//署名作成=============================================================================================
			//パラメータ==================
			SortedDictionary<string, string> parameters = new SortedDictionary<string, string> ();
			parameters.Add ("oauth_consumer_key", ConsumerKey);
			parameters.Add ("oauth_signature_method", "HMAC-SHA1");
			parameters.Add ("oauth_timestamp", timeStamp);
			parameters.Add ("oauth_nonce", oauthNonce);
			parameters.Add ("oauth_version", "1.0");
			parameters.Add ("oauth_token", AccessToken);
			//==========================

			string signature = GenSignature ("GET", "https://api.twitter.com/1.1/statuses/home_timeline.json", parameters, ConsumerSecret, AccessTokenSecret);

			//===================================================================================================

			//ヘッダ作成===========================================================================================
			string authHeader = string.Format (
				                    "OAuth oauth_consumer_key=\"{0}\", " +
				                    "oauth_nonce=\"{1}\", " +
				                    "oauth_signature=\"{2}\", " +
				                    "oauth_signature_method=\"{3}\", " +
				                    "oauth_timestamp=\"{4}\", " +
				                    "oauth_token=\"{5}\", " +
				                    "oauth_version=\"{6}\""
				//APIKeyなども形式的に念のため全てURLエンコードする
				, Uri.EscapeDataString (ConsumerKey)
				, Uri.EscapeDataString (oauthNonce)
				, Uri.EscapeDataString (signature)
				, Uri.EscapeDataString ("HMAC-SHA1")
				, Uri.EscapeDataString (timeStamp)
				, Uri.EscapeDataString (AccessToken)
				, Uri.EscapeDataString ("1.0"));
			//===================================================================================================


			//get送信=======================================================
			string reqUrl = "https://api.twitter.com/1.1/statuses/home_timeline.json";
			ServicePointManager.Expect100Continue = false;
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create (reqUrl) as HttpWebRequest;
			req.Method = "GET";
			req.ContentType = "application/x-www-form-urlencoded";
			req.Host = "api.twitter.com";
			req.Headers.Add ("Authorization", authHeader);

			HttpWebResponse res = (HttpWebResponse)req.GetResponse ();
			Stream resStream = res.GetResponseStream ();
			StreamReader sr = new StreamReader (resStream);
			//JSONデータを取得
			string resultJson = sr.ReadToEnd ();
			resStream.Close ();
			sr.Close ();
			//JSONデータのパース
			var root = JsonConvert.DeserializeObject<List<RootObject>> (resultJson);
			List<Tweet> resultList = new List<Tweet> ();
			foreach (RootObject r in root) {

				//debug
				Console.WriteLine ("");
				Console.WriteLine (r.id);
				Console.WriteLine (r.text);
				Console.WriteLine (r.user.screen_name);
				Console.WriteLine (r.user.name);
				Console.WriteLine ("");

				User usr = new User (r.user.name, r.user.screen_name);
				Tweet tweet = new Tweet ((long)r.id, r.text, usr);
				resultList.Add (tweet);
			}	
			return resultList;
			//=============================================================
		}
		//===================================================================================================

		//指定したユーザのスクリーンネームのツイートを取得するメソッド
		//返り値：取得したツイートのリスト
		public List<Tweet> GetUsrTimeline (string screenName)
		{
			string oauthNonce = GenNonce ();
			string timeStamp = GenTimestamp ();

			//署名作成=============================================================================================
			//パラメータ==================
			SortedDictionary<string, string> parameters = new SortedDictionary<string, string> ();
			parameters.Add ("oauth_consumer_key", ConsumerKey);
			parameters.Add ("oauth_signature_method", "HMAC-SHA1");
			parameters.Add ("oauth_timestamp", timeStamp);
			parameters.Add ("oauth_nonce", oauthNonce);
			parameters.Add ("oauth_version", "1.0");
			parameters.Add ("oauth_token", AccessToken);
			parameters.Add ("screen_name", screenName);
			//==========================

			//==========================
			string signature = GenSignature ("GET", "https://api.twitter.com/1.1/statuses/user_timeline.json", parameters, ConsumerSecret, AccessTokenSecret);

//			//<参考> 署名作成の中身======
//			//署名データ
//			string signatureData =
//				"GET" + "&" +
//				Uri.EscapeDataString ("https://api.twitter.com/1.1/statuses/user_timeline.json") + "&" +
//				Uri.EscapeDataString (JoinParameters (parameters));
//			//署名キー
//			string signatureKey = Uri.EscapeDataString (ConsumerSecret) + "&" + Uri.EscapeDataString (AccessTokenSecret);
//			//ハッシュ関数生成
//			HMACSHA1 hMACSHA1 = new HMACSHA1 (Encoding.UTF8.GetBytes (signatureKey));
//			//暗号化
//			byte[] bArray = hMACSHA1.ComputeHash (Encoding.UTF8.GetBytes (signatureData));
//			//ベース64エンコード
//			string signature = Convert.ToBase64String (bArray);
//			//========================
			//==========================

			//===================================================================================================

			//ヘッダ作成===========================================================================================
			string authHeader = string.Format (
				                    "OAuth oauth_consumer_key=\"{0}\", " +
				                    "oauth_nonce=\"{1}\", " +
				                    "oauth_signature=\"{2}\", " +
				                    "oauth_signature_method=\"{3}\", " +
				                    "oauth_timestamp=\"{4}\", " +
				                    "oauth_token=\"{5}\", " +
				                    "oauth_version=\"{6}\""
				//APIKeyなども形式的に念のため全てURLエンコードする
				, Uri.EscapeDataString (ConsumerKey)
				, Uri.EscapeDataString (oauthNonce)
				, Uri.EscapeDataString (signature)
				, Uri.EscapeDataString ("HMAC-SHA1")
				, Uri.EscapeDataString (timeStamp)
				, Uri.EscapeDataString (AccessToken)
				, Uri.EscapeDataString ("1.0"));
			//===================================================================================================


			//get送信=======================================================
			string reqUrl = "https://api.twitter.com/1.1/statuses/user_timeline.json?&screen_name=" + screenName;
			ServicePointManager.Expect100Continue = false;
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create (reqUrl) as HttpWebRequest;
			req.Method = "GET";
			req.ContentType = "application/x-www-form-urlencoded";
			req.Host = "api.twitter.com";
			req.Headers.Add ("Authorization", authHeader);

			HttpWebResponse res = (HttpWebResponse)req.GetResponse ();
			Stream resStream = res.GetResponseStream ();
			StreamReader sr = new StreamReader (resStream);
			//JSONデータを取得
			string resultJson = sr.ReadToEnd ();
			resStream.Close ();
			sr.Close ();
			//JSONデータのパース
			var root = JsonConvert.DeserializeObject<List<RootObject>> (resultJson);
			List<Tweet> resultList = new List<Tweet> ();
			foreach (RootObject r in root) {
				
				//debug
				Console.WriteLine ("");
				Console.WriteLine (r.id);
				Console.WriteLine (r.text);
				Console.WriteLine (r.user.screen_name);
				Console.WriteLine (r.user.name);
				Console.WriteLine ("");

				User usr = new User (r.user.name, r.user.screen_name);
				Tweet tweet = new Tweet ((long)r.id, r.text, usr);
				resultList.Add (tweet);
			}	
			return resultList;
			//=============================================================
		}
		//===================================================================================================


		//指定されたidのツイートを取得するメソッド
		//id：ツイートID
		//返り値：ツイート
		public Tweet GetTweet (long id)
		{
			string oauthNonce = GenNonce ();
			string timeStamp = GenTimestamp ();

			//署名作成=============================================================================================
			//パラメータ==================
			SortedDictionary<string, string> parameters = new SortedDictionary<string, string> ();
			parameters.Add ("oauth_consumer_key", ConsumerKey);
			parameters.Add ("oauth_signature_method", "HMAC-SHA1");
			parameters.Add ("oauth_timestamp", timeStamp);
			parameters.Add ("oauth_nonce", oauthNonce);
			parameters.Add ("oauth_version", "1.0");
			parameters.Add ("oauth_token", AccessToken);
			parameters.Add ("id", id.ToString ());
			//==========================

			string signature = GenSignature ("GET", "https://api.twitter.com/1.1/statuses/show.json", parameters, ConsumerSecret, AccessTokenSecret);

			//===================================================================================================

			//ヘッダ作成===========================================================================================
			string authHeader = string.Format (
				                    "OAuth oauth_consumer_key=\"{0}\", " +
				                    "oauth_nonce=\"{1}\", " +
				                    "oauth_signature=\"{2}\", " +
				                    "oauth_signature_method=\"{3}\", " +
				                    "oauth_timestamp=\"{4}\", " +
				                    "oauth_token=\"{5}\", " +
				                    "oauth_version=\"{6}\""
				//APIKeyなども形式的に念のため全てURLエンコードする
				, Uri.EscapeDataString (ConsumerKey)
				, Uri.EscapeDataString (oauthNonce)
				, Uri.EscapeDataString (signature)
				, Uri.EscapeDataString ("HMAC-SHA1")
				, Uri.EscapeDataString (timeStamp)
				, Uri.EscapeDataString (AccessToken)
				, Uri.EscapeDataString ("1.0"));
			//===================================================================================================


			//get送信=======================================================
			//パラメータinclude_rts=1は推奨値
			string reqUrl = "https://api.twitter.com/1.1/statuses/show.json?&id=" + id.ToString ();
			ServicePointManager.Expect100Continue = false;
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create (reqUrl) as HttpWebRequest;
			req.Method = "GET";
			req.ContentType = "application/x-www-form-urlencoded";
			req.Host = "api.twitter.com";
			req.Headers.Add ("Authorization", authHeader);

			HttpWebResponse res = (HttpWebResponse)req.GetResponse ();
			Stream resStream = res.GetResponseStream ();
			StreamReader sr = new StreamReader (resStream);
			//JSONデータを取得
			string resultJson = sr.ReadToEnd ();
			resStream.Close ();
			sr.Close ();
			//JSONデータのパース
			var root = JsonConvert.DeserializeObject<RootObject> (resultJson);
			User usr = new User (root.user.name, root.user.screen_name);
			Tweet resultTweet = new Tweet ((long)root.id, root.text, usr);

			//debug
			Console.WriteLine ("");
			Console.WriteLine (root.id);
			Console.WriteLine (root.text);
			Console.WriteLine (root.user.screen_name);
			Console.WriteLine (root.user.name);
			Console.WriteLine ("");

			return resultTweet;
			//=============================================================
		}
		//===================================================================================================
				
		//
		//		//Twitterにpostするメソッド
		//		//str：postする文字列
		//		public void Update (string str)
		//		{
		//
		//		}
		//	}
		//

		public class Tweet
		{
			//コンストラクタ
			//id：ツイートID
			//text：ツイート内容
			//user：ツイートユーザ
			public Tweet (long id, string text, User user)
			{
				Id = id;
				Text = text;
				User = user;
		
		
			}
		
			//プロパティ
			public long Id { get; protected set; }

			public string Text{ get; protected set; }

			public User User{ get; protected set; }
		
		}

		public class User
		{
			//コンストラクタ
			//name：ユーザ名
			//screenName：ユーザのスクリーン名
			public User (string name, string screenName)
			{
				Name = name;
				ScreenName = screenName;
			}
		
			//プロパティ
			public string Name{ get; private set; }

			public string ScreenName{ get; private set; }
		
		}

	}
}