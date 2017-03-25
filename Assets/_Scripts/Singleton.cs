using UnityEngine;

public class Singleton<T> : MonoBehaviour  where T : MonoBehaviour {	

    private static T instance = null;
    public static T Instance{
        get {
            T findObject =  FindObjectOfType<T>();
            if(instance == null){
                instance = findObject;
                DontDestroyOnLoad(instance);
            } else if(instance != findObject){
                Destroy(findObject);
            }
            return instance;
        }
    }
}
