
using UnityEngine;
using System.Collections.Generic;
namespace Core
{
    public interface Stackable
    {
        void stackOnThis(List<Card> draggingCards);
    }

    public interface IClickable
    {
        void OnClick();
    }
    public enum CoreInteractableType
    {
        Cards,
        Nodes
    }
    public enum CardStackType
    {
        Cards,
        Nodes
    }

    enum NodeStateTypes
    {
        low,
        medium,
    };

}