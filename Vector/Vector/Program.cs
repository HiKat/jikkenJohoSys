using System;

namespace Vector
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Vector x = new Vector (new double[] { 1.0, 2.0 });
			Vector y = new Vector (new double[] { 3.0, 1.0 });
			Vector z1 = new Vector (9);

			double temp = x.GetValue (1);
			x.ToString ();
			z1.ToString ();
			double xNorm = x.Norm ();
			Vector z = x.Add (y); // (4.0, 3.0)
			z.ToString ();
			Vector a = new Vector(2);
			a = a.Add(x); // (1.0, 2.0)
			a.ToString();
//			Vector b = a.ScalarMultiply(2.0); // (2.0, 4.0)
		}
	}



	class Vector
	{

		private double[] vector;
		private int dimension;

		//コンストラクタ===========================================================
		public Vector (double[] vector)
		{
			this.vector = (double[])vector.Clone ();
			this.dimension = vector.Length;
		}
			
		//0ベクトルインスタンスを生成する
		public Vector (int dimension)
		{
			this.vector = new double[dimension];
			for (int i = 0; i < dimension; i++) {
				this.vector [i] = 0;
			}
			this.dimension = dimension;
		}
		//========================================================================


		//指定された次元の要素値を返すメソッド==================================================
		public double GetValue (int dimension)
		{
			double result = 0;
			try {
				result = this.vector [dimension];	

				//debug
				Console.WriteLine ("");
				Console.Write ("{0}th element is {1}", dimension, result.ToString ());

			} catch (System.IndexOutOfRangeException ex) {
				Console.WriteLine ("{0}:GetValueメソッドの引数が不正です.", ex);
			}
			return result;	
		}
		//==================================================================================


		//ベクトルを"(x1, x2, ...)"の形式の文字列で返すメソッド============
		public override string ToString ()
		{
			string result = string.Format ("(");
			int i = 0;
			foreach (double d in this.vector) {
				if (i != (this.vector.Length - 1)) {
					result += string.Format ("{0:F1}, ", d);
				} else {
					result += (string.Format ("{0:f1})", d));
				}
				i++;
			}

			//debug
			Console.WriteLine ("");
			Console.WriteLine (result);

			return result;
		}
		//============================================================

		//============================================================================
		//自身と指定されたベクトルの和を返すメソッド======
		public Vector Add (Vector other)
		{
			return AddOrSub (other.vector, "add");
		}

		//自身と指定されたベクトルの和を返すメソッド
		public Vector Add (double[] other)
		{
			return AddOrSub (other, "add");
		}
		//==========================================


		//自身と指定されたベクトルの差を返すメソッド======
		public Vector Sub (Vector other)
		{
			return AddOrSub (other.vector, "sub");
		}

		public Vector Sub (double[] other)
		{
			return AddOrSub (other, "sub");
		}
		//=========================================

		private Vector AddOrSub (double[] other, string op)
		{
			double[] resultArr;
			if (this.dimension != other.Length) {
				Console.WriteLine ("2引数ベクトルの次元が異なっています.");
			}
			try {
				resultArr = new double[this.dimension];
				for (int i = 0; i < this.dimension; i++) {
					if (op == "add") {
						resultArr [i] = (this.vector [i] + other [i]);
					}
					if (op == "sub") {
						resultArr [i] = (this.vector [i] - other [i]);
					}
				}
			} catch (System.IndexOutOfRangeException ex) {
				resultArr = new double[1];
				resultArr [1] = -1;
				Console.WriteLine ("{0}:メソッドの引数が不正です.", ex);
			}
			Vector resultVec = new Vector (resultArr);
			return resultVec;
		}
		//==============================================================================










		//ノルムを取得するメソッド===============================
		public double Norm ()
		{
			double sum = 0;
			foreach (double d in this.vector) {
				sum += Math.Pow (d, 2.0);
			}
			double result = Math.Sqrt (sum);
			Console.WriteLine ("Norm = {0:f1}", result);
			return result;
		}
		//===================================================

		//ベクトルの次元を取得するプロパティ
		int Dimension {
			get{ return this.dimension; }
			set{ }
		}

		double[] Elements {
			get{ return this.vector; }
			set{ }
		}
	}
}
