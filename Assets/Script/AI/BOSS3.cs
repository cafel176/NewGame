using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//杀手
public class BOSS3 : MonoBehaviour
{
    private GameObject player;
    private Player playerScript;
    private Enemy enemy;
    private Vector3 targetPos;
    private float attackRange;
    private bool inDo = false;

    public float time = 4f;
    private float timer = 0;

    private float bigTimer = 0;

    void Start()
    {
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
        if (timer >= time && inDo == false && !enemy.pause)
        {
            if (!enemy.beAttack && !enemy.die)
                enemy.Move(0);
            if (player != null && enemy.jumpState == 0 && !enemy.inDefend && !enemy.die)
            {
                if (Mathf.Abs(targetPos.x) < 4)
                {
                    float j = Random.Range(0f, 1f);
                    if (bigTimer > 5 && j < 0.3f)
                    {
                        StartCoroutine(fight2());
                        bigTimer = 0;
                    }
                    else if (j < 0.5f)
                    {
                        StartCoroutine(fight3());
                    }
                    else if (j>0.7f)
                    {
                        enemy.Move(targetPos.x > 0 ? 0.01f : -0.01f);
                        enemy.Move(0);
                        enemy.Attack5();
                    }
                }
                else if (Mathf.Abs(targetPos.x) < attackRange)
                {
                    float j = Random.Range(0f, 1f);
                    if (!enemy.beAttack)
                    {
                        if (j < 0.5f)
                        {
                            if (playerScript.die == false)
                            {
                                enemy.Move(targetPos.x > 0 ? 0.01f : -0.01f);
                                enemy.Move(0);
                                enemy.Attack3S(targetPos.x > 0 ? 15 : -15);

                            }
                        }
                        else if (j > 0.7f)
                        {
                            StartCoroutine(fight3());
                        }
                    }
                }
                else
                {
                    if (!enemy.beAttack)
                    {
                        float i = Random.Range(0f, 1f);
                        if (i < 0.6f)
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
        enemy.call();
        yield return new WaitForSeconds(0.8f);
        Vector2 a = player.transform.position;
        yield return new WaitForSeconds(0.3f);
        transform.position = a + new Vector2(2, 0);
        enemy.Move(targetPos.x > 0 ? 0.01f : -0.01f);
        enemy.Move(0);
        enemy.Attack5();
        enemy.Move(0.01f);
        enemy.Move(0);
        yield return new WaitForSeconds(0.7f);
        Vector2 b = player.transform.position;
        yield return new WaitForSeconds(0.3f);
        transform.position = b + new Vector2(-8, 0);
        enemy.Attack3S(15);
        yield return new WaitForSeconds(0.7f);
        Vector2 c = player.transform.position;
        yield return new WaitForSeconds(0.3f);
        enemy.Move(-0.01f);
        enemy.Move(0);
        transform.position = c + new Vector2(2, 3);
        enemy.AttackDown();
        yield return new WaitForSeconds(0.4f);
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
        Vector2 a = player.transform.position;
        enemy.Move(targetPos.x > 0 ? 0.01f : -0.01f);
        enemy.Move(0);
        transform.position = a + new Vector2(targetPos.x > 0 ? -8 : 8, 0);
        enemy.Attack3S(targetPos.x > 0 ? 15 : -15);
        yield return new WaitForSeconds(1f);
        transform.position = a + new Vector2(targetPos.x > 0 ? -2 : 2, 2);
        enemy.Move(targetPos.x > 0 ? 0.01f : -0.01f);
        enemy.Move(0);
        enemy.AttackDown();
        yield return new WaitForSeconds(0.4f);
        gameManager.instance.gravity = true;
        gameManager.instance.control = true;
        inDo = false;
    }


    IEnumerator fight3()
    {
        inDo = true;
        enemy.call();
        yield return new WaitForSeconds(0.8f);
        Vector2 a = player.transform.position;
        yield return new WaitForSeconds(0.3f);
        enemy.Move(-0.01f);
        enemy.Move(0);
        transform.position = a + new Vector2(2, 3);
        enemy.AttackDown();
        yield return new WaitForSeconds(0.7f);
        Vector2 b = player.transform.position;
        yield return new WaitForSeconds(0.3f);
        enemy.Move(0.01f);
        enemy.Move(0);
        transform.position = b + new Vector2(-2, 3);
        enemy.AttackDown();
        yield return new WaitForSeconds(0.7f);
        Vector2 c = player.transform.position;
        yield return new WaitForSeconds(0.3f);
        enemy.Move(-0.01f);
        enemy.Move(0);
        transform.position = c + new Vector2(2, 3);
        enemy.AttackDown();
        yield return new WaitForSeconds(0.4f);
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
