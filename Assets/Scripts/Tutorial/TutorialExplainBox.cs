using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TutorialExplainBox : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> _textList;
    
    private void OnEnable()
    {
        var strMgr = StringManager.Instance;
        if (strMgr == null) return;
        foreach (var t in _textList)
            t.text = strMgr.GetString(t.text);
    }
}
