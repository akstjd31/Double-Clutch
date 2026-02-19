using System;
using UnityEngine;

[Serializable]
public struct Event_DataModel
{
    public string eventId;
    public potentialType mainPotentialType;
    public int requiredPotentialValue;
    public float potentialPercent;
    public string eventPriority;
    public int cooldownTurn;


    public Event_DataModel(string eventId, potentialType mainPotentialType, int requiredPotentialValue, float potentialPercent, string eventPriority, int cooldownTurn)
    {
        this.eventId = eventId;
        this.mainPotentialType = mainPotentialType;
        this.requiredPotentialValue = requiredPotentialValue;
        this.potentialPercent = potentialPercent;
        this.eventPriority = eventPriority;
        this.cooldownTurn = cooldownTurn;
    }

}
