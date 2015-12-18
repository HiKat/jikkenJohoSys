using System;
using Newtonsoft.Json;
using System.Collections.Generic;

//ユーザーデータ
namespace InstagramJsonUser
{
	public class Meta
	{
		public int code { get; set; }
	}

	public class Counts
	{
		public int media { get; set; }
		public int followed_by { get; set; }
		public int follows { get; set; }
	}

	public class Data
	{
		public string username { get; set; }
		public string bio { get; set; }
		public string website { get; set; }
		public string profile_picture { get; set; }
		public string full_name { get; set; }
		public Counts counts { get; set; }
		public string id { get; set; }
	}

	public class Datum
	{
		public string username { get; set; }
		public string bio { get; set; }
		public string website { get; set; }
		public string profile_picture { get; set; }
		public string full_name { get; set; }
		public string id { get; set; }
	}

	//単一ユーザのみが返ってくる場合
	public class RootObjectOneUser
	{
		public Meta meta { get; set; }
		public Data data { get; set; }
	}

	//ユーザのリストが返ってくる場合
	public class RootObjectUserList
	{
		public Meta meta { get; set; }
		public List<Datum> data { get; set; }
	}
}

//メディアデータ
namespace InstagramJsonMedia
{
	public class Meta
	{
		public int code { get; set; }
	}

	public class Location
	{
		public double latitude { get; set; }
		public string name { get; set; }
		public double longitude { get; set; }
		public int id { get; set; }
	}

	public class Comments
	{
		public int count { get; set; }
	}

	public class Likes
	{
		public int count { get; set; }
	}

	public class LowResolution
	{
		public string url { get; set; }
		public int width { get; set; }
		public int height { get; set; }
	}

	public class Thumbnail
	{
		public string url { get; set; }
		public int width { get; set; }
		public int height { get; set; }
	}

	public class StandardResolution
	{
		public string url { get; set; }
		public int width { get; set; }
		public int height { get; set; }
	}

	public class Images
	{
		public LowResolution low_resolution { get; set; }
		public Thumbnail thumbnail { get; set; }
		public StandardResolution standard_resolution { get; set; }
	}

	public class User
	{
		public string username { get; set; }
		public string profile_picture { get; set; }
		public string id { get; set; }
		public string full_name { get; set; }
	}

	public class Data
	{
		public object attribution { get; set; }
		public List<object> tags { get; set; }
		public string type { get; set; }
		public Location location { get; set; }
		public Comments comments { get; set; }
		public string filter { get; set; }
		public string created_time { get; set; }
		public string link { get; set; }
		public Likes likes { get; set; }
		public Images images { get; set; }
		public List<object> users_in_photo { get; set; }
		public object caption { get; set; }
		public bool user_has_liked { get; set; }
		public string id { get; set; }
		public User user { get; set; }
	}

	public class Datum
	{
		public object attribution { get; set; }
		public List<object> tags { get; set; }
		public string type { get; set; }
		public Location location { get; set; }
		public Comments comments { get; set; }
		public string filter { get; set; }
		public string created_time { get; set; }
		public string link { get; set; }
		public Likes likes { get; set; }
		public Images images { get; set; }
		public List<object> users_in_photo { get; set; }
		public object caption { get; set; }
		public bool user_has_liked { get; set; }
		public string id { get; set; }
		public User user { get; set; }
	}

	public class RootObjectOneMedia
	{
		public Meta meta { get; set; }
		public Data data { get; set; }
	}

	public class RootObjectMediaList
	{
		public Meta meta { get; set; }
		public List<Datum> data { get; set; }
	}

	//get_locations_media_recent(GLMR)のとき===============
	public class Pagination{}
	public class RootObjectMediaListGLMR
	{
		public Pagination pagination { get; set; }
		public Meta meta { get; set; }
		public List<Datum> data { get; set; }
	}
	//==============================================

}

//ロケーション情報
namespace InstagramJsonLocation
{
	public class Meta
	{
		public int code { get; set; }
	}

	public class Data
	{
		public double latitude { get; set; }
		public string id { get; set; }
		public double longitude { get; set; }
		public string name { get; set; }
	}


	public class Datum
	{
		public double latitude { get; set; }
		public string id { get; set; }
		public double longitude { get; set; }
		public string name { get; set; }
	}

	public class RootObjectOneLocation
	{
		public Meta meta { get; set; }
		public Data data { get; set; }
	}

	public class RootObjectLocationList
	{
		public Meta meta { get; set; }
		public List<Datum> data { get; set; }
	}
}
