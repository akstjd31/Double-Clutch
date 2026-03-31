using System.Collections.Generic;
using Game.Constants;
using TMPro;
using UnityEngine;

using UnityEngine.UI;

public class EventUI : MonoBehaviour
{
    [Header("선수이미지 (좌측부터 0)")]
    [SerializeField] private Image[] _characterImage = new Image[3];

    [Header("말풍선")]
    [SerializeField] private GameObject _bubblePrefab;
    [SerializeField] private GameObject _bubbleParantPrefab;
    [SerializeField] private List<GameObject> _bubbleList;

    [Header("선택지 (위부터 0)")]
    [SerializeField] private GameObject _choicePanel;
    [SerializeField] private TextMeshProUGUI[] _choiceText = new TextMeshProUGUI[3];

    [Header("결과창")]
    [SerializeField] private GameObject _resultPanel;
    [SerializeField] private TextMeshProUGUI _state;
    [SerializeField] private TextMeshProUGUI _stat;
    [SerializeField] private TextMeshProUGUI _resultText;
    [SerializeField] private Image _resultImage;


    //bool _isFirstText = true;
    int _textTurn;
    string beforeDirection = null;

    //Color initColor;

    public void UIStart()
    {
        //_isFirstText = true;
        _textTurn = 0;
        ResultInit();
        TextInit();
        ImageInit();
    }

    public void UpdateText(string name, string scriptText, string speakDirection, bool isNameTagOn, string characterImage)
    {
        TextBubbleScript textBubbleScript;
        GameObject textBubble;

        //말풍선이 턴수보다 부족하면 생성
        if (_bubbleList.Count < _textTurn)
        {
            textBubble = Instantiate(_bubblePrefab, _bubbleParantPrefab.transform);
            //리스트에 추가
            _bubbleList.Add(textBubble);
        }

        //리스트의 말풍선, 스크립트 가져오기
        textBubble = _bubbleList[_textTurn];
        textBubbleScript = textBubble.GetComponent<TextBubbleScript>();
        var nameTag = textBubble.transform.GetChild(0).gameObject;

        nameTag.SetActive(isNameTagOn);

        //맨 앞으로 이동시킨 뒤 활성화
        textBubble.transform.SetSiblingIndex(0);
        textBubble.SetActive(true);

        textBubbleScript.NameText.text = name;
        textBubbleScript.PrintText.text = scriptText;

        if (_textTurn > 0)
        {
            //이전 말풍선 어둡게 처리
            var beforeBubble = _bubbleList[_textTurn - 1].gameObject.GetComponent<Image>();
            var beforenameTag = beforeBubble.transform.GetChild(0).gameObject.GetComponent<Image>(); ;
            var continueIcon = beforeBubble.transform.GetChild(2).gameObject.GetComponent<Image>(); ;

            Color _dim = new Color(0.6f, 0.6f, 0.6f, 1f);
            beforenameTag.color = _dim;
            beforeBubble.color = _dim;
            continueIcon.color = _dim;
        }

        UpdateImage(speakDirection, characterImage);
        _textTurn++;
    }

    public void UpdateImage(string speakDirection, string characterImageId)
    {
        Debug.Log($"스피커 : {speakDirection}");

        if (string.IsNullOrEmpty(characterImageId))
        {
            Debug.Log($"{characterImageId} : 이미지 파일 없는 파트");
            return;
        }

        ImageInit();

        Debug.Log($"선수 이미지 id : {characterImageId}");

        Color on = Color.white;
        on.a = 1f;
        Color _dim = new Color(0.6f, 0.6f, 0.6f, 1f);

        switch (speakDirection)
        {
            case "Left":
                _characterImage[0].color = on;
                _characterImage[0].sprite = SpriteManager.Instance.GetSprite(characterImageId);
                _characterImage[0].transform.SetSiblingIndex(2);
                beforeDirection = speakDirection;
                break;
            case "Middle":
                _characterImage[1].color = on;
                _characterImage[1].sprite = SpriteManager.Instance.GetSprite(characterImageId);
                _characterImage[1].transform.SetSiblingIndex(2);
                beforeDirection = speakDirection;
                break;
            case "Right":
                _characterImage[2].color = on;
                _characterImage[2].sprite = SpriteManager.Instance.GetSprite(characterImageId);
                _characterImage[2].transform.SetSiblingIndex(2);
                beforeDirection = speakDirection;
                break;
            default:
                Debug.Log($"이미지 갱신 없음 : {_dim.a}");

                if (beforeDirection == null)
                {
                    break;
                }
                else if (beforeDirection == "Left")
                {
                    _characterImage[0].color = _dim;

                }
                else if (beforeDirection == "Middle")
                {
                    _characterImage[1].color = _dim;
                }
                else if (beforeDirection == "Right")
                {
                    _characterImage[2].color = _dim;

                }
                break;
        }
    }

    public void ImageInit()
    {
        Color _dim = new Color(0.6f, 0.6f, 0.6f, 0f);
        //색 초기화
        for (int i = 0; i < _characterImage.Length; i++)
        {
            _characterImage[i].color = _dim;
        }
        //위치 초기화
        _characterImage[0].transform.SetSiblingIndex(0);
        _characterImage[1].transform.SetSiblingIndex(2);
        _characterImage[2].transform.SetSiblingIndex(1);
    }

    public void TextInit()
    {
        //텍스트 비우기
        _textTurn = 0;
        for (int i = 0; i < _bubbleList.Count; i++)
        {
            _bubbleList[i].SetActive(false);
            //색 초기화
            var beforeBubble = _bubbleList[i].GetComponent<Image>();
            var beforenameTag = beforeBubble.transform.GetChild(0).gameObject.GetComponent<Image>();
            var continueIcon = beforeBubble.transform.GetChild(2).gameObject.GetComponent<Image>(); ;

            beforeBubble.color = Color.white;
            beforenameTag.color = Color.white;
            continueIcon.color = Color.white;
        }
    }

    public void UpdateChiceText(string text1, string text2, string text3)
    {
        //패널 띄우기
        _choicePanel.SetActive(true);

        //텍스트 출력하기
        _choiceText[0].text = text1;
        StringManager.Instance.ApplyFont(_choiceText[0]);
        _choiceText[1].text = text2;
        StringManager.Instance.ApplyFont(_choiceText[1]);
        _choiceText[2].text = text3;
        StringManager.Instance.ApplyFont(_choiceText[2]);
    }

    private void ResultInit()
    {
        if (_stat == null) return;
        _stat.text = "";
        _stat.color = Color.black;

        if (_state == null) return;
        _state.text = "";
        _state.color = Color.black;
    }


    public void UpdateEventResult(potential potentialChangeType, int potentialChangeValue, string resultScriptKey, string reactionPortraitId, string currentState, string statusChange)
    {
        string nm = "";

        if (reactionPortraitId.Contains("Normal"))
            nm = SoundName.SE_NONE;
        
        else if (reactionPortraitId.Contains("Sad"))
            nm = SoundName.SE_FAIL;
        
        else
            nm = SoundName.SE_GREAT;       

        #region 스텟한글변환
        string transText = "";
        switch (potentialChangeType)
        {
            case potential.None:
                break;
            case potential.Stat2pt:
            case potential.Stat3pt:
                transText = "득점";
                break;
            case potential.StatPass:
            case potential.StatRebound:
                transText = "지원";
                break;
            case potential.StatSteal:
            case potential.StatBlock:
                transText = "저지";
                break;
        }
        #endregion

        //득점지원저지 증감 텍스트
        if (potentialChangeValue < 0)
        {
            _stat.text = transText + "↓";
            _stat.color = new Color(0.9f, 0.3f, 0.3f, 1f);

        }
        else if (potentialChangeValue == 0)
        {
            _stat.text = "";
        }
        else
        {
            _stat.text = transText + "↑";
            _stat.color = new Color(0.2f, 0.8f, 0.4f, 1f);
        }
        //득점지원저지 위치
        var rect = _stat.rectTransform;
        var pos = rect.anchoredPosition;
        pos.y = Random.Range(-45f, 45f);
        rect.anchoredPosition = pos;

            Debug.LogWarning($"아이디 : {reactionPortraitId} ");
        //이미지 넣기
        if (string.IsNullOrEmpty(reactionPortraitId))
        {
            Debug.LogWarning($"불러오기 실패 : {reactionPortraitId} ");
        }
        else
        {
            _resultImage.sprite = SpriteManager.Instance.GetSprite(reactionPortraitId);
        }


        //결과 텍스트 출력
        _resultText.text = resultScriptKey;

        //패널 띄우기
        _resultPanel.SetActive(true);

        UpdateState(currentState, statusChange, nm);//상태변경
    }

    public void UpdateState(string currentState, string statusChange, string soundName)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySoundOneShot(soundName); 

        var rect = _state.rectTransform;
        var pos = rect.anchoredPosition;
        pos.y = Random.Range(-45f, 45f);
        rect.anchoredPosition = pos;


        //변화 없으면 넘어감
        if (statusChange == StudentState.None.ToString())
        {
            _state.text = "";
        }
        //변화 있으면 상태 변경 띄워줌
        else
        {
            //이전 상태가 없었으면
            if (statusChange == StudentState.OverWorked.ToString())
            {
                _state.text = "과로 획득";
                _state.color = new Color(0.9f, 0.3f, 0.3f, 1f);
            }
            else if (statusChange == StudentState.Injured.ToString())
            {
                _state.text = "부상 획득";
                _state.color = new Color(0.9f, 0.3f, 0.3f, 1f);
            }
        }
        #region 회복 포함 코드
        ////과거상태 = 변화상태 같으면 넘어감
        //if (currentState == lastState)
        //{
        //    _state.text = "";
        //}
        ////다르면 따로 판단.
        //else
        //{
        //    //이전 상태가 없었으면
        //    if (lastState == StudentState.None.ToString())
        //    {
        //        if (currentState == StudentState.OverWorked.ToString())
        //        {
        //            _state.text = "과로 획득";
        //            _state.color = new Color(0.9f, 0.3f, 0.3f, 1f);
        //        }
        //        else if (currentState == StudentState.Injured.ToString())
        //        {
        //            _state.text = "부상 획득";
        //            _state.color = new Color(0.9f, 0.3f, 0.3f, 1f);
        //        }
        //    }
        //    //이전 상태가 과로
        //    else if (lastState == StudentState.OverWorked.ToString())
        //    {
        //        if (currentState == StudentState.None.ToString())
        //        {
        //            _state.text = "과로 회복";
        //            _state.color = new Color(0.2f, 0.8f, 0.4f, 1f);
        //        }
        //    }

        //    //이전 상태가 부상
        //    else if (lastState == StudentState.Injured.ToString())
        //    {
        //        if (currentState == StudentState.None.ToString())
        //        {
        //            _state.text = "부상 회복";
        //            _state.color = new Color(0.2f, 0.8f, 0.4f, 1f);
        //        }
        //    }
        #endregion
    }
}

