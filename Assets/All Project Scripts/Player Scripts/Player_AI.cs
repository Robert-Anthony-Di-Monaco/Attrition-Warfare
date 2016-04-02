using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

using Random = UnityEngine.Random;
using Coroutine = System.Collections.IEnumerator;
using BTCoroutine = System.Collections.Generic.IEnumerator<BTNodeResult>;


/*
 * 		This script controls the player AI 
 */


public class Player_AI : MonoBehaviour
{
    public float speed,
               damageOutput,
               attackRange;
    public const int maxHealth = 100;
    public int health = maxHealth;

    public float aimThreshold = 10f;

    //basic attack variables
    public GameObject basicShot;
    public float shotCoolDown = 0.5f;
    public float nextShotTime = 0f;
    //


    public GameObject target;
    public bool isAttackOrder;
    public bool isSquadCommander;

	
	private BehaviorTree bt;
    NavMeshAgent agent;
	void Awake () 
	{
        agent = GetComponent<NavMeshAgent>();
		InitBT();
		bt.Start();
	}
	
	private void InitBT()
	{
		bt = new BehaviorTree(Application.dataPath + "/Player-AI-Tree.xml", this);
	}

    //checks if the player currently has an order to move
    [BTLeaf("has-move-order")]
    public bool hasMoveOrder()
    {
        return target == null;
    }

    //check if the current order is an attack order or just a move order
    [BTLeafAttribute("is-order-attack-order")]
    public bool isOrderAttackOrder()
    {
        return isAttackOrder;
    }

    //check if the target is a unit or a place on the ground
    [BTLeaf("is-target-a-unit")]
    public bool isTargetAUnit()
    {
        if (target.tag.Equals("WaypointMarker"))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    //check if there in an enemy within our attakc range
    [BTLeaf ("is-enemy-in-range")]
    public bool isEnemyInRange()
    {
       int layerMask = 0; //TODO: CHANGE TO INCLUDE EVERYTHING BUT THE ENEMY LAYER
       return Physics.CheckSphere(this.transform.position, attackRange, layerMask);
    }

    //checks if the targeted enemy is within our attack range (different from above)
    [BTLeaf ("is-target-unit-in-range")]
    public bool isTargetunitInRange()
    {
        return ((target.transform.position - this.transform.position ).magnitude) < attackRange;
    }

    [BTLeaf ("is-commanding-squad")]
    public bool isCommandingSquad()
    {
        return isSquadCommander;
    }

    [BTLeaf ("is-facing-target")]
    public bool isFacingTarget()
    {
        Vector3 dirVect = target.transform.position - this.transform.position;

        if (Mathf.Abs(Vector3.Angle(this.transform.forward, dirVect)) < aimThreshold)
        {
            return true;
        }else {
            return false;
        }
    }
    [BTLeaf ("is-facing-nearest-enemy")]
    public bool isFacingNearestEnemy()
    {
        GameObject closestEnemy = this.getClosestEnemy();
        Vector3 enemyDir = closestEnemy.transform.position - this.transform.position;
        float angleDifference = Mathf.Abs(Vector3.Angle(this.transform.forward, enemyDir));


        if (angleDifference < aimThreshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //NOT A COROUTINE just used in one
    public GameObject getClosestEnemy()
    {
        //get nearest enemy in attack range
        int layerMask = 0;  //TODO: set to ignore everything but the enemy
        Collider[] cols = Physics.OverlapSphere(this.transform.position, attackRange, layerMask);
        GameObject closestEnemy = null;
        float closestDistance = float.MaxValue;

         //check for the closest enemy object
        foreach( Collider collision in cols){
            Vector3 colliderPos = collision.gameObject.transform.position;
            float currentDistance = Math.Abs((colliderPos - this.transform.position).magnitude);
            if(currentDistance < closestDistance){
                closestEnemy = collision.gameObject;
                closestDistance = currentDistance;
            }
        }
        return closestEnemy;
    }

    [BTLeaf ("move-to-target")]
    public BTCoroutine MoveToTarget()
    {

        //not sure this is nescessary but just in case target dies or is destroyed maybe
        if (target == null)
        {
            yield return BTNodeResult.Failure;
        }
        //if we are at our target set it to null
        else if ((target.transform.position - this.transform.position).magnitude < 0.5f)
        {
           target = null;
           yield return BTNodeResult.Success;
        }
        //set destination to current target position
        else
        {
            agent.SetDestination(target.transform.position);
            yield return BTNodeResult.NotFinished;
        }
    }

    //handles the case where the shot is on cooldown
    [BTLeaf ("shoot-target")]
    public BTCoroutine shootTarget()
    {
        //we know we are already facing the target
        if(Time.time > nextShotTime){
            //TODO: APPLY DAMAGE TO THE TARGET HERE MAYBE(IF WE DONT DECIDE TO DO IT ON COLLISION)
            Instantiate(basicShot, transform.position + (transform.forward * 1f), this.transform.rotation);
            nextShotTime = Time.time + shotCoolDown;
            yield return BTNodeResult.Success;
        }
        else
        {
            yield return BTNodeResult.NotFinished;
        }
    }

    [BTLeaf("face-target")]
    public BTCoroutine faceTarget()
    {
        //check if we are within aim Threshold of the target
        Vector3 enemyDir = target.transform.position - this.transform.position;
        float angleDifference = Mathf.Abs(Vector3.Angle(this.transform.forward, enemyDir));

        if (angleDifference < aimThreshold)
        {
            yield return BTNodeResult.Success;
        }
        else
        {
            //TODO: tune the facing speed here
            transform.forward = Vector3.RotateTowards(this.transform.forward, enemyDir, 3f, 180);
            yield return BTNodeResult.Success;
        }
    }

  

    //shoots the nearest enemy if the shot is off cooldown
    [BTLeaf ("shoot-nearest-enemy")]
    public BTCoroutine shootNearestEnemy()
    {
        GameObject closestEnemy = this.getClosestEnemy();

        if (Time.time > nextShotTime)
        {
            //TODO: APPLY DAMAGE TO THE TARGET HERE MAYBE(IF WE DONT DECIDE TO DO IT ON COLLISION)
            Instantiate(basicShot, transform.position + (transform.forward * 1f), this.transform.rotation);
            nextShotTime = Time.time + shotCoolDown;
            yield return BTNodeResult.Success;
        }
        else
        {
            yield return BTNodeResult.NotFinished;
        }
    }
}



/*		TEMPLATES FOR MAKING THE AI ACTIONS 


	// CONDITION TEMPLATE  --> how to make a condition
	[BTLeaf(" this is the name the BehaviourTree sees, it is an Alias for this function ")]
	public bool exampleFunction()
	{
		return true;
	}


	// LEAF TEMPLATE  --> how to make an action
	[BTLeaf( this is the name the BehaviourTree sees, it is an Alias for this Coroutine ")]
	public BTCoroutine exampleCoroutine()
	{
		IF YOUVE NEVER USED COROUTINES --> dont worry, theyre awesome
		--> They are like functions except they are capable of pausing their execution between frames  
		--> THIS PAUSING IS DONE LIKE THIS: yield return new WaitForSeconds(3);  // Coroutine waits for 3 seconds before continuing to execute  

		// THE FOLLOWING ARE FOR THE BEHAVIOUR TREE ---> ASK ROBERT IF YOU DONT UNDERSTAND ---> dont feel stupid for asking, this wastes time, just ask me, we are all students here, its fine
		yield return BTNodeResult.Success;		   // Return this when Action is successful 
		yield return BTNodeResult.NotFinished;     // Return this when Action is still in progress
		yield return BTNodeResult.Failure;		   // Return this when Action failed
	}
 */