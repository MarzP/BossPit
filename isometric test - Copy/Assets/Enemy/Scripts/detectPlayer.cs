using System;
using UnityEngine;

public class detectPlayer : MonoBehaviour
{

	//The target player
	private Transform player;
	//At what distance will the enemy be able to detect the player?
	public float detectRange = 10.0f;
	//Execute attack every x secounds
	public float attackSpeed = 1f;

	//the attack object
	public Rigidbody shell;

	//Players y + shellSpawnHeight = spawnpoint for shell
	public float shellSpawnHeight = 30f;
	public float launchForce = 1000f;
	public AudioSource engineIdle;

	private Rigidbody rb;
	private RaycastHit hit;
	private float nextActionTime = 0.0f;

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
		//Calculate distance between player
		float distance = Vector3.Distance(transform.position, player.position);

		//Get a vector3 in direction from enemy to player
		Vector3 rayDirection = player.transform.position - transform.position;

		//If the distance is smaller than the detectRange
		if (distance < detectRange)
		{	
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
					Debug.Log("Player in LOS!");
					//Execute every x secounds while player is in LOS
					if (Time.time > nextActionTime)
					{
						nextActionTime = Time.time + attackSpeed;
						FireAttack();
					}
				}
				//If player is not in line of sight
				else
				{
					Debug.Log("Cant see the player!");
				}
			}

		}
		else
		{
			engineIdle.Pause();
		}
	}

	private void FireAttack()
	{
		Debug.Log("BOOM!");
		Vector3 target = player.transform.position;
		target = new Vector3(target.x, target.y + shellSpawnHeight, target.z);
		Quaternion targetRotation = Quaternion.Euler(90, 0, 0);
		Rigidbody shellInstance = Instantiate(shell, target, targetRotation) as Rigidbody;
	}
}
