using System;
using System.Collections.Generic;
using System.Xml;
using System.Collections;
using System.Net;
using System.Text;



namespace TextSimilarity
{
	class MainClass
	{
//		public static void Main (string[] args)
//		{
//			string text1 = "私の名前は、田中です。あなたの名前は山田です。";
//			string text2 = "私の名前は、山田です。";
//			TextSimilarity ts = new TextSimilarity(text1, text2);
//			double jaccardSim = ts.Jaccard(); // Jaccard係数
//			double cosineSim = ts.Cosine(); // コサイン類似度
//
//			Console.WriteLine ("=============================================");
//			Console.WriteLine ("Jaccard係数　= " + (string.Format("{0:f2}", jaccardSim)));
//			Console.WriteLine ("コサイン類似度　= " + (string.Format("{0:f2}", cosineSim)));
//		}
	}

	class TextSimilarity{

		//類似度計算を行いたい2つの文書文字列
		private string text1;
		private string text2;
		//類似度計算を行う文章からsearchPosで指定した品詞の単語を
		//抽出して類似度を計算する
		private string searchPos = "名詞";
		private List<string> wordList1 = new List<string>();
		private List<string> wordList2 = new List<string>();


		//コンストラクタ
		public TextSimilarity(string text1, string text2){
			this.text1 = text1;
			this.text2 = text2;
			wordList1 = Analyse(this.text1);
			wordList2 = Analyse(this.text2);

			//debug 
			Console.WriteLine("wordList1 =");
			foreach (string s in wordList1) {
				Console.Write ("{0} ", s);
			}
			Console.WriteLine ("");
			Console.WriteLine("wordList2 =");
			foreach (string s in wordList2) {
				Console.Write ("{0} ", s);
			}
			Console.WriteLine ("");

		}

		//Jaccard係数による類似度計算を行うメソッド==================================================================================
		//類似度を返す
		//public double Jaccard(){
		public double Jaccard(){

			//重複要素を削除して単語の和集合リストwordUniListを作成====================================
			List<string> wordUniListTemp = new List<string>(wordList1);
			List<string> wordUniList = new List<string>();

			//リスト連結
			wordUniListTemp.AddRange (wordList2);

			foreach (string s in wordUniListTemp) {
				if(!wordUniList.Contains(s)){
					wordUniList.Add(s);
				}
			}

			//debug
			Console.WriteLine("wordUniList =");
			foreach (string s in wordUniList) {
				Console.Write (s+" ");
			}
			Console.WriteLine ("");
			//===================================================================================


			//単語の積集合のリストを作成=============================================================
			List<string> wordInterList = new List<string>();
			foreach (string s in wordList1) {
				//wordList1内の単語sがwordList2にも入っていてかつ未だwordInterListに入っていないなら追加
				if((wordList2.Contains(s)) & !(wordInterList.Contains(s))){
					wordInterList.Add(s);
				}
			}

			//debug	
			Console.WriteLine("wordInterList =");
			foreach (string s in wordInterList) {
				Console.Write (s+" ");
			}
			Console.WriteLine ("");
			//====================================================================================


			double result = ((double)(wordInterList.Count) / (double)(wordUniList.Count));

			//debug
			Console.WriteLine("JaccardValue = " + (string.Format("{0:f2}", result)));

			return result;
		}
		//=====================================================================================================================



		//コサイン類似度の計算を行うメソッド======================================================================================
		public double Cosine(){
			//抽出した単語リストのベクトルの文字列をアスキーコードにして
			//ベクトル計算を行う

			//重複要素を削除して単語の和集合リストwordUniListを作成====================================
			List<string> wordUniListTemp = new List<string>(wordList1);
			List<string> wordUniList = new List<string>();

			//リスト連結
			wordUniListTemp.AddRange (wordList2);

			foreach (string s in wordUniListTemp) {
				if(!wordUniList.Contains(s)){
					wordUniList.Add(s);
				}
			}

			//debug
			Console.WriteLine("wordUniList =");
			foreach (string s in wordUniList) {
				Console.Write (s+" ");
			}
			Console.WriteLine ("");
			//===================================================================================


			//出現頻度ベクトル（ディクショナリとし実装）を作成===========================================
			//int dicLength = wordUniList.Count;
			//keyがwordUniListの値であるようなリストをそれぞれ作成
			List<int> text1Vec = new List<int>();
			List<int> text2Vec = new List<int>();

			//和集合の各要素がwordList1,2のそれぞれで出現する回数をリストにする
			foreach (string s in wordUniList) {
				int count1 = 0;
				int count2 = 0;
				foreach (string w1 in wordList1) {
					if (s == w1) {
						count1++;
					}
				}
				text1Vec.Add(count1);
				foreach (string w2 in wordList2) {
					if (s == w2) {
						count2++;
					}
				}
				text2Vec.Add(count2);			
			}

			//debug
			Console.WriteLine("text1Vec =");
			foreach (int i in text1Vec) {
				Console.Write (i+" ");
			}
			Console.WriteLine ("");

			Console.WriteLine("text2Vec =");
			foreach (int i in text2Vec) {
				Console.Write (i+" ");
			}
			Console.WriteLine ("");

			//各ベクトルの長さを計算=================
			double lengText1Vec;
			double powSum1 = 0;
			foreach (int i in text1Vec) {
				powSum1 += Math.Pow (i, 2.0);
			}
			lengText1Vec = Math.Sqrt (powSum1);

			double lengText2Vec;
			double powSum2 = 0;
			foreach (int i in text2Vec) {
				powSum2 += Math.Pow (i, 2.0);
			}
			lengText2Vec = Math.Sqrt (powSum2);

			//debug
			Console.WriteLine("lengText1Vec =" + lengText1Vec.ToString());
			Console.WriteLine("lengText2Vec =" + lengText2Vec.ToString());
			//===================================

			//それぞれのベクトルの内積を計算====================================
			double prodVec1And2 = 0;
			//各ベクトル要素数（同じであるはず）
			int vecCount;
			if (text1Vec.Count == text2Vec.Count) {
				vecCount = text1Vec.Count;
			} else {
				vecCount = 0;
				Console.WriteLine ("error!");
			}

			for(int i = 0; i < vecCount; i++){
				prodVec1And2 += text1Vec[i] * text2Vec[i];
			}

			//debug
			Console.WriteLine("prodVec1And2 =" + prodVec1And2.ToString());
			//=============================================================

			double result = (prodVec1And2 / (lengText1Vec * lengText2Vec));

			//debug
			Console.WriteLine("CosineSimilarityValue = " + (string.Format("{0:f2}", result)));

			return result;
		}
		//==================================================================================================================


		//単語抽出メソッド（Yahoo形態素解析APIを使用）
		private List<string> Analyse(string str)
		{
			//形態素解析
			//リクエストURL
			string url = "http://jlp.yahooapis.jp/MAService/V1/parse";
			Uri uri = new Uri (url);
			string appId = "dj0zaiZpPWk3TUVNOTdKUmxNTSZzPWNvbnN1bWVyc2VjcmV0Jng9OGQ-";
			//string sentence = Uri.EscapeUriString("9人や。うちを入れて。");
			string sentence = Uri.EscapeUriString(str);
			string resultsParm = "ma";
			//POSTリクエストを作成
			string postData = "appid=" + appId + "&sentence=" + sentence + "&results=" + resultsParm;
			System.Net.WebClient wc = new System.Net.WebClient();
			//文字コードを指定する	
			wc.Encoding = Encoding.GetEncoding("utf-8");
			//ヘッダにContent-Typeを加える
			wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
			string resText = wc.UploadString(uri, postData);
			//受信したデータを表示するとき
			//Console.WriteLine(resText);

			XmlDocument resXml = new XmlDocument();
			resXml.LoadXml(resText);
			//ResultSet取得
			XmlElement resultSet = resXml.DocumentElement;
			//debug
			//Console.WriteLine("{0} {1} {2}", resultSet.NodeType.ToString(), resultSet.LocalName, resultSet.Name);
			//ma_result取得
			XmlNode maResult = resultSet.FirstChild;
			//word_list取得
			//プロパティを指定してタグを探索	
			XmlElement wordList = maResult["word_list"];

			//word_listノードの子ノードの反復処理に使用する列挙子の取得
			IEnumerator wordListEnum = wordList.GetEnumerator ();
			XmlNodeList surface = resXml.GetElementsByTagName("surface");
			XmlNodeList pos = resXml.GetElementsByTagName ("pos");
			List<string> resultList = new List<string> ();
			int i = 0;
			while (wordListEnum.MoveNext()) {
				string ithWord;
				if (pos.Item (i).InnerText == this.searchPos) {
					ithWord = surface.Item (i).InnerText;
					resultList.Add (ithWord);
				}
				//Morpheme ithResult = new Morpheme (surface.Item(i).InnerText, pos.Item(i).InnerText); 
				//debug
				//Console.WriteLine(surface.Item(i).InnerText);
				//Console.WriteLine(pos.Item(i).InnerText);
				i++;
			}
			return resultList;
		} 


	}
}
