using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item1 : MonoBehaviour {

    public Sprite next;
    private Player player;
    private GamePanelManager gamePanel;
    public AudioSource audio;
    private bool canAttack=true;
    private SpriteRenderer spriteRender;

	void Start () {
        spriteRender = GetComponent<SpriteRenderer>();
        gamePanel = gameManager.instance.getGamePanel().GetComponent<GamePanelManager>();
        player = gamePanel.player.GetComponent<Player>();
    }

    private void Update()
    {
        player = gamePanel.player.GetComponent<Player>();
    }

    public void doSth()
    {
        if (canAttack)
        {
            canAttack = false;
            audio.Play();
            spriteRender.sprite = next;
            player.Hp = gameManager.instance.MaxHp;
            player.Sp = gameManager.instance.MaxSp;
        }
    }
}
