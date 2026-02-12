using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingController : Singleton<LoadingController>
{
    private IEnumerator Start()
    {
        // 임시 대기 시간 적용
        yield return new WaitForSeconds(2f);

        var target = GameManager.Instance.NextSceneName;

        var op = SceneManager.LoadSceneAsync(target);
        op.allowSceneActivation = true;

        yield return op;      // 로드 + 활성화 완료까지 대기

        GameManager.Instance.NotifyLoadingDone();

        Destroy(this.gameObject);
    }
}
