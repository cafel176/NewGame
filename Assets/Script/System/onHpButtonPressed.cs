using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class onHpButtonPressed : MonoBehaviour,IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    private GamePanelManager gamePanelManager;
    // Use this for initialization
    void Start() {
        gamePanelManager = this.gameObject.GetComponentInParent<GamePanelManager>();

    }

    // 当按钮被按下后系统自动调用此方法
    public void OnPointerDown(PointerEventData eventData)
    {
        gamePanelManager.inGetHp=true;
    }

    // 当按钮抬起的时候自动调用此方法
    public void OnPointerUp(PointerEventData eventData)
    {
        gamePanelManager.inGetHp = false;
    }

    // 当鼠标从按钮上离开的时候自动调用此方法
    public void OnPointerExit(PointerEventData eventData)
    {
        gamePanelManager.inGetHp = false;
    }
}
