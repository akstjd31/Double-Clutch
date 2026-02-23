using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [SerializeField] private Button _startButton;

    private void Awake()
    {
        if (_startButton != null)
        {
            _startButton.onClick.AddListener(OnClickGameStart);
        }
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

    }

    public void OnClickGameStart() => GameManager.Instance.Dispatch(UIAction.Main_Start);
    public void OnClickQuitButton() => GameManager.Instance.Dispatch(UIAction.Main_Quit);
}
