using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SplashScreenController : MonoBehaviour {

    public int indexOfLevelToLoad;
    public float loadDelay;
    public Image blackPanel;

    void Start() {
        StartCoroutine (LoadMainMenu ());
    }

    IEnumerator LoadMainMenu() {
        yield return new WaitForSeconds (loadDelay);
        while (blackPanel.color.a < 1) {
            blackPanel.color += new Color(0,0,0,0.05f);
            yield return new WaitForSeconds(0.005f);
        }
        Application.LoadLevel (indexOfLevelToLoad);
    }
}
