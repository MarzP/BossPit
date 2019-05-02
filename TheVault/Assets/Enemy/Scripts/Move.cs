using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Move : MonoBehaviour
{
	public List<Vector3> Waypoints;
	private NavMeshAgent agent;
	private Vector3 currentWaypoint;
	private List<Vector2> deadEnds;
	public MazeGenerator mazeGenerator;

	private void Awake()
	{
		deadEnds = mazeGenerator.getDeadEndList();
		foreach (Vector2 v in deadEnds)
		{
			Waypoints.Add(new Vector3(v.x, 2.5f, v.y));
		}
	}
	private void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		SetNewDestination();
	}

	private void Update()
	{
		if ((transform.position - currentWaypoint).magnitude <= 1)
		{
			SetNewDestination();
		}
	}

	void SetNewDestination()
	{
		currentWaypoint = Waypoints[Random.Range(0, Waypoints.Count)];
		agent.SetDestination(currentWaypoint);
	}
}
