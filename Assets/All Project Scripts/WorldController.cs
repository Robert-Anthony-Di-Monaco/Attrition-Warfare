using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*	
  	CONTROLS WORLD EVENTS, HOLDS WORLD VARIABLES 
    
 */


public class WorldController : MonoBehaviour 
{
	// The Player
	public Transform thePlayer;

    public AudioSource ambient1, ambient2, ambient3;

	// Holds all Player Units   
	public List<List<Transform>> playerSquads = new List<List<Transform>>();  // a list of squads and each squad is just a list of its units

	// Holds all Enemy Mobile Units
	public List<Transform> mobileEnemyUnits = new List<Transform> ();

	// Holds all Enemy Stationary Units --> turrets and towers
	public List<Transform> stationaryEnemyUnits = new List<Transform> ();

	//All units in the game, counter used to generated unique IDs
	public int totalUnitsInstantiated = 0;
	public List<Unit_Base> allUnits = new List<Unit_Base>();

	//All squads in the game
	public int totalSquadsInstantiated = 0;
	public List<Squad> allSquads = new List<Squad>();
	
	int allyCrystalsDestroyed = 0, enemyCrystalsDestroyed = 0;
	
	void Start () 
	{
		LakeAreas.Initialize();
	}

	void Update () 
	{
        if (ambient1.isPlaying == false && ambient2.isPlaying == false && ambient3.isPlaying == false)
        {
            int selection = Random.Range(1, 4);
            if (selection == 1) ambient1.Play();
            else if (selection == 2) ambient2.Play();
            else ambient3.Play();
        }
	}
	
	public void victory()
	{
		if(++enemyCrystalsDestroyed == 2)
		{
			// put winning code here for splash screen or whatever
			Debug.Log("You win! Congratulations on having no life!");
		}
	}
	
	public void defeat()
	{
		if(++allyCrystalsDestroyed == 2)
		{
			// put losing code here for splash screen or whatever
			Debug.Log("You lose! You must suck!");
		}
	}
	
	
}