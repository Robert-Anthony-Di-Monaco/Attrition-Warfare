using UnityEngine;
using System.Collections;

public class ClickToMoveCharacter : MonoBehaviour {


    public NavMeshAgent agent;
    public float beamShotCooldown, nextBeamShot, shotRange;
    public GameObject beamShot;
    public bool enemyClose;

	// Use this for initialization
	void Start () {
        shotRange = 50f;
        nextBeamShot = Time.time;
        beamShotCooldown = 0.5f;
        agent = this.GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {

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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ShootBeam();
        }


    }

    void clickToMove()
    {

            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                agent.SetDestination(hit.point);
                //TODO: check if an enemy is clicked and if in range then attack it
            }
    }
    void SnapCameraToPlayer()
    {

        Camera.main.transform.parent.position = this.transform.position;
    }

    //I want to do some more work on this function is doesnt look natural when the player faces his target
    //before shooting
    void ShootBeam()
    {
        if (Time.time > nextBeamShot)
        {
            //get the vector pointing towards the mouse position
            RaycastHit vHit = new RaycastHit();
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out vHit);
            Vector3 moveDir = vHit.point - transform.position;

            //stop movement
            agent.SetDestination(this.transform.position);
            //rotate towards towards mouse position

            transform.forward = Vector3.RotateTowards(transform.forward, moveDir, 90f , 0.0f);
            Instantiate(beamShot, transform.position + (transform.forward * 1f), this.transform.rotation);
            
        }
    }

    //Casts a sphere with radius shotRange around the targets position and checks colliding objects
    //if they are tagged as Enemy.  Returns the closest Enemy if there is one within range otherwise returns null
    GameObject EnemyInRange()
    {
        GameObject closestEnemy = null;
        float closestDistance = float.MaxValue;
        Collider[] col = Physics.OverlapSphere(transform.position, shotRange);
        foreach(Collider collision in col){
            if(collision.gameObject.tag.Equals("Enemy")){

                float distance = (collision.gameObject.transform.position - transform.position).magnitude;
                if (distance < closestDistance)
                {
                    closestEnemy = collision.gameObject;
                    closestDistance = distance;
                }
            }
        }
        return closestEnemy;
    }


}
