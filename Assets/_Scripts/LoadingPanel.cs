using UnityEngine;
using System.Collections;

public class LoadingPanel : MonoBehaviour {	

    private Vector3 loadingPanelScale;

    // Use this for self-initialization
	void Awake() {
        loadingPanelScale = transform.localScale;
	}
	
	// Use this for initialization
	void Start () {
	    LevelManager.instance.SetLoadingPanel(this);
	}
	
    public void Show(){
        transform.localScale = loadingPanelScale;
    }
    public void Hide(){
        transform.localScale = Vector3.zero;
    }
}
