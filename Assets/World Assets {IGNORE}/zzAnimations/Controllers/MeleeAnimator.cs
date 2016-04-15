using UnityEngine;
using System.Collections;

/*
	Dont touch --> JUST ALTER THE VARIABLES IN THE INSPECTOR
		- Rob
*/

[RequireComponent (typeof (NavMeshAgent))]
[RequireComponent (typeof (Animator))]
public class MeleeAnimator : BaseAnimator
{
    new void Start()
    {
        base.Start();

        angularSpeed = 0.8f;
        angularAimingSpeed = 0;
        coolDown = 0;  // not used by melee unit
        angularAimingSpeed = 0; // not used either
    }

    new void Update()
    {
        base.Update();
        
        // Agent is in range ---> start attacking
        if ( target != null && isAttacking && Vector3.Distance(target.transform.position, transform.position) <= attackRange )
        {
            // Stop moving
            anim.SetBool("moving", false);
            anim.SetBool("injured", false);

            // Set attacking speed
            anim.speed = 1.5f;

            // Apply attacking animation 
            anim.SetInteger("attacking", (int)Random.Range(1f, 4f));  // randomly selects 1 of 4 different attacks
        }
        // Agent is moving
        else if (Vector3.Distance (agent.nextPosition, transform.position) > 0.5f)
        {			
			anim.SetInteger ("attacking", 0); // stop attacking

            // Update position and rotation
            Update_Transform();

            // Apply sprinting animations
            bool shouldMove = (Vector3.Distance(transform.position, agent.destination) > stopAnimationDistance) ? true : false;
            if (health >= 50f)
            {
                anim.SetBool("injured", false);
                anim.SetBool("moving", shouldMove);
            }
            else
            {
                anim.SetBool("moving", false);
                anim.SetBool("injured", shouldMove);
            }
        }

        if (agent.velocity.sqrMagnitude < 25f)
        {
            anim.SetBool("moving", false);
            anim.SetBool("injured", false);
        }
	}	
}
