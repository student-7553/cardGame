using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Core;
using Helpers;
using System.Linq;

public class LeftClickHandler : MonoBehaviour
{
	private InputAction leftClick;
	private Camera mainCamera;
	private LayerMask baseInteractableLayerMask;
	public static LeftClickHandler current;

	private readonly float clickTimer = 0.15f;
	private float checkIntervel = 0.01f;

	private void Awake()
	{
		if (current != null)
		{
			Destroy(gameObject);
			return;
		}
		current = this;

		leftClick = new InputAction(binding: "<Mouse>/leftButton");
		string[] layerNames = { "Interactable", "EnemyInteractable" };
		baseInteractableLayerMask = LayerMask.GetMask(layerNames);
		mainCamera = Camera.main;
	}

	private void OnEnable()
	{
		leftClick.Enable();
		leftClick.performed += MousePressed;
	}

	private void OnDisable()
	{
		leftClick.performed -= MousePressed;
		leftClick.Disable();
	}

	private void MousePressed(InputAction.CallbackContext context)
	{
		Vector3 mousePosition = Mouse.current.position.ReadValue();
		Ray ray = mainCamera.ScreenPointToRay(mousePosition);
		RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 40, baseInteractableLayerMask);
		if (hit.collider != null)
		{
			GameObject hitGameObject = hit.collider.gameObject;
			Interactable interactableObject = hitGameObject.GetComponent(typeof(Interactable)) as Interactable;

			if (interactableObject == null)
			{
				hitGameObject.GetComponent<IClickable>()?.OnClick();
			}
			else
			{
				StartCoroutine(handleClickingInteractable(interactableObject));
			}
		}
	}

	// ################ CUSTOM FUNCTION ################

	private IEnumerator handleClickingInteractable(Interactable interactableObject)
	{
		if (interactableObject.isInteractiveDisabled)
		{
			yield break;
		}

		GameObject interactableGameObject = interactableObject.gameObject;

		Vector3 clickedDifferenceInWorld = findclickedDifferenceInWorld(interactableGameObject);

		yield return new WaitForSeconds(clickTimer);

		if (leftClick.ReadValue<float>() == 0f)
		{
			// Still clicking
			// Is there a better way to do this?
			interactableGameObject.GetComponent<IClickable>()?.OnClick();
		}
		else
		{
			// dragging

			interactableGameObject.transform.position = new Vector3(
				interactableGameObject.transform.position.x,
				interactableGameObject.transform.position.y,
				HelperData.draggingBaseZ
			);

			Node previousStackedNode =
				interactableObject.interactableType == CoreInteractableType.Cards && interactableObject.getCard().isStacked
					? (Node)interactableObject.getCard().joinedStack.connectedNode
					: null;

			List<Interactable> draggingObjects = new List<Interactable>();
			draggingObjects.Add(interactableObject);

			StartCoroutine(dragUpdate(draggingObjects, clickedDifferenceInWorld, previousStackedNode));
		}

		Vector3 findclickedDifferenceInWorld(GameObject interactableGameObject)
		{
			Vector3 mousePosition = Mouse.current.position.ReadValue();
			Vector3 mousePositionInWorld = mainCamera.ScreenToWorldPoint(mousePosition);

			Vector3 clickedDifferenceInWorld = new Vector3(
				interactableGameObject.transform.position.x - mousePositionInWorld.x,
				interactableGameObject.transform.position.y - mousePositionInWorld.y,
				0
			);
			return clickedDifferenceInWorld;
		}
	}

	private IEnumerator dragUpdate(List<Interactable> draggingObjects, Vector3 clickedDifferenceInWorld, Node previousStackedNode)
	{
		float initialDistanceToCamera = Vector3.Distance(draggingObjects[0].gameObject.transform.position, mainCamera.transform.position);

		Vector3 initialPostionOfStack = draggingObjects[0].gameObject.transform.position;

		float dragTimer = 0;

		bool isMiddleLogicEnabled = this.isMiddleLogicEnabled(draggingObjects[0]);
		Debug.Log(isMiddleLogicEnabled);

		while (leftClick.ReadValue<float>() != 0)
		{
			Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
			Vector3 movingToPoint = ray.GetPoint(initialDistanceToCamera);
			movingToPoint = movingToPoint + clickedDifferenceInWorld;

			this.moveInteractableObjects(movingToPoint, draggingObjects);

			dragTimer += Time.deltaTime;
			if (isMiddleLogicEnabled == true && dragTimer > this.checkIntervel)
			{
				dragTimer = 0;
				isMiddleLogicEnabled = this.handleSpecialLogic(draggingObjects[0], initialPostionOfStack, draggingObjects);
			}

			yield return null;
		}
		if (isMiddleLogicEnabled)
		{
			Debug.Log("Are we called?");
			List<Card> draggingCards = draggingObjects
				.Where((draggingObject) => draggingObject.interactableType == CoreInteractableType.Cards)
				.Select((draggingObject) => draggingObject.getCard())
				.ToList();

			draggingObjects[0].getCard().joinedStack.removeCardsFromStack(draggingCards);
		}

		this.dragFinishHandler(draggingObjects, previousStackedNode);
	}

	private bool handleSpecialLogic(Interactable rootInteractable, Vector3 initialPostionOfStack, List<Interactable> draggingObjects)
	{
		Vector3 currentPositionOfCard = rootInteractable.gameObject.transform.position;
		bool isDraggingMoreCardsFromStack = DragAndDropHelper.getDraggingCardsAngle(initialPostionOfStack, currentPositionOfCard);
		if (isDraggingMoreCardsFromStack)
		{
			this.applyDownDragLogic(rootInteractable.getCard(), initialPostionOfStack, draggingObjects);
			return true;
		}
		else
		{
			List<Card> draggingCards = draggingObjects
				.Where((draggingObject) => draggingObject.interactableType == CoreInteractableType.Cards)
				.Select((draggingObject) => draggingObject.getCard())
				.ToList();
			rootInteractable.getCard().joinedStack.removeCardsFromStack(draggingCards);
			return false;
		}
	}

	private void applyDownDragLogic(Card hitCard, Vector3 initialPostionOfCard, List<Interactable> draggingObjects)
	{
		List<Card> qualifiedCards = new List<Card>(
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
		Card rootCard = rootInteractable.getCard();
		return rootCard.isStacked;
	}

	public void dragFinishHandler(List<Interactable> draggingObjects, Node previousStackedNode)
	{
		if (draggingObjects[0].interactableType == CoreInteractableType.Cards)
		{
			IStackable stackableObject = this.findTargetToStack(draggingObjects[0].getCard());

			if (stackableObject != null)
			{
				for (int i = 0; i < draggingObjects.Count; i++)
				{
					stackableObject.stackOnThis(draggingObjects[i].getCard(), previousStackedNode);
				}
				return;
			}

			if (draggingObjects.Count > 1)
			{
				// stacking on the top card of dragging
				for (int i = 1; i < draggingObjects.Count; i++)
				{
					draggingObjects[0].getCard().stackOnThis(draggingObjects[i].getCard(), previousStackedNode);
				}
				return;
			}

			GameObject bottomGameObject = this.findInteractableGameObject(draggingObjects[0].getCard());
			GameObject draggingGameObject = draggingObjects[0].gameObject;

			if (bottomGameObject != null)
			{
				draggingObjects[0].gameObject.transform.position = new Vector3(
					draggingObjects[0].gameObject.transform.position.x,
					draggingObjects[0].gameObject.transform.position.y,
					bottomGameObject.transform.position.z - 1f
				);

				return;
			}

			draggingObjects[0].gameObject.transform.position = new Vector3(
				draggingObjects[0].gameObject.transform.position.x,
				draggingObjects[0].gameObject.transform.position.y,
				HelperData.baseZ
			);

			return;
		}
		else if (draggingObjects[0].interactableType == CoreInteractableType.Nodes)
		{
			GameObject draggingGameObject = draggingObjects[0].gameObject;
			draggingGameObject.transform.position = new Vector3(
				draggingGameObject.transform.position.x,
				draggingGameObject.transform.position.y,
				HelperData.baseZ
			);
		}
	}

	private IStackable findTargetToStack(Card hitCard)
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

	private GameObject findInteractableGameObject(Card hitCard)
	{
		hitCard.computeCorners();
		Vector3[] corners =
		{
			hitCard.corners.leftTopCorner,
			hitCard.corners.rightTopCorner,
			hitCard.corners.leftBottomCorner,
			hitCard.corners.rightBottomCorner
		};

		int i = 0;
		while (i < corners.Length)
		{
			Ray ray = new Ray(corners[i], Vector3.forward);
			RaycastHit2D cornerHit = Physics2D.GetRayIntersection(ray, 20, baseInteractableLayerMask);
			if (cornerHit.collider != null)
			{
				return cornerHit.collider.gameObject;
			}
			i++;
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
