using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour {
    //管理器的实例
    public static BattleManager instance;
    //需要的预设
    public GameObject playerModel;
    public GameObject threadModel;
    public GameObject threadModel2;
    public Sprite GGpanel;
    public GameObject levelPanel;
    //内部变量
    private GamePanelManager gamePanel;  //需要赋值
    private List<GameObject> list = new List<GameObject>();
    private Player player;
    private Transform birthPoint;
    private GameObject trigger;
    private GameObject walls;
    public int nowBuff=-1;
    private float buffTimer = 0;
    public float buffTime = 30;
    [HideInInspector]
    public bool inBattle = false;
    private bool finish = false;
    private AudioClip battleBGM;
    private Vector3 tt;
    [HideInInspector]
    public int combo = 0;
    private float comboTimer = 0;
    private float STimer = 0;
    private int rebirthpoint=0;

    private bool inLevel = false;
    [HideInInspector]
    public float levelTimer = 0;
    [HideInInspector]
    public int levelKill = 0;
    [HideInInspector]
    public int comboScore = 0;
    [HideInInspector]
    public int hurt = 0;
    //音乐音效
    public AudioClip gg;

    private int h = 0;

    private void Awake()
    {
        //创造管理器实例
        BattleManager.instance = this;
        DontDestroyOnLoad(BattleManager.instance.gameObject);
    }

    private void Update()
    {
        if (inLevel)
            levelTimer += Time.deltaTime;
        if (combo > 0)
        {
            comboTimer += Time.deltaTime;
            if (comboTimer > 2 && combo > 0)
            {
                combo--;
                comboTimer = 1.5f;
            }
        }
        if (combo > 80)
        {
            STimer += Time.deltaTime;
            if (STimer > 1)
            {
                STimer = 0;
                comboScore += 300;
            }
        }else
            STimer = 0;
        if (inBattle)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == null)
                    list.RemoveAt(i);
            }
            if (list.Count == 0 && finish)
            {
                endBattle();
            }
        }
        if(gamePanel!=null)
            gamePanel.buffImage.GetComponent<Image>().sprite = gameManager.instance.skillImage[nowBuff>=0?nowBuff + 3:9];
    }

    private void FixedUpdate()
    {
        if (nowBuff >= 0)
        {
            buffTimer += Time.deltaTime;
            if (buffTimer > buffTime)
            {
                gameManager.instance.checkBuff(nowBuff, false);
                buffTimer = 0;
            }
        }
    }

    public void pauseEnemy(bool pause)
    {
        for(int i = 0; i < list.Count; i++)
        {
            list[i].GetComponent<Enemy>().pause = pause;
        }
    }

    //战斗结束。finish打开后清怪结束自动触发，之后进度now自增
    public void endBattle()
    {
        gameManager.instance.now++;
        inLevel = false;
        inBattle = false;
        finish = false;
        if (battleBGM != null)
        {
            gameManager.instance.stopBgm();
            battleBGM = null;
        }
        Camera.main.GetComponent<CameraFollow>().minX = tt.x;
        Camera.main.GetComponent<CameraFollow>().maxX = tt.y;
        Camera.main.GetComponent<CameraFollow>().maxY = tt.z;
        walls.SetActive(false);
    }

    public void PlayerCombo()
    {
        comboTimer = 0;
        if (combo < 100)
            combo++;
        if (inLevel)
        {
            if (combo == 20)
                comboScore += 100;
            else if (combo == 50)
                comboScore += 300;
            else if (combo == 80)
            {
                comboScore += 500;
                h++;
                if (h >= 3)
                {
                    if (!gameManager.instance.ach.Contains("05"))
                        gameManager.instance.getAchievemrnt(5);
                }
            }
        }
    }

    public void PlayerAttacked()
    {
        if (gameManager.instance.attackAll)
        {
            float k = Random.Range(0f, 1f);
            if (k <= 0.1f+gameManager.instance.Luck/4)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].GetComponent<Enemy>().takeDamage(2 * gameManager.instance.AttackValue);
                }
            }
        }
        hurt++;
    }

    public void PlayerDie()
    {
        bool a = false;
        if (gameManager.instance.rebirth)
        {
            float i = Random.Range(0f, 1f);
            if (i <= 0.65f)
            {
                GameObject go = Instantiate(playerModel, player.gameObject.transform.position + new Vector3(0, 3, 0), Quaternion.identity);
                gamePanel.player = go;
                Camera.main.GetComponent<CameraFollow>().player = go;
                Destroy(player.gameObject);
                player = go.GetComponent<Player>();
                a = true;
            }
        }
        if (!a)
        {
            //重新开始界面
            StartCoroutine(GG());
        }
    }

    IEnumerator GG()
    {
        inLevel = false;
        inBattle = false;
        gameManager.instance.playBgm(gg);
        gameManager.instance.beBlack(0.02f);
        yield return new WaitForSeconds(1);
        gamePanel.showCG(GGpanel);
        gameManager.instance.beWhite(0.02f);
        yield return new WaitForSeconds(3);
        gameManager.instance.stopBgm();
        gameManager.instance.now = rebirthpoint;
        gameManager.instance.loadScene(gameManager.instance.scene);
    }

    //===========================需要在场景管理器用到的方法======================
    public void setGamePanel(GamePanelManager gamePanel)
    {
        this.gamePanel = gamePanel;
        player = gamePanel.player.GetComponent<Player>();
    }

    //开始关卡时调用，开始计分
    public void startLevel()
    {
        inLevel = true;
        combo = 0;
        comboTimer = 0;
    }

    //设置重生点和复活点
    public void setBirthPoint(Transform pos, GameObject tri, GameObject wall)
    {
        birthPoint = pos;
        trigger = tri;
        walls = wall;
    }

    //进入战斗，胜利后进度now自增
    public void startBattle(AudioClip bgm,int minX,int maxX,int maxY,int point)
    {
        rebirthpoint = point;
        trigger.SetActive(false);
        walls.SetActive(true);
        battleBGM = bgm;  inBattle = true; finish = false;
        tt = new Vector3(Camera.main.GetComponent<CameraFollow>().minX, Camera.main.GetComponent<CameraFollow>().maxX, Camera.main.GetComponent<CameraFollow>().maxY);
        Camera.main.GetComponent<CameraFollow>().minX = minX;
        Camera.main.GetComponent<CameraFollow>().maxX = maxX;
        Camera.main.GetComponent<CameraFollow>().maxY = maxY;
        if (battleBGM != null)
            gameManager.instance.playBgm(battleBGM);
        inLevel = true;
    }


    //指定位置添加敌人  //添加完毕手动处理finish开关
    public void addEnemy(int i, Vector2 pos, bool finish)
    {
        GameObject go = Instantiate(gameManager.instance.enmeyList[i], pos, Quaternion.identity);
        list.Add(go);
        this.finish = finish;
        switch (i)
        {
            case 6: gamePanel.showBOSSSlider(go.GetComponent<Enemy>(), 1);break;
            case 7: gamePanel.showBOSSSlider(go.GetComponent<Enemy>(), 2); break;
            case 8: gamePanel.showBOSSSlider(go.GetComponent<Enemy>(), 3); break;
            case 9: gamePanel.showBOSSSlider(go.GetComponent<Enemy>(), 4); break;
            case 10: gamePanel.showBOSSSlider(go.GetComponent<Enemy>(), 5); break;
            default:break;
        }
    }

    public void pauseLevel(bool take)
    {
        inLevel = take;
    }

    //显示计分面板
    public void showLevelPanel()
    {
        inLevel = false;
        GameObject go = GameObject.Instantiate(levelPanel) as GameObject;
        go.transform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>());
        go.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
    }

    //看过计分面板之后自动调用
    public void endLevel()
    {
        combo = 0;
        comboTimer = 0;
        comboScore = 0;
        levelKill = 0;
        levelTimer = 0;
        hurt = 0;
        h = 0;
    }
}
