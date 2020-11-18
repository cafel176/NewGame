using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    public Sprite next;
    public int spirit;
    public AudioSource audio;
    private bool canAttack=true;
    private SpriteRenderer spriteRender;

	void Start () {
        spriteRender = GetComponent<SpriteRenderer>();
	}
	
    public void doSth()
    {
        if (canAttack)
        {
            canAttack = false;
            audio.Play();
            spriteRender.sprite = next;
            gameManager.instance.spirit += spirit;
        }
    }
}
