using System;
using System.Collections.Generic;

namespace Raydreams.Common.Services.Config
{
	/// <summary>A list of words that are bad in generating letter sequence</summary>
	public class ProfanityDictionary
	{
		/// <summary>All the words in the list</summary>
		public static List<String> All
		{
			get
			{
				List<String> all = new List<String>(VulgarWords);
				all.AddRange(ReservedWords);
				all.AddRange(OffensiveWords);
				return all;
			}
		}

		public static readonly string[] ReservedWords = new string[] { "jesus", "allah", "usa", "john", "gun", "kill", "amen", "moses", "admin", "god" };

		public static readonly string[] OffensiveWords = new string[] { "nigga", "niga", "jew","spic","milf","bang",
			"meat","chinc","chink","coon","suck","sucit","suxit","kike","dago","cajun","gook", "gouk", "honky", "injun",
			"kafir","kraut","negro","mammy","spade","moist","wet", "slut", "whore", "jerk", "jack", "dumb", "oily", "wad", "hate", "kkk", "sick", "sikh", "antifa" };

		public static readonly string[] VulgarWords = new String[] { "damn", "cock", "cunt", "dick", "shit",
			"ass", "fuck", "fuk","fuc", "prick", "bitch", "hell", "tit", "shite", "perv", "scum",
			"penis", "pussy", "clit", "cum", "gay", "fag", "dike", "dildo", "piss" };
	}
}
