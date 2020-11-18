using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class onRightButtonPressed : MonoBehaviour,IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{
    private GamePanelManager gamePanelManager;
    // Use this for initialization
    void Start() {
        gamePanelManager = this.gameObject.GetComponentInParent<GamePanelManager>();

    }

    // 当按钮被按下后系统自动调用此方法
    public void OnPointerDown(PointerEventData eventData)
    {
        gamePanelManager.move = 1;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        gamePanelManager.move = 1;
    }

    // 当按钮抬起的时候自动调用此方法
    public void OnPointerUp(PointerEventData eventData)
    {
        gamePanelManager.move = 0;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gamePanelManager.move = 0;
    }
}
