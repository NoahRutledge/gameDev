using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CBState
{
    toDrawpile,
    drawpile,
    toHand,
    hand,
    toTarget,
    target,
    discard,
    to,
    idle
}

public class CardBartok : Card {

    static public float MOVE_DURATION = 0.5f;
    static public string MOVE_EASING = Easing.InOut;
    static public float CARD_HEIGHT = 3.5f;
    static public float CARD_WIDTH = 2f;

    [Header("Set Dynamically: CardBartok")]
    public CBState state = CBState.drawpile;
    //moving and rotation
    public List<Vector3> bezierPts;
    public List<Quaternion> bezierRots;
    public float timeStart, timeDuration;
    public int eventualSortOrder;
    public string eventualSortLayer;

    //When the card is done moving, it will call reportFinishTo.SendMessage()
    public GameObject reportFinishTo = null;
    [System.NonSerialized]
    public Player callbackPlayer = null;

    //MoveTo tells the card to interpolate to a new position and rotation
    public void MoveTo(Vector3 ePos, Quaternion eRot)
    {
        bezierPts = new List<Vector3>();
        bezierPts.Add(transform.localPosition);
        bezierPts.Add(ePos);

        bezierRots = new List<Quaternion>();
        bezierRots.Add(transform.rotation);
        bezierRots.Add(eRot);

        if(timeStart == 0)
        {
            timeStart = Time.time;
        }

        timeDuration = MOVE_DURATION;

        state = CBState.to;
    }

    public void MoveTo(Vector3 ePos)
    {
        MoveTo(ePos, Quaternion.identity);
    }
    
	// Update is called once per frame
	void Update () {
        switch (state)
        {
            case CBState.toHand:
            case CBState.toTarget:
            case CBState.toDrawpile:
            case CBState.to:
                float u = (Time.time - timeStart) / timeDuration;
                float uC = Easing.Ease(u, MOVE_EASING);

                if(u < 0)
                {
                    transform.localPosition = bezierPts[0];
                    transform.rotation = bezierRots[0];
                    return;
                }
                else if(u >= 1)
                {
                    uC = 1;
                    //move from the "to" state to the proper state
                    if (state == CBState.toHand) state = CBState.hand;
                    if (state == CBState.toDrawpile) state = CBState.drawpile;
                    if (state == CBState.toTarget) state = CBState.target;
                    if (state == CBState.to) state = CBState.idle;

                    //move to final position
                    transform.localPosition = bezierPts[bezierPts.Count - 1];
                    transform.rotation = bezierRots[bezierRots.Count - 1];

                    //reset timeStart to 0 so it gets rewritten next time it moves
                    timeStart = 0;

                    if(reportFinishTo != null)
                    {
                        reportFinishTo.SendMessage("CBCallback", this);
                        reportFinishTo = null;
                    }
                    else if(callbackPlayer != null)
                    {
                        //if there is a callback player
                        //call CBCallback directly on this player
                        callbackPlayer.CBCallback(this);
                        callbackPlayer = null;
                    }
                    else
                    {
                        //if nothing to callback then just stay still
                    }
                }
                else//if u is not at 1 or less than 0 then just do normal interpolate behavior
                {
                    Vector3 pos = Utils.Bezier(uC, bezierPts);
                    transform.localPosition = pos;
                    Quaternion rotQ = Utils.Bezier(uC, bezierRots);
                    transform.rotation = rotQ;

                    if(u > 0.5f)
                    {
                        SpriteRenderer sRend = spriteRenderers[0];
                        if(sRend.sortingOrder != eventualSortOrder)
                        {
                            SetSortOrder(eventualSortOrder);
                        }
                        if(sRend.sortingLayerName != eventualSortLayer)
                        {
                            SetSortingLayerName(eventualSortLayer);
                        }
                    }
                }
                break;
        }
	}

    public override void OnMouseUpAsButton()
    {
        //call the cardclicked method on the bartok singleton
        Bartok.S.CardClicked(this);
        //also call the base class version of this method
        base.OnMouseUpAsButton();
    }
}
