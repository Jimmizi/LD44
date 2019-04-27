using UnityEngine;

public class Upgrade
{
    public GameObject upgradeObject;

    public int stage = 1;
    public int temporaryStage = 0;

    public int CurrentStage
    {
        get { return Mathf.Max(stage, temporaryStage); }
    }
}