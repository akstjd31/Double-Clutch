using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public class AddressableManager : Singleton<AddressableManager>
{
    private Dictionary<string, AsyncOperationHandle> _assetHandles = new Dictionary<string, AsyncOperationHandle>();
    private Dictionary<GameObject, string> _instanceToAddress = new Dictionary<GameObject, string>();

    public void LoadAsset<T>(string address, Action<T> onComplete) where T : UnityEngine.Object
    {
        if (_assetHandles.ContainsKey(address))
        {
            onComplete?.Invoke(_assetHandles[address].Convert<T>().Result);
            return;
        }

        var handle = Addressables.LoadAssetAsync<T>(address);
        handle.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                _assetHandles[address] = op;
                onComplete?.Invoke(op.Result);
            }
        };
    }

    // UI 프리팹 생성
    public void InstantiateUI(string address, Transform parent, Action<GameObject> onComplete)
    {
        var handle = Addressables.InstantiateAsync(address, parent);
        handle.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                _instanceToAddress[op.Result] = address;
                onComplete?.Invoke(op.Result);
            }
        };
    }

    // 메모리 해제
    public void UnloadAsset(string address)
    {
        if (_assetHandles.TryGetValue(address, out var handle))
        {
            Addressables.Release(handle);
            _assetHandles.Remove(address);
        }
    }

    public void ReleaseInstance(GameObject obj)
    {
        if (obj == null) return;
        Addressables.ReleaseInstance(obj);
        _instanceToAddress.Remove(obj);
    }
}
