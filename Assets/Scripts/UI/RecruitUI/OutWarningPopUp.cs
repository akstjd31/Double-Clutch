using TMPro;
using UnityEngine;
using Game.Constants;

public class OutWarningPopUp : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _warningText;

    private void OnEnable()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySoundOneShot(SoundName.UI_WARNING_01);
    }

    public void Init(int number)
    {
        _warningText.text = $"선수 {number} 명을 추가로 방출하셔야 합니다.";
    }
}
