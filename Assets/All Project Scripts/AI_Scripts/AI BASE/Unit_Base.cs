using UnityEngine;
using System.Collections;


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
	 			 damageOutput,
				 attackRange;
	public const int maxHealth = 100; 
	public int health = maxHealth;

	public int UpgradeLevel = 0;   // Determines this unit's abilities

	public bool isEnemy;   		   // Friendly or enemy unit 
								   //	  --> use this.squad.faction, details in Squad script
								   //     --> moved this to Squad script, that way you don't have to set it every time you instantiate a unit
								   //	  --> changed to int faction: might be used as an index, less confusing (enemies would attack when isEnemy is false)

	// THESE WILL BE DONE BY ROBERT ----> ignore for now
	public Animator anim; 

	public float deathAnimationLength = 0; // How long to wait before destroying the gameobject when the unit dies, see Kill()


	//**** Functions ******

	//Made this function virtual, it is overriden by children using the override keyword and base.Awake()
	//This is the only way to make everything in here happen in all children, while also letting each child do other things
	public virtual void Awake()
	{
		//anim = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();

		WorldController wc = GameObject.Find("WorldController").GetComponent<WorldController>();
		ID = wc.totalUnitsInstantiated;
		wc.totalUnitsInstantiated++;
		wc.allUnits.Add (this);



		/* In derived class Initialize that Unit's variables here

				speed = 
				damageOutput = 

				etc...
		 */
	}

	//Functions to be used by behaviour trees, call within Behaviour tree functions

	//Only for non-leader units
	public void MarchInFormation(){ //Will need to be changed if we don't want to update this every frame
		if(squad != null && squad.leader != null){
			//Transform the offset vector from local space to world space to take into account the leader's rotation
			NavMeshTarget = squad.transform.position + squad.transform.TransformVector(offsetFromAnchor);

			//Make the unit seek 1 world unit in front of the slot position,
			//  resolves issues with the unit sometimes facing the wrong direction when stationary
			NavMeshTarget += squad.transform.forward; 
			NavMeshSeek ();
		}
	}

	public bool isSquadLeader(){
		return (squad != null && squad.leader != null && squad.leader.ID == ID);
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
		if(NavMeshTarget != Vector3.zero)
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





}

