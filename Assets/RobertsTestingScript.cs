using UnityEngine;
using System.Collections;

public class RobertsTestingScript : MonoBehaviour {
	
	public Transform target;
	
	private NavMeshAgent agent;
	
	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent>();
		agent.SetDestination (target.position);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}


