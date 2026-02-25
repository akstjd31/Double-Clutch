using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableManager : MonoBehaviour
{
    AsyncOperationHandle<GameObject> instanceHandle;

    private void Start()
    {
        instanceHandle = Addressables.InstantiateAsync("이름");
    }

    private void OnDestroy()
    {
        if (instanceHandle.IsValid())
        {
            Addressables.ReleaseInstance(instanceHandle);
        }
    }
}
