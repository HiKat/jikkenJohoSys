using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;





namespace OauthTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");
		}
	}

	//OAuthクラスはアカウントごとに生成されそれぞれの値を保存しておきます
	public class OAuth
	{
		//面倒くさいからという理由でプロパティを乱用したのは秘密
		public string ConsumerKey { get { return "hogehoge"; } }
		public string ConsumerSecret { get { return "hogehoge"; } }

		public string RequestKey { get; set; }
		public string RequestSecret { get; set; }

		public string AccessKey { get; set; }
		public string AccessSecret { get; set; }
		public string UserId { get; set; }
		public string ScreenName { get; set; }

		//付随情報
		public Image icon { get; set; }
		public string UserName { get; set; }

		public string PostUrl { get; set; }
		public string[] PostString { get; set; }

		//リクエストトークンの取得
		public async Task<string[]> GetRequesttoken()
		{
			string APIURL = "https://api.twitter.com/oauth/request_token";

			var keys = new SortedDictionary<string, string>()
			{
				{"oauth_nonce",OAuthUtility.GenerateNonce()},
				{"oauth_callback","oob"},
				{"oauth_signature_method","HMAC-SHA1"},
				{"oauth_timestamp",OAuthUtility.GenerateTimeStamp()},
				{"oauth_consumer_key",ConsumerKey},
				{"oauth_version", "1.0"}
			};

			string signature = OAuthUtility.GenerateSignature(APIURL, HttpMethod.Post, ConsumerSecret, AccessSecret, keys);
			keys.Add("oauth_signature", signature);

			string RequestURL = "https://api.twitter.com/oauth/request_token?";
			string AuthorizationParms = null;

			foreach (string key in keys.Keys)
			{
				RequestURL += key + "=" + WebUtility.UrlEncode(keys[key]) + "&";
				AuthorizationParms += key + "=\"" + keys[key] + "\", ";
			}
			RequestURL = RequestURL.Remove(RequestURL.Length - 1);
			AuthorizationParms = AuthorizationParms.Remove(AuthorizationParms.Length - 2, 2);

			var request = new HttpRequestMessage();
			var response = new HttpResponseMessage();
			var client = new HttpClient();

			request.Method = HttpMethod.Post;
			request.RequestUri = new Uri(RequestURL);
			request.Headers.Authorization = new AuthenticationHeaderValue("OAuth", AuthorizationParms);
			request.Content = null;

			string result = null;

			response = await client.SendAsync(request);
			if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
			{
				string[] alternative = await GetRequesttoken();
				return alternative;
			}
			result = await response.Content.ReadAsStringAsync();


			RequestKey = Regex.Match(result, @"oauth_token=(.*?)&oauth_token_secret=.*?&oauth_callback.*").Groups[1].Value;
			RequestSecret = Regex.Match(result, @"oauth_token=(.*?)&oauth_token_secret=(.*?)&oauth_callback.*").Groups[2].Value;

			PostUrl = "https://api.twitter.com/oauth/authorize?oauth_token=" + RequestKey +
				"&oauth_token_secret=" + RequestSecret;
			var returnresult = new string[3];
			returnresult[0] = PostUrl;
			returnresult[1] = RequestKey;
			returnresult[2] = RequestSecret;
			PostString = returnresult;
			return returnresult;
		}
		//アクセストークンの取得(PINコードは別窓で取得してます)
		public async Task GetAccessKey(string PIN)
		{
			const string APIURL = "https://api.twitter.com/oauth/access_token";
			string ACSURL = "https://api.twitter.com/oauth/access_token?";

			var postdata = new SortedDictionary<string, string> 
			{
				{"oauth_consumer_key",ConsumerKey},
				{"oauth_nonce",OAuthUtility.GenerateNonce()},
				{"oauth_signature_method","HMAC-SHA1"},
				{"oauth_timestamp",OAuthUtility.GenerateTimeStamp()},
				{"oauth_token",RequestKey},
				{"oauth_version","1.0"},
				{"oauth_verifier",PIN}
			};

			string signature = OAuthUtility.GenerateSignature(APIURL, HttpMethod.Post, ConsumerSecret, RequestSecret, postdata);
			postdata.Add("oauth_signature", WebUtility.UrlEncode(signature));

			var postquery = new Dictionary<string, string>
			{
				{"oauth_consumer_key",ConsumerKey},
				{"oauth_nonce",postdata["oauth_nonce"]},
				{"oauth_signature_method","HMAC-SHA1"},
				{"oauth_timestamp",postdata["oauth_timestamp"]},
				{"oauth_token",RequestKey},
				{"oauth_version","1.0"},
				{"oauth_signature",WebUtility.UrlEncode(signature)},
				{"oauth_verifier",PIN}
			};

			foreach (string key in postquery.Keys)
			{
				ACSURL += key + "=" + postquery[key] + "&";
			}
			ACSURL = ACSURL.Remove(ACSURL.Length - 1);

			var content = new FormUrlEncodedContent(postdata);
			var request = new HttpRequestMessage(HttpMethod.Post, ACSURL);
			request.Content = content;
			var response = new HttpResponseMessage();
			var client = new System.Net.Http.HttpClient();
			string result = null;

			response = await client.SendAsync(request);
			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				result = await response.Content.ReadAsStringAsync();
			}
			else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
			{
//				var dialog = new MessageDialog("Twitterアプリ認証に失敗しました再度読み込んでください", "権限が取得できませんでした");
//				await dialog.ShowAsync();
				Console.WriteLine("Twitterアプリ認証に失敗しました再度読み込んでください", "権限が取得できませんでした");
			}
			//終わりの文字にありそうもない文字を追加して取得しやすくします
			result += "🍸";
			AccessKey = Regex.Match(result, @"oauth_token=(.*?)&oauth_token_secret=(.*?)&user_id=(.*?)&screen_name=(.*?)🍸(.*?)").Groups[1].Value;
			AccessSecret = Regex.Match(result, @"oauth_token=(.*?)&oauth_token_secret=(.*?)&user_id=(.*?)&screen_name=(.*?)🍸(.*?)").Groups[2].Value;
			UserId = Regex.Match(result, @"oauth_token=(.*?)&oauth_token_secret=(.*?)&user_id=(.*?)&screen_name=(.*?)🍸(.*?)").Groups[3].Value;
			ScreenName = Regex.Match(result, @"oauth_token=(.*?)&oauth_token_secret=(.*?)&user_id=(.*?)&screen_name=(.*?)🍸(.*?)").Groups[4].Value;

		}
	}
	public class HttpRequest
	{
		//GET系RestAPI postdataにパラメーターを入れて使います。付随パラメーターが空の場合でもnullのSortedDictionaryが必要です
		public static async Task<string> RestGET(OAuth oauth, string APIURL, SortedDictionary<string, string> postdata)
		{

			var Header = OAuthUtility.HeaderCreate(APIURL, oauth, HttpMethod.Get, postdata);
			string posturl = APIURL + "?";

			string authorizationHeaderParams = null;

			foreach (string key in Header.Keys)
			{
				posturl += key + "=" + Header[key] + "&";
				authorizationHeaderParams += key + "=\"" + Header[key] + "\", ";
			}
			posturl = posturl.Remove(posturl.Length - 1);
			authorizationHeaderParams = authorizationHeaderParams.Remove(authorizationHeaderParams.Length - 2);

			var request = new HttpRequestMessage();

			var client = new HttpClient();
			request.Method = HttpMethod.Get;
			request.RequestUri = new Uri(posturl);
			request.Headers.Host = "api.twitter.com";
			request.Headers.Authorization = new AuthenticationHeaderValue("OAuth", authorizationHeaderParams);

			var response = await client.SendAsync(request);
			string result = await response.Content.ReadAsStringAsync();
			return result;
		}
		//POST系RestAPI postdataにパラメーターを入れて使います。付随パラメーターが空の場合でもnullのSortedDictionaryが必要です
		public static async Task<string> RestPOST(OAuth oauth, string APIURL, SortedDictionary<string, string> postdata)
		{
			var postquery = new Dictionary<string, string>(postdata);

			SortedDictionary<string, string> Header = OAuthUtility.HeaderCreate(APIURL, oauth, HttpMethod.Post, postdata);
			string posturl = APIURL + "?";

			//最初に送るべきパラメーターを追加してからoauth関連のパラメーターを入れる
			postquery.Add("oauth_consumer_key",oauth.ConsumerKey);
			postquery.Add("oauth_nonce", Header["oauth_nonce"]);
			postquery.Add("oauth_timestamp", Header["oauth_timestamp"]);
			postquery.Add("oauth_token", oauth.AccessKey);
			postquery.Add("oauth_signature_method", "HMAC-SHA1");
			postquery.Add("oauth_version", "1.0");
			postquery.Add("oauth_signature", Header["oauth_signature"]);

			string authorizationHeaderParams = null;

			foreach (string key in postquery.Keys)
			{
				posturl += key + "=" + postquery[key] + "&";
				authorizationHeaderParams += key + "=\"" + postquery[key] + "\", ";
			}
			posturl = posturl.Remove(posturl.Length - 1);
			authorizationHeaderParams = authorizationHeaderParams.Remove(authorizationHeaderParams.Length - 2);

			var request = new HttpRequestMessage();

			var client = new HttpClient();
			request.Method = HttpMethod.Post;
			request.Content = new FormUrlEncodedContent(Header);
			request.RequestUri = new Uri(posturl);
			request.Headers.Host = "api.twitter.com";
			request.Headers.Authorization = new AuthenticationHeaderValue("OAuth", authorizationHeaderParams);

			var response = await client.SendAsync(request);
			string result = await response.Content.ReadAsStringAsync();
			return result;
		}
	}

	//OAuth関係で使うものはここにしまっておきます
	public static class OAuthUtility
	{
		//Timestampを生成します
		public static string GenerateTimeStamp()
		{
			TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return Convert.ToInt64(ts.TotalSeconds).ToString();
		}
		//Nonceを生成します
		public static string GenerateNonce()
		{
			Random random = new Random();
			string strings = "0123456789abcdefghijklmnopqrstuvwxyABCDEFGHIJKLMNOPQRSTUVWXYZ";
			string res = "";
			for (int i = 0; i < 32; i++)
				res += strings[random.Next(0, strings.Length - 1)];
			return res;
		}
		//一番汎用的なSignatureの使い方
		public static string GenerateSignature(string APIURL, HttpMethod type, string ConsumerSecret, string AccessSecret, SortedDictionary<string, string> parms)
		{
			string key = ConsumerSecret + "&" + AccessSecret;

			string data = null;
			foreach (string keys in parms.Keys)
			{
				data += WebUtility.UrlEncode(keys) + "=" + WebUtility.UrlEncode(parms[keys]) + "&";
			}
			data = data.Remove(data.Length - 1);
			data = WebUtility.UrlEncode(data);
			string URL = WebUtility.UrlEncode(APIURL);
			data = type.ToString() + "&" + URL + "&" + data;

			var KeyMaterial = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);
			var Converter = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1");
			var MacKey = Converter.CreateKey(KeyMaterial);

			var DataToBeSigned = CryptographicBuffer.ConvertStringToBinary(data, BinaryStringEncoding.Utf8);
			var SignBuffer = CryptographicEngine.Sign(MacKey, DataToBeSigned);

			string signature = CryptographicBuffer.EncodeToBase64String(SignBuffer);
			return signature;
		}
		//RestAPI使うときはこちらで十分
		public static string GenerateSignature(string APIURL, OAuth oauth, HttpMethod Method, SortedDictionary<string, string> postdata)
		{
			Uri url = new Uri(APIURL);
			string normalizedRequestParameters = null;

			string nonce = GenerateNonce();
			string timestamp = GenerateTimeStamp();

			SortedDictionary<string, string> parameters = postdata;
			parameters.Add("oauth_version", "1.0");
			parameters.Add("oauth_nonce", nonce);
			parameters.Add("oauth_timestamp", timestamp);
			parameters.Add("oauth_signature_method", "HMAC-SHA1");
			parameters.Add("oauth_consumer_key", oauth.ConsumerKey);
			parameters.Add("oauth_token", oauth.AccessKey);

			foreach (string key in parameters.Keys)
			{
				normalizedRequestParameters += key + "=" + parameters[key] + "&";
			}
			normalizedRequestParameters = normalizedRequestParameters.Remove(normalizedRequestParameters.Length - 1);

			string SignatureBase = Method.ToString() + "&" + WebUtility.UrlEncode(APIURL) + "&" + WebUtility.UrlEncode(normalizedRequestParameters);

			string Key = WebUtility.UrlEncode(oauth.ConsumerSecret) + "&" + WebUtility.UrlEncode(oauth.AccessSecret);

			var KeyMaterial = CryptographicBuffer.ConvertStringToBinary(Key, BinaryStringEncoding.Utf8);
			var Converter = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1");
			var MacKey = Converter.CreateKey(KeyMaterial);

			var DataToBeSigned = CryptographicBuffer.ConvertStringToBinary(SignatureBase, BinaryStringEncoding.Utf8);
			var SignBuffer = CryptographicEngine.Sign(MacKey, DataToBeSigned);

			string signature = CryptographicBuffer.EncodeToBase64String(SignBuffer);
			return signature;
		}
		//RestAPI専用、自動でsignatureを含めたパラメーターを作ってくれます
		public static SortedDictionary<string, string> HeaderCreate(string APIURL, OAuth oauth, HttpMethod Method, SortedDictionary<string, string> postdata)
		{
			Uri url = new Uri(APIURL);
			string normalizedUrl = APIURL;
			string normalizedRequestParameters = null;

			string nonce = GenerateNonce();
			string timestamp = GenerateTimeStamp();

			SortedDictionary<string, string> parameters = postdata;
			parameters.Add("oauth_consumer_key", oauth.ConsumerKey);
			parameters.Add("oauth_token", oauth.AccessKey);
			parameters.Add("oauth_nonce", nonce);
			parameters.Add("oauth_timestamp", timestamp);
			parameters.Add("oauth_version", "1.0");
			parameters.Add("oauth_signature_method", "HMAC-SHA1");

			foreach (string key in parameters.Keys)
			{
				normalizedRequestParameters += key + "=" + parameters[key] + "&";
			}
			normalizedRequestParameters = normalizedRequestParameters.Remove(normalizedRequestParameters.Length - 1);

			StringBuilder signatureBase = new StringBuilder();
			signatureBase.AppendFormat("{0}&", Method.ToString());
			signatureBase.AppendFormat("{0}&", WebUtility.UrlEncode(normalizedUrl));
			signatureBase.AppendFormat("{0}", WebUtility.UrlEncode(normalizedRequestParameters));
			string SignatureBase = signatureBase.ToString();


			string Key = string.Format("{0}&{1}", WebUtility.UrlEncode(oauth.ConsumerSecret), WebUtility.UrlEncode(oauth.AccessSecret));

			var crypt = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1");
			var keyBuffer = CryptographicBuffer.CreateFromByteArray(Encoding.UTF8.GetBytes(Key));
			var cryptKey = crypt.CreateKey(keyBuffer);

			var dataBuffer = CryptographicBuffer.CreateFromByteArray(Encoding.UTF8.GetBytes(SignatureBase));
			var signBuffer = CryptographicEngine.Sign(cryptKey, dataBuffer);

			byte[] value;
			CryptographicBuffer.CopyToByteArray(signBuffer, out value);

			string signature = Convert.ToBase64String(value);
			signature = WebUtility.UrlEncode(signature);
			parameters.Add("oauth_signature", signature);
			return parameters;
		}
	}
}
