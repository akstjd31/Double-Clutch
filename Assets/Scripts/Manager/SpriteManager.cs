using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : Singleton<SpriteManager>
{
    [SerializeField] ResourceDataReader _db;

    // [ID : 전체경로]를 미리 저장해서 검색 속도를 최적화
    private Dictionary<string, string> _pathIndex = new Dictionary<string, string>();

    // 이미 로드된 스프라이트 재사용 (메모리 관리)
    private Dictionary<string, Sprite> _spriteCache = new Dictionary<string, Sprite>();

    protected override void Awake()
    {
        base.Awake();
        InitPathTable();
    }

    /// <summary>
    /// DB의 리스트를 딕셔너리로 변환하여 준비
    /// </summary>
    public void InitPathTable()
    {
        if (_db == null)
        {
            Debug.LogError("ResourceDataReader가 연결되지 않았습니다!");
            return;
        }

        _pathIndex.Clear();
        foreach (var data in _db.DataList)
        {
            if (string.IsNullOrEmpty(data.resourceId)) continue;

            // 경로와 아이디를 결합하여 실제 Resources.Load 경로 생성            
            string fullPath = $"{data.resourcePath}/{data.resourceId}";

            if (!_pathIndex.ContainsKey(data.resourceId))
            {
                _pathIndex.Add(data.resourceId, fullPath);
            }
        }
        Debug.Log($"SpriteManager {_pathIndex.Count}개의 리소스 경로 Init 완료.");
    }

    
    // ID를 통해 스프라이트를 가져오기    
    public Sprite GetSprite(string resourceId)
    {
        // 1. 이미 불러온 적이 있는지 확인
        //if (!_spriteCache.ContainsKey(resourceId))
        //{
        //    Debug.Log("해당 리소스 키 캐시에 없음");
        //}
        if (_spriteCache.TryGetValue(resourceId, out Sprite cachedSprite))
        {
            return cachedSprite;
        }

        // 2. 인덱스 테이블에서 실제 경로 가져오기
        if (_pathIndex.TryGetValue(resourceId, out string fullPath))
        {
            Sprite loadedSprite = Resources.Load<Sprite>(fullPath);

            if (loadedSprite != null)
            {
                _spriteCache.Add(resourceId, loadedSprite);
                return loadedSprite;
            }
        }

        Debug.LogWarning($"[SpriteManager] 리소스를 로드할 수 없습니다. ID: {resourceId}");
        return null;
    }

    
    // 메모리 최적화를 위한 캐시 클리어.    
    public void ClearCache()
    {
        _spriteCache.Clear();
        Resources.UnloadUnusedAssets();
    }
}
