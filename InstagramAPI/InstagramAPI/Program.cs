using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using Newtonsoft.Json;
using InstagramJsonUser;
using InstagramJsonMedia;
using InstagramJsonLocation;


//留意点
//2015年11月17日以降より
//app審査前の段階ではsandboxに招待されたユーザのコンテンツしか取得できなくなりました.
//実際に使用するには開発後に取得可能コンテンツのスコープにpublic_contentを
//追加申請する必要があります.
namespace ConsoleApplication1
{
	public class Program
	{
		static void Main ()
		{	
			//CLIENT ID 20ef08d527854613983922dac131c7cd
//			//アクセストークン取得（リダイレクト先のアドレスからコピー）
//			string getATUrl = "https://api.instagram.com/oauth/authorize/";
//			string getATParam = "?client_id=20ef08d527854613983922dac131c7cd&redirect_uri=http://www.yahoo.co.jp/&response_type=token";
//			string getATScope = "&scope=basic+likes+comments+relationships+public_content";
//			//このURLが認証画面のURL
//			string url = getATUrl + getATParam + getATScope;

			//ここからリダイレクトURLをパースしてアクセストークンを取得
			//実際のアドレスは（サンドボックス内でのデバッグ用/悪用厳禁）
			//https://api.instagram.com/oauth/authorize/?client_id=b440a5f469e040b487afbefdb433c279&redirect_uri=https://github.com/HiKat/jikkenJohoSys/blob/master/InstagramAPI/README.txt&response_type=token&scope=basic+likes+comments+relationships+public_content
			//取得したアクセストークンは（サンドボックス内でのデバッグ用/悪用厳禁）
			string accessToken = "588760295.b440a5f.2a69bec450c94e66b26dc1b1e078df0f";
			InstagramConnetor ic = new InstagramConnetor (accessToken);
			string myID = "588760295";
			double lat = 35.696668;
			double lng = 139.769054;
			int dis = 300;

			double lat2 = 34.691197;
			double lng2 = 135.516443;
			int dis2 = 3000;
			string myMediaId = "1066618951385260380_588760295";
			long myLocationId = 560621;

			ic.GetUsers (myID);
			ic.GetUsersSearch ("katsumi");
			ic.GetMedia (myMediaId);
			ic.GetMediaSearch (lat, lng, dis);
			ic.GetMediaSearch (lat2, lng2, dis2);
			ic.GetLocations (myLocationId);
			ic.GetLocationsMediaRecent (myLocationId);
			ic.GetLocationsSearch (lat, lng, dis);

			//debug
			Console.ReadKey ();
		}
	}

	public class InstagramConnetor
	{
		//============================================================================
		public InstagramConnetor (string accessToken)
		{
			AccessToken = accessToken;
		}

		public string AccessToken{ get; protected set; }

		//Getメソッドで送信を行う
		public string HttpGet (string url)
		{
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create (url);
			req.Method = "GET";
			req.ContentType = "application/x-www-form-urlencoded";
			HttpWebResponse res = (HttpWebResponse)req.GetResponse ();
			Stream resStream = res.GetResponseStream ();
			StreamReader sr = new StreamReader (resStream);
			//JSONデータを取得
			return sr.ReadToEnd ();
		}
		//===========================================================================


		//===========================================================================
		//get_users
		//IDで指定した任意のユーザについての情報を取得する
		public User GetUsers (string id)
		{
			string url = "https://api.instagram.com/v1/users/" + id + "/?access_token=" + AccessToken;
			//JSONデータを取得
			string result = HttpGet (url);
			//JSONデータのパース
			var root = JsonConvert.DeserializeObject<RootObjectOneUser> (result);
			User usr = new User (root.data.username, root.data.id);

			//debug
			Console.WriteLine ("username = " + root.data.username);
			Console.WriteLine ("id = " + root.data.id);
			Console.WriteLine ("");

			return usr;
		}
		//===========================================================================
		//get_users_search
		//クエリで検索したユーザについての情報を取得する
		public List<User> GetUsersSearch (string query)
		{
			string url = "https://api.instagram.com/v1/users/search?q=" + query + "&access_token=" + AccessToken;
			//JSONデータを取得
			string result = HttpGet (url);
			//JSONデータのパース
			var root = JsonConvert.DeserializeObject<RootObjectUserList> (result);
			List<User> resList = new List<User> ();
			foreach (InstagramJsonUser.Datum d in root.data) {
				User usr = new User (d.username, d.id);
				resList.Add (usr);

				//debug
				Console.WriteLine ("username = " + d.username);
				Console.WriteLine ("id = " + d.id);
				Console.WriteLine ("");
			}
			return resList;
		}
		//===========================================================================
		//get_media
		//指定したメディアidのコンテンツを取得
		//ビデオのidを指定した場合はJSONの形式が異なるためエラーが出るので注意
		public Media GetMedia (string id)
		{
			string url = "https://api.instagram.com/v1/media/" + id + "?access_token=" + AccessToken;
			//JSONデータを取得
			string result = HttpGet (url);
			//JSONデータのパース
			var root = JsonConvert.DeserializeObject<RootObjectOneMedia> (result);
			Media media = new Media (root.data.link, root.data.id);

			//debug
			Console.WriteLine ("link = " + root.data.link);
			Console.WriteLine ("id = " + root.data.id);
			Console.WriteLine ("");

			return media;
		}
		//===========================================================================
		//get_media_search
		//緯度経度で指定した場所周辺でアップロードされたコンテンツを取得する
		public List<Media> GetMediaSearch (double lat, double lng, int distance)
		{
			string url = 
				"https://api.instagram.com/v1/media/search?lat=" + lat.ToString () + "&lng=" + lng.ToString () +
				"&distance=" + distance.ToString () + "&access_token=" + AccessToken;
			//JSONデータを取得
			string result = HttpGet (url);
			var root = JsonConvert.DeserializeObject<RootObjectMediaList> (result);
			List<Media> resList = new List<Media> ();
			foreach (InstagramJsonMedia.Datum d in root.data) {
				Media media = new Media (d.link, d.id);
				resList.Add (media);

				//debug
				Console.WriteLine ("link = " + d.link);
				Console.WriteLine ("id = " + d.id);
				Console.WriteLine ("");
			}
			return resList;
		}
		//===========================================================================
		//get_locations
		//location_idで指定した場所に関する情報を取得する
		public Location GetLocations (long id)
		{
			string url = "https://api.instagram.com/v1/locations/" + id + "?access_token=" + AccessToken;
			//JSONデータを取得
			string result = HttpGet (url);
			var root = JsonConvert.DeserializeObject<RootObjectOneLocation> (result);
			Location loc = new Location (root.data.latitude, root.data.longitude, root.data.name, root.data.id);

			//debug
			Console.WriteLine ("lat = " + root.data.latitude);
			Console.WriteLine ("lng = " + root.data.longitude);
			Console.WriteLine ("name = " + root.data.name);
			Console.WriteLine ("id = " + root.data.id);
			Console.WriteLine ("");

			return loc;
		}
		//===========================================================================
		//get_locations_media_recent
		//location_idで指定した場所でアップロードされたコンテンツを取得する
		public List<Media> GetLocationsMediaRecent (long id)
		{
			string url = "https://api.instagram.com/v1/locations/" + id + "/media/recent?access_token=" + AccessToken;
			//JSONデータを取得
			string result = HttpGet (url);
			var root = JsonConvert.DeserializeObject<RootObjectMediaListGLMR> (result);
			List<Media> resList = new List<Media> ();
			foreach (InstagramJsonMedia.Datum d in root.data) {
				Media media = new Media (d.link, d.id);
				resList.Add (media);

				//debug
				Console.WriteLine ("link = " + d.link);
				Console.WriteLine ("id = " + d.id);
				Console.WriteLine ("");
			}
			return resList;
		}
		//===========================================================================
		//get_locations_search
		//緯度軽度で指定した場所周辺につけられたidやその他の情報を取得する
		public List<Location> GetLocationsSearch (double lat, double lng, int distance)
		{
			string url = 
				"https://api.instagram.com/v1/locations/search?lat=" + lat.ToString () +
				"&lng=" + lng.ToString () + "&distance=" + distance.ToString () + "&access_token=" + AccessToken;
			//JSONデータを取得
			string result = HttpGet (url);
			var root = JsonConvert.DeserializeObject<RootObjectLocationList> (result);
			List<Location> resList = new List<Location> ();
			foreach (InstagramJsonLocation.Datum d in root.data) {
				Location loc = new Location (d.latitude, d.longitude, d.name, d.id);

				//debug
				Console.WriteLine ("lat = " + d.latitude);
				Console.WriteLine ("lng = " + d.longitude);
				Console.WriteLine ("name = " + d.name);
				Console.WriteLine ("id = " + d.id);
				Console.WriteLine ("");
			}
			return resList;
		}
		//===========================================================================
	}

	//ユーザー情報を格納
	public class User
	{
		public User (string userName, string id)
		{
			UserName = userName;
			UserId = id;
		}
		//アカウント名
		public string UserName{ get; protected set; }
		//ユーザーID
		public string UserId{ get; protected set; }
	}

	//メディア情報を格納
	public class Media
	{
		public Media (string link, string id)
		{
			Link = link;
			Id = id;
		}
		//アドレス
		public string Link{ get; protected set; }
		//メディアID
		public string Id{ get; protected set; }
	}

	//ロケーション情報を格納
	public class Location
	{
		public Location (double lat, double lng, string name, string id)
		{
			Latitude = lat;
			Longitude = lng;
			Name = name;
			Id = id;
		}
		//緯度
		public double Latitude{ get; protected set; }
		//経度
		public double Longitude{ get; protected set; }
		//場所名
		public string Name{ get; protected set; }
		//ロケーションID
		public string Id{ get; protected set; }
	}
}

