using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Text;
using System.Security;
using System.Security.Cryptography;


namespace MyOauthIkaros
{
	class MainClass
	{
		
		public static void Main (string[] args)
		{
			string oauthCallback = "http://www.yahoo.co.jp";
			string apiKey = "92PJFWHX7tj8KSPYF6SxtM2JV";//consumer keyと同一
			string accessTokenSec = "XhtPKN1pZ9zGcrj7A5x7OiLtFGCDqjX1Y7utTSxxYAMiC";
			string requestUrl = "https://api.twitter.com/oauth/request_token";



			Console.WriteLine ("Hello World!");

			//署名作成用キー作成
			string key = WebUtility.UrlEncode(apiKey) + "&" + WebUtility.UrlEncode(accessTokenSec);


			//署名作成用データ作成
			var parms = new Dictionary<string,string> {
				//アルファベット順に並べる
				{ "oauth_callback", oauthCallback },
				{ "oauth_consumer_key", apiKey },
				{ "oauth_nonce",GenerateNonce () },
				{ "oauth_signature_method","HMAC-SHA1" },
				{ "oauth_timestamp",GenerateTimeStamp () },
				{ "oauth_version","1.0" }
			};

			string data 
			= 
				WebUtility.UrlEncode ("POST") +
				"&" +
				WebUtility.UrlEncode (requestUrl) +
				"&" +
				WebUtility.UrlEncode (
					"oauth_callback" + "=" + (parms["oauth_callback"]) + 
					"&" + "oauth_consumer_key" + "=" + (WebUtility.UrlEncode(parms["oauth_consumer_key"])) +
					"&" + "oauth_nonce" + "=" + (WebUtility.UrlEncode(parms["oauth_nonce"])) +
					"&" + "oauth_signature_method" + "=" + (WebUtility.UrlEncode(parms["oauth_signature_method"])) + 
					"&" + "oauth_timestamp" + "=" + (WebUtility.UrlEncode(parms["oauth_timestamp"])) +
					"&" + "oauth_version" + "=" + (WebUtility.UrlEncode(parms["oauth_version"]))
				);

			//debug
			Console.WriteLine ("signature data = {0}", data);

			//キーとデータから署名作成
			//キーとデータからHMAC-SHA1方式のハッシュ値を求め、その結果をbase64エンコードする.

			//キーからハッシュ関数作成
			HMACSHA1 hMACSHA1 = new HMACSHA1(System.Text.Encoding.UTF8.GetBytes(key));
			byte[] bArray = hMACSHA1.ComputeHash (Encoding.UTF8.GetBytes (data));
			//作成した署名
			string signature = Convert.ToBase64String(bArray);


			Console.WriteLine ("signature = " + signature);


			//送信（GET）



			//APIURL
			string RequestURL = "https://api.twitter.com/oauth/request_token?";
			//変数を初期化（OAuthという文字列は後で関数で勝手に入れてくれるので不要）
			string AuthorizationParms = null;

			foreach(string name in parms.Keys)
			{
				RequestURL += name + "=" + WebUtility.UrlEncode(parms[name]) + "&";
				//"(ダブルクオーテーション)は「\"」と書いて表現
				AuthorizationParms += name + "=\"" + parms[name] + "\", ";
			}
			//最後に余計な記号(&だの,だの)たちを消去
			RequestURL = RequestURL.Remove(RequestURL.Length - 1);
			AuthorizationParms = AuthorizationParms.Remove(AuthorizationParms.Length - 2,2);

			var request = new HttpRequestMessage();
			//var response = new HttpResponseMessage();
			var client = new HttpClient();

			//POSTリクエストに設定
			request.Method = HttpMethod.Post;
			//URLを指定
			request.RequestUri = new Uri(RequestURL);
			//Authorizationヘッダーを設定、ここで"OAuth"という文字列も追加される
			request.Headers.Authorization = new AuthenticationHeaderValue("OAuth", AuthorizationParms);
			//中身はなんもないよと指定
			request.Content = null;

			//結果を受け取る変数を初期化
			string result = null;

			//Twitterにリクエストを送信、結果をresponseに格納する
			//response = client.SendAsync(request);
			//結果をstring形式で変数resultに保存
			//result = response.Content.ReadAsStringAsync();
			int response = client.SendAsync(request);
			result = response.ReadAsStringAsync();
			return result;


			Console.WriteLine("response = " + result);
//





//			//形態素解析
//			Uri uri = new Uri (requestUrl);
//			System.Net.WebClient wc = new System.Net.WebClient();
//			//文字コードを指定する	
//			wc.Encoding = Encoding.GetEncoding("utf-8");
//			//ヘッダにContent-Typeを加える
//			wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
//			string resText = wc.UploadString(uri, "");
//			//受信したデータを表示するとき
//			//Console.WriteLine(resText);





			//debug
			Console.ReadKey();
		}

		//タイムスタンプ
		public static string GenerateTimeStamp()
		{
			TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return Convert.ToInt64(ts.TotalSeconds).ToString();
		}

		//Nonce作成
		public static string GenerateNonce()
		{
			Random random = new Random();
			string strings = "0123456789abcdefghijklmnopqrstuvwxyABCDEFGHIJKLMNOPQRSTUVWXYZ";
			string res = "";
			for (int i = 0; i < 32; i++)
				res += strings[random.Next(0, strings.Length - 1)];
			return res;
		}



	}
}
