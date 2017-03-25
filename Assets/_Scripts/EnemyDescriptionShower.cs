using UnityEngine;
using System.Collections;

public class EnemyDescriptionShower : MonoBehaviour {	

    private float fadeTime = 5f;
	// Use this for initialization
	void Start () {
        Invoke("FadeDescription",fadeTime+2f);
	}
	
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
