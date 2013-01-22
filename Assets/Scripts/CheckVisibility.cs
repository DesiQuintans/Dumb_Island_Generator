using UnityEngine;

public class CheckVisibility : MonoBehaviour {
	
	public void DestroyIfHidden ()
	{
		if ((CheckFilled (new Vector3(1f, 0f, 0f)) == false) /* posX */ ||
			(CheckFilled (new Vector3(-1f, 0f, 0f)) == false) /* negX */ ||
			(CheckFilled (new Vector3(0f, 0f, 1f)) == false) /* posZ */ ||
			(CheckFilled (new Vector3(0f, 0f, -1f)) == false) /* negZ */ ||
			(CheckFilled (new Vector3(0f, 1f, 0f)) == false) /* posY */)
		{
			// if CheckFilled == false at any point, this block is visible.
			// Destroy this script for performance.
			Destroy (this);
			return;
		}
		else
		{
			// If this method hasn't exited yet, it means this block is hidden.
			Destroy (gameObject);
			return;
		}
	}
	
	private bool CheckFilled (Vector3 direction)
	{
		return Physics.Raycast (transform.position, direction, 1f);
	}
}
