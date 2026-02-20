using System;
using UnityEngine;


[Serializable]
public struct Event_DataModel
{
    public string eventId;
    public potential mainPotentialType;
    public int requiredPotentialValue;
    public float potentialPercent;
    public string eventPriority;
    public int cooldownTurn;


    public Event_DataModel(string eventId, potential mainPotentialType, int requiredPotentialValue, float potentialPercent, string eventPriority, int cooldownTurn)
    {
        this.eventId = eventId;
        this.mainPotentialType = mainPotentialType;
        this.requiredPotentialValue = requiredPotentialValue;
        this.potentialPercent = potentialPercent;
        this.eventPriority = eventPriority;
        this.cooldownTurn = cooldownTurn;
    }
}
