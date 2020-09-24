using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Paintball : MonoBehaviour
{
	public GameObject splatParticlePrefab;

	public void Start()
	{
		GetComponent<Rigidbody>().useGravity = false;
	}

	public void OnCollisionEnter(Collision collision)
	{
		Vector3 position = collision.contacts[0].point + collision.contacts[0].normal * 0.1f;
		GameObject newParticles = Instantiate(splatParticlePrefab, position, Quaternion.identity);

		newParticles.transform.LookAt(collision.contacts[0].point + collision.contacts[0].normal * 2f);

		Destroy(gameObject, 0.1f);
	}
}
