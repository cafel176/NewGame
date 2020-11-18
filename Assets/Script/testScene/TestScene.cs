using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScene : MonoBehaviour {
    private GamePanelManager gamePanel;
    public GameObject[] triggers;
    public GameObject[] walls;
    public Transform[] birthpoints;
    public Player player;

    public AudioClip[] bgms;

    void Start()
    {
        gamePanel = gameManager.instance.getGamePanel().GetComponent<GamePanelManager>();
        gamePanel.showBattleBtn(player.gameObject);
        BattleManager.instance.setGamePanel(gamePanel);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Camera.main.GetComponent<CameraFollow>().Active = true;             
            StartCoroutine(startScene());
            this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    void Update () {
    }

    IEnumerator startScene()
    {
        gameManager.instance.beWhite(0.02f);
        yield return new WaitForSeconds(1);
    }

    public void triggerRun(int i)
    {
        switch (i)
        {
            case 0:StartCoroutine(event0());break;
            default:break;
        }
    }

    IEnumerator event0()
    {
        BattleManager.instance.setBirthPoint(birthpoints[0], triggers[0],walls[0]);
        BattleManager.instance.startBattle(bgms[0], 0, 10, 10,0);
        BattleManager.instance.addEnemy(1, new Vector2(5, 0), false);
        yield return new WaitForSeconds(5);
        BattleManager.instance.addEnemy(0, new Vector2(5, 0), true);
    }
}
