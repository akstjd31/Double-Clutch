using UnityEngine;

/// <summary>
/// 제네릭 싱글톤 베이스
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static bool _isQuitting;                        // 플레이어가 접속을 종료하였는가?
    private static readonly object _lock = new object();
    [SerializeField] private bool dontDestroyOnLoad = true;
    public static T Instance
    {
        get
        {
            if (_isQuitting) return null;

            lock (_lock)
            {
                if (_instance != null) return _instance;

                _instance = FindFirstObjectByType<T>();
                if (_instance != null) return _instance;

                var go = new GameObject(typeof(T).Name);
                _instance = go.AddComponent<T>();

                return _instance;
            }
        }
    }

    protected virtual void Awake()
    {
        // 접속 종료 == 소멸
        if (_isQuitting)
        {
            Destroy(this.gameObject);
            return;
        }
        
        // 검증 (존재 여부)
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this as T;

        // 옵션 적용
        if (dontDestroyOnLoad)
            DontDestroyOnLoad(this.gameObject);
    }

    protected virtual void OnApplicationQuit() 
    {
        _isQuitting = true;
    }

    protected virtual void OnDestroy() 
    {
        if (!_isQuitting && ReferenceEquals(_instance, this))
            _instance = null;
    }
}
