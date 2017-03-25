using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

#pragma warning disable 0649

public class LevelManager : MonoBehaviour {

    public static LevelManager instance = null;

    public float autoLoadNextLevelAfter;
    [SerializeField] private int numberOfLevels = 3;
    public int NumberOfLevels {get {return numberOfLevels;}}

    void Awake(){
        if(instance == null){
            instance = this;
        } else if(instance != this){
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        numberOfLevels = 3;
    }

    void Start(){
        if(autoLoadNextLevelAfter>0)
            Invoke("LoadNextLevel",autoLoadNextLevelAfter);
        else if(autoLoadNextLevelAfter == 0)
            Debug.Log("AutoLoad disabled");
        else Debug.Log("Wrong time value");
    }
    public void LoadLevel(string name){
        Debug.Log("Level load requested for: " + name);
        SceneManager.LoadScene(name,LoadSceneMode.Single);
    }
    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1,LoadSceneMode.Single);
    }
    public void QuitRequest(){
        Debug.Log("Quit");
        Application.Quit();
    }

    public void ResetAll(){
        GameObject.FindObjectOfType<OptionsController>().SetDefaults();
        for(int i=2; i<=NumberOfLevels; i++){
            PlayerPrefsManager.ResetLevelProgress(i);
        }
        FindObjectOfType<LevelUnlocker>().CheckLevelsUnlocked();
    }
}
