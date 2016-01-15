using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections;
using TwitterOAuth;
using TwitterTweetJson;
using TwitterApi;
using YahooMorphologicalAnalyzer;
using System.Linq;
using Classify;


//留意点
//* Newtonsoft.Json.dllはJSON.NETの外部ライブラリです.
//* TwiiterOAuth.csはTwitterAPIを使用する際にOAuth認証を通過するための
//メソッドやプロパティを揃えたクラスです
//* TwitterApi.csはTwitterAPIを使用するためのクラスで、
//認証はTwitterOAuth.cs内のAuthクラスを継承したTwitterConnectorクラスの
//インスタンスを使用することでパスすることができます.
//* TwitterJson.csはツイート情報のJSONオブジェクトをC#のクラスの形式にして書いたものです.

//これはコンソール上で動作する簡単なTwitterクライアントです
//最低限の実装なのでtry~catchの類は書いていません.
//存在しないTweetID等を入力すると当然エラーが発生します.
namespace Main
{

	class Program
	{
		static void Main (string[] args)
		{
//			//リクエストトークン取得====================================================================
//			//ApiTestNoCallback
//			//callbackurl設定なしアカウント
			const string CONSUMER_KEY = "8369eInu3C2YzkAgx3YrWbXMJ";
			const string CONSUMER_SECRET = "8uWxX1iTTR1yxdA25RqlQxxn82tDipnsC5482UU3Etm3EpwQGn";
//
//			Auth auth = new Auth (CONSUMER_KEY, CONSUMER_SECRET);
//
//			// リクエストトークンを取得する
//			auth.GetRequestToken ();
//
//			//ユーザーにRequestTokenを認証してもらう
//			Console.WriteLine ("Please jump to the AuthenticationURL：");
//			Console.WriteLine ("https://api.twitter.com/oauth/authorize?oauth_token=" + auth.RequestToken);
//			Console.Write ("Enter your PIN:");
//			string pin = Console.ReadLine ();
//			// アクセストークンを取得する
//			auth.GetAccessToken (pin);
//
//			// 結果を表示する
//			Console.WriteLine ("Your AccessToken: " + auth.AccessToken);
//			Console.WriteLine ("Your AccessTokenSecret: " + auth.AccessTokenSecret);
//			Console.WriteLine ("Your UserId: " + auth.UserId);
//			Console.WriteLine ("Your ScreenName: " + auth.ScreenName);
//			//=====================================================================================

			//====================================================
			//debug
//			TwitterConnector tc = new TwitterConnector (
//				auth.ConsumerKey, auth.ConsumerSecret, auth.AccessToken, 
//				auth.AccessTokenSecret, auth.UserId, auth.ScreenName);
			//debug限定
			TwitterConnector tc = new TwitterConnector (
				                      CONSUMER_KEY, CONSUMER_SECRET, 
				                      "4452190160-WVQ7leuqf48yruElrdZtbl2IeEcPE4JbNUqBrnp",
				                      "hQvfQmZA13uNDEFWEf9HGb3GQflogJZlbWddnVqQQPaKP",
				                      "4452190160",
				                      "apiTestJohoSys");
			
			Console.WriteLine ("debug start! Enter return key!");
			Console.ReadKey (); 
			Console.Write ("<===== This is a tiny Twitter client on console =====>\n");
			List<UserEx> res = new List<UserEx> ();
			//res = tc.GetFriendsList ("shimizu111485");
			res = tc.GetFriendsList ("apiTestJohoSys");
			Dictionary<string, List<string>> allWordLists = new Dictionary<string, List<string>> ();
			foreach (UserEx u in res) {
				MakeWordList wl = new MakeWordList (u.Description);
				//wl.Analyはユーザーのプロフィール文から名詞を抜き出してList<string>としたもの.
				allWordLists.Add (u.ScreenName, wl.Analy ());
			}
			//allWrodListはクラスタリング対象の全てのユーザーのプロフィールから抽出した単語のリストList<List<string>>
			MakeWordVec wv = new MakeWordVec (allWordLists);
			//wv.MakeWordVecsList()はそれぞれのベクトル名と値を登録したDictionary<string, List<int>>を返す.
			ClassifyVector cv = new ClassifyVector (wv.MakeWordVecsList (), 20);
			//cv.HierarchicalClustering()はCluster[]を返す
			foreach (Cluster c in cv.HierarchicalClustering()) {
				foreach (string s in c.VecNamesArr()) {
					Console.Write (s + " ");
				}
				Console.WriteLine ("\n\n");
			}
			//debug
			Console.WriteLine ("debug end! Enter return key!");
			Console.ReadKey ();
			//=====================================================
		}
	}
	//=================================================================================
	//一人のユーザーのプロフィール文から名詞を抜き出してAnaly()メソッドでList<string>を返す
	public class MakeWordList
	{
		private string searchPos = "名詞";

		//コンストラクタ
		public MakeWordList (string str)
		{
			Text = str;
		}

		//プロパティ
		public string Text{ get; protected set; }

		public List<string> Analy ()
		{
			//形態素解析
			//リクエストURL
			string url = "http://jlp.yahooapis.jp/MAService/V1/parse";
			Uri uri = new Uri (url);
			string appId = "dj0zaiZpPWk3TUVNOTdKUmxNTSZzPWNvbnN1bWVyc2VjcmV0Jng9OGQ-";
			string sentence = Uri.EscapeUriString (Text);
			string resultsParm = "ma";
			//POSTリクエストを作成
			string postData = "appid=" + appId + "&sentence=" + sentence + "&results=" + resultsParm;
			System.Net.WebClient wc = new System.Net.WebClient ();
			//文字コードを指定する	
			wc.Encoding = Encoding.GetEncoding ("utf-8");
			//ヘッダにContent-Typeを加える
			wc.Headers.Add ("Content-Type", "application/x-www-form-urlencoded");
			string resText = wc.UploadString (uri, postData);
			//受信したデータを表示するとき
			//Console.WriteLine(resText);

			XmlDocument resXml = new XmlDocument ();
			resXml.LoadXml (resText);
			//ResultSet取得
			XmlElement resultSet = resXml.DocumentElement;
			//debug
			//Console.WriteLine("{0} {1} {2}", resultSet.NodeType.ToString(), resultSet.LocalName, resultSet.Name);
			//ma_result取得
			XmlNode maResult = resultSet.FirstChild;
			//word_list取得
			//プロパティを指定してタグを探索	
			XmlElement wordList = maResult ["word_list"];

			//word_listノードの子ノードの反復処理に使用する列挙子の取得
			IEnumerator wordListEnum = wordList.GetEnumerator ();
			XmlNodeList surface = resXml.GetElementsByTagName ("surface");
			XmlNodeList pos = resXml.GetElementsByTagName ("pos");
			List<string> resultList = new List<string> ();
			int i = 0;
			while (wordListEnum.MoveNext ()) {
				string ithWord;
				if (pos.Item (i).InnerText == this.searchPos) {
					ithWord = surface.Item (i).InnerText;
					resultList.Add (ithWord);
				}
				i++;
			}
			//debug
//			bool first = true;
//			foreach (string s in resultList) {
//				if (first) {
//					Console.Write ("\n\n>>" + s);
//					first = false;
//				} else {
//					Console.Write ("  " + s);
//				}
//			}

			return resultList;
		}
	}
	//=================================================================================
	public class MakeWordVec
	{
		//コンストラクタ
		public MakeWordVec (Dictionary<string, List<string>> tgtWordLists)
		{
			TgtWordLists = tgtWordLists;
		}
		//プロパティ
		public Dictionary<string, List<string>> TgtWordLists { get; protected set; }

		public Dictionary<string, List<int>> MakeWordVecsList ()
		{
			List<string> wordUniListTemp = new List<string> ();
			List<string> wordUniList = new List<string> ();

			//出現する全ての単語を重複を許して列挙
			foreach (KeyValuePair<string, List<string>> wl in TgtWordLists) {
				wordUniListTemp.AddRange(wl.Value);
			}
			//wordUniListTempから重複を除いてwordUniListを作成
			//debug
			Console.WriteLine("全ユーザーのプロフィールから抽出された語>>");
			foreach (string s in wordUniListTemp) {
				if (!wordUniList.Contains (s)) {
					wordUniList.Add (s);
					//debug
					Console.Write (s + " ");
				}
			}
			Dictionary<string, List<int>> resultList = new Dictionary<string, List<int>> ();
			//int i = 0;
			foreach (KeyValuePair<string, List<string>> tgt in TgtWordLists) {
				//lは一人のユーザープロフィールから作成した語ベクトル
				List<int> l = new List<int> ();
				//ベクトルの作成==========================
				//debug
				Console.WriteLine();
				Console.WriteLine("@" + tgt.Key);
				Console.Write ("{ ");
				foreach (string str in wordUniList) {
						if (tgt.Value.Contains (str)) {
							l.Add (1);
							//debug
							Console.Write ("1 ");
						} else {
							l.Add (0);
							//debug
							Console.Write ("0 ");
						}
				}
				//debug
				Console.WriteLine ("}\n\n");
				resultList.Add (tgt.Key, l);
				//======================================
			}
			return resultList;
		}
	}
	//=================================================================================
}