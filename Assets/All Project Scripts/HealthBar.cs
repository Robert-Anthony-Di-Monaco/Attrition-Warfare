using UnityEngine;
using System.Collections;


// IGNORE THIS SCRIPT, IT WILL TAKE CARE OF DISPLAYING A UNIT'S HEALTH, if its position around the unit needs adjusting --- > tell ROBERT he will fix it 


public class HealthBar : MonoBehaviour {
	GUIStyle healthStyle;
	GUIStyle backStyle;
	GUIStyle borderStyle;
	Unit_Base health;
	
	void Awake()
	{
		health = GetComponent<Unit_Base>();
	}
	
	void OnGUI()
	{
		InitStyles();
		
		// Draw Health Bar
		//---------------

		//Project point above the unit, not at its feet
		Vector3 aboveUnit = new Vector3(transform.position.x, transform.position.y + 30, transform.position.z);
		if(health.isRange())
			aboveUnit = new Vector3(aboveUnit.x, aboveUnit.y - 10, aboveUnit.z);
		else if (health.isSiege())
			aboveUnit = new Vector3(aboveUnit.x, aboveUnit.y + 40, aboveUnit.z);
		
		Vector3 pos = Camera.main.WorldToScreenPoint(aboveUnit);


		//Draw Border
		GUI.color = Color.grey;
		GUI.backgroundColor = Color.grey;
		GUI.Box(new Rect(pos.x - 25, Screen.height - pos.y - 20, Unit_Base.maxHealth/2 + 2, 9), ".", borderStyle);

		// Draw health bar background
		GUI.color = Color.grey;
		GUI.backgroundColor = Color.red;
		GUI.Box(new Rect(pos.x - 24, Screen.height - pos.y - 19, Unit_Base.maxHealth/2, 6), ".", backStyle);

		// Draw health bar amount
		GUI.color = Color.green;
		GUI.backgroundColor = Color.green;
		GUI.Box(new Rect(pos.x - 24, Screen.height - pos.y - 19, (Unit_Base.maxHealth * (health.health / Unit_Base.maxHealth)) / 2 , 6), ".", healthStyle);
	}
	
	void InitStyles()
	{
		if( healthStyle == null )
		{
			healthStyle = new GUIStyle( GUI.skin.box );
			healthStyle.normal.background = MakeTex( 1, 1, new Color( 0f, 1f, 0f, 0.3f ) );
		}
		
		if( backStyle == null )
		{
			backStyle = new GUIStyle( GUI.skin.box );
			backStyle.normal.background = MakeTex( 1, 1, new Color( 1f, 0f, 0f, 0.3f ) );
		}

		if (borderStyle == null)
		{
			borderStyle = new GUIStyle( GUI.skin.box );
			borderStyle.normal.background = MakeTex( 2, 2, new Color( 0.4f, 0.4f, 0.4f, 0.4f ) );
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
}
