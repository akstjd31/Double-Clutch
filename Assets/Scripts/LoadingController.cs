using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingController : MonoBehaviour
{
    [SerializeField] private Slider _slider;

    private void Start()
    {
        _slider.value = 0;
        StartCoroutine(LoadingTimer());
    }

    private IEnumerator LoadingTimer()
    {
        float minTime = 2f;
        float timer = 0f;

        string nextSceneName = GameManager.Instance.NextSceneName;
        Debug.Log($"다음 씬 호출: {nextSceneName}");

        var op = SceneManager.LoadSceneAsync(nextSceneName);
        op.allowSceneActivation = false;

        while (timer < minTime || op.progress < 0.9f)
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(op.progress / 0.9f);
            _slider.value = progress;

            yield return null;
        }

        // 씬 로드 완료 이벤트 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
        op.allowSceneActivation = true;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        Debug.Log($"씬 로드 완료: {scene.name}");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.NotifyLoadingDone();
        }
    }
}