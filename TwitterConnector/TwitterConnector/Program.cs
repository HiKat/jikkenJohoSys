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
using OAuth;





//## 留意点
//* 署名作成時のパラメータはキーがアルファベット順に並んでいる必要がある
//* callbackURLを指定している場合はpinコード画面が出ずレスポンスからpinを入手する必要がある.
//* AccessToken及びAccessTokenSecretは、デベロッパは公開せずwebアプリケーションのユーザ
//に取得してもらう.
//* URLエンコードについてはUri.EscapeDataStringを使用
//（HttpUtility.UrlEncodeメソッドでは%20が+に変換されるなどの危険性）
//* レスポンスの仕様変更に注意


namespace OAuth
{
	public class Auth
	{
		//
		//
		//
		//タイムスタンプ生成====================================
		//UNIXエポック時刻
		private readonly static DateTime dtUnixEpoch = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		protected string GenerateTimestamp ()
		{
			//現在時刻からUNIXエポック時刻を引く
			TimeSpan ts = DateTime.UtcNow - dtUnixEpoch;
			return Convert.ToInt64 (ts.TotalSeconds).ToString ();
		}
		//===================================================

		//ランダム文字列生成====================================
		protected string GenNonce ()
		{
			string result = Convert.ToBase64String (new ASCIIEncoding ().GetBytes (DateTime.Now.Ticks.ToString ()));
			return result;
		}
		//===================================================

		//ディクショナリ型のパラメータを"&キー=値"のクエリに変換するメソッド===============
		//なお署名作成時のパラメータはアルファベット順である必要があるためSortedDictionary型
		//クエリ列はアルファベット順である必要はなし
		protected string JoinParameters (IDictionary<string, string> parameters)
		{
			StringBuilder result = new StringBuilder ();
			//該当パラメータが先頭かどうかを判断
			bool first = true;
			foreach (var parameter in parameters) {
				if (first) {
					//先頭のときはそのまま（&を入れない）
					first = false;
				} else {
					//先頭でないときは&をはさむ
					result.Append ('&');
				}
				//キーと値を=でつなぐ
				result.Append (parameter.Key);
				result.Append ('=');
				result.Append (parameter.Value);
			}
			return result.ToString ();
		}
		//========================================================================
		//
		//
		//


		//プロパティ===========================================
		public string ConsumerKey { get; protected set; }

		public string ConsumerSecret { get; protected set; }

		public string RequestToken { get; protected set; }

		public string RequestTokenSecret { get; protected set; }

		public string AccessToken { get; protected set; }

		public string AccessTokenSecret { get; protected set; }

		public string UserId { get; protected set; }

		public string ScreenName { get; protected set; }
		//===================================================


		//コンストラクタ================================================================
		public Auth (string consumerKey, string consumerSecret)
		{
			ServicePointManager.Expect100Continue = false;
			ConsumerKey = consumerKey;
			ConsumerSecret = consumerSecret;
		}

		public Auth (
			string consumerKey, string consumerSecret, string accessToken, 
			string accessTokenSecret, string userId, string screenName)
		{
			ServicePointManager.Expect100Continue = false;
			ConsumerKey = consumerKey;
			ConsumerSecret = consumerSecret;
			AccessToken = accessToken;
			AccessTokenSecret = accessTokenSecret;
			UserId = userId;
			ScreenName = screenName;
		}
		//=============================================================================

		//リクエストトークン取得=====================================================================
		public void GetRequestToken ()
		{
			//ランダム文字列生成
			string oauthNonce = GenNonce ();
			//タイムスタンプ生成
			string timeStamp = GenerateTimestamp ();

			//署名作成=============================================================================================

			//パラメータ==================
			SortedDictionary<string, string> parameters = new SortedDictionary<string, string> ();
			parameters.Add ("oauth_consumer_key", ConsumerKey);
			parameters.Add ("oauth_signature_method", "HMAC-SHA1");
			parameters.Add ("oauth_timestamp", timeStamp);
			parameters.Add ("oauth_nonce", oauthNonce);
			parameters.Add ("oauth_version", "1.0");
			//==========================

			//==========================
			//署名データ
			string signatureData = 
				"GET" + "&" +
				Uri.EscapeDataString ("https://api.twitter.com/oauth/request_token") + "&" +
				Uri.EscapeDataString (JoinParameters (parameters));	
			//署名キー
			string signatureKey = Uri.EscapeDataString (ConsumerSecret) + "&";
			//ハッシュ関数生成
			HMACSHA1 hMACSHA1 = new HMACSHA1 (Encoding.UTF8.GetBytes (signatureKey));
			//暗号化
			byte[] bArray = hMACSHA1.ComputeHash (Encoding.UTF8.GetBytes (signatureData));
			//ベース64エンコード
			string signature = Convert.ToBase64String (bArray);
			//==========================

			//署名もパラメータに追加
			parameters.Add ("oauth_signature", Uri.EscapeDataString (signature));

			//get送信=======================================================
			WebRequest req = WebRequest.Create ("https://api.twitter.com/oauth/request_token?" + JoinParameters (parameters));
			WebResponse res = req.GetResponse ();
			Stream stream = res.GetResponseStream ();
			StreamReader reader = new StreamReader (stream);
			string response = reader.ReadToEnd ();
			reader.Close ();
			stream.Close ();
			//=============================================================

			//debug
			Console.WriteLine ("get request token response = " + response);
			//レスポンス例（こうなっていないとずれる可能性）
			//oauth_token=「リクエストトークン」&oauth_token_secret=「シークレット」&oauth_callback_confirmed=true
			//キーと値の組が3つ

			//レスポンスをパース================================================================
			// =と&で分割
			char[] delimiterChars = { '=', '&' };
			string[] ary = response.Split (delimiterChars, StringSplitOptions.RemoveEmptyEntries);
			Dictionary<string, string> result = new Dictionary<string, string> ();
			for (int i = 0; i < (3 - 1); i++) {
				//i*2番目がキーで対応する値はi*2+1番目
				result [ary [i * 2]] = ary [i * 2 + 1];
			}
			RequestToken = result ["oauth_token"];
			RequestTokenSecret = result ["oauth_token_secret"];
			//===============================================================================
			//===================================================================================================
		}
		//=======================================================================================


		//アクセストークン取得==================================================================================
		public void GetAccessToken (string pin)
		{
			//ランダム文字列生成
			string oauthNonce = GenNonce ();
			//タイムスタンプ生成
			string timeStamp = GenerateTimestamp ();

			//パラメータ==================
			SortedDictionary<string, string> parameters = new SortedDictionary<string, string> ();
			parameters.Add ("oauth_consumer_key", ConsumerKey);
			parameters.Add ("oauth_signature_method", "HMAC-SHA1");
			parameters.Add ("oauth_timestamp", timeStamp);
			parameters.Add ("oauth_nonce", oauthNonce);
			parameters.Add ("oauth_version", "1.0");
			parameters.Add ("oauth_token", RequestToken);
			parameters.Add ("oauth_verifier", pin);
			//==========================

			//==========================
			//署名データ
			string signatureData = 
				"GET" + "&" +
				Uri.EscapeDataString ("https://api.twitter.com/oauth/access_token") + "&" +
				Uri.EscapeDataString (JoinParameters (parameters));	
			//署名キー
			string signatureKey = Uri.EscapeDataString (ConsumerSecret) + "&" + Uri.EscapeDataString (RequestTokenSecret);
			//ハッシュ関数生成
			HMACSHA1 hMACSHA1 = new HMACSHA1 (Encoding.UTF8.GetBytes (signatureKey));
			//暗号化
			byte[] bArray = hMACSHA1.ComputeHash (Encoding.UTF8.GetBytes (signatureData));
			//ベース64エンコード
			string signature = Convert.ToBase64String (bArray);
			//==========================

			//署名もパラメータに追加
			parameters.Add ("oauth_signature", Uri.EscapeDataString (signature));

			//get送信=======================================================
			WebRequest req = WebRequest.Create ("https://api.twitter.com/oauth/access_token?" + JoinParameters (parameters));
			WebResponse res = req.GetResponse ();
			Stream stream = res.GetResponseStream ();
			StreamReader reader = new StreamReader (stream);
			string response = reader.ReadToEnd ();
			reader.Close ();
			stream.Close ();
			//=============================================================

			//debug
			Console.WriteLine ("get access token response = " + response);
			//レスポンス例（こうなっていないとずれる可能性）
			//oauth_token=「アクセストークン」&oauth_token_secret=「シークレット」&user_id=「ID」&screen_name=「Name」&x_auth_expires=0
			//キーと値の組が5つ

			//レスポンスをパース================================================================
			// =と&で分割
			char[] delimiterChars = { '=', '&' };
			string[] ary = response.Split (delimiterChars, StringSplitOptions.RemoveEmptyEntries);
			Dictionary<string, string> result = new Dictionary<string, string> ();
			for (int i = 0; i < (5 - 1); i++) {
				//i*2番目がキーで対応する値がi*2+1番目
				result [ary [i * 2]] = ary [i * 2 + 1];
			}
			//===============================================================================


			//レスポンスをパースしてアクセストークン、アクセストークンシークレットを取り出す
			//IDなどは任意
			AccessToken = result ["oauth_token"];
			AccessTokenSecret = result ["oauth_token_secret"];
			UserId = result ["user_id"];
			ScreenName = result ["screen_name"];
		}
		//===================================================================================================


		//		//タイムライン取得======================================================================================
		//		public string GetTimeLine ()
		//		{
		//			//ランダム文字列生成
		//			string oauthNonce = GenNonce ();
		//			//タイムスタンプ生成
		//			//なぜか署名作成時の時刻とヘッダ作成時の時刻が異なっていても受け付けるが、
		//			//ランダム文字列は異なっていると401エラーを吐く模様
		//			string timeStamp = GenerateTimestamp ();
		//
		//
		//			//署名作成=============================================================================================
		//			//パラメータ==================
		//			SortedDictionary<string, string> parameters = new SortedDictionary<string, string> ();
		//			parameters.Add ("oauth_consumer_key", ConsumerKey);
		//			parameters.Add ("oauth_signature_method", "HMAC-SHA1");
		//			parameters.Add ("oauth_timestamp", timeStamp);
		//			parameters.Add ("oauth_nonce", oauthNonce);
		//			parameters.Add ("oauth_version", "1.0");
		//			parameters.Add ("oauth_token", AccessToken);
		//			//parameters.Add ("screen_name", "apiTestJohoSys");
		//			parameters.Add ("screen_name", "wsvqncxko4");
		//			//==========================
		//
		//			//==========================
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
		//			//==========================
		//
		//			//===================================================================================================
		//
		//			//ヘッダ作成===========================================================================================
		//			string authHeader = string.Format (
		//				                    "OAuth oauth_consumer_key=\"{0}\", " +
		//				                    "oauth_nonce=\"{1}\", " +
		//				                    "oauth_signature=\"{2}\", " +
		//				                    "oauth_signature_method=\"{3}\", " +
		//				                    "oauth_timestamp=\"{4}\", " +
		//				                    "oauth_token=\"{5}\", " +
		//				                    "oauth_version=\"{6}\""
		//						//APIKeyなども形式的に念のため全てURLエンコードする
		//						, Uri.EscapeDataString (ConsumerKey)
		//						, Uri.EscapeDataString (oauthNonce)
		//						, Uri.EscapeDataString (signature)
		//						, Uri.EscapeDataString ("HMAC-SHA1")
		//						, Uri.EscapeDataString (timeStamp)
		//						, Uri.EscapeDataString (AccessToken)
		//						, Uri.EscapeDataString ("1.0"));
		//			//===================================================================================================
		//
		//
		//			//get送信=======================================================
		//			string reqUrl = "https://api.twitter.com/1.1/statuses/user_timeline.json?&screen_name=wsvqncxko4";
		//			ServicePointManager.Expect100Continue = false;
		//			HttpWebRequest req = (HttpWebRequest)WebRequest.Create (reqUrl) as HttpWebRequest;
		//			req.Method = "GET";
		//			req.ContentType = "application/x-www-form-urlencoded";
		//			req.Host = "api.twitter.com";
		//			req.Headers.Add ("Authorization", authHeader);
		//
		//			//Encoding enc = Encoding.UTF8;
		//			HttpWebResponse res = (HttpWebResponse)req.GetResponse ();
		//
		//
		//			Stream resStream = res.GetResponseStream ();
		//			//StreamReader sr = new StreamReader (resStream, enc);
		//			StreamReader sr = new StreamReader (resStream);
		//			//JSONデータを取得
		//			string resultJson = sr.ReadToEnd ();
		//			resStream.Close ();
		//			sr.Close ();
		//
		//			//debug
		//			Console.WriteLine (resultJson);
		//
		//
		//
		//
		//
		//
		//			//			WebRequest req = WebRequest.Create ("https://api.twitter.com/oauth/access_token?" + JoinParameters (parameters));
		//			//			WebResponse res = req.GetResponse ();
		//			//			Stream stream = res.GetResponseStream ();
		//			//			StreamReader reader = new StreamReader (stream);
		//			//			string response = reader.ReadToEnd ();
		//			//			reader.Close ();
		//			//			stream.Close ();
		//
		//			return "end";
		//			//=============================================================
		//		}
		//		//===================================================================================================
	}
}

namespace GetContent
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
		//プロパティ
		//ScreenNameは継承

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
			//parameters.Add ("screen_name", "apiTestJohoSys");
			parameters.Add ("screen_name", "wsvqncxko4");
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
			string reqUrl = "https://api.twitter.com/1.1/statuses/user_timeline.json?&screen_name=wsvqncxko4";
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