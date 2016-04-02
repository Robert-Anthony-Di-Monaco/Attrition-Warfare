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
	public float speed,
	 			 damageOutput,
				 attackRange;
	public const int maxHealth = 100; 
	public int health = maxHealth;

	public int ID,   			   // Uniquely identify this unit
			   squadID;   		   // Identify this Unit's squad
	public int UpgradeLevel = 0;   // Determines this unit's abilities
	public bool isEnemy;   		   // Friendly or enemy unit

	public Transform NavMeshTarget;  // This unit's current target ---> to access its script use NavMeshTarget.gameObject.GetComponent<Unit_Base>();

	// THESE WILL BE DONE BY ROBERT ----> ignore for now
	Animator anim;  
	NavMeshAgent agent;

	void Awake()
	{
		//anim = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();


		/* In derived class Initialize that Unit's variables here

				speed = 
				damageOutput = 

				etc...
		 */
	}

// **********************************************************  SHARED FUNCTIONS BY ALL UNITS  ***********************************************
	// This and only this is used to move the NPC-unit
	public void NavMeshSeek()
	{
		if(NavMeshTarget != null)
			agent.SetDestination (NavMeshTarget.position); 
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
			Destroy(this.gameObject);		
		}
	}
	// Heal this unit   --> the HealthBar script takes care of the rest
	public void Heal(int amount) 
	{
		health += amount;
	}
}

