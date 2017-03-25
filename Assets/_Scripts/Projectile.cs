using UnityEngine;
#pragma warning disable 0649

public enum ProtType{
    rock, arrow, fireball
};

public class Projectile : MonoBehaviour {	

    [SerializeField] private float attackStrength;
    [SerializeField] private float speed;
    [SerializeField] private ProtType projectileType;
    private GameObject target;
    private Vector3 destinationPosition;

    public float AttackStrength{
        get{ return attackStrength;}
    }
    public ProtType ProjectileType{
        get{ return projectileType;}
    }

    void Start(){
        transform.parent = GameManager.Instance.Projectiles.transform;
    }

    void Update(){
        
    }
    void FixedUpdate(){
        Move();
        Rotate();       
    }
    void Move(){
        if(target){
            destinationPosition = target.transform.position;
        }
        transform.position = Vector2.MoveTowards(transform.position,destinationPosition,speed*Time.deltaTime);
        if(Vector2.Distance(transform.position,destinationPosition) <= 0.01f) //eg lost target
            Destroy(gameObject);
    }
    void Rotate(){
        Vector3 look = destinationPosition-transform.position;
        var angleDirection = Mathf.Atan2(look.y,look.x) * Mathf.Rad2Deg;
        if(look.magnitude > 0)
            transform.rotation =  Quaternion.AngleAxis(angleDirection, Vector3.forward);
    }

    public void ProjectileInstantiate(GameObject target){
        this.target = target;
        if(target){
            destinationPosition = target.transform.position;
        }
        Rotate();
    }
    void OnTriggerEnter2D(Collider2D other){
        if(other.tag == "Enemy"){
            if(other.gameObject == target){
                other.GetComponent<Enemy>().TakeDMG(attackStrength,projectileType);
                Destroy(gameObject);
            }
        }
    }

}
