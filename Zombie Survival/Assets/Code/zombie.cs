using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zombie : MonoBehaviour 
{
	public int health = 1;
	GameObject player;
	float playerdistance = 0;
	bool hunt = false;
	world scriptE;
	player scriptP;
	public float moveTime = 120;
	public bool timer = true;
	public bool move = true;
	public Vector3 moveTo;
	public float MoveDistance;
	public bool landed = false;
	public float angle;
	// Use this for initialization
	void Start () 
	{
		player = GameObject.Find ("Player");
		scriptE = GameObject.Find ("World").GetComponent<world> ();
		scriptE.zombiesSpawned += 1;
		scriptP = GameObject.Find ("Player").GetComponent<player> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//killed
		if (health <= 0) 
		{
			scriptE.zombiesSpawned -= 1;
			Destroy (this.gameObject);
		}
		//distance from player
		if (scriptP.move == true) 
		{
			playerdistance = Vector3.Distance (this.transform.position, player.transform.position);
		}
		//detect player
		if (playerdistance < 10 && hunt == false) 
		{
			hunt = true;
		} 
		else if (playerdistance > 30 && hunt == true) 
		{
			hunt = false;
		}
		//hunt player
		if (hunt == true) 
		{
			//rotate towards player
			if (angle > 2) 
			{
				transform.Rotate (0, 2, 0);
			}
			transform.LookAt (player.transform.position);
			transform.Translate (0, 0, 1 * Time.deltaTime);
			//move towards player if angle between zomgie and player is 
			//if (Vector3.Angle (player.transform.position, this.transform.position) < 10 && playerdistance > 1.7f) 
			{

			}
		}
		//movement (not hunting)
		if (hunt == false) 
		{
			if (move == true && hunt == false) 
			{
				transform.LookAt (moveTo);
				transform.Translate (0, 0, 5 * Time.deltaTime);
				MoveDistance = Vector3.Distance (transform.position, moveTo);
			}
			if (MoveDistance <= 3)
			{
				move = false;
				timer = true;
			}
		}
	}
	void FixedUpdate()
	{
		angle = Vector3.Angle (transform.forward, player.transform.position);
		if (landed == false) 
		{
			Ray ray = new Ray(transform.position, -Vector3.up);

			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 1)) 
			{
				if (hit.collider != null) 
				{
					landed = true;
				}
			}
		}
		//wait time until can move again
		if (timer == true && hunt == false) 
		{
			if (moveTime >= 0) 
			{
				moveTime -= 1;
			} 
			else if (moveTime <= 0 && landed == true)
			{
				timer = false;
				moveTime = Random.Range (60, 600);
				move = true;
				moveTo = new Vector3 (transform.position.x + Random.Range (-30, 30), 1, transform.position.z + Random.Range (-30, 30));
			}
		}
	}
}
