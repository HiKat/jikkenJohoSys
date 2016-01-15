using System;
using System.Collections.Generic;

namespace Classify
{
	//Dictinary<string:ベクトル名, List<int>:ベクトル本体>からなるベクトルの集合を
	//階層的クラスタリングに基づいてクラスタリングするためのクラス
	public class ClassifyVector
	{
		//後の処理のためにベクトル部分をList<int>[]で、
		//ベクトル名部分をstring[]としてそれぞれをインデックスで対応させる
		//ベクトルの名前の配列
		private string[] vecNames;
		//ベクトル本体の配列
		private List<int>[] vecData;

		//コンストラクタ
		//Dictionary<string, List<int>>:ベクトル集合
		//int:クラスタリング操作の回数
		public ClassifyVector (Dictionary<string, List<int>> vectors, int steps)
		{
			Vectors = vectors;
			Steps = steps;
			vecNames = new string[Vectors.Count];
			vecData = new List<int>[Vectors.Count];
			int i = 0;
			foreach (KeyValuePair<string, List<int>> kvp in Vectors) {
				vecNames [i] = kvp.Key;
				vecData [i] = kvp.Value;
				i++;
			}
		}

		//プロパティ
		//元となったベクトル名とベクトルデータ本体が格納されたDictionary
		public Dictionary<string, List<int>> Vectors{ get; protected set; }
		//階層クラスタリングの実行回数
		public int Steps{ get; protected set; }
		//全ベクトルのi番目のベクトル名を取得するメソッド
		public string VecNameI (int i)
		{
			return vecNames [i];
		}
		//全ベクトルのi番目のベクトルの値を取得するメソッド
		public List<int> VecValI (int i)
		{
			return vecData [i];
		}
	
		//2ベクトル間の距離を計算するメソッド
		//ユークリッド距離を用いる
		private double DisVec (List<int> vec1, List<int> vec2)
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
		//ユークリッド距離による群平均距離を用いて算出する
		//群平均距離はクラスタ内のベクトル同士の距離を全て計算して平均を取る
		private double DisC (Cluster c1, Cluster c2)
		{
			double tmp = 0;
			for (int i = 0; i < c1.ClusterSize; i++) {
				for (int j = 0; j < c2.ClusterSize; j++){
					tmp += DisVec (c1.VecValI (i), c2.VecValI (j));
				}
			}
			double result = tmp / (c1.ClusterSize * c2.ClusterSize);
			return result;
		}

		//2つのクラスタをマージして1つのクラスタにするメソッド
		public Cluster MergeCluster(Cluster c1, Cluster c2){
			Dictionary<string, List<int>> res = new Dictionary<string, List<int>> (c1.ClusterSize + c2.ClusterSize);
			//クラスタ1を追加
			foreach(KeyValuePair<string, List<int>> kvp in c1.Data){
				Console.WriteLine ("c1 >" + kvp.Key);
				res.Add (kvp.Key, kvp.Value);
			}
			//クラスタ2を追加
			foreach(KeyValuePair<string, List<int>> kvp in c2.Data){
				Console.WriteLine ("c2 >" + kvp.Key);
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
		public Cluster[] HierarchicalClustering ()
		{
			//まず、各ベクトル単体からなるクラスタを用意する
			//uは全クラスタの集合
			//クラスタ総数(入力ベクトル総数)
			int numClusters = Vectors.Count;
			Cluster[] u = new Cluster[numClusters];
			for (int i = 0; i < numClusters; i++) {
				Dictionary<string, List<int>> dic = new Dictionary<string, List<int>> ();
				dic.Add (vecNames [i], vecData [i]);
				Cluster cl = new Cluster (dic);
				u [i] = cl;
			}
				
			//全クラスタの組み合わせの中から最も距離が近いクラスタをマージして新しいクラスタとする.
			//探索中で最も近くにあるベクトルの名前の組と距離を逐次記憶、更新する
			for(int s = 0; s < Steps; s++){
				int tmpi = 0;
				int tmpj = 0;
				double disBuf = 0;
				bool first = true;
				//最も近い2クラスタを探索
				for (int i = 0; i < numClusters; i++) {
					for (int j = i + 1; j < numClusters; j++) {
						double tmpDis = DisC (u[i], u[j]);
						if (first == true) {
							disBuf = tmpDis;
							tmpi = i;
							tmpj = j;
							first = false;
						} else {
							if (disBuf > tmpDis) {
								disBuf = tmpDis;
								tmpi = i;
								tmpj = j;
							} else {
							}
						}
					}
				}
				//ここでtmpi番目のクラスタとtmpj番目のクラスタは全クラスタの中で
				//最も距離の近いクラスタ同士であるから、これらをマージする
				//また、これに従ってクラスタの全体集合uを定義し直す
				int uJ = 0;
				for(int i = 0;i < (numClusters - 1); i++){
					//i番目のクラスタを新しいuの要素とするかを判別
					//tmpi番目とtmpj番目のクラスタは飛ばす
					if(i == tmpi || i == tmpj){
						continue;
					}
					//追加する場合は先頭からj番目のクラスタとして登録し直す
					else{
						u[uJ] = u[i];
						uJ++;
					}
				}
				//最後にマージしたクラスを追加してクラスタの全集合の再定義が完了する
				u[uJ] = MergeCluster(u[tmpi], u[tmpj]);
			}
			return u;
		}
	}

	public class Cluster
	{
		//後の処理のためにベクトル部分をList<int>[]で、
		//ベクトル名部分をstring[]としてそれぞれをインデックスで対応させる
		//ベクトルの名前の配列
		private string[] vecNames;
		//ベクトル本体の配列
		private List<int>[] vecData;

		//コンストラクタ
		public Cluster (Dictionary<string, List<int>> data)
		{
			Data = data;	
			ClusterSize = data.Count;
			vecNames = new string[data.Count];
			vecData = new List<int>[data.Count];
			int i = 0;
			foreach (KeyValuePair<string, List<int>> kvp in data) {
				vecNames [i] = kvp.Key;
				vecData [i] = kvp.Value;
				i++;
			}

		}

		//プロパティ
		//元のデータ
		public Dictionary<string, List<int>> Data{ get; protected set; }
		//クラスタのサイズ(要素数)を取得するプロパティ
		public int ClusterSize{get; protected set;}

		//全ベクトルのi番目のベクトル名を取得するメソッド
		public string VecNameI (int i)
		{
			return vecNames [i];
		}
		//全ベクトルのi番目のベクトルの値を取得するメソッド
		public List<int> VecValI (int i)
		{
			return vecData [i];
		}
		//クラスタに含まれるベクトルの名前をstring[]で取得するメソッド
		public string[] VecNamesArr(){
			return vecNames;
		}
	}
}

