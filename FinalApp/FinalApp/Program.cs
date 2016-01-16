using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections;
using TwitterOAuth;
using TwitterTweetJson;
using TwitterApi;
using WordVector;
using System.Linq;
using Classify;


//留意点
//* Newtonsoft.Json.dllはJSON.NETの外部ライブラリです.
//* TwiiterOAuth.csはTwitterAPIを使用する際にOAuth認証を通過するための
//メソッドやプロパティを揃えたクラスです
//* TwitterApi.csはTwitterAPIを使用するためのクラスで、
//認証はTwitterOAuth.cs内のAuthクラスを継承したTwitterConnectorクラスの
//インスタンスを使用することでパスすることができます.
//* TwitterJson.csはツイート情報のJSONオブジェクトをC#のクラスの形式にして書いたものです.

//最低限の実装なのでtry~catchの類は書いていません.
//存在しないTweetID等を入力すると当然エラーが発生します.

//反省
//もっとデータ構造を設計してからプログラムを書くべきだった
//例)ベクトルを名前と本体を対応させたDictinaryで処理しているが、
//ベクトルクラスのオブジェクトとして処理するような設計をすべきだった
namespace Main
{

	class Program
	{
		static void Main (string[] args)
		{
//			//リクエストトークン取得====================================================================
//			//ApiTestNoCallback
//			//callbackurl設定なしアカウント
			const string CONSUMER_KEY = "8369eInu3C2YzkAgx3YrWbXMJ";
			const string CONSUMER_SECRET = "8uWxX1iTTR1yxdA25RqlQxxn82tDipnsC5482UU3Etm3EpwQGn";
//
//			Auth auth = new Auth (CONSUMER_KEY, CONSUMER_SECRET);
//
//			// リクエストトークンを取得する
//			auth.GetRequestToken ();
//
//			//ユーザーにRequestTokenを認証してもらう
//			Console.WriteLine ("Please jump to the AuthenticationURL：");
//			Console.WriteLine ("https://api.twitter.com/oauth/authorize?oauth_token=" + auth.RequestToken);
//			Console.Write ("Enter your PIN:");
//			string pin = Console.ReadLine ();
//			// アクセストークンを取得する
//			auth.GetAccessToken (pin);
//
//			// 結果を表示する
//			Console.WriteLine ("Your AccessToken: " + auth.AccessToken);
//			Console.WriteLine ("Your AccessTokenSecret: " + auth.AccessTokenSecret);
//			Console.WriteLine ("Your UserId: " + auth.UserId);
//			Console.WriteLine ("Your ScreenName: " + auth.ScreenName);
//			//=====================================================================================

			//====================================================
			//debug
//			TwitterConnector tc = new TwitterConnector (
//				auth.ConsumerKey, auth.ConsumerSecret, auth.AccessToken, 
//				auth.AccessTokenSecret, auth.UserId, auth.ScreenName);
			//debug限定
			TwitterConnector tc = new TwitterConnector (
				                      CONSUMER_KEY, CONSUMER_SECRET, 
				                      "4452190160-WVQ7leuqf48yruElrdZtbl2IeEcPE4JbNUqBrnp",
				                      "hQvfQmZA13uNDEFWEf9HGb3GQflogJZlbWddnVqQQPaKP",
				                      "4452190160",
				                      "apiTestJohoSys");
			
			Console.WriteLine ("debug start! Enter return key!");
			Console.ReadKey (); 
			Console.Write ("<===== This is a tiny Twitter client on console =====>\n");
			//対象ユーザーのTwitterのFriendsListを100人まで取得します
			string tgtName = "shimizu111485";
			//string tgtName = "apiTestJohoSys";
			//階層クラスタリングアルゴリズムを実行する回数を指定します
			//この数が小さいほどFriedsListを細かく、大きいほど粗くクラスタリングします.
			int clusteringSteps = 80;

			List<UserEx> res = new List<UserEx> ();
			res = tc.GetFriendsList (tgtName);
			Dictionary<string, List<string>> allWordLists = new Dictionary<string, List<string>> ();
			foreach (UserEx u in res) {
				MakeWordList wl = new MakeWordList (u.Description);
				//wl.Analyはユーザーのプロフィール文から名詞を抜き出してList<string>としたもの.
				allWordLists.Add (u.ScreenName, wl.Analy ());
			}
			//allWrodListはクラスタリング対象の全てのユーザーのプロフィールから抽出した単語のリストList<List<string>>
			MakeWordVec wv = new MakeWordVec (allWordLists);
			//wv.MakeWordVecsList()はそれぞれのベクトル名と値を登録したDictionary<string, List<int>>を返す.
			ClassifyVector cv = new ClassifyVector (wv.MakeWordVecsList (), clusteringSteps);
			//cv.HierarchicalClustering()はCluster[]を返す
			foreach (Cluster c in cv.HierarchicalClustering()) {
				Console.Write ("{ ");
				foreach (string s in c.VecNamesArr()) {
					Console.Write ("@" + s + " ");
				}
				Console.Write ("}");
				Console.WriteLine ("\n\n");
			}
			//debug
			Console.WriteLine ("debug end! Enter return key!");
			Console.ReadKey ();
			//=====================================================
		}
	}
}
