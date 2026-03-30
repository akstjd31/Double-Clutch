using System.Collections;
using Game.Constants;
using UnityEngine;
using UnityEngine.UI;

public class EndingUIController : MonoBehaviour
{
    [SerializeField] Image _backgroundImage;
    [SerializeField] Image _characterImage;
    [SerializeField] EndingTextBox _endingTextBox;
    [SerializeField] CanvasGroup _fadePanel;
    [SerializeField] GameObject _andYouPanel;
    CanvasGroup _characterCanvasGroup;

    public bool IsFading { get; private set; } = false;

    private Coroutine _fadeRoutine;

    private void Awake()
    {
        _characterCanvasGroup = _characterImage.GetComponent<CanvasGroup>();
    }

    string _currentBackgroundKey = string.Empty;
    string _currentCharacterKey = string.Empty;
    string _currentBgmKey = string.Empty;    

    public void SetBackgroundImage(string imageKey)
    {
        if (_currentBackgroundKey != imageKey)
        {
            _backgroundImage.sprite = SpriteManager.Instance.GetSprite(imageKey);
        }        
    }

    public void PlayBGM(string bgmKey)
    {
        if (string.IsNullOrEmpty(bgmKey))
        {
            _currentBgmKey = bgmKey;
            return;
        }

        if (_currentBgmKey != bgmKey)
        {
            _currentBgmKey = bgmKey;
            
            //새 BGM 재생
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySoundOneShot(SoundName.SE_MATCH_WIN);
        }
    }
    public void PlaySFX(string sfxKey)
    {
        if (string.IsNullOrEmpty(sfxKey))
        {            
            return;
        }

        //효과음 재생
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySound(SoundName.BGM_ENDING);
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

    public void PlayAndYouSequence()
    {
        StartCoroutine(AndYouRoutine());
    }

    private IEnumerator AndYouRoutine()
    {        
        _andYouPanel.SetActive(true);
        _fadePanel.alpha = 1f;
        _fadePanel.blocksRaycasts = true;

        
        yield return StartCoroutine(FadeRoutine(true));

        GameManager.Instance.SetEnding();
        yield return new WaitForSeconds(EndingManager.Instance.AndYouTime);
        

        yield return StartCoroutine(FadeRoutine(false));

        
        GameManager.Instance.GoToLobby();
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
        float duration = EndingManager.Instance.FadeTime;
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
        _fadePanel.blocksRaycasts = false;

        IsFading = false;
        _fadeRoutine = null;
    }

}
