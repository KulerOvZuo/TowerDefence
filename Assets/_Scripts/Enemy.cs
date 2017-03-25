using UnityEngine;
using UnityEngine.UI;
using System.Collections;
#pragma warning disable 0649

public class Enemy : MonoBehaviour {	

    private int target = 0;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private Transform[] wayPoints;
    [SerializeField] private float navigationUpdate;
    [SerializeField] private float speed = 0.5f;
    [SerializeField] private AudioClip dieClip;
    private float currentSpeed;
    [SerializeField] private bool taunt;
    public bool Taunt{
        get{return taunt;}
    }

    [SerializeField] private int dmg = 1;
    public int Dmg { get {return dmg;}}

    private float maxHealth;
    [SerializeField] private float Health;
    private float DOT = 0f;
    [SerializeField] private float gold;
    public float Gold{
        get{return gold;}
    }

    private Vector3 scale;
    private float navigationTime = 0;
    private bool isDead = false;
    public bool IsDead{
        get{return isDead;}
    }
    private Animator anim;

    private SpriteRenderer image;
    // Use this for self-initialization
	void Awake() {
        anim = GetComponent<Animator>();
        currentSpeed = speed;
        maxHealth = Health;
        image = GetComponent<SpriteRenderer>();
        scale = transform.localScale;
	}
	
	// Use this for initialization
	void Start () {
        GameManager.Instance.RegisterEnemy(this);
	}

    public void SetStartParameters(Transform[] waypoints, Transform exitPoint){
        this.wayPoints = waypoints;
        this.exitPoint = exitPoint;
    }
	// Update is called once per frame
	void Update () {
        if(wayPoints != null && !isDead) {
            TakeDMG(DOT*Time.deltaTime);
            navigationTime += Time.deltaTime;
            if(navigationTime >= navigationUpdate){
                currentSpeed += navigationUpdate*speed*0.2f;
                if(currentSpeed>speed)
                    currentSpeed = speed;
                navigationTime -= navigationUpdate;
                Transform nextPosition;
                if(target < wayPoints.Length){
                    nextPosition = wayPoints[target];
                } else {
                    nextPosition = exitPoint;
                }
                transform.position = Vector2.MoveTowards(transform.position,nextPosition.position,navigationUpdate*currentSpeed*Time.deltaTime);
                float xVal = (nextPosition.position-transform.position).x;
                if(xVal >= 0f)
                    transform.localScale = scale;
                else if(xVal <= -1f)
                    transform.localScale = new Vector3(-scale.x,scale.y,scale.z);
            }
        }
	}

    void OnTriggerEnter2D(Collider2D other){
        if(other.tag == "Checkpoint"){
            target++;
        } else if(other.tag == "Finish"){
            GameManager.Instance.UnregisterEnemy(this);
            GameManager.Instance.Escaped += dmg;
        } 
    }

    public float DistanceToEnd(){
        float nearest = 0f;
        if(target < wayPoints.Length)
            nearest = Vector2.Distance(transform.position,wayPoints[target].position);
        else
            nearest = Vector2.Distance(transform.position,exitPoint.position);
        return (wayPoints.Length - target)*10f + nearest;
        //return (wayPoints.Length - target)*100f + Mathf.Abs((wayPoints[target].position - transform.position).magnitude);
    }

    public void TakeDMG(float dmg){
        float percent = Health/maxHealth;
        Color col = new Color(1,percent,percent);
        image.color = col;
        Health -= dmg;
        if(Health <= 0){
            Die();
        } else if(dmg > 0){            
            anim.SetTrigger("Hurt");      
        }  
    }
    public void TakeDMG(float dmg, ProtType projectileType){
        TakeDMG(dmg);
        switch(projectileType){
            case ProtType.arrow:
                currentSpeed = speed*0.25f;
                break;
            case ProtType.fireball:
                DOT = 130;
                break;
        }
    }
    void Die(){
        GameManager.Instance.AddMoney((int)gold);
        isDead = true;
        anim.SetTrigger("Die");
        AudioSource.PlayClipAtPoint(dieClip,transform.position,0.8f);
        StartCoroutine(Disappear());
    }
    IEnumerator Disappear(){
        yield return new WaitForSeconds(2f);
        GameManager.Instance.UnregisterEnemy(this);
    }
}
