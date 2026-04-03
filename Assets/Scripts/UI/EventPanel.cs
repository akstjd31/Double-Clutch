using UnityEngine;
using Game.Constants;

public class EventPanel : MonoBehaviour
{
    private void OnEnable()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySoundOneShot(SoundName.SE_EVENT_TRIGGER);
        }
        // PlaySound(SoundName.BGM_RANDOM_EVENT);
    }

    private void OnDisable()
    {
        // PlaySound(SoundName.BGM_LOBBY_01);
    }
}
