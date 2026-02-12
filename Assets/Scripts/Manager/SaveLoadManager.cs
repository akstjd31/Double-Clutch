using System.IO;
using UnityEngine;

public class SaveLoadManager : Singleton<SaveLoadManager>
{
    private string GetPath(string fileName) => Path.Combine(Application.persistentDataPath, fileName);

    public void Save<T> (string fileName, T data) where T : SaveBase
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            Debug.LogError("파일명이 비어있습니다!");
            return;
        }

        var path = GetPath(fileName);
        var tmpPath = path + ".tmp";
        var json = JsonUtility.ToJson(data, true);

        File.WriteAllText(tmpPath, json);

        // 깨짐 방지를 위한 기존 파일 제거
        if (File.Exists(path)) File.Delete(path);
        File.Move(tmpPath, path);

        Debug.Log("파일 저장 성공!");
    }

    public bool TryLoad<T> (string fileName, out T data) where T : SaveBase, new()
    {
        data = null;

        if (string.IsNullOrWhiteSpace(fileName))
        {
            Debug.LogError("파일명이 비어있습니다!");
            return false;
        }

        var path = GetPath(fileName);
        if (!File.Exists(path))
        {
            Debug.LogError("해당 파일명이 존재하지 않음!");
            return false;
        }

        var json = File.ReadAllText(path);
        data = JsonUtility.FromJson<T>(json);

        if (data == null)
        {
            Debug.LogError("로드 실패! 데이터가 비어있음.");
            return false;
        }

        return true;
    }
}
