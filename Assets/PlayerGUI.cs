using UnityEngine;
using System.Collections;

public class PlayerGUI : MonoBehaviour {


    public Transform healthbar;
    public Transform attackMoveButton;
    public Transform squadCommandButton;
    public Transform SquadAttackButton;
    public GameObject Player;
    bool attack;
    bool squadAttack;
    bool command;

	// Use this for initialization
	void Start () {
        //healthbar = transform.Find("HealthBar");
        //attackMoveButton = transform.Find("attackMoveOrderButton");
        //squadCommandButton = transform.Find("CommandSquadButton");
        //SquadAttackButton = transform.Find("SquadAttackOrderButton");

        attack = Player.GetComponent<PlayerController>().attackFlag;
        squadAttack = Player.GetComponent<PlayerController>().squadAttackFlag;
        command = Player.GetComponent<PlayerController>().squadCommandFlag;
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void setHealth(int health)
    {
        healthbar.transform.Find("HealthBar").gameObject.GetComponent<PlayerHealthBar>().updateHealthbar(health);
    }
}
