using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour
{
    [SerializeField] private string _stringKey;

    [Header("Font Size Multiplier (1 = 기본값 유지)")]
    [SerializeField] private float _koMultiplier = 1f;
    [SerializeField] private float _enMultiplier = 1f;
    [SerializeField] private float _jaMultiplier = 1f;

    private TextMeshProUGUI _text;
    private float _defaultFontSize;
    private TMP_FontAsset _defaultFont;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _defaultFontSize = _text.fontSize;
        _defaultFont = _text.font;
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
        ApplyFont();
        ApplyFontSize();
    }
    private void ApplyFont()
    {
        var font = StringManager.Instance.GetFont();
        _text.font = font != null ? font : _defaultFont;
    }

    private void ApplyFontSize()
    {
        float multiplier = StringManager.Instance.CurrentLanguage switch
        {
            Language.En => _enMultiplier,
            Language.Ja => _jaMultiplier,
            _ => _koMultiplier,
        };

        _text.fontSize = _defaultFontSize * multiplier;
    }
}
