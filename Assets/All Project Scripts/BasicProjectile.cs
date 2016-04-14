using UnityEngine;
using System.Collections;

public class BasicProjectile : MonoBehaviour {

    public GameObject target;
    public float bulletSpeed;
    public int terrainLayer = 9;
    public int enemyLayer = 8;

	// Use this for initialization
	void Start () {
        bulletSpeed = 5f * Time.fixedDeltaTime;
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

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer.Equals(enemyLayer))
        {
            Destroy(this.gameObject);
        }
    }
}

