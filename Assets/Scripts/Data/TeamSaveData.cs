using System.Collections.Generic;
using UnityEngine;

public class TeamSaveData : SaveBase
{    
    public List<Team> teamList; 

    public TeamSaveData(List<Team> teamList)
    {
        this.teamList = teamList;        
    }

    
    public TeamSaveData()
    {

    }
}
