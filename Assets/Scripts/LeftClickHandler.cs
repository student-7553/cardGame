using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Core;
using Helpers;
using System.Linq;

public class LeftClickHandler : MonoBehaviour
{
	private Camera mainCamera;
	private LayerMask baseInteractableLayerMask;
	public static LeftClickHandler current;
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

	private GameObject getMouseCloseGameObject(Vector3 mousePosition)
	{
		Ray ray = mainCamera.ScreenPointToRay(mousePosition);
		RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 40, baseInteractableLayerMask);
		if (hit.collider == null)
		{
			return null;
		}

		return hit.collider.gameObject;
	}

	public void handleClickHold(Vector3 pressMousePosition)
	{
		GameObject hitGameObject = this.getMouseCloseGameObject(pressMousePosition);
		if (hitGameObject == null)
		{
			return;
		}
		Interactable interactableObject = hitGameObject.GetComponent(typeof(Interactable)) as Interactable;
		if (interactableObject == null)
		{
			return;
		}
		handleHoldingInteractable(interactableObject);
	}

	public void handleClickHoldEnd()
	{
		isHolding = false;
	}

	public void handleClick()
	{
		Vector3 mousePosition = Mouse.current.position.ReadValue();
		GameObject hitGameObject = this.getMouseCloseGameObject(mousePosition);
		if (hitGameObject == null)
		{
			return;
		}
		hitGameObject.GetComponent<IClickable>()?.OnClick();
	}

	// ################ CUSTOM FUNCTION ################

	public void handleHoldingInteractable(Interactable interactableObject)
	{
		if (interactableObject.isInteractiveDisabled)
		{
			return;
		}

		GameObject interactableGameObject = interactableObject.gameObject;

		interactableGameObject.transform.position = new Vector3(
			interactableGameObject.transform.position.x,
			interactableGameObject.transform.position.y,
			HelperData.draggingBaseZ
		);

		Node previousStackedNode =
			interactableObject.interactableType == CoreInteractableType.Cards && interactableObject.getBaseCard().isStacked
				? (Node)interactableObject.getBaseCard().joinedStack.connectedNode
				: null;

		List<Interactable> draggingObjects = new List<Interactable>();
		draggingObjects.Add(interactableObject);

		StartCoroutine(dragUpdate(draggingObjects, previousStackedNode));
	}

	private IEnumerator dragUpdate(List<Interactable> draggingObjects, Node previousStackedNode)
	{
		float initialDistanceToCamera = Vector3.Distance(draggingObjects[0].gameObject.transform.position, mainCamera.transform.position);

		Vector3 initialPostionOfStack = draggingObjects[0].gameObject.transform.position;

		float dragTimer = 0;

		bool isMiddleLogicEnabled = this.isMiddleLogicEnabled(draggingObjects[0]);

		isHolding = true;
		while (isHolding)
		{
			Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
			Vector3 movingToPoint = ray.GetPoint(initialDistanceToCamera);

			this.moveInteractableObjects(movingToPoint, draggingObjects);

			dragTimer += Time.deltaTime;
			if (isMiddleLogicEnabled == true && dragTimer > this.checkIntervel)
			{
				dragTimer = 0;
				isMiddleLogicEnabled = this.handleMiddleLogic(draggingObjects[0], initialPostionOfStack, draggingObjects);
			}

			yield return null;
		}
		if (isMiddleLogicEnabled)
		{
			List<BaseCard> draggingCards = draggingObjects
				.Where((draggingObject) => draggingObject.interactableType == CoreInteractableType.Cards)
				.Select((draggingObject) => draggingObject.getBaseCard())
				.ToList();

			draggingObjects[0].getBaseCard().joinedStack.removeCardsFromStack(draggingCards);
		}

		this.dragFinishHandler(draggingObjects, previousStackedNode);
	}

	private bool handleMiddleLogic(Interactable rootInteractable, Vector3 initialPostionOfStack, List<Interactable> draggingObjects)
	{
		Vector3 currentPositionOfCard = rootInteractable.gameObject.transform.position;
		bool isEnded = DragAndDropHelper.getDraggingCardsAngle(initialPostionOfStack, currentPositionOfCard);
		if (isEnded)
		{
			this.applyDownDragLogic(rootInteractable.getBaseCard(), initialPostionOfStack, draggingObjects);
			return true;
		}
		else
		{
			List<BaseCard> draggingCards = draggingObjects
				.Where((draggingObject) => draggingObject.interactableType == CoreInteractableType.Cards)
				.Select((draggingObject) => draggingObject.getBaseCard())
				.ToList();
			rootInteractable.getBaseCard().joinedStack.removeCardsFromStack(draggingCards);
			return false;
		}
	}

	private void applyDownDragLogic(BaseCard hitCard, Vector3 initialPostionOfCard, List<Interactable> draggingObjects)
	{
		List<BaseCard> qualifiedCards = new List<BaseCard>(
			hitCard.joinedStack.cards.Where(stacksSingleCard =>
			{
				return !draggingObjects.Any(
					singleDraggingObject => singleDraggingObject.gameObject.GetInstanceID() == stacksSingleCard.gameObject.GetInstanceID()
				);
			})
		);
		foreach (Card singleCard in qualifiedCards)
		{
			if (
				hitCard.gameObject.transform.position.y < singleCard.gameObject.transform.position.y
				&& initialPostionOfCard.y > singleCard.gameObject.transform.position.y
			)
			{
				Interactable interactableGameObject = DragAndDropHelper.getInteractableFromGameObject(singleCard.gameObject);
				draggingObjects.Add(interactableGameObject);
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
		if (rootInteractable.interactableType != CoreInteractableType.Cards)
		{
			return false;
		}
		BaseCard rootCard = rootInteractable.getBaseCard();
		return rootCard.isStacked;
	}

	public void dragFinishHandler(List<Interactable> draggingObjects, Node previousStackedNode)
	{
		if (draggingObjects[0].interactableType == CoreInteractableType.Cards)
		{
			IStackable stackableObject = this.findTargetToStack(draggingObjects[0].getBaseCard());

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
			this.moveInteractableObjects(movingToPoint, singleInteractable);
		}
	}

	private void moveInteractableObjects(Vector3 movingToPoint, Interactable interactableObject)
	{
		GameObject interactableGameObject = interactableObject.gameObject;
		Vector3 finalMovingPoint = movingToPoint;
		finalMovingPoint.z = interactableGameObject.transform.position.z;
		interactableGameObject.transform.position = finalMovingPoint;
	}
}
