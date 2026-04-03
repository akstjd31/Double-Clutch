using UnityEngine;

public class SeasonOutChecker : MonoBehaviour
{
    [SerializeField] private GameObject _popupObj;

    public void SetPopupActivate(bool active)
    {
        if (_popupObj == null) return;
        _popupObj.SetActive(active);

        _popupObj.GetComponent<SeasonOutUI>().Init();
    }
}
