
using UnityEngine;
using System.Collections.Generic;
namespace Core
{

    public interface Stackable
    {
        void stackOnThis(List<Card> draggingCards);
    }

    public interface Dragable
    {
        // void stackOnThis(List<Card> draggingCards);
    }
}