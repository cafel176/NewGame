using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class levelPanel : MonoBehaviour {

    public Text time;
    public Text kill;
    public Text combo;
    public Text hurt;
    public Text final;
    public Image image;
    public Sprite[] finals;

	void Start () {
        time.text = BattleManager.instance.levelTimer.ToString();
        kill.text = BattleManager.instance.levelKill.ToString();
        combo.text = BattleManager.instance.comboScore.ToString();
        hurt.text = BattleManager.instance.hurt.ToString();
        int i=0;
        if (BattleManager.instance.levelTimer < 600)
        {
            if (!gameManager.instance.ach.Contains("14"))
                gameManager.instance.getAchievemrnt(14);
            i = 5000;
        }
        else if (BattleManager.instance.levelTimer < 900)
            i = 3000;
        else if (BattleManager.instance.levelTimer < 1200)
            i = 1000;
        if (BattleManager.instance.hurt < 10)
        {
            if (!gameManager.instance.ach.Contains("06"))
                gameManager.instance.getAchievemrnt(6);
            i += 5000;
        }
        else if (BattleManager.instance.hurt < 15)
            i += 3000;
        else if (BattleManager.instance.hurt < 20)
            i += 1000;
        i += (BattleManager.instance.levelKill * 100+ BattleManager.instance.comboScore);
        final.text = i.ToString();
        if (i > 15000)
        {
            image.sprite = finals[0];
            if (!gameManager.instance.ach.Contains("04"))
                gameManager.instance.getAchievemrnt(4);
        }
        else if (i > 12000)
            image.sprite = finals[1];
        else if (i > 8000)
            image.sprite = finals[2];
        else
            image.sprite = finals[3];
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android ? Input.GetTouch(0).phase == TouchPhase.Began : Input.GetKeyDown(KeyCode.Z))
        {
            BattleManager.instance.endLevel();
            gameManager.instance.now++;
            Destroy(this.gameObject);
        }
    }
}
