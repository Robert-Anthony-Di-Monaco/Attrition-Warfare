﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*	
  	CONTROLS WORLD EVENTS, HOLDS WORLD VARIABLES 

	NOTE ---> TRANSFORM HAS A "gameObject" variable, use it to access scripts 
				--->  thePlayer.gameObject.GetComponent<PlayerController>();               For everything else --> GetComponent<Unit_Base>();  
 */

public class WorldController : MonoBehaviour 
{
	// The Player
	public Transform thePlayer;   // not initialized cause Player isnt done yet ----	ROBERT WILL TAKE CARE OF THIS
	public EnemyController theEnemy;  				 
	public Transform playerBase,
					 enemyBase;

	// Holds all Player Units   
	public List<List<Transform>> playerSquads = new List<List<Transform>>();  // a list of squads and each squad is just a list of its units

	// Holds all Enemy Mobile Units
	public List<Transform> mobileEnemyUnits = new List<Transform> ();

	// Holds all Enemy Stationary Units --> turrets and towers
	public List<Transform> stationaryEnemyUnits = new List<Transform> ();

	//All units in the game, used to generated unique IDs
	public List<Unit_Base> allUnits = new List<Unit_Base>();

	void Start () 
	{
	
	}

	void Update () 
	{
	
	}
}