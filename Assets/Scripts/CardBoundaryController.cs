using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBoundaryController : MonoBehaviour {

    public CardController card;

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log(name + " colliding with " + other.name);
        //card.CollidingWith(name, other.transform);
    }
}
