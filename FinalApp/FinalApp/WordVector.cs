using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections;
namespace WordVector
{
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

		public Dictionary<string, List<double>> MakeWordVecsList ()
		{
			List<string> wordUniListTemp = new List<string> ();
			List<string> wordUniList = new List<string> ();

			//出現する全ての単語を重複を許して列挙
			foreach (KeyValuePair<string, List<string>> wl in TgtWordLists) {
				wordUniListTemp.AddRange (wl.Value);
			}
			//wordUniListTempから重複を除いてwordUniListを作成
			//debug
			Console.WriteLine ("全ユーザーのプロフィールから抽出された語>>");
			foreach (string s in wordUniListTemp) {
				if (!wordUniList.Contains (s)) {
					wordUniList.Add (s);
					//debug
					Console.Write (s + " ");
				}
			}
			Dictionary<string, List<double>> resultList = new Dictionary<string, List<double>> ();
			//int i = 0;
			foreach (KeyValuePair<string, List<string>> tgt in TgtWordLists) {
				//lは一人のユーザープロフィールから作成した語ベクトル
				List<double> l = new List<double> ();
				//ベクトルの作成==========================
				//debug
				Console.WriteLine ();
				Console.WriteLine ("@" + tgt.Key + " のプロフィールの特徴語ベクトル");
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
