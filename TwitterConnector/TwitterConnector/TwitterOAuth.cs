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
//* 署名作成時のパラメータはキーがアルファベット順に並んでいる必要があります
//* callbackURLを指定している場合はpinコード画面が出ずレスポンスからpinを入手する必要があります.
//* URLエンコードについてはUri.EscapeDataStringを使用
//（HttpUtility.UrlEncodeメソッドでは%20が+に変換されるなど）
//* レスポンスの仕様変更に注意


namespace TwitterOAuth
{
	public class Auth
	{
		//タイムスタンプ生成====================================
		//UNIXエポック時刻
		private readonly static DateTime dtUnixEpoch = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		protected string GenTimestamp ()
		{
			//現在時刻からUNIXエポック時刻を引く
			TimeSpan ts = DateTime.UtcNow - dtUnixEpoch;
			return Convert.ToInt64 (ts.TotalSeconds).ToString ();
		}
		//===================================================

		//ランダム文字列生成====================================
		protected string GenNonce ()
		{
			string result = Convert.ToBase64String (new UTF8Encoding ().GetBytes (DateTime.Now.Ticks.ToString ()));
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

		//署名作成==================================================================================================
		protected string GenSignature (
			string method, string url, SortedDictionary<string, string> parameters, string conSec, string token)
		{
			//署名データ
			string signatureData = 
				method + "&" +
				Uri.EscapeDataString (url) + "&" +
				Uri.EscapeDataString (JoinParameters (parameters));	
			//署名キー
			string signatureKey = Uri.EscapeDataString (conSec) + "&" + Uri.EscapeDataString (token);
			//ハッシュ関数生成
			HMACSHA1 hMACSHA1 = new HMACSHA1 (Encoding.UTF8.GetBytes (signatureKey));
			//暗号化
			byte[] bArray = hMACSHA1.ComputeHash (Encoding.UTF8.GetBytes (signatureData));
			//ベース64エンコード
			string signature = Convert.ToBase64String (bArray);
			return signature;
		}
		//=========================================================================================================

		//Authヘッダー作成===========================================================================================
		protected string GenAuthHeader(string consumerKey, string oauthNonce, string signature, string timeStamp, string accessToken){
			string authHeader = string.Format (
				"OAuth oauth_consumer_key=\"{0}\", " +
				"oauth_nonce=\"{1}\", " +
				"oauth_signature=\"{2}\", " +
				"oauth_signature_method=\"{3}\", " +
				"oauth_timestamp=\"{4}\", " +
				"oauth_token=\"{5}\", " +
				"oauth_version=\"{6}\""
				//APIKeyなども形式的に念のため全てURLエンコードする
				, Uri.EscapeDataString (consumerKey)
				, Uri.EscapeDataString (oauthNonce)
				, Uri.EscapeDataString (signature)
				, Uri.EscapeDataString ("HMAC-SHA1")
				, Uri.EscapeDataString (timeStamp)
				, Uri.EscapeDataString (accessToken)
				, Uri.EscapeDataString ("1.0"));
			return authHeader;
		}
		//=========================================================================================================

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
			string timeStamp = GenTimestamp ();

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
			string signature = GenSignature ("GET", "https://api.twitter.com/oauth/request_token", parameters, ConsumerSecret, "");

//			//<参考> 署名作成の中身======
//			//署名データ
//			string signatureData = 
//				"GET" + "&" +
//				Uri.EscapeDataString ("https://api.twitter.com/oauth/request_token") + "&" +
//				Uri.EscapeDataString (JoinParameters (parameters));	
//			//署名キー
//			string signatureKey = Uri.EscapeDataString (ConsumerSecret) + "&";
//			//ハッシュ関数生成
//			HMACSHA1 hMACSHA1 = new HMACSHA1 (Encoding.UTF8.GetBytes (signatureKey));
//			//暗号化
//			byte[] bArray = hMACSHA1.ComputeHash (Encoding.UTF8.GetBytes (signatureData));
//			//ベース64エンコード
//			string signature = Convert.ToBase64String (bArray);
//			//=========================
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
			//Console.WriteLine ("get request token response = " + response);

			//レスポンス例（こうなっていないとずれる可能性がある）
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
			string timeStamp = GenTimestamp ();

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
			string signature = GenSignature ("GET", "https://api.twitter.com/oauth/access_token", parameters, ConsumerSecret, RequestTokenSecret);

//			//<参考> 署名作成の中身======
//			//署名データ
//			string signatureData = 
//				"GET" + "&" +
//				Uri.EscapeDataString ("https://api.twitter.com/oauth/access_token") + "&" +
//				Uri.EscapeDataString (JoinParameters (parameters));	
//			//署名キー
//			string signatureKey = Uri.EscapeDataString (ConsumerSecret) + "&" + Uri.EscapeDataString (RequestTokenSecret);
//			//ハッシュ関数生成
//			HMACSHA1 hMACSHA1 = new HMACSHA1 (Encoding.UTF8.GetBytes (signatureKey));
//			//暗号化
//			byte[] bArray = hMACSHA1.ComputeHash (Encoding.UTF8.GetBytes (signatureData));
//			//ベース64エンコード
//			string signature = Convert.ToBase64String (bArray);
//			//========================
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
			//Console.WriteLine ("get access token response = " + response);

			//レスポンス例（こうなっていないとずれる可能性がある）
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
	}
}
