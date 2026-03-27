using GoogleSheetsToUnity;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptDataReader", menuName = "Scriptable Object/ScriptDataReader", order = int.MaxValue)]
public class ScriptDataReader : DataReaderBase
{
    [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")]
    [SerializeField] public List<ScriptData> DataList = new List<ScriptData>();

    internal void UpdateStats(List<GSTU_Cell> list, int rowIndex)
    {
        // 필드별 변수 선언 (초기값 설정)
        string scriptId = "";
        int currentId = 0;
        int scriptType = 0;
        string text = "";
        bool speaking = false;
        string name = "";
        string background = "";
        string portrait = "";
        string cg = "";
        string bgm = "";
        string sfx = "";
        float textSpeed = 0f;
        string cameraEffectId = "";
        string startEffectId = "";

        for (int i = 0; i < list.Count; i++)
        {
            string col = list[i].columnId;
            string val = list[i].value;

            if (string.IsNullOrWhiteSpace(val) || val == "-")
                val = "";

            switch (col)
            {
                case "scriptId": scriptId = val; break;
                case "currentId": int.TryParse(val, out currentId); break;
                case "scriptType": int.TryParse(val, out scriptType); break;
                case "text": text = val; break;
                case "speaking": speaking = ParseBool(val); break;
                case "name": name = val; break;
                case "background": background = val; break;
                case "portrait": portrait = val; break;
                case "cg": cg = val; break;
                case "bgm": bgm = val; break;
                case "sfx": sfx = val; break;
                case "textSpeed": float.TryParse(val, out textSpeed); break;
                case "cameraEffectId": cameraEffectId = val; break;
                case "startEffectId": startEffectId = val; break;
            }
        }

        // scriptId가 없으면 데이터가 없는 행으로 간주하여 스킵
        if (string.IsNullOrEmpty(scriptId)) return;

        // 리스트에 추가 (생성자 호출)
        DataList.Add(new ScriptData(
            scriptId, currentId, scriptType, text, speaking,
            name, background, portrait, cg, bgm,
            sfx, textSpeed, cameraEffectId, startEffectId
        ));
    }

    private static bool ParseBool(string val)
    {
        if (string.IsNullOrEmpty(val)) return false;
        val = val.Trim().ToLowerInvariant();
        return val == "1" || val == "true" || val == "y" || val == "yes";
    }
}