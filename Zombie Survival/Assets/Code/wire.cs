using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wire : MonoBehaviour 
{
	public bool connected = false;
	public bool powered = false;
	public bool powerStorage = false;
	world scriptE;
	player scriptP;
	public List <GameObject> wires = new List<GameObject>();
	// Use this for initialization
	void Start () 
	{
		scriptE = GameObject.Find ("World").GetComponent<world> ();
		scriptP = GameObject.Find ("Player").GetComponent<player> ();
	}

	// Update is called once per frame
	void Update () 
	{
		//set's name on wire
		if (scriptP.placed == true && this.name == "Cable(Clone)") 
		{
			this.name = "Cable";
			this.gameObject.layer = 0;
			scriptP.spawnB = false;
			scriptP.placed = false;
			scriptP.wireCheck = true;
			scriptE.all_wires.Add (this.gameObject);
		}
		if (scriptP.wireCheck == true) 
		{
			foreach (GameObject i in wires) 
			{
				//if not currently connected
				if (connected == false) 
				{
					//check if any wires connected
					if (i != null) 
					{
						connected = true;
						i.GetComponent<wire> ().connected = true;
					} 
					//if not then no connection
					else 
					{
						connected = false;
					}
				}
				//checks power
				if (i.GetComponent<wire> ().powered == true) 
				{
					powered = true;
				}
				//checks power storage
				if (GetComponent<wire> ().powerStorage == true) 
				{
					powerStorage = true;
				}
			}
			//stops checking to wire update
			scriptP.wireCheck = false;
		}
	}
	//adds wire to list
	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Cable_Simple")
		{
			Debug.Log ("Detected");
			wires.Add (other.gameObject);
		}
		if (other.tag == "Power") 
		{
			powered = true;

		}

	}
	//removes wire from list
	void OnTriggerExit (Collider left)
	{
		wires.Remove (left.gameObject);
	}

}
