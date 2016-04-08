using UnityEngine;
using System.Collections;

public class BasicProjectile : MonoBehaviour {

    public GameObject target;
    public float bulletSpeed;

	// Use this for initialization
	void Start () {
        bulletSpeed = 10f * Time.deltaTime;
	}
	
	// Update is called once per frame
	void Update () {
        moveToTarget();
	}

    //function to vary velocity and direction
    void moveToTarget()
    {
        Vector3 moveDir = target.transform.position - this.transform.position;
        this.transform.position += moveDir.normalized * bulletSpeed;
    }
}
