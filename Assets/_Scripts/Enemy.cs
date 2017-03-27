using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Assertions;

#pragma warning disable 0649

public class Enemy : MonoBehaviour {	

    private int targetWaypoint = 0;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private Transform[] wayPoints;
    [SerializeField] private float navigationUpdate;
    [SerializeField] private float speed = 0.5f;
    [SerializeField] private AudioClip dieClip;
    //actual speed of enemy
    private float currentSpeed;
    [Tooltip("Enemy can taunt tower to attack him")]
    [SerializeField] private bool taunt;
    public bool Taunt{
        get{return taunt;}
    }
    [Tooltip("If enemy reaches end, ppl lost that much HP")]
    [SerializeField] private int dmg = 1;
    public int Dmg { get {return dmg;}}

    private float maxHealth;
    [SerializeField] private float Health;
    private float DOT = 0f;
    [SerializeField] private float gold;
    public float Gold{
        get{return gold;}
    }

    //scale of object
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
        Assert.IsNotNull(exitPoint);
        Assert.IsNotNull(wayPoints);
        Assert.IsNotNull(dieClip);

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
     
	// Update is called once per frame
	void Update () {
        if(!IfCanUpdate())
            return;
        TakeDMG(DOT*Time.deltaTime);
        navigationTime += Time.deltaTime;           
	}

    void FixedUpdate(){
        if(!IfCanUpdate())
            return;
        Navigate();
    }
    bool IfCanUpdate(){
        return wayPoints != null && !isDead;
    }

    //movesenemy on map to the next point
    public void Navigate(){
        if(navigationTime >= navigationUpdate){
            IncreaseSpeedBy20percentUpMax();
            navigationTime -= navigationUpdate;
            Vector3 nextPosition = FindNextPosition().position;
            transform.position = Vector2.MoveTowards(transform.position,nextPosition,navigationUpdate*currentSpeed*Time.deltaTime);
            float xVal = (nextPosition-transform.position).x;
            if(xVal >= 0f)
                transform.localScale = scale;
            else if(xVal <= -0.7f)
                transform.localScale = new Vector3(-scale.x,scale.y,scale.z);
        }
    }
    public void IncreaseSpeedBy20percentUpMax(){
        currentSpeed += navigationUpdate*speed*0.2f;
        if(currentSpeed>speed)
            currentSpeed = speed;
    }
    public Transform FindNextPosition(){
        return targetWaypoint < wayPoints.Length ? wayPoints[targetWaypoint] : exitPoint;
    }
    public void SetStartParameters(Transform[] waypoints, Transform exitPoint){
        this.wayPoints = waypoints;
        this.exitPoint = exitPoint;
    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.tag == "Checkpoint"){
            targetWaypoint++;
        } else if(other.tag == "Finish"){
            GameManager.Instance.UnregisterEnemy(this);
            GameManager.Instance.Escaped += dmg;
        } 
    }
    //counts distance to the end of map
    public float DistanceToEnd(){
        float nearest = 0f;
        if(targetWaypoint < wayPoints.Length)
            nearest = Vector2.Distance(transform.position,wayPoints[targetWaypoint].position);
        else
            nearest = Vector2.Distance(transform.position,exitPoint.position);
        return (wayPoints.Length - targetWaypoint)*10f + nearest;
        //return (wayPoints.Length - target)*100f + Mathf.Abs((wayPoints[target].position - transform.position).magnitude);
    }

    public void TakeDMG(float dmg){
        ChangeColorOnHealth();
        Health -= dmg;
        if(Health <= 0){
            Die();
        } else if(dmg > 0){            
            anim.SetTrigger("Hurt");      
        }  
    }
    public void ChangeColorOnHealth(){
        float percent = Health/maxHealth;
        Color col = new Color(1,percent,percent);
        image.color = col;
    }
    public void TakeDMG(float dmg, ProtType projectileType){
        TakeDMG(dmg);
        switch(projectileType){
            case ProtType.arrow:
                currentSpeed = speed*0.25f;
                break;
            case ProtType.fireball:
                DOT = 110;
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
