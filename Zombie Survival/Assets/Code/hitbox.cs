using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hitbox : MonoBehaviour 
{
	public GameObject detected;
	public bool hit = false;
	void OnTriggerEnter(Collider enter)
	{
		detected = enter.gameObject;
		hit = true;
	}
	void OnTriggerExit(Collider exit)
	{
		detected = null;
		hit = false;
	}
}
