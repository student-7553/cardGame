using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class NodeHightlightHandler : MonoBehaviour
{
	public SO_Highlight soHightlight;
	private SpriteHightlightToggler spriteHightlightToggler;

	// private TextHightlightToggler textHightlightToggler;
	public Node node;

	public bool isOverriden;

	private void Awake()
	{
		spriteHightlightToggler = GetComponent<SpriteHightlightToggler>();
		Assert.IsNotNull(soHightlight);
		Assert.IsNotNull(spriteHightlightToggler);
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
			spriteHightlightToggler.isOverriden = false;
		}
		else
		{
			spriteHightlightToggler.isOverriden = true;
		}
		spriteHightlightToggler.triggerDimRefresh();
	}
}
