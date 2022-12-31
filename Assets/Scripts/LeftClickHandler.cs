using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Core;
using Helpers;

public class LeftClickHandler : MonoBehaviour
{
	private InputAction leftClick;
	private Camera mainCamera;
	private LayerMask interactableLayerMask;

	private readonly float clickTimer = 0.15f;

	private void Awake()
	{
		leftClick = new InputAction(binding: "<Mouse>/leftButton");
		interactableLayerMask = LayerMask.GetMask("Interactable");
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
		RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 40, interactableLayerMask);
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
		if (interactableObject.isDisabled)
		{
			yield break;
		}

		GameObject interactableGameObject = interactableObject.getGameObject();

		Vector3 clickedDifferenceInWorld = findclickedDifferenceInWorld(interactableGameObject);

		yield return new WaitForSeconds(clickTimer);

		if (leftClick.ReadValue<float>() == 0f)
		{
			// Still clicking
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

			Card singleCard = interactableObject.getCard();
			if (singleCard != null && singleCard.isStacked)
			{
				List<Card> draggingCards = new List<Card>();
				draggingCards.Add(singleCard);
				singleCard.joinedStack.removeCardsFromStack(draggingCards);
			}

			StartCoroutine(dragUpdate(interactableObject, clickedDifferenceInWorld));
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

	private IEnumerator dragUpdate(Interactable draggingObject, Vector3 clickedDifferenceInWorld)
	{
		GameObject rootObject = draggingObject.getGameObject();

		float initialDistanceToCamera = Vector3.Distance(rootObject.transform.position, mainCamera.transform.position);

		Vector3 initialPostionOfStack = rootObject.transform.position;

		while (leftClick.ReadValue<float>() != 0)
		{
			Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
			Vector3 movingToPoint = ray.GetPoint(initialDistanceToCamera);
			movingToPoint = movingToPoint + clickedDifferenceInWorld;

			this.moveInteractableObjects(movingToPoint, draggingObject);

			yield return null;
		}

		dragFinishHandler(draggingObject);
	}

	private void dragFinishHandler(Interactable draggingObject)
	{
		// WE ARE ALWAYS ASSUMING THAT EVERYTHING IN A LIST IS THE SAME CLASSES
		Interactable bottomInteractable = draggingObject;
		if (bottomInteractable.interactableType == CoreInteractableType.Cards)
		{
			// is card
			Card bottomCard = bottomInteractable.getCard();
			IStackable stackableObject = this.findTargetToStack(bottomCard);
			if (stackableObject != null)
			{
				Card stackingCard = draggingObject.getCard();
				// List<Card> stackingCards = new List<Card> { stackingCard };
				List<Card> stackingCards = new List<Card>();
				stackingCards.Add(stackingCard);

				stackableObject.stackOnThis(stackingCards);
			}
			else
			{
				GameObject bottomGameObject = this.findInteractableGameObject(bottomCard);
				GameObject draggingGameObject = draggingObject.getGameObject();

				if (bottomGameObject != null)
				{
					draggingGameObject.transform.position = new Vector3(
						draggingGameObject.transform.position.x,
						draggingGameObject.transform.position.y,
						bottomGameObject.transform.position.z - 1f
					);
				}
				else
				{
					draggingGameObject.transform.position = new Vector3(
						draggingGameObject.transform.position.x,
						draggingGameObject.transform.position.y,
						HelperData.baseZ
					);
				}
			}
		}
		else if (bottomInteractable.interactableType == CoreInteractableType.Nodes)
		{
			GameObject draggingGameObject = draggingObject.getGameObject();
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
		int index = 0;
		while (index < corners.Length)
		{
			Ray ray = new Ray(corners[index], Vector3.forward);

			RaycastHit2D cornerHit = Physics2D.GetRayIntersection(ray, 20, interactableLayerMask);
			if (cornerHit.collider != null)
			{
				IStackable stackableObject = cornerHit.collider.gameObject.GetComponent(typeof(IStackable)) as IStackable;
				if (stackableObject != null)
				{
					return stackableObject;
				}
			}
			index++;
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
			RaycastHit2D cornerHit = Physics2D.GetRayIntersection(ray, 20, interactableLayerMask);
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
		GameObject interactableGameObject = interactableObject.getGameObject();
		Vector3 finalMovingPoint = movingToPoint;
		finalMovingPoint.z = interactableGameObject.transform.position.z;
		interactableGameObject.transform.position = finalMovingPoint;
	}
}
