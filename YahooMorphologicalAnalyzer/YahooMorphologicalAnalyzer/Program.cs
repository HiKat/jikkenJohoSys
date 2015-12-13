using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Xml.Linq;
using System.Collections.Specialized;
using System.Xml;
using System.IO;
using System.Collections;

//appID = dj0zaiZpPWk3TUVNOTdKUmxNTSZzPWNvbnN1bWVyc2VjcmV0Jng9OGQ-

namespace YahooMorphologicalAnalyzer
{
	class Program
	{
		public static void Main(string[] args)
		{
//			string str = "今日はいい天気です";
			string str = "すもももももももものうち";
			MorphologicalAnalyzer analyser = new MorphologicalAnalyzer();
			//Morpheme[] morphemes = analyser.Analyse(str);
			List<Morpheme> resutlList = analyser.Analyse(str);
			foreach (Morpheme m in resutlList) {
				Console.WriteLine (m.ToString());
			}

			//ルビふりAPIのとき
			//テストアドレス
//			string url = "http://jlp.yahooapis.jp/FuriganaService/V1/furigana";
//			Uri uri = new Uri (url);
//			string appId = "dj0zaiZpPWk3TUVNOTdKUmxNTSZzPWNvbnN1bWVyc2VjcmV0Jng9OGQ-";
//			string sentence = Uri.EscapeUriString("明鏡止水");
//			//POSTリクエストを作成
//			string postData = "appid=" + appId + "&sentence=" + sentence;
//			System.Net.WebClient wc = new System.Net.WebClient();
//			//文字コードを指定する
//			wc.Encoding = Encoding.GetEncoding("utf-8");
//			//ヘッダにContent-Typeを加える
//			wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
//			//wc.Headers.Add("Content-Length", "990");
//			//仕様書ではUploadStringの引数はstringと指定されているが
//			string resText = wc.UploadString(uri, postData);
//			//受信したデータを表示する
//			Console.WriteLine(resText);

		}
	}
		

	public class MorphologicalAnalyzer
	{
		//コンストラクタ
		public MorphologicalAnalyzer(){}

		//本当は
		public List<Morpheme> Analyse(string str)
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
			List<Morpheme> resultList = new List<Morpheme> ();
			int i = 0;
			while (wordListEnum.MoveNext()) {
				Morpheme ithResult = new Morpheme (surface.Item(i).InnerText, pos.Item(i).InnerText); 
				//debug
				//Console.WriteLine(surface.Item(i).InnerText);
				//Console.WriteLine(pos.Item(i).InnerText);
				resultList.Add (ithResult);
				i++;
			}
			return resultList;
		} 
	}

	public class Morpheme
	{
		private string surface;
		private string pos;

		//コンストラクタ
		public Morpheme(string surface, string pos)
		{
			this.surface = surface;	
			this.pos = pos;
		}

		public string Surface{
			get{ return this.surface; }
			set{ }
		}

		public string Pos{
			get{ return this.pos; }
			set{ }
		}

		public override string ToString(){
			string result = this.surface + " (" + this.pos + ") ";
			return result;
		}
	}
}