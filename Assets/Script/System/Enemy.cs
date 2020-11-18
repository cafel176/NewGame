using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//敌人类
public class Enemy : MonoBehaviour
{
    private float attackS;   //暴击率
    private float attackP;   //破防率
    private float luck;

    //敌人的数据
    public int enemyIndex = 0;
    public int hp;
    public int attackValue;
    public int defendValue;
    public float attackRange;
    public float JumpForce;
    public float MoveSpeed;
    public int spirit;
    public GameObject[] callList;

    //需要的组件
    public AudioSource audio;
    public GameObject damageText;
    [HideInInspector]
    public bool pause = false;
    [HideInInspector]
    public GameObject player;
    private GamePanelManager gamePanel;
    [HideInInspector]
    public Player playerScript;
    private GameObject An;
    [HideInInspector]
    public Animator animator;
    private Rigidbody2D _rigidbody;
    private CapsuleCollider2D _collider;

    //内部变量
    [HideInInspector]
    public char turn = 'L';
    private float attackTime=0.3f;
    [HideInInspector]
    public int jumpState = 0;
    private int attackState = 0;
    private float attackTimer = 0;
    [HideInInspector]
    public bool beAttack = false;
    [HideInInspector]
    public bool inDefend = false;
    [HideInInspector]
    public bool die = false;
    private float beAttackTimer = 0;
    private float defendTimer = 0;
    private float LieTimer = 0;
    private List<GameObject> list = new List<GameObject>();
    public bool canAttacked = true;
    public bool canUp = true;
    [HideInInspector]
    public bool inRun = true;
    [HideInInspector]
    public bool gravity = true;

    //音乐和音效
    public AudioClip attacked;
    public AudioClip attack;
    public AudioClip attack2;
    public AudioClip attackB;
    public AudioClip jump;
    public AudioClip other;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CapsuleCollider2D>();
        An = GetComponentInChildren<SpriteRenderer>().gameObject;
        gamePanel = gameManager.instance.getGamePanel().GetComponent<GamePanelManager>();
        player = gamePanel.player;
        playerScript = player.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        player = gamePanel.player;
        playerScript = player.GetComponent<Player>();
        if (_rigidbody.velocity.x == 0)
            animator.SetBool("isRun", false);
        if (attackTimer > attackTime)
            attackState = 0;
        if (gravity)
        {
            _rigidbody.gravityScale = 3;
            animator.SetFloat("GSpeed", _rigidbody.velocity.y);
        }
        else
        {
            _rigidbody.gravityScale = 0;
            animator.SetFloat("GSpeed", 0);
            animator.SetBool("isInAir", false);
        }
    }

    private void FixedUpdate()
    {
        if (beAttack)
            beAttackTimer += Time.deltaTime;
        attackTimer += Time.deltaTime;
        if(inDefend)
            defendTimer += Time.deltaTime;
        if (die && hp > 0)
        {
            LieTimer += Time.deltaTime;
            if (LieTimer > 3)
            {
                animator.SetTrigger("Up");
                die = false;
                LieTimer = 0;
            }
        }
        if (defendTimer > 4)
        {
            animator.SetBool("Defend", false);
            inDefend = false;
        }
        if (beAttack && beAttackTimer > 0.3f)
        {
            beAttack = false;
            beAttackTimer = 0;
            animator.SetTrigger("EndAttacked");
            if (canUp)
                gravity = true;
        }
        //audio.volume = gameManager.instance.SE;
        luck = gameManager.instance.Luck;
        attackS = gameManager.instance.attackS;
        attackP = gameManager.instance.attackP;
    }

    //========================================行动，跳跃============================
    public void Jump(float go)
    {
        if (!beAttack && gravity && !die)
        {
            audio.clip = jump;
            audio.Play();
            animator.SetBool("isRun", false);
            float move = go * MoveSpeed;
            Move(go > 0 ? 0.01f : -0.01f);
            Move(0);
            if (jumpState == 0)
            {
                animator.SetTrigger("Jump");
                _rigidbody.velocity = new Vector2(move, JumpForce);
                jumpState = 1;
            }
        }
    }

    //===========================================行动，移动========================
    public void Move(float go)
    {
        float move = beAttack && !die ? 0:go * MoveSpeed;
        if (move > 0)
        {
            inRun = true;
            if (jumpState == 0 && attackState == 0)
                animator.SetBool("isRun", true);
            else
                animator.SetBool("isRun", false);
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            turn = 'R';
        }
        else if (move < 0)
        {
            inRun = true;
            if (jumpState == 0 && attackState == 0)
                animator.SetBool("isRun", true);
            else
                animator.SetBool("isRun", false);
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            turn = 'L';
        }
        else
        {
            inRun = false;
            animator.SetBool("isRun", false);
        }

        _rigidbody.velocity = new Vector2(move, _rigidbody.velocity.y);
    }

    //=======================================行动，冲刺==============================
    public void rush(float go)
    {
        if (!die)
        {
            inDefend = true;
            beAttack = false;
            gravity = true;
            beAttackTimer = 0;
            animator.SetTrigger("EndAttacked");
            Move(go > 0 ? 0.01f : -0.01f);
            Move(0);
            animator.SetBool("Rush", true);
            float move = beAttack ? 0 : go * 15;
            _rigidbody.velocity = new Vector2(move, _rigidbody.velocity.y);
            StartCoroutine(Rush());
        }
    }

    IEnumerator Rush()
    {
        yield return new WaitForSeconds(0.6f);
        animator.SetBool("Rush", false);
        Move(0);
        inDefend = false;
    }

    //=================================普通的攻击，挥剑======================
    public void Attack()
    {
        if (!beAttack && gravity && !die)
        {
            animator.SetBool("isRun", false);
            if (jumpState == 0 && attackState == 0)
            {
                audio.clip = attack;
                audio.Play();
                attackState = 1;
                attackTimer = 0;
                animator.SetTrigger("Attack");
                StartCoroutine(fight());
            }
        }
    }

    IEnumerator fight()
    {
        yield return new WaitForSeconds(0.66f);
        if (!beAttack)
        {
            Collider2D[] list = Physics2D.OverlapCapsuleAll(transform.position + new Vector3(turn == 'L' ? -1.5f : 1.5f, 0, 0), new Vector2(4, 2), CapsuleDirection2D.Horizontal, 0);
            foreach (var got in list)
            {
                if (got.gameObject.tag == "Player")
                {
                    Player go = got.gameObject.GetComponent<Player>();
                    go.takeDamage(attackValue);
                }
            }
        }
    }

    //==============================特殊攻击，跳砍===================================
    public void Attack2(float go)
    {
        if (!beAttack && gravity && !die)
        {
            audio.clip = jump;
            audio.Play();
            animator.SetBool("isRun", false);
            if (jumpState == 0 && attackState == 0)
            {
                audio.clip = attack;
                audio.Play();
                attackState = 1;
                attackTimer = 0;
                float move = go * MoveSpeed;
                animator.SetTrigger("Attack2");
                StartCoroutine(fight2(move));
            }
        }
    }

    IEnumerator fight2(float move)
    {
        yield return new WaitForSeconds(0.33f);
        _rigidbody.velocity = new Vector2(move, JumpForce);
        yield return new WaitForSeconds(0.75f);
        if (!beAttack)
        {
            Collider2D[] list = Physics2D.OverlapCapsuleAll(transform.position + new Vector3(turn == 'L' ? -1 : 1, -1, 0), new Vector2(6, 6), CapsuleDirection2D.Horizontal, 0);
            foreach (var got in list)
            {
                if (got.gameObject.tag == "Player")
                {
                    Player go = got.gameObject.GetComponent<Player>();
                    go.takeDamage(attackValue);
                }
            }
        }
        audio.clip = attack2;
        audio.Play();
    }

    //==================================特殊攻击，射箭==============================
    public void Thread()
    {
        if (!beAttack && gravity)
        {
            Vector2 tar = player.transform.position - transform.position;
            if(tar.y>5)
                animator.SetTrigger("Threadup");
            else if (tar.y < -5)
                animator.SetTrigger("Threaddown");
            else
                animator.SetTrigger("Thread");
            audio.clip = attack;
            audio.Play();

            GameObject go = Instantiate(BattleManager.instance.threadModel, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity) as GameObject;
            go.GetComponent<Thread>().target = tar;
            go.GetComponent<Thread>().damage = attackValue;
        }
    }

    public void Thread3()
    {
        if (!beAttack && gravity)
        {
            Vector2 tar = player.transform.position - transform.position;
            if (tar.y > 5)
                animator.SetTrigger("Threadup");
            else if (tar.y < -5)
                animator.SetTrigger("Threaddown");
            else
                animator.SetTrigger("Thread");
            audio.clip = attack;
            audio.Play();

            GameObject go = Instantiate(BattleManager.instance.threadModel, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity) as GameObject;
            go.GetComponent<Thread>().target = tar;
            go.GetComponent<Thread>().damage = attackValue;
            GameObject go2 = Instantiate(BattleManager.instance.threadModel, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity) as GameObject;
            go2.GetComponent<Thread>().target = new Vector2(tar.x,tar.y + 3);
            go2.GetComponent<Thread>().damage = attackValue;
            GameObject go3 = Instantiate(BattleManager.instance.threadModel, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity) as GameObject;
            go3.GetComponent<Thread>().target = new Vector2(tar.x, tar.y - 3);
            go3.GetComponent<Thread>().damage = attackValue;
        }
    }

    public void Thread5()
    {
        if (!beAttack && gravity)
        {
            gravity = false;
            Vector2 tar = player.transform.position - transform.position;
            animator.SetBool("isInAir", false);
            if (tar.y > 5)
                animator.SetTrigger("Threadup");
            else if (tar.y < -5)
                animator.SetTrigger("Threaddown");
            else
                animator.SetTrigger("Thread");
            audio.clip = attack;
            audio.Play();

            GameObject go = Instantiate(BattleManager.instance.threadModel, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity) as GameObject;
            go.GetComponent<Thread>().target = tar;
            go.GetComponent<Thread>().damage = attackValue;
            GameObject go2 = Instantiate(BattleManager.instance.threadModel, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity) as GameObject;
            go2.GetComponent<Thread>().target = new Vector2(tar.x, tar.y + 3);
            go2.GetComponent<Thread>().damage = attackValue;
            GameObject go3 = Instantiate(BattleManager.instance.threadModel, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity) as GameObject;
            go3.GetComponent<Thread>().target = new Vector2(tar.x, tar.y +6);
            go3.GetComponent<Thread>().damage = attackValue;
            GameObject go4 = Instantiate(BattleManager.instance.threadModel, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity) as GameObject;
            go4.GetComponent<Thread>().target = new Vector2(tar.x, tar.y - 3);
            go4.GetComponent<Thread>().damage = attackValue;
            GameObject go5 = Instantiate(BattleManager.instance.threadModel, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity) as GameObject;
            go5.GetComponent<Thread>().target = new Vector2(tar.x, tar.y - 6);
            go5.GetComponent<Thread>().damage = attackValue;
            gravity = true;
        }
    }

    //==================================特殊攻击，发剑气==============================
    public void Thread2()
    {
        if (!beAttack && gravity)
        {
            Vector2 tar = player.transform.position - transform.position;
            animator.SetTrigger("call");
            audio.clip = attack;
            audio.Play();

            GameObject go = Instantiate(BattleManager.instance.threadModel2, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity) as GameObject;
            go.GetComponent<Thread>().target = tar;
            go.GetComponent<Thread>().damage = attackValue;
        }
    }

    public void Thread2X3()
    {
        if (!beAttack && gravity)
        {
            Vector2 tar = player.transform.position - transform.position;
            animator.SetTrigger("call");
            audio.clip = attack;
            audio.Play();

            GameObject go = Instantiate(BattleManager.instance.threadModel2, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity) as GameObject;
            go.GetComponent<Thread>().target = tar;
            go.GetComponent<Thread>().damage = attackValue;
            GameObject go2 = Instantiate(BattleManager.instance.threadModel2, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity) as GameObject;
            go2.GetComponent<Thread>().target = new Vector2(tar.x,tar.y-4);
            go2.GetComponent<Thread>().damage = attackValue;
            GameObject go3 = Instantiate(BattleManager.instance.threadModel2, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity) as GameObject;
            go3.GetComponent<Thread>().target = new Vector2(tar.x, tar.y+4);
            go3.GetComponent<Thread>().damage = attackValue;
        }
    }

    public void Thread2X5()
    {
        if (!beAttack && gravity)
        {
            Vector2 tar = player.transform.position - transform.position;
            gravity = false;
            animator.SetBool("isInAir", false);
            animator.SetTrigger("call");
            audio.clip = attack;
            audio.Play();

            GameObject go = Instantiate(BattleManager.instance.threadModel2, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity) as GameObject;
            go.GetComponent<Thread>().target = tar;
            go.GetComponent<Thread>().damage = attackValue;
            GameObject go2 = Instantiate(BattleManager.instance.threadModel2, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity) as GameObject;
            go2.GetComponent<Thread>().target = new Vector2(tar.x, tar.y - 4);
            go2.GetComponent<Thread>().damage = attackValue;
            GameObject go3 = Instantiate(BattleManager.instance.threadModel2, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity) as GameObject;
            go3.GetComponent<Thread>().target = new Vector2(tar.x, tar.y + 4);
            go3.GetComponent<Thread>().damage = attackValue;
            GameObject go4 = Instantiate(BattleManager.instance.threadModel2, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity) as GameObject;
            go4.GetComponent<Thread>().target = new Vector2(tar.x, tar.y - 8);
            go4.GetComponent<Thread>().damage = attackValue;
            GameObject go5 = Instantiate(BattleManager.instance.threadModel2, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity) as GameObject;
            go5.GetComponent<Thread>().target = new Vector2(tar.x, tar.y + 8);
            go5.GetComponent<Thread>().damage = attackValue;
            gravity = true;
        }
    }

    //==============================特殊攻击，突刺=================================
    public void Attack3(float go,float attackTime)
    {
        if (!beAttack && !die)
        {
            animator.SetBool("isRun", false);
            if (jumpState == 0 && attackState == 0)
            {
                attackState = 1;
                attackTimer = 0;
                float move = go * MoveSpeed;
                StartCoroutine(fight3(move,attackTime));
            }
        }
    }

    IEnumerator fight3(float move,float attackTime)
    {
        animator.SetTrigger("Attack3");
        yield return new WaitForSeconds(0.34f);
        audio.clip = attack;
        audio.Play();
        _rigidbody.velocity = new Vector2(move, 0);
        yield return new WaitForSeconds(0.1f);
        if (!beAttack)
        {
            Collider2D[] list = Physics2D.OverlapCapsuleAll(transform.position + new Vector3(turn == 'L' ? -1.5f : 1.5f, 0, 0), new Vector2(12, 3), CapsuleDirection2D.Horizontal, 0);
            foreach (var got in list)
            {
                if (got.gameObject.tag == "Player")
                {
                    Player go = got.gameObject.GetComponent<Player>();
                    go.takeDamage(attackValue);
                }
            }
        }
        yield return new WaitForSeconds(attackTime);
        if (!beAttack)
        {
            Collider2D[] list = Physics2D.OverlapCapsuleAll(transform.position + new Vector3(turn == 'L' ? -1.5f : 1.5f, 0, 0), new Vector2(12, 3), CapsuleDirection2D.Horizontal, 0);
            foreach (var got in list)
            {
                if (got.gameObject.tag == "Player")
                {
                    Player go = got.gameObject.GetComponent<Player>();
                    go.takeDamage(attackValue);
                }
            }
        }
        yield return new WaitForSeconds(0.2f);
    }

    public void Attack3S(float go)
    {
        if (!die)
        {
            die = true;
            animator.SetBool("isRun", false);
            beAttack = false;
            beAttackTimer = 0;
            animator.SetTrigger("EndAttacked");
            if (jumpState == 0 && attackState == 0)
            {
                attackState = 1;
                attackTimer = 0;
                gravity = false;
                StartCoroutine(fight3S(go));
            }
        }
    }

    IEnumerator fight3S(float move)
    {
        animator.SetTrigger("Attack3");
        yield return new WaitForSeconds(0.4f);
        audio.clip = attack;
        audio.Play();
        Vector2 a1 = transform.position;
        transform.position += new Vector3(move, 0, 0);
        Vector2 a2 = transform.position;
        if ((player.transform.position.x > a1.x && player.transform.position.x < a2.x) || (player.transform.position.x < a1.x && player.transform.position.x > a2.x))
        {
            if (Mathf.Abs(player.transform.position.y - a1.y) < 3)
            {
                Player go = player.gameObject.GetComponent<Player>();
                go.takeDamage(attackValue);
            }
        }
        yield return new WaitForSeconds(0.35f);
        gravity = true;
        die = false;
    }

    //=====================================特殊攻击，回身刺=========================
    public void DefendS()
    {
        if (!beAttack && !die)
        {
            animator.SetBool("isRun", false);
            if (jumpState == 0 && attackState == 0)
            {
                StartCoroutine(def());
            }
        }
    }

    IEnumerator def()
    {
        animator.SetTrigger("DefendS");
        _rigidbody.velocity = new Vector2(turn == 'L' ? 5 : -5, 0);
        yield return new WaitForSeconds(0.4f);
        _rigidbody.velocity = new Vector2(turn == 'L' ? -15 : 15, 0);
        if (!beAttack)
        {
            Collider2D[] list = Physics2D.OverlapCapsuleAll(transform.position + new Vector3(turn == 'L' ? -1.5f : 1.5f, -0.5f, 0), new Vector2(4, 2), CapsuleDirection2D.Horizontal, 0);
            foreach (var got in list)
            {
                if (got.gameObject.tag == "Player")
                {
                    Player go = got.gameObject.GetComponent<Player>();
                    go.takeDamage(attackValue);
                }
            }
        }
        audio.clip = attack;
        audio.Play();
        yield return new WaitForSeconds(0.5f);
        Move(0);
    }

    //====================================特殊攻击，挑空===========================
    public void AttackUp()
    {
        if (!beAttack && gravity && !die)
        {
            animator.SetBool("isRun", false);
            if (jumpState == 0 && attackState == 0)
            {
                audio.clip = attack;
                audio.Play();
                attackState = 1;
                attackTimer = 0;
                animator.SetTrigger("Attack");
                StartCoroutine(fightUp());
            }
        }
    }

    IEnumerator fightUp()
    {
        yield return new WaitForSeconds(0.6f);
        if (!beAttack)
        {
            Collider2D[] list = Physics2D.OverlapCapsuleAll(transform.position + new Vector3(turn == 'L' ? -1.5f : 1.5f, 0, 0), new Vector2(6, 4), CapsuleDirection2D.Horizontal, 0);
            foreach (var got in list)
            {
                if (got.gameObject.tag == "Player")
                {
                    Player go = got.gameObject.GetComponent<Player>();
                    go.takeDamage(attackValue);
                    go._rigidbody.velocity = new Vector2(2,25);
                }
            }
        }
        yield return new WaitForSeconds(0.2f);
    }

    //====================================特殊攻击，下劈===========================
    public void AttackDown()
    {
        if ( gravity && !die)
        {
            die = true;
            animator.SetBool("isRun", false);

                audio.clip = attack2;
                audio.Play();           
                beAttack = false;
                beAttackTimer = 0;
                animator.SetTrigger("EndAttacked");
                attackState = 1;
                attackTimer = 0;
                gravity = false;
                StartCoroutine(fightDown());
        }
    }

    IEnumerator fightDown()
    {
        animator.SetTrigger("Attack4");
        yield return new WaitForSeconds(0.25f);
        if (!beAttack)
        {
            Collider2D[] list = Physics2D.OverlapCapsuleAll(transform.position + new Vector3(turn == 'L' ? -1.5f : 1.5f, 0, 0), new Vector2(6, 4), CapsuleDirection2D.Horizontal, 0);
            foreach (var got in list)
            {
                if (got.gameObject.tag == "Player")
                {
                    Player go = got.gameObject.GetComponent<Player>();
                    go.takeDamage(attackValue);
                    go.gravity = true;
                    go._rigidbody.velocity = new Vector2(0, -30);
                }
            }
        }
        gravity = true;
        _rigidbody.velocity = new Vector2(0, -35);
        yield return new WaitForSeconds(0.35f);
        die = false;
    }

    //=================================特殊攻击，连斩======================
    public void Attack5()
    {
        if (!beAttack && gravity && !die)
        {
            animator.SetBool("isRun", false);
            if (jumpState == 0 && attackState == 0)
            {
                audio.clip = attack;
                audio.Play();
                attackState = 1;
                attackTimer = 0;
                animator.SetTrigger("Attack5");
                StartCoroutine(fight5());
            }
        }
    }

    IEnumerator fight5()
    {
        yield return new WaitForSeconds(0.25f);
            Collider2D[] list = Physics2D.OverlapCapsuleAll(transform.position + new Vector3(turn == 'L' ? -1.5f : 1.5f, 0, 0), new Vector2(6,4), CapsuleDirection2D.Horizontal, 0);
            foreach (var got in list)
            {
                if (got.gameObject.tag == "Player")
                {
                    Player go = got.gameObject.GetComponent<Player>();
                    go.takeDamage(attackValue);
                }
            }
        yield return new WaitForSeconds(0.25f);
            Collider2D[] list2 = Physics2D.OverlapCapsuleAll(transform.position + new Vector3(turn == 'L' ? -1.5f : 1.5f, 0, 0), new Vector2(6, 4), CapsuleDirection2D.Horizontal, 0);
            foreach (var got in list2)
            {
                if (got.gameObject.tag == "Player")
                {
                    Player go = got.gameObject.GetComponent<Player>();
                    go.takeDamage(attackValue);
                }
            }
        yield return new WaitForSeconds(0.25f);
        Collider2D[] list3 = Physics2D.OverlapCapsuleAll(transform.position + new Vector3(turn == 'L' ? -1.5f : 1.5f, 0, 0), new Vector2(6, 4), CapsuleDirection2D.Horizontal, 0);
        foreach (var got in list3)
        {
            if (got.gameObject.tag == "Player")
            {
                Player go = got.gameObject.GetComponent<Player>();
                go.takeDamage(attackValue);
            }
        }
        yield return new WaitForSeconds(0.25f);
    }

    //=================================特殊攻击，召唤=========================
    public void call()
    {
        if (gravity && !die)
        {
            die = true;
            beAttack = false;
            gravity = true;
            beAttackTimer = 0;
            animator.SetTrigger("EndAttacked");
            animator.SetTrigger("call");
            if (callList.Length > 0)
            {
                int i = Random.Range(0, callList.Length + 1);
                GameObject go = Instantiate(callList[i], transform.position + new Vector3(1, 0, 0), Quaternion.identity) as GameObject;
                GameObject go2 = Instantiate(callList[i], transform.position + new Vector3(-1, 0, 0), Quaternion.identity) as GameObject;
                list.Add(go);
                list.Add(go2);
            }
            StartCoroutine(cal());
        }
    }

    IEnumerator cal()
    {
        audio.clip = other;
        audio.Play();
        yield return new WaitForSeconds(1);
        die = false;
    }

    public void defend()
    {
        if (!die)
        {
            inDefend = true;
            animator.SetTrigger("EndAttacked");
            animator.SetBool("Defend", true);
            beAttack = false;
            gravity = true;
            beAttackTimer = 0;
            defendTimer = 0;
        }
    }

    public void takeDamage(int damage)
    {
        if (!die && !inDefend)
        {
            if (canAttacked)
            {
                audio.clip = attacked;
                audio.Play();
                _rigidbody.velocity = Vector2.zero;
                beAttack = true;
                if (canUp)
                    gravity = false;
                An.transform.Rotate(0, 0, 60);
                animator.ResetTrigger("EndAttacked");
                animator.SetTrigger("beAttacked");
            }
            //暴击
            float i = Random.Range(0f, 1f);
            if (i < attackS + luck / 3)
              damage+=damage;
            //破甲
            float j = Random.Range(0f, 1f);
            if (j >= attackP + luck / 3)
                damage -= defendValue;
            if (damage > 0)
                hp -= damage;
            else
                damage = 0;
            GameObject damageGo = Instantiate(damageText, transform.position + new Vector3(0, 1, 0), Quaternion.identity) as GameObject;
            damageGo.GetComponent<DamageText>().Value = damage;
            if (hp <= 0)
                dead();
        }
    }

    public void dead()
    {
        if (!die)
        {
            die = true;
            for (int i = 0; i < list.Count; i++)
                if (list[i] != null)
                    list[i].GetComponent<Enemy>().dead();
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        animator.SetTrigger("Die");
        int i = 0;
        if (BattleManager.instance.combo > 80)
            i = 10;
        else if (BattleManager.instance.combo > 50)
            i = 6;
        else if (BattleManager.instance.combo > 20)
            i = 3;
        gameManager.instance.spirit += (spirit+i);
        gameManager.instance.kill++;
        BattleManager.instance.levelKill++;
        if (!gameManager.instance.MYes.Contains(enemyIndex < 10 ? "0" + enemyIndex : "" + enemyIndex))
            gameManager.instance.unlock('M', enemyIndex);
        yield return new WaitForSeconds(2);
        GameObject.Destroy(this.gameObject);
    }

    public void lie()
    {
        if (canAttacked)
        {
            die = true;
            beAttack = false;
            gravity = true;
            beAttackTimer = 0;
            animator.ResetTrigger("EndAttacked");
            animator.SetTrigger("Lie");
        }
    }

    public void forBottomTrigger(string choose)
    {
        if (choose == "can")
        {
            jumpState = 0;
            if (gravity)
            {
                animator.SetBool("isInAir", false);
            }
        }
        else
        {
            jumpState = 1;
            if (gravity)
            {
                animator.SetBool("isInAir", true);
            }
        }
    }

}
