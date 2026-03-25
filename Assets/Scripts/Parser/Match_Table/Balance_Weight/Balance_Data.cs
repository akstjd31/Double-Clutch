using System;

[Serializable]

public struct Balance_Data
{
    public int index;
    public string weightId;
    public float value;

    public Balance_Data(int index, string weightId, float value)
    {
        this.index = index;
        this.weightId = weightId;
        this.value = value;
    }
}
