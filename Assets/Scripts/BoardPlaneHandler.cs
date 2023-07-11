using UnityEngine;

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
