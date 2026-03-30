using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class EndingRollRow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] TextMeshProUGUI _positionText;
    [SerializeField] TextMeshProUGUI _typeText;
    

    string _typeKey;
    int _classNum;
    string _position;
    string _nameKey;
    string[] _nameKeys;

    private void OnEnable()
    {
        StringManager.OnLanguageChanged += Refresh;
    }

    private void OnDisable()
    {
        StringManager.OnLanguageChanged -= Refresh;
    }
    public void Init(string typeKey, string nameKey) //스태프용
    {
        _positionText.gameObject.SetActive(false);
        _typeKey = typeKey;
        _nameKey = nameKey;
        _nameKeys = null;

        StringManager stringManager = StringManager.Instance;

        _typeText.text = stringManager.GetString(typeKey);
        _nameText.text = stringManager.GetString(nameKey);

        stringManager.ApplyFont(_typeText);
        stringManager.ApplyFont(_nameText);
    }

    public void Init(int classNum, string position, string[] nameKey) //학생용
    {
        _typeKey = null;
        _nameKey = null;
        _classNum = classNum;
        _nameKeys = nameKey;
        _position = position;

        StringManager stringManager = StringManager.Instance;

        if (classNum == -1)
        {
            _typeText.text = stringManager.GetString("Str_End_Player_02");
        }
        else
        {
            _typeText.text = classNum.ToString()+stringManager.GetString("Str_End_Player_01");
        }

        _positionText.text = position;
        _nameText.text = stringManager.GetString(nameKey[0]) + stringManager.GetString(nameKey[1]) + stringManager.GetString(nameKey[2]);
        stringManager.ApplyFont(_typeText);        
        stringManager.ApplyFont(_nameText);
    }
    public void Init(string content) //소제목용(나중에 스트링 테이블 해주기)
    {
        _typeText.gameObject.SetActive(false);
        _nameText.gameObject.SetActive(false);

        if (!string.IsNullOrEmpty(content))
        {
            _positionText.text = content;            
        }
        else
        {
            _positionText.gameObject.SetActive(false);
        }
    }

    public void Init() //빈칸용
    {
        _typeText.text = string.Empty;
        _nameText.text = string.Empty;
        _positionText.text = string.Empty;
    }

    private void Refresh()
    {
        if (_nameKey != null) //스태프라면
        {
            Init(_typeKey, _nameKey);
        }
        else
        {
            Init(_classNum, _position,_nameKeys);
        }
    }
}
