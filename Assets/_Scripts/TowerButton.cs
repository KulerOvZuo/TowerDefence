using UnityEngine;
using UnityEngine.UI;
using System.Collections;
#pragma warning disable 0649

public class TowerButton : MonoBehaviour {	

    [SerializeField] private GameObject towerPrefab;
    public GameObject TowerPrefab{
        get { return towerPrefab;}
    }
    private Image image;
    private GameObject description = null;

    void Awake(){
        Transform temp = transform.Find("DMG/DMG");
        if(temp)
            temp.GetComponent<Text>().text = TowerPrefab.GetComponent<Tower>().ProjectileDMG().ToString();
        temp = transform.Find("Cost/Cost");
        if(temp)
            temp.GetComponent<Text>().text = TowerPrefab.GetComponent<Tower>().Cost.ToString();
        image = GetComponent<Image>();
        temp = transform.Find("Label");
        if(temp)
            description = temp.gameObject;
        Deselect();
    }
    public void Select(){
        Color col = Color.white;
        col.a = 1f;
        image.color = col;
        if(description)
            description.SetActive(true);
        Invoke("FadeDescription",2f);
    }
    public void Deselect(){
        Color col = Color.white;
        col.a = 0.6f;
        image.color = col;
        if(description)
            description.SetActive(false);
        CancelInvoke();
    }
    void FadeDescription(){
        if(description)
            description.SetActive(false);
    }

     
}
