using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class challenge1Trigger : Trigger
{
    public challengeManager1 sceneManager;
    public int i;
    // Use this for initialization
    void Start()
    {
        gamePanel = gameManager.instance.getGamePanel().GetComponent<GamePanelManager>();
    }

    public override void enter()
    {
        sceneManager.triggerRun(i);
    }
}
