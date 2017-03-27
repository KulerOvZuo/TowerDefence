using UnityEngine;
using UnityEngine.UI;
using System.Collections;

#pragma warning disable 0649

public class OptionsController : MonoBehaviour {	

    [SerializeField] private Slider volume;
       
    // Use this for self-initialization
	void Awake() {
        volume.value = PlayerPrefsManager.GetMasterVolume();
	}
	
	// Update is called once per frame
	void Update () {
        AudioListener.volume = volume.value;
        PlayerPrefsManager.SetMasterVolume(volume.value);
	}
    
    public void SetDefaults(){
        volume.value = PlayerPrefsManager.MASTER_VOLUME_DEF;
    }
    public void SaveAndExit(){
        PlayerPrefsManager.SetMasterVolume(volume.value);
    }
}
