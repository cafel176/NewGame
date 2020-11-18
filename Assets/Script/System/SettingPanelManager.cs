using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanelManager : MonoBehaviour {

    [SerializeField]
    private GameObject settingBack;
    [SerializeField]
    private GameObject settingAbout;
    [SerializeField]
    private GameObject aboutPanel;

    public AudioClip Yes;
    public AudioClip No;
    public Slider musicSlider;
    public Slider SESlider;

    // Use this for initialization
    void Start() {
        Button btn = settingBack.GetComponent<Button>();
            btn.onClick.AddListener(delegate () {
                this.OnClick(settingBack, this.gameObject);
            });

        Button btn2 = settingAbout.GetComponent<Button>();
        btn2.onClick.AddListener(delegate () {
            this.OnClick(settingAbout, this.gameObject);
        });
        musicSlider.value = gameManager.instance.music;
        SESlider.value = gameManager.instance.SE;
    }
    public void OnClick(GameObject sender, GameObject nowPanel)
    {
        switch (sender.name)
        {
            case "SettingBackBtn":
                gameManager.instance.playSE(No);
                Destroy(nowPanel);
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameManager.instance.playSE(No);
            Destroy(this.gameObject);
        }
    }

    public void saveChange1()
    {
        gameManager.instance.music = musicSlider.value;
        PlayerPrefs.SetFloat("music", gameManager.instance.music);
    }

    public void saveChange2()
    {
        gameManager.instance.SE = SESlider.value;
        PlayerPrefs.SetFloat("se", gameManager.instance.SE);
    }
}
