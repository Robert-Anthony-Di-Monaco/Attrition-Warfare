using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Coroutine = System.Collections.IEnumerator;
using BTCoroutine = System.Collections.Generic.IEnumerator<BTNodeResult>;

/************************************************************************************************************************************************************************
	TO MAKE SURE THERE IS NO CONFUSION THIS IS WHAT EACH TERM MEANS:
		Unit --> a single NPC; its type is one of the following:
									Melee 
									Range
									Siege
									Tower --> the player's HomeBase, the EnemyBase, and all towers inbetween which need to be destroyed in order to advance
									Turret --> between the player's HomeBase and the EnemyBase  --> they are hazards for the player + his units
 ***********************************************************************************************************************************************************************/



public class Unit_Base : MonoBehaviour 
{
	//**** Unit Variables shared by all unit types ****

	//These have been implemented
	//***************************
	public int ID;	 			   // Uniquely identify this unit by its index in the WorldController's allUnits list

	public int faction;            // Represents what side this unit fights for

	public Squad squad;   		   // Points to this Unit's squad
	// IMPORTANT: not in this script
	// this.squad.isInCombat -> if a unit needs to enter combat, make the whole squad enter combat,
	//                              can be changed if exiting combat is tricky
	// this.squad.leader -> this unit's squad leader (Unit_Base type)

	public NavMeshAgent agent;	   // See Awake()

	//Changed NavMeshTarget to be a Vector3 from Transform
	public Vector3 NavMeshTarget;  // This unit's target position/destination, call NavMeshSeek() after setting
								   // If the unit just needs to follow the squad leader, call MarchInFormation() instead (inherited)

	//This unit's slot position in formation, handled in the Squad script
	//Use this.MarchInFormation() for non-leader units;
	public Vector3 offsetFromAnchor = Vector3.zero; 



	//These have not yet been implemented
	//***********************************
	public Unit_Base Target;		//The enemy that this unit is targetting
									//Anything targettable should inherit Unit_Base

    public float speed, //Should just use navmesh speed through agent.speed
                 attackRange,
                 visionRange,
                 aimThreshold,
                 nextAttackTime,
                 attackCooldown;
	public const int maxHealth = 100; 
	public int health = maxHealth;

	public int UpgradeLevel = 0;   // Determines this unit's abilities

    public int enemyLayer;
    public int damageOutput;
    public bool isInCombat;
	//public bool isEnemy;   		   // Friendly or enemy unit 
								   //	  --> use this.squad.faction, details in Squad script
								   //     --> moved this to Squad script, that way you don't have to set it every time you instantiate a unit
								   //	  --> changed to int faction: might be used as an index, less confusing (enemies would attack when isEnemy is false)

	// THESE WILL BE DONE BY ROBERT ----> ignore for now
	public Animator anim; 

	public float deathAnimationLength = 0; // How long to wait before destroying the gameobject when the unit dies, see Kill()


	//**** Functions ******

	//Made this function virtual, it is overriden by children using the override keyword and base.Awake()
	//This is the only way to make everything in here happen in all children, while also letting each child do other things
	public BehaviorTree bt;
	
	public virtual void Awake()
	{
		//anim = GetComponent<Animator>();
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
	
	
	private void InitBT()
	{
		bt = new BehaviorTree(Application.dataPath + "/All Project Scripts/AI_Scripts/AI Melee/Melee-AI-Tree.xml", this);
	}
	
	void FixedUpdate(){
		
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
	
	//Functions to be used by behaviour trees, call within Behaviour tree functions

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
	public bool isSquadLeader(){
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

    [BTLeaf ("face-nearest-enemy")]
    public BTCoroutine faceNearestEnemy()
    {
        this.agent.SetDestination(this.transform.position);
        GameObject nearestEnemy = this.getClosestEnemy();
        if (nearestEnemy != null)
        {
            
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
        yield return BTNodeResult.Failure;
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
    
	
	Collider[] enemyCols;
	[BTLeaf ("retreat")]
    public BTCoroutine retreat()
    {
        Collider[] allyCols = Physics.OverlapSphere(this.transform.position, visionRange, this.gameObject.layer);
		
		if ((double)(allyCols.Length) / enemyCols.Length < 1)
		{
			Debug.Log(faction + " is retreating!");
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

	//This is an example of how handle the unit dying,
	//  could be used for the Player depending on how we handle the player dying
	public void Kill(){

		if (squad != null)
			squad.removeUnit (this);
			
		WorldController wc = GameObject.Find ("WorldController").GetComponent<WorldController> ();

		wc.allUnits.Remove (this);
		   //if this unit belongs to other lists in the world controller, remove it here
		 

		//Start death animation here
		 
		//Destroy the game object after the death animation finishes, or even later
		Destroy(gameObject, deathAnimationLength);

	}
	//These are overloaded to return true in their respective classes
	public virtual bool isPlayer(){ return false; }
	public virtual bool isMelee(){ return false; }
	public virtual bool isRange(){ return false; }
	public virtual bool isSiege(){ return false; }
	public virtual bool isBuilding(){ return false; }

	// **************************  SHARED FUNCTIONS BY ALL UNITS  **************************
	// This and only this is used to move the NPC-unit
	public void NavMeshSeek()
	{
		if((NavMeshTarget - transform.position) != Vector3.zero)
			agent.SetDestination (NavMeshTarget); //changed from navmeshtarget.position
	}
	// Stop moving the NPC-unit
	public void NavMeshStop()
	{
		agent.Stop ();
	}

	// Apply damage to this unit  --> the HealthBar script takes care of the rest
	public void ApplyDamage(int amount)
	{
		health -= amount;
		if (health <= 0)
		{
			Kill();		
		}
	}
	// Heal this unit   --> the HealthBar script takes care of the rest
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

