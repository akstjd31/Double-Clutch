using System.Collections.Generic;
using UnityEngine;

//특정한 테이블의 ROW객체를 ID를 통해 저장 및 조회할 수 있는 클래스
/// <typeparam name="TKey">테이블의 ID값</typeparam>
/// <typeparam name="TRow">실제 ROW타입 데이터</typeparam>

public class Table<TKey, TRow> where TRow : TableBase
{
    private readonly Dictionary<TKey, TRow> _data;
    private readonly string _tableName;
    private readonly string _idColumName;

    public IReadOnlyDictionary<TKey, TRow> Data => _data;

    public Table(string tableName, string idColumName, Dictionary<TKey, TRow> data)
    {
        _tableName = tableName;
        _idColumName = idColumName;
        _data = data;
    }

    public TRow this[TKey id]
    {
        get
        {
            if (_data.TryGetValue(id, out var row)) return row;
            Debug.LogError($"존재하지 않는 ID:{id}");
            return null;
        }
    }
}
