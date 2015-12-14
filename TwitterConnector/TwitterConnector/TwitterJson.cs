using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TwitterUserTimelineJson
{

	public class Description
	{
		public List<object> urls { get; set; }
	}

	public class Entities
	{
		public Description description { get; set; }
	}

	//ユーザー情報
	public class User
	{
		public object id { get; set; }
		public string id_str { get; set; }
		//ユーザー名
		public string name { get; set; }
		//スクリーンネーム
		public string screen_name { get; set; }
		public string location { get; set; }
		public string description { get; set; }
		public object url { get; set; }
		public Entities entities { get; set; }
		public bool @protected { get; set; }
		public int followers_count { get; set; }
		public int friends_count { get; set; }
		public int listed_count { get; set; }
		public string created_at { get; set; }
		public int favourites_count { get; set; }
		public object utc_offset { get; set; }
		public object time_zone { get; set; }
		public bool geo_enabled { get; set; }
		public bool verified { get; set; }
		public int statuses_count { get; set; }
		public string lang { get; set; }
		public bool contributors_enabled { get; set; }
		public bool is_translator { get; set; }
		public bool is_translation_enabled { get; set; }
		public string profile_background_color { get; set; }
		public string profile_background_image_url { get; set; }
		public string profile_background_image_url_https { get; set; }
		public bool profile_background_tile { get; set; }
		public string profile_image_url { get; set; }
		public string profile_image_url_https { get; set; }
		public string profile_banner_url { get; set; }
		public string profile_link_color { get; set; }
		public string profile_sidebar_border_color { get; set; }
		public string profile_sidebar_fill_color { get; set; }
		public string profile_text_color { get; set; }
		public bool profile_use_background_image { get; set; }
		public bool has_extended_profile { get; set; }
		public bool default_profile { get; set; }
		public bool default_profile_image { get; set; }
		public bool following { get; set; }
		public bool follow_request_sent { get; set; }
		public bool notifications { get; set; }
	}

	public class Description2
	{
		public List<object> urls { get; set; }
	}

	public class Url2
	{
		public string url { get; set; }
		public string expanded_url { get; set; }
		public string display_url { get; set; }
		public List<int> indices { get; set; }
	}

	public class Url
	{
		public List<Url2> urls { get; set; }
	}

	public class Entities2
	{
		public Description2 description { get; set; }
		public Url url { get; set; }
	}

	public class User2
	{
		public long id { get; set; }
		public string id_str { get; set; }
		public string name { get; set; }
		public string screen_name { get; set; }
		public string location { get; set; }
		public string description { get; set; }
		public string url { get; set; }
		public Entities2 entities { get; set; }
		public bool @protected { get; set; }
		public int followers_count { get; set; }
		public int friends_count { get; set; }
		public int listed_count { get; set; }
		public string created_at { get; set; }
		public int favourites_count { get; set; }
		public int? utc_offset { get; set; }
		public string time_zone { get; set; }
		public bool geo_enabled { get; set; }
		public bool verified { get; set; }
		public int statuses_count { get; set; }
		public string lang { get; set; }
		public bool contributors_enabled { get; set; }
		public bool is_translator { get; set; }
		public bool is_translation_enabled { get; set; }
		public string profile_background_color { get; set; }
		public string profile_background_image_url { get; set; }
		public string profile_background_image_url_https { get; set; }
		public bool profile_background_tile { get; set; }
		public string profile_image_url { get; set; }
		public string profile_image_url_https { get; set; }
		public string profile_banner_url { get; set; }
		public string profile_link_color { get; set; }
		public string profile_sidebar_border_color { get; set; }
		public string profile_sidebar_fill_color { get; set; }
		public string profile_text_color { get; set; }
		public bool profile_use_background_image { get; set; }
		public bool has_extended_profile { get; set; }
		public bool default_profile { get; set; }
		public bool default_profile_image { get; set; }
		public bool following { get; set; }
		public bool follow_request_sent { get; set; }
		public bool notifications { get; set; }
	}

	public class Small
	{
		public int w { get; set; }
		public int h { get; set; }
		public string resize { get; set; }
	}

	public class Medium2
	{
		public int w { get; set; }
		public int h { get; set; }
		public string resize { get; set; }
	}

	public class Large
	{
		public int w { get; set; }
		public int h { get; set; }
		public string resize { get; set; }
	}

	public class Thumb
	{
		public int w { get; set; }
		public int h { get; set; }
		public string resize { get; set; }
	}

	public class Sizes
	{
		public Small small { get; set; }
		public Medium2 medium { get; set; }
		public Large large { get; set; }
		public Thumb thumb { get; set; }
	}

	public class Medium
	{
		public object id { get; set; }
		public string id_str { get; set; }
		public List<int> indices { get; set; }
		public string media_url { get; set; }
		public string media_url_https { get; set; }
		public string url { get; set; }
		public string display_url { get; set; }
		public string expanded_url { get; set; }
		public string type { get; set; }
		public Sizes sizes { get; set; }
	}

	public class Entities3
	{
		public List<object> hashtags { get; set; }
		public List<object> symbols { get; set; }
		public List<object> user_mentions { get; set; }
		public List<object> urls { get; set; }
		public List<Medium> media { get; set; }
	}

	public class Small2
	{
		public int w { get; set; }
		public int h { get; set; }
		public string resize { get; set; }
	}

	public class Medium4
	{
		public int w { get; set; }
		public int h { get; set; }
		public string resize { get; set; }
	}

	public class Large2
	{
		public int w { get; set; }
		public int h { get; set; }
		public string resize { get; set; }
	}

	public class Thumb2
	{
		public int w { get; set; }
		public int h { get; set; }
		public string resize { get; set; }
	}

	public class Sizes2
	{
		public Small2 small { get; set; }
		public Medium4 medium { get; set; }
		public Large2 large { get; set; }
		public Thumb2 thumb { get; set; }
	}

	public class Medium3
	{
		public object id { get; set; }
		public string id_str { get; set; }
		public List<int> indices { get; set; }
		public string media_url { get; set; }
		public string media_url_https { get; set; }
		public string url { get; set; }
		public string display_url { get; set; }
		public string expanded_url { get; set; }
		public string type { get; set; }
		public Sizes2 sizes { get; set; }
	}

	public class ExtendedEntities
	{
		public List<Medium3> media { get; set; }
	}

	public class RetweetedStatus
	{
		public string created_at { get; set; }
		public object id { get; set; }
		public string id_str { get; set; }
		public string text { get; set; }
		public string source { get; set; }
		public bool truncated { get; set; }
		public object in_reply_to_status_id { get; set; }
		public object in_reply_to_status_id_str { get; set; }
		public object in_reply_to_user_id { get; set; }
		public object in_reply_to_user_id_str { get; set; }
		public object in_reply_to_screen_name { get; set; }
		public User2 user { get; set; }
		public object geo { get; set; }
		public object coordinates { get; set; }
		public object place { get; set; }
		public object contributors { get; set; }
		public bool is_quote_status { get; set; }
		public int retweet_count { get; set; }
		public int favorite_count { get; set; }
		public Entities3 entities { get; set; }
		public ExtendedEntities extended_entities { get; set; }
		public bool favorited { get; set; }
		public bool retweeted { get; set; }
		public bool possibly_sensitive { get; set; }
		public string lang { get; set; }
	}

	public class UserMention
	{
		public string screen_name { get; set; }
		public string name { get; set; }
		public long id { get; set; }
		public string id_str { get; set; }
		public List<int> indices { get; set; }
	}

	public class Small3
	{
		public int w { get; set; }
		public int h { get; set; }
		public string resize { get; set; }
	}

	public class Medium6
	{
		public int w { get; set; }
		public int h { get; set; }
		public string resize { get; set; }
	}

	public class Large3
	{
		public int w { get; set; }
		public int h { get; set; }
		public string resize { get; set; }
	}

	public class Thumb3
	{
		public int w { get; set; }
		public int h { get; set; }
		public string resize { get; set; }
	}

	public class Sizes3
	{
		public Small3 small { get; set; }
		public Medium6 medium { get; set; }
		public Large3 large { get; set; }
		public Thumb3 thumb { get; set; }
	}

	public class Medium5
	{
		public object id { get; set; }
		public string id_str { get; set; }
		public List<int> indices { get; set; }
		public string media_url { get; set; }
		public string media_url_https { get; set; }
		public string url { get; set; }
		public string display_url { get; set; }
		public string expanded_url { get; set; }
		public string type { get; set; }
		public Sizes3 sizes { get; set; }
		public object source_status_id { get; set; }
		public string source_status_id_str { get; set; }
		public long source_user_id { get; set; }
		public string source_user_id_str { get; set; }
	}

	public class Entities4
	{
		public List<object> hashtags { get; set; }
		public List<object> symbols { get; set; }
		public List<UserMention> user_mentions { get; set; }
		public List<object> urls { get; set; }
		public List<Medium5> media { get; set; }
	}

	public class Small4
	{
		public int w { get; set; }
		public int h { get; set; }
		public string resize { get; set; }
	}

	public class Medium8
	{
		public int w { get; set; }
		public int h { get; set; }
		public string resize { get; set; }
	}

	public class Large4
	{
		public int w { get; set; }
		public int h { get; set; }
		public string resize { get; set; }
	}

	public class Thumb4
	{
		public int w { get; set; }
		public int h { get; set; }
		public string resize { get; set; }
	}

	public class Sizes4
	{
		public Small4 small { get; set; }
		public Medium8 medium { get; set; }
		public Large4 large { get; set; }
		public Thumb4 thumb { get; set; }
	}

	public class Medium7
	{
		public object id { get; set; }
		public string id_str { get; set; }
		public List<int> indices { get; set; }
		public string media_url { get; set; }
		public string media_url_https { get; set; }
		public string url { get; set; }
		public string display_url { get; set; }
		public string expanded_url { get; set; }
		public string type { get; set; }
		public Sizes4 sizes { get; set; }
		public object source_status_id { get; set; }
		public string source_status_id_str { get; set; }
		public object source_user_id { get; set; }
		public string source_user_id_str { get; set; }
	}

	public class ExtendedEntities2
	{
		public List<Medium7> media { get; set; }
	}

	//ツイート本体
	public class RootObject
	{
		public string created_at { get; set; }
		//ツイートID
		public object id { get; set; }
		public string id_str { get; set; }
		//ツイート内容
		public string text { get; set; }
		public string source { get; set; }
		public bool truncated { get; set; }
		public object in_reply_to_status_id { get; set; }
		public object in_reply_to_status_id_str { get; set; }
		public object in_reply_to_user_id { get; set; }
		public object in_reply_to_user_id_str { get; set; }
		public object in_reply_to_screen_name { get; set; }
		//ツイートユーザー情報
		public User user { get; set; }
		public object geo { get; set; }
		public object coordinates { get; set; }
		public object place { get; set; }
		public object contributors { get; set; }
		public RetweetedStatus retweeted_status { get; set; }
		public bool is_quote_status { get; set; }
		public int retweet_count { get; set; }
		public int favorite_count { get; set; }
		public Entities4 entities { get; set; }
		public ExtendedEntities2 extended_entities { get; set; }
		public bool favorited { get; set; }
		public bool retweeted { get; set; }
		public bool possibly_sensitive { get; set; }
		public string lang { get; set; }
	}

}

