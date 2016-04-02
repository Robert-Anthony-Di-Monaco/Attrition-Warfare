using UnityEngine;
using System.Collections;


/* I REMOVED THE PlayerHealthBar script ----> the HealthBar script the units use completely takes care of it. WE CAN ADD IT BACK LATER, but its small potatoes compared to what we need to do. 
 * 				- Robert
 * 
 * 		This script controls everything relating to the player ---> Health, AI, Commands, User input. 
 * 		WorldController has a reference to the player so it knows all about the player 
 */


public class PlayerController : MonoBehaviour
{
	public float speed = 5f; 

	public const int maxPlayerHealth = 100;
	public int playerHealth;

	public Transform NavMeshTarget;
	NavMeshAgent agent;
	
	void Awake () 
	{
		agent = GetComponent<NavMeshAgent>();
	}

	void Update () 
	{
		// 	this will be deleted
//		float moveHorizontal = Input.GetAxis ("Horizontal");
//		float moveVertical = Input.GetAxis ("Vertical");
//		Vector3 movement = new Vector3 (moveHorizontal, 0, moveVertical) * speed * Time.deltaTime;
//		transform.Translate(movement.x, 0, movement.z);

		agent.SetDestination (NavMeshTarget.position);
	}
}