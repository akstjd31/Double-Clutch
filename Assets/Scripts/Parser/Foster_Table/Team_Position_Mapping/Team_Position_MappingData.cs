using System;


[Serializable]
public struct Team_Position_MappingData
{
    public Position position;
    public potential mainPotential;
    public potential subPotential;




    public Team_Position_MappingData(Position position, potential mainPotential, potential subPotential)
    {
        this.position = position;
        this.mainPotential = mainPotential;
        this.subPotential = subPotential;
    }
}