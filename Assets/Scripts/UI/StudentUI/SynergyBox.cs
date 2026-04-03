using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SynergyBox : MonoBehaviour
{
    [SerializeField] Image _synergy;
    [SerializeField] Image _trait1;
    [SerializeField] Image _trait2;
    [SerializeField] TextMeshProUGUI _synergyName;
    [SerializeField] TextMeshProUGUI _synergyDesc;
    public void Init(PlayerSynergyData synergyData, Player_TraitData trait1, Player_TraitData trait2)
    {
        SpriteManager spriteManager = SpriteManager.Instance;
        StringManager stringManager = StringManager.Instance;

        _synergy.sprite = spriteManager.GetSprite(synergyData.synergyResource);        

        _trait1.sprite = spriteManager.GetSprite(trait1.traitResource);
        _trait2.sprite = spriteManager.GetSprite(trait2.traitResource);

        _synergyName.text = stringManager.GetString(synergyData.synergyName);
        _synergyDesc.text = stringManager.GetString(synergyData.synergyDesc).Replace("{effectValue}", synergyData.effectValue.ToString());

        stringManager.ApplyFont(_synergyName);
        stringManager.ApplyFont(_synergyDesc);
    }
}
