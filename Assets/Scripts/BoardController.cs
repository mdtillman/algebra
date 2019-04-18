using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour {

    public SideController left, right;
    public ParticleSystem divider;

	// Use this for initialization
	void Awake () {
        left = Instantiate(left, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);
        left.name = "left";
        right = Instantiate(right, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);
        right.name = "right";
        divider.Play();
    }
	
	public SideController GetSide(int side)
    {
        switch (side)
        {
            case 0:
                return left;
            case 1:
                return right;
            default:
                return left;
        }
    }

    public void SetSide(SideController side, int index)
    {
        switch (index)
        {
            case 0:
                left = side;
                break;
            case 1:
                right = side;
                break;
        }
    }

    public float GetWidth()
    {
        return left.GetWidth() > right.GetWidth() ? left.GetWidth() : right.GetWidth();
    }

    public void DistributeSides()
    {
        //Debug.Log("Left width = " + left.GetWidth());
        left.transform.localPosition = new Vector3(divider.transform.localPosition.x,
                                                    divider.transform.localPosition.y,
                                                    0f);
        right.transform.localPosition = new Vector3(divider.transform.localPosition.x,
                                                    divider.transform.localPosition.y,
                                                    0f);
        left.DistributeColumns();
        right.DistributeColumns();
    }
}
