using UnityEngine;
using System;
[Serializable]
public struct ProfileData
{
    public string profileId;
    public string visualId;
    public int unlockCount;
    public bool isDefault;
    public string playerImage;

    public ProfileData(string profileId, string visualId, int unlockCount, bool isDefault, string playerImage)
    {
        this.profileId = profileId;
        this.visualId = visualId;
        this.unlockCount = unlockCount;
        this.isDefault = isDefault;
        this.playerImage = playerImage;
    }
}
