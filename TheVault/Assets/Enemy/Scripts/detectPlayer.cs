using System;
using UnityEngine;

public class detectPlayer : MonoBehaviour
{
	//The target player
	private Transform player;
	//At what distance will the enemy be able to detect the player?
	public float detectRange = 10.0f;
	//Execute attack every x secounds
	public float attackSpeed;
	//the attack object
	public Rigidbody shell;
    //sphere attack object (damage)
    public Rigidbody attackSphere;

	//Players y + shellSpawnHeight = spawnpoint for shell
	public float shellSpawnHeight = 30f;
	public float launchForce = 1000f;
	public AudioSource engineIdle;

	//Variables needed for detecting player and fire shell.
	private Rigidbody rb;
	private RaycastHit hit;
	public float nextActionTime = 0.0f;

	private void Start()
	{
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
		rb = GetComponent<Rigidbody>();

		//codelines make enemy stand in place when looking at player
		rb.centerOfMass = Vector3.zero;
		rb.freezeRotation = true;
	}

	void Update()
	{
        nextActionTime += Time.deltaTime;
        //Calculate distance between player
        float distance = Vector3.Distance(transform.position, player.position);

		//Get a vector3 in direction from enemy to player
		Vector3 rayDirection = player.transform.position - transform.position;

		//If the distance is smaller than the detectRange
		if (distance < detectRange)
		{	
			//Play tanksound if its not playing
			if (!engineIdle.isPlaying)
			{
				engineIdle.Play();
			}
			Vector3 lookAtVector = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
			//Look at the player
			transform.LookAt(lookAtVector);

			//Debug.DrawRay(transform.position, rayDirection, Color.red);	//Draw test ray

			//Cast rayCast towards player
			if (Physics.Raycast(transform.position, rayDirection, out hit))
			{
				//If player is in line of sight
				if (hit.transform == player)
				{
					//Execute every x secounds while player is in LOS
					if (nextActionTime > attackSpeed)
					{
						FireAttack();
                        nextActionTime = 0.0f;
                    }
                }
				//If player is not in line of sight
				else
				{
					//Debug.Log("Cant see the player!");
				}
			}
		}
		else
		{
			engineIdle.Pause();
		}
	}

	//Method to spawn a falling bombshell over the players position.
	private void FireAttack()
	{
		Vector3 target = player.transform.position;
		target = new Vector3(target.x, target.y + shellSpawnHeight, target.z);
		Quaternion targetRotation = Quaternion.Euler(90, 0, 0);
		Rigidbody shellInstance = Instantiate(shell, target, targetRotation) as Rigidbody;
	}

    //// Method to spawn spheres from every direction
    //private void FireSpheres() {
    //    Quaternion targetRotation = Quaternion.Euler(90, 0, 0);
    //    Rigidbody attackSphereInstance = Instantiate(attackSphere, transform.forward, targetRotation) as Rigidbody;

    //}
}
