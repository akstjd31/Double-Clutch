using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [SerializeField] private GameObject tutorialObj;
    [SerializeField] private Button _startButton;

    private void Awake()
    {
        if (_startButton != null)
            _startButton.onClick.AddListener(OnClickGameStart);
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

    // 처음 실행하는건지?
    private bool IsFirstRun() => PlayerPrefs.GetInt(PrefKeys.KEY_FIRST_RUN_DONE, 0) == 0;

    public void OnClickGameStart() 
    {
        if (IsFirstRun())
        {
            tutorialObj.SetActive(true);
        }
        else
        {
            GameManager.Instance.Dispatch(UIAction.Main_Start); 
        }
    }
    public void OnClickQuitButton() => GameManager.Instance.Dispatch(UIAction.Main_Quit);
}
