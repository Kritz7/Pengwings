using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XInputDotNetPure;

public class Player : MonoBehaviour
{
	public static float points = 0.5f;
	public Canvas theCanvas;

	GamePadState state;
	GamePadState prevState;
	PlayerIndex playerIndex;
	public int myPlayerID;
	public Color playerColour;
	public Color playerVibrantColour;

	Rigidbody myRigidbody;
	Animator myAnimator;

	float walkForce = 35f;
	float slideForce = 100f;
	float walkDamping = 0.85f;
	float slideDamping = 0.98f;
	float timeSpendDashing = 0f;

	public bool currentlyDashing = false;

	float currentSlideForce = 40f;

	bool forceStopDashing = false;

	Vector3 currentForce = Vector3.zero;

	Vector3 moveDirAtDashStart = Vector3.zero;
	List<Vector3> movementNormalHistory = new List<Vector3>();

	public int myPebbleCount = 0;

	void Awake()
	{
		if(GetComponent<Rigidbody>()) myRigidbody = GetComponent<Rigidbody>();
		myAnimator = GetComponentInChildren<Animator>();
		GetComponentInChildren<Renderer>().material.color = playerColour;

		theCanvas = transform.root.GetComponentInChildren<Canvas>();

	}

	void ColourOn()
	{
		GetComponentInChildren<Renderer>().material.color = new Color(1.0f, 0.5f, 0.5f);
	}

	void ColourOff()
	{
		GetComponentInChildren<Renderer>().material.color = playerColour;
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

	void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.tag.Contains("stone") && collision.gameObject.layer != LayerMask.NameToLayer("clip"))
		{
			if(collision.gameObject.GetComponent<Stone>().thrownBy == null)
			{
				Destroy(collision.gameObject);
				myPebbleCount ++;
			}
		}
	}

	public void DoADance()
	{
		foreach(Nest n in Nest.nests)
		{
			if(n.playerOnNest == this)
			{
				Lover.theLover.Attract(n);
			}
		}
	}

	public void DropPebbles()
	{
		bool usedNest = false;

		foreach(Nest n in Nest.nests)
		{
			if(n.playerOnNest == this)
			{
				n.AddStones(myPebbleCount);
				usedNest = true;
				break;
			}
		}

		if(!usedNest)
		{
			for(int i=0; i<myPebbleCount; i++)
			{
				Vector3 rand = Random.insideUnitSphere;
				rand.y = 0;
				rand *= Random.Range(1f,2f);

				if(state.ThumbSticks.Left.X == 0 && state.ThumbSticks.Left.Y == 0) rand = Vector3.zero;

				GameObject stoney = GameObject.Instantiate(Resources.Load("stone"), transform.position + rand + new Vector3(state.ThumbSticks.Left.X, 0, state.ThumbSticks.Left.Y),
				                       Quaternion.Euler(90, 0, 0)) as GameObject;
				Vector3 force = ((rand * Random.Range(10,60)) + new Vector3(state.ThumbSticks.Left.X, 0, state.ThumbSticks.Left.Y) * 120f);

				stoney.GetComponent<Stone>().Throw(force, this);
			}
		}

		myPebbleCount = 0;
	}

	public void AddPebbleToNest(Nest n, int count)
	{
		n.AddStones(count);
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
			if(!forceStopDashing)
			{
				if(prevState.Triggers.Left <= 0f)
				{
					currentSlideForce = slideForce;
					moveDirAtDashStart = new Vector3(state.ThumbSticks.Left.X, 0, state.ThumbSticks.Left.Y);
				}

				dashing = true;
				currentMaxForce = currentSlideForce;
				currentDamping = slideDamping;

				if(timeSpendDashing > 0.5f && myRigidbody.velocity.magnitude < 0.3f)
				{
					dashing = false;
					forceStopDashing = true;
					currentMaxForce = walkForce;
				}

				timeSpendDashing += Time.deltaTime;
			}
		}

		if(prevState.Triggers.Left > 0f && Mathf.Approximately(state.Triggers.Left, 0f))
		{
			if(forceStopDashing)
			{
				forceStopDashing = false;
			}
		}

		if((state.ThumbSticks.Left.X != 0f || state.ThumbSticks.Left.Y != 0f)
		   && myRigidbody.velocity.magnitude < 1f)
		{
			currentMaxForce = slideForce;
		}

		if(state.Buttons.A == ButtonState.Pressed)
		{
			DropPebbles();
			DoADance();
		}

		Vector3 forceToAdd = new Vector3(state.ThumbSticks.Left.X, 0, state.ThumbSticks.Left.Y);

		if(dashing) forceToAdd = moveDirAtDashStart;
		currentlyDashing = dashing;

		myAnimator.SetFloat("horizontal", forceToAdd.x);
		//myAnimator.SetFloat("vertical", forceToAdd.y);
		myAnimator.SetBool("sliding", dashing);

		currentForce += forceToAdd * Time.deltaTime;
		currentForce *= currentDamping * 0.01f;

		myRigidbody.velocity *= currentDamping;

		myRigidbody.AddForce(currentForce.normalized * currentMaxForce);

		currentSlideForce *= slideDamping * 0.88f;
	}

}
