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

    //checks which levels are already unlocked and changes buttons visibility
    public void CheckLevelsUnlocked(){
        bool all = true;
        //always leave button for first lvl; <= cause to "unlock" win label after unlocking NumberOfLevels+1 levels
        for(int i=1; i<=LevelManager.instance.NumberOfLevels; i++){
            if(!PlayerPrefsManager.IsLevelUnlocked(i+1)){
                if(i<levelButtons.Length){
                    LockButton(levelButtons[i]);
                }
                all = false;
            } else {
                if(i<levelButtons.Length){
                    UnlockButton(levelButtons[i]);
                }
            }
        }
        if(winLabel)
            winLabel.SetActive(all);            
    }
    //shows button as unlocked
    private void UnlockButton(GameObject button){
        button.GetComponent<Button>().interactable = true;
        Color col = Color.white;
        col.a = 1f;
        button.GetComponent<Image>().color = col;
    }
    //shows button as locked
    private void LockButton(GameObject button){
        button.GetComponent<Button>().interactable = false;
        Color col = Color.black;
        col.a = 0.5f;
        button.GetComponent<Image>().color = col;
    }
}
