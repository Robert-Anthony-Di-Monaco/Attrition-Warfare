using UnityEngine;
using System.Collections;

/*
	Dont touch --> JUST ALTER THE VARIABLES IN THE INSPECTOR
		- Rob

    This is exactly like the RangeAnimator class --> kept seperate for now --> may delete later on
*/

[RequireComponent (typeof (NavMeshAgent))]
[RequireComponent (typeof (Animator))]
public class PlayerAnimator : BaseAnimator
{
    new void Start()
	{
        base.Start();

        angularSpeed = 1.1f;
        angularAimingSpeed = 4.2f;
        coolDown = 1.5f;
    }
	
	new void Update ()
	{
        base.Update();

        // Agent is in range ---> turn on the spot to face it
        if (target != null && isAttacking && Vector3.Distance(target.transform.position, transform.position) <= attackRange)
        {
            // Stop moving
            anim.SetBool("moving", false);
            anim.SetBool("injured", false);
            anim.speed = 1.5f;   // set animation speed for turning on the spot

            // Turn on the spot to face target
            AimTowards();
        }
        // Agent is moving
        else if (Vector3.Distance(agent.nextPosition, transform.position) > 0.5f)
        {
            anim.SetBool("aim", false);  // stop aiming

            // Update position and rotation
            Update_Transform();

            // Apply proper movement animations
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
