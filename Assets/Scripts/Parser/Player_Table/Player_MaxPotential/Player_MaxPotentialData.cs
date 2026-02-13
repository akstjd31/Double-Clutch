using System;


[Serializable]
public struct Player_MaxPotentialData
{
    public int key;
    public int minPotentialValue;
    public int maxPotentialValue;

   

    public Player_MaxPotentialData(int _key, int _minPotentialValue, int _maxPotentialValue)
    {
        key = _key;
        minPotentialValue = _minPotentialValue;
        maxPotentialValue = _maxPotentialValue;     
    }
}
