using System;

[Serializable]

public struct Balance_Data
{
    public int index;
    public string weightId;
    public int value;

    public Balance_Data(int index, string weightId, int value)
    {
        this.index = index;
        this.weightId = weightId;
        this.value = value;
    }
}
