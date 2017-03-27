using UnityEngine;
using System.Collections;

public class EnemyDescriptionShower : MonoBehaviour {	

    [Tooltip("How long will be delayed fading of first description")]
    [SerializeField] private float delayFadeTime = 5f;
    [Tooltip("How long will last 1 description")]
    [SerializeField] private float fadeTime = 5f;
	// Use this for initialization
	void Start () {
        Invoke("FadeDescription",fadeTime+delayFadeTime);
	}
    //fades first actived description
    void FadeDescription(){
        foreach(Transform ob in transform){
            if(ob.gameObject.activeSelf){
                ob.gameObject.SetActive(false);
                CancelInvoke();
                Invoke("FadeDescription",fadeTime);
                return;
            }
        }
    }
}
