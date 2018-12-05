using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum PlayerType
{
    human,
    ai
}

[System.Serializable]
public class Player{

    public PlayerType type = PlayerType.ai;
    public int playerNum;
    public SlotDef handSlotDef;
    public List<CardBartok> hand;
    
    public CardBartok AddCard(CardBartok eCB)
    {
        if (hand == null) hand = new List<CardBartok>();

        hand.Add(eCB);

        if(type == PlayerType.human)
        {
            CardBartok[] cards = hand.ToArray();

            cards = cards.OrderBy(cd => cd.rank).ToArray();

            hand = new List<CardBartok>(cards);
        }

        eCB.SetSortingLayerName("10");
        eCB.eventualSortLayer = handSlotDef.layerName;

        FanHand();
        return eCB;
    }

    public CardBartok RemoveCard(CardBartok cb)
    {
        if (hand == null || !hand.Contains(cb)) return null;
        hand.Remove(cb);
        FanHand();
        return cb;
    }

    public void FanHand()
    {
        //startRot is the rotation about z of the first card
        float startRot = 0;
        startRot = handSlotDef.rot;
        if(hand.Count > 1)
        {
            startRot += Bartok.S.handFanDegrees * (hand.Count - 1) / 2;
        }

        //move cards to new positions
        Vector3 pos;
        float rot;
        Quaternion rotQ;
        for(int i = 0; i < hand.Count; i++)
        {
            rot = startRot - Bartok.S.handFanDegrees * i;
            rotQ = Quaternion.Euler(0, 0, rot);

            pos = Vector3.up * CardBartok.CARD_HEIGHT / 2f;

            pos = rotQ * pos;

            //add the base position of the players hand (which will be at the bottom-center of the fan of cards)
            pos += handSlotDef.pos;
            pos.z = -0.5f * i;

            //if not the initial deal, start moving the card immediately
            if(Bartok.S.phase != TurnPhase.idle)
            {
                hand[i].timeStart = 0;
            }

            hand[i].MoveTo(pos, rotQ);
            hand[i].state = CBState.toHand;

            //set the localPosition and rotation of the ith card in the hand
            //hand[i].transform.localPosition = pos;
            //hand[i].transform.rotation = rotQ;
            //hand[i].state = CBState.hand;

            hand[i].faceUp = (type == PlayerType.human);

            hand[i].eventualSortOrder = i * 4;
            //hand[i].SetSortOrder(i * 4);
        }
    }

    public void TakeTurn()
    {
        //dont do anything if it's the human player
        if (type == PlayerType.human) return;

        Bartok.S.phase = TurnPhase.waiting;

        CardBartok cb;

        //find valid plays
        List<CardBartok> validCards = new List<CardBartok>();
        foreach(CardBartok tCB in hand)
        {
            if (Bartok.S.ValidPlay(tCB))
            {
                validCards.Add(tCB);
            }
        }
        //if there are no valid cards
        if(validCards.Count == 0)
        {
            //draw a card
            cb = AddCard(Bartok.S.Draw());
            cb.callbackPlayer = this;
            return;
        }

        //there are cards to play so pick one
        cb = validCards[Random.Range(0, validCards.Count)];
        RemoveCard(cb);
        Bartok.S.MoveToTarget(cb);
        cb.callbackPlayer = this;
    }

    public void CBCallback(CardBartok tCB)
    {
        Bartok.S.PassTurn();
    }
}
