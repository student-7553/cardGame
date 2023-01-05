using UnityEngine;
using Core;

public class BoardPlaneHandler : MonoBehaviour, IClickable
{
	[System.NonSerialized]
	private NodePlaneHandler currentNodePlaneHandler = null;

	public void OnClick()
	{
		if (currentNodePlaneHandler != null)
		{
			currentNodePlaneHandler.gameObject.SetActive(false);
		}
	}

	public void setActiveNodePlane(NodePlaneHandler newNodePlaneHandler)
	{
		if (currentNodePlaneHandler != null && currentNodePlaneHandler != newNodePlaneHandler)
		{
			currentNodePlaneHandler.gameObject.SetActive(false);
		}

		currentNodePlaneHandler = newNodePlaneHandler;
	}
}
