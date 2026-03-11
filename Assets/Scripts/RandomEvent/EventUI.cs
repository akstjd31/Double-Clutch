using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventUI : MonoBehaviour
{
    [Header("선수이미지 (좌측부터 0)")]
    [SerializeField] private Image[] _characterImage = new Image[3];

    [Header("말풍선 (위부터 0)")]
    [SerializeField] private TextMeshProUGUI[] _nameText = new TextMeshProUGUI[2];
    [SerializeField] private TextMeshProUGUI[] _printText = new TextMeshProUGUI[2];

    [Header("선택지 (위부터 0)")]
    [SerializeField] private GameObject _choicePanel;
    [SerializeField] private TextMeshProUGUI[] _choiceText = new TextMeshProUGUI[3];

    [Header("결과창")]
    [SerializeField] private GameObject _resultPanel;
    [SerializeField] private TextMeshProUGUI _state;
    [SerializeField] private TextMeshProUGUI _stat;
    [SerializeField] private TextMeshProUGUI _resultText;
    [SerializeField] private Image _resultImage;


    bool _isFirstText = true;

    private void OnEnable()
    {
        _isFirstText = true;
    }

    public void UpdateText(string name, string scriptText)
    {
        //이전 대화가 존재한다면 기존 내용을 아래 말풍선으로 내리고 
        if (!_isFirstText)
        {
            _nameText[1].text = _nameText[0].text;
            _printText[1].text = _printText[0].text;
        }
        
        //위쪽 말풍선 갱신
        _nameText[0].text = name;
        _printText[0].text = scriptText;
        _isFirstText = false;
    }

    public void UpdateImage(string speakDirection)
    {
        Color on = Color.white;

        Init();

        switch (speakDirection)
        {
            case "Left":
                _characterImage[0].color = on;
                _characterImage[0].transform.SetSiblingIndex(2);
                break;
            case "Middle":
                _characterImage[1].color = on;
                _characterImage[1].transform.SetSiblingIndex(2);
                break;
            case "Right":
                _characterImage[2].color = on;
                _characterImage[2].transform.SetSiblingIndex(2);
                break;
        }
    }

    private void Init()
    {
        Color off = new Color(128, 128, 128);

        //색 초기화
        for (int i = 0; i < 3; i++)
        {
            _characterImage[i].color = off;
        }
        //위치 초기화
        _characterImage[0].transform.SetSiblingIndex(0);
        _characterImage[1].transform.SetSiblingIndex(2);
        _characterImage[2].transform.SetSiblingIndex(1);

        //텍스트 비우기
        for (int i = 0; i < _nameText.Length; i++)
        {
            _nameText[i].text = "";
            _printText[i].text = "";
        }
    }

    public void UpdateChiceText(string text1, string text2, string text3)
    {
        //패널 띄우기
        _choicePanel.SetActive(true);

        //텍스트 출력하기
        _choiceText[0].text = text1;
        _choiceText[1].text = text2;
        _choiceText[2].text = text3;
    }

    public void EventResult()
    {
        //패널 띄우기
        _resultPanel.SetActive(true);
        _state.text = "";
        _stat.text = "";
        _resultText.text = "";
        //_resultText.sprite = ;
    }

    public void OnClickOk()
    {
        //다음 학생으로 넘어가기
        //마지막 학생이라면 로비로 가기=큐가 비었다면 로비로
    }
}
