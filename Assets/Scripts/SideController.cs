using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideController : MonoBehaviour {

    public List<ColumnController> columns;
    public ColumnController columnPrefab;

    // Use this for initialization
    void Awake()
    {
        columns = new List<ColumnController>();
    }

    public ColumnController GetColumn(int column)
    {
        if(column < columns.Count)
        {
            return columns[column];
        } else
        {
            return null;
        }
    }

    public ColumnController GetCurrentColumn()
    {
        //Debug.Log("GetCurrentColumn: columns currently at " + columns.Count);
        if (columns.Count == 0)
        {
            AddColumn();
        }
        return columns[columns.Count - 1];
    }

    public int NumColumns()
    {
        return columns.Count;
    }

    public void InsertCardAfter(ColumnController leftColumn, CardController newCard)
    {
        int index = columns.IndexOf(leftColumn);
        //Debug.Log("index = " + index + " while columns are " + columns.Count);
        if (((index + 1) >= columns.Count) ||
            ((index + 1) < columns.Count 
              && columns[index + 1].card))
        {
            //Debug.Log("Need to add a column");
            ColumnController newColumn = Instantiate(columnPrefab);
            columns.Insert(index + 1, newColumn);
            newColumn.transform.parent = transform;
            newColumn.SetSide(this);
            newColumn.transform.localPosition = Vector3.zero;
            Destroy(columns[index + 1].card.gameObject);
            columns[index + 1].SetCard(newCard);
            newCard.SetColumn(columns[index + 1]);
            newCard.transform.localPosition = Vector3.zero;
            newCard.GetComponent<Collider2D>().enabled = true;
        }
        else if((index + 1) < columns.Count)
        {
            //Debug.Log("Don't need to add column");
            columns[index + 1].SetCard(newCard);
            newCard.transform.localPosition = Vector3.zero;
            newCard.GetComponent<Collider2D>().enabled = true;
        }
        else
        {
            //Debug.Log("Neither");
        }
    }

    public void AddColumn()
    {
        //Debug.Log("AddColumn: start");
        columns.Add(Instantiate(columnPrefab, new Vector3(0f, 0f, 0f), 
                    Quaternion.identity, transform));
        columns[columns.Count - 1].transform.localScale = new Vector3(1f, 1f, 1f);
        columns[columns.Count - 1].GetCard().name = columns[columns.Count - 1].GetCard().name
                                                    + columns.Count;
        columns[columns.Count - 1].name = name + " column " + columns.Count;
        columns[columns.Count - 1].SetSide(this);
        DistributeColumns();
    }

    public void AddColumn(ColumnController newColumn, bool toDestroy)
    {
        //Debug.Log("AddColumn: start");
        newColumn.GetSide().RemoveColumn(newColumn, toDestroy);
        columns.Add(newColumn);
        columns[columns.Count - 1].transform.localScale = new Vector3(1f, 1f, 1f);
        columns[columns.Count - 1].GetCard().name = columns[columns.Count - 1].GetCard().name
                                                    + columns.Count;
        columns[columns.Count - 1].SetSide(this);
        DistributeColumns();
    }


    public void MoveColumn(ColumnController column, int index)
    {
        if (index < columns.Count)
        {
            columns.Remove(column);
            columns.Insert(index, column);
        }
        DistributeColumns();
    }

    public void RemoveColumn(ColumnController column, bool toDestroy)
    {
        //Debug.Log("Side " + name + " removing " + column.name + " toDestroy = " + toDestroy);
        columns.Remove(column);
        if (toDestroy)
        {
            Destroy(column.gameObject);
        }
        DistributeColumns();
    }

    public void RemoveCard(CardController card, bool toDestroy)
    {
        //Debug.Log("Side " + name + " removing " + column.name + " toDestroy = " + toDestroy);
        if (card.GetColumn().GetCard().Equals(card))
        {
            RemoveColumn(card.GetColumn(), toDestroy);
        }
        else
        {
            CardController parentCard = card.GetParentCard();

            if (parentCard)
            {
                parentCard.RemoveChildCard(card);
            }
        }
        
    }

    public float GetWidth()
    {
        //Debug.Log("Side: " + name + " GetWidth: Start");
        if (columns.Count == 0)
        {
            return 0f;
        }
        else
        {
            float width = 0f;
            for (int i = 0; i < columns.Count; i++)
            {
                //Debug.Log("Side " + name + "; i = " + i + " getting column " + columns[i].name + " width = " + columns[i].GetWidth());
                width += columns[i].GetWidth();
            }

            //Debug.Log("Side " + name + " width = " + width);
            return width;
        }
    }

    public float GetHeight()
    {
        if (columns.Count == 0)
        {
            return 0f;
        }
        else
        {
            float maxColumnHeight = 0f;
            float current = 0f;
            foreach (ColumnController column in columns)
            {
                //Debug.Log(name + " maxColumnHeight = " + maxColumnHeight);
                current = column.GetHeight();
                //Debug.Log(name + " checking height of " + column + " = " + current);
                if (current > maxColumnHeight)
                {
                    maxColumnHeight = current;
                }
            }

            //Debug.Log(name + " returning " + maxColumnHeight);
            return maxColumnHeight;
        }
    }

    public float GetBottom()
    {
        return columns[0].GetBottom();
    }

    public void DistributeColumns()
    {
        float sideSign = 1f;
        float width = GetWidth();
        //Debug.Log(name + " side width = " + width);
        if (name.Equals("right"))
        {
            sideSign = -1;
        }

        for (int i = 0; i < columns.Count; i++)
        {
            if (i == 0)
            {
                columns[i].transform.localPosition = new Vector3((-width/2f) + (columns[i].GetWidth() / 2f),
                                                                 0f, 
                                                                 0f);
                columns[i].transform.Translate(0f, 
                                               (sideSign * columns[i].GetHeight()) / 2f, 
                                               0f);
            }
            else
            {
                columns[i].PositionRight(columns[i - 1]);
                if(sideSign > 0)
                {
                    columns[i].transform.Translate(0f, 0f - columns[i].GetBottom(), 0f);
                }
                else
                {
                    columns[i].transform.Translate(0f, 0f - columns[i].GetTop(), 0f);
                }
            }
        }
    }
}
