using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lover : MonoBehaviour
{
	public static Lover theLover;
	public ParticleSystem parts;
	public ParticleEmitter parte;

	float moveSpeed = 300f;
	float maxForce = 200f;
	float damping = 0.92f;

	float attractStartTime = 0f;
	float attractDuration = 0.2f;

	public List<Nest> goalNests = new List<Nest>();
	Rigidbody myRigidbody;

	float currentDistanceFromGoal = 9999f;
	
	Vector3 startPos;

	// Use this for initialization
	void Start ()
	{
		theLover = this;
		myRigidbody = GetComponent<Rigidbody>();
		startPos = transform.position;
		parts = GetComponentInChildren<ParticleSystem>();
		parte = GetComponentInChildren<ParticleEmitter>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 normal = (new Vector3(startPos.x, 0, startPos.z) - new Vector3(transform.position.x, 0, transform.position.z)).normalized;
		Debug.DrawLine(transform.position, transform.position + normal * 10f, Color.red, 10f);
		
		myRigidbody.AddForce(((normal * moveSpeed).normalized * maxForce) * Time.deltaTime * (2 * 0.35f));
		myRigidbody.velocity *= damping;

		
		if(currentDistanceFromGoal < 5f) parts.emissionRate = 10f;
		else parts.emissionRate = 0f;

		/*
		if(goalNests.Count > 0)
		{
			for(int i=0; i<goalNests.Count; i++)
			{
				if(goalNests[i].stones.Count <= 0 || goalNests[i].playerOnNest == null)
				{
					goalNests.RemoveAt(i);
					i--;
				}
			}
		}

		int goalNestID = -1;
		int highestStones = 0;
		foreach(Nest n in goalNests)
		{
			if(n.stones.Count > highestStones)
			{
				highestStones = n.stones.Count;
				goalNestID = goalNests.IndexOf(n);
			}
		}

		if(goalNestID != -1)
		{
			Nest goalNest = goalNests[goalNestID];

			if(goalNest!=null)
			{
				if(attractStartTime + attractDuration > Time.time)
				{
					Vector3 normal = (new Vector3(goalNest.transform.position.x, 0, goalNest.transform.position.z) - new Vector3(transform.position.x, 0, transform.position.z)).normalized;
					Debug.DrawLine(transform.position, transform.position + normal * 10f, Color.red, 10f);

					myRigidbody.AddForce(((normal * moveSpeed * goalNest.stones.Count).normalized * maxForce) * Time.deltaTime);
					myRigidbody.velocity *= damping;

					currentDistanceFromGoal = (transform.position - goalNest.transform.position).magnitude;

					if(currentDistanceFromGoal < 1.5f)
					{
						Debug.Log(goalNest.playerOnNest.name + " wins");
					}
				}
			}
		}
		*/
	}

	public void Attract(Nest nest)
	{
		/*
		int highestStones = 0;
		foreach(Nest n in goalNests)
		{
			if(n.stones.Count > highestStones) highestStones = n.stones.Count;
		}

		if(nest.stones.Count >= highestStones)
		{
			attractStartTime = Time.time;
			if(!goalNests.Contains(nest))
			{
				goalNests.Add(nest);
			}
		}
		*/

		Vector3 normal = (new Vector3(nest.transform.position.x, 0, nest.transform.position.z) - new Vector3(transform.position.x, 0, transform.position.z)).normalized;
		Debug.DrawLine(transform.position, transform.position + normal * 10f, Color.red, 10f);
		
		myRigidbody.AddForce(((normal * moveSpeed).normalized * maxForce) * Time.deltaTime * (nest.stones.Count * 2f));
		myRigidbody.velocity *= damping;
		
		currentDistanceFromGoal = (transform.position - nest.transform.position).magnitude;

		GetComponentInChildren<ParticleSystemRenderer>().material.color = nest.playerOnNest.playerVibrantColour;

		if(currentDistanceFromGoal < 2f)
		{
			if(nest.playerOnNest.myPlayerID == 0)
			{
				Player.points += Time.deltaTime * 0.5f;
			}
			else
			{
				Player.points -= Time.deltaTime * 0.5f;
			}

			barthing.mainBar.SetBarPercentage(Player.points);
		}
	}

	IEnumerator ManageNests()
	{
		while(true)
		{


			yield return new WaitForSeconds(1f);
		}
	}

}
