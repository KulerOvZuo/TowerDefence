using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
#pragma warning disable 0649

public class TowerManager : Singleton<TowerManager> {	

    public const string BUILD_SITE = "BuildSite";
    public const string BUILDED_SITE = "BuildedSite";
    private const string TOWER_GO = "Towers";

    //currently selected tower button
    private TowerButton towerBtnPressed = null;
    //previously selected tower button - use to not show label every time
    private TowerButton lastTowerBtnPressed = null;
    //used to show image if tower
    private SpriteRenderer spriteRenderer;
    private LineRenderer attackRange;
    //used to store towers in hierarchy

    private GameObject towers;

    //currenlty selected tower on map (ex. to upgade or show range)
    public Tower towerSelected = null;

    [SerializeField] private AudioClip towerBuildClip;
    [SerializeField] private AudioClip towerDestroyClip;
    //buttons used to place towers
    [SerializeField] private GameObject[] towerButtons;

    // Use this for self-initialization
	void Awake() {
        towers = new GameObject();
        towers.name = TOWER_GO;
        spriteRenderer = GetComponent<SpriteRenderer>();
        attackRange = GetComponent<LineRenderer>();
        attackRange.enabled = false;
	}
	
	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {        
        if(Input.touchSupported)
            Input_touchSupported();
        else
            Input_touchNotSupported();
        SpriteFollow();
	}
    //mobile
    void Input_touchSupported(){
        if(Input.touchCount == 1){
            if(Input.GetTouch(0).phase.Equals(TouchPhase.Ended)){
                Vector2 point = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                RaycastHit2D hit = Physics2D.Raycast(point,Vector2.zero);
                HandleClick(hit);
            }
        }
    }
    //PC - mouse input
    void Input_touchNotSupported(){
        if(Input.GetMouseButtonDown(0)){
            Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(point,Vector2.zero);
            HandleClick(hit);
        } else if(Input.GetMouseButtonDown(1)){
            UnhandleTower();           
        } 
    }
    //removes all towers and changes buildsites "statuses"
    public void RestartGame(){
        foreach(Transform tower in towers.transform){
            Destroy(tower.gameObject);
        }
        foreach(Transform tile in GameObject.Find("BuildSites").transform){
            tile.tag = BUILD_SITE;
        }
    }

    //handles clicks on map
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
            HideInformation();
        }
    }
    //metod used to call unhandle tower at the end of wave/game
    public void UnhandleTower_EndWave(){
        UnhandleTower();
        lastTowerBtnPressed = null;
    }
    //metod called by tower button - highlights button, shows sprite etc
    public void SelectedTower(TowerButton towerSelected){
        if(towerBtnPressed != towerSelected){
            UnhandleTower();
            towerBtnPressed = towerSelected;
            spriteRenderer.sprite = towerBtnPressed.TowerPrefab.GetComponent<SpriteRenderer>().sprite;  

            towerBtnPressed.Select(lastTowerBtnPressed != towerBtnPressed);            
            lastTowerBtnPressed = lastTowerBtnPressed != towerBtnPressed ? towerBtnPressed : lastTowerBtnPressed;
                        
            attackRange.enabled = true;
            Tower.DrowAttackRange(attackRange, towerSelected.TowerPrefab.GetComponent<Tower>().AttackRadius);
        } else
            UnhandleTower();
    }
    //show information of tower or hide if click on the same
    private void ShowInformation(RaycastHit2D hit){
        GameObject tower = FindCloseTowerToPoint(hit);
        //hide information from current tower
        if(towerSelected)
            towerSelected.ShowInformation(false);
        //if no tower - return
        if(!tower){
            towerSelected = null;
            return;
        }
        //if previously selected is null
        if(!towerSelected){
            towerSelected = tower.GetComponent<Tower>();
            towerSelected.ShowInformation(true);
            return;
        }
        if(towerSelected.gameObject == tower){ //same - do nothing
            towerSelected = null;
        }
        //if different tower selected 
        else {
            towerSelected = tower.GetComponent<Tower>();
            towerSelected.ShowInformation(true);
        }        
    }
    //hides information of tower
    private void HideInformation(){
        if(towerSelected)
            towerSelected.ShowInformation(false);
        towerSelected = null;
    }
    private void PlaceTower(RaycastHit2D hit){
        //"tower" used for destroying others       
        if(towerBtnPressed.TowerPrefab.tag == "Destroyer"){
            if(hit.collider.tag == BUILDED_SITE){
                UnhandleTower();
                DestroyCloseTowerToPoint(hit);
            }
            return;
        }   
        if(hit.collider.tag == BUILD_SITE){
            int cost = (int)towerBtnPressed.TowerPrefab.GetComponent<Tower>().Cost;
            if(GameManager.Instance.CanBuildThis(cost)){
                GameObject newTower = Instantiate(towerBtnPressed.TowerPrefab,hit.transform.position,Quaternion.identity) as GameObject;
                newTower.transform.parent = towers.transform;
                hit.collider.tag = BUILDED_SITE;
                UnhandleTower();
                GameManager.Instance.RemoveMoney(cost);
                AudioSource.PlayClipAtPoint(towerBuildClip,transform.position,1f);
            }
        }
        //UnhandleTower();
    }
    //finds tower close to hitpoint
    private GameObject FindCloseTowerToPoint(RaycastHit2D hit){
        Vector3 pos = hit.transform.position;
        foreach(Transform tower in towers.transform){
            if(Vector2.Distance(pos,tower.position) <= 0.4f){ //tile is 0.85x0.85                
                return tower.gameObject;
            }
        }
        return null;
    }
    //destroys tower close to hit point if on builded site - changes to build
    private void DestroyCloseTowerToPoint(RaycastHit2D hit){
        GameObject tower = FindCloseTowerToPoint(hit);
        if(tower){
            hit.collider.tag = BUILD_SITE;
            Destroy(tower);
            AudioSource.PlayClipAtPoint(towerDestroyClip,transform.position,1f);
        }
    }

    //metod to follow sprite under cursor
    private void SpriteFollow(){
        if(Input.touchSupported){
            SpriteFollow_TouchSupported();
        } else {
            SpriteFollow_TouchNotSupported();
        }
       
    }
    private void SpriteFollow_TouchSupported(){
        Vector3 pos;
        if(Input.touchCount > 0){
            pos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            transform.position = new Vector3(pos.x,pos.y);
        }       
    }
    private void SpriteFollow_TouchNotSupported(){
        Vector3 pos;
        pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(pos.x,pos.y);      
    }
}
