using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour {

    Vector3 mouseOffset, home;
    int column;
    public int number;
    public Collider2D tileBelow;
    public bool moving;
    public ParticleSystem particles;
    public CardController card;
    public float height;
    float padding;

	// Use this for initialization
	void Awake () {
        moving = false;
        home = new Vector3(0f, 0f, 0f);
        padding = 0f;//0.02f;
        particles.Stop();
    }

    public SideController GetSide()
    {
        return card.GetSide();
    }

    public void SetCard(CardController newCard)
    {
        card = newCard;
    }

    public CardController GetCard()
    {
        return card;
    }

    public void SetColumn(int val)
    {
        column = val;
    }

    public int GetColumn()
    {
        return column;
    }

    public string GetColliderStates()
    {
        string result = "";

        result += GetComponent<Collider2D>().name + " is ";
        result += GetComponent<Collider2D>().enabled + "; ";

        foreach(Transform t in transform)
        {
            result += t.GetComponent<Collider2D>().name + " is ";
            result += t.GetComponent<Collider2D>().enabled + "; ";
        }

        return result;
    }

    public void PositionAbove(TileController tile)
    {
        transform.position = tile.transform.position;
        transform.Translate(0f, tile.GetHeight() + padding, 0f);
    }

    public void GoToBottom(CardController container)
    {
        Debug.Log("GoToBottom: tile " + name + " is going to the bottom of " + container.name);
        Debug.Log("GoToBottom: tile " + name + "'s bottom is " + GetBottom() + " while its card's bottom is " + container.GetBottom());
        transform.Translate(0f, container.GetBottom() - GetBottom(), 0f);
        Debug.Log("GoToBottom: NOW tile " + name + "'s bottom is " + GetBottom() + " while its card's bottom is " + container.GetBottom());
    }

    public void GoToTop(CardController container)
    {
        Debug.Log("GoToTop: tile " + name + " is going to the top of " + container.name);
        Debug.Log("GoToTop: tile " + name + "'s card's top is " + container.GetTop() + "while its top is " + GetTop());
        transform.Translate(0f, container.GetTop() - GetTop(), 0f);
        Debug.Log("GoToTop: NOW tile " + name + "'s card's top is " + container.GetTop() + "while its top is " + GetTop());
    }

    public void PositionBelow(TileController tile)
    {
        transform.position = tile.transform.position;
        transform.Translate(0f, -(tile.GetHeight() + padding), 0f);
    }

    public float GetHeight()
    {
        float result = GetComponent<SpriteRenderer>().bounds.size.y;
        float pixelsPerUnit = GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
        result *= pixelsPerUnit / 100;
        //Debug.Log(name + " height = " + result);
        height = result;
        return result;
    }

    public float GetWidth()
    {
        Debug.Log(name + " GetWidth: Start");
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
}
