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
            NGramParser parser = new NGramParser(gramNum);
            string[] result = parser.Parse(str);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class NGramParser
    {

        private int n;
        private string str;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="gramNum"></param>
        public NGramParser(int gramNum)
        {
            n = gramNum;
        }

        /// <summary>
        /// 指定された文字列からn-gramを生成するメソッド
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string[] Parse(string str)
        {

        }

        /// <summary>
        /// nについてのプロパティ
        /// </summary>
        public int GramNum
        {
            set { this.n = value; }
            get { return this.n; }
        }





    }


    public class UnigramExtractor : IExtractable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string[] Extract(string str)
        {
            //make list
            List<string> list = new List<string>();

            //put ith character into list
            for (int i = 0; i < str.Length; i++)
            {
                //get ith char
                string ithWord = str.Substring(i, 1);

                //add ith char to list
                list.Add(ithWord);
            }

            //output
            string[] result = list.ToArray();
            return result;

        }
    }
}