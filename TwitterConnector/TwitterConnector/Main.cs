using System;
//using System.Text;
//using System.Collections;
//using System.Collections.Generic;
//using System.Net.Http;
using TwitterOAuth;
using TwitterTweetJson;
using TwitterApi;

namespace Main
{
	
	class Program
	{
		static void Main (string[] args)
		{
//			//リクエストトークン取得====================================================================
//			//ApiTestNoCallback
//			//callbackurl設定なしアカウント
//			const string CONSUMER_KEY = "8369eInu3C2YzkAgx3YrWbXMJ";
//			const string CONSUMER_SECRET = "8uWxX1iTTR1yxdA25RqlQxxn82tDipnsC5482UU3Etm3EpwQGn";
//
//			Auth auth = new Auth (CONSUMER_KEY, CONSUMER_SECRET);
//
//			// リクエストトークンを取得する
//			auth.GetRequestToken ();
//			Console.WriteLine ("RequestToken: " + auth.RequestToken);
//			Console.WriteLine ("RequestTokenSecret: " + auth.RequestTokenSecret);
//
//			//ユーザーにRequestTokenを認証してもらう
//			Console.WriteLine ("Authentication URL：");
//			Console.WriteLine ("https://api.twitter.com/oauth/authorize?oauth_token=" + auth.RequestToken);
//			Console.Write ("PIN：");
//			string pin = Console.ReadLine ().Trim ();
//			// アクセストークンを取得する
//			auth.GetAccessToken (pin);
//
//			// 結果を表示する
//			Console.WriteLine ("AccessToken: " + auth.AccessToken);
//			Console.WriteLine ("AccessTokenSecret: " + auth.AccessTokenSecret);
//			Console.WriteLine ("UserId: " + auth.UserId);
//			Console.WriteLine ("ScreenName: " + auth.ScreenName);
//			//=====================================================================================


			//デバッグ用================================================================================
			const string CONSUMER_KEY = "IKBVUl9gifsSDrqEOeGBYEv3c";
			const string CONSUMER_SECRET = "6qqHzsX24YiyRyKHfeOuV4vAdPobKkPvChxgid07QTQJsnfapu";
			const string ACCESS_TOKEN = "4452190160-YwOa1h9VQUNRRYhp2MMMXMDssSyeEO9DcsDgydQ";
			const string ACCESS_TOKEN_SECRET = "YYT4zmqKMd7w6W0k2SulA5yHPByQ2KCFAmRK1NFYetfIk";
			const string USER_ID = "4452190160";
			const string SCREEN_NAME = "apiTestJohoSys";
		
			Auth auth = new Auth (
				            CONSUMER_KEY,
				            CONSUMER_SECRET,
				            ACCESS_TOKEN,
				            ACCESS_TOKEN_SECRET,
				            USER_ID,
				            SCREEN_NAME
			            );
			//=======================================================================================





			//====================================================
			//debug
			Console.WriteLine ("debug start! Enter return key!");
			Console.ReadKey ();

			//auth.GetTimeLine ();

			//タイムライン取得例

			TwitterConnector tc = new TwitterConnector (
				                      auth.ConsumerKey, auth.ConsumerSecret, auth.AccessToken, 
				                      auth.AccessTokenSecret, auth.UserId, auth.ScreenName);
			var usrTl = tc.GetUsrTimeline ("nanjolno");
			//var homeTl = tc.GetHomeTimeline ();
			//var mentions = tc.GetMentionsTimeline ();
			var idTweet = tc.GetTweet (674435882917584896);

			//debug
			Console.WriteLine ("debug end! Enter return key!");
			Console.ReadKey ();
			//=====================================================



			//debug
			Console.ReadKey ();
		}
	}
}