using UnityEngine;
using System.Collections;


/*
	No one touch anything here please  --> JUST ALTER THE VARIABLES IN THE INSPECTOR
		- Rob
 */


[RequireComponent (typeof (NavMeshAgent))]
[RequireComponent (typeof (Animator))]
public class SiegeAnimator : MonoBehaviour 
{
	public float movementSpeed, stopAnimationDistance;
	public float animationMovementSpeed;

	Unit_Base script;
	GameObject target;
	float attackRange;
	bool isAttacking;
	int health;
	Vector2 smoothing = Vector2.zero;
	NavMeshAgent agent;
	Animator anim;
	bool isShooting = false;

	public GameObject lazerShotPrefab;
	public Transform spawnPoint;


	void Start ()
	{
		agent = GetComponent<NavMeshAgent> ();
		anim = GetComponent<Animator> ();

		script = GetComponent<Unit_Base> ();
		attackRange = script.attackRange;
		health = script.health; 
		isAttacking = script.isInCombat; 
		target = script.theTarget;

		
		// Dont auto update
		agent.updatePosition = false;
		agent.updateRotation = false;
	}
	
	void Update ()
	{
		attackRange = script.attackRange;
		health = script.health; 
		isAttacking = script.isInCombat;
		target = script.theTarget;
		anim.speed = animationMovementSpeed;  // playback for movement animation speed
		
		if (health == 0) {
			anim.SetBool ("die", true);
		} else {
		
			// Agent is moving
			if (Vector3.Distance (agent.nextPosition, transform.position) > 0.5f) {
				anim.SetBool ("aim", false);  // stop aiming
				//anim.SetBool ("fire", false);  // stop firing
			
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
				if (Vector3.Distance (transform.position, agent.destination) > 5f)
					transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, 0.3f * Time.deltaTime);
			
				// Update position
				transform.position = Vector3.Lerp (transform.position, agent.nextPosition, movementSpeed * Time.deltaTime);
				// Apply sprinting animations
				bool shouldMove = (Vector3.Distance (transform.position, agent.destination) > stopAnimationDistance) && agent.velocity.magnitude > 10f ? true : false;
				anim.SetBool ("moving", shouldMove);
				anim.SetFloat ("velx", deltaPos.x);
				anim.SetFloat ("vely", deltaPos.y);
			} 
		// Agent is in range ---> turn to it to fire
			else if (target != null && Vector3.Distance (agent.destination, transform.position) <= attackRange && isAttacking)
            {
				anim.speed = 0.9f;   // set animation playback speed for turning on the spot
			
				// Aim at target
				Vector3 lookDir = (target.transform.position - transform.position).normalized;
				Quaternion look2Target = Quaternion.identity; 
				if (lookDir != Vector3.zero)
					look2Target = Quaternion.LookRotation (lookDir);
				if (Quaternion.Angle (transform.rotation, look2Target) > 9f) {
					transform.rotation = Quaternion.Slerp (transform.rotation, look2Target, 1.2f * Time.deltaTime);
					anim.SetBool ("aim", true);
				
					Vector3 result = Vector3.Cross (transform.forward, new Vector3 (target.transform.position.x - transform.position.x, 0, target.transform.position.z - transform.position.z));
					float aimDir = (result.normalized == Vector3.up) ? 1f : 0; 
					anim.SetFloat ("aiming", aimDir); 
				} else
				{
					anim.SetBool ("aim", false);
					if(isShooting == false)
					{
						StartCoroutine ("shoot");
					}
				}
			}
		}
	}	
	IEnumerator shoot()
	{
		isShooting = true;
		GameObject shotInstant = Instantiate(lazerShotPrefab, spawnPoint.position, transform.rotation) as GameObject;
		shotInstant.GetComponent<LazerShot>().Fire(target.transform.position);
		yield return new WaitForSeconds (3f);
		isShooting = false;
	}
	void OnAnimatorMove ()
	{
		// Update position based on navmesh height
		Vector3 position = anim.rootPosition;
		position.y = agent.nextPosition.y;
		transform.position = position;
	}
}
