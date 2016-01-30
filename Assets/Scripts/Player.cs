using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XInputDotNetPure;

public class Player : MonoBehaviour
{
	GamePadState state;
	GamePadState prevState;
	PlayerIndex playerIndex;
	public int myPlayerID;
	public Color playerColour;

	Rigidbody myRigidbody;

	float walkForce = 35f;
	float slideForce = 100f;
	float walkDamping = 0.85f;
	float slideDamping = 0.98f;

	float currentSlideForce = 40f;

	Vector3 currentForce = Vector3.zero;

	Vector3 moveDirAtDashStart = Vector3.zero;
	List<Vector3> movementNormalHistory = new List<Vector3>();

	void Awake()
	{
		if(GetComponent<Rigidbody>()) myRigidbody = GetComponent<Rigidbody>();

		GetComponent<Renderer>().material.color = playerColour;
	}

	void ColourOn()
	{
		GetComponent<Renderer>().material.color = new Color(1.0f, 0.5f, 0.5f);
	}

	void ColourOff()
	{
		GetComponent<Renderer>().material.color = playerColour;
	}

	
	Vector3 GetAvgNormal()
	{
		Vector3 avgCounting = Vector3.zero;
		foreach(Vector3 v in movementNormalHistory)
		{
			avgCounting += v.normalized;
		}

		return avgCounting / movementNormalHistory.Count;
	}

	void Update ()
	{
		playerIndex = (PlayerIndex)myPlayerID;
		prevState = state;
		state = GamePad.GetState(playerIndex);
	
		bool dashing = false;
		float currentMaxForce = walkForce;
		float currentDamping = walkDamping;
		if(state.Triggers.Left > 0f)
		{
			if(prevState.Triggers.Left <= 0f)
			{
				currentSlideForce = slideForce;
				moveDirAtDashStart = new Vector3(state.ThumbSticks.Left.X, 0, state.ThumbSticks.Left.Y);
			}

			dashing = true;
			currentMaxForce = currentSlideForce;
			currentDamping = slideDamping;

		}

		if((state.ThumbSticks.Left.X != 0f || state.ThumbSticks.Left.Y != 0f)
		   && myRigidbody.velocity.magnitude < 1f)
		{
			currentMaxForce = slideForce;
		}

		Vector3 forceToAdd = new Vector3(state.ThumbSticks.Left.X, 0, state.ThumbSticks.Left.Y);

		if(dashing) forceToAdd = moveDirAtDashStart;

		currentForce += forceToAdd * Time.deltaTime;
		currentForce *= currentDamping * 0.01f;

		myRigidbody.velocity *= currentDamping;

		myRigidbody.AddForce(currentForce.normalized * currentMaxForce);

		currentSlideForce *= slideDamping * 0.88f;
	}

}
