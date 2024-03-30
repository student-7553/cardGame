using UnityEngine;
using TMPro;
using Core;
using Helpers;

public class NodePlaneHandler : MonoBehaviour, IStackable
{
	private BaseNode connectedNode;
	public TextMeshPro titleTextMesh;
	public SO_Interactable so_Interactable;
	public SO_Highlight so_Highlight;
	public GameObject dimSpriteObject;

	private void Awake()
	{
		Component[] textMeshes = gameObject.GetComponentsInChildren(typeof(TextMeshPro));
		titleTextMesh = textMeshes[0] as TextMeshPro;
		so_Highlight.triggerAction.Add(triggerDimRefresh);
	}

	private void Start()
	{
		triggerDimRefresh();
	}

	private void OnDestroy()
	{
		so_Highlight.triggerAction.Remove(triggerDimRefresh);
	}

	private void triggerDimRefresh()
	{
		if (so_Highlight.isHighlightEnabled)
		{
			dimSpriteObject.SetActive(true);
		}
		else
		{
			dimSpriteObject.SetActive(false);
		}
	}

	public void init(BaseNode parentNode)
	{
		connectedNode = parentNode;
	}

	private void OnEnable()
	{
		if (connectedNode == null)
		{
			return;
		}

		so_Interactable.setActiveNodePlane(this);
	}

	public void stackOnThis(BaseCard draggingCard, Node prevNode)
	{
		connectedNode.stackOnThis(draggingCard, prevNode);
	}

	private void FixedUpdate()
	{
		if (connectedNode == null)
		{
			return;
		}
		updatePosition();
		reflectToScreen();
	}

	public void updatePosition()
	{
		Vector3 nodePlanePositon = new Vector3(
			connectedNode.gameObject.transform.position.x,
			connectedNode.gameObject.transform.position.y + 27,
			HelperData.nodeBoardZ
		);
		if (transform.position != nodePlanePositon)
		{
			transform.position = nodePlanePositon;
		}
	}

	private void reflectToScreen()
	{
		titleTextMesh.text = "";
		if (!connectedNode.isActive)
		{
			titleTextMesh.text = "[No Food] " + titleTextMesh.text;
		}
	}
}
