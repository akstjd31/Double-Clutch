using UnityEngine;
using UnityEngine.Pool;

public class GenericObjectPool<T> where T : Component
{
    private T _prefab;
    private Transform _container;
    private IObjectPool<T> _pool;

    public GenericObjectPool(T prefab, Transform container, int defaultSize = 10, int maxSize = 20)
    {
        _prefab = prefab;
        _container = container;

        _pool = new ObjectPool<T>(
            OnCreateItem,       // 새 객체 생성 시
            OnTakeFromPool,     // 풀에서 꺼낼 때
            OnReturnedToPool,   // 풀에 반납할 때
            OnDestroyPoolObject,// 풀이 꽉 찼을 때 삭제
            true,               // 컬렉션 체크 (중복 반납 방지)
            defaultSize,
            maxSize
        );
    }
    private T OnCreateItem() => Object.Instantiate(_prefab, _container); //프리팹 최초 생성 및 컨테이너에 보관
    private void OnTakeFromPool(T obj) => obj.gameObject.SetActive(true); //이미 컨테이너에 있는 프리팹 사용
    private void OnReturnedToPool(T obj) => obj.gameObject.SetActive(false); //사용 후 컨테이너에 프리팹 반납
    private void OnDestroyPoolObject(T obj) => Object.Destroy(obj.gameObject); //오브젝트를 씬과 풀에서 파괴

    public T Get() => _pool.Get();
    public void Release(T obj) => _pool.Release(obj);
}