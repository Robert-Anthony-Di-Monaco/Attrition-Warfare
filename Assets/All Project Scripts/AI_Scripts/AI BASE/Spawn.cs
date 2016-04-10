using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawn : MonoBehaviour {
	
	public GameObject melee, ranged, siege;
	public GameObject enemySpawnPoint;
	public GameObject emptySquad;
	
	private GameObject currentSquad;
	private int meleeUnits = 0, rangedUnits = 0, siegeUnits = 0;
	private int curMaxMelee = 3, curMaxRanged = 3, curMaxSiege = 0;
    public int faction;
	
	// Use this for initialization
	void Start () {
		if(faction == 1)
		{
			curMaxMelee *=6;
		}
	}

	float squadCounter = 5;
	float unitCounter = 0;
	// Update is called once per frame
	void Update () {
		if(squadCounter > 5)
		{
			if(currentSquad == null)
			{
				currentSquad = Instantiate (emptySquad, transform.position, Quaternion.identity) as GameObject;
				currentSquad.GetComponent<Squad>().advanceTarget = enemySpawnPoint.transform.position;
				currentSquad.GetComponent<Squad>().retreatTarget = this.transform.position;
			}
			if(unitCounter > 0.2f && meleeUnits++ < curMaxMelee)
			{
				GameObject temp1 = Instantiate(melee, transform.position, Quaternion.identity) as GameObject;
				//temp1.transform.localScale = new Vector3(10f,10f,10f);
				temp1.GetComponent<Unit_Melee>().enabled = true;
                temp1.GetComponent<Unit_Base>().faction = faction;
				currentSquad.GetComponent<Squad>().addUnit(temp1.GetComponent<Unit_Melee>());
				unitCounter = 0;
			}
			else if(unitCounter > 1.2f && rangedUnits++ < curMaxRanged)
			{
				GameObject temp1 = Instantiate(ranged, transform.position, Quaternion.identity) as GameObject;
				//temp1.transform.localScale = new Vector3(10f,10f,10f);
				temp1.GetComponent<Unit_Range>().enabled = true;
                temp1.GetComponent<Unit_Base>().faction = faction;
				currentSquad.GetComponent<Squad>().addUnit(temp1.GetComponent<Unit_Range>());
				unitCounter = 0;
			}
			else if(unitCounter > 1.2f && siegeUnits++ < curMaxSiege)
			{
				GameObject temp1 = Instantiate(siege, transform.position, Quaternion.identity) as GameObject;
				//temp1.transform.localScale = new Vector3(10f,10f,10f);
				temp1.GetComponent<Unit_Siege>().enabled = true;
                temp1.GetComponent<Unit_Base>().faction = faction;
				currentSquad.GetComponent<Squad>().addUnit(temp1.GetComponent<Unit_Siege>());
				unitCounter = 0;
				
				if(faction == 0)
					curMaxMelee *=8;
				
			}
			else
			{
				unitCounter += Time.deltaTime;
			}
			
			if(siegeUnits == curMaxSiege)
			{
				meleeUnits = 0;
				rangedUnits = 0;
				siegeUnits = 0;
				squadCounter = 0;
				unitCounter = 0;
				currentSquad = null;
				if(curMaxMelee < 25)
				{
					++curMaxMelee;
					++curMaxRanged;
					++curMaxSiege;
				}
			}
			
		}
		else
		{
			squadCounter += Time.deltaTime;
		}
		
		
	}
}
