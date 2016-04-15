using UnityEngine;
using System.Collections;

/*
    Handles Animation
*/

public class BaseAnimator : MonoBehaviour
{
    public float movementSpeed, stopAnimationDistance,
                 movementAnimationSpeed, injuredAnimationSpeed;
    protected float angularSpeed, angularAimingSpeed;

    protected Unit_Base script;
    protected GameObject target;
    protected float attackRange;
    protected bool isAttacking;
    protected bool isShooting = false;
    protected float coolDown;
    protected int health;

    Vector2 smoothing = Vector2.zero;
    protected NavMeshAgent agent;
    protected Animator anim;

    public GameObject lazerShotPrefab;
    public Transform spawnPoint;

    protected void Start ()
    {
        // Initialize variables
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        // Get variables from AI scripts
        script = GetComponent<Unit_Base>();
        target = script.theTarget;
        attackRange = script.attackRange;
        isAttacking = script.isInCombat;
        health = script.health;
        
        // Dont auto update
        agent.updatePosition = false;
        agent.updateRotation = false;
    }
	
	protected void Update ()
    {
        target = script.theTarget;
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
    }

    protected void Update_Transform()
    {
        // World space change
        Vector3 positionChange = agent.nextPosition - transform.position;

        // Get local space change
        float dx = Vector3.Dot(transform.right, positionChange);
        float dy = Vector3.Dot(transform.forward, positionChange);

        // Smoothing
        Vector2 deltaPos = new Vector2(dx, dy);
        float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
        smoothing = Vector2.Lerp(smoothing, deltaPos, smooth);
        deltaPos = smoothing / Time.deltaTime;

        // Calculate direction to in 
        Vector3 lookDir = positionChange.normalized;
        Quaternion lookRotation = Quaternion.identity;
        if (lookDir != Vector3.zero)
            lookRotation = Quaternion.LookRotation(lookDir);

        // Update rotation
        if (Vector3.Distance(transform.position, agent.destination) > 0.5f)  // To prevent rotating on the spot
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, angularSpeed * Time.deltaTime);

        // Update position
        transform.position = Vector3.Lerp(transform.position, agent.nextPosition, movementSpeed * Time.deltaTime);

        // Set floats in animator for blend tree 
        anim.SetFloat("velx", deltaPos.x);
        anim.SetFloat("vely", deltaPos.y);
    }
    protected void OnAnimatorMove()
    {
        // Update position based on navmesh height
        Vector3 position = anim.rootPosition;
        position.y = agent.nextPosition.y;
        transform.position = position;
    }
    protected IEnumerator shoot()
    {
        isShooting = true;
        GameObject shotInstant = Instantiate(lazerShotPrefab, spawnPoint.position, transform.rotation) as GameObject;
        shotInstant.GetComponent<LazerShot>().Fire(target.transform.position);
        yield return new WaitForSeconds(coolDown);
        isShooting = false;
    }
}