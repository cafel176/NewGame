using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class challengeManager2 : MonoBehaviour {

    [HideInInspector]
    public GamePanelManager gamePanel;
    public Player player;
    public Transform[] checkPoint;
    public GameObject[] triggers;
    public GameObject[] walls;
    public GameObject trigger5;
    public GameObject trigger6;

    public Box itemBox;
    public GameObject hpBox;
    public GameObject[] items;

    //剧情用变量
    public Sprite CG1;
    public Sprite chapter2;
    public Sprite face1;
    public Sprite face2;
    public Enemy enemy1;
    public Enemy enemy2;
    public Enemy enemy3;
    private bool playerMove = false;
    private float move = 0;
    private bool moveCamera = false;
    private Vector3 movePos;
    private bool autoEvent = false;
    private bool npcAttack = false;
    private float attackTimer = 0;

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
                case 1:StartCoroutine(event1()); break;
                case 2: gamePanel.showTextPanel("墨铭", "这里驻扎的匈奴不在少数，一定要小心为上", 'L', face1, "bottom"); break;
                case 3:
                    Camera.main.GetComponent<CameraFollow>().Active = true;
                    gamePanel.showBattleBtn(player.gameObject);
                    BattleManager.instance.startLevel();
                    break;
                case 8: gamePanel.showTextPanel("墨铭", "要小心天上的暗箭！！", 'L', face1, "bottom"); break;
                case 9: StartCoroutine(battle5()); break;
                case 11: gamePanel.showTextPanel("？？？", "不过也就到此为止了！", 'R', face2, "bottom"); break;
                case 12: StartCoroutine(boss()); break;
                case 14: StartCoroutine(event14()); break;
                default: break;
            }
        }
        if (autoEvent)
        {
            switch (gameManager.instance.now)
            {
                case 10: gamePanel.showTextPanel("？？？", "能躲过我的逆风箭，身手不错", 'R', face2, "bottom"); autoEvent = false; break;
                case 13:
                    if (!BattleManager.instance.inBattle)
                    {
                        autoEvent = false;
                        gamePanel.showTextPanel("墨铭", "你该为了这座城偿命", 'L', face1, "bottom");
                    }
                    break;
                case 15: StartCoroutine(event15()); autoEvent = false; break;
            }
        }
    }

    void Update()
    {
        if (playerMove)
            player.Move(move);
        if (npcAttack)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer > 5)
            {
                enemy3.Thread5();
                attackTimer = 0;
            }
        }
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
        gamePanel.showTextPanel("hide", "一众部队到达定襄，这里早已被匈奴洗劫一空，匈奴把这里作为军事要塞驻守起来", 'L', null, "center");
    }

    IEnumerator event1()
    {
        gameManager.instance.beBlack(0.02f);
        yield return new WaitForSeconds(1);
        gamePanel.showCG(chapter2);
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
        gamePanel.showTextPanel("墨铭", "这里就是定襄了？竟已是如此荒芜…", 'L', face1, "bottom");
    }

    IEnumerator event14()
    {
        Camera.main.GetComponent<CameraFollow>().Active = false;
        yield return new WaitForSeconds(2f);
        BattleManager.instance.showLevelPanel();
        autoEvent = true;
    }

    IEnumerator event15()
    {
        gameManager.instance.level++;
        gameManager.instance.now = 0;
        gameManager.instance.Save();
        yield return new WaitForSeconds(1);
        gameManager.instance.loadScene(5);
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
            case 5: StartCoroutine(hint1()); break;
            case 6: StartCoroutine(hint2()); break;
            default: break;
        }
    }

    IEnumerator battle0()
    {
        BattleManager.instance.setBirthPoint(checkPoint[0], triggers[0], walls[0]);
        BattleManager.instance.startBattle(null, 22, 40, 6, 0);
        int i = Random.Range(0, 2);
        BattleManager.instance.addEnemy(i, new Vector2(42, 0), false);
        i = Random.Range(0, 2);
        BattleManager.instance.addEnemy(i, new Vector2(20, 0), false);
        yield return new WaitForSeconds(10);
        i = Random.Range(0, 2);
        BattleManager.instance.addEnemy(i, new Vector2(42, 5), false);
        i = Random.Range(0, 2);
        BattleManager.instance.addEnemy(i, new Vector2(20, 5), false);
        yield return new WaitForSeconds(10);
        i = Random.Range(0, 2);
        BattleManager.instance.addEnemy(i, new Vector2(42, 5), false);
        i = Random.Range(0, 2);
        BattleManager.instance.addEnemy(i, new Vector2(30, 0), false);
        i = Random.Range(0, 2);
        BattleManager.instance.addEnemy(i, new Vector2(20, 5), true);
    }

    IEnumerator battle1()
    {
        int i = Random.Range(0, 4);
        if (i == 0)
        {
            BattleManager.instance.setBirthPoint(checkPoint[1], triggers[1], walls[1]);
        BattleManager.instance.startBattle(null, 80, 95, 6, 4);
            i = Random.Range(0, 2);
            BattleManager.instance.addEnemy(i, new Vector2(97, 0), false);
            i = Random.Range(0, 2);
            BattleManager.instance.addEnemy(i, new Vector2(77, 0), false);
        yield return new WaitForSeconds(10);
            i = Random.Range(0, 2);
            BattleManager.instance.addEnemy(i, new Vector2(97, 0), false);
        yield return new WaitForSeconds(5);
            i = Random.Range(0, 2);
            BattleManager.instance.addEnemy(i, new Vector2(77, 0), false);
        yield return new WaitForSeconds(5);
            i = Random.Range(0, 2);
            BattleManager.instance.addEnemy(i, new Vector2(77, 0), false);
            i = Random.Range(0, 2);
            BattleManager.instance.addEnemy(i, new Vector2(97, 5), false);
        yield return new WaitForSeconds(10);
            i = Random.Range(0, 2);
            BattleManager.instance.addEnemy(i, new Vector2(77, 5), false);
            i = Random.Range(0, 2);
            BattleManager.instance.addEnemy(i, new Vector2(90, 5), false);
            i = Random.Range(0, 2);
            BattleManager.instance.addEnemy(i, new Vector2(97, 5), true);
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
        Camera.main.GetComponent<CameraFollow>().Active = false;
        trigger5.SetActive(false);
        gamePanel.hideBattleBtn();
        player.Move(0);
        movePos = Camera.main.transform.position + new Vector3(45, 0, 0);
        moveCamera = true;
        yield return new WaitForSeconds(5f);
        movePos = Camera.main.transform.position - new Vector3(45, 0, 0);
        yield return new WaitForSeconds(4f);
        moveCamera = false;
        Camera.main.GetComponent<CameraFollow>().Active = true;
        gamePanel.showBattleBtn(player.gameObject);
    }

    IEnumerator hint2()
    {
        Camera.main.GetComponent<CameraFollow>().Active = false;
        trigger6.SetActive(false);
        gamePanel.hideBattleBtn();
        player.Move(0);
        movePos = Camera.main.transform.position + new Vector3(45, 0, 0);
        moveCamera = true;
        yield return new WaitForSeconds(4f);
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
        BattleManager.instance.addEnemy(2, enemy1.transform.position, false);
        int i = Random.Range(0, 3);
        BattleManager.instance.addEnemy(i, new Vector2(149, 7), false);
        i = Random.Range(0, 3);
        BattleManager.instance.addEnemy(i, new Vector2(179, 7), false);
        yield return new WaitForSeconds(12);
        i = Random.Range(0, 3);
        BattleManager.instance.addEnemy(i, new Vector2(149, 7), false);
        i = Random.Range(0, 3);
        BattleManager.instance.addEnemy(i, new Vector2(179, 7), false);
        yield return new WaitForSeconds(12);
        i = Random.Range(0, 3);
        BattleManager.instance.addEnemy(i, new Vector2(164, 7), false);
        i = Random.Range(0, 3);
        BattleManager.instance.addEnemy(i, new Vector2(149, 7), false);
        i = Random.Range(0, 3);
        BattleManager.instance.addEnemy(i, new Vector2(179, 7), true);
    }

    IEnumerator battle3()
    {
        enemy2.gameObject.SetActive(false);
        BattleManager.instance.setBirthPoint(checkPoint[3], triggers[3], walls[3]);
        BattleManager.instance.startBattle(null, 230, 245, 6, 6);
        BattleManager.instance.addEnemy(3, enemy2.transform.position, false);
        int i = Random.Range(0, 4);
        BattleManager.instance.addEnemy(i, new Vector2(238, 0), false);
        i = Random.Range(0, 4);
        BattleManager.instance.addEnemy(i, new Vector2(245, 5), false);
        yield return new WaitForSeconds(12);
        i = Random.Range(0, 4);
        BattleManager.instance.addEnemy(i, new Vector2(230, 5), false);
        i = Random.Range(0, 4);
        BattleManager.instance.addEnemy(i, new Vector2(238, 0), false);
        i = Random.Range(0, 4);
        BattleManager.instance.addEnemy(i, new Vector2(245, 5), false);
        yield return new WaitForSeconds(12);
        i = Random.Range(0, 4);
        BattleManager.instance.addEnemy(i, new Vector2(230, 5), false);
        i = Random.Range(0, 4);
        BattleManager.instance.addEnemy(i, new Vector2(238, 0), false);
        i = Random.Range(0, 4);
        BattleManager.instance.addEnemy(i, new Vector2(245, 5), true);
    }

    IEnumerator battle4()
    {
        player.Move(0);
        yield return new WaitForSeconds(1);
        gamePanel.hideBattleBtn();
        Camera.main.GetComponent<CameraFollow>().Active = false;
        movePos = Camera.main.transform.position + new Vector3(10, 3, 0);
        moveCamera = true;
        yield return new WaitForSeconds(1f);
        enemy3.Thread5();
        yield return new WaitForSeconds(1f);
        movePos = Camera.main.transform.position - new Vector3(10, 3, 0);
        yield return new WaitForSeconds(1f);
        moveCamera = false;
        Camera.main.GetComponent<CameraFollow>().Active = true;
        move = 2;
        playerMove = true;
        yield return new WaitForSeconds(0.5f);
        playerMove = false;
        player.Move(-0.1f);
        yield return new WaitForSeconds(1f);
        gamePanel.showTextPanel("墨铭", "有埋伏！！!", 'L', face1, "bottom");
    }

    IEnumerator battle5()
    {
        npcAttack = true;
        gamePanel.showBattleBtn(player.gameObject);
        BattleManager.instance.setBirthPoint(checkPoint[4], triggers[4], walls[4]);
        BattleManager.instance.startBattle(null, 280, 315, 6, 7);
        int i = Random.Range(0, 4);
        BattleManager.instance.addEnemy(i, new Vector2(297, 5), false);
        i = Random.Range(0, 4);
        BattleManager.instance.addEnemy(i, new Vector2(303, 0), false);
        i = Random.Range(0, 4);
        BattleManager.instance.addEnemy(i, new Vector2(314, 5), false);
        yield return new WaitForSeconds(15);
        i = Random.Range(0, 4);
        BattleManager.instance.addEnemy(i, new Vector2(297, 5), false);
        i = Random.Range(0, 4);
        BattleManager.instance.addEnemy(i, new Vector2(303, 0), false);
        i = Random.Range(0, 4);
        BattleManager.instance.addEnemy(i, new Vector2(314, 5), false);
        yield return new WaitForSeconds(15);
        i = Random.Range(0, 4);
        BattleManager.instance.addEnemy(i, new Vector2(297, 5), false);
        i = Random.Range(0, 4);
        BattleManager.instance.addEnemy(i, new Vector2(303, 0), false);
        i = Random.Range(0, 4);
        BattleManager.instance.addEnemy(i, new Vector2(314, 5), true);
        autoEvent = true;
        yield return new WaitForSeconds(15);
        npcAttack = false;
    }

    IEnumerator boss()
    {
        player.Move(0);
        enemy3.Jump(-2);
        yield return new WaitForSeconds(2);
        gamePanel.showBattleBtn(player.gameObject);
        BattleManager.instance.setBirthPoint(checkPoint[4], triggers[4], walls[4]);
        BattleManager.instance.startBattle(BGM2, 280, 315, 6, 7);
        gameManager.instance.showBOSSHint(2);
        yield return new WaitForSeconds(3);
        enemy3.gameObject.SetActive(false);
        BattleManager.instance.addEnemy(7, enemy3.transform.position, true);
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
