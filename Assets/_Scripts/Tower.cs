using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
#pragma warning disable 0649

public class Tower : MonoBehaviour {	

    
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private float attackRadius;
    public float AttackRadius{
        get { return attackRadius;}
    }
    [SerializeField] private float cost;
    public float Cost{
        get { return cost;}
    }
    [SerializeField] private GameObject projectile;
    [SerializeField] private Vector3 startingOffset;
    private Enemy targetEnemy = null;
    private float timeCounter;
    private float timeCounterPrev;

    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject particle;

    [SerializeField] private int upgradeCost = 50;
    private int maxLVL = 1;
    private int LVL = 0;

    private LineRenderer circle;


    // Use this for self-initialization
	void Awake() {
        timeCounter = timeBetweenAttacks;
        timeCounterPrev = 0f;

        circle = GetComponent<LineRenderer>();
        circle.enabled = false;
        canvas.SetActive(false);
        particle.SetActive(false);
        Transform temp = canvas.transform.FindChild("UpgradeTower/Cost/Text");
        if(temp)
            temp.gameObject.GetComponent<Text>().text = upgradeCost.ToString();
	}
	
	// Use this for initialization
	void Start () {
	    //EventTrigger trigger = GetComponent<EventTrigger>();
	}
	
	// Update is called once per frame
	void Update () {
        if(GameManager.Instance.CurrentState != GameStatus.gameover){
            timeCounter += Time.deltaTime;
            if(timeCounter >= timeBetweenAttacks){
                if(timeCounter > timeCounterPrev + 0.1f){ // to not find new enemy every frame
                    timeCounterPrev = timeCounter; 
                    targetEnemy = GetEnemyToKill();
                    if(targetEnemy != null){ //not null -> attack
                        Attack();
                    }
                }
            }
        }
	}

    public void Attack(){
        GameObject proj = Instantiate(projectile,transform.position+startingOffset,Quaternion.identity) as GameObject;
        proj.GetComponent<Projectile>().ProjectileInstantiate(targetEnemy.gameObject);
        timeCounterPrev = timeCounter = 0;
    }
    private List<Enemy> GetEnemiesInRange(){
        List<Enemy> enemies = new List<Enemy>();
        foreach(Enemy enemy in GameManager.Instance.EnemyList){
            if(Vector2.Distance(transform.position,enemy.transform.position) <= attackRadius)
                if(!enemy.IsDead)
                    enemies.Add(enemy);
        }
        return enemies;
    }
    private Enemy GetEnemyToKill(){
        Enemy tempEnemy = null;
        float currentDistance = float.PositiveInfinity;
        foreach(Enemy enemy in GetEnemiesInRange()){
            float dist = enemy.DistanceToEnd();
            if(enemy.Taunt)
                dist *= 0.1f;
            if(dist < currentDistance){
                currentDistance = dist;
                tempEnemy = enemy;
            }
        }
        return tempEnemy;
    }
    public float ProjectileDMG(){
        return projectile.GetComponent<Projectile>().AttackStrength;
    }


    public void ShowInformation(bool draw){
        DrowAttackRange(draw);
        ShowUpgrade(draw);
    }

    public void ShowUpgrade(bool show){
        if(show){
            if(LVL < maxLVL){
                canvas.SetActive(show);
            }
        } else {
            canvas.SetActive(show);
        }
    } 
    public void DrowAttackRange(bool draw){
        if(draw){
            circle.enabled = true;
            float theta_scale = 0.1f;
            int size = (int)((2.0f*Mathf.PI)/theta_scale);
            circle.SetWidth(0.1f,0.1f);
            circle.SetVertexCount(size);
            for(int i=0; i<size; i++){
                float x = attackRadius*Mathf.Cos(i*theta_scale);
                float y = attackRadius*Mathf.Sin(i*theta_scale);
                Vector3 pos = new Vector3(x,y,0);
                circle.SetPosition(i,pos);
            }
        } else
            circle.enabled = false;
    }
    public static void DrowAttackRange(LineRenderer circleTemp, float attackRadius){
        float theta_scale = 0.1f;
        int size = (int)((2.0f*Mathf.PI)/theta_scale);
        circleTemp.SetWidth(0.1f,0.1f);
        circleTemp.SetVertexCount(size);
        for(int i=0; i<size; i++){
            float x = attackRadius*Mathf.Cos(i*theta_scale);
            float y = attackRadius*Mathf.Sin(i*theta_scale);
            Vector3 pos = new Vector3(x,y,0);
            circleTemp.SetPosition(i,pos);
        }
    }

    public void UpgradeTower(){
        //Debug.Log("UpgradeTower");
        if(GameManager.Instance.CanBuildThis(upgradeCost)){
            GameManager.Instance.RemoveMoney(upgradeCost);
            timeBetweenAttacks *= 0.5f;
            attackRadius *= 1.3f;
            LVL++;
            canvas.SetActive(false);
            particle.SetActive(true);
            DrowAttackRange(true);
        }
    }

    #if UNITY_EDITOR
    void OnDrawGizmos(){
        UnityEditor.Handles.DrawWireDisc(transform.position,Vector3.forward,attackRadius);
    }
    #endif

}
