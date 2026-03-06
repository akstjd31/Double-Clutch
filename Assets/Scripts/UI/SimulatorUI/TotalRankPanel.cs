using UnityEngine;
using UnityEngine.UI;

public class TotalRankPanel : MonoBehaviour
{
    [SerializeField] private Button _btnClose;

    private void Awake()
    {
        // 닫기 버튼에 패널 비활성화 기능 연결
        if (_btnClose != null)
            _btnClose.onClick.AddListener(() => gameObject.SetActive(false));
    }

    public void OpenPanel()
    {
        gameObject.SetActive(true);
    }
}
