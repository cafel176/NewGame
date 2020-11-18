using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager2 : MonoBehaviour
{

    [HideInInspector]
    public GamePanelManager gamePanel;
    public Player player;
    public Transform[] checkPoint;
    public GameObject[] triggers;
    public GameObject[] walls;
    public SpriteRenderer[] Hints;

    //剧情用变量
    public Sprite CG1;
    public Sprite chapter0;
    public Sprite face1;
    public Sprite face2;
    public GameObject npc1;
    public GameObject npc2;
    public Enemy enemy1;
    public Enemy enemy2;
    private bool npcMove = false;
    private float NPCmove = 0;
    private bool playerMove = false;
    private float move = 0;
    private bool moveCamera = false;
    private Vector3 movePos;
    private bool autoEvent = false;

    public AudioClip changeChapter;
    public AudioClip BGM;

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
            StartCoroutine(event0());
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
                case 1: gamePanel.showTextPanel("hide", "故事的主角，一位叫墨铭的年轻人，作为副将跟随着大将军卫青，踏上了保家卫国的出塞征途……", 'L', null, "center"); break;
                case 2: StartCoroutine(event2()); break;
                case 3: StartCoroutine(event3()); break;
                case 4: StartCoroutine(event4()); break;
                case 5:
                    Camera.main.GetComponent<CameraFollow>().Active = true;
                    gamePanel.showBattleBtn(player.gameObject);
                    BattleManager.instance.startLevel();
                    StartCoroutine(event5());
                    break;
                case 9: StartCoroutine(event9()); break;
                case 10: gamePanel.showTextPanel("墨铭", "只是小股的侦查部队吗，也好，回去向将军报告一下，匈奴接下来可能会有大动作了！", 'L', face1, "bottom"); break;
                case 11: StartCoroutine(event11()); break;
                default: break;
            }
        }
        if (autoEvent)
        {
            switch (gameManager.instance.now)
            {
                case 12: StartCoroutine(event12()); autoEvent = false; break;
            }
        }
    }

    private void Update()
    {
        if (playerMove)
            player.Move(move);
        if (npcMove)
        {
            enemy1.Move(NPCmove);
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
        gamePanel.showTextPanel("hide", "今天我们要讲的，是一个发生在很久很久之前的故事，那时，一个叫“汉”的大一统王朝，为了打击一直骚扰边境的来犯者，发动了一场规模极大的反击战。", 'L', null, "center");
    }

    IEnumerator event2()
    {
        gameManager.instance.beBlack(0.02f);
        yield return new WaitForSeconds(1);
        gamePanel.showCG(chapter0);
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
        playerMove = false;
        player.Move(0);
        gamePanel.showTextPanel("墨铭", "我们担任的是巡逻和打击敌人的骚扰的任务，大家时刻保持警惕！", 'L', face1, "bottom");
    }

    IEnumerator event3()
    {
        move = 1;
        playerMove = true;
        yield return new WaitForSeconds(0.5f);
        playerMove = false;
        player.Move(0);
        yield return new WaitForSeconds(1f);
        gamePanel.showTextPanel("墨铭", "嗯？？", 'L', face1, "bottom");
    }

    IEnumerator event4()
    {
        movePos = Camera.main.transform.position + new Vector3(30, 0, 0);
        moveCamera = true;
        yield return new WaitForSeconds(5f);
        movePos = Camera.main.transform.position - new Vector3(30, 0, 0);
        yield return new WaitForSeconds(3f);
        moveCamera = false;
        gamePanel.showTextPanel("墨铭", "前方有匈奴部队的踪迹，我们上！！", 'L', face1, "bottom");
    }

    IEnumerator event5()
    {
        StartCoroutine(hint(0));
        yield return new WaitForSeconds(5f);
        StartCoroutine(hint(1));
        yield return new WaitForSeconds(5f);
    }

    IEnumerator hint(int k)
    {
        for (int i = 0; i < 50; i++)
        {
            Color color = Hints[k].color;
            color.a += 0.02f;
            Hints[k].color = color;
            yield return new WaitForSeconds(0.02f);
        }
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < 50; i++)
        {
            Color color = Hints[k].color;
            color.a -= 0.02f;
            Hints[k].color = color;
            yield return new WaitForSeconds(0.02f);
        }
    }

    IEnumerator event9()
    {
        NPCmove = 1;
        npcMove = true;
        yield return new WaitForSeconds(2f);
        npcMove = false;
        gamePanel.showTextPanel("墨铭", "已经离开营地这么远，再继续前进有些冒进了", 'L', face1, "bottom");
    }

    IEnumerator event11()
    {
        Camera.main.GetComponent<CameraFollow>().Active = false;
        move = -1;
        playerMove = true;
        yield return new WaitForSeconds(2f);
        playerMove = false;
        player.Move(0);
        BattleManager.instance.showLevelPanel();
        autoEvent = true;
    }

    IEnumerator event12()
    {
        gameManager.instance.level++;
        gameManager.instance.now=0;
        gameManager.instance.Save();
        yield return new WaitForSeconds(1);
        gameManager.instance.loadScene(3);
    }

//========================================战斗块的管理===============================
    public void triggerRun(int i)
    {
        switch (i)
        {
            case 0: StartCoroutine(battle0()); break;
            case 1: StartCoroutine(battle1()); break;
            case 2: StartCoroutine(battle2()); break;
            case 3: StartCoroutine(boss()); break;
            default: break;
        }
    }

    IEnumerator battle0()
    {
        BattleManager.instance.setBirthPoint(checkPoint[0], triggers[0], walls[0]);
        BattleManager.instance.startBattle(null, 22, 40, 3,0);
        BattleManager.instance.addEnemy(0, npc1.transform.position, false);
        BattleManager.instance.addEnemy(0, npc2.transform.position, false);
        npc1.SetActive(false);
        npc2.SetActive(false);
        yield return new WaitForSeconds(10);
        BattleManager.instance.addEnemy(0, npc2.transform.position+new Vector3(5,0,0), false);
        yield return new WaitForSeconds(5);
        BattleManager.instance.addEnemy(0, npc1.transform.position + new Vector3(-5, 0, 0), true);
    }

    IEnumerator battle1()
    {
        BattleManager.instance.setBirthPoint(checkPoint[1], triggers[1], walls[1]);
        BattleManager.instance.startBattle(null, 80, 95, 3,6);
        BattleManager.instance.addEnemy(0, new Vector2(90,-1), false);
        BattleManager.instance.addEnemy(0, new Vector2(85, -1), false);
        StartCoroutine(hint(3));
        yield return new WaitForSeconds(10);
        BattleManager.instance.addEnemy(0, new Vector2(92, -1), false);
        StartCoroutine(hint(4));
        yield return new WaitForSeconds(5);
        BattleManager.instance.addEnemy(0, new Vector2(83, -1), false);
        StartCoroutine(hint(5));
        yield return new WaitForSeconds(5);
        BattleManager.instance.addEnemy(0, new Vector2(83, -1), false);
        BattleManager.instance.addEnemy(0, new Vector2(92, -1), true);

    }

    IEnumerator battle2()
    {
        BattleManager.instance.setBirthPoint(checkPoint[2], triggers[2], walls[2]);
        BattleManager.instance.startBattle(null, 135, 150, 3,7);
        BattleManager.instance.addEnemy(0, new Vector2(137, -1), false);
        BattleManager.instance.addEnemy(0, new Vector2(145, -1), false);
        StartCoroutine(hint(7));
        yield return new WaitForSeconds(10);
        BattleManager.instance.addEnemy(0, new Vector2(127, 5), false);
        BattleManager.instance.addEnemy(0, new Vector2(157, 5), false);
        StartCoroutine(hint(8));
        yield return new WaitForSeconds(10);
        BattleManager.instance.addEnemy(0, new Vector2(142, 5), false);
        BattleManager.instance.addEnemy(0, new Vector2(157, 5), false);
        StartCoroutine(hint(9));
        yield return new WaitForSeconds(10);
        BattleManager.instance.addEnemy(0, new Vector2(142, 5), false);
        BattleManager.instance.addEnemy(0, new Vector2(127, 5), true);

    }

    IEnumerator boss()
    {
        triggers[3].SetActive(false);
        player.Move(0);
        yield return new WaitForSeconds(0.5f);
        enemy1.Move(-0.1f);
        enemy1.Move(0);
        player.Move(0);
        gamePanel.showTextPanel("匈奴兵", "可恶，都追到这里来了，撤！！", 'R', face2, "bottom");
    }
}
