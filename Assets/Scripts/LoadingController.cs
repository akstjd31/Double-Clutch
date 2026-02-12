using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingController : MonoBehaviour
{
    private AsyncOperation _op;

    IEnumerator Start()
    {
        if (GameManager.Instance == null) yield break;

        yield return new WaitForSeconds(2.0f);  // 임시
        
        // 다음 씬으로 전환 가능함을 알림
        var target = GameManager.Instance.NextSceneName;
        _op = SceneManager.LoadSceneAsync(target);
        _op.allowSceneActivation = true;
    }

    private void Update()
    {
        if (_op == null) return;

        if (_op.isDone)
        {
            GameManager.Instance.NotifyLoadingDone();
            _op = null;
        }
    }
}
