using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class challengeManager5 : MonoBehaviour {

    [HideInInspector]
    public GamePanelManager gamePanel;
    public Player player;
    public Transform[] checkPoint;
    public GameObject[] triggers;
    public GameObject[] walls;

    public Box itemBox;
    public GameObject hpBox;
    public GameObject[] items;

    //剧情用变量
    public Sprite CG1;
    public Sprite CG2;
    public Sprite chapter5;
    public Sprite face1;
    public Sprite face2;
    public Enemy enemy2;
    private bool playerMove = false;
    private float move = 0;
    private bool moveCamera = false;
    private Vector3 movePos;
    private bool autoEvent = false;
    private bool npcMove = false;
    private float NPCmove = 0;

    public AudioClip changeChapter;
    public AudioClip BGM;
    public AudioClip BGM2;

    void Start () {
        gamePanel = gameManager.instance.getGamePanel().GetComponent<GamePanelManager>();
        gameManager.instance.beBlack(1);
        BattleManager.instance.setGamePanel(gamePanel);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if(gameManager.instance.now==0)
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
                case 2: gamePanel.showTextPanel("墨铭", "将士们！为大汉尽献忠诚吧！", 'L', face1, "bottom"); break;
                case 3:
                    Camera.main.GetComponent<CameraFollow>().Active = true;
                    gamePanel.showBattleBtn(player.gameObject);
                    BattleManager.instance.startLevel();
                    break;
                case 8: gamePanel.showTextPanel("？？？", "你们必须止步这里！", 'R', face2, "bottom"); break;
                case 9: gamePanel.showTextPanel("？？？", "绝对不会让你追上单于", 'R', face2, "bottom"); break;
                case 10: StartCoroutine(battle5()); break;
                case 12: StartCoroutine(boss()); break;
                case 14: gamePanel.showTextPanel("墨铭", "可恶，战斗浪费了太多的时间，要赶快去追击单于！！", 'L', face1, "bottom"); break;
                case 15: StartCoroutine(event15()); break;
                case 17: gamePanel.showTextPanel("hide", "虽然未能活捉单于，漠北之战最终仍以汉军的全面胜利而告终。在这次战役中，共歼灭匈奴军九万余人，使其一时无力渡漠南下。", 'L', null, "center");break;
                case 18: gamePanel.showTextPanel("hide", "作为卫边战争的漠北决战的胜利，制止了匈奴奴隶主对汉边境的残暴掠夺，加速了我国北部地区的进一步统一和开发，具有深远的历史意义，可以说是我国古代反抗外敌侵略的重大胜利", 'L', null, "center"); break;
                case 19: gamePanel.showTextPanel("hide", "“明犯强汉者,虽远必诛！”		——陈汤", 'L', null, "center"); break;
                case 20: gamePanel.showTextPanel("hide", "感谢您的游玩！", 'L', null, "center"); break;
                case 21:
                    if (!gameManager.instance.ach.Contains("08"))
                        gameManager.instance.getAchievemrnt(8);
                    gameManager.instance.showMakerName(); break;
                default: break;
            }
        }
        if (autoEvent)
        {
            switch (gameManager.instance.now)
            {
                case 11: gamePanel.showTextPanel("墨铭", "不管你们出于什么理由，进犯我大汉边境，屠戮我大汉人民，不论单于他逃到哪里，我们都会追击，直到彻底击垮你们！", 'L', face1, "bottom"); autoEvent = false; break;
                case 13:
                    if (!BattleManager.instance.inBattle)
                    {
                        autoEvent = false;
                        gamePanel.showTextPanel("墨铭", "事关我大汉荣辱与百姓的生死存亡，挡我者，死！", 'L', face1, "bottom");
                    }
                    break;
                case 16: StartCoroutine(event16()); autoEvent = false; break;
            }
        }
    }

    void Update () {
        if (playerMove)
            player.Move(move);
        if (npcMove)
        {
            enemy2.Move(NPCmove);
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
        gamePanel.showTextPanel("hide", "汉军左校捕到俘虏，知伊稚斜单于已逃脱，急报卫青。卫青立即遣小队人马连夜追击，墨铭领命，踏上追击单于的最后征途", 'L', null, "center");
    }

    IEnumerator event1()
    {
        gameManager.instance.beBlack(0.02f);
        yield return new WaitForSeconds(1);
        gamePanel.showCG(chapter5);
        gameManager.instance.beWhite(0.02f);
        yield return new WaitForSeconds(1);
        gameManager.instance.playSE(changeChapter);
        yield return new WaitForSeconds(2);
        gameManager.instance.beBlack(0.02f);
        yield return new WaitForSeconds(1);
        gamePanel.hideCG();
        gameManager.instance.playBgm(BGM);
        gameManager.instance.beWhite(0.02f);
        move = 1;
        playerMove = true;
        yield return new WaitForSeconds(1.5f);
        move = 0;
        playerMove = false;
        gamePanel.showTextPanel("墨铭", "抓紧时间追击！不能让单于跑掉！", 'L', face1, "bottom");
    }

    IEnumerator event15()
    {
        Camera.main.GetComponent<CameraFollow>().Active = false;
        move = 1;
        playerMove = true;
        yield return new WaitForSeconds(2f);
        playerMove = false;
        BattleManager.instance.showLevelPanel();
        autoEvent = true;
    }

    IEnumerator event16()
    {
        yield return new WaitForSeconds(1);
        gameManager.instance.beBlack(0.02f);
        yield return new WaitForSeconds(1);
        gamePanel.showCG(CG2);
        gameManager.instance.beWhite(0.02f);
        yield return new WaitForSeconds(1);
        gamePanel.showTextPanel("hide", "至天明，汉军追出两百余里，未能追上伊稚斜单于，沿途歼敌万余人，进至寘颜山赵信城，获得匈奴大批屯粮，补充了军队。整休一日，尽焚其城及剩余军资而还。此战卫青军歼敌近两万人。", 'L', null, "center");
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
            default: break;
        }
    }

    IEnumerator battle0()
    {
        BattleManager.instance.setBirthPoint(checkPoint[0], triggers[0], walls[0]);
        BattleManager.instance.startBattle(null, 22, 40, 3,0);
        int i = Random.Range(4, 6);
        BattleManager.instance.addEnemy(i, new Vector2(42, 0), false);
        i = Random.Range(4, 6);
        BattleManager.instance.addEnemy(i, new Vector2(20, 0), false);
        yield return new WaitForSeconds(10);
        i = Random.Range(4, 6);
        BattleManager.instance.addEnemy(i, new Vector2(42, 0), false);
        i = Random.Range(4, 6);
        BattleManager.instance.addEnemy(i, new Vector2(20, 0), false);
        yield return new WaitForSeconds(10);
        i = Random.Range(4, 6);
        BattleManager.instance.addEnemy(i, new Vector2(42, 0), false);
        i = Random.Range(4, 6);
        BattleManager.instance.addEnemy(i, new Vector2(30, 0), false);
        i = Random.Range(4, 6);
        BattleManager.instance.addEnemy(i, new Vector2(20, 0), true);
    }

    IEnumerator battle1()
    {
        int i = Random.Range(0, 4);
        if (i == 0)
        {
            BattleManager.instance.setBirthPoint(checkPoint[1], triggers[1], walls[1]);
        BattleManager.instance.startBattle(null, 80, 95, 3,4);
            i = Random.Range(4, 6);
            BattleManager.instance.addEnemy(i, new Vector2(97, -1), false);
            i = Random.Range(4, 6);
            BattleManager.instance.addEnemy(i, new Vector2(77, -1), false);
        yield return new WaitForSeconds(10);
            i = Random.Range(4, 6);
            BattleManager.instance.addEnemy(i, new Vector2(97, -1), false);
        yield return new WaitForSeconds(5);
            i = Random.Range(4, 6);
            BattleManager.instance.addEnemy(i, new Vector2(77, -1), false);
        yield return new WaitForSeconds(5);
            i = Random.Range(4, 6);
            BattleManager.instance.addEnemy(i, new Vector2(77, -1), false);
            i = Random.Range(4, 6);
            BattleManager.instance.addEnemy(i, new Vector2(97, -1), false);
        yield return new WaitForSeconds(10);
            i = Random.Range(4, 6);
            BattleManager.instance.addEnemy(i, new Vector2(77, -1), false);
            i = Random.Range(4, 6);
            BattleManager.instance.addEnemy(i, new Vector2(90, -1), false);
            i = Random.Range(4, 6);
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

    IEnumerator battle2()
    {
        BattleManager.instance.setBirthPoint(checkPoint[2], triggers[2], walls[2]);
        BattleManager.instance.startBattle(null, 150, 172, 3,5);
        int i = Random.Range(4, 6);
        BattleManager.instance.addEnemy(i, new Vector2(149, 5), false);
        i = Random.Range(4, 6);
        BattleManager.instance.addEnemy(i, new Vector2(179, 5), false);
        yield return new WaitForSeconds(12);
        i = Random.Range(4, 6);
        BattleManager.instance.addEnemy(i, new Vector2(149, 0), false);
        i = Random.Range(4, 6);
        BattleManager.instance.addEnemy(i, new Vector2(179, 0), false);
        yield return new WaitForSeconds(10);
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
        BattleManager.instance.startBattle(null, 230, 245, 3,6);
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
        gamePanel.hideBattleBtn();
        player.Move(0);
        yield return new WaitForSeconds(1);
        gamePanel.showTextPanel("墨铭", "........", 'L', face1, "bottom");
    }

    IEnumerator battle5()
    {
        NPCmove = -1;
        npcMove = true;
        yield return new WaitForSeconds(2f);
        npcMove = false;
        gameManager.instance.now++;
        autoEvent = true;
    }

        IEnumerator boss()
    {
        player.Move(0);
        enemy2.Jump(-4);
        yield return new WaitForSeconds(2);
        gamePanel.showBattleBtn(player.gameObject);
        BattleManager.instance.setBirthPoint(checkPoint[4], triggers[4], walls[4]);
        BattleManager.instance.startBattle(BGM2, 280, 315, 3,7);
        gameManager.instance.showBOSSHint(5);
        yield return new WaitForSeconds(3);
        enemy2.gameObject.SetActive(false);
        BattleManager.instance.addEnemy(10, enemy2.transform.position, true);
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
