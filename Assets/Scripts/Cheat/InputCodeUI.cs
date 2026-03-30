using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Game.Constants;

/// <summary>
/// 치트를 위해 만들어진 코드 입력 창 UI
/// </summary>
public class InputCodeUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Button _btn;
    [SerializeField] private GameObject _cheatPanelObj;

    private void OnEnable()
    {
        if (_btn == null) return;
        _btn.onClick.AddListener(OnClickConfirmButton);
    }

    private void OnDisable()
    {
        _btn.onClick.RemoveAllListeners();
    }

    public void OnClickConfirmButton()
    {
        if (GameManager.Instance == null) return;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySoundOneShot(SoundName.SE_BUTTON_SELECT);

        if (_inputField.text != "")
        {
            if (_inputField.text.Equals(GameManager.CHEAT_CODE))
            {
                if (_cheatPanelObj == null) return;

                _cheatPanelObj.SetActive(true);
            }
        }

        _inputField.text = "";
        this.gameObject.SetActive(false);
    }
}
