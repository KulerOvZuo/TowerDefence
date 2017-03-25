using UnityEngine;
using UnityEngine.UI;
using System.Collections;

#pragma warning disable 0649

public class LevelUnlocker : MonoBehaviour {	

    [SerializeField] private GameObject[] levelButtons;
    [SerializeField] private GameObject winLabel;

    // Use this for self-initialization
	void Awake() {
	    
	}
	
	// Use this for initialization
	void Start () {
        CheckLevelsUnlocked();
	}

    public void CheckLevelsUnlocked(){
        bool all = true;
        for(int i=1; i<=LevelManager.instance.NumberOfLevels; i++){
            if(!PlayerPrefsManager.IsLevelUnlocked(i+1)){
                if(i<levelButtons.Length){
                    levelButtons[i].GetComponent<Button>().interactable = false;
                    Color col = Color.black;
                    col.a = 0.5f;
                    levelButtons[i].GetComponent<Image>().color = col;
                }
                all = false;
            } else {
                if(i<levelButtons.Length){
                    levelButtons[i].GetComponent<Button>().interactable = true;
                    Color col = Color.white;
                    col.a = 1f;
                    levelButtons[i].GetComponent<Image>().color = col;
                }
            }
        }
        winLabel.SetActive(all);            
    }
}
