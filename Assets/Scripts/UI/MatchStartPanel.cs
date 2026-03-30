using UnityEngine;
using Game.Constants;

public class MatchStartPanel : MonoBehaviour
{
    private void OnEnable()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySoundOneShot(SoundName.SE_MATCH_START);
    }
}
