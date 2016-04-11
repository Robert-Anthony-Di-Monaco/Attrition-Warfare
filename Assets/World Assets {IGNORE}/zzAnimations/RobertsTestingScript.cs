﻿using UnityEngine;
using System.Collections;

public class RobertsTestingScript : MonoBehaviour 
{
	RaycastHit hitInfo = new RaycastHit();
	NavMeshAgent agent;
	
	void Start () {
		agent = GetComponent<NavMeshAgent> ();
	}

	void Update () 
	{
		if (Input.GetMouseButtonDown (0)) 
		{
			agent.Resume ();

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray.origin, ray.direction, out hitInfo))
				agent.destination = hitInfo.point;
		}
		if (Input.GetMouseButtonDown (1))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray.origin, ray.direction, out hitInfo))
				agent.destination = hitInfo.point;

			agent.Stop ();
		}
	}
}