using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PausePanelManager : MonoBehaviour {

    [SerializeField]
    private GameObject pauseQuitPanel;
    [SerializeField]
    private GameObject pauseSettingButton;
    [SerializeField]
    private GameObject pauseContinueButton;
    [SerializeField]
    private GameObject pauseQuitButton;

    public AudioClip Btn;
    public AudioClip Yes;
    public AudioClip No;

    // Use this for initialization
    void Start() {
        Time.timeScale = 0;
        List<GameObject> btnList = new List<GameObject>();
        btnList.Add(pauseSettingButton);
        btnList.Add(pauseContinueButton);
        btnList.Add(pauseQuitButton);

        foreach (var btnObj in btnList)
        {
            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(delegate () {
                this.OnClick(btnObj, this.gameObject);
            });
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1;
    }

    public void OnClick(GameObject sender, GameObject nowPanel)
    {
        switch (sender.name)
        {
            case "PauseContinueBtn":
                gameManager.instance.playSE(Yes);
                GameObject.Destroy(nowPanel);
                break;
            case "PauseSettingBtn":
                gameManager.instance.playSE(Btn);
                gameManager.instance.createSettingPanel();
                break;
            case "PauseQuitBtn":
                gameManager.instance.playSE(No);
                pauseQuitPanel.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void onQuitYes()
    {
        gameManager.instance.playSE(Yes);
        pauseQuitPanel.SetActive(false);
        gameManager.instance.Save();
        GameObject.Destroy(this.gameObject);
        gameManager.instance.hard = 0;
        gameManager.instance.level = 0;
        gameManager.instance.now = 0;
        gameManager.instance.gameState = 0;
        gameManager.instance.saveIndex = 0;
        gameManager.instance.back();
    }

    public void onQuitNo()
    {
        gameManager.instance.playSE(No);
        pauseQuitPanel.SetActive(false);
    }
}
