﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class challengeManager4 : MonoBehaviour {

    [HideInInspector]
    public GamePanelManager gamePanel;
    public Player player;
    public Transform[] checkPoint;
    public GameObject[] triggers;
    public GameObject[] walls;
    public GameObject trigger6;

    public Box itemBox;
    public GameObject hpBox;
    public GameObject[] items;

    //剧情用变量
    public Sprite CG1;
    public Sprite chapter4;
    public Sprite face1;
    public Sprite face2;
    public Enemy enemy1;
    public Enemy enemy2;
    private bool playerMove = false;
    private float move = 0;
    private bool moveCamera = false;
    private Vector3 movePos;
    private bool autoEvent = false;

    public AudioClip changeChapter;
    public AudioClip BGM;
    public AudioClip BGM2;

    void Start()
    {
        gamePanel = gameManager.instance.getGamePanel().GetComponent<GamePanelManager>();
        gameManager.instance.beBlack(1);
        BattleManager.instance.setGamePanel(gamePanel);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (gameManager.instance.now == 0)
                StartCoroutine(event0());
            else if (gameManager.instance.now == 4)
            {
                Camera.main.GetComponent<CameraFollow>().Active = true;
                gamePanel.showBattleBtn(player.gameObject);
                BattleManager.instance.startLevel();
                player.transform.position = checkPoint[1].position;
                gameManager.instance.beWhite(0.02f);
            }
            else if (gameManager.instance.now == 5)
            {
                Camera.main.GetComponent<CameraFollow>().Active = true;
                gamePanel.showBattleBtn(player.gameObject);
                BattleManager.instance.startLevel();
                player.transform.position = checkPoint[2].position;
                gameManager.instance.beWhite(0.02f);
            }
            else if (gameManager.instance.now == 6)
            {
                Camera.main.GetComponent<CameraFollow>().Active = true;
                gamePanel.showBattleBtn(player.gameObject);
                BattleManager.instance.startLevel();
                player.transform.position = checkPoint[3].position;
                gameManager.instance.beWhite(0.02f);
            }
            else if (gameManager.instance.now == 7)
            {
                Camera.main.GetComponent<CameraFollow>().Active = true;
                gamePanel.showBattleBtn(player.gameObject);
                BattleManager.instance.startLevel();
                player.transform.position = checkPoint[4].position;
                gameManager.instance.beWhite(0.02f);
            }
            this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    private void FixedUpdate()
    {
        if (gamePanel.showFinish && (Application.platform == RuntimePlatform.Android ? Input.GetTouch(0).phase == TouchPhase.Began : Input.GetKeyDown(KeyCode.Z)))
        {
            gamePanel.hideTextPanel(true);
            switch (gameManager.instance.now)
            {
                case 1: StartCoroutine(event1()); break;
                case 2: gamePanel.showTextPanel("墨铭", "将士们冲啊！跟随大将军击垮匈奴军！活捉单于！", 'L', face1, "bottom"); break;
                case 3:
                    Camera.main.GetComponent<CameraFollow>().Active = true;
                    gamePanel.showBattleBtn(player.gameObject);
                    BattleManager.instance.startLevel();
                    break;
                case 8: gamePanel.showTextPanel("？？？", "哼哼.....", 'R', face2, "bottom"); break;
                case 9: gamePanel.showTextPanel("？？？", ".......", 'R', face2, "bottom"); break;
                case 10: StartCoroutine(boss()); break;
                case 12: StartCoroutine(event12()); break;
                default: break;
            }
        }
        if (autoEvent)
        {
            switch (gameManager.instance.now)
            {
                case 11:
                    if (!BattleManager.instance.inBattle)
                    {
                        autoEvent = false; player.Move(0);
                        gamePanel.showTextPanel("墨铭", "躲在暗影中的杀手...真是难缠的对手，耽误了不少时间，要赶紧去抓捕单于！！", 'L', face1, "bottom");
                    }
                    break;
                case 13: StartCoroutine(event13()); autoEvent = false; break;
            }
        }
    }

    void Update()
    {
        if (playerMove)
            player.Move(move);
        if (moveCamera)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, movePos, Time.deltaTime);
        }
    }

    IEnumerator event0()
    {
        gamePanel.showCG(CG1);
        gameManager.instance.beWhite(0.02f);
        yield return new WaitForSeconds(1);
        gamePanel.showTextPanel("hide", "卫青大军出塞一千余里，涉过大沙漠，终于与伊稚斜单于所部相遇。见匈奴军早有准备，他便下令用武刚车环绕为营，扎站住阵脚，随即汉军以精锐向匈奴发起冲击", 'L', null, "center");
    }

    IEnumerator event1()
    {
        gameManager.instance.beBlack(0.02f);
        yield return new WaitForSeconds(1);
        gamePanel.showCG(chapter4);
        gameManager.instance.beWhite(0.02f);
        yield return new WaitForSeconds(1);
        gameManager.instance.playSE(changeChapter);
        yield return new WaitForSeconds(2);
        gameManager.instance.beBlack(0.02f);
        yield return new WaitForSeconds(1);
        gamePanel.hideCG();
        player.Move(0.01f);
        player.Move(0);
        gameManager.instance.playBgm(BGM);
        gameManager.instance.beWhite(0.02f);
        gamePanel.showTextPanel("墨铭", "前面就是单于的主力部队！我们的战车已经站住阵脚，精忠报国的时机已经到来！", 'L', face1, "bottom");
    }

    IEnumerator event12()
    {
        Camera.main.GetComponent<CameraFollow>().Active = false;
        move = 1;
        playerMove = true;
        yield return new WaitForSeconds(2f);
        playerMove = false;
        BattleManager.instance.showLevelPanel();
        autoEvent = true;
    }

    IEnumerator event13()
    {
        gameManager.instance.level++;
        gameManager.instance.now = 0;
        gameManager.instance.Save();
        yield return new WaitForSeconds(1);
        gameManager.instance.loadScene(7);
    }

    //========================================战斗块的管理===============================
    public void triggerRun(int i)
    {
        switch (i)
        {
            case 0: StartCoroutine(battle0()); break;
            case 1: StartCoroutine(battle1()); break;
            case 2: StartCoroutine(battle2()); break;
            case 3: StartCoroutine(battle3()); break;
            case 4: StartCoroutine(battle4()); break;
            case 6: StartCoroutine(hint1()); break;
            default: break;
        }
    }

    IEnumerator battle0()
    {
        BattleManager.instance.setBirthPoint(checkPoint[0], triggers[0], walls[0]);
        BattleManager.instance.startBattle(null, 22, 40, 6, 0);
        int i = Random.Range(2, 5);
        BattleManager.instance.addEnemy(i, new Vector2(42, 0), false);
        i = Random.Range(2, 5);
        BattleManager.instance.addEnemy(i, new Vector2(20, 0), false);
        yield return new WaitForSeconds(10);
        i = Random.Range(2, 5);
        BattleManager.instance.addEnemy(i, new Vector2(42, 0), false);
        i = Random.Range(2, 5);
        BattleManager.instance.addEnemy(i, new Vector2(20, 0), false);
        yield return new WaitForSeconds(10);
        i = Random.Range(2, 5);
        BattleManager.instance.addEnemy(i, new Vector2(42, 0), false);
        i = Random.Range(2, 5);
        BattleManager.instance.addEnemy(i, new Vector2(30, 0), false);
        i = Random.Range(2, 5);
        BattleManager.instance.addEnemy(i, new Vector2(20, 0), true);
    }

    IEnumerator battle1()
    {
        int i = Random.Range(0, 4);
        if (i == 0)
        {
            BattleManager.instance.setBirthPoint(checkPoint[1], triggers[1], walls[1]);
        BattleManager.instance.startBattle(null, 80, 95, 6, 4);
            i = Random.Range(2, 5);
            BattleManager.instance.addEnemy(i, new Vector2(97, -1), false);
            i = Random.Range(2, 5);
            BattleManager.instance.addEnemy(i, new Vector2(77, -1), false);
        yield return new WaitForSeconds(10);
            i = Random.Range(2, 5);
            BattleManager.instance.addEnemy(i, new Vector2(97, -1), false);
            i = Random.Range(2, 5);
            BattleManager.instance.addEnemy(i, new Vector2(77, -1), false);
        yield return new WaitForSeconds(10);
            i = Random.Range(2, 5);
            BattleManager.instance.addEnemy(i, new Vector2(77, -1), false);
            i = Random.Range(2, 5);
            BattleManager.instance.addEnemy(i, new Vector2(97, -1), false);
        yield return new WaitForSeconds(10);
            i = Random.Range(2, 5);
            BattleManager.instance.addEnemy(i, new Vector2(77, -1), false);
            i = Random.Range(2, 5);
            BattleManager.instance.addEnemy(i, new Vector2(97, -1), false);
        yield return new WaitForSeconds(10);
            i = Random.Range(2, 5);
            BattleManager.instance.addEnemy(i, new Vector2(77, -1), false);
            i = Random.Range(2, 5);
            BattleManager.instance.addEnemy(i, new Vector2(90, -1), false);
            i = Random.Range(2, 5);
            BattleManager.instance.addEnemy(i, new Vector2(97, -1), true);
        }
        else if (i == 1)
        {
            triggers[1].SetActive(false);
            randomItem(itemBox);
            gameManager.instance.now++;
        }
        else if (i == 2)
        {
            triggers[1].SetActive(false);
            hpBox.SetActive(true);
            gameManager.instance.now++;
        }
        else if (i == 3)
        {
            triggers[1].SetActive(false);
            randomItem2(items);
            gameManager.instance.now++;
        }
    }

    IEnumerator hint1()
    {
        player.Move(0);
        Camera.main.GetComponent<CameraFollow>().Active = false;
        trigger6.SetActive(false);
        gamePanel.hideBattleBtn();
        movePos = Camera.main.transform.position + new Vector3(45, 0, 0);
        moveCamera = true;
        yield return new WaitForSeconds(4f);
        enemy1.Attack();
        yield return new WaitForSeconds(2f);
        movePos = Camera.main.transform.position - new Vector3(45, 0, 0);
        yield return new WaitForSeconds(4f);
        moveCamera = false;
        Camera.main.GetComponent<CameraFollow>().Active = true;
        gamePanel.showBattleBtn(player.gameObject);
    }

    IEnumerator battle2()
    {
        enemy1.gameObject.SetActive(false);
        BattleManager.instance.setBirthPoint(checkPoint[2], triggers[2], walls[2]);
        BattleManager.instance.startBattle(null, 150, 172, 6, 5);
        BattleManager.instance.addEnemy(5, enemy1.transform.position, false);
        int i = Random.Range(4, 6);
        BattleManager.instance.addEnemy(i, new Vector2(149, 5), false);
        i = Random.Range(4, 6);
        BattleManager.instance.addEnemy(i, new Vector2(179, 5), false);
        yield return new WaitForSeconds(12);
        i = Random.Range(4, 6);
        BattleManager.instance.addEnemy(i, new Vector2(149, 0), false);
        i = Random.Range(4, 6);
        BattleManager.instance.addEnemy(i, new Vector2(179, 0), false);
        yield return new WaitForSeconds(12);
        i = Random.Range(4, 6);
        BattleManager.instance.addEnemy(i, new Vector2(164, 5), false);
        i = Random.Range(4, 6);
        BattleManager.instance.addEnemy(i, new Vector2(149, 5), false);
        i = Random.Range(4, 6);
        BattleManager.instance.addEnemy(i, new Vector2(179, 5), true);
    }

    IEnumerator battle3()
    {
        BattleManager.instance.setBirthPoint(checkPoint[3], triggers[3], walls[3]);
        BattleManager.instance.startBattle(null, 230, 245, 6, 6);
        int i = Random.Range(4, 6);
        BattleManager.instance.addEnemy(i, new Vector2(230, 5), false);
        i = Random.Range(4, 6);
        BattleManager.instance.addEnemy(i, new Vector2(238, 0), false);
        i = Random.Range(4, 6);
        BattleManager.instance.addEnemy(i, new Vector2(245, 5), false);
        yield return new WaitForSeconds(12);
        i = Random.Range(4, 6);
        BattleManager.instance.addEnemy(i, new Vector2(230, 5), false);
        i = Random.Range(4, 6);
        BattleManager.instance.addEnemy(i, new Vector2(238, 0), false);
        i = Random.Range(4, 6);
        BattleManager.instance.addEnemy(i, new Vector2(245, 5), false);
        yield return new WaitForSeconds(12);
        i = Random.Range(4, 6);
        BattleManager.instance.addEnemy(i, new Vector2(230, 5), false);
        i = Random.Range(4, 6);
        BattleManager.instance.addEnemy(i, new Vector2(238, 0), false);
        i = Random.Range(4, 6);
        BattleManager.instance.addEnemy(i, new Vector2(245, 5), true);
    }

    IEnumerator battle4()
    {
        player.Move(0);
        yield return new WaitForSeconds(1);
        gamePanel.hideBattleBtn();
        Camera.main.GetComponent<CameraFollow>().Active = false;
        enemy2.call();
        yield return new WaitForSeconds(1f);
        enemy2.transform.position = player.transform.position + new Vector3(-4, 0, 0);
        enemy2.Move(0.01f);
        yield return new WaitForSeconds(0.5f);
        enemy2.call();
        yield return new WaitForSeconds(0.5f);
        player.Move(-0.01f);
        player.Move(0f);
        yield return new WaitForSeconds(1f);
        enemy2.transform.position = player.transform.position + new Vector3(4, 0, 0);
        enemy2.Move(-0.01f);
        yield return new WaitForSeconds(0.5f);
        enemy2.call();
        yield return new WaitForSeconds(0.5f);
        player.Move(0.01f);
        player.Move(0f);
        yield return new WaitForSeconds(1f);
        enemy2.transform.position = player.transform.position + new Vector3(10, 0, 0);
        Camera.main.GetComponent<CameraFollow>().Active = true;
        gamePanel.showTextPanel("墨铭", "是谁！！别躲在暗处偷偷摸摸！", 'L', face1, "bottom");
    }

    IEnumerator boss()
    {
        gamePanel.showBattleBtn(player.gameObject);
        BattleManager.instance.setBirthPoint(checkPoint[4], triggers[4], walls[4]);
        BattleManager.instance.startBattle(BGM2, 280, 315, 6, 7);
        gameManager.instance.showBOSSHint(4);
        yield return new WaitForSeconds(3);
        enemy2.gameObject.SetActive(false);
        BattleManager.instance.addEnemy(9, enemy2.transform.position, true);
        autoEvent = true;
    }

    public void randomItem(Box itemBox)
    {
        itemBox.gameObject.SetActive(true);
        int i;
        do
        {
            i = Random.Range(0, 8);
        } while (gameManager.instance.item.Contains(i.ToString()));
        itemBox.itemNum = i;
        itemBox.item = gameManager.instance.itemImage[i];
        itemBox.hintDescribe = "获得 " + gameManager.instance.itemName[i];
    }

    public void randomItem2(GameObject[] items)
    {
        int j = Random.Range(1, items.Length + 1);
        for (int k = 0; k < j; k++)
        {
            int i = Random.Range(0, items.Length);
            items[i].SetActive(true);
        }
    }
}
