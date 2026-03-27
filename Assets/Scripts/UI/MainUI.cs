using UnityEngine;
using UnityEngine.UI;
using Game.Constants;

public class MainUI : MonoBehaviour
{
    [SerializeField] private GameObject tutorialObj;
    [SerializeField] private Button _startButton;

    private void Awake()
    {
        if (_startButton != null)
            _startButton.onClick.AddListener(OnClickGameStart);
    }

    private void Start()
    {
        PlaySound();
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

    private void PlaySound()
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.PlaySound(SoundName.BGM_START);
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
