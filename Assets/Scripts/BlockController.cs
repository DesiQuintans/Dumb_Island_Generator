using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockController : MonoBehaviour
{
	public GameObject terrain;
	public int expansionProb;
	public int verticality;
	private int adjacentFacesRemaining = 5;
	
	private List<Vector3> vertEmptySpaces = new List<Vector3>() {};
	private List<Vector3> horEmptySpaces = new List<Vector3>() {};
	
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
			if (adjacentFacesRemaining <= 0)
			{
				Destroy (gameObject);
				break;
			}
		
			if ((parentManager.getAttempts () <= 0))
			{
//				Debug.Log ("This block is exhausted. Cleaning up.");
				Destroy (this);
				GetComponent<CheckVisibility>().DestroyIfHidden ();
				break;
			}
			
			// Step 1. Check for empty spaces, and store them in separate lists.
			// Check size of lists. If both empty, there are no empty spaces around it.
			castRay (posX);
			castRay (negX);
			castRay (posZ);
			castRay (negZ);
			castRay (posY);
			
//			Debug.Log ("Vert list size: " + vertEmptySpaces.Count + ". Hor list size: " + horEmptySpaces.Count + ".");
			if (vertEmptySpaces.Count == 0 && horEmptySpaces.Count == 0)
			{
				// Do nothing.
//				Debug.Log ("Both lists empty. Doing nothing.");
				adjacentFacesRemaining = 0;
				ClearAdjacencyLists ();
				continue;
			}
			else
			{
				parentManager.spendAttempt ();
			}
			
			yield return 0;
			
			// Step 4. Generate random int and check against expansion probability.
			if (Random.Range (0, 101) > expansionProb)
			{
				// Generated number was outside expansion probability. Expansion should not occur.
//				Debug.Log ("Not expanding this cycle.");
				ClearAdjacencyLists ();
				continue;
			}
			
			// Step 5. Choose a direction to expand. If one of the lists is empty, then we do an extra check to see if the placement goes ahead.
			Vector3 destinationPosition;
			if (horEmptySpaces.Count == 0 && vertEmptySpaces.Count != 0)
			{
				// Must use vert list.
//				Debug.Log ("Must use vert list.");
				if (Random.Range (0, 101) <= verticality)
				{
//					Debug.Log ("Building upwards (the only option)");
					destinationPosition = vertEmptySpaces[Random.Range (0, vertEmptySpaces.Count)];
				}
				else
				{
//					Debug.Log ("Not expanding this cycle.");
					ClearAdjacencyLists ();
					continue;
				}
			}
			else if (vertEmptySpaces.Count == 0 && horEmptySpaces.Count != 0)
			{
				// Must use hor list.
//				Debug.Log ("Must use hor list.");
				if (Random.Range (0, 101) > verticality)
				{
//					Debug.Log ("Building horizontally (the only option).");
					destinationPosition = horEmptySpaces[Random.Range (0, horEmptySpaces.Count)];
				}
				else
				{
//					Debug.Log ("Not expanding this cycle.");
					ClearAdjacencyLists ();
					continue;
				}
			}
			else if (vertEmptySpaces.Count != 0 && horEmptySpaces.Count != 0)
			{
				// Can use any list.
//				Debug.Log ("Both lists are non-empty.");
				
				if (Random.Range (0, 101) <= verticality)
				{
					// Use vert list.
//					Debug.Log ("Building upwards.");
					destinationPosition = vertEmptySpaces[Random.Range (0, vertEmptySpaces.Count)];
				}
				else
				{
					// Use hor list.
//					Debug.Log ("Building horizontally.");
					destinationPosition = horEmptySpaces[Random.Range (0, horEmptySpaces.Count)];
				}
			}
			else
			{
				// This is an error.
				Debug.LogError ("ERROR: Both lists should not be empty. Destination set to world origin.");
				destinationPosition = Vector3.zero;
			}
			
			yield return 0;
			
			// Step 6. Place a new block at the destination, and parent it to the Generator.
			GameObject newBlock = Instantiate (terrain, destinationPosition, Quaternion.identity) as GameObject;
			newBlock.transform.parent = transform.parent.transform;
			newBlock.name = "Grass";
			adjacentFacesRemaining--;
			
			// Method finished running.
			ClearAdjacencyLists ();
			
			yield return 0;
		}
	}
	
	private void castRay (Vector3 direction)
	{
		if (Physics.Raycast (transform.position, direction, 1f) == false)
		{
			// Empty space here
			if (direction == posY)
			{
				vertEmptySpaces.Add (transform.position + direction);
			}
			else
			{
				horEmptySpaces.Add (transform.position + direction);
			}
		}
	}
	
	private void ClearAdjacencyLists ()
	{
		vertEmptySpaces.Clear();
		horEmptySpaces.Clear();
		return;
	}
}


/*
			// Step 5. Choose a direction to expand. If one of the lists is empty, then we do an extra check to see if the placement goes ahead.
			Vector3 destinationPosition;
			if (horEmptySpaces.Count == 0 && vertEmptySpaces.Count != 0)
			{
				// Must use vert list.
//				Debug.Log ("Must use vert list.");
				if (Random.Range (0, 101) <= verticality)
				{
//					Debug.Log ("Building upwards (the only option)");
					destinationPosition = vertEmptySpaces[Random.Range (0, vertEmptySpaces.Count)];
				}
				else
				{
//					Debug.Log ("Not expanding this cycle.");
					ClearAdjacencyLists ();
					continue;
				}
			}
			else if (vertEmptySpaces.Count == 0 && horEmptySpaces.Count != 0)
			{
				// Must use hor list.
//				Debug.Log ("Must use hor list.");
				if (Random.Range (0, 101) > verticality)
				{
//					Debug.Log ("Building horizontally (the only option).");
					destinationPosition = horEmptySpaces[Random.Range (0, horEmptySpaces.Count)];
				}
				else
				{
//					Debug.Log ("Not expanding this cycle.");
					ClearAdjacencyLists ();
					continue;
				}
			}
			else if (vertEmptySpaces.Count != 0 && horEmptySpaces.Count != 0)
			{
				// Can use any list.
//				Debug.Log ("Both lists are non-empty.");
				
				if (Random.Range (0, 101) <= verticality)
				{
					// Use vert list.
//					Debug.Log ("Building upwards.");
					destinationPosition = vertEmptySpaces[Random.Range (0, vertEmptySpaces.Count)];
				}
				else
				{
					// Use hor list.
//					Debug.Log ("Building horizontally.");
					destinationPosition = horEmptySpaces[Random.Range (0, horEmptySpaces.Count)];
				}
			}
			else
			{
				// This is an error.
				Debug.LogError ("ERROR: Both lists should not be empty. Destination set to world origin.");
				destinationPosition = Vector3.zero;
			}
*/


/*
			// Step 5. Choose a direction to expand.
			Vector3 destinationPosition;
			if (horEmptySpaces.Count == 0 && vertEmptySpaces.Count != 0)
			{
				// Must use vert list.
//				Debug.Log ("Must use vert list.");
				destinationPosition = vertEmptySpaces[Random.Range (0, vertEmptySpaces.Count)];
			}
			else if (vertEmptySpaces.Count == 0 && horEmptySpaces.Count != 0)
			{
				// Must use hor list.
//				Debug.Log ("Must use hor list.");
				destinationPosition = horEmptySpaces[Random.Range (0, horEmptySpaces.Count)];
			}
			else if (vertEmptySpaces.Count != 0 && horEmptySpaces.Count != 0)
			{
				// Can use any list.
//				Debug.Log ("Both lists are non-empty.");
				
				if (Random.Range (0, 101) <= verticality)
				{
					// Use vert list.
//					Debug.Log ("Building upwards.");
					destinationPosition = vertEmptySpaces[Random.Range (0, vertEmptySpaces.Count)];
				}
				else
				{
					// Use hor list.
//					Debug.Log ("Building horizontally.");
					destinationPosition = horEmptySpaces[Random.Range (0, horEmptySpaces.Count)];
				}
			}
			else
			{
				// This is an error.
				Debug.LogError ("ERROR: Both lists should not be empty. Destination set to world origin.");
				destinationPosition = Vector3.zero;
			}
*/