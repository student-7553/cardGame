using UnityEngine;
using Core;

public class BoardPlaneHandler : MonoBehaviour, IClickable
{
	private NodePlaneHandler currentNodePlaneHandler;

	public void OnClick()
	{
		if (currentNodePlaneHandler != null)
		{
			currentNodePlaneHandler.gameObject.SetActive(false);
		}
	}

	public void setActiveNodePlane(NodePlaneHandler newNodePlaneHandler)
	{
		if (currentNodePlaneHandler != newNodePlaneHandler)
		{
			currentNodePlaneHandler.gameObject.SetActive(false);
		}

		currentNodePlaneHandler = newNodePlaneHandler;
	}
}
