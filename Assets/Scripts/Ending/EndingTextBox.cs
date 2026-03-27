using TMPro;
using UnityEngine;

public class EndingTextBox : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] TextMeshProUGUI _contentText;

    string _currentNameKey;
    string _currentContentKey;

    private void Start()
    {
        StringManager.OnLanguageChanged += Refresh;
    }

    private void OnDestroy()
    {
        StringManager.OnLanguageChanged -= Refresh;
    }
    
    public void SetName(string nameKey)
    {
        if (nameKey == _currentNameKey)
        {
            return;
        }
        _currentNameKey = nameKey;

        StringManager stringManager = StringManager.Instance;

        _nameText.text = stringManager.GetString(_currentNameKey).Replace("{ME}", GameManager.Instance.SaveData.coachName);

        stringManager.ApplyFont(_nameText);
    }

    public void SetText(string contentKey)
    {        
        _currentContentKey = contentKey;

        StringManager stringManager =  StringManager.Instance;
        
        _contentText.text = stringManager.GetString(_currentContentKey);
        
        stringManager.ApplyFont(_contentText);
    }

    private void Refresh()
    {
        SetName(_currentNameKey);
        SetText(_currentContentKey);
    }
}
