﻿using UnityEngine;
using System.Collections;


/* I REMOVED THE PlayerHealthBar script ----> the HealthBar script the units use completely takes care of it. WE CAN ADD IT BACK LATER, but its small potatoes compared to what we need to do. 
 * 				- Robert
 * 
 * 		This script controls everything relating to the player ---> Health, AI, Commands, User input. 
 * 		WorldController has a reference to the player so it knows all about the player 
 */


public class PlayerController : MonoBehaviour
{
    public int terrainLayer = 9;
    public int enemyLayer =  8;
    public int allyLayer = 10;

	public float speed = 5f; 

	public const int maxPlayerHealth = 100;
	public int playerHealth = 1;
    public bool attackFlag;
    public bool squadCommandFlag = false;
    public bool squadAttackFlag = false;

    public GameObject attackMoveWaypoint;
    public GameObject moveWaypoint;
    Player_AI playerAI;
	
	void Awake () 
	{
        playerAI = GetComponent<Player_AI>();
	}

	void Update () 
	{
        ProcessKeys();
	}

    void ProcessKeys()
    {
        //this is to signal the next click is an attack move
        if (Input.GetKeyDown(KeyCode.A))
        {
            attackFlag = true;
        }
        //Right Mouse Click
        if (Input.GetMouseButtonDown(0))
        {
            //if the A button was pressed first then this is true and we issue an attack move order
            if (attackFlag)
            {
                clickToAttackMove();
            }
            else if (squadCommandFlag)
            {
                
                takeControlOfSquad();
            }
            else if(squadAttackFlag)
            {
                giveSquadAttackMoveOrder();
            }
            else
            {
                clickToMove();
            }
        }
        //SpaceBar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SnapCameraToPlayer();
        }

        //SQUAD DONTROL COMMANDS

        //R for command Squad
        if (Input.GetKeyDown(KeyCode.W))
        {
            squadCommandFlag = true;
        }

        if (Input.GetKeyDown(KeyCode.E) && playerAI.isSquadCommander)
        {
            squadAttackFlag = true;
        }
		if (Input.GetKeyDown (KeyCode.R) && playerAI.isSquadCommander) {
			playerAI.squad.stopControlling ();
			playerAI.isSquadCommander = false;
		}
		//Hide the victory/defeat message after it has been displayed
		if(Input.GetKeyDown(KeyCode.Escape)){
			//GameObject.Find("WorldController").GetComponent<WorldController>().message = null;
		}
    }


   
    //This is going to be called in the process keys function
    //it raycasts from the camera to the  mouse pointer and gets
    //the intersecting point we check if the point is on a enemy unit or on
    //the ground
    void clickToMove()
    {
        squadAttackFlag = false;
        squadCommandFlag = false;
        attackFlag = false;
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            //if we hit the terrain
            if (hit.collider.gameObject.layer.Equals(terrainLayer))
            {
                GameObject waypoint = Instantiate(moveWaypoint, hit.point, Quaternion.identity) as GameObject;
                //if a previous waypoint exists destroy it
                if (playerAI.target != null && playerAI.target.tag == "MoveWaypoint")
                {
                    Destroy(playerAI.target);
                }
                //create waypoint and move to it
                
                playerAI.target = waypoint;
                playerAI.isAttackOrder = false;
                return;
            }
            //if we hit the enemy
            else if (hit.collider.gameObject.layer.Equals(enemyLayer))
            {
                //if a previous waypoint exists destroy it
                if (playerAI.target != null && playerAI.target.tag == "MoveWaypoint")
                {
                    Destroy(playerAI.target);
                }
               
                playerAI.target = hit.collider.gameObject;
                playerAI.isAttackOrder = true;
                return;
            }

        }
    }

    void clickToAttackMove()
    {
        squadAttackFlag = false;
        squadCommandFlag = false;
        attackFlag = false;
        RaycastHit hit;

        //same as click to move but we need to set the attack flag in the AI to true
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            //if we hit the terrain
            if (hit.collider.gameObject.layer.Equals(terrainLayer))
            {
                GameObject waypoint = Instantiate(attackMoveWaypoint, hit.point, Quaternion.identity) as GameObject;
                //if a previous waypoint exists destroy it
                if (playerAI.target != null && playerAI.target.tag.Equals("MoveWaypoint"))
                {
                    //destroy previous waypoint
                    Destroy(playerAI.target);
                }
                //create waypoint and move to it
                playerAI.isAttackOrder = true;
                playerAI.target = waypoint;
                return;
            }
        }
        if (hit.collider.gameObject.layer.Equals(enemyLayer))
        {
            //if a previous waypoint exists destroy it
            if (playerAI.target != null && playerAI.target.tag == "MoveWaypoint")
            {
                //destroy previous waypoint
                Destroy(playerAI.target);
            }

            //sets the target to the clicked enemy
            //all movement and basic attacks should be handled by the Player_AI
            playerAI.target = hit.collider.gameObject;
            playerAI.isAttackOrder = true;
            return;
        }

    }

    public void SnapCameraToPlayer()
    {
        Camera.main.transform.parent.position = this.transform.position;
    }

    public void takeControlOfSquad()
    {
        Debug.Log("ControlSquad has been Called");
        squadCommandFlag = false;
        RaycastHit hit;
        //same as click to move we check if we clicked on an ally
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
             //if we hit the terrain
            if (hit.collider.gameObject.layer.Equals(allyLayer))
            {
                playerAI.isSquadCommander = true;
                hit.collider.gameObject.GetComponent<Unit_Base>().squad.giveControlTo(playerAI);
            }
        }
    }

    void giveSquadAttackMoveOrder()
    {
        RaycastHit hit;
        squadAttackFlag = false;
        squadCommandFlag = false;
        attackFlag = false;

        //First remove the player From the Squad
        Squad controlledSquad = playerAI.squad;
        playerAI.isSquadCommander = false;
        playerAI.squad.stopControlling();
        Unit_Base leader = controlledSquad.leader;
		
		//Make the squad a kamikaze squad
		controlledSquad.suicideAttack = true;

        //same code as attackMove really just modifies the unit/squad
        //AI variables instead of the playerAI

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            //if we hit the terrain
            if (hit.collider.gameObject.layer.Equals(terrainLayer))
            {

                leader.NavMeshTarget = hit.transform.position;
                leader.NavMeshSeek();
                return;
            }
            //Add a case for clicking directly on an enemy
            
        }

    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag.Equals("MoveWaypoint"))
        {
            Destroy(col.gameObject);
        }
    }

    
}