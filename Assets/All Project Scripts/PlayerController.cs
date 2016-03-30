using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	public float speed = 5f; 
	public Transform NavMeshTarget;
	NavMeshAgent agent;

	void Start () 
	{
		agent = GetComponent<NavMeshAgent>();
	}

	void Update () 
	{
//		float moveHorizontal = Input.GetAxis ("Horizontal");
//		float moveVertical = Input.GetAxis ("Vertical");
//		Vector3 movement = new Vector3 (moveHorizontal, 0, moveVertical) * speed * Time.deltaTime;
//		transform.Translate(movement.x, 0, movement.z);

		agent.SetDestination (NavMeshTarget.position);
	}
}
