using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene6Trigger : Trigger
{
    public Scene6Manager sceneManager;
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
