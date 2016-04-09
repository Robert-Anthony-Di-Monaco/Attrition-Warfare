using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawn : MonoBehaviour {
	
	public GameObject melee;
	public GameObject enemySpawnPoint;
	public GameObject emptySquad;
	private GameObject currentSquad;
	private int meleeUnits = 0;
	// Use this for initialization
	void Start () {
		
	}
	float squadCounter = 0;
	float unitCounter = 0;
	// Update is called once per frame
	void Update () {
		if(squadCounter > 10)
		{
			if(currentSquad == null)
			{
				currentSquad = Instantiate (emptySquad, transform.position, Quaternion.identity) as GameObject;
				currentSquad.GetComponent<Squad>().target = enemySpawnPoint.transform.position;
			}
			if(unitCounter > 1.2f && meleeUnits++ < 5)
			{
				GameObject temp1 = Instantiate(melee, transform.position, Quaternion.identity) as GameObject;
				//temp1.transform.localScale = new Vector3(10f,10f,10f);
				temp1.GetComponent<Unit_Melee>().enabled = true;
				currentSquad.GetComponent<Squad>().addUnit(temp1.GetComponent<Unit_Melee>());
				unitCounter = 0;
			}
			else
			{
				unitCounter += Time.deltaTime;
			}
			
			if(meleeUnits == 6)
			{
				meleeUnits = 0;
				squadCounter = 0;
				unitCounter = 0;
				currentSquad = null;
			}
			
		}
		else
		{
			squadCounter += Time.deltaTime;
		}
		
		
	}
}
