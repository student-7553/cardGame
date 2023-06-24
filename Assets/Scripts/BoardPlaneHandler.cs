using UnityEngine;

// Stores the current enabled board
// Okay why is this not just the Unity data storer? Yes it should be tbh
// In there it should also store all the current nodes in play to
public class BoardPlaneHandler : MonoBehaviour
{
	[System.NonSerialized]
	private NodePlaneHandler currentNodePlaneHandler = null;

	public void setActiveNodePlane(NodePlaneHandler newNodePlaneHandler)
	{
		if (currentNodePlaneHandler != null && currentNodePlaneHandler != newNodePlaneHandler)
		{
			currentNodePlaneHandler.gameObject.SetActive(false);
		}

		currentNodePlaneHandler = newNodePlaneHandler;
	}
}
