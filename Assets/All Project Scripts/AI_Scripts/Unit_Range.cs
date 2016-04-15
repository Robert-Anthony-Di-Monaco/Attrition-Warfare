using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

using Random = UnityEngine.Random;
using Coroutine = System.Collections.IEnumerator;
using BTCoroutine = System.Collections.Generic.IEnumerator<BTNodeResult>;

public class Unit_Range : Unit_Base
{

    //public BehaviorTree bt;
    public override void Awake()
    {
        attackRange = 125;
        attackCooldown = 0.4f;
        damageOutput = 24;
        visionRange = 200f;
        isInCombat = false;
        base.Awake();

        //InitBT();
        //bt.Start();
    }
    /*
	private void InitBT()
	{
		bt = new BehaviorTree(Application.dataPath + "/All Project Scripts/AI_Scripts/AI Range/Range-AI-Tree.xml", this);
	}
*/
    public override void ApplyDamage(int amount)
    {
        health -= amount / 3;
        if (health <= 0)
        {
            StartCoroutine("Kill");
        }
    }

    public void getNewAnchorPosition(int unitIndex, int numMeleeLines)
    {
		int numRange = squad.rangeUnits.Count;
		int perLine = squad.numUnitsPerLine;
		float unitDistance = squad.distanceBetweenUnits;

		int inFirstLine = numRange % perLine;
		if (inFirstLine == 0)
			inFirstLine = perLine;

		int line = unitIndex / inFirstLine;
		if(line != 0)
			line = ( (unitIndex - inFirstLine) / perLine) + 1;

		//Ranged units stand in a line behind the melee units by a distance of (1.5 * unitDistance)
		float zOffset = 0;
		zOffset += ((-1.5f * unitDistance) - ((numMeleeLines - 1) * unitDistance)); //Get the offset of the last melee unit line
		zOffset += ((-1.5f * unitDistance) - (line * unitDistance)); //Add the offset for this range unit's line

		//Lines up the units side by side (comments in Unit_Melee)
		float xOffset;
		if (line == 0) {
			xOffset = ((float)unitIndex / inFirstLine) * (unitDistance * inFirstLine);
			xOffset -= (unitDistance / 2) * (inFirstLine - 1);
		} else {
			xOffset = ((float)(unitIndex % perLine) / perLine) * (unitDistance * perLine);
			xOffset -= (unitDistance / 2) * (perLine - 1);
		}

		offsetFromAnchor = new Vector3 (xOffset, 0, zOffset);
	}

	//Overloads the function in Unit_Base
	public override bool isRange(){
		return true;
	}

/*
    //The time check for attack is already done just write the instantiate in here
    public void attack(GameObject targetEnemy)
    {
        Debug.Log("Ranged is Attacking!");
        targetEnemy.SendMessage("ApplyDamage", damageOutput);
    }


    //NOT A COROUTINE just used in one
    public GameObject getClosestEnemy()
    {
        //get nearest enemy in attack range
        Collider[] cols = Physics.OverlapSphere(this.transform.position, visionRange, enemyLayer);
        GameObject closestEnemy = null;
        float closestDistance = float.MaxValue;

        //check for the closest enemy object
        foreach (Collider collision in cols)
        {
            Vector3 colliderPos = collision.gameObject.transform.position;
            float currentDistance = Math.Abs((colliderPos - this.transform.position).magnitude);
            if (currentDistance < closestDistance)
            {
                closestEnemy = collision.gameObject;
                closestDistance = currentDistance;
            }
        }
        return closestEnemy;
    }
	
	
    [BTLeaf("is-squad-in-combat")]
    public bool isSquadInCombat()
    {
        if (squad != null)
        {
            return squad.leader.isInCombat;
        }
        return false;
    }


    [BTLeaf("is-enemy-in-vision-range")]
    public bool isEnemyInVisionRange()
    {
        return Physics.CheckSphere(this.transform.position, visionRange, enemyLayer);
    }

    [BTLeaf("is-enemy-in-attack-range")]
    public bool isEnemyInAttackRange()
    {
        return Physics.CheckSphere(this.transform.position, attackRange, enemyLayer);
    }

    [BTLeaf("is-facing-nearest-enemy")]
    public bool isFacingNearestEnemy()
    {
        GameObject closestEnemy = this.getClosestEnemy();
        if (closestEnemy == null)
        {
            return false;
        }
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

    [BTLeaf("face-nearest-enemy")]
    public BTCoroutine faceNearestEnemy()
    {

        this.agent.SetDestination(this.transform.position);
        GameObject nearestEnemy = this.getClosestEnemy();
        Vector3 enemyDir = nearestEnemy.transform.position - this.transform.position;
        float angleDifference = Mathf.Abs(Vector3.Angle(this.transform.forward, enemyDir));

        if (angleDifference < aimThreshold)
        {
            yield return BTNodeResult.Success;
        }
        else
        {
            //TODO: tune the facing speed here
            transform.forward = Vector3.RotateTowards(this.transform.forward, enemyDir, 3f, 180);
            yield return BTNodeResult.NotFinished;
        }
    }

    [BTLeaf("attack-nearest-enemy")]
    public BTCoroutine attackNearestEnemy()
    {
        this.agent.SetDestination(this.transform.position);
        GameObject closestEnemy = this.getClosestEnemy();
        if (closestEnemy != null)
        {

            if (Time.time > nextAttackTime)
            {
                attack(closestEnemy);
                yield return BTNodeResult.Success;
            }
            else
            {
                yield return BTNodeResult.NotFinished;
            }
        }
        else
        {
            yield return BTNodeResult.Failure;
        }
    }

    [BTLeaf("pursue-nearest-enemy")]
    public BTCoroutine pursueNearestEnemy()
    {
        GameObject closestEnemy = this.getClosestEnemy();
        if (closestEnemy != null)
        {
            if ((closestEnemy.transform.position - this.transform.position).magnitude < attackRange)
            {
                squad.isInCombat = true;
                this.isInCombat = true;
                yield return BTNodeResult.Success;
            }
            else
            {
                squad.isInCombat = true;
                this.isInCombat = true;
                NavMeshTarget = closestEnemy.transform.position;
                NavMeshSeek();
                yield return BTNodeResult.NotFinished;
            }
        }

        else
        {
            yield return BTNodeResult.Failure;
        }
    }


    [BTLeaf("move-to-leader-target")]
    public BTCoroutine MoveToLeaderTarget()
    {
        squad.leader.NavMeshTarget = squad.advanceTarget;
        squad.leader.NavMeshSeek();
        squad.isInCombat = false;
        yield return BTNodeResult.NotFinished;

    }
	
	// LEAFS and CONDITIONS definitions ---> SEE TEMPLATES BELOW FOR HOW TO DO THEM!!!!!!!!
	[BTLeaf("has-target")]
	public bool hasTarget()   // Does this unit have a target
	{
		if(NavMeshTarget != Vector3.zero)
			return true;
		else 
			return false;
	}
	[BTLeaf("seek-target")]
	public BTCoroutine Seek()   // Seek target
	{
		NavMeshSeek ();

		if (Vector3.Distance (NavMeshTarget, transform.position) < attackRange)
			yield return BTNodeResult.Success;
		else
			yield return BTNodeResult.NotFinished;
	}
	
	*/
	
}
	
	


