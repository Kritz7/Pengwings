using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Nest : MonoBehaviour
{
	public Player playerOnNest;
	public Player playerWhoOwnsNest;

	public static List<Nest> nests = new List<Nest>();
	public List<GameObject> stones = new List<GameObject>();

	float stoneRingDist = 0.2f;
	int stonesInBaseRing = 5;

	void Awake()
	{
		nests.Add(this);
		AddStones(11);
	}


	void OnTriggerStay(Collider other)
	{
		// destroy nest by dashing
		if(other.tag.ToLower().Contains("player"))
		{
			Player thisPlayer = other.GetComponent<Player>();
			
			if(playerWhoOwnsNest != thisPlayer && thisPlayer.currentlyDashing)
			{
				ExplodeNest();
				playerWhoOwnsNest = null;
				return;
			}
		}

		// no player on nest
		if(playerOnNest == null)
		{
			// this is a player, and the nest currently has no stones
			if(other.tag.ToLower().Contains("player") && stones.Count > 0)
			{
				playerOnNest = other.gameObject.GetComponent<Player>();

				// the nest isn't owned by anyone, or the nest is owned by the player touching it
				if(playerWhoOwnsNest == null || playerWhoOwnsNest == other.gameObject.GetComponent<Player>())
				{
					playerWhoOwnsNest = playerOnNest;

					Debug.Log("new player on nest " + nests.IndexOf(this) + " " + playerOnNest.name);

					foreach(GameObject go in stones)
					{
						go.GetComponent<SpriteRenderer>().color = playerWhoOwnsNest.playerVibrantColour;
					}
				}
			}

			// a stone hit the pile
			if(other.tag.ToLower().Contains("stone") && other.gameObject.layer != LayerMask.NameToLayer("clip"))
			{
				if(other.GetComponent<Stone>().thrownBy!=null)
				{
					Player playerThrowing = other.GetComponent<Stone>().thrownBy;

					if(playerWhoOwnsNest == playerThrowing || playerWhoOwnsNest == null)
					{
						AddStones(1);

						if(playerWhoOwnsNest == playerThrowing)
						{
							foreach(GameObject go in stones)
							{
								go.GetComponent<SpriteRenderer>().color = playerThrowing.playerVibrantColour;
							}
						}

						GameObject.Destroy(other.gameObject);
					}
					else if(playerWhoOwnsNest != null)
					{
						ExplodeNest();
						playerWhoOwnsNest = null;
						return;
					}
				}
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other.tag.ToLower().Contains("player"))
		{
			if(playerOnNest == other.gameObject.GetComponent<Player>())
			{
				playerOnNest = null;
			}
		}

	}

	public void AddStones(int count)
	{
		int currentStoneCount = stones.Count;

		for(int i=0; i<count; i++)
		{
			int actualIndex = i + currentStoneCount;
			int stoneRing = 0;

			if(actualIndex > 0)
			{
				stoneRing = Mathf.CeilToInt( (float)actualIndex / (float)(stonesInBaseRing) );
			}

			float angle = 0;
			if(stoneRing!=0) angle = -(360f / (float)(stonesInBaseRing * stoneRing)) * actualIndex;

			Vector3 stonePos = (angle!=0f?Quaternion.Euler(0, angle, 0):Quaternion.identity) * new Vector3(0, 0, stoneRingDist * stoneRing);
			stonePos += new Vector3(transform.position.x, -0.6f, transform.position.z);

			GameObject newStone = GameObject.Instantiate(Resources.Load("stone"), stonePos, Quaternion.Euler(90,0,0)) as GameObject;
			newStone.GetComponent<SphereCollider>().enabled = false;
			if(playerWhoOwnsNest) newStone.GetComponent<SpriteRenderer>().color = playerWhoOwnsNest.playerVibrantColour;

			stones.Add(newStone);
		}
	}

	void DestroyAllStones(int stonesToDestroy=-1)
	{
		if(stonesToDestroy == -1) stonesToDestroy = stones.Count;

		int stonesLeftNeeded = stones.Count - stonesToDestroy;

		while(stones.Count > stonesLeftNeeded)
		{
			GameObject.Destroy(stones[0]);
			stones.RemoveAt(0);
		}
	}

	public void ExplodeNest()
	{
		if(stones.Count>0)
		{
			int stonesToDestroy = 1;//  Mathf.CeilToInt(stones.Count / 2f);

			for(int i=0; i<stonesToDestroy; i++)
			{
				Vector3 rand = Random.insideUnitSphere;
				rand.y = 0;
				Vector3 pos =  transform.position;
				pos.y = -0.6f;

				GameObject stoney = GameObject.Instantiate(Resources.Load("stone"), pos + (rand * Random.Range(1,2)),
				                                           Quaternion.Euler(90, 0, 0)) as GameObject;
				
				stoney.GetComponent<Rigidbody>().AddForce((rand * Random.Range(60,100)));
				stoney.GetComponent<Stone>().MakeUnableToPickup();
			}
			
			DestroyAllStones(stonesToDestroy);
		}
	}
}
