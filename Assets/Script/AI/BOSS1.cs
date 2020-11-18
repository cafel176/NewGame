using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//弓王
public class BOSS1 : MonoBehaviour {
    private GameObject player;
    private Player playerScript;
    private Enemy enemy;
    private Vector3 targetPos;
    private float attackRange;
    private bool inDo = false;

    public float time = 4f;
    private float timer = 0;

    private float bigTimer = 0;

    void Start() {
        attackRange = this.gameObject.GetComponent<Enemy>().attackRange;
        enemy = this.gameObject.GetComponent<Enemy>();
        player = enemy.player;
        playerScript = player.GetComponent<Player>();
    }
    private void Update()
    {
        if (player != null)
        {
            targetPos = player.transform.position - transform.position;
        }
        if (timer >= time && inDo==false && !enemy.pause)
        {
            if (!enemy.beAttack && !enemy.die)
                enemy.Move(0);
            if (player != null && enemy.jumpState == 0 && !enemy.inDefend && !enemy.die)
            {
                if (Mathf.Abs(targetPos.x) < 4)
                {
                    float j = Random.Range(0f, 1f);
                    if (bigTimer>5 && j<0.3f)
                    {
                        StartCoroutine(fight2());
                        bigTimer = 0;
                    }
                    else
                    {
                        StartCoroutine(fight());
                    }
                }
                else if (Mathf.Abs(targetPos.x) < attackRange)
                {
                        float j = Random.Range(0f, 1f);
                        if (!enemy.beAttack)
                        {
                            if (j < 0.3f)
                            {
                                enemy.rush(targetPos.x > 0 ? -1 : 1);
                            }
                            else if (j > 0.7f)
                            {
                                StartCoroutine(fight3());
                            }
                            else
                                StartCoroutine(fight());
                        }
                }
                else
                {
                    if (!enemy.beAttack)
                    {
                        float i = Random.Range(0f, 1f);
                        if (i < 0.5f)
                        {
                            enemy.Move(targetPos.x > 0 ? 2 : -2);
                        }
                        else if (i > 0.8f)
                        {
                            StartCoroutine(fight());
                        }
                        else
                        {
                            StartCoroutine(fight3());
                        }
                    }
                }
            }
            timer = 0;
        }
    }

    IEnumerator fight()
    {
        inDo = true;
        enemy.Move(targetPos.x > 0 ? 0.01f : -0.01f);
        enemy.Move(0);
        enemy.Jump(targetPos.x > 0 ? 1f : -1f);
        yield return new WaitForSeconds(1f);
        enemy.Move(targetPos.x > 0 ? 0.01f : -0.01f);
        enemy.Move(0);
        enemy.Thread5();
        yield return new WaitForSeconds(1f);
        enemy.gravity = true;
        inDo = false;
    }

    IEnumerator fight2()
    {
        inDo = true;
        enemy.Move(targetPos.x > 0 ? 0.01f : -0.01f);
        enemy.Move(0);
        enemy.AttackUp();
        yield return new WaitForSeconds(1.5f);
        if (playerScript.beAttack)
        {
            gameManager.instance.gravity = false;
            playerScript.gravity = false;
            playerScript._rigidbody.velocity = Vector2.zero;
            gameManager.instance.control = false;
        }
        enemy.Move(targetPos.x > 0 ? 0.01f : -0.01f);
        enemy.Move(0);
        enemy.Thread();
        yield return new WaitForSeconds(1f);
        enemy.Move(targetPos.x > 0 ? 0.01f : -0.01f);
        enemy.Move(0);
        enemy.Thread();
        yield return new WaitForSeconds(1f);
        enemy.Move(targetPos.x > 0 ? 0.01f : -0.01f);
        enemy.Move(0);
        enemy.Thread();
        yield return new WaitForSeconds(1f);
        gameManager.instance.gravity = true;
        gameManager.instance.control = true;
        inDo = false;
    }


    IEnumerator fight3()
    {
        inDo = true;
        enemy.Move(targetPos.x > 0 ? 0.01f : -0.01f);
        enemy.Move(0);
        enemy.Thread3();
        yield return new WaitForSeconds(1f);
        enemy.Move(targetPos.x > 0 ? 0.01f : -0.01f);
        enemy.Move(0);
        enemy.Thread3();
        yield return new WaitForSeconds(1f);
        enemy.Move(targetPos.x > 0 ? 0.01f : -0.01f);
        enemy.Move(0);
        enemy.Thread3();
        yield return new WaitForSeconds(1f);
        inDo = false;
    }

    private void FixedUpdate()
    {
        player = enemy.player;
        playerScript = player.GetComponent<Player>();
        timer += Time.deltaTime;
        bigTimer += Time.deltaTime;
    }
}
