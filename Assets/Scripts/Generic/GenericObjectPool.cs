using UnityEngine;
using UnityEngine.Pool;

public class GenericObjectPool<T> where T : Object
{
    private T _prefab;
    private Transform _container;
    private IObjectPool<T> _pool;

    public GenericObjectPool(T prefab, Transform container, int defaultSize = 10, int maxSize = 50)
    {
        _prefab = prefab;
        _container = container;

        _pool = new ObjectPool<T>(
            OnCreateItem,
            OnTakeFromPool,
            OnReturnedToPool,
            OnDestroyPoolObject,
            collectionCheck: true,
            defaultCapacity: defaultSize,
            maxSize: maxSize
        );
    }

    private T OnCreateItem()
    {
        return Object.Instantiate(_prefab, _container);
    }

    private void OnTakeFromPool(T obj)
    {        
        if (obj is GameObject go) go.SetActive(true);
        else if (obj is Component comp) comp.gameObject.SetActive(true);
    }

    private void OnReturnedToPool(T obj)
    {
        if (obj is GameObject go) go.SetActive(false);
        else if (obj is Component comp) comp.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(T obj)
    {
        if (obj is GameObject go) Object.Destroy(go);
        else if (obj is Component comp) Object.Destroy(comp.gameObject);
    }

    public T Get() => _pool.Get();
    public void Release(T obj) => _pool.Release(obj);
}