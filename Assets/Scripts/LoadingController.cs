using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingController : Singleton<LoadingController>
{
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);

        var target = GameManager.Instance.NextSceneName;

        var op = SceneManager.LoadSceneAsync(target);
        op.allowSceneActivation = true;

        yield return op;      // 로드 + 활성화 완료까지 대기
        yield return null;    // 다음 씬 첫 프레임 보장(선택이지만 추천)

        GameManager.Instance.NotifyLoadingDone();

        // 로딩 컨트롤러는 한 번 쓰고 제거해도 됨
        Destroy(this.gameObject);
    }
}
