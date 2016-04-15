using UnityEngine;
using System.Collections;

public class NewTurret_AI : Unit_Base
{
    public Transform head;
    public Transform shotPoint;
    public GameObject lazerShot;
    [HideInInspector]
    public GameObject target;

    public Quaternion faceIdle;

    public override void Awake()
    {
        base.Awake();
        attackRange = 300f;
        attackCooldown = 0.5f;
        damageOutput = 40;
        visionRange = 300f;
        nextAttackTime = 0.0f;
        isInCombat = false;
    }

    new void Start()
    {
        layerSetUp();
    }

    // Turret Behaviour
    void FixedUpdate()
    {
        GameObject closestEnemy = getClosestEnemy();
        //Check if an enemy is in range
        if (closestEnemy != null)
        {
            //check if we are facing the closestEnemy
            if (isFacingEnemy())
            {
                //check if he is in range
                if (isEnemyInAttackRange(closestEnemy))
                {
                    if (Time.time >= nextAttackTime)
                    {
                        target = closestEnemy;
                        shoot();
                        target = null;
                    }
                }
            }
            else
            {
                //we are not facing him so face him
                faceEnemy(closestEnemy);
            }
        }
    }

    public override void ApplyDamage(int amount)
    {
        health -= amount / 8;
        if (health <= 0)
        {
            StartCoroutine("Kill");
        }
    }

    public bool isFacingEnemy()
    {
        GameObject closestEnemy = this.getClosestEnemy();
        if (closestEnemy == null)
        {
            return false;
        }
        Vector3 enemyDir = closestEnemy.transform.position - this.transform.position;
        float angleDifference = Mathf.Abs(Vector3.Angle(head.transform.forward, enemyDir));


        if (angleDifference < aimThreshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //public GameObject getClosestEnemy()
    //{
    //    //get nearest enemy in attack range

    //    Collider[] cols = Physics.OverlapSphere(this.transform.position, visionRange, enemyLayer);
    //    GameObject closestEnemy = null;
    //    float closestDistance = float.MaxValue;

    //    //check for the closest enemy object
    //    foreach (Collider collision in cols)
    //    {
    //        Vector3 colliderPos = collision.gameObject.transform.position;
    //        float currentDistance = Mathf.Abs((colliderPos - this.transform.position).magnitude);
    //        if (currentDistance < closestDistance)
    //        {
    //            closestEnemy = collision.gameObject;
    //            closestDistance = currentDistance;
    //        }
    //    }
    //    return closestEnemy;
    //}

    public bool isEnemyInAttackRange(GameObject enemy)
    {
        Vector3 distance = enemy.transform.position - this.transform.position;
        return Mathf.Abs(distance.magnitude) < attackRange;
    }

    public void faceEnemy(GameObject enemy)
    {
        Vector3 dir = enemy.transform.position - this.transform.position;
        head.transform.forward = Vector3.RotateTowards(head.transform.forward, dir, 1f, 30f);
    }

    public void shoot()
    {
        if (Time.time > nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            GameObject bullet = Instantiate(lazerShot, shotPoint.position, head.transform.rotation) as GameObject;

            bullet.GetComponent<LazerShot>().damage = damageOutput;
            bullet.GetComponent<LazerShot>().Fire(target.transform.position);
            target.SendMessage("ApplyDamage", damageOutput);
        }
    }
}