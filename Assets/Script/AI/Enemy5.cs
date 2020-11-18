using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//匈奴弓箭手
public class Enemy5 : MonoBehaviour {
    private GameObject player;
    private Player playerScript;
    private Enemy enemy;
    private Vector3 targetPos;
    private float attackRange;

    public float time = 1.5f;
    private float timer = 0;


    void Start() {
        attackRange = this.gameObject.GetComponent<Enemy>().attackRange;
        enemy = this.gameObject.GetComponent<Enemy>();
        player = enemy.player;
        playerScript = player.GetComponent<Player>();
    }

    private void Update()
    {
        if (player != null && !enemy.pause)
        {
            targetPos = player.transform.position - transform.position;
            if (Mathf.Abs(targetPos.x) < attackRange && enemy.inRun)
                enemy.Move(0);
        }
        if (timer >= time && !enemy.pause)
        {
            if (!enemy.beAttack && !enemy.die)
                enemy.Move(0);
            if (player != null && enemy.jumpState == 0 && !enemy.inDefend && !enemy.die)
            {
                if (Mathf.Abs(targetPos.x) < attackRange)
                {
                        if (!enemy.beAttack)
                        {
                                if (playerScript.die == false)
                                {
                                    enemy.Move(targetPos.x > 0 ? 0.01f : -0.01f);
                                    enemy.Move(0);
                                    enemy.Thread();
                                }
                        }
                }
                else
                {
                    if (!enemy.beAttack)
                    {
                        float i = Random.Range(0f, 1f);
                        if (i < 0.8f)
                            enemy.Move(targetPos.x > 0 ? 2 : -2);
                        else
                        {
                            enemy.Move(targetPos.x > 0 ? 0.01f : -0.01f);
                            enemy.Move(0);
                            enemy.Jump(targetPos.x > 0 ? 1.5f : -1.5f);
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
        playerScript = player.GetComponent<Player>();
        timer += Time.deltaTime;
    }
}
