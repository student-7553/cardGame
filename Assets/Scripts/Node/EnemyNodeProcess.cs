using UnityEngine;
using System.Collections.Generic;
using Core;
using Helpers;

public class EnemyNodeProcess : MonoBehaviour
{
	private EnemyNode connectedNode;

	public void Awake()
	{
		connectedNode = gameObject.GetComponent(typeof(EnemyNode)) as EnemyNode;
	}

	private void FixedUpdate()
	{
		List<int> cardIds = connectedNode.processCardStack.getAllActiveCardIds();
		int currentCardValue = CardHelpers.getTypeValueFromCardIds(CardsTypes.CombatUnit, cardIds);
		if (connectedNode.powerValue <= currentCardValue)
		{
			Debug.Log("Destoryed it ");
			// Todo:destroy
			connectedNode.isActive = false;
		}
	}
}
