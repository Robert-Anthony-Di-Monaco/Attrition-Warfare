using UnityEngine;
using System.Collections;

/*
	Dont touch --> JUST ALTER THE VARIABLES IN THE INSPECTOR
		- Rob
*/

[RequireComponent (typeof (NavMeshAgent))]
[RequireComponent (typeof (Animator))]
public class RangeAnimator : BaseAnimator
{
    new void Start()
    {
        base.Start();

        angularSpeed = 1.1f;
        angularAimingSpeed = 4.2f;
        coolDown = 1.5f;
    }

    new void Update()
    {
        base.Update();

        // Agent is in range ---> turn on the spot to face it
        if (target != null && isAttacking && Vector3.Distance(target.transform.position, transform.position) <= attackRange)
        {
            // Stop moving
            anim.SetBool("moving", false);
            anim.SetBool("injured", false);

            anim.speed = 1.5f;   // set animation playback speed for turning on the spot

            // Aim at target
            Vector3 lookDir = (target.transform.position - transform.position).normalized;
            Quaternion look2Target = Quaternion.identity;
            if (lookDir != Vector3.zero)
                look2Target = Quaternion.LookRotation(lookDir);
            if (Quaternion.Angle(transform.rotation, look2Target) > 2f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, look2Target, 4.2f * Time.deltaTime);
                anim.SetBool("aim", true);

                Vector3 result = Vector3.Cross(transform.forward, new Vector3(target.transform.position.x - transform.position.x, 0, target.transform.position.z - transform.position.z));
                float aimDir = (result.normalized == Vector3.up) ? 1f : 0;
                anim.SetFloat("aiming", aimDir);
            }
            else
            {
                anim.SetBool("aim", false);
                if (isShooting == false)
                {
                    StartCoroutine("shoot");
                }
            }
        }
        // Agent is moving
        else if (Vector3.Distance (agent.nextPosition, transform.position) > 0.5f)
        {
			anim.SetBool ("aim", false);  // stop aiming

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
