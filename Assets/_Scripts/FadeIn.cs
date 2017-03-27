using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeIn : MonoBehaviour {	

    private Image panel;
    [Range(0.01f,3f)]
    [SerializeField]
    private float fadeTime = 1f;

    // Use this for self-initialization
	void Awake() {
        panel = GetComponent<Image>();
        Color col = panel.color;
        col.a = 1f;
        panel.color = col;
	}
	// Use this for initialization
	void Start () {
	}

    void Update(){
        Fade();
    }

    void Fade(){
        Color col = panel.color;
        float alpha = col.a - Time.deltaTime/fadeTime;
        if(alpha<0){
            alpha = 0;
            panel.enabled = false;
        }
        col.a = alpha;
        panel.color = col;
    }
}
