using UnityEngine;
using System.Collections;

public class CreditsController : MonoBehaviour {

    public static CreditsController instance;

	public float scrollSpeed;
	public RectTransform[] creditTransforms;
    public GameObject creditsGO;
    public GameObject mainMenuGO;

    Vector3[] startLocations = new Vector3[4];
    bool creditsRolling;

    void Awake() {
        instance = this;
    }

    void Start() {
        for (int i = 0; i < startLocations.Length; i++) {
            startLocations[i] = creditTransforms[i].position;
            Debug.Log (startLocations[i].ToString () + " " + creditTransforms[i].gameObject.name);
        }
        creditsGO = GameObject.Find ("CreditsGO");
        creditsGO.SetActive (false);
        creditsRolling = false;
        mainMenuGO = GameObject.Find ("MainMenuCanvas");
        
    }

	void Update(){
        if (creditsRolling) {
            foreach (RectTransform rt in creditTransforms) {
                rt.Translate (0, scrollSpeed * Time.deltaTime, 0);
            }

            if (creditTransforms[0].position.y >= 15000) {
                /*
                foreach (RectTransform rt in creditTransforms){
                    rt.transform.position -= new Vector3(0f, 15151f, 0f);
                }
                */
                BeginCredits ();
            }
        }
	}

    public void ShowCredits() {
        mainMenuGO.SetActive (false);
        creditsGO.SetActive (true);
        BeginCredits ();
    }

    public void HideCredits() {
        mainMenuGO.SetActive (true);
        creditsGO.SetActive (false);
        creditsRolling = false;
    }

    void BeginCredits() {
        Debug.Log ("begin game");
        creditsRolling = true;
        for (int i = 0; i < startLocations.Length; i++) {
            creditTransforms[i].position = startLocations[i];
        }
    }

}
