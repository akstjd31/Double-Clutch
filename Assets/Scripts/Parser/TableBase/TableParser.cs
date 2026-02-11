using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Loading;
using UnityEngine;

/// <summary>
/// CSV파일을 파싱하고 정의된 ROW클래스에 데이터를 채워넣는 클래스
/// 
/// </summary>
public class TableParser
{
    //실제 컬럼 이름이 적혀있는 행의 인덱스
    private const int HeaderRowIndex = 0;
    //데이터가 시작되는 첫 줄의 인덱스
    private const int DataStartRowIndex = 2;

    public static Table<TKey, TRow> Parse<TKey, TRow>(
        TextAsset csvFile,
        string idColumnName
        ) where TRow : TableBase, new()
    {
        if (csvFile == null)
        {
            Debug.LogError("CSV 파일이 null입니다.");
            return null;
        }

        //1.CSV 내용 읽기 및 줄/셀 분리
        List<string[]> rows;
        try
        {
            rows = csvFile.text.
                Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Split('|').Select(cell => cell.Trim()).ToArray())
                .ToList();
        }
        catch (Exception e)
        {
            Debug.LogError($"CSV파일 읽기 오류");
            return null;
        }

        //2.컬럼 헤더 추출
        if (rows.Count <= HeaderRowIndex)
        {
            Debug.LogError($"CSV에 컬럼 헤더{HeaderRowIndex}가 없습니다");
            return null;
        }

        string[] columnNames = rows[HeaderRowIndex];

        int idIndex = Array.IndexOf(columnNames, idColumnName);
        if (idIndex < 0)
        {
            Debug.LogError($"CSV에 id 컬럼 {idColumnName}가 없습니다");
            return null;
        }

        //3.TRow클래스 속성 및 필드 컬럼명 맵핑
        Type rowType = typeof(TRow);
        Dictionary<string, MemberInfo> memberMap = new Dictionary<string, MemberInfo>();
        foreach (var name in columnNames)
        {
            var prop = rowType.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if (prop != null)
            {
                memberMap.Add(name, prop);
                continue;
            }

            var field = rowType.GetField(name, BindingFlags.Public | BindingFlags.Instance);
            if (field != null)
            {
                memberMap.Add(name, field);
                continue;
            }
        }

        //4.데이터 파싱 및 TRow인스턴스 생성
        Dictionary<TKey, TRow> allRowData = new Dictionary<TKey, TRow>();
        for (int i = DataStartRowIndex; i < rows.Count; i++)
        {
            string[] rowValues = rows[i];

            if (rowValues.Length < columnNames.Length)
            {
                if (rowValues.All(string.IsNullOrEmpty))
                    continue;
            }

            //ID추출 및 TKey타입으로 변환
            string idString = rowValues[idIndex];
            if (string.IsNullOrEmpty(idString))
                continue;

            TKey idValue = default;
            try
            {
                idValue = (TKey)Convert.ChangeType(idString, typeof(TKey));

            }
            catch (Exception e)
            {
                Debug.LogError($"id{idString} 변환 실패");
            }

            if (allRowData.ContainsKey(idValue))
            {
                Debug.LogError($"{idValue} 중복");
                continue;
            }

            TRow newRow = new TRow();

            //안스턴스에 값 채우기
            for (int j = 0; j < columnNames.Length; j++)
            {
                string columnName = columnNames[j];
                string stringValue = rowValues[j];

                if (memberMap.TryGetValue(columnName, out MemberInfo member))
                {
                    Type targetType = (member is PropertyInfo prop) ? prop.PropertyType : ((FieldInfo)member).FieldType;

                    try
                    {
                        object convertedValue = ConvertValue(stringValue, targetType);
                        if (member is PropertyInfo propInfo)
                        {
                            propInfo.SetValue(newRow, convertedValue);
                        }
                        else if (member is FieldInfo fieldInfo)
                        {
                            fieldInfo.SetValue(newRow, convertedValue);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"값 변환 오류: 칼럼'{columnName}'의 값 '{stringValue}' 을(를) {targetType} 타입으로 변환할 수 없습니다. 오류:{e.Message}");
                    }
                }
            }
            allRowData.Add(idValue, newRow);
        }
        string tableName = csvFile.name;
        return new Table<TKey, TRow>(tableName, idColumnName, allRowData);
    }

    private static object ConvertValue(string value, Type targetType)
    {
        value = value.Trim();

        if (string.IsNullOrEmpty(value))
        {
            if (targetType.IsValueType && Nullable.GetUnderlyingType(targetType) == null)
            {
                return Activator.CreateInstance(targetType);
            }
            return null;
        }
        if (targetType == typeof(bool))
        {
            if (int.TryParse(value, out int intValue))
            {
                return intValue != 0;
            }
            return bool.Parse(value);
        }
        if (targetType.IsEnum)
        {
            if (int.TryParse(value, out int enumInt))
            {
                return Enum.ToObject(targetType, enumInt);
            }
            return Enum.Parse(targetType, value, ignoreCase: true);
        }
        return Convert.ChangeType(value, targetType);
    }
}
