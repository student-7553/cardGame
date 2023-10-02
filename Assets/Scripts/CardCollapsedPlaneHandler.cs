using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

public class CardCollapsedPlaneHandler : MonoBehaviour, IClickable
{
	private CardCollapsed cardCollapsed;

	// Start is called before the first frame update
	void Start() { }

	// Update is called once per frame
	void Update() { }

	public void init(CardCollapsed cardCollapsed)
	{
		this.cardCollapsed = cardCollapsed;
	}

	public void OnClick()
	{
		gameObject.SetActive(false);
	}
}
