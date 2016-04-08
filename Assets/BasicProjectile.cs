using UnityEngine;
using System.Collections;

public class BasicProjectile : MonoBehaviour {
    /**We want to give the projectile a target and a time to get to it. The projectile should vary its own speed to
     * to arrive at the target when the time expires
    */


    public GameObject target;
    public float timeToTarget;

	// Use this for initialization
	void Start () {
        timeToTarget = 0.5f;
	}
	
	// Update is called once per frame
	void Update () {
        moveToTarget();
        if (timeToTarget < 0)
        {
            Destroy(this.gameObject);
        }
	}

    //function to vary velocity and direction
    void moveToTarget()
    {
        //update time to target
        timeToTarget -= Time.deltaTime;

        Vector3 moveDir = target.transform.position - this.transform.position;
        float distance = moveDir.magnitude;

        float velocityMag = distance / timeToTarget;

        moveDir.Normalize();
        moveDir *= velocityMag;

        this.transform.position = this.transform.position + moveDir;
    }
}
