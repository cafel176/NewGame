using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour
{
    //管理器的实例
    public static gameManager instance;
    //需要的预设
    public GameObject Quitpanel;
    public GameObject SettingPanel;
    public GameObject GamePanel;
    public GameObject PausePanel;
    public GameObject BlackPanel;
    public GameObject HintPanel;
    public GameObject AchPanel;
    public GameObject PlayerPanel;
    public GameObject AchHint;
    public GameObject BOSSHint;
    public GameObject MakerName;
    public List<GameObject> enmeyList = new List<GameObject>();
    public AudioSource BGMaudio;
    public AudioSource SEaudio;
    //成就图像集
    public Sprite[] pictrue;
    //boss图像集
    public Sprite[] boss;
    //主动技能图标
    public Sprite[] skillImage;
    //图标
    public Sprite[] itemImage;
    public string[] itemName; 

    //角色数据，不同的场合都要设置HP=0的情况，要设置初始值    
    public int AttackValue = 10;
    public int DefendValue = 10;
    public int MaxHp = 500;
    public int MaxSp = 100;
    public float JumpForce = 18;
    public int AttackForce = 9;      //击退强度
    public float MoveSpeed = 9;
    public int getHp = 20;
    public int getSp = 5;             //角色单次平A敌人获得的SP
    public float Luck = 0;          //幸运值

    //存档数据
    public string Askill = "";//用来保存已经拥有的主动技能，用#分隔
    public string Nskill = "";//用来保存已经拥有的被动技能，用#分隔
    public string item = "";//用来保存已经拥有的道具，用#分隔
    public string AskillYes = "";//用来保存已经解锁的主动技能，用#分隔
    public string NskillYes = "";//用来保存已经解锁的被动技能，用#分隔
    public string itemYes = "";//用来保存已经解锁的道具，用#分隔
    public string MYes = "";//用来保存已经解锁的资料，用#分隔
    public int spirit = 0;          //灵魂
    public int kill = 0;        //  击杀数
    public int now = 0;//记录当前进度的序号，同时用于整个游戏的流程控制
    public int level = 0;//剧情模式关卡进度0-5，挑战模式0-3

    //游戏数据
    public float attackS = 0.1f;   //暴击率
    public float attackP = 0.15f;   //破防率
    public float miss = 0.05f;   //闪避率
    public float AttackSpeed = 0.3f;    //允许攻击的按键检测间隔时间
    public float AttackRange = 3f;      
    public float ComboTime = 0.3f;      //用于角色连击动画判定
    public float ComboJudge = 0.4f;     //用于连击系统判定
    public int getHpNeed = 20;      //角色回血技能消耗的SP
    public bool canAttacked = true; //受伤动作
    public bool crazy = false; //暴乱
    public bool get = false; //汲取
    public bool attackAll = false; //震效果
    public bool AllSp = false; //坎效果
    public bool AllHp = false; //离效果
    public bool getSpirit = false; //艮效果
    public bool rebirth = false; //重生
    public bool noBreak = false; //不破
    public bool AgetHp = false; //盛华
    public bool AgetSp = false; //沐血
    public bool jump = false;  // 跳跃的开关
    public bool attack = false;  // 攻击的开关
    public bool fly = false;  // 二段跳的开关
    public bool gravity = true;  // 重力的开关
    public bool control = true;
    public bool debug = false;  //测试模式
    public bool debug2 = false;  //测试模式

    public int sp1=9;      //主动技能装备槽//9为UIMask
    public int sp2 = 9;
    public int skill1 = 9;      //被动技能装备槽
    public int skill2 = 9;

    //调试数据
    public int hard = 0;//困难度
    public int gameState = 0;//记录当前游戏类型
    public int saveIndex = 0;//记录当前存档编号
    //需要的内部执行块
    [HideInInspector]
    public bool downPlatform = false;
    [HideInInspector]
    public int scene = 2; //用于加载新场景
    private GameObject blackPanel = null;//当前的蒙板，用于制造黑屏和淡入淡出
    [HideInInspector]
    public GameObject gamePanel = null;
    [HideInInspector]
    public string ach = "";//用来保存达成过的技能，用#分隔
    //音乐和音效
    public AudioClip MakerNameBGM;
    public AudioClip Btn;
    public AudioClip Yes;
    public AudioClip No;
    public AudioClip save;
    //音量
    [HideInInspector]
    public float music = 0.3f;
    [HideInInspector]
    public float SE = 0.5f;

    private void Awake()
    {
        //创造管理器实例
        gameManager.instance = this;
        DontDestroyOnLoad(gameManager.instance.gameObject);
    }

    private void Start()
    {
        //读取成就和音量
        ach = PlayerPrefs.GetString("ach");
        music = PlayerPrefs.GetFloat("music");
        SE = PlayerPrefs.GetFloat("se");
        if (PlayerPrefs.GetString("AskillYes") != "")
        {
            AskillYes = PlayerPrefs.GetString("AskillYes");
            NskillYes = PlayerPrefs.GetString("NskillYes");
            itemYes = PlayerPrefs.GetString("itemYes");
            MYes = PlayerPrefs.GetString("MYes");
        }
    }

    private void Update()
    {
        //更新音量
        BGMaudio.volume = music;
        SEaudio.volume = SE;
    }

    private void FixedUpdate()
    {
        //杀敌成就
        if (!ach.Contains("00") && kill >= 1)
            getAchievemrnt(0);
        else if (!ach.Contains("01") && kill >= 30)
            getAchievemrnt(1);
        else if (!ach.Contains("02") && kill >= 60)
            getAchievemrnt(2);
        else if (!ach.Contains("03") && kill >= 100)
            getAchievemrnt(3);
        if (spirit >= 1000)
        {
            if (!gameManager.instance.ach.Contains("09"))
                gameManager.instance.getAchievemrnt(9);
        }
        if (AskillYes.Length >= 18 && NskillYes.Length >= 18)
        {
            if (!gameManager.instance.ach.Contains("10"))
                gameManager.instance.getAchievemrnt(10);
        }
        if (itemYes.Length>=16)
        {
            if (!gameManager.instance.ach.Contains("11"))
                gameManager.instance.getAchievemrnt(11);
        }
        if (MYes.Length >= 33)
        {
            if (!gameManager.instance.ach.Contains("12"))
                gameManager.instance.getAchievemrnt(12);
        }
    }

    //退出游戏
    public void Exit()
    {
        Application.Quit();
    }


    //加载场景
    public void loadScene(int scene)
    {
        stopBgm();
        this.scene = scene;
        beBlack(0.02f);
        StartCoroutine(wait(1)); ;
    }

    IEnumerator wait(float time)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(1);
    }

    //回到主页
    public void back()
    {
        stopBgm();
        beBlack(0.02f);
        StartCoroutine(back(1));
    }

    //播放BGM
    public void playBgm(AudioClip clip)
    {
        if (clip != null)
        {
            BGMaudio.clip = clip;
            BGMaudio.Play();
        }
    }

    //停止BGM
    public void stopBgm()
    {
        BGMaudio.Stop();
    }

    //播放SE
    public void playSE(AudioClip clip)
    {
        if (clip != null)
        {
            SEaudio.clip = clip;
            SEaudio.Play();
        }
    }

    //在不同场景中按下返回键的效果(待更新)
    public void checkBackButton(string choose)
    {
        switch (choose)
        {
            case "main":
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    playSE(Btn);
                    createQuitPanel();
                }
                break;
            case "game":
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    playSE(Btn);
                    createPausePanel();
                }
                break;
            default: break;
        }
    }

    //在不同场景中按下菜单键的效果(待更新)
    public void checkMenuButton(string choose)
    {
        switch (choose)
        {
            case "main":
                if (Input.GetKeyDown(KeyCode.Menu))
                {
                    playSE(Btn);
                    createSettingPanel();
                }
                break;
            default: break;
        }
    }

    //创建设置菜单
    public void createSettingPanel()
    {
        GameObject go = GameObject.Instantiate(SettingPanel) as GameObject;
        go.transform.SetParent(GameObject.Find("Canvas").transform);
        go.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
    }

    //创建人物面板
    public void createPlayerPanel()
    {
        GameObject go = GameObject.Instantiate(PlayerPanel) as GameObject;
        go.transform.SetParent(GameObject.Find("Canvas").transform);
        go.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
    }

    //创建退出询问框
    public void createQuitPanel()
    {
        GameObject go = GameObject.Instantiate(Quitpanel) as GameObject;
        go.transform.SetParent(GameObject.Find("Canvas").transform);
        go.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);

        string[] btnsName = new string[2];
        btnsName[0] = "YesButton";
        btnsName[1] = "NoButton";

        foreach (string btnName in btnsName)
        {
            GameObject btnObj = GameObject.Find(btnName);
            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(delegate () {
                this.OnClick(btnObj, go);
            });
        }
    }

    //创建游戏暂停框
    public void createPausePanel()
    {
        GameObject go = GameObject.Instantiate(PausePanel) as GameObject;
        go.transform.SetParent(GameObject.Find("Canvas").transform);
        go.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
    }

    //所有动态创建的按钮的监听集合，记得添加nowPanel的索引
    public void OnClick(GameObject sender, GameObject nowPanel)
    {
        switch (sender.name)
        {
            case "YesButton":
                playSE(Yes);
                Exit();
                break;
            case "NoButton":
                playSE(No);
                Destroy(nowPanel);
                break;
            default:
                break;
        }
    }

    //创建游戏面板
    public GameObject getGamePanel()
    {
        if (gamePanel == null)
        {
            gamePanel = GameObject.Instantiate(GamePanel) as GameObject;
            gamePanel.transform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>());
            gamePanel.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
            return gamePanel;
        }
        else
            return gamePanel;
    }

    //播放制作人员名单(需要更新)
    public void showMakerName()
    {
        StartCoroutine(Show());
    }

    IEnumerator Show()
    {
        GameObject go = GameObject.Instantiate(MakerName) as GameObject;
        go.transform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>());
        beBlack(0.02f);
        yield return new WaitForSeconds(1);
        stopBgm();
        go.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
        beWhite(0.02f);
        yield return new WaitForSeconds(1);
        playBgm(MakerNameBGM);
        yield return new WaitForSeconds(75);
        beBlack(0.01f);
        yield return new WaitForSeconds(3);
        stopBgm();
        back();
    }

    //获得成就(需要更新)
    public void getAchievemrnt(int i)
    {
        ach += i < 10 ? "#0" + i : "#" + i;
        GameObject go = GameObject.Instantiate(AchHint) as GameObject;
        go.transform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>());
        string name = "";
        Sprite pic = null;
        switch (i)
        {
            case 0: name = "第一滴血"; pic = pictrue[i]; break;
            case 1: name = "初露锋芒"; pic = pictrue[i];unlock('N',2); break;
            case 2: name = "久经沙场"; pic = pictrue[i]; unlock('A', 2); break;
            case 3: name = "浴血重生"; pic = pictrue[i]; unlock('A', 5); break;
            case 4: name = "内明"; pic = pictrue[i]; unlock('A', 8); break;
            case 5: name = "能立"; pic = pictrue[i]; unlock('I', 6); break;
            case 6: name = "涅槃"; pic = pictrue[i]; unlock('N', 5); break;
            case 7: name = "流转"; pic = pictrue[i]; unlock('N', 4); break;
            case 8: name = "轮回"; pic = pictrue[i]; unlock('N', 6); break;
            case 9: name = "灭谛"; pic = pictrue[i]; unlock('N', 8); break;
            case 10: name = "刹"; pic = pictrue[i]; unlock('I', 2); break;
            case 11: name = "梵"; pic = pictrue[i]; break;
            case 12: name = "慧"; pic = pictrue[i]; break;
            case 13: name = "罪业"; pic = pictrue[i]; unlock('I', 7); break;
            case 14: name = "降魔"; pic = pictrue[i]; unlock('I', 5); break;
        }
        go.GetComponentInChildren<Text>().text = name;
        go.GetComponentInChildren<Image>().sprite = pic;
        PlayerPrefs.SetString("ach", ach);
        StartCoroutine(destroyHint(go));
    }

    //播放BOSS特写
    public void showBOSSHint(int i)
    {
        GameObject go = GameObject.Instantiate(BOSSHint) as GameObject;
        go.transform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>());
        go.GetComponent<Image>().sprite = boss[i - 1];
        StartCoroutine(destroyHint(go));
    }

    IEnumerator destroyHint(GameObject go)
    {
        yield return new WaitForSeconds(4);
        Destroy(go);
    }

    //创建成就面板
    public void createAchPanel()
    {
        GameObject go = GameObject.Instantiate(AchPanel) as GameObject;
        go.transform.SetParent(GameObject.Find("Canvas").transform);
        go.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
    }

    //淡入，使用协程执行，可能需要手动写等待时间
    public void beBlack(float smoothing)
    {
        if (blackPanel == null)
        {
            GameObject go = GameObject.Instantiate(BlackPanel) as GameObject;
            blackPanel = go;
            go.transform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>());
            go.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
            go.GetComponent<BlackPanel>().beBlack(smoothing);
        }
        else
        {
            blackPanel.GetComponent<BlackPanel>().beBlack(smoothing);
        }
    }

    //淡出，使用协程执行，可能需要手动写等待时间
    public void beWhite(float smoothing)
    {
        if (blackPanel == null)
        {
            GameObject go = GameObject.Instantiate(BlackPanel) as GameObject;
            blackPanel = go;
            go.transform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>());
            go.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
            Color color = go.GetComponent<RawImage>().color;
            color.a = 1;
            go.GetComponent<RawImage>().color = color;
            go.GetComponent<BlackPanel>().beWhite(smoothing);
        }
        else
        {
            blackPanel.GetComponent<BlackPanel>().beWhite(smoothing);
        }
    }

    IEnumerator back(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
        SceneManager.LoadScene(0);
    }

    //存档，保存数据
    public void Save()
    {
        string lx = "save" + gameManager.instance.gameState + "_" + gameManager.instance.saveIndex;
        playSE(save);
        PlayerPrefs.SetInt(lx, 1);
        PlayerPrefs.SetString("time" + gameManager.instance.gameState + "_" 
            + gameManager.instance.saveIndex,System.DateTime.Now.Month+"/"+System.DateTime.Now.Day+" "
            +System.DateTime.Now.Hour+":"+(System.DateTime.Now.Minute<10?("0"+ System.DateTime.Now.Minute) : System.DateTime.Now.Minute.ToString()));
        PlayerPrefs.SetString(lx + "Askill", Askill);
        PlayerPrefs.SetString(lx + "Nskill", Nskill);
        PlayerPrefs.SetString(lx + "item", item);
        PlayerPrefs.SetInt(lx + "spirit", spirit);
        PlayerPrefs.SetInt(lx + "kill", kill);
        int j= PlayerPrefs.GetInt(lx + "level"); ;
        if(gameManager.instance.level>j)
            PlayerPrefs.SetInt(lx + "level", level);

        GameObject go = Instantiate(HintPanel) as GameObject;
        go.transform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>());
        go.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
        StartCoroutine(destroySaveHint(go));
    }

    IEnumerator destroySaveHint(GameObject go)
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(go);
    }

    //读档，读取数据
    public void Load(string lx)
    {
        if (gameManager.instance.debug)
            spirit = 99999;
        else
            spirit = PlayerPrefs.GetInt(lx + "spirit");
        kill = PlayerPrefs.GetInt(lx + "kill");
        level = PlayerPrefs.GetInt(lx + "level");
        Askill = PlayerPrefs.GetString(lx + "Askill");
        Nskill = PlayerPrefs.GetString(lx + "Nskill");
        item = PlayerPrefs.GetString(lx + "item");

        for(int i = 0; i < 8; i++)
        {
            if (item.Contains(i.ToString()))
                buyItem(i);             
        }
    }

    //根据技能情况增加数据
    public void checkSkill(int i,bool take)
    {
        switch(i)
        {
            case 0:
                if(take)
                    fly = true;
                else
                    fly = false;
                break;
            case 1:
                if (take)
                {
                    MoveSpeed += 1; JumpForce += 1; canAttacked = true;
                }
                else
                {
                    MoveSpeed -= 1; JumpForce -= 1; canAttacked = false;
                }
                    break;
            case 2:
                if (take) { AttackValue += 15;AttackForce += 3;attackP += 0.2f; }
                else { AttackValue -= 15; AttackForce -= 3; attackP -= 0.2f; }
                break;
            case 3:
                if (take) { DefendValue += 20;miss += 0.15f; }
                else { DefendValue -= 20; miss -= 0.15f; }
                break;
            case 4:
                if (take) { Luck += 0.25f; }
                else { Luck -= 0.25f; }
                break;
            case 5:
                if (take) { crazy = true; }
                else { crazy = false; }
                break;
            case 6:
                if (take) { get = true; }
                else { get = false; }
                break;
            case 7:
                if (take) { MaxHp += 300; }
                else { MaxHp -= 300; }
                break;
            case 8:
                if (take) { rebirth = true; }
                else { rebirth = false; }
                break;
            default:break;
        }
    }

    public void checkBuff(int i, bool take)
    {
        switch (i)
        {
            case 0:
                if (take)
                {
                    AttackValue += 20;
                }else
                {
                    AttackValue -= 20;
                }
                break;
            case 1:
                if (take)
                {
                    DefendValue += 30;
                }
                else
                {
                    DefendValue += 30;
                }
                break;
            case 2:
                if (take)
                    noBreak = true;
                else
                    noBreak = false;
                break;
            case 3:
                if (take)
                    AgetHp = true;
                else
                    AgetHp = false;
                break;
            case 4:
                if (take)
                    AgetSp = true;
                else
                    AgetSp = false;
                break;
            case 5:
                if (take)
                {
                    Luck += 0.3f;
                }
                else
                {
                    Luck -= 0.3f;
                }
                break;
            default:break;
        }
        if (take)
            BattleManager.instance.nowBuff = i;
        else
            BattleManager.instance.nowBuff = -1;
    }

    //获得道具
    public void buyItem(int i)
    {
        switch (i)
        {
            case 0:AttackValue += 10;attackS += 0.15f;Luck += 0.1f; break;
            case 1:DefendValue += 10;MaxHp += 150;MaxSp += 100;break;
            case 2:Luck += 0.15f;attackAll = true;break;
            case 3:MoveSpeed += 1;JumpForce += 1;break;
            case 4:MaxSp += 100;AllSp = true;break;
            case 5:MaxHp += 150;AllHp = true;break;
            case 6:DefendValue += 10;getSpirit = true;break;
            case 7:getHp += 30;break;
            default:break;
        }
        if(!item.Contains(i.ToString()))
            item += "#" + i;
    }

    //获得技能
    public void buySkill(char type,int i)
    {
        if (type == 'A')
                Askill += "#" + i;
        else if (type == 'N')
                Nskill += "#" + i;
    }

    //解锁，跟在成就提示后面
    public void unlock(char type,int i)
    {
        switch (type)
         {
            case 'A':AskillYes += "#" + i; PlayerPrefs.SetString("AskillYes", AskillYes); break;
            case 'N':NskillYes += "#" + i; PlayerPrefs.SetString("NskillYes", NskillYes); break;
            case 'I': itemYes += "#" + i; PlayerPrefs.SetString("itemYes", itemYes); break;
            case 'M': MYes += i < 10 ? "#0" + i : "#" + i; PlayerPrefs.SetString("MYes", MYes); break;
        }
    }

    //改变困难度(待更新)
    public void changeHard(int i)
    {
        if (i == 0)
        {
            gameManager.instance.AttackValue += 25;
            gameManager.instance.DefendValue += 25;
            gameManager.instance.MaxHp += 100;
        }
        else if (i == 1)
        {
            gameManager.instance.AttackValue += 15;
            gameManager.instance.DefendValue += 15;
        }
        else if (i == 2)
        {

        }
    }
}

//每次加载一个新场景，首先设置的方法
//1.放一个游戏物体加载该场景的脚本
//2.放一个canvas和eventsystem，如果有角色移动戏并设置canvas参数，给摄像机挂载脚本
//3.在build setting里加入这个场景
//4.脚本里调用createGamePanel，加入刚进入场景的淡出




