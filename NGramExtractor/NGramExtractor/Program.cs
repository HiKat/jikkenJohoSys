using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGramExtractor
{
	class Program
	{
		static void Main(string[] args)
		{
			string str = "今日はいい天気です";
			int gramNum = 2;
			NGramParser parser = new NGramParser(gramNum, str);
			Console.WriteLine("文字列 = {0}\n", parser.GramStr);
			Console.WriteLine("n = {0}\n", parser.GramNum);
			string[] result = parser.Parse(str);
			Console.Write("[ ");
			foreach(string s in result){
				Console.Write(s+" ");
			}	
			Console.Write ("]");
		}
	}
		
	public class NGramParser
	{

		private int n;
		private string str;


		public NGramParser(int gramNum, string gramStr)
		{
			n = gramNum;
			str = gramStr;
		}

		/// <summary>
		/// 指定された文字列からn-gramを生成するメソッド
		/// </summary>
		/// <param name="str">str：n-gramにかける文字列</param>
		/// <returns>gramNum文字ずつになった文字列の配列</returns>
		public string[] Parse(string str)
		{
			List<string> list = new List<string>();

			//put ith character into list
			for (int i = 0; i < (str.Length - this.n + 1); i++)
			{
				//i番の文字からn文字を取得する
				string ithWord = str.Substring(i, this.n);
				list.Add(ithWord);
			}

			//output
			string[] result = list.ToArray();
			return result;
		}
			
		public int GramNum
		{
			set { this.n = value; }
			get { return this.n; }
		}


		public string GramStr
		{
			set { this.str = value; }
			get { return this.str; }
		}
	}
}