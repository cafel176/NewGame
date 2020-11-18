using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextPanel : MonoBehaviour {
    [SerializeField]
    private GameObject mainText;
    [SerializeField]
    private GameObject nameText;
    [SerializeField]
    private GameObject namePanel;
    [SerializeField]
    private GameObject leftAvator;
    [SerializeField]
    private GameObject rightAvator;

    public void setMainText(string txt)
    {
        mainText.GetComponent<Text>().text = txt;
    }

    public void setNameText(string txt)
    {
        if (txt == "hide")
        {
            namePanel.SetActive(false);
            nameText.SetActive(false);
        }
        else
        {
            namePanel.SetActive(true);
            nameText.SetActive(true);
            nameText.GetComponent<Text>().text = txt;
        }
    }

    public void setLeftAvator(Sprite avator)
    {
        if (avator == null)
        {
            rightAvator.SetActive(false);
            leftAvator.SetActive(false);
        }
        else
        {
            rightAvator.SetActive(false);
            leftAvator.SetActive(true);
            leftAvator.GetComponent<Image>().sprite = avator;
        }
    }

    public void setRightAvator(Sprite avator)
    {
        if (avator == null)
        {
            rightAvator.SetActive(false);
            leftAvator.SetActive(false);
        }
        else
        {
            leftAvator.SetActive(false);
            rightAvator.SetActive(true);
            rightAvator.GetComponent<Image>().sprite = avator;
        }
    }
}
