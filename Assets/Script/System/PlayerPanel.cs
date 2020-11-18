using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//角色面板
public class PlayerPanel : MonoBehaviour {
    //表示面板状态，0主界面，1技能，2阵牌，3资料
    private int state = 0;
    public Sprite[] ASkills;//9为UIMask
    public Sprite[] NSkills;//9为UIMask
    public Sprite[] items;
    public Sprite[] enemys;
    //======================主面板组件==========================
    public Button skillBtn;
    public Button itemBtn;
    public Button messageBtn;
    public Button backBtn;

    public Text Hp;
    public Text Sp;
    public Text attack;
    public Text defend;
    public Text speed;
    public Text luck;

    public GameObject skillPanel;
    public GameObject itemPanel;
    public GameObject messagePanel;

    public AudioClip Yes;
    public AudioClip No;
    public AudioClip Btn;

    private int index = 0;
    //======================技能面板组件========================
    private char ty = 'A';
    public int[] Acost;
    public int[] Ncost;
    public int[] Icost;

    public GameObject mask1;
    public GameObject mask2;
    public GameObject positivePanel;
    public GameObject negativePanel;
    public GameObject buyPanel;
    public GameObject takePanel;
    public GameObject skillAskPanel;
    public GameObject skillHintPanel;

    public Text BSkillname;
    public Image BSkillImage;
    public Text BskillDescribe;
    public Text TSkillname;
    public Image TSkillImage;
    public Text TskillDescribe;
    public Text skillCost;
    public Text spirit;
    public Image sp1Ima;
    public Image sp2Ima;
    public Button[] ASkillBtn;
    public Button[] NSkillBtn;

    //======================道具面板组件========================
    public Button[] IBtn;
    public Text Ispirit;
    public GameObject IbuyPanel;
    public GameObject ItakePanel;
    public GameObject IaskPanel;
    public GameObject IhintPanel;
    public Text IBuyname;
    public Image IBuyImage;
    public Text IBuyDescribe;
    public Text ICostText;
    public Text ITakename;
    public Image ITakeImage;
    public Text ITakeDescribe;

    //======================资料面板组件========================
    public Button[] MBtn;
    public SpriteRenderer pos;
    public Text MHp;
    public Text MAttack;
    public Text MDefend;
    public Text MSpirit;
    public Text MDescribe;

    public void reFresh()
    {
        Hp.text = gameManager.instance.MaxHp.ToString();
        Sp.text = gameManager.instance.MaxSp.ToString();
        attack.text = gameManager.instance.AttackValue.ToString();
        defend.text = gameManager.instance.DefendValue.ToString();
        speed.text = gameManager.instance.MoveSpeed.ToString();
        luck.text = (gameManager.instance.Luck * 100).ToString() + "%";
        for (int j = 0; j < 11; j++)
        {
            if (!gameManager.instance.MYes.Contains(j < 10 ? "0" + j : "" + j))
                MBtn[j].gameObject.SetActive(false);
        }
        if (gameManager.instance.gameState == 2)
            itemBtn.gameObject.SetActive(false);
    }

    void Start()
    {
        Time.timeScale = 0;
        reFresh();
    }

    private void OnDestroy()
    {
        Time.timeScale = 1;
    }

    public void onSkillBtnDown()
    {
        skillPanel.SetActive(true);
        updateSkillPanel1('A');
        updateSkillPanel2('A');
        state = 1;
    }

    public void onSkillBackBtnDown()
    {
        skillPanel.SetActive(false);
        index = 0;
        state = 0;
        ty = 'A';
    }

    public void onItemBtnDown()
    {
        itemPanel.SetActive(true);
        reFresh();
        updateItemPanel0();
        updateItemPanel2();
        state = 2;
    }

    public void onItemBackBtnDown()
    {
        itemPanel.SetActive(false);
        reFresh();
        index = 0;
        state = 0;
    }

    public void onMessageBtnDown()
    {
        messagePanel.SetActive(true);
        showExample(0);
        state = 3;
    }

    public void onMessageBackBtnDown()
    {
        messagePanel.SetActive(false);
        state = 0;
    }

    public void onAchBtnDown()
    {
        gameManager.instance.createAchPanel();
    }

    public void onBackBtnDown()
    {
        Destroy(this.gameObject);
    }

    //===========================技能面板方法====================

    //更新左侧面板
    public void updateSkillPanel1(char i)
    {
        if (i == 'A')
        {
            ty = 'A';
            positivePanel.SetActive(true);
            negativePanel.SetActive(false);

            for(int j = 0; j < ASkillBtn.Length; j++)
            {
                if (!gameManager.instance.AskillYes.Contains(j.ToString()))
                    ASkillBtn[j].gameObject.SetActive(false);
            }
            updatePosSkillPanel(0);
        }else if (i == 'N')
        {
            ty = 'N';
            positivePanel.SetActive(false);
            negativePanel.SetActive(true);

            for (int j = 0; j < NSkillBtn.Length; j++)
            {
                if (!gameManager.instance.NskillYes.Contains(j.ToString()))
                    NSkillBtn[j].gameObject.SetActive(false);
            }
            updateNegSkillPanel(0);
        }
    }

    //技能选择按钮
    public void updatePosSkillPanel(int i)
    {
        index = i;
        if (gameManager.instance.Askill.Contains(i.ToString()))
        {
            takePanel.SetActive(true);
            buyPanel.SetActive(false);
            setTakePanel('A',i);
        }
        else
        {
            takePanel.SetActive(false);
            buyPanel.SetActive(true);
            setBuyPanel('A', i);
        }
    }

    public void updateNegSkillPanel(int i)
    {
        index = i;
        if (gameManager.instance.Nskill.Contains(i.ToString()))
        {
            takePanel.SetActive(true);
            buyPanel.SetActive(false);
            setTakePanel('N', i);
        }
        else
        {
            takePanel.SetActive(false);
            buyPanel.SetActive(true);
            setBuyPanel('N', i);
        }
    }

    //更新右侧面板
    public void setTakePanel(char type,int i)
    {
        if (type == 'A')
        {
            switch (i)
            {
                case 0:TSkillname.text = "突进";TskillDescribe.text = "向前快速直线移动并攻击路线上所有敌人，自身无敌"; break;
                case 1: TSkillname.text = "重斩"; TskillDescribe.text = "重击身下地面给两侧的敌人造成伤害并附加击退击倒效果"; break;
                case 2: TSkillname.text = "连环斩"; TskillDescribe.text = "对自己面前连续施加斩击，造成大量伤害"; break;
                case 3: TSkillname.text = "剑闪"; TskillDescribe.text = "在一段时间内提升自己的攻击力"; break;
                case 4: TSkillname.text = "凝练"; TskillDescribe.text = "在一段时间内提升自己的防御力"; break;
                case 5: TSkillname.text = "不破"; TskillDescribe.text = "在极短时间内使敌人对你只能造成固定的伤害"; break;
                case 6: TSkillname.text = "盛华"; TskillDescribe.text = "在一段时间内缓慢回复HP"; break;
                case 7: TSkillname.text = "沐血"; TskillDescribe.text = "在一段时间内缓慢回复SP"; break;
                case 8: TSkillname.text = "虔诚"; TskillDescribe.text = "在一段时间内提升幸运值"; break;
                default: break;
            }
            TSkillImage.sprite = ASkills[i];
        }
        else if (type == 'N')
        {
            switch (i)
            {
                case 0: TSkillname.text = "燕影"; TskillDescribe.text = "允许角色二段跳"; break;
                case 1: TSkillname.text = "灵动"; TskillDescribe.text = "提升移动速度，跳跃力，并且没有受伤动作"; break;
                case 2: TSkillname.text = "狂舞"; TskillDescribe.text = "提升攻击力和击退能力，提高破防率"; break;
                case 3: TSkillname.text = "坚韧"; TskillDescribe.text = "提升防御力，增加闪避率"; break;
                case 4: TSkillname.text = "幸运"; TskillDescribe.text = "提升幸运值"; break;
                case 5: TSkillname.text = "暴乱"; TskillDescribe.text = "使角色固定在一套连招最后一击发动大暴击"; break;
                case 6: TSkillname.text = "汲取"; TskillDescribe.text = "攻击敌人可以回复自己的生命"; break;
                case 7: TSkillname.text = "休养"; TskillDescribe.text = "提升HP上限"; break;
                case 8: TSkillname.text = "重生"; TskillDescribe.text = "死亡后有65%的概率原地复活，次数不限"; break;
                default: break;
            }
            TSkillImage.sprite = NSkills[i];
        }

    }

    public void setBuyPanel(char type,int i)
    {
        if (type == 'A')
        {
            switch (i)
            {
                case 0: BSkillname.text = "突进"; BskillDescribe.text = "向前快速直线移动并攻击路线上所有敌人，自身无敌";skillCost.text = Acost[0].ToString(); break;
                case 1: BSkillname.text = "重斩"; BskillDescribe.text = "重击身下地面给两侧的敌人造成伤害并附加击退击倒效果"; skillCost.text = Acost[1].ToString(); break;
                case 2: BSkillname.text = "连环斩"; BskillDescribe.text = "对自己面前连续施加斩击，造成大量伤害"; skillCost.text = Acost[2].ToString(); break;
                case 3: BSkillname.text = "剑闪"; BskillDescribe.text = "在一段时间内提升自己的攻击力"; skillCost.text = Acost[3].ToString(); break;
                case 4: BSkillname.text = "凝练"; BskillDescribe.text = "在一段时间内提升自己的防御力"; skillCost.text = Acost[4].ToString(); break;
                case 5: BSkillname.text = "不破"; BskillDescribe.text = "在极短时间内使敌人对你只能造成固定的伤害"; skillCost.text = Acost[5].ToString(); break;
                case 6: BSkillname.text = "盛华"; BskillDescribe.text = "在一段时间内缓慢回复HP"; skillCost.text = Acost[6].ToString(); break;
                case 7: BSkillname.text = "沐血"; BskillDescribe.text = "在一段时间内缓慢回复SP"; skillCost.text = Acost[7].ToString(); break;
                case 8: BSkillname.text = "虔诚"; BskillDescribe.text = "在一段时间内提升幸运值"; skillCost.text = Acost[8].ToString(); break;
                default: break;
            }
            BSkillImage.sprite = ASkills[i];
        }
        else if (type == 'N')
        {
            switch (i)
            {
                case 0: BSkillname.text = "燕影"; BskillDescribe.text = "允许角色二段跳"; skillCost.text = Ncost[0].ToString(); break;
                case 1: BSkillname.text = "灵动"; BskillDescribe.text = "提升移动速度，跳跃力，并且没有受伤动作"; skillCost.text = Ncost[1].ToString(); break;
                case 2: BSkillname.text = "狂舞"; BskillDescribe.text = "提升攻击力和击退能力，提高破防率"; skillCost.text = Ncost[2].ToString(); break;
                case 3: BSkillname.text = "坚韧"; BskillDescribe.text = "提升防御力，增加闪避率"; skillCost.text = Ncost[3].ToString(); break;
                case 4: BSkillname.text = "幸运"; BskillDescribe.text = "提升幸运值"; skillCost.text = Ncost[4].ToString(); break;
                case 5: BSkillname.text = "暴乱"; BskillDescribe.text = "使角色固定在一套连招最后一击发动大暴击"; skillCost.text = Ncost[5].ToString(); break;
                case 6: BSkillname.text = "汲取"; BskillDescribe.text = "攻击敌人可以回复自己的生命"; skillCost.text = Ncost[6].ToString(); break;
                case 7: BSkillname.text = "休养"; BskillDescribe.text = "提升HP上限"; skillCost.text = Ncost[7].ToString(); break;
                case 8: BSkillname.text = "重生"; BskillDescribe.text = "死亡后有65%的概率原地复活，次数不限"; skillCost.text = Ncost[8].ToString(); break;
                default: break;
            }
            BSkillImage.sprite = NSkills[i];
        }
    }

    //更新上方装备图标和灵魂数
    public void updateSkillPanel2(char i)
    {
        if (i == 'A')
        {
            sp1Ima.sprite = ASkills[gameManager.instance.sp1];
            sp2Ima.sprite = ASkills[gameManager.instance.sp2];
        }
        else if (i == 'N')
        {
            sp1Ima.sprite = NSkills[gameManager.instance.skill1];
            sp2Ima.sprite = NSkills[gameManager.instance.skill2];
        }
        spirit.text = gameManager.instance.spirit.ToString();
    }

    //按下主动技能选择按钮
    public void onPositiveBtn()
    {
        mask1.SetActive(true);
        mask2.SetActive(false);
        updateSkillPanel1('A');
        updateSkillPanel2('A');
    }

    //按下被动技能选择按钮
    public void onNegativeBtn()
    {
        mask1.SetActive(false);
        mask2.SetActive(true);
        updateSkillPanel1('N');
        updateSkillPanel2('N');
    }

    //按下购买按钮
    public void buySkill()
    {
        skillAskPanel.SetActive(true);
    }

    public void skillBuyYes()
    {
        gameManager.instance.playSE(Yes);
        if (gameManager.instance.spirit < (ty=='A'?Acost[index]:Ncost[index]))
            skillHintPanel.SetActive(true);
        else
        {
            gameManager.instance.spirit -= (ty == 'A' ? Acost[index] : Ncost[index]);
            if (ty == 'A')
                gameManager.instance.buySkill('A', index);
            else if (ty == 'N')
                gameManager.instance.buySkill('N', index);
            updateSkillPanel2(ty);
            takePanel.SetActive(true);
            buyPanel.SetActive(false);
            setTakePanel(ty, index);
            skillAskPanel.SetActive(false);
        }
    }

    public void skillBuyNo()
    {
        gameManager.instance.playSE(No);
        skillAskPanel.SetActive(false);
    }

    //按下装备按钮            主动技能块待更新
    public void takeSkill(int i)
    {
        if (ty == 'A')
        {
            if (i == 0)
                gameManager.instance.sp1 = index;
            else if (i == 1)
                gameManager.instance.sp2 = index;
            updateSkillPanel2(ty);
        }
        else if (ty == 'N')
        {
            if (i == 0)
            {
                gameManager.instance.checkSkill(gameManager.instance.skill1, false);
                gameManager.instance.skill1 = index;
            }
            else if (i == 1)
            {
                gameManager.instance.checkSkill(gameManager.instance.skill2, false);
                gameManager.instance.skill2 = index;
            }
            gameManager.instance.checkSkill(index, true);
            updateSkillPanel2(ty);
        }
    }

    //===========================道具面板方法====================

    public void updateItemPanel0()
    {
        for (int j = 0; j < IBtn.Length; j++)
        {
            if (!gameManager.instance.itemYes.Contains(j.ToString()))
                IBtn[j].gameObject.SetActive(false);
        }
        updateItemPanel1(0);
    }

    public void updateItemPanel1(int i)
    {
        index = i;
        if (gameManager.instance.item.Contains(i.ToString()))
        {
            ItakePanel.SetActive(true);
            IbuyPanel.SetActive(false);
            setITakePanel(i);
        }
        else
        {
            ItakePanel.SetActive(false);
            IbuyPanel.SetActive(true);
            setIBuyPanel(i);
        }
    }

    //更新右侧面板
    public void setITakePanel(int i)
    {
            switch (i)
            {
                case 0: ITakename.text = "乾"; ITakeDescribe.text = "增加攻击和暴击和幸运"; break;
                case 1: ITakename.text = "坤"; ITakeDescribe.text = "增加防御，HP上限，SP上限"; break;
                case 2: ITakename.text = "震"; ITakeDescribe.text = "增加幸运，有概率在受伤时给予全部敌人伤害"; break;
                case 3: ITakename.text = "巽"; ITakeDescribe.text = "增加移动速度和跳跃"; break;
                case 4: ITakename.text = "坎"; ITakeDescribe.text = "增加SP上限，有极小概率在受伤时把SP回满"; break;
                case 5: ITakename.text = "离"; ITakeDescribe.text = "增加HP上限，有极小概率在受伤时把HP回满"; break;
                case 6: ITakename.text = "艮"; ITakeDescribe.text = "增加防御，小概率在受伤时获得灵力"; break;
                case 7: ITakename.text = "兑"; ITakeDescribe.text = "增加回复获得HP"; break;
                default: break;
            }
            ITakeImage.sprite = items[i];

    }

    public void setIBuyPanel(int i)
    {
        switch (i)
        {
            case 0: IBuyname.text = "乾"; IBuyDescribe.text = "增加攻击和暴击和幸运"; ICostText.text = Icost[0].ToString(); break;
            case 1: IBuyname.text = "坤"; IBuyDescribe.text = "增加防御，HP上限，SP上限"; ICostText.text = Icost[1].ToString(); break;
            case 2: IBuyname.text = "震"; IBuyDescribe.text = "增加幸运，有概率在受伤时给予全部敌人伤害"; ICostText.text = Icost[2].ToString(); break;
            case 3: IBuyname.text = "巽"; IBuyDescribe.text = "增加移动速度和跳跃"; ICostText.text = Icost[3].ToString(); break;
            case 4: IBuyname.text = "坎"; IBuyDescribe.text = "增加SP上限，有极小概率在受伤时把SP回满"; ICostText.text = Icost[4].ToString(); break;
            case 5: IBuyname.text = "离"; IBuyDescribe.text = "增加HP上限，有极小概率在受伤时把HP回满"; ICostText.text = Icost[5].ToString(); break;
            case 6: IBuyname.text = "艮"; IBuyDescribe.text = "增加防御，小概率在受伤时获得灵力"; ICostText.text = Icost[6].ToString(); break;
            case 7: IBuyname.text = "兑"; IBuyDescribe.text = "增加回复获得HP"; ICostText.text = Icost[7].ToString(); break;
            default:break;
        }
        IBuyImage.sprite = items[i];
    }

    public void buyItem()
    {
        IaskPanel.SetActive(true);
    }

    public void itemBuyYes()
    {
        gameManager.instance.playSE(Yes);
        if (gameManager.instance.spirit < Icost[index])
            IhintPanel.SetActive(true);
        else
        {
            gameManager.instance.spirit -= Icost[index];
            gameManager.instance.buyItem(index);
            updateItemPanel2();
            ItakePanel.SetActive(true);
            IbuyPanel.SetActive(false);
            setITakePanel(index);
            IaskPanel.SetActive(false);
        }
    }

    public void itemBuyNo()
    {
        gameManager.instance.playSE(No);
        IaskPanel.SetActive(false);
    }

    //更新上方装备图标和灵魂数
    public void updateItemPanel2()
    {
        Ispirit.text = gameManager.instance.spirit.ToString();
    }

    //===========================资料面板方法====================
    public void showExample(int i)
    {
        Enemy example = gameManager.instance.enmeyList[i].GetComponent<Enemy>();
        pos.sprite = enemys[i];
        MHp.text = example.hp.ToString();
        MAttack.text= example.attackValue.ToString();
        MDefend.text= example.defendValue.ToString();
        MSpirit.text= example.spirit.ToString();

        switch (i)
        {
            case 0:MDescribe.text = "普通的匈奴步兵，匈奴军队最普通的成员"; break;
            case 1: MDescribe.text = "弓箭手把他们的破木头弓视为至宝"; break;
            case 2: MDescribe.text = "为什么他们穿着夜行衣却叫尖兵不叫刺客呢？"; break;
            case 3: MDescribe.text = "据说他们的剑是刻舟求来的"; break;
            case 4: MDescribe.text = "小心这些枪兵！他们会在你靠近的时候反戈一击！"; break;
            case 5: MDescribe.text = "哲学的肌肉兄贵，无需解释"; break;
            case 6: MDescribe.text = "焰总是带着奇怪的面具，据说他在玩火的时候毁了容"; break;
            case 7: MDescribe.text = "逆才能真正称为百步穿杨，他最多可以一次射出5只箭"; break;
            case 8: MDescribe.text = "血的刀法极其的快，离他太近可不是明智之举"; break;
            case 9: MDescribe.text = "人如其名，影神出鬼没，他随时可能出现在你身边给你突然一击！"; break;
            case 10: MDescribe.text = "谛是单于最可靠的亲卫，善长刀法和箭术，绝对不是等闲之辈！"; break;
            default: MDescribe.text = "暂无资料"; break;
        }
    }
}
