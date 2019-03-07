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

    // The min and max size the maze can be spawn at
    int minSizeX = 6;
    int minSizeY = 6;
    int maxSizeX = 10;
    int maxSizeY = 10;

    public int desiredEnemies; // Desired number of Enemies


    // Start is called before the first frame update
    void Start() {
        enemies = new List<GameObject>();
        mazeGenerator.GenerateMaze(Random.Range(minSizeX, maxSizeX), Random.Range(minSizeY, maxSizeY)); // Instructs the mazeGenerator to generate a Maze
        mazeGenerator.InstantiateMaze(); //Instructs the mazeGenerator to instantiate that maze within the game world
        deadEnds = mazeGenerator.getDeadEndList(); // Retrieve a list of all deadends within the maze (to be used as spawn positions
        SpawnEnemies(desiredEnemies); //Spawn enemies
    }

    // Update is called once per frame
    void Update() {
        
    }

    //function to spawn the number of enemies specified in
    void SpawnEnemies(int numberOfEnemiesToSpawn) {
        // Spawns enemies until desired number or spawn postions are used up
        for (int i = 0; i < numberOfEnemiesToSpawn || deadEnds.Count <= 0; i++) {
            Vector2 spawn = deadEnds[Random.Range(0, deadEnds.Count)]; //Get a random spawn postion
            deadEnds.Remove(spawn); //remove spawn from the list of possible spawns

            //Instantiate an enemy prefab 
            GameObject enemy = Object.Instantiate(enemeyPrefab, Vector3.zero, Quaternion.identity);
            enemy.transform.SetParent(mazeSpawn);
            enemy.transform.localPosition = new Vector3(spawn.x, 1.5f, spawn.y);
            enemies.Add(enemy); // Add enemey to enemies list
        }
    }
   
}