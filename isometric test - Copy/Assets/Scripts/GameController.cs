using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameController : MonoBehaviour {
    public MazeGenerator mazeGenerator; // Holds a reference to the maze Generator script
    public Transform mazeSpawn; //The transform of the Maze's parent object
    public GameObject enemeyPrefab; // An enemy Prefab

    private List<GameObject> enemies; // List to hold all enemies
    private List<Vector2> deadEnds; // A list to hold potential spawn positions
    private List<GameObject> innerWalls; // A list to hold all the inner wall gameObjects

    public float wallDescentRate; //Controlls the speed at which inner walls descend when the boss fight begins
    public float wallShakeAmount; //Range the walls will move side to side to simulate shaking
    private bool bossFightTransition; //True when maze is transitioning to boss fight

    // The min and max size the maze can be spawn at
    int minSizeX = 6;
    int minSizeY = 6;
    int maxSizeX = 10;
    int maxSizeY = 10;

    public int desiredEnemies; // Desired number of Enemies

    public int diffcultyLevel; //Stores the current diffculty level

    // Start is called before the first frame update
    void Start() {
        enemies = new List<GameObject>();
        mazeGenerator.GenerateMaze(Random.Range(minSizeX, maxSizeX), Random.Range(minSizeY, maxSizeY)); // Instructs the mazeGenerator to generate a Maze
        mazeGenerator.InstantiateMaze(); //Instructs the mazeGenerator to instantiate that maze within the game world
        deadEnds = mazeGenerator.getDeadEndList(); // Retrieve a list of all deadends within the maze (to be used as spawn positions
        SpawnEnemies(desiredEnemies); //Spawn enemies
        innerWalls = mazeGenerator.getInnerWalls(); //Get the list of inner Walls from the generator
        bossFightTransition = false;
    }

    // Update is called once per frame
    void Update() {
        //temp to test transition
        if (Input.GetButton("Fire3")) {
            bossFightTransition = true;
        }
        //Check if transitioning to boss Fight
        if (bossFightTransition) {
            for(int i = 0; i < innerWalls.Count; i++) {
                Vector3 wallPosition = innerWalls[i].transform.position;
                innerWalls[i].transform.position = new Vector3(wallPosition.x + Random.Range(-1 * wallShakeAmount, wallShakeAmount), wallPosition.y - wallDescentRate, wallPosition.z + Random.Range(-1 * wallShakeAmount, wallShakeAmount));
            }
        }
    }

    //function to spawn the number of enemies specified in
    void SpawnEnemies(int numberOfEnemiesToSpawn) {
        // Spawns enemies until desired number or spawn postions are used up
        for (int i = 0; i < numberOfEnemiesToSpawn || deadEnds.Count <= 0; i++) {
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
            enemies.Add(enemy); // Add enemey to enemies list
        }
    }
    //Call this function when the bossPit becomes available to the player
    void unlockBossPit() {

    }
    //Call This function when the player has started the boss fight
    void createBossPit() {

    }
    // End of Game Controller Class
}