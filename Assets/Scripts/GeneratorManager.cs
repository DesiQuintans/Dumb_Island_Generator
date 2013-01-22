using UnityEngine;
using System.Collections;

public class GeneratorManager : MonoBehaviour {
	
	public int expansionAttempts;
	
	public int getAttempts ()
	{
		if (expansionAttempts == 0)
			Debug.Log ("Done!");
		
		return expansionAttempts;
	}
	
	public void spendAttempt ()
	{
		expansionAttempts--;
	}
}
