using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Helpers
{
	public class DictionaryHelper
	{
		//private static DictionaryHelper instance = new DictionaryHelper("Dic");
		//public static DictionaryHelper Instance
		//{
		//	get { return instance; }
		//	set { instance = value; }
		//}

		private HashSet<string> dics;
		public DictionaryHelper(string fileName)
		{	
			this.dics = new HashSet<string>();
			TextAsset textAsset = Resources.Load(fileName) as TextAsset;
			StreamReader file = new StreamReader(new MemoryStream(textAsset.bytes), Encoding.UTF8);

			string line = null;
			while ((line = file.ReadLine()) != null)
			{	
				if (line != "")
				{
					this.dics.Add(line);
				}
				line = null;
			}

			file.Close();
		}

		public bool CheckText(string str)
		{
			return this.dics.Any (x => x == str);
		}

		public string RandomWord(){
			
			string[] _words = this.dics.ToArray ();
			string _word = _words [UnityEngine.Random.Range (0, _words.Length)];
			return _word;
		}

		public string[] GetDictionary(){
			string[] _dic = this.dics.ToArray ();
			return _dic;
		}
	}

	[Serializable]
	public class JsonHelper {

		public string[] verb;//
		public string[] noun;//
		public string[] adjective;//
		public string[] adverb;
		public string[] pronoun;
		public string[] preposition;
		public string[] exclamation;
		public string[] possessive_determiner;
		public string[] plural_noun;
		public string[] proper_noun;
		public string[] ordinal_number;
		public string[] cardinal_number;
		public string[] phrase;
		public string[] determiner;

		public static JsonHelper WordInfoCreateFromJson(string jsonString){
			return JsonUtility.FromJson<JsonHelper> (jsonString);
		}
	}
}
