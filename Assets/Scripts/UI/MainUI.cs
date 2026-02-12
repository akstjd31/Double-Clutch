using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [SerializeField] private Button _gameStartBtn;
    [SerializeField] private Button _quitBtn;

    private void Awake()
    {
        if (_gameStartBtn != null)
            _gameStartBtn.onClick.AddListener(OnClickGameStartButton);
        
        if (_quitBtn != null)
            _quitBtn.onClick.AddListener(OnClickQuitButton);
    }

    public void OnClickGameStartButton() => GameManager.Instance.Dispatch(UIAction.Main_Start);
    public void OnClickQuitButton() => GameManager.Instance.Dispatch(UIAction.Main_Quit);
}
