using UnityEngine;
using System.Collections;

public class Lover : MonoBehaviour
{
	float moveSpeed;
	float maxForce = 100f;
	float damping = 0.7f;

	float attractStartTime = 0f;
	float attractDuration = 0.2f;

	Nest goalNest;
	Rigidbody myRigidbody;

	// Use this for initialization
	void Start ()
	{
		Attract(Nest.nests[0]);

		myRigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(attractStartTime + attractDuration > Time.time)
		{
			Vector3 normal = -(new Vector3(transform.position.x, 0, transform.position.z) + new Vector3(goalNest.transform.position.x, 0, goalNest.transform.position.z)).normalized;
			Debug.DrawLine(transform.position, transform.position + normal * 10f, Color.red, 10f);

			myRigidbody.AddForce(normal * maxForce * Time.deltaTime);
		}
	}

	void Attract(Nest nest)
	{
		attractStartTime = Time.time;
		goalNest = nest;
	}

}
