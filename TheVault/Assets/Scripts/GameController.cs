using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.AI;

public class GameController : MonoBehaviour {
    public GameObject enemeyPrefab; // An enemy Prefab
    public GameObject bossPrefab; // The boss prefab
    public GameObject playerPrefab; //The player prefab

    public MazeGenerator mazeGenerator; // Holds a reference to the maze Generator script
    public Transform mazeSpawn; //The transform of the Maze's parent object

    private GameObject player; //Holds reference to the player
    private GameObject boss; //Holds reference to the boss
    private List<GameObject> enemies; // List to hold all enemies
    private List<Vector2> deadEnds; // A list to hold potential spawn positions
    public List<GameObject> innerWalls; // A list to hold all the inner wall gameObjects

    public float wallDescentRate; //Controlls the speed at which inner walls descend when the boss fight begins
    public float wallShakeAmount; //Range the walls will move side to side to simulate shaking
    private bool bossFightTransition; //True when maze is transitioning to boss fight
    public float wallDescentTime; //When to stop the walls
    private float descentTimer; // How long the walls have been descending
    private bool bossSpawned; //Whether the boss has been Spawned

    // The min and max size the maze can be spawn at
    int minSizeX = 6;
    int minSizeY = 6;
    int maxSizeX = 10;
    int maxSizeY = 10;

    //Game Diffculty variables
    public int currentDiffcultyLevel; //Stores the current diffculty level
    public int minEnemyCount; //Min number of enemies to spawn
    private bool levelCleared; //Flag when level cleared
    private bool gameOver; //Flag when the game is over

    //UI Text boxes
    public TextMeshProUGUI gameOverText, levelComplete, startTips;

    //Timers for ui Components
    private float tipTimer, tipDuration, delayTimer;
    public float levelClearDelay;

    //Variables for health bar, should go in seperate script
    public Slider m_Slider;                             // The slider to represent how much health the tank currently has.
    public Image m_FillImage;                           // The image component of the slider.
    public Color m_FullHealthColor = Color.green;       // The color the health bar will be when on full health.
    public Color m_ZeroHealthColor = Color.red;         // The color the health bar will be when on no health.
    public float m_StartingHealth = 100f;               // The amount of health the player starts with.
    private float m_CurrentHealth;                      // How much health the player currently has.

	public Camera minimapCamera;

	public NavMeshSurface surface;

    public float score;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI currentLevel;

    void Awake() {
        currentDiffcultyLevel = DiffcultyLevel.diffcultyLevel;
        mazeGenerator.GenerateMaze(Random.Range(minSizeX, maxSizeX), Random.Range(minSizeY, maxSizeY)); // Instructs the mazeGenerator to generate a Maze
        mazeGenerator.InstantiateMaze(); //Instructs the mazeGenerator to instantiate that maze within the game world
        deadEnds = mazeGenerator.getDeadEndList(); // Retrieve a list of all deadends within the maze (to be used as spawn positions)

		//Bake navmesh
		surface.BuildNavMesh();

        SpawnPlayer();
        //Disabled all UI Text
        gameOverText.enabled = false;
        levelComplete.enabled = false;
        startTips.enabled = false;
    }

    // Start is called before the first frame update
    void Start() {
        //Start timer for Starting tips
        tipTimer = 0.0f;
        //Display tips
        startTips.enabled = true;
        gameOver = false;
        levelCleared = false;
        enemies = new List<GameObject>();
        //Spawn enemies based on diffculty level
        if (currentDiffcultyLevel < minEnemyCount) {
            SpawnEnemies(minEnemyCount);
        } else {
            SpawnEnemies(currentDiffcultyLevel);
        }
        innerWalls = mazeGenerator.getInnerWalls(); //Get the list of inner Walls from the generator
        bossFightTransition = false;
        bossSpawned = false;
        descentTimer = 0.0f;
        delayTimer = 0.0f;
        tipDuration = 3;
        if(currentDiffcultyLevel > 1) {
            startTips.text = "Level " + currentDiffcultyLevel;

            currentLevel.text = "Current Level: " + (currentDiffcultyLevel+1);
            scoreText.text = "Score: " + (score + 100 + getPlayerHP());
            //updateScore();
        }
        //updateScore();
        currentLevel.text = "Current Level: " + (currentDiffcultyLevel+1);
    }

    // Update is called once per frame
    void Update() {
        if(bossSpawned && boss.transform.position.y < player.transform.position.y - 3f) {
            RegisterEnemyDeath(boss);
        }
        if (!gameOver) {
            SetHealthUI();
        }

        if (levelCleared) {
            delayTimer += Time.deltaTime;
        }
        if(levelCleared && delayTimer > levelClearDelay && Input.GetButton("Jump")) {
            DiffcultyLevel.diffcultyLevel = currentDiffcultyLevel + 1;

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (gameOver && Input.GetButton("Jump")) {
            DiffcultyLevel.diffcultyLevel = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        //Increase timer
        tipTimer += Time.deltaTime;
        if(tipTimer > tipDuration) {
            startTips.enabled = false;
        }
        //Check if transitioning to boss Fight
        if (bossFightTransition) {
            descentTimer += Time.deltaTime; // Add to the time the walls have been descending
            for (int i = 0; i < innerWalls.Count; i++) {
                Vector3 wallPosition = innerWalls[i].transform.position;
                innerWalls[i].transform.position = new Vector3(wallPosition.x + Random.Range(-1 * wallShakeAmount, wallShakeAmount), wallPosition.y - wallDescentRate, wallPosition.z + Random.Range(-1 * wallShakeAmount, wallShakeAmount));
            }
        }
        //Check if the wall descent animation is finished
        if (descentTimer > wallDescentTime && !bossSpawned) {
            bossFightTransition = false; // Set transition flag to false
            //Destroy the inner walls
            for (int i = 0; i < innerWalls.Count; i++) {
                Destroy(innerWalls[i].gameObject);
            }
            innerWalls.Clear();
			//Bake navmesh
			surface.BuildNavMesh();
			//spawn boss
			SpawnBoss();
            bossSpawned = true;
		}
        if (!gameOver && getPlayerHP() <= 0) {
            RegiesterPlayerDeath();
            Debug.Log("Player Died");

        }
     
    }

    private void SetHealthUI() {
        // Set the slider's value appropriately.
        m_Slider.value = getPlayerHP();

        // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, getPlayerHP() / m_StartingHealth);
    }

    //function to spawn in the player
    void SpawnPlayer() {
        Vector2 spawn = deadEnds[Random.Range(0, deadEnds.Count)]; //Get a random spawn postion
        deadEnds.Remove(spawn); //remove spawn from the list of possible spawns

        //Instantiate an empty game object to hold the position for the player
        GameObject spawnSpace = new GameObject();
        spawnSpace.transform.SetParent(mazeSpawn);
        spawnSpace.transform.localPosition = new Vector3(spawn.x, 2.5f, spawn.y);

        //Instantiate the player
        player = Object.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        player.transform.localPosition = spawnSpace.transform.position; //use the empty game object position
        Destroy(spawnSpace); //Get rid of the space now that the player is inplace

		MinimapScript minimap = minimapCamera.GetComponent<MinimapScript>();
		minimap.player = player.transform;
    }

    //function to spawn the number of enemies specified in
    void SpawnEnemies(int numberOfEnemiesToSpawn) {
        // Spawns enemies until desired number or spawn postions are used up
        for (int i = 0; i < numberOfEnemiesToSpawn && deadEnds.Count > 0; i++) {
            Vector2 spawn = deadEnds[Random.Range(0, deadEnds.Count)]; //Get a random spawn postion
            deadEnds.Remove(spawn); //remove spawn from the list of possible spawns

            //Instantiate an empty game object to hold the position for the enemy
            GameObject spawnSpace = new GameObject();
            spawnSpace.transform.SetParent(mazeSpawn);
            spawnSpace.transform.localPosition = new Vector3(spawn.x, 1.5f, spawn.y);
            //Instantiate an enemy prefab 
            GameObject enemy = Object.Instantiate(enemeyPrefab, Vector3.zero, Quaternion.identity);
            enemy.transform.localPosition = spawnSpace.transform.position; //use the empty game object position
            Destroy(spawnSpace); //Get rid of the space now that the enemy is inplace
            enemy.GetComponent<TankHealth>().gameController = this; //Pass a reference to the enemy health script
            if (currentDiffcultyLevel <= 2) {
                enemy.GetComponent<detectPlayer>().attackSpeed = 3.5f - (0.5f * currentDiffcultyLevel);
            } else {
                enemy.GetComponent<detectPlayer>().attackSpeed = 2.5f - (0.1f * currentDiffcultyLevel);
            }
            enemies.Add(enemy); // Add enemey to enemies list
        }
    }
    //Spawns the boss enemy
    void SpawnBoss() {
        //get the position of the maze Center from maze Generator
        Vector2 center = mazeGenerator.getMazeCenter();
        //Instantiate an empty game object to hold the position for the boss
        GameObject spawn = new GameObject();
        spawn.transform.SetParent(mazeSpawn);
        spawn.transform.localPosition = new Vector3(center.x, 1.5f, center.y);

        //Spawn the actual boss
        boss = Object.Instantiate(bossPrefab, Vector3.zero, Quaternion.identity);
        boss.transform.localScale = new Vector3(2, 2, 2);
        //boss.transform.localPosition = spawn.transform.position; //use the empty game object position
		boss.GetComponent<NavMeshAgent>().Warp(spawn.transform.position);//use the empty game object position but with navmesh warp
		boss.GetComponent<TankHealth>().gameController = this; //Pass a reference to the boss health script
        if (currentDiffcultyLevel <= 2) {
            boss.GetComponent<detectPlayer>().attackSpeed = 2.5f - (0.5f * currentDiffcultyLevel);
        } else {
            boss.GetComponent<detectPlayer>().attackSpeed = 1f - (0.1f * currentDiffcultyLevel);
        }

    }
    public float requestEnemyStartingHealth() {
        if (bossSpawned) {
            return 200 + (35 * currentDiffcultyLevel);
        }
        return 100 + (25 * currentDiffcultyLevel);
    }
    //Call this function to get the players transform
    public Transform getPlayerTransform() {
        if (gameOver) {
            return null;
        }
        return player.transform;
    }
    //Call this function to remove an enemy from the enemies list
    public void RegisterEnemyDeath(GameObject enemy) {
        if (bossSpawned) {
            //this will mean the boss died
            levelCleared = true;
            levelComplete.enabled = true;
        } else {
            enemies.Remove(enemy);
            //if there are no more enemies begin the boss transition
            if (enemies.Count == 0) {
                bossFightTransition = true;
            }
        }
    }

    public float getPlayerHP() {
        float playerHP = player.GetComponent<charController>().playerCurrentHealth;
        return playerHP;
    }
    //Call This function when the player has died
    public void RegiesterPlayerDeath() {
        Destroy(player);
        for(int i = 0; i < enemies.Count; i++) {
            Destroy(enemies[i]);
        }
        if (bossSpawned) {
            Destroy(boss);
        }
        gameOverText.enabled = true;
        gameOver = true;
    }

    public void updateScore()
    {
        scoreText.text = "Score: " + score;
    }

    // End of Game Controller Class
}
