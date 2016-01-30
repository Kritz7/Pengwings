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
		AddStones(2);
		AddStones(2);
		AddStones(2);
		AddStones(2);

	}

	void OnTriggerEnter(Collider other)
	{
		if(playerOnNest == null)
		{
			if(other.tag.ToLower().Contains("player") && stones.Count > 0)
			{
				playerOnNest = other.gameObject.GetComponent<Player>();
				playerWhoOwnsNest = playerOnNest;

				foreach(GameObject go in stones)
				{
					go.GetComponent<SpriteRenderer>().color = playerWhoOwnsNest.playerColour;
				}
			}

			if(other.tag.ToLower().Contains("stone") && other.gameObject.layer != LayerMask.NameToLayer("clip"))
			{
				if(other.GetComponent<Stone>().thrownBy!=null)
				{
					Player playerThrowing = other.GetComponent<Stone>().thrownBy;
					AddStones(1);

					foreach(GameObject go in stones)
					{
						go.GetComponent<SpriteRenderer>().color = playerThrowing.playerColour;
					}

					GameObject.Destroy(other.gameObject);
				}
			}
		}

		if(playerWhoOwnsNest != null)
		{
			if(other.tag.ToLower().Contains("player"))
			{
				if(playerWhoOwnsNest != other.gameObject.GetComponent<Player>())
				{
					ExplodeNest();
					playerWhoOwnsNest = null;
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
			if(playerWhoOwnsNest) newStone.GetComponent<SpriteRenderer>().color = playerWhoOwnsNest.playerColour;

			stones.Add(newStone);
		}
	}

	void DestroyAllStones()
	{
		while(stones.Count > 0)
		{
			GameObject.Destroy(stones[0]);
			stones.RemoveAt(0);
		}

		stones.Clear();
	}

	public void ExplodeNest()
	{
		if(stones.Count>0)
		{
			for(int i=0; i<stones.Count; i++)
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
			
			DestroyAllStones();
		}
	}
}
