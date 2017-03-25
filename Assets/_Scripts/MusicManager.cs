using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour {	

    public AudioClip[] levelMusicChangeArray;
    private AudioSource audioSource;

    // Use this for self-initialization
	void Awake() {
	    DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
        SceneManager.sceneLoaded += OnSceneLoaded;
	}

    void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        int sceneIndex = scene.buildIndex;
        Debug.Log("Scene loaded " + sceneIndex);
        if(scene.buildIndex < levelMusicChangeArray.Length){
            if(levelMusicChangeArray[sceneIndex] || sceneIndex == 0){
                if(levelMusicChangeArray[sceneIndex] != audioSource.clip){
                    audioSource.volume = PlayerPrefsManager.GetMasterVolume();
                    audioSource.clip = levelMusicChangeArray[sceneIndex];
                    audioSource.loop = true;
                    audioSource.Play();
                }
                switch(sceneIndex){
                    case 0: audioSource.volume = PlayerPrefsManager.GetMasterVolume(); break;
                    case 6: audioSource.loop = false; break;
                    case 7: audioSource.loop = false; break;
                }
            } else {
                Debug.LogWarning("No music for scene " + sceneIndex);
            }
        }
        else Debug.LogWarning("No music for scene " + sceneIndex);
    }

    public void SetVolume(float volume){
        if(volume >= 0f && volume <= 1f){
            //AudioListener.volume = volume;
            audioSource.volume = volume;
        }
        else Debug.LogError("Master volume out of range");
    }
}
