using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour
{
    [SerializeField] private string _stringKey;

    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        StringManager.OnLanguageChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        StringManager.OnLanguageChanged -= Refresh;
    }

    private void Refresh()
    {
        if (StringManager.Instance == null) return;
        _text.text = StringManager.Instance.GetString(_stringKey);
    }
}
