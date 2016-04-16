using UnityEngine;
using System.Collections;

/*
	Dont touch --> JUST ALTER THE VARIABLES IN THE INSPECTOR
		- Rob

    This is exactly like the RangeAnimator class --> its seperate because PlayerAI was handled differently
*/

[RequireComponent (typeof (NavMeshAgent))]
[RequireComponent (typeof (Animator))]
public class PlayerAnimator : BaseAnimator
{ 
	public Player_AI playerScript;

    new void Start()
	{
        base.Start();

		playerScript = GetComponent<Player_AI>();

        angularSpeed = 1.1f;
        angularAimingSpeed = 4.2f;
        coolDown = 0.5f;
        aimThreshold = 2f;
    }
	
	new void Update ()
	{
		target = playerScript.target;
		attackRange = script.attackRange;
		isAttacking = script.isInCombat;
		health = script.health;

		// Check if dead
		if (health == 0)
		{
			anim.SetBool("die", true);
			return;
		}

		// Set speed for movement animations
		if (health >= 50)
			anim.speed = movementAnimationSpeed;
		else
			anim.speed = injuredAnimationSpeed;

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
