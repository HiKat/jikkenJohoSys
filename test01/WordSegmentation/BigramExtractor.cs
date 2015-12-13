using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordSegmentation
{
    public class BigramExtractor : IExtractable
    {
        public string[] Extract(string str)
        {
            //make a list for separated word
            //class => List<string>
            List<string> list = new List<string>();

            //add ith 2word to list
            for(int i = 0; i < str.Length - 1; i++)
            {

                //get ith 2word
                string ithWord = str.Substring(i, 2);

                //add ith 2word to list
                list.Add(ithWord);
            }
            //Output
            string[] result = list.ToArray();
            return result;
        }
    }
}
