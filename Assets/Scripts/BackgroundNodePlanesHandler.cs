using UnityEngine;
using Core;

public class BackgroundNodePlanesHandler : MonoBehaviour, IClickable
{
    public void OnClick()
    {
        NodePlaneManagers nodePlaneManagers = gameObject.GetComponentInParent(typeof(NodePlaneManagers)) as NodePlaneManagers;
        nodePlaneManagers?.OnClick();
        
    }
}
