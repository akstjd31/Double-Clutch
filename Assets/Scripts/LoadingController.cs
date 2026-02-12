using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingController : MonoBehaviour
{
    private AsyncOperation _op;

    IEnumerator Start()
    {
        if (GameManager.Instance == null) yield break;

        // 데이터 로드와 같이 모든 준비가 완료되었다면
        // 아마 코루틴으로 바뀔듯?
        yield return new WaitForSeconds(2.0f);  // 임시

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
