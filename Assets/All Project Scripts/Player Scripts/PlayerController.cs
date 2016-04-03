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
        //Left Mouse Click
        if (Input.GetMouseButtonDown(0))
        {
            clickToMove();

        }
        //SpaceBar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SnapCameraToPlayer();
        }
    }


   
    //This is going to be called in the process keys function
    //it raycasts to the from the camera to the  mouse pointer and gets
    //the intersecting point we check if the point is on a enemy unit or on a
    //the ground
    void clickToMove()
    {

        RaycastHit hit;

        //int navMeshLayer = 1;
        //LayerMask navMesh = 1 << navMeshLayer;
        //int enemyLayer = 3; //TODO: FIX LAYERMASKS
        //LayerMask  enemy  = 1 << enemyLayer;

        //if we hit the terrain
         if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            GameObject waypoint = Instantiate(moveWaypoint, hit.point, Quaternion.identity) as GameObject;
            playerAI.target = waypoint;
            return;
        }
        //if we hit the enemy
        else if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            //sets the target to the clicked enemy
            //all movement and basic attacks should be handled by the Player_AI
            playerAI.target = hit.collider.gameObject;
            return;
        }
        
    }

    public void SnapCameraToPlayer()
    {
        Camera.main.transform.parent.position = this.transform.position;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag.Equals("MoveWaypoint"))
        {
            Destroy(col.gameObject);
        }
    }
}