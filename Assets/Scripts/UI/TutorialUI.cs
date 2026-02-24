using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private InputField _schoolNameField;
    [SerializeField] private InputField _playerNameField;
    [SerializeField] private Button _confirmButton;

    private void Start()
    {
        if (_confirmButton != null)
            _confirmButton.onClick.AddListener(OnClickConfirmButton);
    }
    public void OnClickConfirmButton()
    {
        if (_schoolNameField.text == "" || _playerNameField.text == "") return;
        if (GameManager.Instance == null) return;

        var gm = GameManager.Instance;
        var data = new PlayerSaveData();

        data.schoolName = _schoolNameField.text;
        data.coachName = _playerNameField.text;

        gm.InitData(data);

        this.gameObject.SetActive(false);

        // 튜토리얼 수행 완료
        PlayerPrefs.SetInt(PrefKeys.KEY_FIRST_RUN_DONE, 1);
        PlayerPrefs.Save();
    }
}
