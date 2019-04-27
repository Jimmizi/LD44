using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade
{
    public GameObject upgradeObject;
    public int stage;

    public Upgrade(GameObject upgradeObject, int stage)
    {
        this.upgradeObject = upgradeObject;
        this.stage = stage;
    }
}