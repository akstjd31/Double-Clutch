using System;
[Serializable]
public struct Player_VisualData
{
    public string visualId;
    public string speciesId;
    public string desc;
    public string playerImageResource, playerImageResourceAngry, playerImageResourceAnnoyed, playerImageResourceGaki,
        playerImageResourceCringe, playerImageResourceDisappointed, playerImageResourceEmbarrassed, playerImageResourceFrustrated,
        playerImageResourceHappy, playerImageResourceQuestioning, playerImageResourceRolling, playerImageResourceSad, playerImageResourceSerious,
        playerImageResourceSurprised, playerImageResourceTalking, playerImageResourceThinking, portraitResource, playerCutInResourceId01, playerCutInResourceId02, playerCutInResourceId03;

    public Player_VisualData(string visualId, string speciesId, string desc,string playerImageResource, string playerImageResourceAngry, string playerImageResourceAnnoyed, string playerImageResourceGaki,
        string playerImageResourceCringe, string playerImageResourceDisappointed, string playerImageResourceEmbarrassed, string playerImageResourceFrustrated,
        string playerImageResourceHappy, string playerImageResourceQuestioning, string playerImageResourceRolling, string playerImageResourceSad, string playerImageResourceSerious,
        string playerImageResourceSurprised, string playerImageResourceTalking, string playerImageResourceThinking, string portraitResource, string playerCutInResourceId01, string playerCutInResourceId02, string playerCutInResourceId03)
    {
        this.visualId = visualId;
        this.speciesId = speciesId;
        this.desc = desc;
        this.playerImageResource = playerImageResource;
        this.playerImageResourceAngry = playerImageResourceAngry;
        this.playerImageResourceAnnoyed = playerImageResourceAnnoyed;
        this.playerImageResourceGaki = playerImageResourceGaki;
        this.playerImageResourceCringe = playerImageResourceCringe;
        this.playerImageResourceDisappointed = playerImageResourceDisappointed;
        this.playerImageResourceEmbarrassed = playerImageResourceEmbarrassed;
        this.playerImageResourceFrustrated = playerImageResourceFrustrated;
        this.playerImageResourceHappy = playerImageResourceHappy;
        this.playerImageResourceQuestioning = playerImageResourceQuestioning;
        this.playerImageResourceRolling = playerImageResourceRolling;
        this.playerImageResourceSad = playerImageResourceSad;
        this.playerImageResourceSerious = playerImageResourceSerious;
        this.playerImageResourceSurprised = playerImageResourceSurprised;
        this.playerImageResourceTalking = playerImageResourceTalking;
        this.playerImageResourceThinking = playerImageResourceThinking;
        this.portraitResource = portraitResource;
        this.playerCutInResourceId01 = playerCutInResourceId01;
        this.playerCutInResourceId02 = playerCutInResourceId02;
        this.playerCutInResourceId03 = playerCutInResourceId03;
    }
}
