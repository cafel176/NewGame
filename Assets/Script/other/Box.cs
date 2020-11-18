using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Box : MonoBehaviour {

    public Sprite opening;
    public Sprite opened;
    private bool show = false;
    private bool canOpen = true;
    private float timer = 0;
    public GameObject itemSprite;
    public AudioSource audio;
    private SpriteRenderer spriteRender;
    public GameObject AchHint;
    [HideInInspector]
    public Sprite item;
    [HideInInspector]
    public int itemNum;
    [HideInInspector]
    public string hintDescribe;

    void Start () {
        spriteRender = GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update () {       
        if (show)
        {
            timer += Time.deltaTime;
            if(timer < 0.5)
                itemSprite.transform.position += new Vector3(0, 0.1f, 0);
        }
        if (show && timer > 1f)
        {
            show = false;
            itemSprite.SetActive(false);
            gameManager.instance.buyItem(itemNum);
            GameObject go = GameObject.Instantiate(AchHint) as GameObject;
            go.transform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>());
            go.GetComponentInChildren<Text>().text = hintDescribe;
            go.GetComponentInChildren<Image>().sprite = item;
            StartCoroutine(destroyHint(go));
        }
    }

    public void doSth()
    {
        if (canOpen)
        {
            canOpen = false;
            audio.Play();
            StartCoroutine(open());
        }
    }

    IEnumerator destroyHint(GameObject go)
    {
        yield return new WaitForSeconds(4);
        Destroy(go);
    }

    IEnumerator open()
    {
        spriteRender.sprite = opening;
        yield return new WaitForSeconds(0.3f);
        spriteRender.sprite = opened;
        yield return new WaitForSeconds(0.3f);
        show = true;
        itemSprite.GetComponent<SpriteRenderer>().sprite = item;
    }
}
