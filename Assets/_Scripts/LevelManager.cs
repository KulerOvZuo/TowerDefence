using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

#pragma warning disable 0649

public class LevelManager : MonoBehaviour {

    public static LevelManager instance = null;

    public float autoLoadNextLevelAfter;
    [SerializeField] private int numberOfLevels = 3;
    public LoadingPanel loadingPanel = null;
    public int NumberOfLevels {get {return numberOfLevels;}}

    void Awake(){
        if(instance == null){
            instance = this;
        } else if(instance != this){
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start(){
        if(autoLoadNextLevelAfter>0)
            Invoke("LoadNextLevel",autoLoadNextLevelAfter);
        else if(autoLoadNextLevelAfter == 0)
            Debug.Log("AutoLoad disabled");
        else Debug.Log("Wrong time value");
    }

    public void FindLoadingPanel(){        
        instance.loadingPanel = GameObject.FindObjectOfType<LoadingPanel>();
        if(instance.loadingPanel)
            instance.loadingPanel.Hide();
    }
    public void LoadLevel(string name){
        Debug.Log("Level load requested for: " + name);
        if(instance.loadingPanel) instance.loadingPanel.Show();
        instance.loadingPanel = null;
        SceneManager.LoadScene(name,LoadSceneMode.Single);
        //instance.StartCoroutine(LoadLevel_after(name));
    }
    IEnumerator LoadLevel_after(string name){
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(name,LoadSceneMode.Single);
    }
    public void LoadNextLevel()
    {
        if(instance.loadingPanel) instance.loadingPanel.Show();
        instance.loadingPanel = null;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1,LoadSceneMode.Single);
    }
    public void QuitRequest(){
        Debug.Log("Quit");
        Application.Quit();
    }

    public void ResetAll(){
        GameObject.FindObjectOfType<OptionsController>().SetDefaults();
        //@todo delete
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
