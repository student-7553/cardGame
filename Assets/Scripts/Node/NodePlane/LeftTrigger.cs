using UnityEngine;
using Core;

public class LeftTrigger : MonoBehaviour, IStackable
{
	public void stackOnThis(Card draggingCard, Node prevNode)
	{
		object[] tempStorage = new object[3];
		tempStorage[0] = draggingCard;
		tempStorage[1] = "left";
		tempStorage[2] = prevNode;

		gameObject.SendMessageUpwards("cardIsStacking", tempStorage);
	}
}
