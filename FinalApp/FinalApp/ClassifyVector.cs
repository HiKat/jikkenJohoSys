using System;
using System.Collections.Generic;

namespace Classify
{
	//Dictinary<string:ベクトル名, List<double>:ベクトル本体>からなるベクトルの集合を
	//階層的クラスタリングに基づいてクラスタリングするためのクラス
	public class ClassifyVector
	{
		//後の処理のためにベクトル部分をList<double>[]で、
		//ベクトル名部分をstring[]としてそれぞれをインデックスで対応させる
		//ベクトルの名前の配列
		private string[] vecNames;
		//ベクトル本体の配列
		private List<double>[] vecData;

		//コンストラクタ
		//Dictionary<string, List<double>>:ベクトル集合
		//int:クラスタリング操作の回数
		public ClassifyVector (Dictionary<string, List<double>> vectors, int steps)
		{
			Vectors = vectors;
			Steps = steps;
			vecNames = new string[Vectors.Count];
			vecData = new List<double>[Vectors.Count];
			int i = 0;
			foreach (KeyValuePair<string, List<double>> kvp in Vectors) {
				vecNames [i] = kvp.Key;
				vecData [i] = kvp.Value;
				i++;
			}
		}

		//プロパティ
		//元となったベクトル名とベクトルデータ本体が格納されたDictionary
		public Dictionary<string, List<double>> Vectors{ get; protected set; }
		//階層クラスタリングの実行回数
		public int Steps{ get; protected set; }

		//全ベクトルのi番目のベクトル名を取得するメソッド
		public string VecNameI (int i)
		{
			return vecNames [i];
		}
		//全ベクトルのi番目のベクトルの値を取得するメソッド
		public List<double> VecValI (int i)
		{
			return vecData [i];
		}
	
		//2ベクトル間の距離を計算するメソッド
		//ユークリッド距離を用いる
		private double DisVec (List<double> vec1, List<double> vec2)
		{
			double result;
			if (vec1.Count == vec2.Count) {
				double tmp = 0;
				for (int i = 0; i < vec1.Count; i++) {
					tmp += Math.Pow ((vec1 [i] - vec2 [i]), 2.0);
				}
				result = Math.Sqrt (tmp);
			} else {
				Console.WriteLine ("ERROR! ");
				result = -1;
			}
			return result;
		}

		//2クラスタ間の距離を計算するメソッド
		//f=0のとき
		//ユークリッド距離による群平均距離を用いて算出する
		//f=1のとき
		//クラスタ間の重心間距離で距離を算出する
		private double DisC (Cluster c1, Cluster c2, int f)
		{
			if(f == 0){
			double tmp = 0;
				for (int i = 0; i < c1.ClusterSize(); i++) {
					for (int j = 0; j < c2.ClusterSize(); j++) {
					tmp += DisVec (c1.VecValI (i), c2.VecValI (j));
				}
			}
				double result = (tmp / (c1.ClusterSize() * c2.ClusterSize()));
			return result;
			}
			if (f == 1) {
				return DisVec (c1.GetClusterGravity (), c2.GetClusterGravity ());
			} else {
				Console.WriteLine ("ERROR");
				return -1;
			}
		}

		//2つのクラスタをマージして1つのクラスタにするメソッド
		public Cluster MergeCluster (Cluster c1, Cluster c2)
		{
			Dictionary<string, List<double>> res = new Dictionary<string, List<double>> (c1.ClusterSize() + c2.ClusterSize());
			//クラスタ1を追加
			foreach (KeyValuePair<string, List<double>> kvp in c1.Data) {
				res.Add (kvp.Key, kvp.Value);
			}
			//クラスタ2を追加
			foreach (KeyValuePair<string, List<double>> kvp in c2.Data) {
				res.Add (kvp.Key, kvp.Value);
			}
			Cluster result = new Cluster (res);
			return result;
		}

		//ユーザによって指定された回数だけ階層クラスタリングを行うメソッド
		//入力データはVectorsを使用
		//出力はクラスタの配列
		//なお、出力されるクラスタの配列のサイズは1ステップごとにクラスタの総数が
		//1減るので、(「入力ベクトル数」-「ステップ数」)である.
		public List<Cluster> HierarchicalClustering ()
		{
			//まず、各ベクトル単体からなるクラスタを用意する
			//uは全クラスタの集合
			//クラスタ総数(入力ベクトル総数)
			int numClusters = Vectors.Count;
			//Cluster[] u = new Cluster[numClusters];
			List<Cluster> u = new List<Cluster> ();
			for (int i = 0; i < numClusters; i++) {
				Dictionary<string, List<double>> dic = new Dictionary<string, List<double>> ();
				dic.Add (vecNames [i], vecData [i]);
				Cluster cl = new Cluster (dic);
				u.Add (cl);
			}
				
			//総計算量
			double allSteps = 0;
			for (int s = 0; s < Steps; s++) {
				allSteps += 0.5 * (numClusters - s) * (numClusters - 1 - s);
			}
			double done = 0;

			//全クラスタの組み合わせの中から最も距離が近いクラスタをマージして新しいクラスタとする.
			//探索中で最も近くにあるベクトルの名前の組と距離を逐次記憶、更新する
			for (int s = 0; s < Steps; s++) {
				int tmpi = 0;
				int tmpj = 1;
				double disBuf = DisC (u [0], u [1], 1);
				int uSize = u.Count;
				//最も近い2クラスタを探索
				for (int i = 0; i < (uSize - 1); i++) {
					for (int j = i + 1; j < uSize; j++) {
						double tmpDis = DisC (u [i], u [j], 1);
						if (disBuf > tmpDis) {
							tmpi = i;
							tmpj = j;
							Console.Write (PrintProgBar(done/allSteps));
						}
						done++;
					}
				}
				//ここでtmpi番目のクラスタとtmpj番目のクラスタは全クラスタの中で
				//最も距離の近いクラスタ同士であるから、これらをマージする
				//また、これに従ってクラスタの全体集合uを定義し直す
				//具体的にはuから一旦tmpi, tmpj番目の要素を削除して、
				//それらをマージした新しいクラスを加える
				Cluster tmpC = MergeCluster (u [tmpi], u [tmpj]);
				//tmpi番目を削除
				u.RemoveAt (tmpi);
				//tmpi番目を削除したことにより、インデックスが一つずれる
				//なお、常にtmpi < tmpjである
				u.RemoveAt (tmpj - 1);
				u.Add (tmpC);
			}
			Console.WriteLine ("\nFinish Clustering!");
			return u;
		}

		//おまけ
		//プログレスバーを作成するメソッド
		//finはすべての計算処理のうち計算の完了した処理の割合
		private string PrintProgBar(double fin){
			int done = (int)(Math.Ceiling (fin * 100));
			if (done > 100) {
				done = 100;
			}
			int yet = 100 - done;
			string bar = "[ ";
			for(int i = 0; i < (done-1); i++){
				bar = string.Format (bar + "=");
			}
			for (int i = 0; i < yet; i++) {
				bar = string.Format (bar + " ");
			}
			bar = string.Format (bar + " ]" + done + "%");
			return string.Format ("\r\u001b\u005b\u004b" + bar);
		}
	}

	public class Cluster
	{
		//後の処理のためにベクトル部分をList<double>[]で、
		//ベクトル名部分をstring[]としてそれぞれをインデックスで対応させる
		//ベクトルの名前の配列
		private string[] vecNames;
		//ベクトル本体の配列
		private List<double>[] vecData;
		//クラスターサイズ(要素数)
		private int clusterSize;

		//コンストラクタ
		public Cluster (Dictionary<string, List<double>> data)
		{
			Data = data;	
			clusterSize = data.Count;
			vecNames = new string[clusterSize];
			vecData = new List<double>[clusterSize];
			int i = 0;
			foreach (KeyValuePair<string, List<double>> kvp in data) {
				vecNames [i] = kvp.Key;
				vecData [i] = kvp.Value;
				i++;
			}
		}

		//プロパティ
		//元のデータ
		public Dictionary<string, List<double>> Data{ get; protected set; }


		//クラスターのクラスタのサイズ(要素数)を返すメソッド
		public int ClusterSize(){
			return clusterSize;
		}
		//2つのList<double>で表されたベクトルの和をとるメソッド
		private List<double> AddVec (List<double> vec1, List<double> vec2)
		{
			List<double> res = new List<double> ();
			int vec1Count = vec1.Count;
			for (int i = 0; i < vec1Count; i++) {
				res [i] = vec1 [i] + vec2 [i];
			}
			return res;
		}
		//全ベクトルのi番目のベクトル名を取得するメソッド
		public string VecNameI (int i)
		{
			return vecNames [i];
		}
		//全ベクトルのi番目のベクトルの値を取得するメソッド
		public List<double> VecValI (int i)
		{
			return vecData [i];
		}
		//クラスタに含まれるベクトルの名前をstring[]で取得するメソッド
		public string[] VecNamesArr ()
		{
			return vecNames;
		}
		//クラスタの重心ベクトルを返すメソッド
		public List<double> GetClusterGravity(){
			//ベクトルの次元
			int dim = (vecData [0]).Count;
			List<double> g = new List<double> ();
			for (int i = 0; i < dim; i++) {
				g.Add (0);
			}
			for(int i = 0; i < dim; i++){
				double tmp = 0;
				foreach(List<double> v in vecData){
					tmp += tmp + v[i];
				}
				g[i] = tmp / clusterSize;
			}
			return g;
		}
	}
}

