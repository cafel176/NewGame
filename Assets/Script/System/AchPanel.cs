using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//成就面板
public class AchPanel : MonoBehaviour {

    public GameObject[] achMask;
    public AudioClip no;

    public void onBackBtn()
    {
        gameManager.instance.playSE(no);
        Destroy(this.gameObject);
    }

    private void Awake()
    {
        for (int i = 0; i < achMask.Length; i++)
        {
            if (gameManager.instance.ach.Contains(i < 10 ? "0" + i : "" + i))
            {
                achMask[i].SetActive(true);
            }
        }
    }
}
