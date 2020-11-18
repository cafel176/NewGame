using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScene : MonoBehaviour {
    //界面组件
    public Button startBtn1;
    public Button startBtn2;
    public Button quitBtn;
    public Button newBtn;
    public Button continueBtn;
    public Button backBtn;

    public GameObject easyMask;
    public GameObject normalMask;
    public GameObject hardMask;
    public Text text;
    public GameObject[] levels;

    public Button save1;
    public Text time1;
    public Button save2;
    public Text time2;
    public Button save3;
    public Text time3;

    public GameObject Panel;
    public GameObject savePanel;
    public GameObject HintPanel;
    public GameObject ChoosePanel;

    //音乐
    public AudioClip BGM;
    public AudioClip Btn;
    public AudioClip Yes;
    public AudioClip No;

    private int state=0;    //1是开始按钮，2是继续按妞
    private int j = 0;      //暂存存档序号

    void Start () {
        gameManager.instance.beWhite(0.02f);
        gameManager.instance.playBgm(BGM);
	}
	
	void Update () {
        gameManager.instance.checkBackButton("main");
        gameManager.instance.checkMenuButton("main");

    }

    public void showBtns1()
    {
        startBtn1.GetComponent<Animator>().SetTrigger("right");
        startBtn2.GetComponent<Animator>().SetTrigger("right");
        quitBtn.GetComponent<Animator>().SetTrigger("right");
    }

    public void hideBtns1()
    {
        startBtn1.GetComponent<Animator>().SetTrigger("left");
        startBtn2.GetComponent<Animator>().SetTrigger("left");
        quitBtn.GetComponent<Animator>().SetTrigger("left");
    }

    public void showBtns2()
    {
        if (newBtn.IsActive())
        {
            newBtn.GetComponent<Animator>().SetTrigger("right");
            continueBtn.GetComponent<Animator>().SetTrigger("right");
            backBtn.GetComponent<Animator>().SetTrigger("right");
        }
        else
        {
            newBtn.gameObject.SetActive(true);
            continueBtn.gameObject.SetActive(true);
            backBtn.gameObject.SetActive(true);
        }
    }

    public void hideBtns2()
    {
        newBtn.GetComponent<Animator>().SetTrigger("left");
        continueBtn.GetComponent<Animator>().SetTrigger("left");
        backBtn.GetComponent<Animator>().SetTrigger("left");
    }

    public void onStartBtn1Down()
    {
        gameManager.instance.gameState = 1;
        gameManager.instance.playSE(Yes);
        StartCoroutine(changeBtn(gameManager.instance.gameState));
    }

    public void onStartBtn2Down()
    {
        gameManager.instance.gameState = 2;
        gameManager.instance.playSE(Yes);
        StartCoroutine(changeBtn(gameManager.instance.gameState));
    }

    public void onQuitBtnDown()
    {
        gameManager.instance.playSE(No);
        gameManager.instance.Exit();
    }

    IEnumerator changeBtn(int state)
    {
        if (state == 0)
        {
            hideBtns2();
            yield return new WaitForSeconds(1f);
            showBtns1();
        }
        else
        {
            hideBtns1();
            yield return new WaitForSeconds(1f);
            showBtns2();
        }
    }

    public void onNewBtnDown()
    {
        gameManager.instance.playSE(Yes);
        state = 1;
        savePanel.SetActive(true);
        updateSave();
    }

    public void onContinueBtnDown()
    {
        gameManager.instance.playSE(Btn);
        state = 2;
        savePanel.SetActive(true);
        updateSave();
    }

    IEnumerator destroyHint(GameObject go)
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(go);
    }

    public void onBackBtnDown()
    {
        gameManager.instance.gameState = 0;
        gameManager.instance.playSE(Yes);
        StartCoroutine(changeBtn(gameManager.instance.gameState));
    }

    public void onSettingBtnDown()
    {
        gameManager.instance.playSE(Btn);
        gameManager.instance.createSettingPanel();
    }

    public void onAchBtnDown()
    {
        gameManager.instance.playSE(Btn);
        gameManager.instance.createAchPanel();
    }

    public void onAboutBtnDown()
    {
        gameManager.instance.playSE(Btn);
        gameManager.instance.showMakerName();
    }

    //=====================存档面板====================

    public void updateSave()
    {
        if (PlayerPrefs.GetInt("save"+ gameManager.instance.gameState+"_1") == 1)
        {
            time1.text = PlayerPrefs.GetString("time" + gameManager.instance.gameState + "_1");
        }
        else
        {
            time1.text = "无存档";
        }
        if (PlayerPrefs.GetInt("save" + gameManager.instance.gameState + "_2") == 1)
        {
            time2.text = PlayerPrefs.GetString("time" + gameManager.instance.gameState + "_2");
        }
        else
        {
            time2.text = "无存档";
        }
        if (PlayerPrefs.GetInt("save" + gameManager.instance.gameState + "_3") == 1)
        {
            time3.text = PlayerPrefs.GetString("time" + gameManager.instance.gameState + "_3");
        }
        else
        {
            time3.text = "无存档";
        }
    }

    //按下存档按妞的方法，三个按钮分别是1,2,3
    public void onSaveBtnDown(int i)
    {
        gameManager.instance.playSE(Yes);
        if (state == 1)
        {
            if (PlayerPrefs.GetInt("save" + gameManager.instance.gameState + "_"+i) == 0)//开始新游戏且无存档
            {
                gameManager.instance.level = 0;
                gameManager.instance.saveIndex = i;
                savePanel.SetActive(false);
                ChoosePanel.SetActive(true);
                updateLevelPanel(gameManager.instance.level);
            }
            else if (PlayerPrefs.GetInt("save" + gameManager.instance.gameState + "_"+i) == 1)//开始新游戏但有存档
            {
                j = i;
                Panel.SetActive(true);
            }
        }
        else if (state == 2)
        {
            if (PlayerPrefs.GetInt("save" + gameManager.instance.gameState + "_" + i) == 1)//继续游戏有存档     //待更新
            {
                gameManager.instance.Load("save" + gameManager.instance.gameState + "_" + i);
                gameManager.instance.saveIndex = i;
                ChoosePanel.SetActive(true);
                updateLevelPanel(gameManager.instance.level);
            }
            else    //继续游戏无存档
            {
                GameObject go = Instantiate(HintPanel) as GameObject;
                go.GetComponentInChildren<Text>().text = "没有可用的存档！";
                go.transform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>());
                go.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
                StartCoroutine(destroyHint(go));
            }
        }
    }

    //存档面板的返回按钮
    public void onSaveNoBtnPressed()
    {
        gameManager.instance.playSE(No);
        savePanel.SetActive(false);
        state = 0;
    }

    //覆盖存档面板按钮
    public void onYesBtnPressed()
    {
        gameManager.instance.playSE(Yes);
        gameManager.instance.level = 0;
        gameManager.instance.saveIndex = j;
        savePanel.SetActive(false);
        Panel.SetActive(false);
        gameManager.instance.Save();
        ChoosePanel.SetActive(true);
        updateLevelPanel(gameManager.instance.level);
    }

    public void onNoBtnPressed()
    {
        gameManager.instance.playSE(No);
        Panel.SetActive(false);
    }

    public void onChooseNo()
    {
        ChoosePanel.SetActive(false);
        gameManager.instance.hard = 0;
    }

    //选择困难度的按钮，0简单，1普通，2困难
    public void chooseHard(int i)
    {
        gameManager.instance.hard = i;
        if (i == 0)
        {
            easyMask.SetActive(true);
            normalMask.SetActive(false);
            hardMask.SetActive(false);
        }
        else if (i == 1)
        {
            normalMask.SetActive(true);
            easyMask.SetActive(false);
            hardMask.SetActive(false);
        }
        if (i == 2)
        {
            hardMask.SetActive(true);
            easyMask.SetActive(false);
            normalMask.SetActive(false);
        }
    }

    //选择关卡的按钮
    public void chooseLevel(int i)
    {
        gameManager.instance.changeHard(gameManager.instance.hard);
        gameManager.instance.level = i;
        gameManager.instance.now = 0;
        //进入游戏 
        switch (i)
        {
            case 0: gameManager.instance.loadScene(gameManager.instance.gameState == 1 ? 2 : 8); break;
            case 1: gameManager.instance.loadScene(gameManager.instance.gameState == 1 ? 3 : 8); break;
            case 2: gameManager.instance.loadScene(gameManager.instance.gameState == 1 ? 4 : 9); break;
            case 3: gameManager.instance.loadScene(gameManager.instance.gameState == 1 ? 5 : 10); break;
            case 4: gameManager.instance.loadScene(gameManager.instance.gameState == 1 ? 6 : 11); break;
            case 5: gameManager.instance.loadScene(gameManager.instance.gameState == 1 ? 7 : 12); break;
            default:break;
        }
    }

    //刷新面板
    public void updateLevelPanel(int level)
    {
        chooseHard(0);
        if (gameManager.instance.gameState == 2)
        {
            levels[0].gameObject.SetActive(false);
            levels[1].gameObject.SetActive(true);
            levels[5].gameObject.SetActive(false);
            text.text = "挑战模式";
            if (!gameManager.instance.debug2)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (i != level)
                        levels[i + 1].SetActive(false);
                }
            }
        }
        else
        {
            levels[0].gameObject.SetActive(true);
            text.text = "剧情模式";
            if (!gameManager.instance.debug2)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (i > level)
                        levels[i].SetActive(false);
                }
            }
        }

    }


    public void debug()
    {
        if (gameManager.instance.debug)
        {
            GameObject go = Instantiate(HintPanel) as GameObject;
            gameManager.instance.debug = false;
            go.GetComponentInChildren<Text>().text = "测试模式关闭！";
            go.transform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>());
            go.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
            StartCoroutine(destroyHint(go));
        }
        else
        {
            gameManager.instance.spirit = 99999;
            GameObject go = Instantiate(HintPanel) as GameObject;
                gameManager.instance.debug = true;
                go.GetComponentInChildren<Text>().text = "测试模式开启！";
            go.transform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>());
            go.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
            StartCoroutine(destroyHint(go));
        }
    }

    public void debug2()
    {
        if (gameManager.instance.debug2)
        {
            GameObject go = Instantiate(HintPanel) as GameObject;
            gameManager.instance.debug2 = false;
            go.GetComponentInChildren<Text>().text = "选关模式关闭！";
            go.transform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>());
            go.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
            StartCoroutine(destroyHint(go));
        }
        else
        {
            GameObject go = Instantiate(HintPanel) as GameObject;
            gameManager.instance.debug2 = true;
            go.GetComponentInChildren<Text>().text = "选关模式开启！";
            go.transform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>());
            go.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
            StartCoroutine(destroyHint(go));
        }
    }
}
