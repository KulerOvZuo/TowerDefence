using UnityEngine;
using UnityEngine.UI;
using System.Collections;

#pragma warning disable 0649

public class LevelUnlocker : MonoBehaviour {	

    [SerializeField] private GameObject[] levelButtons;

    // Use this for self-initialization
	void Awake() {
	    
	}
	
	// Use this for initialization
	void Start () {
        CheckLevelsUnlocked();
	}

    public void CheckLevelsUnlocked(){
        for(int i=2; i<=LevelManager.instance.NumberOfLevels; i++){
            if(!PlayerPrefsManager.IsLevelUnlocked(i)){
                levelButtons[i-1].GetComponent<Button>().interactable = false;
                Color col = Color.black;
                col.a = 0.5f;
                levelButtons[i-1].GetComponent<Image>().color = col;
            }

        }
    }
}
