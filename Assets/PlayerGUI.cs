using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerGUI : MonoBehaviour {


    public Transform healthbar;
    public GameObject Player;
   // bool attack;
   // bool squadAttack;
    //bool command;
    public Button buttonW, buttonA, buttonSpace;

    // Use this for initialization
    void Start ()
    {
        //attack = Player.GetComponent<PlayerController>().attackFlag;
       // squadAttack = Player.GetComponent<PlayerController>().squadAttackFlag;
       // command = Player.GetComponent<PlayerController>().squadCommandFlag;
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Update button colors when user presses them
        var cbW = buttonW.colors;
        cbW.normalColor = (Input.GetKey(KeyCode.W)) ? Color.red : Color.white;
        buttonW.colors = cbW;
        //
        var cbA = buttonA.colors;
        cbA.normalColor = (Input.GetKey(KeyCode.A)) ? Color.red : Color.white;
        buttonA.colors = cbA;
        //
        var cbSpace = buttonSpace.colors;
        cbSpace.normalColor = (Input.GetKey(KeyCode.Space)) ? Color.red : Color.white;
        buttonSpace.colors = cbSpace;
    }

    public void setHealth(int health)
    {
        healthbar.transform.Find("HealthBar").gameObject.GetComponent<PlayerHealthBar>().updateHealthBar();
    }
}