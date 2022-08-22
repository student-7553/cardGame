
using UnityEngine;

namespace CardGlobal
{
    public class Card : MonoBehaviour 
    {
        public Vector3 leftTopCorner;
        public Vector3 rightTopCorner;
        public Vector3 leftBottomCorner;
        public Vector3 rightBottomCorner;
        public void convertToCard(GameObject clickedObject){

            leftTopCorner = new Vector3();
            rightTopCorner = new Vector3();
            leftBottomCorner = new Vector3();
            rightBottomCorner = new Vector3();
            
        }
    }
}