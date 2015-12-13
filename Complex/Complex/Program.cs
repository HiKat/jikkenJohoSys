using System;

namespace Complex
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Complex x = new Complex(1.0, 2.0);
			Complex y = new Complex("4.0+1.0i");
			Complex w = x.Add(y); // 5.0 + 3.0i
			Complex z = x.Multiply(y); // 2.0 + 9.0i
			Console.WriteLine(x.ToString());
			Console.WriteLine(y.ToString());
			Console.WriteLine(w.ToString()); // 5.0 + 3.0i
			Console.WriteLine(z.ToString()); 
		}
	}

	public class Complex{

		//実部、虚部
		private double re;
		private double im;
		//極形式の絶対値、偏角
		private double z;
		private float arg;//°(degree)



		//コンストラクタ
		//どのコンストラクタで初期化されてもフィールドが初期化されるようにする
		public Complex(string str){
			char[] delimiterChars = { 'i', '+' };
			string[] aAndB = str.Split (delimiterChars, StringSplitOptions.RemoveEmptyEntries);
			this.re = double.Parse(aAndB [0]);
			this.im = double.Parse(aAndB [1]);
			this.z = Math.Sqrt (Math.Pow (this.re, 2) + Math.Pow (this.im, 2));
			this.arg = (float)((Math.Atan2 (this.im, this.re)) * 180 / Math.PI);
			//debug
			//Console.WriteLine ("re = {0} im = {1}", this.re.ToString(), this.im.ToString());
		}
		public Complex(double re, double im){
			this.re = re;
			this.im = im;
			this.z = Math.Sqrt (Math.Pow (this.re, 2) + Math.Pow (this.im, 2));
			this.arg = (float)((Math.Atan2 (this.im, this.re)) * 180 / Math.PI);
			//debug
			//Console.WriteLine ("re = {0} im = {1}", this.re.ToString(), this.im.ToString());
		}
		public Complex(double z, float arg){
			this.z = z;
			this.arg = arg;
			float angle = (float)(Math.PI * (arg / 180));
			this.re = z * Math.Cos (angle);
			this.im = z * Math.Sin (angle);
			//debug
			//Console.WriteLine ("z = {0} im = {1}", this.z.ToString(), this.arg.ToString());
		}

		//自身と指定された複素数の和を返すメソッド
		public Complex Add(Complex other){
			Complex resComp = new Complex((this.re)+(other.Real), (this.im)+(other.Imaginary));
			return resComp;
		}
		//自身と指定された複素数の差を返すメソッド
		public Complex Substract(Complex other){
			Complex resComp = new Complex((this.re)-(other.Real), (this.im)-(other.Imaginary));
			return resComp;
		}
		//自身と指定された実数の積を返すメソッド
		public Complex Multiply(double d){
			Complex resComp = new Complex((this.re)*d, (this.im)*d);
			return resComp;
		}
		//自身と指定された複素数の積を返すメソッド
		public Complex Multiply(Complex other){
			double a = this.re;
			double b = this.im;
			double c = other.Real;
			double d = other.Imaginary;
			Complex resComp = new Complex((a*c-b*d), (a*d+b*c));
			return resComp;
		}
		//別のインスタンスを生成するメソッド
		public Complex Clone(){
			Complex resComp = new Complex(this.re, this.im);
			return resComp;
		}
		//"a+bi"の形式の文字列を返すメソッド
//		public override string ToString ()
//		{
//			return string.Format ("{0}+{1}i", this.re, this.im);
//		} 
		public override string ToString ()
		{
			//0のプレースメントでも指定可能（どちらも小数点以下1位までを表示）
			return string.Format ("{0:0.0}+{1:f1}i", this.re, this.im);
		}


		//プロパティ
		public double Real{
			get { return this.re; }
			set { }
		}
		public double Imaginary{
			get { return this.im; }
			set { }
		}
		public double Z{
			get { return this.z; }
			set { }
		}
		public double Arg{
			get { return this.arg; }
			set { }
		}
	} 
}
