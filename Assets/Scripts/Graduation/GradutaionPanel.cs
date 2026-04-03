using UnityEngine;
using TMPro;
using Game.Constants;


public class GradutaionPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _graduationText;
    [SerializeField] private GameObject _graduationListPanel;
    [SerializeField] private GraduationManager _graduationManager;
    [SerializeField] private GameObject _promotionPanel;


    private void Start()
    {
        GraduationText();
    }

    private void OnEnable()
    {
        StringManager.OnLanguageChanged += GraduationText;
    }

    private void OnDisable()
    {
        StringManager.OnLanguageChanged -= GraduationText;
    }

    private void GraduationText()
    {
        StringManager stringManager = StringManager.Instance;
        if (_graduationManager.IsGraduationSkip)
        {
            _graduationText.text = stringManager.GetString("UI_Graduation_졸업식팝업2");
        }
        else
        {
            _graduationText.text = stringManager.GetString("UI_Graduation_졸업식팝업") + "\n" + stringManager.GetString("UI_Graduation_졸업식팝업2");
        }
        stringManager.ApplyFont(_graduationText);
    }

    public void OnclickNextButton()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySoundOneShot(SoundName.SE_BUTTON_SELECT);

        if (_graduationManager.IsGraduationSkip)
        {
            gameObject.SetActive(false);
            _graduationManager.PromotionPanel.gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
            _graduationListPanel.SetActive(true);
        }
    }
}
