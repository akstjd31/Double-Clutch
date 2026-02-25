using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player_VisualDataReader", menuName = "Scriptable Object/Player_VisualDataReader", order = int.MaxValue)]
public class Player_VisualDataReader : DataReaderBase
{
    [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")]
    [SerializeField] public List<Player_VisualData> DataList = new List<Player_VisualData>();

    // ✅ ItemData처럼 List<GSTU_Cell> 한 줄을 받아서 파싱
    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        string visualId = null;
        string speciesId = null;
        string desc = null;
        string playerImageResource = null;
        string playerImageResourceAngry = null;
        string playerImageResourceAnnoyed = null;
        string playerImageResourceGaki = null;
        string playerImageResourceCringe = null;
        string playerImageResourceDisappointed = null;
        string playerImageResourceEmbarrassed = null;
        string playerImageResourceFrustrated = null;
        string playerImageResourceHappy = null;
        string playerImageResourceQuestioning = null;
        string playerImageResourceRolling = null;
        string playerImageResourceSad = null;
        string playerImageResourceSerious = null;
        string playerImageResourceSurprised = null;
        string playerImageResourceTalking = null;
        string playerImageResourceThinking = null;
        string portraitResource = null;
        string playerCutInResourceId01 = null;
        string playerCutInResourceId02 = null;
        string playerCutInResourceId03 = null;


        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;   // ✅ "weekId", "desc" 등 (시트 2행 헤더)
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "visualId":
                    visualId = val;
                    break;
                case "speciesId":
                    speciesId = val;
                    break;
                case "desc":
                    desc = val;
                    break;
                case "playerImageResource":
                    playerImageResource = val;
                    break;
                case "playerImageResourceAngry":
                    playerImageResourceAngry = val;
                    break;
                case "playerImageResourceAnnoyed":
                    playerImageResourceAnnoyed = val;
                    break;
                case "playerImageResourceGaki":
                    playerImageResourceGaki = val;
                    break;
                case "playerImageResourceCringe":
                    playerImageResourceCringe = val;
                    break;
                case "playerImageResourceDisappointed":
                    playerImageResourceDisappointed = val;
                    break;
                case "playerImageResourceEmbarrassed":
                    playerImageResourceEmbarrassed = val;
                    break;
                case "playerImageResourceFrustrated":
                    playerImageResourceFrustrated = val;
                    break;
                case "playerImageResourceHappy":
                    playerImageResourceHappy = val;
                    break;
                case "playerImageResourceQuestioning":
                    playerImageResourceQuestioning = val;
                    break;
                case "playerImageResourceRolling":
                    playerImageResourceRolling = val;
                    break;
                case "playerImageResourceSad":
                    playerImageResourceSad = val;
                    break;
                case "playerImageResourceSerious":
                    playerImageResourceSerious = val;
                    break;
                case "playerImageResourceSurprised":
                    playerImageResourceSurprised = val;
                    break;
                case "playerImageResourceTalking":
                    playerImageResourceTalking = val;
                    break;
                case "playerImageResourceThinking":
                    playerImageResourceThinking = val;
                    break;
                case "portraitResource":
                    portraitResource = val;
                    break;
                case "playerCutInResourceId01":
                    playerCutInResourceId01 = val;
                    break;
                case "playerCutInResourceId02":
                    playerCutInResourceId02 = val;
                    break;
                case "playerCutInResourceId03":
                    playerCutInResourceId03 = val;
                    break;
            }
        }

        DataList.Add(new Player_VisualData(visualId, speciesId, desc, playerImageResource, playerImageResourceAngry, 
            playerImageResourceAnnoyed, playerImageResourceGaki, playerImageResourceCringe, playerImageResourceDisappointed
            , playerImageResourceEmbarrassed, playerImageResourceFrustrated, playerImageResourceHappy, playerImageResourceQuestioning
            , playerImageResourceRolling, playerImageResourceSad, playerImageResourceSerious, playerImageResourceSurprised, 
            playerImageResourceTalking, playerImageResourceThinking, portraitResource, playerCutInResourceId01, playerCutInResourceId02, playerCutInResourceId03
        ));
    }

    private static bool ParseBool(string val)
    {
        if (string.IsNullOrEmpty(val)) return false;
        val = val.Trim().ToLowerInvariant();
        return val == "1" || val == "true" || val == "y" || val == "yes";
    }
}
