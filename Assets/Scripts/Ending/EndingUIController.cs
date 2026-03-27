using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndingUIController : MonoBehaviour
{
    [SerializeField] Image _backgroundImage;
    [SerializeField] Image _characterImage;
    [SerializeField] EndingTextBox _endingTextBox;
    [SerializeField] CanvasGroup _fadePanel;
    CanvasGroup _characterCanvasGroup;

    [Header("페이드 인/아웃 연출 소요시간")]
    [SerializeField] float _fadeTime = 0.5f;

    public bool IsFading { get; private set; } = false;

    private Coroutine _fadeRoutine;

    private void Awake()
    {
        _characterCanvasGroup = _characterImage.GetComponent<CanvasGroup>();
    }

    string _currentBackgroundKey = string.Empty;
    string _currentCharacterKey = string.Empty;


    public void SetBackgroundImage(string imageKey)
    {
        if (_currentBackgroundKey != imageKey)
        {
            _backgroundImage.sprite = SpriteManager.Instance.GetSprite(imageKey);
        }        
    }

    public void SetCharacterSpeaking(bool isSpeaking)
    {
        _characterCanvasGroup.alpha = isSpeaking ? 1 : 0.5f;
    }

    public void SetCharacterImage(string imageKey)
    {
        if (string.IsNullOrEmpty(imageKey))
        {
            _characterImage.sprite = null;
            _characterImage.gameObject.SetActive(false);
            return;
        }
        if (_currentCharacterKey != imageKey)
        {
            _characterImage.gameObject.SetActive(true);
            _characterImage.sprite = SpriteManager.Instance.GetSprite(imageKey);
        }        
    }

    public void SetText(string nameKey, string contentKey)
    {
        _endingTextBox.SetName(nameKey);
        _endingTextBox.SetText(contentKey);
    }

    public void FadeIn()
    {
        if (_fadeRoutine != null)
        {
            StopCoroutine(_fadeRoutine);
        }        
        _fadeRoutine = StartCoroutine(FadeRoutine(true));
    }

    public void FadeOut()
    {
        if (_fadeRoutine != null)
        {
            StopCoroutine(_fadeRoutine);
        }
        _fadeRoutine = StartCoroutine(FadeRoutine(false));
    }

    private IEnumerator FadeRoutine(bool fadeIn)
    {
        IsFading = true;
        float duration = _fadeTime;
        float timer = 0f;

        
        _fadePanel.blocksRaycasts = true;
        
        float startAlpha = fadeIn ? 1f : 0f;
        float endAlpha = fadeIn ? 0f : 1f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            _fadePanel.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / duration);
            yield return null;
        }
        
        _fadePanel.alpha = endAlpha;        
        _fadePanel.blocksRaycasts = (endAlpha == 1f);

        IsFading = false;
        _fadeRoutine = null;
    }

}
