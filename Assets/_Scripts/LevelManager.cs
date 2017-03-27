using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

#pragma warning disable 0649

public class LevelManager : MonoBehaviour {

    public static LevelManager instance = null;

    public float autoLoadNextLevelAfter;
    //number of levels in game, not scenes
    [Tooltip("Number of levels in game. Must be set properly.")]
    [SerializeField] private int numberOfLevels = 3;
    public int NumberOfLevels {get {return numberOfLevels;}}
    //panel with label "loading"; can be null
    private LoadingPanel loadingPanel = null;

    void Awake(){
        Singleton();
    }

    void Start(){
        if(autoLoadNextLevelAfter>0)
            Invoke("LoadNextLevel",autoLoadNextLevelAfter);
        else if(autoLoadNextLevelAfter == 0)
            Debug.Log("AutoLoad disabled");
        else Debug.Log("Wrong time value");
    }

    //makes LevelManager a singleton
    void Singleton(){
        if(instance == null){
            instance = this;
        } else if(instance != this){
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    //on scene start, if panel exists, it calls this function 
    public void SetLoadingPanel(LoadingPanel obj){        
        instance.loadingPanel = obj;
        if(instance.loadingPanel)
            instance.loadingPanel.Hide();
    }
    private void ShowLoadingPanel(){
        if(instance.loadingPanel)
            instance.loadingPanel.Show();
    }

    public void LoadLevel(string name){
        Debug.Log("Level load requested for: " + name);
        ShowLoadingPanel();
        SceneManager.LoadScene(name,LoadSceneMode.Single);
    }

    IEnumerator LoadLevel(string name, float loadAfterSeconds){
        yield return new WaitForSeconds(loadAfterSeconds);
        SceneManager.LoadScene(name,LoadSceneMode.Single);
    }
    public void LoadNextLevel(){
        ShowLoadingPanel();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1,LoadSceneMode.Single);
    }
    public void QuitRequest(){
        Debug.Log("Quit");
        Application.Quit();
    }

    //@todo test function to lock and unlock all levels
    public void ResetAll(){
        GameObject.FindObjectOfType<OptionsController>().SetDefaults();
        if(PlayerPrefsManager.IsLevelUnlocked(4)){
            for(int i=1; i<=NumberOfLevels; i++){
                PlayerPrefsManager.ResetLevelProgress(i+1);
            }
        } else {
            for(int i=1; i<=NumberOfLevels; i++){
                PlayerPrefsManager.UnlockLevel(i+1);
            }
        }
        FindObjectOfType<LevelUnlocker>().CheckLevelsUnlocked();
    }
}
