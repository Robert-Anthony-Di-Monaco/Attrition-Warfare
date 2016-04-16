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


public class Player_AI : Unit_Base
{
    // Player AI variables
    public GameObject basicShot;
    public Vector3 BulletOffset;
    public GameObject PlayerGUI;
    public float shotCoolDown = 0.5f;
    public float nextShotTime = 0f;
    public GameObject target;
    public bool isAttackOrder;
    public bool isSquadCommander;
	public Vector3 respawnPosition;

    //NavMeshAgent agent;
    public override void Awake() 
	{
		respawnPosition = transform.position;
        isInCombat = false;
        aimThreshold = 2f;
        enemyLayer = 8;
        health = maxHealth;
        attackRange = 150f;
		damageOutput = 64;
		attackCooldown = 0.75f;
        target = null;
        agent = GetComponent<NavMeshAgent>();
		InitBT();
		bt.Start();
	}
	
	private void InitBT()
	{
		bt = new BehaviorTree(Application.dataPath + "/All Project Scripts/AI_Scripts/Player Scripts/Player-AI-Tree.xml", this);
	}

    public override bool isPlayer() { return true; }

	public override void ApplyDamage(int amount)
	{
		health -= amount/4;
        PlayerGUI.GetComponent<PlayerGUI>().setHealth(health);
		if (health <= 0)
		{
			respawnPlayer ();
		}
	}

	public void respawnPlayer()
	{
		agent.enabled = false;
		transform.position = respawnPosition;
		agent.enabled = true;
		Destroy(target);
		health = maxHealth;
        PlayerGUI.GetComponent<PlayerGUI>().setHealth(health);
	}
	
    //checks if the player currently has an order to move
    [BTLeaf("has-move-order")]
    public bool hasMoveOrder()
    {
        return target != null;
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
        if (target == null)
        {
            return false;
        }
        if (target.tag.Equals("MoveWaypoint"))
        {
            return false;
        }
        else
        {
            return true;
        }
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

        if (Quaternion.Angle(transform.rotation, Quaternion.LookRotation(dirVect.normalized)) <= aimThreshold)
        {
            return true;
        }else {
            return false;
        }
    }

    [BTLeaf ("is-facings-nearest-enemy")]
    public bool _isFacingNearestEnemy()
    {
        GameObject closestEnemy = this.getClosestEnemy();
        if (closestEnemy == null)
        {
            return false;
        }
        Vector3 enemyDir = closestEnemy.transform.position - this.transform.position;
        float angleDifference = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(enemyDir.normalized));


        if (angleDifference <= aimThreshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    [BTLeaf ("move-to-target")]
    public BTCoroutine MoveToTarget()
    {
        isInCombat = false;
        //not sure this is nescessary but just in case target dies or is destroyed maybe
        if (target == null)
        {
            yield return BTNodeResult.Failure;
        }
        //if we are at our target set it to null
        else if (Vector3.Distance(target.transform.position, this.transform.position) < 0.5f)  
        {
           target = null;
           yield return BTNodeResult.Success;
        }
        //set destination to current target position
        else
        {
            this.agent.SetDestination(target.transform.position);
            yield return BTNodeResult.NotFinished;
        }
    }

    

    [BTLeaf("face-target")]
    public BTCoroutine faceTarget()
    {
        this.agent.SetDestination(this.transform.position);
        //check if we are within aim Threshold of the target
        Vector3 enemyDir = target.transform.position - this.transform.position;
        float angleDifference = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(enemyDir.normalized));

        if (angleDifference <= aimThreshold)
        {
            yield return BTNodeResult.Success;
        }
        else
        {
           // transform.forward = Vector3.RotateTowards(this.transform.forward, enemyDir, 3f, 180);
            yield return BTNodeResult.Success; 
        }
    }

    //handles the case where the shot is on cooldown
    [BTLeaf("shoot-target")]
    public BTCoroutine shootTarget()
    {
        isInCombat = true;
        this.agent.SetDestination(this.transform.position);
        if (target != null)
        {
            isInCombat = true;
            attack(target);
            yield return BTNodeResult.Success;
            ////we know we are already facing the target
            //if (Time.time > nextShotTime)
            //{
            //    nextShotTime = Time.time + shotCoolDown;
            //    attack(target);
            //    yield return BTNodeResult.Success;
            //}
            //else
            //{
            //    yield return BTNodeResult.NotFinished;
            //}
        }
        else
        {
            yield return BTNodeResult.Failure;
        }
    }

    //shoots the nearest enemy if the shot is off cooldown
    [BTLeaf ("shoot-nearest-enemy")]
    public BTCoroutine shootNearestEnemy()
    {
        isInCombat = true;
        this.agent.SetDestination(this.transform.position);
        GameObject closestEnemy = this.getClosestEnemy();
        if (closestEnemy != null)
        {
            isInCombat = true;
            attack(closestEnemy);
            yield return BTNodeResult.Success;
            //if (Time.time > nextShotTime)
            //{
            //    nextShotTime = Time.time + shotCoolDown;
            //    attack(closestEnemy);
            //    yield return BTNodeResult.Success;
            //}
            //else
            //{
            //    yield return BTNodeResult.NotFinished;
            //}
        }
        else
        {
            yield return BTNodeResult.Failure;
        }
    }


    [BTLeaf("face-nearest-enemy")]
    public BTCoroutine faceNearestEnemy()
    {
        this.agent.SetDestination(this.transform.position);
        GameObject nearestEnemy = this.getClosestEnemy();
        if (nearestEnemy != null)
        {
            Vector3 enemyDir = nearestEnemy.transform.position - this.transform.position;
            float angleDifference = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(enemyDir.normalized));

            if (angleDifference <= aimThreshold)
            {
                yield return BTNodeResult.Success;
            }
            else
            {
                // Animation Handles turning
                //transform.forward = Vector3.RotateTowards(this.transform.forward, enemyDir, 3f, 180);
                yield return BTNodeResult.NotFinished;
            }
        }
    }
	
    [BTLeaf ("move-to-squad-anchor")]
    public BTCoroutine moveToSquadAnchor()
    {
        yield return BTNodeResult.NotFinished;
    }

    [BTLeaf("idle")]
    public BTCoroutine idle()
    {
        isInCombat = false;
        yield return BTNodeResult.Success;
    }
}
