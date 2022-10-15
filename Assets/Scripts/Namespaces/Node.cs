
using UnityEngine;
using System.Collections.Generic;
using TMPro;
namespace Core
{
    public class Node : MonoBehaviour, Stackable
    {
        enum NodeStateTypes
        {
            low,
            medium,
        };


        private TextMeshPro titleTextMesh;
        private TextMeshPro availableInventoryTextMesh;
        private TextMeshPro hungerTextMesh;

        public bool isActive;

        public string title;


        [System.NonSerialized]
        private CardStack activeStack;

        private NodePlaneManagers nodePlaneManagers;


        // --------------------STATS-------------------------

        private int _inventoryLimit;
        public int inventoryLimit
        {
            get { return _inventoryLimit; }
            set { _inventoryLimit = value; }
        }

        private int _currentAvailableInventory;
        public int currentAvailableInventory
        {
            get { return _currentAvailableInventory; }
            set { _currentAvailableInventory = value; }
        }

        private int _currentHungerCheck;
        public int currentHungerCheck
        {
            get { return _currentHungerCheck; }
            set { _currentHungerCheck = value; }
        }

        private int _hungerSetIntervalTimer; // sec, next time the check is applied 
        public int hungerSetIntervalTimer
        {
            get { return _hungerSetIntervalTimer; }
            set { _hungerSetIntervalTimer = value; }
        }

        private float intervalTimer;


        // --------------------INTERVAL CHECK-------------------------
        NodeStateTypes nodeState;

        private void initlizeBaseStats()
        {
            _inventoryLimit = 10;
            _currentAvailableInventory = 10;
            _hungerSetIntervalTimer = 60;
            _currentHungerCheck = 1;
        }

        private void Awake()
        {
            nodeState = NodeStateTypes.low;
            isActive = false;
            activeStack = new CardStack(CardStackType.Nodes);

            Component[] textMeshes = gameObject.GetComponentsInChildren(typeof(TextMeshPro));
            titleTextMesh = textMeshes[0] as TextMeshPro;
            availableInventoryTextMesh = textMeshes[1] as TextMeshPro;
            hungerTextMesh = textMeshes[2] as TextMeshPro;
            initlizeBaseStats();
        }

        public void init()
        {
            reflectToScreen();
            nodePlaneManagers = gameObject.GetComponent(typeof(NodePlaneManagers)) as NodePlaneManagers;
        }

        private void handleHungerInterval()
        {

        }

        public void stackOnThis(List<Card> newCards)
        {
            bool isRootCardChanged = activeStack.cards.Count == 0 ? true  : false;
            activeStack.addCardsToStack(newCards);
            if(isActive == false){
                activeStack.changeActiveStateOfAllCards(false);
            } 
            if(isRootCardChanged){
                nodePlaneManagers.notifyCardsChanged(isRootCardChanged);
            }
        
            computeStatsFormCards();
        }

        private void computeStatsFormCards(){

        }

        private void reflectToScreen()
        {
            titleTextMesh.text = title;
            availableInventoryTextMesh.text = "" + _inventoryLimit + "/" + _currentAvailableInventory;
            hungerTextMesh.text = "" + _currentHungerCheck;
        }

        private void Update()
        {
            intervalTimer = intervalTimer + Time.deltaTime;
            if (intervalTimer > hungerSetIntervalTimer)
            {
                handleHungerInterval();
                intervalTimer = 0;
            }
        }


        public void clearCardStack()
        {
            activeStack = null;
        }

        public CardStack getCardStack(){
            return activeStack;
        }

    }
}