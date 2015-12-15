using System;
using System.Text;
using TwitterOAuth;
using TwitterTweetJson;
using TwitterApi;
//留意点
//* Newtonsoft.Json.dllはJSON.NETの外部ライブラリです.
//* TwiiterOAuth.csはTwitterAPIを使用する際にOAuth認証を通過するための
//メソッドやプロパティを揃えたクラスです
//* TwitterApi.csはTwitterAPIを使用するためのクラスで、
//認証はTwitterOAuth.cs内のAuthクラスを継承したTwitterConnectorクラスの
//インスタンスを使用することでパスすることができます.
//* TwitterJson.csはツイート情報のJSONオブジェクトをC#のクラスの形式にして書いたものです.

//これはコンソール上で動作する簡単なTwitterクライアントです
//最低限の実装なのでtry~catchの類は書いていません.
//存在しないTweetID等を入力すると当然エラーが発生します.
namespace Main
{
	
	class Program
	{
		static void Main (string[] args)
		{
			//リクエストトークン取得====================================================================
			//ApiTestNoCallback
			//callbackurl設定なしアカウント
			const string CONSUMER_KEY = "8369eInu3C2YzkAgx3YrWbXMJ";
			const string CONSUMER_SECRET = "8uWxX1iTTR1yxdA25RqlQxxn82tDipnsC5482UU3Etm3EpwQGn";

			Auth auth = new Auth (CONSUMER_KEY, CONSUMER_SECRET);

			// リクエストトークンを取得する
			auth.GetRequestToken ();

			//ユーザーにRequestTokenを認証してもらう
			Console.WriteLine ("Please jump to the AuthenticationURL：");
			Console.WriteLine ("https://api.twitter.com/oauth/authorize?oauth_token=" + auth.RequestToken);
			Console.Write ("Enter your PIN:");
			string pin = Console.ReadLine ();
			// アクセストークンを取得する
			auth.GetAccessToken (pin);

			// 結果を表示する
			Console.WriteLine ("Your AccessToken: " + auth.AccessToken);
			Console.WriteLine ("Your AccessTokenSecret: " + auth.AccessTokenSecret);
			Console.WriteLine ("Your UserId: " + auth.UserId);
			Console.WriteLine ("Your ScreenName: " + auth.ScreenName);
			//=====================================================================================

			//====================================================
			//debug
			Console.WriteLine ("debug start! Enter return key!");
			Console.ReadKey (); 
			Console.Write ("<===== This is a tiny Twitter client on console =====>\n");

			TwitterConnector tc = new TwitterConnector (
				                      auth.ConsumerKey, auth.ConsumerSecret, auth.AccessToken, 
				                      auth.AccessTokenSecret, auth.UserId, auth.ScreenName);
			
			Console.WriteLine ("Enter some screen name you want to get its Tweets");
			Console.WriteLine ("e.g)@:muji_net");
			Console.Write ("@:"); 
			string sn = Console.ReadLine ();
			tc.GetUsrTimeline (sn);
			Console.WriteLine ("=====================================================================");
			Console.WriteLine ("Completed getting his Tweets!");
			Console.WriteLine ("Enter return key!\n");
			Console.ReadKey ();

			Console.WriteLine ("Please enter return key to start to get your home timeline");
			Console.ReadKey ();
			tc.GetHomeTimeline ();
			Console.WriteLine ("=====================================================================");
			Console.WriteLine ("Completed getting your home timeline!");
			Console.WriteLine ("Enter return key!\n");
			Console.ReadKey ();

			Console.WriteLine ("Enter some tweet id you want to get");
			Console.WriteLine ("e.g)id:669840927989456896");
			Console.Write ("tweet id:");
			string id = Console.ReadLine ();
			tc.GetTweet (long.Parse(id));
			Console.WriteLine ("=====================================================================");
			Console.WriteLine ("Completed getting tweet from tweet id!");
			Console.WriteLine ("Enter return key!\n");
			Console.ReadKey ();

			Console.WriteLine ("Please enter return key to start to get your mentions timeline");
			Console.ReadKey ();
			tc.GetMentionsTimeline ();
			Console.WriteLine ("=====================================================================");
			Console.WriteLine ("Completed getting your mentions timeline!");
			Console.WriteLine ("Enter return key!\n");
			Console.ReadKey ();

			Console.WriteLine ("Please enter some text to post. (Caution!: This text will post your time line!)");
			Console.Write ("text:");
			Console.ReadKey (); 
			string text = Console.ReadLine ();
			tc.Update (text);
			Console.WriteLine ("=====================================================================");
			Console.WriteLine ("Completed posting!");
			Console.WriteLine ("Enter return key!\n");
			Console.ReadKey ();

			//debug
			Console.WriteLine ("debug end! Enter return key!");
			Console.ReadKey ();
			//=====================================================
		}
	}
}