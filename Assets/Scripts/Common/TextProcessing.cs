using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextProcessing : MonoBehaviour {

    private string startText;
    private Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
        startText = Localisation.GetString("Connection");
    }

    void OnEnable () {
        StartCoroutine(SearchTextUpdate());
	}

    private void OnDisable()
    {
        text.text = startText;
    }

    IEnumerator SearchTextUpdate() {
        text.text = startText;
        yield return null;        
        while (this.gameObject.activeSelf) {
            text.text = startText;
            yield return new WaitForSeconds(0.5f);
            text.text = startText +" . ";
            yield return new WaitForSeconds(0.5f);
            text.text = startText + " . .";
            yield return new WaitForSeconds(0.5f);
            text.text = startText + " . . .";
            yield return new WaitForSeconds(0.5f);
        }
    }
}
