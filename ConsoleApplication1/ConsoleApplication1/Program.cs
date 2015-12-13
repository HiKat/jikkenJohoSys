using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] a = new int[3] { 1, 2, 3 };
            int[] b = new int[3] { 11, 22, 33 };
            Debug.WriteLine("a = {0} {1} {2}\n", a[0], a[1], a[2]);
            Debug.WriteLine("b = {0} {1} {2}\n", b[0], b[1], b[2]);
        }

        public void Test(int[] a, int[] b)
        {
            a = new int[3] { 2, 4, 8 };
            b[0] = 111;
        }
    }
}
