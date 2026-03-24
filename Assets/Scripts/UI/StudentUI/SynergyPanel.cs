using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class SynergyPanel : MonoBehaviour
{
    [Header("Data Readers")]
    [SerializeField] private Player_SynergyDataReader _synergyDataReader;
    [SerializeField] private Player_TraitDataReader _traitDataReader;

    [Header("UI References (Two-Page)")]
    [SerializeField] private Transform _leftContent;  // 왼쪽 페이지 부모
    [SerializeField] private Transform _rightContent; // 오른쪽 페이지 부모
    [SerializeField] private SynergyBox _synergyBoxPrefab;
    [SerializeField] private TextMeshProUGUI _txtPageInfo;
    [SerializeField] private Button _btnPrev;
    [SerializeField] private Button _btnNext;

    [Header("Paging Settings")]
    [SerializeField] private int _itemsPerPage = 5; // '한 페이지당' 개수 (화면엔 총 x2개 노출)

    private List<SynergyBox> _leftBoxes = new List<SynergyBox>();
    private List<SynergyBox> _rightBoxes = new List<SynergyBox>();
    private int _currentPageGroup = 0; // 0, 1, 2... (한 그룹당 2페이지씩 포함)
    private int _maxPageGroup = 0;

    private void Awake()
    {
        _btnPrev.onClick.AddListener(() => ChangePageGroup(-1));
        _btnNext.onClick.AddListener(() => ChangePageGroup(1));
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



    private void OnDestroy()
    {
        _btnPrev.onClick.RemoveAllListeners();
        _btnNext.onClick.RemoveAllListeners();
    }

    private void RefreshPanel()
    {
        var allData = _synergyDataReader.DataList;
        int displayCount = _itemsPerPage * 2; // 한 화면에 보일 총 개수

        _maxPageGroup = Mathf.CeilToInt((float)allData.Count / displayCount) - 1;
        _currentPageGroup = Mathf.Clamp(_currentPageGroup, 0, _maxPageGroup);

        // 1. 데이터 추출
        var groupData = allData.Skip(_currentPageGroup * displayCount).Take(displayCount).ToList();

        // 2. 좌측 페이지 갱신
        UpdatePageSide(_leftContent, _leftBoxes, groupData.Take(_itemsPerPage).ToList());

        // 3. 우측 페이지 갱신
        UpdatePageSide(_rightContent, _rightBoxes, groupData.Skip(_itemsPerPage).ToList());

        UpdateUI();
    }

    private void UpdatePageSide(Transform parent, List<SynergyBox> boxPool, List<PlayerSynergyData> dataList)
    {
        for (int i = 0; i < _itemsPerPage; i++)
        {
            // 풀 생성
            if (i >= boxPool.Count)
            {
                boxPool.Add(Instantiate(_synergyBoxPrefab, parent));
            }

            if (i < dataList.Count)
            {
                var synergy = dataList[i];
                Player_TraitData? t1 = _traitDataReader.DataList.Find(t => t.traitId == synergy.traitId1);
                Player_TraitData? t2 = _traitDataReader.DataList.Find(t => t.traitId == synergy.traitId2);

                if (t1 != null && t2 != null)
                {
                    boxPool[i].gameObject.SetActive(true);
                    boxPool[i].Init(synergy, t1.Value, t2.Value);
                }
            }
            else
            {
                boxPool[i].gameObject.SetActive(false);
            }
        }
    }

    private void ChangePageGroup(int direction)
    {
        _currentPageGroup += direction;
        RefreshPanel();
    }

    private void UpdateUI()
    {
        // 페이지 표시는 (현재그룹*2 + 1) ~ (현재그룹*2 + 2) 형태로 표시 가능
        int leftPageNum = (_currentPageGroup * 2) + 1;
        int rightPageNum = leftPageNum + 1;
        _txtPageInfo.text = $"{leftPageNum} - {rightPageNum}";

        _btnPrev.interactable = (_currentPageGroup > 0);
        _btnNext.interactable = (_currentPageGroup < _maxPageGroup);
    }
}