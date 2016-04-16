using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerGUI : MonoBehaviour {


    public Transform healthbar;
    public GameObject Player;
    public Button buttonW, buttonA, buttonSpace, buttonE;

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
        //
        var cbE = buttonE.colors;
        cbE.normalColor = (Input.GetKey(KeyCode.E)) ? Color.red : Color.white;
        buttonE.colors = cbE;
    }

    public void setHealth(int health)
    {
        healthbar.transform.Find("HealthBar").gameObject.GetComponent<PlayerHealthBar>().updateHealthBar();
    }
}