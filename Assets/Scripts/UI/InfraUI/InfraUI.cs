using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Game.Constants;

public class InfraUI : MonoBehaviour
{
    [Header("UpgradePanel")]
    [SerializeField] private InfraUpgradeUI _infraUpgradeUI;

    private void OnEnable()
    {
        PlaySound(SoundName.BGM_INFRA);
    }

    private void OnDisable()
    {
        PlaySound(SoundName.BGM_LOBBY_01);
    }

    private void PlaySound(string id)
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.PlaySound(id);
    }

    public void SetInfraUpgradePanelUI(InfraController Controller, Infra infra)
    {
        _infraUpgradeUI.gameObject.SetActive(true);
        _infraUpgradeUI.Init(Controller, infra);
    }
}
