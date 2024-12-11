using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Splash : MonoBehaviour {

	void Start () {
        StartCoroutine(LoadStartScene());
        PlayerPrefs.SetInt("LaunchCount", PlayerPrefs.GetInt("LaunchCount", 0) + 1);
        PlayerPrefs.SetInt("RateShowed", 0);
    }

    private IEnumerator LoadStartScene() {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(ScenesIndexes.Start);
    }
}
