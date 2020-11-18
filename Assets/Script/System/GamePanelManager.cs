using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//游戏面板
public class GamePanelManager : MonoBehaviour {
    //按钮和其他UI组件
    [SerializeField]
    private GameObject gameBackBtn;
    [SerializeField]
    private GameObject PlayerBtn;
    [SerializeField]
    private GameObject Spirit;
    [SerializeField]
    private GameObject leftMoveBtn;
    [SerializeField]
    private GameObject rightMoveBtn;
    [SerializeField]
    private GameObject upMoveBtn;
    [SerializeField]
    private GameObject downMoveBtn;
    [SerializeField]
    private GameObject HpBtn;
    [SerializeField]
    private GameObject jumpBtn;
    [SerializeField]
    private GameObject attackBtn;
    [SerializeField]
    private GameObject sp1Btn;
    [SerializeField]
    private Image sp1mask;
    [SerializeField]
    private GameObject sp2Btn;
    [SerializeField]
    private Image sp2mask;
    [SerializeField]
    private GameObject textPanel;
    [SerializeField]
    private GameObject HpSlider;
    [SerializeField]
    private GameObject HpText;
    [SerializeField]
    private GameObject SpSlider;
    [SerializeField]
    private GameObject SpText;
    public GameObject buffImage;
    [SerializeField]
    private GameObject BOSSSlider;
    [SerializeField]
    private GameObject BOSSStext;
    [SerializeField]
    private Image comboImage;
    [SerializeField]
    private GameObject comboSlider;

    //需要的内置变量
    [SerializeField]
    private Sprite UIMask;  //文字头像块透明蒙版
    [HideInInspector]
    public float move =0;   //按键操作的移动数值
    [HideInInspector]
    public bool Aup = false;    //上挑攻击开关
    [HideInInspector]
    public float AupTimer = 0;//上挑攻击计时器
    [HideInInspector]
    public bool Ad= false;    //下斩攻击开关
    [HideInInspector]
    public float AdTimer = 0;//下斩攻击计时器
    public bool inGetHp=false;  //是否在回血状态
    [HideInInspector]
    public bool showFinish=false;   //文字是否显示完成
    [HideInInspector]
    public int State = 0;         //游戏面板的状态，0表示只有一个返回按钮，1表示战斗面板，2表示人物面板
    private int targetHp;       //用于更新条
    private int targetSp;
    private int targetBOSSHp;
    private float sp1Timer = 0;
    private bool sp1wait=false;
    private float sp2Timer = 0;
    private bool sp2wait = false;
    public float spTime = 1;
    private Enemy BOSS;
    private bool onSp = false;
    private float spTimer = 0;

    public Sprite[] combos;

    //需要赋值的变量
    [HideInInspector]
    public GameObject player;      //代码赋值，当需要控制人物时把生成的人物给它赋值
    public float showCharTime = 0.05f;//文字显示速度

    //音效
    public AudioClip Btn;
    public AudioClip Yes;
    public AudioClip No;

    //进行初始化操作
    void Start() {
        
        Button btn = gameBackBtn.GetComponent<Button>();
        btn.onClick.AddListener(delegate () {
            this.OnClick(gameBackBtn, this.gameObject);
        });
    } 

    //设置需要的更新
    private void Update()
    {
        gameManager.instance.checkBackButton("game");
        gameManager.instance.checkMenuButton("game");

        if (State == 1)
        {
            if (sp1wait)
            {
                sp1Timer += Time.deltaTime;
                if (sp1Timer <= spTime)
                {
                    sp1mask.fillAmount = 1-sp1Timer;
                }
                else
                {
                    sp1wait = false;
                    sp1Timer = 0;
                }
            }
            if (sp2wait)
            {
                sp2Timer += Time.deltaTime;
                if (sp2Timer <= spTime)
                {
                    sp2mask.fillAmount = 1-sp2Timer;
                }
                else
                {
                    sp2wait = false;
                    sp2Timer = 0;
                }
            }
            if (onSp)
            {
                if (spTimer >= 1.3f)
                {
                    onSp = false;
                    spTimer = 0;
                }
            }
            if (Mathf.Abs(HpSlider.GetComponent<Slider>().value - targetHp) >= 5)
            {
                HpSlider.GetComponent<Slider>().value += HpSlider.GetComponent<Slider>().value > targetHp ? -5 : 5;
                HpText.GetComponent<Text>().text = targetHp + " / " + gameManager.instance.MaxHp;
            }
            else
            {
                HpSlider.GetComponent<Slider>().value += HpSlider.GetComponent<Slider>().value > targetHp ? -1 : 1;
                HpText.GetComponent<Text>().text = targetHp + " / " + gameManager.instance.MaxHp;
            }
            if (SpSlider.GetComponent<Slider>().value != targetSp)
            {
                SpSlider.GetComponent<Slider>().value += SpSlider.GetComponent<Slider>().value > targetSp ? -1 : 1;
                SpText.GetComponent<Text>().text = targetSp + " / " + gameManager.instance.MaxSp;
            }
            if (Mathf.Abs(BOSSSlider.GetComponent<Slider>().value - targetBOSSHp) >= 10)
            {
                BOSSSlider.GetComponent<Slider>().value += BOSSSlider.GetComponent<Slider>().value > targetBOSSHp ? -10 : 10;
            }
            else
            {
                BOSSSlider.GetComponent<Slider>().value += BOSSSlider.GetComponent<Slider>().value > targetBOSSHp ? -1 : 1;
            }
            Spirit.GetComponent<Text>().text = "" + gameManager.instance.spirit;
            if (Aup && AupTimer >0.5f)
                Aup = false;
            if (Ad && AdTimer > 0.5f)
                Ad = false;
        }
    }

    private void FixedUpdate() {
        if (State == 1)
        {
            setASkill(1, gameManager.instance.sp1);
            setASkill(2, gameManager.instance.sp2);
            if (player != null)
            {
                if (!onSp)
                {
                    if (gameManager.instance.control)
                        player.GetComponent<Player>().Move(move);
                    player.GetComponent<Player>().inGetHp = inGetHp;
                }
                player.GetComponent<Player>().onSp = onSp;
                HpSlider.GetComponent<Slider>().maxValue = gameManager.instance.MaxHp;
                targetHp = player.GetComponent<Player>().Hp;
                SpSlider.GetComponent<Slider>().maxValue = gameManager.instance.MaxSp;
                targetSp = player.GetComponent<Player>().Sp;
            }
            if (BOSS != null)
            {
                targetBOSSHp = BOSS.GetComponent<Enemy>().hp;
                if (BOSS.GetComponent<Enemy>().hp<=0)
                    BOSS = null;
            }
            if (Aup)
                AupTimer += Time.deltaTime;
            if (Ad)
                AdTimer += Time.deltaTime;
            if (onSp)
                spTimer += Time.deltaTime;
            comboSlider.GetComponent<Slider>().value = BattleManager.instance.combo;
            if (BattleManager.instance.combo < 20)
                comboImage.sprite = combos[0];
            else if (BattleManager.instance.combo < 50)
                comboImage.sprite = combos[1];
            else if (BattleManager.instance.combo < 80)
                comboImage.sprite = combos[2];
            else
                comboImage.sprite = combos[3];
        }
    }

    //面板按钮点击事件的设置(待更新)
    public void OnClick(GameObject sender, GameObject nowPanel)
    {
        switch (sender.name)
        {
            case "GameBackBtn":
                gameManager.instance.playSE(No);
                gameManager.instance.createPausePanel();
                break;
            case "PlayerBtn":
                gameManager.instance.playSE(Btn);
                showPlayerPanel();
                break;
            default:
                break;
        }
    }

    public void showBOSSSlider(Enemy Boss, int i)
    {
        this.BOSS = Boss;
        BOSSSlider.SetActive(true);
        BOSSSlider.GetComponent<Slider>().maxValue = Boss.GetComponent<Enemy>().hp;
        BOSSSlider.GetComponent<Slider>().value = Boss.GetComponent<Enemy>().hp;
        switch (i)
        {
            case 1: BOSSStext.GetComponent<Text>().text = "头领•焰"; break;
            case 2: BOSSStext.GetComponent<Text>().text = "弓王•逆"; break;
            case 3: BOSSStext.GetComponent<Text>().text = "刀客•血"; break;
            case 4: BOSSStext.GetComponent<Text>().text = "杀手•影"; break;
            case 5: BOSSStext.GetComponent<Text>().text = "亲卫•谛"; break;
        }
    }

    //显示控制人物的界面，要给player赋值
    public void showBattleBtn(GameObject player)         
    {
        State = 1;
        this.player = player;

        Button btn = PlayerBtn.GetComponent<Button>();
        btn.onClick.AddListener(delegate () {
            this.OnClick(PlayerBtn, this.gameObject);
        });

        HpText.GetComponent<Text>().text = gameManager.instance.MaxHp + " / " + gameManager.instance.MaxHp;
        SpText.GetComponent<Text>().text = 20 + " / " + gameManager.instance.MaxSp;
        HpSlider.GetComponent<Slider>().maxValue = gameManager.instance.MaxHp;
        HpSlider.GetComponent<Slider>().value = player.GetComponent<Player>().Hp;
        SpSlider.GetComponent<Slider>().maxValue = gameManager.instance.MaxSp;
        SpSlider.GetComponent<Slider>().value = player.GetComponent<Player>().Sp;

        leftMoveBtn.SetActive(true);
        rightMoveBtn.SetActive(true);
        if(gameManager.instance.jump)
            jumpBtn.SetActive(true);
        if (gameManager.instance.attack)
        {
            upMoveBtn.SetActive(true);
            downMoveBtn.SetActive(true);
            HpBtn.SetActive(true);
            attackBtn.SetActive(true);
            HpSlider.SetActive(true);
            SpSlider.SetActive(true);
            PlayerBtn.SetActive(true);
            sp1Btn.SetActive(true);
            sp2Btn.SetActive(true);
            setASkill(1, gameManager.instance.sp1);
            setASkill(2, gameManager.instance.sp2);
            comboSlider.SetActive(true);
        }
    }

    public void hideBattleBtn()
    {
        State = 0;
        if (player != null)
            player.GetComponent<Player>().Move(0);
        leftMoveBtn.SetActive(false);
        rightMoveBtn.SetActive(false);
        jumpBtn.SetActive(false);
        attackBtn.SetActive(false);
        PlayerBtn.SetActive(false);
        HpSlider.SetActive(false);
        SpSlider.SetActive(false);
        upMoveBtn.SetActive(false);
        downMoveBtn.SetActive(false);
        sp1Btn.SetActive(false);
        HpBtn.SetActive(false);
        sp2Btn.SetActive(false);
        comboSlider.SetActive(false);
        BOSSSlider.SetActive(false);
        inGetHp = false;
        Aup = false;
        AupTimer = 0;
        Ad = false;
        AdTimer = 0;
        move = 0;
    }

    //显示人物面板
    public void showPlayerPanel()
    {
        gameManager.instance.createPlayerPanel();
    }

    //显示文字面板(待更新)
    public void showTextPanel(string name,string text,char loc,Sprite sprite,string location)   //显示文字框
    {
        hideBattleBtn();
        textPanel.SetActive(true);//要先设置为active，之后的设置才有效
        if (location == "center")
            textPanel.transform.localPosition = new Vector2(0,0);
        else if (location == "bottom")
            textPanel.transform.localPosition = new Vector2(0,-300);
        textPanel.GetComponent<TextPanel>().setNameText(name);
        if (loc == 'L')
            textPanel.GetComponent<TextPanel>().setLeftAvator(sprite);
        else
            textPanel.GetComponent<TextPanel>().setRightAvator(sprite);
        StartCoroutine(showText(text));
    }

    public void hideTextPanel(bool main)
    {
        if(main)
            gameManager.instance.now++;
        textPanel.SetActive(false);
        showFinish = false;
    }

    public void showCG(Sprite sprite)
    {
        hideBattleBtn();
        Image image = this.gameObject.GetComponent<Image>();
        image.sprite = sprite;
        gameBackBtn.SetActive(false);
    }

    public void hideCG()
    {
        Image image = this.gameObject.GetComponent<Image>();
        image.sprite = UIMask;
        gameBackBtn.SetActive(true);
    }

    IEnumerator showText(string text)
    {
        char[] A = text.ToCharArray();
        string txt = "";
        foreach (var a in A)
        {
            if (!showFinish)
            {
                txt += a;
                textPanel.GetComponent<TextPanel>().setMainText(txt);
                yield return new WaitForSeconds(showCharTime);
            }
            else
            {
                textPanel.GetComponent<TextPanel>().setMainText(text);
                break;
            }
        }
        showFinish = true;
    }

    public void skip()
    {
        showFinish = true;
    }

    public void setASkill(int j,int i)
    {
        if (j == 1)
        {
            sp1Btn.GetComponent<Button>().image.sprite = gameManager.instance.skillImage[i];
        }
        else if (j == 2)
        {
            sp2Btn.GetComponent<Button>().image.sprite = gameManager.instance.skillImage[i];
        }
    }


    public void attack()
    {
        if (player != null && !onSp)
        {
            if (Aup)
            {
                player.GetComponent<Player>().AttackUp();
                Aup = false;
            }
            else if (Ad)
            {
                player.GetComponent<Player>().AttackDown();
                Ad = false;
            }
            else
                player.GetComponent<Player>().Attack();
        }
    }

    public void jump()
    {
        if (player != null && !onSp)
        {
            if (Ad)
            {
                gameManager.instance.downPlatform = true;
                Ad = false;
            }
            else
                player.GetComponent<Player>().Jump();
        }
    }

    public void sp1Start()
    {
        if (!sp1wait && !onSp)
        {
            if (player != null)
            {
                switch (gameManager.instance.sp1)
                {
                    case 0: player.GetComponent<Player>().AttackS1(); break;
                    case 1: player.GetComponent<Player>().AttackS2(); break;
                    case 2: player.GetComponent<Player>().AttackS3(); break;
                    case 3: player.GetComponent<Player>().addBuff(0); break;
                    case 4: player.GetComponent<Player>().addBuff(1); break;
                    case 5: player.GetComponent<Player>().addBuff(2); break;
                    case 6: player.GetComponent<Player>().addBuff(3); break;
                    case 7: player.GetComponent<Player>().addBuff(4); break;
                    case 8: player.GetComponent<Player>().addBuff(5); break;
                    default: break;
                }
                onSp = true;
            }
            sp1wait = true;
        }
    }

    public void sp2Start()
    {
        if (!sp2wait && !onSp)
        {
            if (player != null)
            {
                switch (gameManager.instance.sp2)
                {
                    case 0: player.GetComponent<Player>().AttackS1(); break;
                    case 1: player.GetComponent<Player>().AttackS2(); break;
                    case 2: player.GetComponent<Player>().AttackS3(); break;
                    case 3: player.GetComponent<Player>().addBuff(0); break;
                    case 4: player.GetComponent<Player>().addBuff(1); break;
                    case 5: player.GetComponent<Player>().addBuff(2); break;
                    case 6: player.GetComponent<Player>().addBuff(3); break;
                    case 7: player.GetComponent<Player>().addBuff(4); break;
                    case 8: player.GetComponent<Player>().addBuff(5); break;
                    default: break;
                }
                onSp = true;
            }
            sp2wait = true;
        }
    }

    public void upBtnDown()
    {
        AupTimer = 0;
        Aup = true;
    }

    public void downBtnDown()
    {
        AdTimer = 0;
        Ad = true;
    }
}
