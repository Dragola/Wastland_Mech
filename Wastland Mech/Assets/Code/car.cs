using UnityEngine;
using System.Collections;

public class car : MonoBehaviour 
{
	public bool entered = false;
	GameObject scriptP;

	// Use this for initialization
	void Start () 
	{
		scriptP = GameObject.Find ("Player");
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
