using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    public GameObject enemeyPrefab; // An enemy Prefab
    public GameObject playerPrefab; //The player prefab

    public MazeGenerator mazeGenerator; // Holds a reference to the maze Generator script
    public Transform mazeSpawn; //The transform of the Maze's parent object

    private GameObject player; //Holds reference to the player
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

    public int currentDiffcultyLevel; //Stores the current diffculty level
    public int minEnemyCount; //Min number of enemies to spawn
    void Awake() {
        currentDiffcultyLevel = DiffcultyLevel.diffcultyLevel;
        mazeGenerator.GenerateMaze(Random.Range(minSizeX, maxSizeX), Random.Range(minSizeY, maxSizeY)); // Instructs the mazeGenerator to generate a Maze
        mazeGenerator.InstantiateMaze(); //Instructs the mazeGenerator to instantiate that maze within the game world
        deadEnds = mazeGenerator.getDeadEndList(); // Retrieve a list of all deadends within the maze (to be used as spawn positions)
        SpawnPlayer();
    }

    // Start is called before the first frame update
    void Start() {
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
    }

    // Update is called once per frame
    void Update() {
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
            //spawn boss
            SpawnBoss();
            bossSpawned = true;
        }
        if (getPlayerHP() <= 0) {
            RegiesterPlayerDeath();
            Debug.Log("Player Died");

        }
     
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
        spawn.transform.localPosition = new Vector3(center.x, 2.5f, center.y);

        //Spawn the actual boss
        GameObject boss = Object.Instantiate(enemeyPrefab, Vector3.zero, Quaternion.identity);
        boss.transform.localPosition = spawn.transform.position; //use the empty game object position
        boss.GetComponent<TankHealth>().gameController = this; //Pass a reference to the boss health script
    }
    //Call this function to get the players transform
    public Transform getPlayerTransform() {
        return player.transform;
    }
    //Call this function to remove an enemy from the enemies list
    public void RegisterEnemyDeath(GameObject enemy) {
        if (bossSpawned) {
            //this will mean the boss died
            DiffcultyLevel.diffcultyLevel = currentDiffcultyLevel + 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
        DiffcultyLevel.diffcultyLevel = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    // End of Game Controller Class
}