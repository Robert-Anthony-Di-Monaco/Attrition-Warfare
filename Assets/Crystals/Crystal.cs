using UnityEngine;
using System.Collections;

public class Crystal : Unit_Base {

	public Material deadMat;
	public const int maxHP = 1000;

	// Use this for initialization
	public override void Awake () {
		health = maxHP;
	}
	new void Start()
    {
		layerSetUp ();
	}

	public override void ApplyDamage (int amount)
	{
		health -= amount;

		if (health <= 0) {
			CrystalKill ();
		}

	}
	public override bool isCrystal(){
		return true;
	}
	public void CrystalKill(){
		GetComponentInChildren<Renderer> ().material = deadMat;

		Destroy (GetComponent<CrystalHP> ());
		GetComponentInChildren<CrystalHover> ().dead = true;
		gameObject.layer = 1 << 0;
		Destroy (GetComponent<Rigidbody> ());

		if (faction == 0)
			GameObject.Find ("WorldController").GetComponent<WorldController> ().defeat();
		else if (faction == 1)
			GameObject.Find ("WorldController").GetComponent<WorldController> ().victory();

		Destroy(GetComponent<Unit_Base> ());

	}
}
