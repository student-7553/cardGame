
using UnityEngine;
using System.Collections.Generic;
using TMPro;
namespace Core
{
    public class Node : MonoBehaviour, Stackable, Dragable
    {
        enum nodeStateTypes
        {
            low,
            medium,
        };

        [System.NonSerialized]
        public CardStack activeStack;


        private TextMeshPro titleTextMesh;
        private TextMeshPro availableInventoryTextMesh;
        public string title;


        // --------------------STATS-------------------------

        private int _inventoryLimit;
        public int inventoryLimit
        {
            get { return _inventoryLimit; }
            set { _inventoryLimit = value; }
        }

        private int _availableInventory;
        public int availableInventory
        {
            get { return _availableInventory; }
            set { _availableInventory = value; }
        }



        // --------------------INTERVAL CHECK-------------------------

        private float intervalTimer = 0;
        private int hungerSetIntervalTimer; // sec
        private int currentHungerCheck; 

        nodeStateTypes nodeState;

        private void initlizeBaseStats(){
            _inventoryLimit = 10;
            _availableInventory = 10;
            hungerSetIntervalTimer = 60; 
            currentHungerCheck = 1;
        }

        private void Awake()
        {
            nodeState = nodeStateTypes.low;
            Component[] textMeshes = gameObject.GetComponentsInChildren(typeof(TextMeshPro));
            titleTextMesh = textMeshes[0] as TextMeshPro;
            availableInventoryTextMesh = textMeshes[1] as TextMeshPro;
            initlizeBaseStats();
        }

        public void init()
        {
            reflectOnScreen();
        }

        private void reflectOnScreen () {
            titleTextMesh.text = title;
            availableInventoryTextMesh.text = "" + _inventoryLimit + "/" + _availableInventory;
        }

        private void Update() {
            intervalTimer = intervalTimer + Time.deltaTime;
            if (intervalTimer > hungerSetIntervalTimer){
                handleHungerInterval();
                intervalTimer = 0;
            }
        }

        private void handleHungerInterval(){

        }

        

        public void stackOnThis(List<Card> draggingCards)
        {
            Debug.Log("Are we called??");
        }

        public void setCardStackOfNode(CardStack cardStack)
        {
            activeStack = cardStack;
        }

        public void clearCardStack()
        {
            activeStack = null;
        }

    }
}