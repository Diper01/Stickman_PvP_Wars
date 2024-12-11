using UnityEngine;
using System.Collections;
using System;


public class LocalisedUIText : MonoBehaviour {

	UnityEngine.UI.Text CurrentText;
    public AdditionalSettingForUIText[] AdditionalSettingForUIText;

	// Use this for initialization
	void Start () {
		if(transform.GetComponent<UnityEngine.UI.Text>() != null){
		    CurrentText = transform.GetComponent<UnityEngine.UI.Text>();
		    LocaliseString ();
            ApplyAdditionalStringSetting();
		}
	}
	
	void LocaliseString(){
		string CurrentLocalisedString = Localisation.GetString (CurrentText.text);
		CurrentText.text = CurrentLocalisedString;
	}

    void ApplyAdditionalStringSetting()
    {
        if (AdditionalSettingForUIText.Length > 0)
        {
            for (int i = 0; i < AdditionalSettingForUIText.Length; i++)
            {
                if (Localisation.CurrentLanguage == AdditionalSettingForUIText[i].Language)
                {
                    if (AdditionalSettingForUIText[i].FontFile != null)
                    {
                        CurrentText.font = AdditionalSettingForUIText[i].FontFile;
                    }
                }
            }
        }
    }
}

[Serializable]
public class AdditionalSettingForUIText
{
    public Languages Language;
    public Font FontFile;
}