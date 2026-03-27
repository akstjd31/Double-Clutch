using GoogleSheetsToUnity;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EndingRollDataReader", menuName = "Scriptable Object/EndingRollDataReader", order = int.MaxValue)]
public class EndingRollDataReader : DataReaderBase
{
    [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")]
    [SerializeField] public List<EndingRollData> DataList = new List<EndingRollData>();

    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        string rollId = "";
        string typeKey = "";
        string nameKey = "";

        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "rollId":
                    rollId = val;
                    break;
                case "TypeKey":
                    typeKey = val;
                    break;
                case "nameKey":
                    nameKey = val;
                    break;
            }
        }

        // 필수 값인 rollId가 없으면 스킵 (빈 행 방지)
        if (string.IsNullOrEmpty(rollId)) return;

        // 리스트에 추가
        DataList.Add(new EndingRollData(rollId, typeKey, nameKey));
    }
}