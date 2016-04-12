using UnityEngine;
using System.Collections;


/*
	No one touch anything here please  --> JUST ALTER THE VARIABLES IN THE INSPECTOR
		- Rob
 */


[RequireComponent (typeof (NavMeshAgent))]
[RequireComponent (typeof (Animator))]
public class MeleeAnimator : MonoBehaviour 
{
	public float movementSpeed, stopAnimationDistance;
	public float animationMovementSpeed, animationInjuredSpeed;
	
	float attackRange;
	int health;
	Vector2 smoothing = Vector2.zero;
	NavMeshAgent agent;
	Animator anim;
	
	void Start ()
	{
		agent = GetComponent<NavMeshAgent> ();
		anim = GetComponent<Animator> ();
		attackRange = 1f;//GetComponent<Unit_Base>().attackRange;
		health = 55;//GetComponent<Unit_Base>().health; 
		
		// Dont auto update
		agent.updatePosition = false;
		agent.updateRotation = false;
	}
	
	void Update ()
	{
		anim.speed = animationMovementSpeed;  // playback for movement animation speed
		
		if (health == 0) 
		{
			anim.SetBool ("die", true);
		}
		
		// Agent is moving
		if (Vector3.Distance(agent.nextPosition, transform.position) > 0.5f) 
		{			
			anim.SetBool ("attacking", false); // stop attacking

			// World space change
			Vector3 positionChange = agent.nextPosition - transform.position;  
			
			// Get local space change
			float dx = Vector3.Dot (transform.right, positionChange);
			float dy = Vector3.Dot (transform.forward, positionChange);
			// Smoothing
			Vector2 deltaPos = new Vector2 (dx, dy);
			float smooth = Mathf.Min (1.0f, Time.deltaTime / 0.15f);
			smoothing = Vector2.Lerp (smoothing, deltaPos, smooth);
			deltaPos = smoothing / Time.deltaTime;
			
			// Calculate rotation to look in
			Vector3 lookDir = positionChange.normalized;
			Quaternion lookRotation = Quaternion.identity;
			if (lookDir != Vector3.zero)
				lookRotation = Quaternion.LookRotation (lookDir);
			
			// Update rotation
			if (Vector3.Distance (transform.position, agent.destination) > 0.5f)
				transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, 0.5f * Time.deltaTime);
			
			// Update position
			transform.position = Vector3.Lerp (transform.position, agent.nextPosition, movementSpeed * Time.deltaTime);
			// Apply sprinting animations
			bool shouldMove = (Vector3.Distance (transform.position, agent.destination) > stopAnimationDistance) && agent.velocity.magnitude > 10f ? true : false;
			if(health >= 50f)
			{
				anim.SetBool ("injured", false);
				anim.SetBool ("moving", shouldMove);
			}
			else
			{
				anim.speed = animationInjuredSpeed;   // set animation playback speed for injured animations
				anim.SetBool ("moving", false);
				anim.SetBool ("injured", shouldMove);
			}
			anim.SetFloat ("velx", deltaPos.x);
			anim.SetFloat ("vely", deltaPos.y);
		} 
//		// Agent is in range ---> start attacking
//		else if(Vector3.Distance(agent.destination, transform.position) <= attackRange)   
//		{
//			anim.speed = 1.45f;
//
//			// Apply attacking animation sequence
//			anim.SetBool ("attacking", true);
//		}
	}	
	void OnAnimatorMove ()
	{
		// Update position based on navmesh height
		Vector3 position = anim.rootPosition;
		position.y = agent.nextPosition.y;
		transform.position = position;
	}
}
