using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Core;
using Helpers;
using System.Linq;
using UnityEngine.UIElements;

public class LeftClickHandler : MonoBehaviour
{
	private Camera mainCamera;
	private LayerMask baseInteractableLayerMask;
	public static LeftClickHandler current;

	// public StaticVariables staticVariables;
	private bool isHolding;

	private float checkIntervel = 0.01f;

	private void Awake()
	{
		if (current != null)
		{
			Destroy(gameObject);
			return;
		}
		current = this;

		string[] layerNames = { "Interactable", "EnemyInteractable" };
		baseInteractableLayerMask = LayerMask.GetMask(layerNames);
		mainCamera = Camera.main;
	}

	private GameObject getMouseCloseGameObject(Vector2 mousePosition)
	{
		Ray ray = mainCamera.ScreenPointToRay(mousePosition);
		RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 40, baseInteractableLayerMask);
		if (hit.collider == null)
		{
			return null;
		}

		return hit.collider.gameObject;
	}

	public void handleClickHold(Vector2 pressMousePosition)
	{
		GameObject hitGameObject = getMouseCloseGameObject(pressMousePosition);
		if (hitGameObject == null)
		{
			return;
		}
		IMouseHoldable interactableObject = hitGameObject.GetComponent<IMouseHoldable>();
		if (interactableObject == null)
		{
			return;
		}

		Interactable[] holdInteractables = interactableObject.getMouseHoldInteractables();
		if (holdInteractables == null || holdInteractables.Length == 0 || holdInteractables[0].isInteractiveDisabled)
		{
			return;
		}

		handleHoldingInteractable(holdInteractables);
	}

	public void handleClickHoldEnd()
	{
		isHolding = false;
	}

	public void handleClick()
	{
		Vector2 mousePosition = Mouse.current.position.ReadValue();
		GameObject hitGameObject = getMouseCloseGameObject(mousePosition);
		if (hitGameObject == null)
		{
			return;
		}
		hitGameObject.GetComponent<IClickable>()?.OnClick();
	}

	// ################ CUSTOM FUNCTION ################

	public void handleHoldingInteractable(Interactable[] interactableObjects)
	{
		foreach (Interactable interactableObject in interactableObjects)
		{
			interactableObject.gameObject.transform.position = new Vector3(
				interactableObject.gameObject.transform.position.x,
				interactableObject.gameObject.transform.position.y,
				HelperData.draggingBaseZ
			);
		}

		Node previousStackedNode =
			interactableObjects[0].isCardType() && interactableObjects[0].getBaseCard().isStacked()
				? (Node)interactableObjects[0].getBaseCard().joinedStack.getNode()
				: null;

		List<Interactable> draggingObjects = new List<Interactable>(interactableObjects);

		StartCoroutine(dragUpdate(draggingObjects, previousStackedNode));
	}

	private IEnumerator dragUpdate(List<Interactable> draggingObjects, Node previousStackedNode)
	{
		float initialDistanceToCamera = Vector3.Distance(draggingObjects[0].gameObject.transform.position, mainCamera.transform.position);
		Vector3 initialPostionOfStack = draggingObjects[0].gameObject.transform.position;

		float dragTimer = 0;
		bool isMiddleLogicEnabled = this.isMiddleLogicEnabled(draggingObjects[0]);

		foreach (Interactable singleDraggingObject in draggingObjects)
		{
			singleDraggingObject.setSpriteHovering(true, Interactable.SpriteInteractable.hover);
		}

		isHolding = true;
		while (isHolding)
		{
			foreach (Interactable singleDraggingObject in draggingObjects)
			{
				if (singleDraggingObject.isCardType())
				{
					singleDraggingObject.isInteractiveDisabled = true;
				}
			}

			Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
			Vector3 movingToPoint = ray.GetPoint(initialDistanceToCamera);

			moveInteractableObjects(movingToPoint, draggingObjects);

			dragTimer += Time.deltaTime;
			if (isMiddleLogicEnabled == true && dragTimer > checkIntervel)
			{
				dragTimer = 0;
				isMiddleLogicEnabled = handleMiddleLogic(draggingObjects[0], initialPostionOfStack, draggingObjects);
			}

			yield return new WaitForEndOfFrame();
		}
		if (isMiddleLogicEnabled)
		{
			List<BaseCard> draggingCards = draggingObjects
				.Where((draggingObject) => draggingObject.isCardType())
				.Select((draggingObject) => draggingObject.getBaseCard())
				.ToList();

			draggingObjects[0].getBaseCard().joinedStack.removeCardsFromStack(draggingCards);
		}

		foreach (Interactable singleDraggingObject in draggingObjects)
		{
			if (singleDraggingObject.isCardType())
			{
				singleDraggingObject.isInteractiveDisabled = false;
			}
			singleDraggingObject.setSpriteHovering(false, Interactable.SpriteInteractable.hover);
		}

		dragFinishHandler(draggingObjects, previousStackedNode);
	}

	private bool handleMiddleLogic(Interactable rootInteractable, Vector3 initialPostionOfStack, List<Interactable> draggingObjects)
	{
		Vector3 currentPositionOfCard = rootInteractable.gameObject.transform.position;
		bool isEnded = DragAndDropHelper.getDraggingCardsAngle(initialPostionOfStack, currentPositionOfCard);
		if (isEnded)
		{
			applyDownDragLogic(rootInteractable.getBaseCard(), initialPostionOfStack, draggingObjects);
			return true;
		}
		else
		{
			List<BaseCard> draggingCards = draggingObjects
				.Where((draggingObject) => draggingObject.isCardType())
				.Select((draggingObject) => draggingObject.getBaseCard())
				.ToList();
			rootInteractable.getBaseCard().joinedStack.removeCardsFromStack(draggingCards);
			return false;
		}
	}

	private void applyDownDragLogic(BaseCard hitCard, Vector3 initialPostionOfCard, List<Interactable> draggingObjects)
	{
		List<BaseCard> qualifiedCards = new List<BaseCard>(
			hitCard.joinedStack
				.getCards()
				.Where(stacksSingleCard =>
				{
					return !draggingObjects.Any(
						singleDraggingObject =>
							singleDraggingObject.gameObject?.GetInstanceID() == stacksSingleCard.gameObject?.GetInstanceID()
					);
				})
		);
		foreach (BaseCard singleCard in qualifiedCards)
		{
			if (
				hitCard.gameObject.transform.position.y < singleCard.gameObject.transform.position.y
				&& initialPostionOfCard.y > singleCard.gameObject.transform.position.y
			)
			{
				Interactable interactableGameObject = DragAndDropHelper.getInteractableFromGameObject(singleCard.gameObject);
				draggingObjects.Add(interactableGameObject);
				interactableGameObject.setSpriteHovering(true, Interactable.SpriteInteractable.hover);
				singleCard.gameObject.transform.position = new Vector3(
					singleCard.gameObject.transform.position.x,
					singleCard.gameObject.transform.position.y,
					HelperData.draggingBaseZ - ((draggingObjects.Count - 1) * 0.01f)
				);
			}
		}
	}

	private bool isMiddleLogicEnabled(Interactable rootInteractable)
	{
		if (!rootInteractable.isCardType())
		{
			return false;
		}
		BaseCard rootCard = rootInteractable.getBaseCard();
		return rootCard.isStacked();
	}

	public IEnumerator delayedDragFinish(List<BaseCard> cards, Node node)
	{
		for (int index = 0; index < cards.Count; index++)
		{
			cards[index].isInteractiveDisabled = false;
		}

		for (int index = 0; index < cards.Count; index++)
		{
			yield return new WaitForEndOfFrame();
			if (cards[index] != null)
			{
				dragFinishHandler(new List<Interactable>() { cards[index] }, node);
			}
		}
		// }
	}

	public void dragFinishHandler(List<Interactable> draggingObjects, Node previousStackedNode)
	{
		if (draggingObjects[0].isCardType())
		{
			IStackable stackableObject = findTargetToStack(draggingObjects[0].getBaseCard());

			if (stackableObject != null)
			{
				for (int i = 0; i < draggingObjects.Count; i++)
				{
					stackableObject.stackOnThis(draggingObjects[i].getBaseCard(), previousStackedNode);
				}
				return;
			}

			if (draggingObjects.Count > 1)
			{
				// stacking on the top card of dragging
				for (int i = 1; i < draggingObjects.Count; i++)
				{
					draggingObjects[0].getBaseCard().stackOnThis(draggingObjects[i].getBaseCard(), previousStackedNode);
				}
				return;
			}

			draggingObjects[0].gameObject.transform.position = new Vector3(
				draggingObjects[0].gameObject.transform.position.x,
				draggingObjects[0].gameObject.transform.position.y,
				HelperData.baseZ
			);

			return;
		}
		// Node dragging
		GameObject draggingGameObject = draggingObjects[0].gameObject;
		draggingGameObject.transform.position = new Vector3(
			draggingGameObject.transform.position.x,
			draggingGameObject.transform.position.y,
			HelperData.baseZ
		);
	}

	private IStackable findTargetToStack(BaseCard hitCard)
	{
		hitCard.computeCorners();
		Vector3[] corners =
		{
			hitCard.corners.leftTopCorner,
			hitCard.corners.rightTopCorner,
			hitCard.corners.leftBottomCorner,
			hitCard.corners.rightBottomCorner
		};

		for (int index = 0; index < corners.Length; index++)
		{
			Ray ray = new Ray(corners[index], Vector3.forward);

			RaycastHit2D cornerHit = Physics2D.GetRayIntersection(ray, 20, baseInteractableLayerMask);
			if (cornerHit.collider != null)
			{
				IStackable stackableObject = cornerHit.collider.gameObject.GetComponent(typeof(IStackable)) as IStackable;
				if (stackableObject != null)
				{
					return stackableObject;
				}
			}
		}
		return null;
	}

	private void moveInteractableObjects(Vector3 movingToPoint, List<Interactable> interactableObjects)
	{
		foreach (Interactable singleInteractable in interactableObjects)
		{
			GameObject interactableGameObject = singleInteractable.gameObject;

			Vector3 finalMovingPoint = new Vector3(movingToPoint.x, movingToPoint.y, interactableGameObject.transform.position.z);
			PositionRestricted positionRestricted = singleInteractable as PositionRestricted;
			if (positionRestricted != null)
			{
				finalMovingPoint = positionRestricted.getFinalPosition(finalMovingPoint);
			}

			interactableGameObject.transform.position = Vector3.SmoothDamp(
				interactableGameObject.transform.position,
				finalMovingPoint,
				ref singleInteractable.getCurrentVelocity(),
				HelperData.cardReachSmoothTime
			);
		}
	}
}
