using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    private const string MAIN_SOUND_KEY = "BGM_Start";
    [SerializeField] private GameObject tutorialObj;
    [SerializeField] private Button _startButton;

    private void Awake()
    {
        if (_startButton != null)
            _startButton.onClick.AddListener(OnClickGameStart);
    }

    private void Start()
    {
        if (AudioManager.Instance == null) return;
        var mainClip = AudioManager.Instance.GetAudioClip(MAIN_SOUND_KEY);
        AudioManager.Instance.PlaySound(mainClip);
    }

    private void OnDestroy()
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.StopSound();
    }


    private void OnDisable()
    {
        if (_startButton != null)
            _startButton.onClick.RemoveListener(OnClickGameStart);
    }

    private void Update()
    {
        // 모바일 터치
        // if (Input.touchCount > 0)
        // {
        //     Touch touch = Input.GetTouch(0);

        //     if (touch.phase == TouchPhase.Began)
        //     {
        //         OnClickGameStart();
        //     }
        // }

// #if UNITY_EDITOR
//         // 테스트용 (PC)
//         if (Input.GetMouseButtonDown(0))
//         {
                // OnClickGameStart();
//         }
// #endif

        // 모바일 백 버튼 시 게임 종료
        if (Input.GetKeyDown(KeyCode.Escape))
            OnClickQuitButton();
    }

    public void OnClickGameStart() 
    {
        if (GameManager.Instance.SaveData == null)
        {
            tutorialObj.SetActive(true);
            return;
        }
        
        // if (!GameManager.Instance.SaveData.isTutorialCompleted)
        //     GameManager.Instance.Dispatch(UIAction.Tutorial);
        // else
        GameManager.Instance.Dispatch(UIAction.Main_Start); 
    }
    public void OnClickQuitButton() => GameManager.Instance.Dispatch(UIAction.Main_Quit);
}
