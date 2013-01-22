using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleBlock : MonoBehaviour
{
	public GameObject terrain;
	public int expansionProb;
	public int attemptsAllowed;
	private int adjacentFacesRemaining = 5;
	
	private readonly Vector3 posX = new Vector3(1f, 0f, 0f);
	private readonly Vector3 negX = new Vector3(-1f, 0f, 0f);
	private readonly Vector3 posZ = new Vector3(0f, 0f, 1f);
	private readonly Vector3 negZ = new Vector3(0f, 0f, -1f);
	private readonly Vector3 posY = new Vector3(0f, 1f, 0f);
	
	private GeneratorManager parentManager;
	
	void Start ()
	{
		parentManager = transform.parent.GetComponent<GeneratorManager>();
		StartCoroutine (TryToReplicate ());
	}
	
	IEnumerator TryToReplicate ()
	{
		while (true)
		{
			attemptsAllowed--;
			
			if (adjacentFacesRemaining <= 0)
			{
				Destroy (gameObject);
				break;
			}
		
			yield return 0;
		
			if ((attemptsAllowed <= 0) || (parentManager.getAttempts () <= 0))
			{
				Destroy (this);
//				GetComponent<CheckVisibility>().DestroyIfHidden ();
				break;
			}
			
			yield return 0;
			
			// Step 1. Generate random int and check against expansion probability.
			if (Random.Range (0, 101) > expansionProb)
			{
				// Generated number was outside expansion probability. Expansion should not occur.
//				Debug.Log ("Not expanding this cycle.");
				continue;
			}
			
			yield return 0;
			
			// Step 2. Randomly select a face.
			Vector3 destination = Vector3.zero;
			switch (Random.Range (1, 6))
			{
				case 1: destination = posX; break;
				case 2: destination = negX; break;
				case 3: destination = posZ; break;
				case 4: destination = negZ; break;
				case 5: destination = posY; break;
				default: Debug.Log ("ERROR: A face was selected which is outside the allowed range. Direction set to 0.", this); break;
			}
			
			yield return 0;
			
			// Step 3. Check if the face is empty.
			if (castRay (destination) == true)
			{
				// This face isn't empty, so do nothing.
				continue;
			}
			else
			{
				parentManager.spendAttempt ();
			}
		
			yield return 0;
		
			// Step 4. Place a new block at the destination, and parent it to the Generator.
			GameObject newBlock = Instantiate (terrain, (transform.position + destination), Quaternion.identity) as GameObject;
			newBlock.transform.parent = transform.parent.transform;
			newBlock.name = "Grass";
			adjacentFacesRemaining--;
			
			yield return 0;
		}
	}
	
	private bool castRay (Vector3 direction)
	{
		return Physics.Raycast (transform.position, direction, 1f);
	}
}