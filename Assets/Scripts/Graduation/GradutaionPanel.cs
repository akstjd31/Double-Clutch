using UnityEngine;
using TMPro;


public class GradutaionPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _graduationText;
    [SerializeField] private GameObject _graduationListPanel;
    [SerializeField] private GraduationManager _graduationManager;


    private void Start()
    {
        GraduationText();
    }

    private void GraduationText()
    {
        if (_graduationManager.IsGraduationSkip)
        {
            _graduationText.text = "2월이 되었습니다. \n 팀 내에 졸업생이 없음 \n 진급페이지 이동";
        }
        else
        {
            _graduationText.text = "2월이 되었습니다. \n 졸업식을 시작하겠습니다.";
        }
    }

    public void OnclickNextButton()
    {
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
