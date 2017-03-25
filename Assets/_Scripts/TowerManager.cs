using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
#pragma warning disable 0649

public class TowerManager : Singleton<TowerManager> {	

    private TowerButton towerBtnPressed = null;
    private TowerButton lastTowerBtnPressed = null;
    private SpriteRenderer spriteRenderer;
    private GameObject towers;

    private Tower towerSelected = null;

    [SerializeField] private AudioClip towerBuildClip;
    [SerializeField] private AudioClip towerDestroyClip;
    [SerializeField] private GameObject[] towerButtons;

    private LineRenderer attackRange;

    // Use this for self-initialization
	void Awake() {
        towers = new GameObject();
        towers.name = "Towers";
        spriteRenderer = GetComponent<SpriteRenderer>();
        attackRange = GetComponent<LineRenderer>();
        attackRange.enabled = false;
	}
	
	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        //mobile
        if(Input.touchSupported){
            if(Input.touchCount == 1){
                if(Input.GetTouch(0).phase.Equals(TouchPhase.Ended)){
                    Vector2 point = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                    RaycastHit2D hit = Physics2D.Raycast(point,Vector2.zero);
                    HandleClick(hit);
                }
            }
        } 
        //PC - mouse input
        else {
            if(Input.GetMouseButtonDown(0)){
                Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(point,Vector2.zero);
                HandleClick(hit);
            } else if(Input.GetMouseButtonDown(1)){
                UnhandleTower();           
            } 
        }
        SpriteFollow();
	}

    public void RestartGame(){
        foreach(Transform tower in towers.transform){
            Destroy(tower.gameObject);
        }
        foreach(Transform tile in GameObject.Find("BuildSites").transform){
            tile.tag = "BuildSite";
        }
    }

    private void HandleClick(RaycastHit2D hit){
        if(!(EventSystem.current.IsPointerOverGameObject()) && towerBtnPressed != null){
            PlaceTower(hit);
        } else if(!(EventSystem.current.IsPointerOverGameObject()) && towerBtnPressed == null){
            ShowInformation(hit);
        }
    }
    private void UnhandleTower(){     
        if(towerBtnPressed){
            towerBtnPressed.Deselect();
            towerBtnPressed = null;
            spriteRenderer.sprite = null;
            attackRange.enabled = false;
            ShowInformation();
        }
    }
    public void SelectedTower(TowerButton towerSelected){
        if(towerBtnPressed != towerSelected){
            UnhandleTower();
            towerBtnPressed = towerSelected;
            spriteRenderer.sprite = towerBtnPressed.TowerPrefab.GetComponent<SpriteRenderer>().sprite;
            if(lastTowerBtnPressed != towerBtnPressed){
                towerBtnPressed.Select();
                lastTowerBtnPressed = towerBtnPressed;
            }            
            attackRange.enabled = true;
            Tower.DrowAttackRange(attackRange, towerSelected.TowerPrefab.GetComponent<Tower>().AttackRadius);
        } else {
            UnhandleTower();
        }
        //Debug.Log("pressed " + towerBtnPressed.TowerPrefab.name);
    }
    private void ShowInformation(RaycastHit2D hit){
        GameObject go = FindClosestTowerToPoint(hit);
        if(towerSelected)
            towerSelected.ShowInformation(false);
        if(go == null){
            towerSelected = null;
        } else {
            if(towerSelected == null){
                towerSelected = go.GetComponent<Tower>();
                towerSelected.ShowInformation(true);
            } else if(towerSelected.gameObject == go){ //same
                towerSelected = null;
                } else {
                    towerSelected = go.GetComponent<Tower>();
                    towerSelected.ShowInformation(true);
                }
        }
    }
    private void ShowInformation(){
        GameObject go = null;
        if(towerSelected)
            towerSelected.ShowInformation(false);
        if(go == null){
            towerSelected = null;
        } else {
            if(towerSelected == null){
                towerSelected = go.GetComponent<Tower>();
                towerSelected.ShowInformation(true);
            } else if(towerSelected.gameObject == go){ //same
                towerSelected = null;
                } else {
                    towerSelected = go.GetComponent<Tower>();
                    towerSelected.ShowInformation(true);
                }
        }
    }
    private void PlaceTower(RaycastHit2D hit){       
        if(towerBtnPressed.TowerPrefab.tag == "Destroyer"){
            if(hit.collider.tag == "BuildedSite"){
                UnhandleTower();
                DestroyClosestTowerToPoint(hit);
            }
        } else if(hit.collider.tag == "BuildSite"){
            int cost = (int)towerBtnPressed.TowerPrefab.GetComponent<Tower>().Cost;
            if(GameManager.Instance.CanBuildThis(cost)){
                GameObject newTower = Instantiate(towerBtnPressed.TowerPrefab,hit.transform.position,Quaternion.identity) as GameObject;
                newTower.transform.parent = towers.transform;
                hit.collider.tag = "BuildedSite";
                UnhandleTower();
                GameManager.Instance.RemoveMoney(cost);
                AudioSource.PlayClipAtPoint(towerBuildClip,transform.position,1f);
            }
        }
        //UnhandleTower();
    }
    private GameObject FindClosestTowerToPoint(RaycastHit2D hit){
        Vector3 pos = hit.transform.position;
        foreach(Transform tower in towers.transform){
            if(Vector2.Distance(pos,tower.position) <= 0.4f){ //tile is 0.85x0.85                
                return tower.gameObject;
            }
        }
        return null;
    }
    private void DestroyClosestTowerToPoint(RaycastHit2D hit){
        GameObject go = FindClosestTowerToPoint(hit);
        if(go){
            hit.collider.tag = "BuildSite";
            Destroy(go);
            AudioSource.PlayClipAtPoint(towerDestroyClip,transform.position,1f);
        }
    }
    private void SpriteFollow(){
        Vector3 pos;
        if(Input.touchSupported){
            if(Input.touchCount > 0){
                pos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                transform.position = new Vector3(pos.x,pos.y);
            }
        } else {
            pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(pos.x,pos.y);
        }
       
    }
}
