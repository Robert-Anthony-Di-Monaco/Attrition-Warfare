using UnityEngine;
using System.Collections;

/*
	Dont touch --> JUST ALTER THE VARIABLES IN THE INSPECTOR
		- Rob
*/

[RequireComponent (typeof (NavMeshAgent))]
[RequireComponent (typeof (Animator))]
public class SiegeAnimator : BaseAnimator
{
    new void Start()
    {
        base.Start();

        angularSpeed = 0.6f;
        angularAimingSpeed = 1.1f;
        coolDown = 3f;
        injuredAnimationSpeed = movementAnimationSpeed; // not used by siege units
    }

    new void Update()
    {
        base.Update();

        // Agent is in range ---> turn on the spot to face it
        if (target != null && isAttacking && Vector3.Distance(target.transform.position, transform.position) <= attackRange)
        {
            anim.SetBool("moving", false);  // stop moving
            anim.speed = 0.9f;   // set animation speed for turning on the spot

            // Turn on the spot to face target
            AimTowards();
        }
        // Agent is moving
        else if (Vector3.Distance (agent.nextPosition, transform.position) > 0.5f)
        {
            anim.SetBool ("aim", false);  // stop aiming

            // Update position and rotation
            Update_Transform();

            // Apply sprinting animations
            bool shouldMove = (Vector3.Distance (transform.position, agent.destination) > stopAnimationDistance) ? true : false;
			anim.SetBool ("moving", shouldMove);
		} 

        if (agent.velocity.sqrMagnitude < 25f)
        {
            anim.SetBool("moving", false);
        }
	}	
}
