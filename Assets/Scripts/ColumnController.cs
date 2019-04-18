using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColumnController : MonoBehaviour {

    int columnNumber;
    public CardController cardPrefab;
    public CardController card;
    float padding;
    public SideController side;


    // Use this for initialization
    void Awake () {
        padding = 0f;// 0.05f;
        card = CreateNewCard();
        card.name = " Card " + Random.Range(0f, 100f); 
        //Debug.Log("Our card's position is " + card.transform.position);
    }

    public SideController GetSide()
    {
        return side;
    }

    public void SetSide(SideController newSide)
    {
        side = newSide;
        transform.parent = newSide.transform;
    }

    private CardController CreateNewCard()
    {
        CardController tempCard = Instantiate(cardPrefab,
                                              new Vector3(0f, 0f, 0f),
                                              Quaternion.identity,
                                              transform);
        tempCard.SetColumn(this);
        return tempCard;
    }

    public CardController GetCard()
    {
        return card;
    }

    public void SetCard(CardController newCard)
    {
        card = newCard;
    }

    public float GetWidth()
    {
        if (card)
        {
            return card.GetWidth();
        }
        else
        {
            return 0f;
        }
        
    }

    public float GetHeight()
    {
        //Debug.Log(name + " getting height ");
        if (card)
        {
            //Debug.Log(name + " returning " + card.GetHeight());
            return card.GetHeight();
        }
        else
        {
            //Debug.Log(name + " no card. Returning 0f");
            return 0f;
        }
    }

    public Vector3 Position()
    {
        return card.transform.localPosition;
    }

    public void MergeCards(CardController left, CardController right, List<CardController> cards, List<TileController> commonTiles)
    {
        //Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!111111!!!!!!!!!!!!!!!!!!!!!!!!!11Column = " + name + " starting MergeCards");
        CardController newLeft = CreateNewCard();
        //Debug.Log("newLeft = " + newLeft.name);
        if (right.GetColumn())
        {
            //Debug.Log(name + " MergeCards: Now to remove " + right.GetColumn());
            right.GetColumn().GetSide().RemoveColumn(right.GetColumn(), true);
            //Debug.Log(name + " Done");
        }

        newLeft.AddChildCard(left);
        newLeft.AddChildCard(right);
        newLeft.SetTiles(commonTiles);
        
        right.DistributeTiles();
        right.ResizeY();
        left.ResizeX();
        left.ResizeY();
        left.DistributeTiles();

        newLeft.DistributeTiles();
        newLeft.DistributeCards();
        newLeft.ResizeY();
        newLeft.ResizeX();

        SetCard(newLeft);
        //left.GetColumn().GetSide().DistributeColumns();
    }

    public void PositionRight(ColumnController otherColumn)
    {
        //Debug.Log("PositionRight: left column = " + otherColumn.transform.position);
        transform.Translate(otherColumn.card.GetRight() - card.GetLeft(), 0f, 0f);
        //Debug.Log("PositionRight: this column = " + transform.position);
    }

    public void PositionRight(CardController otherCard)
    {
        //Debug.Log("PositionRight: left column = " + otherColumn.transform.position);
        transform.Translate(otherCard.GetRight() - card.GetLeft(), 0f, 0f);
        //Debug.Log("PositionRight: this column = " + transform.position);
    }

    public void RemoveCard()
    {
        //Debug.Log(name + " asking our side to remove us");
        side.RemoveColumn(this, true);
    }


    public float GetBottom()
    {
        if (card)
        {
            return card.GetBottom();
        }
        else
        {
            return 0f;
        }
    }

    public float GetTop()
    {
        if (card)
        {
            return card.GetTop();
        }
        else
        {
            return 0f;
        }
    }

    public void FreeChildCards()
    {
        List<CardController> cards = card.GetChildCards();
        card.FreeChildren();
        Destroy(card.gameObject);
        SetCard(cards[0]);
        cards[0].SetColumn(this);
        cards[0].transform.localPosition = Vector3.zero;
        //Debug.Log("cards[0] = " + cards[0].name + "; column = " + cards[0].GetColumn());
        for (int i = 1; i < cards.Count; i++)
        {
            side.InsertCardAfter(cards[i-1].GetColumn(), cards[i]);
        }
        side.DistributeColumns();
    }
}
