using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField _schoolNameField;
    [SerializeField] private TMP_InputField _playerNameField;
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
        data.weekId = 1;

        gm.InitData(data);

        this.gameObject.SetActive(false);

        var weekId = GameManager.Instance.SaveData.weekId;
        CalendarManager.Instance.CalcWeek(weekId);
    }
}
