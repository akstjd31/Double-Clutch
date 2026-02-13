using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Team_ArchetypeDataReader", menuName = "Scriptable Object/Team_ArchetypeDataReader", order = int.MaxValue)]
public class Team_ArchetypeDataReader : DataReaderBase
{
    [SerializeField] public List<Team_ArchetypeData> DataList = new List<Team_ArchetypeData>();

    internal void UpdateStats(List<GSTU_Cell> list)
    {
        string teamId = null, desc = null, teamCName = null;
        int cPG = 0, cSG = 0, cSF = 0, cPF = 0, cC = 0;
        float w2pt = 0, w3pt = 0, wP = 0, wB = 0, wS = 0, wR = 0;

        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "teamArchetypeId":
                    teamId = val;
                    break;
                
                case "desc":
                    desc = val;
                    break;

                case "teamColorName":
                    teamCName = val;
                    break;
                
                case "countPG":
                    int.TryParse(val, out cPG);
                    break;
                
                case "countSG":
                    int.TryParse(val, out cSG);
                    break;
                
                case "countSF":
                    int.TryParse(val, out cSF);
                    break;
                
                case "countPF":
                    int.TryParse(val, out cPF);
                    break;
                
                case "countC":
                    int.TryParse(val, out cC);
                    break;
                
                case "weight2pt":
                    float.TryParse(val, out w2pt);
                    break;
                
                case "weight3pt":
                    float.TryParse(val, out w3pt);
                    break;
                
                case "weightPass":
                    float.TryParse(val, out wP);
                    break;
                
                case "weightBlock":
                    float.TryParse(val, out wB);
                    break;
                
                case "weightSteal":
                    float.TryParse(val, out wS);
                    break;
                
                case "weightRebound":
                    float.TryParse(val, out wR);
                    break;
            }
        }

        var teamData = new Team_ArchetypeData
        (
            teamId, desc, teamCName, cPG, cSG, cSF, cPF, cC, w2pt, w3pt, wP, wB, wS, wR
        );

        DataList.Add(teamData);
    } 
}
