using GoogleSheetsToUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;

#if UNITY_EDITOR
[CustomEditor(typeof(Player_VisualDataReader))]
public class Player_VisualDataReaderEditor : Editor
{
    private Player_VisualDataReader data;

    // ✅ 너 시트 구조 고정:
    // 1행 한글설명 / 2행 영문헤더 / 3행 타입 / 4행부터 데이터
    private const int DATA_START_ROW_INDEX = 3; // (0-based) 4행

    void OnEnable()
    {
        data = (Player_VisualDataReader)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Label("\n\n스프레드 시트 읽어오기");

        if (GUILayout.Button("데이터 읽기(API 호출)"))
        {
            data.DataList.Clear();
            UpdateStats(UpdateMethodOne);
        }
    }

    void UpdateStats(UnityAction<GstuSpreadSheet> callback, bool mergedCells = false)
    {
        SpreadsheetManager.Read(new GSTU_Search(data.associatedSheet, data.associatedWorksheet), callback, mergedCells);
    }

    void UpdateMethodOne(GstuSpreadSheet ss)
    {
        // DataReaderBase에서 설정한 시작/끝 행(1-based)을 반영
        // 시트 구조: 1행 설명 / 2행 헤더 / 3행 타입 / 4행부터 데이터
        // 너는 4행부터 읽고 싶으니 START_ROW_LENGTH 기본 4가 맞음
        int start0 = Mathf.Max(0, data.START_ROW_LENGTH - 1);
        int end0 = (data.END_ROW_LENGTH <= 0) ? int.MaxValue : (data.END_ROW_LENGTH - 1);

        foreach (var rowIndex in GetRowIndices(ss))
        {
            if (rowIndex < start0) continue;
            if (rowIndex > end0) break;

            var row = GetRow(ss, rowIndex);
            if (row != null)
                data.UpdateStats(row, rowIndex);
        }

        EditorUtility.SetDirty(target);
        AssetDatabase.SaveAssets();
        Debug.Log($"[Calendar_TableDataReader] Loaded: {data.DataList.Count} rows");
    }
    static IEnumerable<int> GetRowIndices(GstuSpreadSheet ss)
    {
        // ss.rows (SecondaryKeyDictionary)에서 primary key 목록을 최대한 폭넓게 찾음
        object rows = ss.rows;

        // 1) PrimaryKeys / primaryKeys / Keys 프로퍼티 시도
        foreach (var propName in new[] { "PrimaryKeys", "primaryKeys", "Keys" })
        {
            var p = rows.GetType().GetProperty(propName);
            if (p == null) continue;

            var val = p.GetValue(rows, null);
            if (val is IEnumerable enumerable)
            {
                var list = new List<int>();
                foreach (var x in enumerable)
                {
                    if (x is int i) list.Add(i);
                    else if (x != null && int.TryParse(x.ToString(), out var parsed)) list.Add(parsed);
                }
                list.Sort();
                return list;
            }
        }

        // 2) GetPrimaryKeys / GetPrimaryKeyList 같은 메서드 시도
        foreach (var mName in new[] { "GetPrimaryKeys", "GetPrimaryKeyList" })
        {
            var m = rows.GetType().GetMethod(mName);
            if (m == null) continue;

            var val = m.Invoke(rows, null);
            if (val is IEnumerable enumerable)
            {
                var list = new List<int>();
                foreach (var x in enumerable)
                {
                    if (x is int i) list.Add(i);
                    else if (x != null && int.TryParse(x.ToString(), out var parsed)) list.Add(parsed);
                }
                list.Sort();
                return list;
            }
        }

        // 3) 아무것도 못 찾으면 0~(안전하게) 500까지 시도 (최후의 안전장치)
        // 시트가 작으면 이걸로도 통과함
        var fallback = new List<int>();
        for (int i = 0; i < 500; i++) fallback.Add(i);
        return fallback;
    }

    static List<GSTU_Cell> GetRow(GstuSpreadSheet ss, int rowIndex)
    {
        object rows = ss.rows;

        // indexer (this[int]) 찾기
        var indexer = rows.GetType().GetProperty("Item", new[] { typeof(int) });
        if (indexer != null)
        {
            try
            {
                return indexer.GetValue(rows, new object[] { rowIndex }) as List<GSTU_Cell>;
            }
            catch { /* ignore */ }
        }

        // TryGetValue(int, out List<GSTU_Cell>) 같은 형태 시도
        var tryGet = rows.GetType().GetMethod("TryGetValue", new[] { typeof(int), typeof(List<GSTU_Cell>).MakeByRefType() });
        if (tryGet != null)
        {
            object[] args = new object[] { rowIndex, null };
            bool ok = (bool)tryGet.Invoke(rows, args);
            if (ok) return args[1] as List<GSTU_Cell>;
        }

        return null;
    }

}
#endif