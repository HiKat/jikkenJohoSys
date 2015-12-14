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
using TwitterJson;

namespace TwitterGetContent
{
	
	class Program
	{
		static void Main (string[] args)
		{
			//リクエストトークン取得====================================================================
			//ApiTestNoCallback
			//callbackurl設定なしアカウント
//		const string CONSUMER_KEY = "8369eInu3C2YzkAgx3YrWbXMJ";
//		const string CONSUMER_SECRET = "8uWxX1iTTR1yxdA25RqlQxxn82tDipnsC5482UU3Etm3EpwQGn";
//
//		Auth auth = new Auth (CONSUMER_KEY, CONSUMER_SECRET);
//
//		// リクエストトークンを取得する
//		auth.GetRequestToken ();
//		Console.WriteLine ("RequestToken: " + auth.RequestToken);
//		Console.WriteLine ("RequestTokenSecret: " + auth.RequestTokenSecret);
//
//		//ユーザーにRequestTokenを認証してもらう
//		Console.WriteLine ("Authentication URL：");
//		Console.WriteLine ("https://api.twitter.com/oauth/authorize?oauth_token=" + auth.RequestToken);
//		Console.Write ("PIN：");
//		string pin = Console.ReadLine ().Trim ();
//		// アクセストークンを取得する
//		auth.GetAccessToken (pin);
//
//		// 結果を表示する
//		Console.WriteLine ("AccessToken: " + auth.AccessToken);
//		Console.WriteLine ("AccessTokenSecret: " + auth.AccessTokenSecret);
//		Console.WriteLine ("UserId: " + auth.UserId);
//		Console.WriteLine ("ScreenName: " + auth.ScreenName);
			//=====================================================================================


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
			string myTL = tc.GetTimeLine ();
			//List<Tweet> myTL = tc.GetUserTimeLine();

			//debug
			Console.WriteLine ("debug end!");
			Console.ReadKey ();
			//=====================================================



			//debug
			Console.ReadKey ();
		}
	}

	public class TwitterConnector : Auth
	{
		//基本クラスのprotectedメソッドが使用可
		//プロパティのScreenNameは継承

		//コンストラクタ
		public TwitterConnector (
			string consumerKey, string consumerSecret, string accessToken, 
			string accessTokenSecret, string userId, string screenName)
			: base (consumerKey, consumerSecret, accessToken, accessTokenSecret, userId, screenName){}
		
		//		//最近の自身へのメンションを取得するメソッド
		//		//返り値：取得したメンションのリスト
		//		public List<Tweet> GetMentionsTimeline ()
		//		{
		//
		//		}
		//
		//		//最近の自身のタイムラインを取得するメソッド
		//		//返り値：取得したツイートのリスト
		//		public List<Tweet> GetUserTimeline ()
		//		{
		//
		//		}
		//
		//		//最近の自身のホームタイムラインを取得するメソッド
		//		//返り値：取得したツイートのリスト
		//		public List<Tweet> GetHomeTimeline ()
		//		{}
		//

		//タイムライン取得======================================================================================
		public string GetTimeLine ()
		{
			//ランダム文字列生成
			string oauthNonce = GenNonce ();
			//タイムスタンプ生成
			//なぜか署名作成時の時刻とヘッダ作成時の時刻が異なっていても受け付けるが、
			//ランダム文字列は異なっていると401エラーを吐く模様
			string timeStamp = GenerateTimestamp ();


			//署名作成=============================================================================================
			//パラメータ==================
			SortedDictionary<string, string> parameters = new SortedDictionary<string, string> ();
			parameters.Add ("oauth_consumer_key", ConsumerKey);
			parameters.Add ("oauth_signature_method", "HMAC-SHA1");
			parameters.Add ("oauth_timestamp", timeStamp);
			parameters.Add ("oauth_nonce", oauthNonce);
			parameters.Add ("oauth_version", "1.0");
			parameters.Add ("oauth_token", AccessToken);
			parameters.Add ("screen_name", ScreenName);
			//==========================

			//==========================
			//署名データ
			string signatureData =
				"GET" + "&" +
				Uri.EscapeDataString ("https://api.twitter.com/1.1/statuses/user_timeline.json") + "&" +
				Uri.EscapeDataString (JoinParameters (parameters));
			//署名キー
			string signatureKey = Uri.EscapeDataString (ConsumerSecret) + "&" + Uri.EscapeDataString (AccessTokenSecret);
			//ハッシュ関数生成
			HMACSHA1 hMACSHA1 = new HMACSHA1 (Encoding.UTF8.GetBytes (signatureKey));
			//暗号化
			byte[] bArray = hMACSHA1.ComputeHash (Encoding.UTF8.GetBytes (signatureData));
			//ベース64エンコード
			string signature = Convert.ToBase64String (bArray);
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
			string reqUrl = "https://api.twitter.com/1.1/statuses/user_timeline.json?&screen_name=" + ScreenName;
			ServicePointManager.Expect100Continue = false;
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create (reqUrl) as HttpWebRequest;
			req.Method = "GET";
			req.ContentType = "application/x-www-form-urlencoded";
			req.Host = "api.twitter.com";
			req.Headers.Add ("Authorization", authHeader);

			//Encoding enc = Encoding.UTF8;
			HttpWebResponse res = (HttpWebResponse)req.GetResponse ();


			Stream resStream = res.GetResponseStream ();
			//StreamReader sr = new StreamReader (resStream, enc);
			StreamReader sr = new StreamReader (resStream);
			//JSONデータを取得
			string resultJson = sr.ReadToEnd ();
			resStream.Close ();
			sr.Close ();

			//debug
			Console.WriteLine (resultJson);

			var root = JsonConvert.DeserializeObject<RootObject>(resultJson);
			Console.WriteLine (root.text);





			//			WebRequest req = WebRequest.Create ("https://api.twitter.com/oauth/access_token?" + JoinParameters (parameters));
			//			WebResponse res = req.GetResponse ();
			//			Stream stream = res.GetResponseStream ();
			//			StreamReader reader = new StreamReader (stream);
			//			string response = reader.ReadToEnd ();
			//			reader.Close ();
			//			stream.Close ();

			return "end";
			//=============================================================
		}
		//===================================================================================================


		//		//指定されたidのツイートを取得するメソッド
		//		//id：ツイートID
		//		//返り値：ツイート
		//		public Tweet GetTweet (long id)
		//		{
		//
		//		}
		//
		//		//Twitterにpostするメソッド
		//		//str：postする文字列
		//		public void Update (string str)
		//		{
		//
		//		}
		//	}
		//
		//
		class Tweet
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

		class User
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