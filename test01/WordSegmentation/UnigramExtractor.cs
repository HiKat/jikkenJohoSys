using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordSegmentation
{
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
