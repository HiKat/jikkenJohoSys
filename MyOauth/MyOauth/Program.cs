using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

//using System.Collections.Specialized;
using System.IO;

//using System.Collections;
using System.Security.Cryptography;
using System.Xml;

//appID = dj0zaiZpPWk3TUVNOTdKUmxNTSZzPWNvbnN1bWVyc2VjcmV0Jng9OGQ-

namespace MyOauth
{
	class Program
	{
		public static void Main (string[] args)
		{

			#region 無駄な抵抗
//			//リクエストトークン取得
//			//URL
//			//https://api.twitter.com/oauth/request_token
//			string url = "https://api.twitter.com/oauth/request_token";
//			Uri uri = new Uri (url);
//			//必須パラメーター
//			//oauth_callback
//			#region #region MyRegion
//			//oauth_signature_method
//			//oauth_signature
//			//oauth_timestamp
//			//oauth_nonce
//			//oauth_version
//
//			//Consumer Key (API Key)        
//			string ApiKey = "92PJFWHX7tj8KSPYF6SxtM2JV";
//			//Access Token Secret
//			string AccessTokenSecret = "XhtPKN1pZ9zGcrj7A5x7OiLtFGCDqjX1Y7utTSxxYAMiC";
//			string OauthSignature = Uri.EscapeUriString(ApiKey) + "&" + Uri.EscapeUriString(AccessTokenSecret);
//
//
//			//callbackUrl
//			string callbackUrl = "http://www.example.com";
//			//consumerKey(ApiSecret)
//			string consumerKey = "TDJ8Iqwskdai0zgP2gWnhORawAM5Q1JS178PfZCxiZKqLvkzLS";
//			//signatureMethod
//			string signatureMethod = "HMAC-SHA1";
//			//timestamp
//			private static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
//			string timeStamp = FromDateTime( DateTime.UtcNow );
			#endregion

			Oauth ot = new Oauth ();
			ot.RequestToken ();

			Console.WriteLine ("debug");
			Console.ReadLine ();
		}
	}

	class Oauth
	{

		private string EncodeCharacters (string data)
		{
			//as per OAuth Core 1.0 Characters in the unreserved character set MUST NOT be encoded
			//unreserved = ALPHA, DIGIT, '-', '.', '_', '~'
			if (data.Contains ("!"))
				data = data.Replace ("!", "%21");
			if (data.Contains ("'"))
				data = data.Replace ("'", "%27");
			if (data.Contains ("("))
				data = data.Replace ("(", "%28");
			if (data.Contains (")"))
				data = data.Replace (")", "%29");
			if (data.Contains ("*"))
				data = data.Replace ("*", "%2A");
			if (data.Contains (","))
				data = data.Replace (",", "%2C");

			return data;
		}


		public void RequestToken ()
		{
			string oauthcallback = "http://www.yahoo.co.jp/";
			string oauthconsumerkey = "92PJFWHX7tj8KSPYF6SxtM2JV";
			string oauthconsumersecret = "TDJ8Iqwskdai0zgP2gWnhORawAM5Q1JS178PfZCxiZKqLvkzLS";

			string oauthtokensecret = string.Empty;
			string oauthtoken = string.Empty;
			string oauthsignaturemethod = "HMAC-SHA1";
			string oauthversion = "1.0";
			string oauthnonce = Convert.ToBase64String (new ASCIIEncoding ().GetBytes (DateTime.Now.Ticks.ToString ()));
			TimeSpan timeSpan = DateTime.UtcNow - new DateTime (1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			string oauthtimestamp = Convert.ToInt64 (timeSpan.TotalSeconds).ToString ();
			string url = "https://api.twitter.com/oauth/request_token?oauth_callback=" + oauthcallback;
			SortedDictionary<string, string> basestringParameters = new SortedDictionary<string, string> ();
			basestringParameters.Add ("oauth_version", oauthversion);
			basestringParameters.Add ("oauth_consumer_key", oauthconsumerkey);
			basestringParameters.Add ("oauth_nonce", oauthnonce);
			basestringParameters.Add ("oauth_signature_method", oauthsignaturemethod);
			basestringParameters.Add ("oauth_timestamp", oauthtimestamp);
			basestringParameters.Add ("oauth_callback", Uri.EscapeDataString (oauthcallback));

			//Build the signature string
			StringBuilder baseString = new StringBuilder ();
			baseString.Append ("POST" + "&");
			baseString.Append (EncodeCharacters (Uri.EscapeDataString (url.Split ('?') [0]) + "&"));
			foreach (KeyValuePair<string, string> entry in basestringParameters) {
				baseString.Append (EncodeCharacters (Uri.EscapeDataString (entry.Key + "=" + entry.Value + "&")));
			}

			//Remove the trailing ambersand char last 3 chars - %26
			string finalBaseString = baseString.ToString ().Substring (0, baseString.Length - 3);

			//Build the signing key
			string signingKey = EncodeCharacters (Uri.EscapeDataString (oauthconsumersecret)) + "&" +
			                    EncodeCharacters (Uri.EscapeDataString (oauthtokensecret));

			//Sign the request
			HMACSHA1 hasher = new HMACSHA1 (new ASCIIEncoding ().GetBytes (signingKey));
			string oauthsignature = Convert.ToBase64String (
				                        hasher.ComputeHash (new ASCIIEncoding ().GetBytes (finalBaseString)));

			//Tell Twitter we don't do the 100 continue thing
			ServicePointManager.Expect100Continue = false;
			HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create (@url);

			StringBuilder authorizationHeaderParams = new StringBuilder ();
			authorizationHeaderParams.Append ("OAuth ");
			authorizationHeaderParams.Append ("oauth_nonce=" + "\"" + Uri.EscapeDataString (oauthnonce) + "\",");
			authorizationHeaderParams.Append ("oauth_signature_method=" + "\"" + Uri.EscapeDataString (oauthsignaturemethod) + "\",");
			authorizationHeaderParams.Append ("oauth_timestamp=" + "\"" + Uri.EscapeDataString (oauthtimestamp) + "\",");
			authorizationHeaderParams.Append ("oauth_consumer_key=" + "\"" + Uri.EscapeDataString (oauthconsumerkey) + "\",");
			authorizationHeaderParams.Append ("oauth_signature=" + "\"" + Uri.EscapeDataString (oauthsignature) + "\",");
			authorizationHeaderParams.Append ("oauth_version=" + "\"" + Uri.EscapeDataString (oauthversion) + "\"");
			webRequest.Headers.Add ("Authorization", authorizationHeaderParams.ToString ());

			webRequest.Method = "POST";
			webRequest.ContentType = "application/x-www-form-urlencoded";

			//Allow us a reasonable timeout in case Twitter's busy
			webRequest.Timeout = 3 * 60 * 1000;

			try {
				webRequest.Proxy = new WebProxy ("enter proxy details/address");
				HttpWebResponse webResponse = webRequest.GetResponse () as HttpWebResponse;
				Stream dataStream = webResponse.GetResponseStream ();
				// Open the stream using a StreamReader for easy access.
				StreamReader reader = new StreamReader (dataStream);
				// Read the content.
				string responseFromServer = reader.ReadToEnd ();

				//debug
				Console.WriteLine ("response is =\n {0}", responseFromServer);

			} catch (Exception ex) {

				Console.WriteLine ("error {0}", ex);
			}
		}
	}
}
