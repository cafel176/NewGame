using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//匈奴步兵
public class Enemy1 : MonoBehaviour
{
    private GameObject player;
    private Player playerScript;
    private Enemy enemy;
    private Vector3 targetPos;
    private float attackRange;

    private float timer = 0;

    public float time = 1.2f; 

    void Start()
    {
        attackRange = this.gameObject.GetComponent<Enemy>().attackRange;
        enemy = this.gameObject.GetComponent<Enemy>();
        player = enemy.player;
        playerScript = enemy.playerScript;
    }

    private void Update()
    {
        if (player != null)
        {
            targetPos = player.transform.position - transform.position;
            if (Mathf.Abs(targetPos.x) < attackRange && enemy.inRun)
                enemy.Move(0);
        }
        if (timer>=time && !enemy.pause)
        {
            if (player != null && enemy.jumpState == 0 && !enemy.inDefend && !enemy.die)
            {
                if (Mathf.Abs(targetPos.x) < attackRange)
                {
                    float i = Random.Range(0f, 1f);
                    if (i <= 0.7f)
                    {
                        float j = Random.Range(0f, 1f);
                        if(!enemy.beAttack)
                        {
                            if (j >= 0.7f)
                                enemy.Jump(targetPos.x > 0 ? 1.5f : -1.5f);
                            else
                            {
                                if (playerScript.die == false)
                                {
                                    enemy.Move(targetPos.x > 0 ? 0.01f : -0.01f);
                                    enemy.Move(0);
                                    enemy.Attack();
                                }
                            }
                        }
                    }
                    else
                    {
                        float j = Random.Range(0f, 1f);
                            if(j>=0.7f)
                                enemy.rush(targetPos.x > 0 ? -1 : 1);
                            else if(j<=0.4f)
                                enemy.defend();
                    }
                }
                else
                {
                    if(!enemy.beAttack)
                    {
                        float i = Random.Range(0f, 1f);
                        if (i < 0.8f)
                            enemy.Move(targetPos.x > 0 ? 2 : -2);
                        else
                        {
                            enemy.Move(targetPos.x > 0 ? 0.01f : -0.01f);
                            enemy.Move(0);
                            enemy.Attack2(targetPos.x > 0 ? 1.5f : -1.5f);
                        }
                    }
                }
            }
            timer = 0;
        }
    }

    private void FixedUpdate()
    {
        player = enemy.player;
        playerScript = enemy.playerScript;
        timer += Time.deltaTime;
    }
}
