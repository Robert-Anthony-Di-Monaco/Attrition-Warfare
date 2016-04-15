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
        injuredAnimationSpeed = movementAnimationSpeed; // not used by siege unit
    }

    new void Update()
    {
        base.Update();

        // Agent is in range ---> turn on the spot to face it
        if (target != null && isAttacking && Vector3.Distance(target.transform.position, transform.position) <= attackRange)
        {
            anim.SetBool("moving", false);  // stop moving
            anim.speed = 0.9f;   // set animation playback speed for turning on the spot

            // Aim at target
            Vector3 lookDir = (target.transform.position - transform.position).normalized;
            Quaternion look2Target = Quaternion.identity;
            if (lookDir != Vector3.zero)
                look2Target = Quaternion.LookRotation(lookDir);
            if (Quaternion.Angle(transform.rotation, look2Target) > 9f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, look2Target, 1.1f * Time.deltaTime);
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
            bool shouldMove = (Vector3.Distance (transform.position, agent.destination) > stopAnimationDistance) ? true : false;
			anim.SetBool ("moving", shouldMove);
		} 

        if (agent.velocity.sqrMagnitude < 25f)
        {
            anim.SetBool("moving", false);
        }
	}	
}
