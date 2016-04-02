using UnityEngine;
using System.Collections;


/* I REMOVED THE PlayerHealthBar script ----> the HealthBar script the units use completely takes care of it. WE CAN ADD IT BACK LATER, but its small potatoes compared to what we need to do. 
 * 				- Robert
 * 
 * 		This script controls everything relating to the player ---> Health, AI, Commands, User input. 
 * 		WorldController has a reference to the player so it knows all about the player 
 */


public class PlayerController : MonoBehaviour
{
	public float speed = 5f; 

	public const int maxPlayerHealth = 100;
	public int playerHealth;

	public Transform NavMeshTarget;
    public
	NavMeshAgent agent;
    Player_AI playerAI;
	
	void Awake () 
	{
        playerAI = GetComponent<Player_AI>();
	}

	void Update () 
	{
		// 	this will be deleted
//		float moveHorizontal = Input.GetAxis ("Horizontal");
//		float moveVertical = Input.GetAxis ("Vertical");
//		Vector3 movement = new Vector3 (moveHorizontal, 0, moveVertical) * speed * Time.deltaTime;
//		transform.Translate(movement.x, 0, movement.z);

		//agent.SetDestination (NavMeshTarget.position);
	}



   
    //This is going to be called in the process keys function
    //it raycasts to the from the camera to the  mouse pointer and gets
    //the intersecting point we check if the point is on a enemy unit or on a
    //the ground
    void clickToMove()
    {

        RaycastHit hit;

        int navMeshLayer = 1;
        LayerMask navMesh = 1 << navMeshLayer;
        int enemyLayer = 3; //TODO: FIX LAYERMASKS
        LayerMask  enemy  = 1 << enemyLayer;

        //if we hit the enemy
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, enemy))
        {
            //sets the target to the clicked enemy
            //all movement and basic attacks should be handled by the Player_AI
            playerAI.target = hit.collider.gameObject;
            return;
        }
        else if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, navMesh))
        {
            //instantiate moveWaypointObject
            //set object to the current AI target
        }
    }
}