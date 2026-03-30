using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class EndingRollRow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _typeText;
    [SerializeField] TextMeshProUGUI _nameText;

    string _typeKey;
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
        _typeKey = typeKey;
        _nameKey = nameKey;
        _nameKeys = null;

        StringManager stringManager = StringManager.Instance;

        _typeText.text = stringManager.GetString(typeKey);
        _nameText.text = stringManager.GetString(nameKey);

        stringManager.ApplyFont(_typeText);
        stringManager.ApplyFont(_nameText);
    }

    public void Init(string typeKey, string[] nameKey) //학생용
    {
        _typeKey = typeKey;
        _nameKey = null;
        _nameKeys = nameKey;

        StringManager stringManager = StringManager.Instance;

        _typeText.text = stringManager.GetString(typeKey);
        _nameText.text = stringManager.GetString(nameKey[0]) + stringManager.GetString(nameKey[1]) + stringManager.GetString(nameKey[2]);
        stringManager.ApplyFont(_typeText);
        stringManager.ApplyFont(_nameText);
    }
    public void Init(string content) //소제목, 빈칸 용(나중에 스트링 테이블 해주기)
    {
        _typeText.gameObject.SetActive(false);

        if (!string.IsNullOrEmpty(content))
        {
            _nameText.text = content;            
        }
        else
        {
            _nameText.gameObject.SetActive(false);
        }
    }

    public void Init()
    {
        _typeText.text = string.Empty;
        _nameText.text = string.Empty;
    }

    private void Refresh()
    {
        if (_nameKey != null) //스태프라면
        {
            Init(_typeKey, _nameKey);
        }
        else
        {
            Init(_typeKey, _nameKeys);
        }
    }
}
