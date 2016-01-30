using UnityEngine;
using System.Collections;

public class Stone : MonoBehaviour
{
	public Player thrownBy;
	Rigidbody myRigidbody;

	float timeSinceThrown = 0f;
	bool otherwiseUnableToPickup = false;
	float timeSinceUnable = 0f;

	// Use this for initialization
	void Awake ()
	{
		myRigidbody = GetComponent<Rigidbody>();
	}

	public void Throw(Vector3 force, Player who)
	{
		thrownBy = who;
		GetComponent<SpriteRenderer>().color = who.playerColour;
		myRigidbody.AddForce(force);
	}

	public void MakeUnableToPickup()
	{
		otherwiseUnableToPickup = true;
		gameObject.layer = LayerMask.NameToLayer("clip");
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(thrownBy)
		{
			timeSinceThrown += Time.deltaTime;

			if((myRigidbody.velocity.magnitude < 2f && timeSinceThrown > 2f) || timeSinceThrown > 5f)
			{
				thrownBy = null;
				GetComponent<SpriteRenderer>().color = Color.white;
				timeSinceThrown = 0f;
			}
		}

		if(otherwiseUnableToPickup)
		{
			timeSinceUnable += Time.deltaTime;

			if(timeSinceUnable > 1f)
			{
				otherwiseUnableToPickup = false;
				timeSinceUnable = 0f;
				gameObject.layer = LayerMask.NameToLayer("Default");
			}
		}
	}
}
