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

    [Header("Tower button fields")]
    [SerializeField] private Text DMGLabel;
    [SerializeField] private Text costLabel;
    [SerializeField] private GameObject description = null;

    void Awake(){
        SetLabelValues();
        Deselect();
    }

    //sets values on labels to default values
    public void SetLabelValues(){
        if(DMGLabel)
            DMGLabel.GetComponent<Text>().text = TowerPrefab.GetComponent<Tower>().ProjectileDMG().ToString();
        if(costLabel)
            costLabel.GetComponent<Text>().text = TowerPrefab.GetComponent<Tower>().Cost.ToString();
        image = GetComponent<Image>();
    }
    //metod called when tower button is pressed
    public void Select(bool showDescription = true){
        Color col = Color.white;
        col.a = 1f;
        image.color = col;
        if(showDescription)
            if(description){
                description.SetActive(true);        
                Invoke("HideDescription",2f);
            }
    }
    //metod called when tower button i deselected
    public void Deselect(){
        Color col = Color.white;
        col.a = 0.6f;
        image.color = col;
        if(description)
            description.SetActive(false);
        CancelInvoke();
    }
    void HideDescription(){
        if(description)
            description.SetActive(false);
    }

     
}
