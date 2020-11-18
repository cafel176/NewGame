using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour {

    //子类需要重写的属性
    private GameObject player;       //玩家

    protected GamePanelManager gamePanel;   //游戏面板，需要从游戏管理器里获取
    protected bool inTrigger = false;

    void Start() {
        gamePanel = gameManager.instance.getGamePanel().GetComponent<GamePanelManager>();
    }

    private void FixedUpdate()
    {
        player = gamePanel.player.gameObject;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
            enter();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
            stay();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
            exit();
    }

    //触发后的效果，子类里面覆盖
    public virtual void enter() { }

    public virtual void stay() { }

    public virtual void exit() { }
}
