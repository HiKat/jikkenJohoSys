using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFVectorGenerator
{
	class Program
	{
		static void Main(string[] args)
		{
			string str = "To be, or not to be";
			TFVectorGenerator generator = new TFVectorGenerator();
			Dictionary<string, int> result = generator.Generate(str);
			Console.Write ("{");
			foreach(KeyValuePair<string, int> kvp in result)
			{
				Console.Write("{{\"{0}\", {1}}}", kvp.Key, kvp.Value);
			}
			Console.WriteLine ("}");
		}
	}

	public class TFVectorGenerator
	{

		//コンストラクタ
		public TFVectorGenerator(){}


		public Dictionary<string, int> Generate(string str)
		{
			// カンマ区切りで分割して配列に格納する
			char[] delimiterChars = {' ', ','};
			string[] ary = str.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
			//文頭の大文字で始まる単語は小文字に変換する
			ary[0] = ary[0].ToLower();

			List<string> wordlist = new List<string>();

			foreach (string s in ary)
			{
				//コレクション内に存在していなければ、追加する
				if (!wordlist.Contains(s))
				{
					wordlist.Add(s);
				}
			}
			Dictionary<string, int> dic = new Dictionary<string, int>(wordlist.Count);

			for (int i = 0; i < wordlist.Count; i++)
			{
				//出現回数はwordlist[i]がaryの中に何個出て来るかカウントする.
				int countWord = 0;
				foreach (string s in ary) {
					if(s == wordlist[i]){
						countWord++;
					}
				}
				dic.Add(wordlist[i], countWord);
			}

			return dic;
		}
	}
}