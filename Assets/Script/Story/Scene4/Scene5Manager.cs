using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene5Manager : MonoBehaviour {

    [HideInInspector]
    public GamePanelManager gamePanel;
    public Player player;
    public Transform[] checkPoint;
    public GameObject[] triggers;
    public GameObject[] walls;
    public GameObject trigger6;

    //剧情用变量
    public Sprite CG1;
    public Sprite chapter3;
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
                case 2: gamePanel.showTextPanel("墨铭", "要穿过这片沙漠！", 'L', face1, "bottom"); break;
                case 3:
                    Camera.main.GetComponent<CameraFollow>().Active = true;
                    gamePanel.showBattleBtn(player.gameObject);
                    BattleManager.instance.startLevel();
                    break;
                case 8: gamePanel.showTextPanel("？？？", "能闯到这里，你们真的很不错", 'R', face2, "bottom"); break;
                case 9: gamePanel.showTextPanel("？？？", "不过，还是成为我的刀下之鬼吧", 'R', face2, "bottom"); break;
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
                         autoEvent = false;
                        gamePanel.showTextPanel("墨铭", "真是可怕的男人....要赶快去追击单于！", 'L', face1, "bottom");
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
        gamePanel.showTextPanel("hide", "卫青出塞后，捕获俘虏，得知伊稚斜单于的确实驻地，便令前将军李广与右将军赵食其两部合并，从东路出击匈奴军侧背，自率精兵直攻匈奴军", 'L', null, "center");
    }

    IEnumerator event1()
    {
        gameManager.instance.beBlack(0.02f);
        yield return new WaitForSeconds(1);
        gamePanel.showCG(chapter3);
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
        gamePanel.showTextPanel("墨铭", "根据俘虏的口供，伊稚斜单于就率兵驻扎在前方！", 'L', face1, "bottom");
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
        gameManager.instance.loadScene(6);
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
        BattleManager.instance.addEnemy(1, new Vector2(42, 0), false);
        BattleManager.instance.addEnemy(0, new Vector2(20, 0), false);
        yield return new WaitForSeconds(10);
        BattleManager.instance.addEnemy(3, new Vector2(42, 0), false);
        BattleManager.instance.addEnemy(2, new Vector2(20, 0), false);
        yield return new WaitForSeconds(10);
        BattleManager.instance.addEnemy(2, new Vector2(42, 0), false);
        BattleManager.instance.addEnemy(1, new Vector2(30, 0), false);
        BattleManager.instance.addEnemy(2, new Vector2(20, 0), true);
    }

    IEnumerator battle1()
    {
        BattleManager.instance.setBirthPoint(checkPoint[1], triggers[1], walls[1]);
        BattleManager.instance.startBattle(null, 80, 95, 6, 4);
        BattleManager.instance.addEnemy(3, new Vector2(97, -1), false);
        BattleManager.instance.addEnemy(3, new Vector2(77, -1), false);
        yield return new WaitForSeconds(10);
        BattleManager.instance.addEnemy(2, new Vector2(97, -1), false);
        BattleManager.instance.addEnemy(2, new Vector2(77, -1), false);
        yield return new WaitForSeconds(10);
        BattleManager.instance.addEnemy(1, new Vector2(77, -1), false);
        BattleManager.instance.addEnemy(1, new Vector2(97, -1), false);
        yield return new WaitForSeconds(10);
        BattleManager.instance.addEnemy(2, new Vector2(77, -1), false);
        BattleManager.instance.addEnemy(3, new Vector2(97, -1), false);
        yield return new WaitForSeconds(10);
        BattleManager.instance.addEnemy(2, new Vector2(77, -1), false);
        BattleManager.instance.addEnemy(3, new Vector2(90, -1), false);
        BattleManager.instance.addEnemy(1, new Vector2(97, -1), true);
    }

    IEnumerator hint1()
    {
        player.Move(0);
        Camera.main.GetComponent<CameraFollow>().Active = false;
        trigger6.SetActive(false);
        gamePanel.hideBattleBtn();
        movePos = Camera.main.transform.position + new Vector3(45, 0, 0);
        moveCamera = true;
        yield return new WaitForSeconds(5f);
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
        BattleManager.instance.addEnemy(4, enemy1.transform.position, false);
        BattleManager.instance.addEnemy(3, new Vector2(149, 5), false);
        BattleManager.instance.addEnemy(3, new Vector2(179, 5), false);
        yield return new WaitForSeconds(12);
        BattleManager.instance.addEnemy(4, new Vector2(149, 0), false);
        BattleManager.instance.addEnemy(4, new Vector2(179, 0), false);
        yield return new WaitForSeconds(12);
        BattleManager.instance.addEnemy(4, new Vector2(164, 5), false);
        BattleManager.instance.addEnemy(4, new Vector2(149, 5), false);
        BattleManager.instance.addEnemy(4, new Vector2(179, 5), true);
    }

    IEnumerator battle3()
    {
        BattleManager.instance.setBirthPoint(checkPoint[3], triggers[3], walls[3]);
        BattleManager.instance.startBattle(null, 230, 245, 6, 6);
        BattleManager.instance.addEnemy(4, new Vector2(230, 5), false);
        BattleManager.instance.addEnemy(2, new Vector2(238, 0), false);
        BattleManager.instance.addEnemy(4, new Vector2(245, 5), false);
        yield return new WaitForSeconds(12);
        BattleManager.instance.addEnemy(4, new Vector2(230, 5), false);
        BattleManager.instance.addEnemy(3, new Vector2(238, 0), false);
        BattleManager.instance.addEnemy(4, new Vector2(245, 5), false);
        yield return new WaitForSeconds(12);
        BattleManager.instance.addEnemy(4, new Vector2(230, 5), false);
        BattleManager.instance.addEnemy(4, new Vector2(238, 0), false);
        BattleManager.instance.addEnemy(4, new Vector2(245, 5), true);
    }

    IEnumerator battle4()
    {
        player.Move(0);
        yield return new WaitForSeconds(1);
        gamePanel.hideBattleBtn();
        Camera.main.GetComponent<CameraFollow>().Active = false;
        enemy2.Attack3S(-28);
        yield return new WaitForSeconds(1f);
        Vector3 a = player.transform.position;
        player._rigidbody.velocity = new Vector2(-20, -8);
        yield return new WaitForSeconds(0.5f);
        enemy2.transform.position = a + new Vector3(2, 3,0);
        enemy2.AttackDown();
        Camera.main.GetComponent<CameraFollow>().Active = true;
        gamePanel.showTextPanel("墨铭", "好快的刀法！！", 'L', face1, "bottom");
    }

    IEnumerator boss()
    {
        gamePanel.showBattleBtn(player.gameObject);
        BattleManager.instance.setBirthPoint(checkPoint[4], triggers[4], walls[4]);
        BattleManager.instance.startBattle(BGM2, 280, 315, 6, 7);
        gameManager.instance.showBOSSHint(3);
        yield return new WaitForSeconds(3);
        enemy2.gameObject.SetActive(false);
        BattleManager.instance.addEnemy(8, enemy2.transform.position, true);
        autoEvent = true;
    }
}
