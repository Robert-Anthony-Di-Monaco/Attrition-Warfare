using UnityEngine;
using System.Collections;


// IGNORE THIS SCRIPT, IT WILL TAKE CARE OF DISPLAYING A UNIT'S HEALTH, if its position around the unit needs adjusting --- > tell ROBERT he will fix it 


public class HealthBar : MonoBehaviour {
	GUIStyle healthStyle;
	GUIStyle backStyle;
	Unit_Base health;
	
	void Awake()
	{
		health = GetComponent<Unit_Base>();
	}
	
	void OnGUI()
	{
		InitStyles();
		
		// Draw Health Bar		
		Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
		
		// Draw health bar background
		GUI.color = Color.grey;
		GUI.backgroundColor = Color.grey;
		GUI.Box(new Rect(pos.x-26, Screen.height - pos.y + 20, Unit_Base.maxHealth/2, 7), ".", backStyle);
		
		// Draw health bar amount
		GUI.color = Color.green;
		GUI.backgroundColor = Color.green;
		GUI.Box(new Rect(pos.x-25, Screen.height - pos.y + 21, health.health/2, 5), ".", healthStyle);
	}
	
	void InitStyles()
	{
		if( healthStyle == null )
		{
			healthStyle = new GUIStyle( GUI.skin.box );
			healthStyle.normal.background = MakeTex( 2, 2, new Color( 0f, 1f, 0f, 1.0f ) );
		}
		
		if( backStyle == null )
		{
			backStyle = new GUIStyle( GUI.skin.box );
			backStyle.normal.background = MakeTex( 2, 2, new Color( 0f, 0f, 0f, 1.0f ) );
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
		result.SetPixels( pix );
		result.Apply();
		return result;
	}
}
