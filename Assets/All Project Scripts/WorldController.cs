using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*	
  	CONTROLS WORLD EVENTS, HOLDS WORLD VARIABLES 
    
 */


public class WorldController : MonoBehaviour 
{
	// The Player
	public Transform thePlayer;   				 
	//Transform playerBase,
	//		 enemyBase;

	// Holds all Player Units   
	public List<List<Transform>> playerSquads = new List<List<Transform>>();  // a list of squads and each squad is just a list of its units

	// Holds all Enemy Mobile Units
	public List<Transform> mobileEnemyUnits = new List<Transform> ();

	// Holds all Enemy Stationary Units --> turrets and towers
	public List<Transform> stationaryEnemyUnits = new List<Transform> ();

	//All units in the game, counter used to generated unique IDs
	public int totalUnitsInstantiated = 0;
	public List<Unit_Base> allUnits = new List<Unit_Base>();

	//All squads in the game
	public int totalSquadsInstantiated = 0;
	public List<Squad> allSquads = new List<Squad>();

	//Victory/defeat
	int allyCrystalsDestroyed = 0, enemyCrystalsDestroyed = 0;

	public Texture2D victorySplash;
	public Texture2D defeatSplash;
	public int XmoveSplashCenter = 0;
	public int YmoveSplashCenter = 0;
	public bool testVictory = false;
	public bool testDefeat = false;

	GUIStyle blackBG;
	
	void Start () 
	{
		LakeAreas.Initialize();
	}

	void Update () 
	{
		
	}
	
	public void victory()
	{
		if(++enemyCrystalsDestroyed == 2)
		{
			foreach (Unit_Base unit in allUnits)
				unit.GetComponent<HealthBar> ().enabled = false;
			// put winning code here for splash screen or whatever
			Debug.Log("You win! Congratulations on having no life!");
		}
	}
	
	public void defeat()
	{
		if(++allyCrystalsDestroyed == 2)
		{
			foreach (Unit_Base unit in allUnits)
				unit.GetComponent<HealthBar> ().enabled = false;
			// put losing code here for splash screen or whatever
			Debug.Log("You lose! You must suck!");
		}
	}

	void OnGUI(){
		InitStyles ();

		if (testVictory || enemyCrystalsDestroyed >= 2) {

			//disable healthbars displaying on top of the splash screen
			foreach (Unit_Base unit in allUnits)
				unit.GetComponent<HealthBar> ().enabled = false;
			
			GUI.Box (new Rect (-1, -1, Screen.width + 1, Screen.height + 1), ".", blackBG);
			GUI.Box (new Rect (Screen.width / 2 - 612 + XmoveSplashCenter, Screen.height / 2 - 256 + YmoveSplashCenter, Screen.width / 2 + 512, Screen.height / 2 + 256), victorySplash);
		} else if (testDefeat || allyCrystalsDestroyed >= 2) {

			//disable healthbars displaying on top of the splash screen
			foreach (Unit_Base unit in allUnits)
				unit.GetComponent<HealthBar> ().enabled = false;
			
			GUI.Box (new Rect (-1, -1, Screen.width + 1, Screen.height + 1), ".", blackBG);
			GUI.Box (new Rect (Screen.width / 2 - 612 + XmoveSplashCenter, Screen.height / 2 - 256 + YmoveSplashCenter, Screen.width / 2 + 512, Screen.height / 2 + 256), defeatSplash);
		}
	}
	Texture2D MakeTex( int width, int height, Color col )
	{
		Color[] pix = new Color[width * height];
		for( int i = 0; i < pix.Length; ++i )
		{
			pix[ i ] = col;
		}
		Texture2D result = new Texture2D( width, height );
		result.wrapMode = TextureWrapMode.Repeat;
		result.SetPixels( pix );
		result.Apply();
		return result;
	}
	void InitStyles()
	{
		if( blackBG == null )
		{
			blackBG = new GUIStyle( GUI.skin.box );
			blackBG.normal.background = MakeTex( 1, 1, new Color( 0f, 0f, 0f, 1f ) );
		}
			
	}
}