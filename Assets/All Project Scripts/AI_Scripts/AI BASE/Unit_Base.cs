using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Coroutine = System.Collections.IEnumerator;
using BTCoroutine = System.Collections.Generic.IEnumerator<BTNodeResult>;


public class Unit_Base : MonoBehaviour 
{
	//**** Unit Variables ****
    [HideInInspector]
	public int ID;   // Uniquely identifies this unit --> used in WorldController's allUnits list
	public int faction;  // Whether this unit is an ally or an enemy     
	public Squad squad;  // This unit's squad	

    [HideInInspector]
    public NavMeshAgent agent;
    [HideInInspector]
    public Vector3 NavMeshTarget;  // For following the squad leader, call MarchInFormation() instead 

    [HideInInspector]
    public Vector3 offsetFromAnchor = Vector3.zero;   // this unit's slot position in formation
    
    [HideInInspector]
    public float attackRange,
                 visionRange,
                 aimThreshold,
                 nextAttackTime,
                 attackCooldown;
	public const int maxHealth = 100;
    [HideInInspector]
    public int health = maxHealth;

    [HideInInspector]
    public int enemyLayer, damageOutput;
    public bool isInCombat;
    [HideInInspector]
    public GameObject theTarget;    // The target this unit is currently attacking --> needed by animations 

    [HideInInspector]
    public float deathAnimationLength = 3f; 

	public BehaviorTree bt;
	
	public virtual void Awake()
	{
		agent = gameObject.GetComponent<NavMeshAgent>();
        isInCombat = false;
		WorldController wc = GameObject.Find("WorldController").GetComponent<WorldController>();
		ID = wc.totalUnitsInstantiated;
		wc.totalUnitsInstantiated++;
		wc.allUnits.Add (this);

        aimThreshold = 10f;
       
		InitBT();
		bt.Start();
	}
    public void Start()
    {
        layerSetUp();
    }

	void Update()
	{
		if(isInCombat == false)
			theTarget = null;
	}
	
	private void InitBT()
	{
        // All units will use the same Behaviour Tree
		bt = new BehaviorTree(Application.dataPath + "/All Project Scripts/AI_Scripts/Behaviour-AI-Tree.xml", this);
	}
	
	void FixedUpdate()
    {

		//Remember that his gets inherited by ALL subclasses: units, player, buildings, turrets, crystal

        //if(isSquadLeader())
        //{
        //    NavMeshSeek();
        //}
        //else
        //{
        //    MarchInFormation();
        //}

	}
	
	//The time check for attack is already done just write the instantiate in here
    public void attack(GameObject targetEnemy)
    {
		theTarget = targetEnemy;
        targetEnemy.SendMessage("ApplyDamage", damageOutput);
		nextAttackTime = Time.time + attackCooldown;
    }
	
	// Finds closest enemy 
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
	

	//Only for non-leader units

    [BTLeaf("march-in-formation")]
	public BTCoroutine MarchInFormation(){ //Will need to be changed if we don't want to update this every frame
		if(squad != null && squad.leader != null){
			//Transform the offset vector from local space to world space to take into account the leader's rotation
			NavMeshTarget = squad.transform.position + squad.transform.TransformVector(offsetFromAnchor);
			NavMeshTarget = new Vector3 (NavMeshTarget.x, transform.position.y, NavMeshTarget.z);
		

			//Check if the unit's slot position is inside a lake, if it is go to the leader's position instead
			if (LakeAreas.isInsideLake (NavMeshTarget)) {
				NavMeshTarget = squad.transform.position;
			}
			
			//Make the unit seek 1 world unit in front of the slot position,
			//  resolves issues with the unit sometimes facing the wrong direction when stationary
			NavMeshTarget += squad.transform.forward; 
			NavMeshSeek ();
            yield return BTNodeResult.NotFinished;
		}
	}

    [BTLeaf("is-squad-Leader")]
	public bool isSquadLeader()
    {
		return (squad != null && squad.leader != null && squad.leader.ID == ID);
	}
	
	[BTLeaf("is-overwhelmed")]
    public bool isOverwhelmed()
    {
		if(squad.suicideAttack)
			return false;
		enemyCols = Physics.OverlapSphere(this.transform.position, visionRange, enemyLayer);
		if(enemyCols.Length == 0)
			return false;
		Collider[] allyCols = Physics.OverlapSphere(this.transform.position, visionRange, 1 << this.gameObject.layer);
		double ratio = (double)(allyCols.Length) / enemyCols.Length;
        return ratio < 0.5;
    }
	
	[BTLeaf("is-squad-retreating")]
    public bool isSquadRetreating()
    {
		return squad != null && squad.isRetreating;
    }
	
    [BTLeaf ("is-squad-in-combat")]
    public bool isSquadInCombat(){
        if (squad != null) { 
            return squad.leader.isInCombat;
        }
        return false;
    }


    [BTLeaf ("is-enemy-in-vision-range")]
    public bool isEnemyInVisionRange()
	{
		if(Physics.CheckSphere(this.transform.position, visionRange, enemyLayer)){
			return true;
		}
		else{
			isInCombat = false;
            if (squad != null)
            {
                squad.isInCombat = false;
            }
            return false;
		}
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
        float angleDifference = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(enemyDir.normalized));

        if (angleDifference < aimThreshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    [BTLeaf ("attack-nearest-enemy")]
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

    [BTLeaf ("pursue-nearest-enemy")]
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


    [BTLeaf ("move-to-leader-target")]
    public BTCoroutine MoveToLeaderTarget()
    {
		if(squad.formUp)
		{
			if(squad.preFormUpTime < 0.5f)
			{
				squad.preFormUpTime += Time.deltaTime;
			}
			else if(squad.preFormUpTime < 0.8f)
			{
				squad.preFormUpTime += Time.deltaTime;
				squad.leader.transform.GetComponent<NavMeshAgent>().Stop();
			}
			else
			{
				squad.formUp = false;
				squad.preFormUpTime = 0;
				squad.leader.transform.GetComponent<NavMeshAgent>().Resume();
			}
				
		}
        squad.leader.NavMeshTarget = squad.advanceTarget;
        squad.leader.NavMeshSeek();
        squad.isInCombat = false;
        yield return BTNodeResult.NotFinished;
    }
	
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
    
	Collider[] enemyCols;
	[BTLeaf ("retreat")]
    public BTCoroutine retreat()
    {
        Collider[] allyCols = Physics.OverlapSphere(this.transform.position, visionRange, this.gameObject.layer);
		
		if ((double)(allyCols.Length) / enemyCols.Length < 1)
		{
			squad.isRetreating = true;
			squad.leader.NavMeshTarget = squad.retreatTarget;
			squad.leader.NavMeshSeek();
			squad.isInCombat = false;
			squad.leader.isInCombat = false;
			yield return BTNodeResult.NotFinished;
		}
		else
		{
			squad.isRetreating = false;
			yield return BTNodeResult.Success;
		}
    }

	// Unit dies
	public IEnumerator Kill()
    {
		if (squad != null)
			squad.removeUnit (this);
			
		WorldController wc = GameObject.Find ("WorldController").GetComponent<WorldController> ();

		wc.allUnits.Remove (this);
        //if this unit belongs to other lists in the world controller, remove it here

        //Destroy the game object after the death animation finishes
        yield return new WaitForSeconds(deathAnimationLength);
        Destroy(this.gameObject);
	}
	//These are overloaded to return true in their respective classes
	public virtual bool isPlayer(){ return false; }
	public virtual bool isMelee(){ return false; }
	public virtual bool isRange(){ return false; }
	public virtual bool isSiege(){ return false; }
	public virtual bool isBuilding(){ return false; }
	public virtual bool isCrystal(){ return false; }

	// **************************  SHARED FUNCTIONS BY ALL UNITS  **************************
	// This and only this is used to move the NPC-unit
	public void NavMeshSeek()
	{
		if((NavMeshTarget - transform.position) != Vector3.zero)
			agent.SetDestination (NavMeshTarget); 
	}
	// Stop moving the NPC-unit
	public void NavMeshStop()
	{
		agent.Stop ();
	}

	// Apply damage to this unit 
	public virtual void ApplyDamage(int amount)
	{
		health -= amount;
		if (health <= 0)
		{
			Kill();		
		}
	}
	// Heal this unit  
	public void Heal(int amount) 
	{
		health += amount;
	}

    public void layerSetUp()
    {
        //Set enemyLayer to the layer of the opposing faction
        if (faction == 0)
        {
            this.gameObject.layer =  10; //ally layer;
            enemyLayer =  1 << 8;

        }
        else if (faction == 1)
        {
            this.gameObject.layer =  8; //enemy layer;
            enemyLayer = 1 << 10;
        }
    }
}

