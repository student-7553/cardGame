using UnityEngine;
using Core;

public class NodePlaneManagers : MonoBehaviour, IClickable
{
    //   This script handles opening and closing of the node plane
    private int baseYPosition = 3;

    public GameObject rootNodePlane;

    public GameObject currentNodePlane;

    public void OnClick()
    {
        if (currentNodePlane.activeSelf == true)
        {
            currentNodePlane.SetActive(false);
        }
        else
        {
            currentNodePlane.SetActive(true);
        }
    }
    public void init()
    {
        currentNodePlane = Instantiate(rootNodePlane, new Vector3(70, baseYPosition, 40), Quaternion.identity, gameObject.transform);
        currentNodePlane.SetActive(false);
    }
}
