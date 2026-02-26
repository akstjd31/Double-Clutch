using System;


[Serializable]
public struct Player_MaxPotentialData
{
    public string potentialId;
    public int minPotentialValue;
    public int maxPotentialValue;



    public Player_MaxPotentialData(string _potentialId, int _minPotentialValue, int _maxPotentialValue)
    {
        potentialId = _potentialId;
        minPotentialValue = _minPotentialValue;
        maxPotentialValue = _maxPotentialValue;
    }
}
