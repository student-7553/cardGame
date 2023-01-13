using UnityEngine;
using Core;

public class RightTrigger : MonoBehaviour, IStackable
{
	public void stackOnThis(Card draggingCard, Node prevNode)
	{
		object[] tempStorage = new object[3];
		tempStorage[0] = draggingCard;
		tempStorage[1] = "right";
		tempStorage[2] = prevNode;

		gameObject.SendMessageUpwards("cardIsStacking", tempStorage);
	}
}
