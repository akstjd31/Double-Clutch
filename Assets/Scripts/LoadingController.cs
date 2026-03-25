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

        var op = SceneManager.LoadSceneAsync(GameManager.Instance.NextSceneName);
        op.allowSceneActivation = false;

        while (timer < minTime || op.progress < 0.9f)
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(op.progress / 0.9f);
            _slider.value = progress;

            yield return null;
        }

        op.allowSceneActivation = true;

        GameManager.Instance.NotifyLoadingDone();
    }
}