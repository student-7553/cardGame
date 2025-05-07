using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class NodeTextHightlightHandler : MonoBehaviour
{
	public SO_Highlight soHightlight;
	private TextHightlightToggler textHightlightToggler;
	public Node node;

	public bool isOverriden;

	private void Awake()
	{
		textHightlightToggler = GetComponent<TextHightlightToggler>();
		Assert.IsNotNull(soHightlight);
		Assert.IsNotNull(textHightlightToggler);
		Assert.IsNotNull(node);

		soHightlight.triggerAction.Add(triggerRefresh);
		triggerRefresh();
	}

	private void OnDestroy()
	{
		soHightlight.triggerAction.Remove(triggerRefresh);
	}

	private void triggerRefresh()
	{
		if (!soHightlight.cardIds.Any((cardId) => cardId == node.id))
		{
			textHightlightToggler.isOverriden = false;
		}
		else
		{
			textHightlightToggler.isOverriden = true;
		}
		textHightlightToggler.triggerDimRefresh();
	}
}
