using UnityEngine;
using System.Collections;

public class AI_Base : MonoBehaviour 
{
	public float speed = 50f;

	public const int maxHealth = 100;
	public int health = maxHealth;

	public int UpgradeLevel = 0;   // Units level

	NavMeshAgent agent;

	void Start () 
	{
		agent = GetComponent<NavMeshAgent>();
	}

	void Update () 
	{

	}
	
	// Primary movement function for all NPCs
	public void Seek(Vector3 target)
	{
		agent.SetDestination (target); 
	}

	// 
	public void ApplyDamage(int amount)
	{
		health -= amount;
		if (health <= 0)
		{
			Destroy(this.gameObject);		
		}
	}
}