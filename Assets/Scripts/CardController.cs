using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    ColumnController column;
    CardController parentCard;
    List<CardController> childCards;
    List<TileController> tiles;
    List<Transform> allChildren;
    bool resizing;
    public bool moving;
    public GameObject one;
    public Sprite positive, negative;
    public float height;
    int sign;
    float tilePadding, topPadding;

    private void Awake()
    {
        childCards = new List<CardController>();
        tiles = new List<TileController>();
        allChildren = new List<Transform>();
        resizing = false;
        moving = false;
        tilePadding = 0f;// 0.05f;
        topPadding = 0.05f;
        SetSign(1);
    }

    public int GetSign()
    {
        return sign;
    }

    public void SetSign(int newSign)
    {
        if(newSign >= 0)
        {
            sign = 1;
        }
        else
        {
            sign = -1;
        }
    }

    public void FlipSign()
    {
        if(sign == -1)
        {
            GetComponent<SpriteRenderer>().sprite = positive;
            sign = 1;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = negative;
            sign = -1;
        }
        DistributeTiles();
    }

    public void SetParentCard(CardController card)
    {
        parentCard = card;
        column = parentCard.GetColumn();
        transform.parent = card.transform;
    }

    public ColumnController GetColumn()
    {
        return column;
    }

    public void SetColumn(ColumnController newColumn)
    {
        column = newColumn;
        transform.parent = newColumn.transform;
    }

    public CardController GetParentCard()
    {
        return parentCard;
    }

    public List<CardController> GetChildCards()
    {
        return childCards;
    }

    public CardController GetChildCard(int index)
    {
        if (index < childCards.Count)
        {
            return childCards[index];
        }
        else
        {
            return null;
        }
    }

    public void MoveChildCard(CardController card, int index)
    {
        if (index < childCards.Count)
        {
            childCards.Remove(card);
            childCards.Insert(index, card);
        }
    }

    public void RemoveChildCard(CardController card)
    {
        childCards.Remove(card);
        DistributeCards();
    }

    public void AddChildCard(CardController card)
    {
        //Debug.Log(name + " adding " + card);
        childCards.Add(card);
        card.transform.parent = this.transform;
        if (!card.GetColumn().Equals(column))
        {
            card.GetColumn().RemoveCard();
        }
        card.parentCard = this;
        ResizeX();
        ResizeY();
        DistributeCards();
    }

    public List<TileController> GetTiles()
    {
        return tiles;
    }

    public void SetTiles(List<TileController> otherTiles)
    {
        foreach(TileController newTile in otherTiles)
        {
            AddTile(newTile);
        }
    }

    public void ClearTiles()
    {
        tiles = new List<TileController>();
    }

    public TileController GetTile(int index)
    {
        if (index < tiles.Count)
        {
            return tiles[index];
        }
        else
        {
            return null;
        }
    }

    public void MergeWith(CardController otherCard, List<CardController> cards, List<TileController> commonTiles)
    {
        //Debug.Log("?????????????????????????????? MergeWith: Card = " + this);
        //Debug.Log("starting merge with " + otherCard);
        //Debug.Log("and commonTiles = " + commonTiles);
        column.MergeCards(this, otherCard, cards, commonTiles);
    }

    public float GetHeight()
    {
        //Debug.Log(name + " GetHeight: Start");
        float result = GetComponent<SpriteRenderer>().bounds.size.y;
        //Debug.Log(name + " size = " + result);
        float pixelsPerUnit = GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
        //Debug.Log(name + " pixelsPerUnit = " + pixelsPerUnit);
        result *= pixelsPerUnit / 100;
        //Debug.Log(name + " height = " + result);
        height = result;
        return result;
    }

    public float GetWidth()
    {
        float result = GetComponent<SpriteRenderer>().bounds.size.x;
        float pixelsPerUnit = GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
        result *= pixelsPerUnit / 100;
        return result;
    }

    public float GetLeft()
    {
        float result = GetComponent<SpriteRenderer>().bounds.extents.x;
        float pixelsPerUnit = GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
        result *= pixelsPerUnit / 100;
        result = transform.position.x - result;
        //Debug.DrawRay(new Vector3(result, transform.position.y, transform.position.z), Vector3.up, Color.red, 100f);
        return result;
    }

    public float GetRight()
    {
        float result = GetComponent<SpriteRenderer>().bounds.extents.x;
        float pixelsPerUnit = GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
        result *= pixelsPerUnit / 100;
        result = transform.position.x + result;
        //Debug.DrawRay(new Vector3(result, transform.position.y, transform.position.z), Vector3.up, Color.red, 100f);
        return result;
    }

    public float GetTop()
    {
        float result = GetComponent<SpriteRenderer>().bounds.extents.y;
        float pixelsPerUnit = GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
        result *= pixelsPerUnit / 100;
        result = transform.position.y + result;
        //Debug.DrawRay(new Vector3(transform.position.x, result, transform.position.z), Vector3.right, Color.red, 100f);
        return result;
    }

    public float GetBottom()
    {
        float result = GetComponent<SpriteRenderer>().bounds.extents.y;
        float pixelsPerUnit = GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
        result *= pixelsPerUnit / 100;
        result = transform.position.y - result;
        Debug.DrawRay(new Vector3(transform.position.x, result, transform.position.z), Vector3.left, Color.red, 100f);
        return result;
    }

    public void AddTile(TileController tile)
    {
        //Debug.Log("CardController: We have been asked to add tile " + tile + " with " + tiles.Count + " tiles so far");
        tiles.Add(tile);
        tile.transform.parent = transform;
        tile.transform.localPosition = GetBottomTilePosition(tile);
        if(tiles.Count > 1)
        {
            tile.transform.localScale = tiles[tiles.Count - 2].transform.localScale;
        }
        //Debug.Log("CardController: We have been asked to add tile " + tile + " " + tile.transform.localScale);

        tile.SetCard(this);
        SortTiles();
    }

    Vector3 GetBottomTilePosition(TileController tile)
    {
        float yPos = -GetHeight() + tile.GetHeight() + tilePadding;
        
        //Debug.Log(name + " yPos = " + yPos);
        return new Vector3(0f, yPos, -0.1f);
    }

    Vector3 GetTopTilePosition(TileController tile)
    {
        float yPos = GetHeight() - tile.GetHeight() - tilePadding;
        //Debug.Log("Height = " + GetHeight() + "; tile height = " + tile.GetHeight() + "; yPos = " + yPos);
        return new Vector3(0f, yPos, -0.1f);
    }

    public void MoveTile(TileController tile, int index)
    {
        if (index < tiles.Count)
        {
            tiles.Remove(tile);
            tiles.Insert(index, tile);
        }
    }

    public SideController GetSide()
    {
        if(parentCard != null)
        {
            return parentCard.GetSide();
        }
        else
        {
            return column.GetSide();
        }
    }

    public void RemoveTile(TileController tile, bool destroy)
    {
        //Debug.Log(name + " is removing tile " + tile + " w/ destroy = " + destroy);
        tiles.Remove(tile);
        if (destroy)
        {
            Destroy(tile.gameObject);
        }
        //Debug.Log("Now to resize Y");
        ResizeY();
        //Debug.Log("And to distribute tiles");
        DistributeTiles();
    }

    public void RemoveTiles()
    {
        Debug.Log("RemoveTiles");
        foreach (TileController t in tiles)
        {
            Debug.Log("RemoveTiles: destroying " + t.name);
            Destroy(t.gameObject);
        }
        tiles.Clear();
    }

    float GetTallestCardHeight()
    {
        //Debug.Log("GetTallestCardHeight: Start");
        float maxChildCardHeight = 0f;
        foreach (CardController card in childCards)
        {
            //Debug.Log("Checking " + card + " whose height is " + card.GetHeight());
            float current = card.GetHeight();
            if (current > maxChildCardHeight)
            {
                maxChildCardHeight = current;
            }
        }

        return maxChildCardHeight;
    }



    public void DistributeCards()
    {
        //Debug.Log("DistributeCards: childCards.Count = " + childCards.Count);
        for (int i = 0; i < childCards.Count; i++)
        {
            if (i == 0)
            {
                childCards[i].GoToLeft(this);
                if(tiles.Count > 0)
                {
                    if (GetSide().name.Equals("left"))
                    {
                        childCards[i].PositionAbove(tiles[tiles.Count - 1]);
                    }
                    else
                    {
                        childCards[i].PositionBelow(tiles[tiles.Count - 1]);
                    }
                }
                else
                {
                    if (GetSide().name.Equals("left"))
                    {
                        childCards[i].GoToBottom(this);
                    }
                    else
                    {
                        childCards[i].GoToTop(this);
                    }
                }

            }
            else
            {
                //Debug.Log("DistributeCards:" + childCards[i] + ": " + childCards[i].GetBottom() + " vs. " + childCards[0].GetBottom());
                childCards[i].PositionRight(childCards[i - 1]);
                if (GetSide().name.Equals("left"))
                {
                    childCards[i].transform.Translate(0f, childCards[i - 1].GetBottom() - childCards[i].GetBottom(), 0f);
                }
                else
                {
                    childCards[i].transform.Translate(0f, childCards[i - 1].GetTop() - childCards[i].GetTop(), 0f);
                }
            }
            childCards[i].DistributeTiles();
            childCards[i].DistributeCards();
        }
    }

    public void PositionCardsAbove()
    {
        if(childCards.Count > 0)
        {
            if(tiles.Count > 0)
            {
                childCards[0].PositionAbove(tiles[tiles.Count - 1]);
            }
            else
            {
                childCards[0].GoToBottom(this);
            }
            for (int i = 1; i < childCards.Count; i++)
            {
                childCards[i].transform.position = new Vector3(
                    childCards[i].transform.position.x,
                    childCards[0].transform.position.y,
                    childCards[i].transform.position.z);
            }
        }
    }

    public void GoToBottom(CardController outer)
    {
        transform.Translate(0f,
                            outer.GetBottom() - GetBottom(),
                            0f);
    }

    public void GoToTop(CardController outer)
    {
        transform.Translate(0f,
                            outer.GetTop() - GetTop(),
                            0f);
    }


    public void PositionAbove(TileController tile)
    {
        transform.Translate(0f, tile.GetTop() - GetBottom(), 0f);
    }

    public void PositionBelow(TileController tile)
    {
        transform.Translate(0f, tile.GetBottom() - GetTop(), 0f);
    }

    public void PositionRight(CardController cardLeft)
    {
        transform.localPosition = new Vector3(cardLeft.transform.localPosition.x,
                                              transform.localPosition.y,
                                              transform.localPosition.z);
        transform.Translate(cardLeft.GetRight() - GetLeft(), 0f, cardLeft.transform.localPosition.z);
    }

    public void GoToLeft(CardController parentCard)
    {
        transform.Translate((parentCard.GetLeft() + tilePadding) - GetLeft(), 0f, -0.1f);
    }

    public void AdjustCardSizeAndContents()
    {
        //Debug.Log("Distributing tiles");
        DistributeTiles();
        //Debug.Log("Position cards");
        PositionCardsAbove();
        DistributeCards();
        ResizeX();
        //Debug.Log("Resize Y");
        ResizeY();
    }

    public void DoubleTileWidths()
    {
        foreach (TileController t in tiles)
        {
            t.transform.localScale = new Vector3(t.transform.localScale.x * 2f,
                                                 t.transform.localScale.y,
                                                 t.transform.localScale.z);
        }
    }

    void SortTiles()
    {
        //Debug.Log("SortTiles");
        tiles.Sort((x, y) => x.number.CompareTo(y.number));
        //Debug.Log("SortTiles: Done");
    }

    public void DistributeTiles()
    {
        Debug.Log(name + " DistributeTiles: Start. tiles.Count = " + tiles.Count);
        SortTiles();
        float factor = 0f;

        for (int i = 0; i < tiles.Count; i++)
        {
            factor = GetWidth() / tiles[i].GetWidth();

            tiles[i].transform.localScale = new Vector3(tiles[i].transform.localScale.x * factor,
                                                        tiles[i].transform.localScale.y,
                                                        tiles[i].transform.localScale.z);

            if (i == 0)
            {
                if (GetSide().name.Equals("left"))
                {
                    Debug.Log(name + " DistributeTiles: left!");
                    tiles[i].GoToBottom(this);
                }
                else
                {
                    Debug.Log(name + " DistributeTiles: right!");
                    tiles[i].GoToTop(this);
                }

                //Debug.Log("DistributeTiles: it is now at " + tiles[i].transform.localPosition.x + ", " + tiles[i].transform.localPosition.y + ", " + tiles[i].transform.localPosition.z);
                //Debug.Log("DistributeTiles: or " + tiles[i].transform.position.x + ", " + tiles[i].transform.position.y + ", " + tiles[i].transform.position.z);
            }
            else
            {
                //Debug.Log("We now have to distribute tile " + i + " which is " + tiles[i]);
                tiles[i].transform.localPosition = tiles[i - 1].transform.localPosition;
                if (GetSide().name.Equals("left"))
                {
                    Debug.Log(name + " DistributeTiles: left again!");
                    tiles[i].PositionAbove(tiles[i - 1]);
                }
                else
                {
                    Debug.Log(name + " DistributeTiles: right again!");
                    tiles[i].PositionBelow(tiles[i - 1]);
                }
            }
        }
    }

    public void ResizeX()
    {
        //Debug.Log(" " + name + " !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!ResizeX: raycasting check start now:");
        FreeChildren();

        float newX = transform.position.x;
        float newWidth = 0f;

        if (childCards.Count > 1)
        {
            newX = (childCards[0].GetLeft() + childCards[childCards.Count - 1].GetRight()) / 2f;
            //Debug.Log("So, the middle is: " + newLocation);
            foreach (CardController c in childCards)
            {
                newWidth += c.GetWidth();
            }
        }
        else if(tiles.Count > 0)
        {
            newWidth = GetWidestTileWidth();
        }
        else
        {
            newWidth = GetWidth();
        }

        transform.Translate(newX - transform.position.x, 0f, 0f);
        
        float factor = newWidth / GetWidth();

        transform.localScale = new Vector3(transform.localScale.x * factor,
                                           transform.localScale.y,
                                           transform.localScale.z);

        RestoreChildren();

    }

    private bool Contains(TileController t)
    {
        foreach(TileController t2 in tiles)
        {
            if(t.number == t2.number)
            {
                return true;
            }
        }
        return false;
    }

    public bool AllChildCardsContain(TileController t)
    {
        foreach(CardController c in childCards)
        {
            if (!c.Contains(t))
            {
                return false;
            }
        }

        return true;
    }

    public void ResizeTiles()
    {
        float cardWidth = GetWidth();
        foreach (TileController t in tiles)
        {
            t.transform.localScale = new Vector3((cardWidth / t.GetWidth()) * t.transform.localScale.x,
                                                 t.transform.localScale.y,
                                                 t.transform.localScale.z);
            t.transform.Translate(GetLeft() - t.GetLeft(), 0f, 0f);
        }
    }

    float GetWidestTileWidth()
    {
        //Debug.Log("GetTallestCardHeight: Start");
        float maxTileWidth = 0f;
        foreach (TileController t in tiles)
        {
            //Debug.Log("Checking " + card + " whose height is " + card.GetHeight());
            float current = t.GetWidth();
            if (current > maxTileWidth)
            {
                maxTileWidth = current;
            }
        }

        return maxTileWidth;
    }

    CardController GetTallestCard()
    {
        float maxChildCardHeight = 0f;
        CardController maxCard = null;
        foreach (CardController card in childCards)
        {
            //Debug.Log("Checking " + card + " whose height is " + card.GetHeight());
            if (card.GetHeight() > maxChildCardHeight)
            {
                maxCard = card;
                maxChildCardHeight = card.GetHeight();
            }
        }

        //Debug.Log(name + " GetTallestCard: returning " + maxCard);
        return maxCard;
    }


    public void ResizeY()
    {
        Debug.Log(name + " ResizeY: Start");
    
        FreeChildren();
        //Debug.Log(name + " ResizeY: Children free");
        float newHeight = GetHeight();
        Debug.Log("Checking card count ");
        Debug.Log("It equals " + childCards.Count);
        Debug.Log("And tiles are " + tiles.Count);
        if ((childCards.Count > 0) && (tiles.Count > 0))
        {
            if (GetSide().name.Equals("left"))
            {
                newHeight = (GetTallestCard().GetTop() + 4 * topPadding) - (tiles[0].GetBottom() - topPadding);
            }
            else
            {
                newHeight = (tiles[0].GetTop() + topPadding) - (GetTallestCard().GetBottom() - 4 * topPadding);
            }
        }
        else if (tiles.Count > 0)
        {
            Debug.Log(name + " ResizeY: We have tiles");
            if (GetSide().name.Equals("left"))
            {
                newHeight = (tiles[tiles.Count - 1].GetTop() + 4 * topPadding) - (tiles[0].GetBottom() - topPadding);
            }
            else
            {
                newHeight = (tiles[0].GetTop() + topPadding) - (tiles[tiles.Count - 1].GetBottom() - 4 * topPadding);
            }
        }
        else if ((childCards.Count > 0))
        {
            newHeight = GetTallestCard().GetHeight() + 4 * topPadding;
        }

        Debug.Log(name + "ResizeY: Halfway done");
        float factor = newHeight / GetHeight();
        Debug.Log(name + "ResizeY: factor = " + factor);
        transform.localScale = new Vector3(transform.localScale.x,
                                           transform.localScale.y * factor,
                                           transform.localScale.z);
        if (tiles.Count > 0)
        {
            Debug.Log(name + "ResizeY: moving card to align with tiles");
            if (GetSide().name.Equals("left"))
            {
                transform.Translate(0f, (tiles[0].GetBottom() - tilePadding) - GetBottom(), 0f);
            }
            else
            {
                transform.Translate(0f, (tiles[0].GetTop() - tilePadding) - GetTop(), 0f);
            }
        }
        else if (childCards.Count > 0)
        {
            //Debug.Log(name + "ResizeY: moving card to align with childCards");
            if (GetSide().name.Equals("left"))
            {
                transform.Translate(0f, (childCards[0].GetBottom() - tilePadding) - GetBottom(), 0f);
            }
            else
            {
                transform.Translate(0f, (childCards[0].GetTop() - tilePadding) - GetTop(), 0f);
            }
        }
        Debug.Log(name + " Now to restore children");
        RestoreChildren();
        transform.localPosition = new Vector3(0f, 0f, 0f);
    }

    public void FreeChildren()
    {
        //Debug.Log(name + " FreeChildren: start");

        allChildren.Clear();

        //Debug.Log(name + " FreeChildren: the list now has " + allChildren.Count + " elements");

        for (int i = 0; i< transform.childCount; i++)
        {
            //Debug.Log(name + " checking " + transform.GetChild(i));
            allChildren.Add(transform.GetChild(i));
        }

        //Debug.Log(name + " FreeChildren: after gathering:");

        foreach (Transform cTrans in allChildren)
        {
            //Debug.Log(name + " checking transform child " + cTrans.name);
            if (cTrans.GetComponent<TileController>() != null
                || cTrans.GetComponent<CardController>() != null)
            {
                //Debug.Log(name + " It's a tile or Card");
                cTrans.parent = null;
            }
        }
    }

    public int GetValue()
    {
        int temp = GetSign();

        Debug.Log("Sign is " + temp);

        foreach(TileController t in tiles)
        {
            Debug.Log("t.number = " + t.number);
            temp *= t.number;
            Debug.Log("Now the total is " + temp);
        }


        return temp;
    }

    private void RestoreChildren()
    {
        resizing = false;

        foreach (Transform child in allChildren)
        {
            //Debug.Log("Reattaching " + child);
            child.parent = transform;
        }
        allChildren = new List<Transform>();
    }
}
