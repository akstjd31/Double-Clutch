using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SynergyPanel : MonoBehaviour
{
    [Header("Data Readers")]
    [SerializeField] private Player_SynergyDataReader _synergyDataReader;
    [SerializeField] private Player_TraitDataReader _traitDataReader;

    [Header("UI References (Single-Page)")]
    [SerializeField] private Transform _content;      // 이제 하나만 사용
    [SerializeField] private SynergyBox _synergyBoxPrefab;
    [SerializeField] private TextMeshProUGUI _txtPageInfo;
    [SerializeField] private Button _btnPrev;
    [SerializeField] private Button _btnNext;

    [Header("Paging Settings")]
    [SerializeField] private int _itemsPerPage = 6;

    private List<SynergyBox> _boxPool = new List<SynergyBox>(); // 통합된 풀
    private int _currentPage = 0;
    private int _maxPage = 0;

    private void Awake()
    {
        _btnPrev.onClick.AddListener(() => ChangePage(-1));
        _btnNext.onClick.AddListener(() => ChangePage(1));
    }

    private void OnEnable()
    {
        StringManager.OnLanguageChanged += RefreshPanel;
        RefreshPanel();
    }

    private void OnDisable()
    {
        StringManager.OnLanguageChanged -= RefreshPanel;
    }

    private void RefreshPanel()
    {
        var allData = _synergyDataReader.DataList;

        // 전체 페이지 수 계산
        _maxPage = Mathf.Max(0, Mathf.CeilToInt((float)allData.Count / _itemsPerPage) - 1);
        _currentPage = Mathf.Clamp(_currentPage, 0, _maxPage);

        // 현재 페이지에 해당하는 데이터 추출
        var pageData = allData.Skip(_currentPage * _itemsPerPage).Take(_itemsPerPage).ToList();

        // 페이지 갱신
        UpdatePage(_content, _boxPool, pageData);

        UpdateUI();
    }

    private void UpdatePage(Transform parent, List<SynergyBox> boxPool, List<PlayerSynergyData> dataList)
    {        
        while (boxPool.Count < _itemsPerPage)
        {
            boxPool.Add(Instantiate(_synergyBoxPrefab, parent));
        }
        
        for (int i = 0; i < _itemsPerPage; i++)
        {
            if (i < dataList.Count)
            {
                var synergy = dataList[i];
                
                var t1 = _traitDataReader.DataList.Find(t => t.traitId == synergy.traitId1);
                var t2 = _traitDataReader.DataList.Find(t => t.traitId == synergy.traitId2);

                boxPool[i].gameObject.SetActive(true);
                boxPool[i].Init(synergy, t1, t2);
            }
            else
            {
                boxPool[i].gameObject.SetActive(false);
            }
        }
    }

    private void ChangePage(int direction)
    {
        _currentPage += direction;
        RefreshPanel();
    }

    private void UpdateUI()
    {        
        _txtPageInfo.text = $"{_currentPage + 1} / {_maxPage + 1}";

        _btnPrev.interactable = (_currentPage > 0);
        _btnNext.interactable = (_currentPage < _maxPage);
    }

    private void OnDestroy()
    {
        _btnPrev.onClick.RemoveAllListeners();
        _btnNext.onClick.RemoveAllListeners();
    }
}