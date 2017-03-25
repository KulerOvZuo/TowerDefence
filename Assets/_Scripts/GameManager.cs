using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
#pragma warning disable 0649

public enum GameStatus{
    beforePlay, next, play, gameover, win
}

public class GameManager : Singleton<GameManager> {	

    [SerializeField] int LEVEL_NUMBER;

    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private GameObject[] enemiesPrefabs;
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private int waveBossIncrease = 5;
    [SerializeField] private int maxEnemiesOnScreen;
    private int enemiesSpawned;
    [SerializeField] private int totalEnemies;
    [SerializeField] private int enemiesPerSpawn;
    [SerializeField] private float spawnTime;
    private float spawnTimeMin = 0.3f;

    [SerializeField] private AudioClip newGameClip;
    [SerializeField] private AudioClip loseClip;
    [SerializeField] private AudioClip winClip;

    #region GUI
    private GameStatus currentState = GameStatus.beforePlay;
    public GameStatus CurrentState{
        get { return currentState;}
    }
    [SerializeField] private Text moneyText; 
    [SerializeField] private int money = 10;
    public int Money{
        get { return money;}
        set{ money = value;
            moneyText.text = money.ToString();}
    }
    [SerializeField] private Text currentWaveText;
    private int currentWave = 0;
    public int CurrentWave{
        get { return currentWave;}
        set{ currentWave = value;
            currentWaveText.text = "Wave " + (currentWave+1).ToString() + "/" + totalWaves;}
    }
    [SerializeField] private int totalWaves = 10;
    [SerializeField] private Text escapedText;
    [SerializeField] private int maxEscaped = 10;
    public int MaxEscaped{
        get { return maxEscaped;}
        set{ maxEscaped = value;
            escapedText.text = "Escaped "+ escaped + "/" + maxEscaped;}
    }
    private int healCost = 300;
    private int escaped = 0;
    public int Escaped{
        get { return escaped;}
        set { escaped = value;
            escapedText.text = "Escaped "+ escaped + "/" + maxEscaped;}
    }
    [SerializeField] private Text playText;
    [SerializeField] private Button playButtonText;
    [SerializeField] private float whichEnemiesToSpawn = 0;

    [SerializeField] private GameObject menuConfirm;

    [SerializeField] private GameObject pauseButton;
    [SerializeField] private Text speedText;
    private float defaultFixeUpdateTime;
    private float gameSpeed = 1f;
    private bool paused = false;
    public bool Paused{ get {return paused;} }

    #endregion

    private List<Waypoints> waypoints;

    public List<Enemy> EnemyList;
    private GameObject enemies;
    private GameObject projectiles;
    public GameObject Projectiles{
        get{ return projectiles;}
    }
    // Use this for self-initialization
	void Awake() {
        enemies = new GameObject();
        enemies.name = "Enemies";
        projectiles = new GameObject();
        projectiles.name = "Projectiles";
        EnemyList = new List<Enemy>();
        playButtonText.gameObject.SetActive(false);
        Money = 35;
        Escaped = 0;
        MaxEscaped = 10;
        CurrentWave = 0;
        GameObject.Find("Health/Health").GetComponent<Text>().text = healCost.ToString();
        defaultFixeUpdateTime = Time.fixedDeltaTime;
        menuConfirm.SetActive(false);

        waypoints = new List<Waypoints>();
        foreach(Waypoints waypoint in GetComponents<Waypoints>()){
            waypoints.Add(waypoint);
        }
	}
	
	// Use this for initialization
	void Start () {
        TowerManager.Instance.RestartGame();
        ShowMenu();
        speedText.text = "x" + gameSpeed.ToString();
        //StartWave();
	}
	
	// Update is called once per frame
	void Update () {
        if(currentState != GameStatus.win && currentState != GameStatus.gameover){
            if(escaped > maxEscaped){
                currentState = GameStatus.gameover;
                ShowMenu();
            }
            else
                CheckIfWaveCleared();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
            TryMenu();
	}

    void RestartGame(){
        menuConfirm.SetActive(false);
        playButtonText.gameObject.SetActive(false);
        Money = 35;
        Escaped = 0;
        MaxEscaped = 10;
        CurrentWave = 0;
        whichEnemiesToSpawn = 0;
        maxEnemiesOnScreen = 5;
        totalEnemies = 10;
        spawnTime = 1f;
        currentState = GameStatus.beforePlay;
        TowerManager.Instance.RestartGame();
        DestroyAllEnemies();
        foreach(Transform projectile in projectiles.transform){
            Destroy(projectile.gameObject);
        }
        StartWave();
    }
    void StartWave(){
        enemiesSpawned = 0;
        currentState = GameStatus.play;
        AudioSource.PlayClipAtPoint(newGameClip,transform.position,0.6f);
        StartCoroutine(SpawnEnemy());
    }
    void CheckIfWaveCleared(){
        if(EnemyList.Count == 0 && enemiesSpawned == totalEnemies){ //wave ended
            if(currentWave + 1 >= totalWaves){ //win
                currentState = GameStatus.win;
            } else {
                currentState = GameStatus.next;
                whichEnemiesToSpawn += 0.5f;
                CurrentWave++;
                if(spawnTime >spawnTimeMin)
                    spawnTime -= 0.08f;
                maxEnemiesOnScreen += 5;
                totalEnemies += 10;
            }
            ShowMenu();
        }
    }
    IEnumerator SpawnEnemy(){
        if(enemiesPerSpawn > 0 && enemiesSpawned < totalEnemies){
            for(int i=0; i<enemiesPerSpawn; i++){
                if(EnemyList.Count < maxEnemiesOnScreen){
                    GameObject newEnemy;
                    SpawnBoss();
                    int index = Random.Range(0,waypoints.Count);
                    newEnemy = Instantiate(enemiesPrefabs[(int)Random.Range(0,(int)Mathf.Clamp(whichEnemiesToSpawn+1,0,enemiesPrefabs.Length))],
                        waypoints[index].startPoint.transform.position,Quaternion.identity) as GameObject;
                    newEnemy.transform.parent = enemies.transform;
                    newEnemy.GetComponent<Enemy>().SetStartParameters(waypoints[index].wayPoints, waypoints[index].endPoint);
                    enemiesSpawned++;
                }
            }
            yield return new WaitForSeconds(spawnTime);
            StartCoroutine(SpawnEnemy());
        }
    }

    private void SpawnBoss(){
        GameObject newEnemy;
        int maxBossesInWave = (int)((currentWave+1)/waveBossIncrease);
        int bossSpawn = (int)(totalEnemies/(maxBossesInWave+0.5f));
        //Debug.Log((currentWave+1) % wave + " " + (currentWave+1) + " " + enemiesSpawned + " " + bossSpawn);
        int index = Random.Range(0,waypoints.Count);
        if(((currentWave+1) % waveBossIncrease == 0 || currentWave+1 == totalWaves) &&
            enemiesSpawned % bossSpawn == 0 && enemiesSpawned >= bossSpawn){
            newEnemy = Instantiate(bossPrefab,waypoints[index].startPoint.transform.position,Quaternion.identity) as GameObject;
            newEnemy.transform.parent = enemies.transform;
            newEnemy.GetComponent<Enemy>().SetStartParameters(waypoints[index].wayPoints, waypoints[index].endPoint);
        }
    }

    public void RegisterEnemy(Enemy enemy){
        EnemyList.Add(enemy);
    }
    public void UnregisterEnemy(Enemy enemy){
        EnemyList.Remove(enemy);
        Destroy(enemy.gameObject);
    }
    public void AddMoney(int amount){
        Money += amount;
    }
    public void RemoveMoney(int amount){
        Money -= amount;
    }
    public void DestroyAllEnemies(){
        foreach(Enemy enemy in EnemyList){
            Destroy(enemy.gameObject);
        }
        EnemyList.Clear();
    }
    public void ShowMenu(){
        switch(currentState){
            case GameStatus.gameover:
                playText.text = "You lost!";
                AudioSource.PlayClipAtPoint(loseClip,transform.position,0.6f);
                StartCoroutine(GoToMainMenu_Cor(false));
                break;
            case GameStatus.next:
                playText.text = "Next wave";
                break;
            case GameStatus.beforePlay:
                playText.text = "Play";
                break;
            case GameStatus.win:
                playText.text = "You won!";
                PlayerPrefsManager.UnlockLevel(LEVEL_NUMBER+1);
                AudioSource.PlayClipAtPoint(winClip,transform.position,0.6f);
                StartCoroutine(GoToMainMenu_Cor(false));
                break;
        }
        playButtonText.gameObject.SetActive(true);
    }
    public void ButtonClicked(){
        switch(currentState){
            case GameStatus.gameover:
                //RestartGame();
                break;
            case GameStatus.next:
                StartWave();
                break;
            case GameStatus.beforePlay:
                RestartGame();
                break;
            case GameStatus.win:
                //RestartGame();
                break;
        }
        playButtonText.gameObject.SetActive(false);
    }
    public bool CanBuildThis(int amount){
        return money >= amount && currentState == GameStatus.play;
    }
    public void Quit(){
        Application.Quit();
    }
    public void AddHealth(){
        if(escaped > 0 && money > healCost){
            Escaped--;
            RemoveMoney(healCost);
        }
    }
    IEnumerator GoToMainMenu_Cor(bool save){
        yield return new WaitForSecondsRealtime(2);
        GoToMainMenu(save);
    }
    void GoToMainMenu(bool save){
         LevelManager.instance.LoadLevel("Menu");
    }

    public void PauseGame(){
        paused = !paused;
        if(paused){
            Time.timeScale = 0f;
            Time.fixedDeltaTime = 0f;
            Color col = Color.white;
            col.a = 1f;
            pauseButton.GetComponent<Image>().color = col;
        } else {
            Color col = Color.white;
            col.a = 0.5f;
            pauseButton.GetComponent<Image>().color = col;
            Time.timeScale = gameSpeed;
            Time.fixedDeltaTime = defaultFixeUpdateTime;
        }
    }
    public void ChangeGameSpeed(){
        if(gameSpeed == 1f)
            gameSpeed = 2f;
        else{
            if(gameSpeed == 2f)
                gameSpeed = 0.5f;
            else
                gameSpeed = 1f;
        }

        speedText.text = "x" + gameSpeed.ToString();
        Time.timeScale = gameSpeed;
        Time.fixedDeltaTime = defaultFixeUpdateTime;
    }

    public void TryMenu(){
        if(menuConfirm.activeSelf){ //already activated -> quit
            LevelManager.instance.LoadLevel("Menu");
        } else
            menuConfirm.SetActive(true);
    }
    public void TryMenu_no(){
        menuConfirm.SetActive(false);
    }

}
