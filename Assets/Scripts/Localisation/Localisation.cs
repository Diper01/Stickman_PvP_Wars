using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;



public static class Localisation
{
	public static Languages CurrentLanguage = Languages.Undefined;
	static public Dictionary<string,string> Strings;
	static private XmlDocument LoadedLanguage;
	public static bool LanguageLoaded = false;
	static TextAsset newbyLanguage;

	public static void DetectLanguage ()
	{
        //if(PlayerPrefs.HasKey("TestLanguage")){
        //CurrentLanguage = (Languages)Enum.Parse (typeof(Languages),PlayerPrefs.GetString("TestLanguage"));
        //}
        //      else {
        //CurrentLanguage = (Languages)Enum.Parse (typeof(Languages),Application.systemLanguage.ToString());
        //}
#if UNITY_EDITOR
		CurrentLanguage = Languages.English;
#else
        try
        {
            CurrentLanguage = (Languages)Enum.Parse(typeof(Languages), Application.systemLanguage.ToString());
        }
        catch (Exception) {
            CurrentLanguage = Languages.English;
        }
#endif
    }

	static public void LoadLanguage ()
	{
		Localisation.DetectLanguage ();
		LoadedLanguage = new XmlDocument ();
		Strings = new Dictionary<string, string> ();	
        //		if (CurrentLanguage.ToString() == "Russian")
        //			newbyLanguage = (TextAsset) Resources.Load ("Localisation/German" + ".xml", typeof(TextAsset));
		newbyLanguage = (TextAsset)Resources.Load ("Localisation/" + CurrentLanguage.ToString (), typeof(TextAsset));
		if (newbyLanguage == null) {
			newbyLanguage = (TextAsset)Resources.Load ("Localisation/English", typeof(TextAsset));
		}
		LoadedLanguage.LoadXml (newbyLanguage.text);
		foreach (XmlNode document in LoadedLanguage.ChildNodes) {
			foreach (XmlNode newbyString in document.ChildNodes) {
				Strings.Add (newbyString.Attributes ["name"].Value, newbyString.InnerText);
			}
		}
		LanguageLoaded = true;
	}

	static public Languages GetCurrentLanguage ()
	{
		if (LanguageLoaded == false) {
			LoadLanguage ();
		}
		return CurrentLanguage;
	}

	static public string GetString (string SearchString)
	{
		if (LanguageLoaded == false) {
			LoadLanguage ();
		}
		if (Strings.ContainsKey (SearchString)) {
			return Strings [SearchString];
		} else {
			return "Unknown string";
		}
	}
}


public enum Languages
{
    Undefined,
    Unknown,

    Russian,
    Ukrainian,
    Belarusian,

    English,
    Italian,
    Spanish,
    French,
    German,
    Polish,
    Czech,

    Chinese,
    ChineseSimplified,
    Japanese,
    Korean,

    Afrikaans,
    Arabic,
    Basque,
    Bulgarian,
    Catalan,
    Danish,
    Dutch,
    Estonian,
    Faroese,
    Finnish,
    Greek,
    Hebrew,
    Icelandic,
    Indonesian,
    Latvian,
    Lithuanian,
    Norwegian,
    Portuguese,
    Romanian,
    Slovak,
    Slovenian,
    Swedish,
    Thai,
    Turkish,
    Vietnamese,
    Hungarian
}