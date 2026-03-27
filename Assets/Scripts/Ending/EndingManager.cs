using UnityEngine;
using UnityEngine.UI;

public class EndingManager : MonoBehaviour
{
    [SerializeField] EndingEventDataReader _endingEventDataReader;
    [SerializeField] ScriptDataReader _endingScriptDataReader;    
    [SerializeField] EndingRollDataReader _endingRollDataReader;
    [SerializeField] EndingUIController _uiController;
    [SerializeField] Button _nextButton;
    public static EndingManager Instance;

    private int _currentIndex = 0;
    private ScriptData _currentScript;

    private void Awake()
    {
        Instance = this;
        
    }

    public void PlayEndingEvent()
    {
        _currentScript = _endingScriptDataReader.DataList[_currentIndex];

        CheckFade();
        _uiController.SetBackgroundImage(_currentScript.background);
        _uiController.SetCharacterImage(_currentScript.portrait);
        _uiController.SetText(_currentScript.name, _currentScript.text);
    }    

    public void OnNextButtonClick()
    {
        if (_uiController.IsFading) //페이드 연출 중에는 버튼 눌러도 반응 x (어차피 Fade 패널로 눌리지도 않음)
        {
            return;
        }

        if (_endingScriptDataReader.DataList[_currentIndex].scriptType == 1)
        {
            //이벤트 종료 후 엔딩롤로 전환
            return;
        }
        _currentIndex++;        
        PlayEndingEvent();
    }

    private void CheckFade()
    {
        switch (_currentScript.startEffectId)
        {
            case "Fade_In": 
                _uiController.FadeIn();
                break;
            case "Fade_Out":
                _uiController.FadeOut();
                break;
            default:
                Debug.Log("Script 테이블의 startEffectId 작성 양식이 Fade_In , Fade_Out 으로 되어 있는지 확인하세요.");
                break;
        }        
    }

}
