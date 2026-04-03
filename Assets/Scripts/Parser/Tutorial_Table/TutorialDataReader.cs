using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TutorialDataReader", menuName = "Scriptable Object/TutorialDataReader", order = int.MaxValue)]
public class TutorialDataReader : DataReaderBase
{
    [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")]
    [SerializeField] public List<TutorialData> DataList = new List<TutorialData>();

    // ✅ ItemData처럼 List<GSTU_Cell> 한 줄을 받아서 파싱
    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        string slideOrder = null, tutorialId = null, tutorialImageId = null, narrationKey = null, dialogueKey = null, speakerKey = null;
        bool isSkippable = false;

        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;   // ✅ "weekId", "desc" 등 (시트 2행 헤더)
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "slideOrder":
                    slideOrder = val;
                    break;

                case "tutorialId":
                    tutorialId = val;
                    break;

                case "tutorialImageId":
                    tutorialImageId = val;
                    break;

                case "narrationKey":
                    narrationKey = val;
                    break;

                case "dialogueKey":
                    dialogueKey = val;
                    break;

                case "speakerKey":
                    speakerKey = val;
                    break;

                case "isSkippable":
                    isSkippable = ParseBool(val);
                    break;
            }
        }


        DataList.Add(new TutorialData(slideOrder, tutorialId, tutorialImageId, narrationKey, dialogueKey, speakerKey,isSkippable
        ));
    }

    private static bool ParseBool(string val)
    {
        if (string.IsNullOrEmpty(val)) return false;
        val = val.Trim().ToLowerInvariant();
        return val == "1" || val == "true" || val == "y" || val == "yes";
    }
}
