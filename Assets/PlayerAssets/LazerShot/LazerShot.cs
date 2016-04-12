using UnityEngine;
using System.Collections;

/*
	For now this script just creates then destroys the shot

	Derek: add this when you want to use the shot  
		GameObject shotInstant = Instantiate(lazerShotPrefab, spawnPoint, player.transform.rotation) as GameObject;
		shotInstant.getComponent<LazerShot>().Fire(target);
 */

public class LazerShot : MonoBehaviour 
{
    public GameObject target;
	public Turret_Ai parent;
    public float bulletSpeed;
    public int terrainLayer = 9;
    public int enemyLayer = 8;
	public int damage;


    public void Start()
    {
        bulletSpeed = 1000f * Time.fixedDeltaTime;
    }
    public void FixedUpdate()
    {
        moveToTarget();
    }

	public void Fire(Vector3 target)
	{
		float shotVelocity = 250f;
		this.gameObject.GetComponent<Rigidbody>().AddForce(shotVelocity * transform.forward, ForceMode.VelocityChange);

		// Destroy shot when it hits target
		Vector3 dir = target - transform.position;
		float time2Target = dir.magnitude / shotVelocity;
		Destroy (this.gameObject, time2Target); 
	}

    public void moveToTarget()
    {
		if (target == null || target.Equals(null)) {
			Destroy (this);
			return;
		}
        Vector3 moveDir = target.transform.position - this.transform.position;
		moveDir = new Vector3 (moveDir.x, moveDir.y + 20, moveDir.z);
		transform.rotation.SetLookRotation (moveDir);
		this.transform.position += moveDir.normalized * bulletSpeed;
    }

	void OnTriggerEnter(Collider col)   // Currently the prefab has a trigger collider
	{
        if (col.gameObject.Equals(target))
        {
			target.SendMessage("ApplyDamage", damage);
            Destroy(this.gameObject);
        }
	}
	void OnCollisionEnter(Collision col)
	{
	}
}

	

