using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //独有数据
    public int Hp;      //当前血量
    public int Sp;      //当前SP
    //需要和管理器同步的数据
    private int AttackValue;       //攻击力
    private int DefendValue;        //防御力，可以抵消敌人的攻击
    private int MaxHp;              //血量上限
    private int MaxSp;              //SP上限
    private float JumpForce;       //跳跃力
    private float AttackForce;      //击退强度
    private float MoveSpeed;       //移动速度
    private int getHp;              //角色单次回血获得的生命
    private int getSp;              //角色单次平A敌人获得的SP
    private float luck;

    private float miss;   //闪避率
    private bool canAttacked = true;
    private bool crazy = false; //暴乱
    private bool get = false; //汲取
    private bool AllSp = false; //坎效果
    private bool AllHp = false; //离效果
    private bool getSpirit = false; //艮效果
    private bool fly = false;      //二段跳
    [HideInInspector]
    public bool gravity = true;    //重力开关
    private int getHpNeed;          //角色回血技能消耗的SP
    private float ComboTime;      //连击判定时间
    private float AttackRange;      //攻击范围
    [HideInInspector]
    public float AttackSpeed;    //允许攻击的按键检测间隔时间

    //其他数据
    [HideInInspector]
    public bool inGetHp = false;       //回血状态开关
    private bool onAttack = false;  //是否在攻击开关
    [HideInInspector]
    public bool onSp = false;  //是否在大招开关
    private float getHpSkillTime = 1;   //回血技能需要的时间

    //计时器和其他属性
    private float attackAnimaTimer = 0; //攻击动画播放计时器，用来确保攻击时不能做其他事情
    private float getHpTimer = 0;       //回血技能计时器
    private float attackTimer = 0;      //攻击间隔计时器，用来计算连击和限制攻击速度
    private float beAttackTimer = 0;
    private float jumpTimer = 0;
    private float gravityTimer = 0;
    private float AgetHpTimer = 0;
    private float AgetSpTimer = 0;

    [HideInInspector]
    public bool beAttack = false;
    [HideInInspector]
    public bool die = false;

    [HideInInspector]
    public int jumpState = 0;    //0表示在地面，1是起跳，2是完成二段跳
    [HideInInspector]
    public int attackState = 0;  //0表示闲置，1表示攻击了一次，2表示连击了两次,3完成连击
    [HideInInspector]
    public char turn = 'L';     //朝向(只用来记录，不用来更改)

    //组件
    private Animator animator;
    [HideInInspector]
    public Rigidbody2D _rigidbody;
    private CapsuleCollider2D _collider;

    public GameObject damageText;

    //音乐音效
    public AudioSource audio;
    public AudioSource Runaudio;
    public AudioClip[] SE;
    public AudioClip down;

    private void Start()
    {
        animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CapsuleCollider2D>();
        initiate();
        Hp = MaxHp;
        Sp = 20;
    }

    void Update()
    {
        if (attackState == 5 && attackTimer > 2*AttackSpeed)
            attackState = 0;
        else if (attackState > 0 && attackTimer > AttackSpeed + ComboTime)
            attackState = 0;
        if (onAttack)
        {
            if (attackAnimaTimer >= AttackSpeed)
            {
                onAttack = false;
                attackAnimaTimer = 0;
            }
        }
        else
        {
            if (gravityTimer > 0.3f && gameManager.instance.gravity)
                gravity = true;
        }
        if (gravity)
        {
            _rigidbody.gravityScale = 3;
            animator.SetFloat("GSpeed", _rigidbody.velocity.y);
        }
        else
        {
            _rigidbody.gravityScale = 0;
            animator.SetFloat("GSpeed", 0);
            if(!gameManager.instance.gravity)
                animator.SetBool("isInAir", false);
        }
        initiate();

        if (Application.platform != RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                Attack();
            }
        }
    }

    private void FixedUpdate()
    {
        attackTimer += Time.deltaTime;
        jumpTimer += Time.deltaTime;
        if(beAttack)
            beAttackTimer += Time.deltaTime;
        if (onAttack)
        {
            attackAnimaTimer += Time.deltaTime;
        }
        if (inGetHp && !onAttack && !beAttack)
        {
            getHpTimer += Time.deltaTime;
            animator.SetBool("getHp", true);
            if (getHpTimer >= getHpSkillTime && Sp >= getHpNeed)
            {
                changeSp(-getHpNeed);
                changeHp(getHp);
                takeDamage(-getHp);
                animator.SetBool("getHp", false);
                getHpTimer = 0;
                inGetHp = false;
            }
        }
        else
        {
            animator.SetBool("getHp", false);
            getHpTimer = 0;
        }
        if (!gravity && gameManager.instance.gravity)
        {
            gravityTimer += Time.deltaTime;
        }
        if (beAttack && beAttackTimer >= 0.3f)
        {
            Move(0);
            beAttack = false;
            animator.SetBool("beAttack", false);
            beAttackTimer = 0;
        }
        if (gameManager.instance.AgetHp)
        {
            AgetHpTimer += Time.deltaTime;
            if (AgetHpTimer > 2)
            {
                AgetHpTimer = 0;
                Hp += 50;
            }
        }
        if (gameManager.instance.AgetSp)
        {
            AgetSpTimer += Time.deltaTime;
            if (AgetSpTimer > 2)
            {
                AgetSpTimer = 0;
                Sp += 30;
            }
        }
        audio.volume = gameManager.instance.SE;
        Runaudio.volume = gameManager.instance.SE;
    }

    //update刷新太快，超过了键盘io的反应时间，用一个Timer来限制读取。
    //Timer计时器和各种物理判定放在fixedUpdate
    //update放读取输入
    //以update的速度读取输入，以fixedUpdate中的Timer的频率决定代码是否执行

    //和管理器同步数据
    private void initiate()
    {
        AttackValue = gameManager.instance.AttackValue;
        DefendValue = gameManager.instance.DefendValue;
        MaxHp = gameManager.instance.MaxHp;
        MaxSp = gameManager.instance.MaxSp;
        JumpForce = gameManager.instance.JumpForce;
        AttackForce = gameManager.instance.AttackForce;
        MoveSpeed = gameManager.instance.MoveSpeed;
        getSp = gameManager.instance.getSp;
        getHp = gameManager.instance.getHp;
        luck = gameManager.instance.Luck;
        miss = gameManager.instance.miss;
        canAttacked = gameManager.instance.canAttacked;
        crazy = gameManager.instance.crazy;
        get = gameManager.instance.get;
        AllHp = gameManager.instance.AllHp;
        AllSp = gameManager.instance.AllSp;
        getSpirit = gameManager.instance.getSpirit;
        fly = gameManager.instance.fly;
        getHpNeed = gameManager.instance.getHpNeed;
        ComboTime = gameManager.instance.ComboTime;
        AttackRange = gameManager.instance.AttackRange;
        AttackSpeed = gameManager.instance.AttackSpeed;
}

    public void Jump()
    {
        if (!gravity || beAttack || die || onSp) { }
        else
        {
            if (jumpState == 0)
            {
                if (SE[3] != null)
                    playSE(SE[3]);
                animator.SetTrigger("Jump");
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, JumpForce);
                jumpState = 1;
                jumpTimer = 0;
            }
            else if (jumpState == 1 && fly && jumpTimer > 0.2f)
            {
                if (SE[3] != null)
                    playSE(SE[3]);
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, JumpForce-12);
                jumpState = 2;
            }
        }
    }

    public void Move(float X)
    {
        if (onSp || onAttack || beAttack || die)
        {
            Runaudio.Stop();
            animator.SetBool("isRun", false);
        }
        else {
            float move = X * MoveSpeed;
            if (move > 0)
            {
                if (jumpState == 0)
                {
                    if (Runaudio.isPlaying == false)
                        Runaudio.Play();
                    animator.SetBool("isRun", true);
                }
                else
                {
                    Runaudio.Stop();
                    animator.SetBool("isRun", false);
                }
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                turn = 'R';
            }
            else if (move < 0)
            {
                if (jumpState == 0)
                {
                    if (Runaudio.isPlaying == false)
                        Runaudio.Play();
                    animator.SetBool("isRun", true);
                }
                else
                {
                    Runaudio.Stop();
                    animator.SetBool("isRun", false);
                }
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                turn = 'L';
            }
            else
            {
                Runaudio.Stop();
                animator.SetBool("isRun", false);
            }

            _rigidbody.velocity = new Vector2(move, _rigidbody.velocity.y);
        }
    }

    public void Attack()
    {
        if (beAttack || die || onSp) { }
        else
        {
                onAttack = true;
                _rigidbody.velocity = Vector2.zero;
                gravity = false;
                gravityTimer = 0;
                 if (attackState == 0)
                {
                    attackTimer = 0;
                    attackState = 1;
                    if (SE[0] != null)
                        playSE(SE[0]);
                    animator.SetTrigger("Attack1");

                    Collider2D[] list = Physics2D.OverlapCapsuleAll(transform.position + new Vector3(turn == 'L' ? -1 : 1, 0.5f, 0), new Vector2(AttackRange, AttackRange), CapsuleDirection2D.Vertical, 0);
                    foreach (var got in list)
                    {
                        if (got.gameObject.tag == "Enemy")
                        {
                            Enemy go = got.gameObject.GetComponent<Enemy>();
                            BattleManager.instance.PlayerCombo();
                            if (go.die == false)
                            {   
                                go.takeDamage(AttackValue);
                                if (go.canAttacked)
                                    go.gameObject.GetComponent<Rigidbody2D>().velocity += new Vector2(turn == 'L' ? -1 : 1, 1);
                                changeSp(getSp);
                                if (get)
                                    changeHp(5);
                            }
                        }
                    if (got.gameObject.tag == "item")
                        got.GetComponent<Item>().doSth();
                    if (got.gameObject.tag == "item1")
                        got.GetComponent<Item1>().doSth();
                        if (got.gameObject.tag == "box")
                            got.GetComponent<Box>().doSth();
                    }
                }
                else if (attackState == 1 && attackTimer > AttackSpeed && attackTimer < AttackSpeed + ComboTime)
                {
                    attackTimer = 0;
                    attackState = 2;
                    if (SE[1] != null)
                        playSE(SE[1]);
                    animator.SetTrigger("Attack2");

                    Collider2D[] list = Physics2D.OverlapCapsuleAll(transform.position + new Vector3(turn == 'L' ? -1 : 1, 0.5f, 0), new Vector2(AttackRange, AttackRange), CapsuleDirection2D.Vertical, 0);
                    foreach (var got in list)
                    {
                        if (got.gameObject.tag == "Enemy")
                        {
                            Enemy go = got.gameObject.GetComponent<Enemy>();
                        BattleManager.instance.PlayerCombo();
                        if (go.die == false)
                            {
                            go.takeDamage(AttackValue);
                            if (go.canAttacked)
                                go.gameObject.GetComponent<Rigidbody2D>().velocity += new Vector2(turn == 'L' ? -1 : 1, 1);
                                changeSp(getSp);
                            if (get)
                                changeHp(5);
                             }
                        }
                    if (got.gameObject.tag == "item")
                        got.GetComponent<Item>().doSth();
                    if (got.gameObject.tag == "item1")
                        got.GetComponent<Item1>().doSth();
                    if (got.gameObject.tag == "box")
                        got.GetComponent<Box>().doSth();
                }
                }
                else if (attackState == 2 && attackTimer > AttackSpeed && attackTimer < AttackSpeed + ComboTime )
                {
                    attackTimer = 0;
                    attackState = 3;
                    if (SE[0] != null)
                        playSE(SE[0]);
                    animator.SetTrigger("Attack3");

                    Collider2D[] list = Physics2D.OverlapCapsuleAll(transform.position + new Vector3(turn == 'L' ? -1 : 1, 0.5f, 0), new Vector2(AttackRange, AttackRange), CapsuleDirection2D.Vertical, 0);
                    foreach (var got in list)
                    {
                        if (got.gameObject.tag == "Enemy")
                        {
                            Enemy go = got.gameObject.GetComponent<Enemy>();
                        BattleManager.instance.PlayerCombo();
                        if (go.die == false)
                            {
                            go.takeDamage(AttackValue);
                            if (go.canAttacked)
                                go.gameObject.GetComponent<Rigidbody2D>().velocity += new Vector2(turn == 'L' ? -1 : 1, 1);
                                changeSp(getSp);
                            if (get)
                                changeHp(5);
                        }
                        }
                    if (got.gameObject.tag == "item")
                        got.GetComponent<Item>().doSth();
                    if (got.gameObject.tag == "item1")
                        got.GetComponent<Item1>().doSth();
                    if (got.gameObject.tag == "box")
                        got.GetComponent<Box>().doSth();
                }
                }
                else if (attackState == 3 && attackTimer > AttackSpeed && attackTimer < AttackSpeed + ComboTime )
                {
                    attackTimer = 0;
                    attackState = 4;
                    if (SE[1] != null)
                        playSE(SE[1]);
                    animator.SetTrigger("Attack4");

                    Collider2D[] list = Physics2D.OverlapCapsuleAll(transform.position + new Vector3(turn == 'L' ? -1 : 1, 0.5f, 0), new Vector2(AttackRange, AttackRange), CapsuleDirection2D.Vertical, 0);
                    foreach (var got in list)
                    {
                        if (got.gameObject.tag == "Enemy")
                        {
                            Enemy go = got.gameObject.GetComponent<Enemy>();
                        BattleManager.instance.PlayerCombo();
                        if (go.die == false)
                            {
                                if (jumpState == 0)
                                {
                                go.takeDamage(AttackValue);
                                if (go.canAttacked)
                                    go.gameObject.GetComponent<Rigidbody2D>().velocity += new Vector2(turn == 'L' ? -1 : 1, 1);
                                 }
                                else
                                {
                                if (crazy)
                                {
                                    go.takeDamage(AttackValue*2);
                                }
                                else
                                {
                                    go.takeDamage(AttackValue);
                                }
                                if (go.canUp)
                                {
                                    go.gravity = true;
                                    go.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(turn == 'L' ? -AttackForce : AttackForce, -25);
                                    if (go.canAttacked)
                                        go.lie();
                                }
                                }
                                    changeSp(getSp);
                                if (get)
                                    changeHp(5);
                            }
                        }
                    if (got.gameObject.tag == "item")
                        got.GetComponent<Item>().doSth();
                    if (got.gameObject.tag == "item1")
                        got.GetComponent<Item1>().doSth();
                    if (got.gameObject.tag == "box")
                        got.GetComponent<Box>().doSth();
                }
                if (jumpState != 0)
                {
                    gravity = true;
                    _rigidbody.velocity = new Vector2(0, -30);
                }
            }
                else if (attackState == 4 && attackTimer > AttackSpeed && attackTimer < AttackSpeed + ComboTime && jumpState==0)
                {
                    attackTimer = 0;
                    attackState = 5;
                    if (SE[2] != null)
                        playSE(SE[2]);
                    animator.SetTrigger("Attack5");

                    Collider2D[] list = Physics2D.OverlapCapsuleAll(transform.position + new Vector3(turn == 'L' ? -1 : 1, 0.5f, 0), new Vector2(AttackRange, AttackRange), CapsuleDirection2D.Vertical, 0);
                    foreach (var got in list)
                    {
                        if (got.gameObject.tag == "Enemy")
                        {
                            Enemy go = got.gameObject.GetComponent<Enemy>();
                        BattleManager.instance.PlayerCombo();
                        if (!go.die && !go.inDefend)
                            {
                                if (crazy)
                                {
                                    go.takeDamage(AttackValue*2);
                                }
                                else
                                {
                                    go.takeDamage(AttackValue);
                                }
                            if (go.canUp)
                            {
                                go.gravity = true;
                                go.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(turn == 'L' ? -AttackForce : AttackForce, 0.6f * AttackForce);
                                if (go.canAttacked)
                                    go.lie();
                            }
                                changeSp(getSp);
                            if (get)
                                changeHp(5);
                        }
                        }
                    if (got.gameObject.tag == "item")
                        got.GetComponent<Item>().doSth();
                    if (got.gameObject.tag == "item1")
                        got.GetComponent<Item1>().doSth();
                    if (got.gameObject.tag == "box")
                        got.GetComponent<Box>().doSth();
                }
                    _rigidbody.velocity += (new Vector2(turn == 'L' ? 2 : -2, 0));

                }
        }
    }

    public void AttackUp()
    {
        if (beAttack || die || jumpState != 0 || onSp) { }
        else
        {
            onAttack = true;
            _rigidbody.velocity = Vector2.zero;
            gravity = false;
            gravityTimer = 0;

            if (SE[2] != null)
                    playSE(SE[2]);
                animator.SetTrigger("Attack5");

                Collider2D[] list = Physics2D.OverlapCapsuleAll(transform.position + new Vector3(turn == 'L' ? -1 : 1, 0.5f, 0), new Vector2(AttackRange, AttackRange), CapsuleDirection2D.Vertical, 0);
                foreach (var got in list)
                {
                    if (got.gameObject.tag == "Enemy")
                    {
                        Enemy go = got.gameObject.GetComponent<Enemy>();
                    BattleManager.instance.PlayerCombo();
                    if (!go.die && !go.inDefend)
                        {
                            go.takeDamage(AttackValue);
                        if (go.canUp)
                        {
                            go.gravity = true;
                            go.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 2f * AttackForce);
                        }
                            changeSp(getSp);
                        if (get)
                            changeHp(5);
                    }
                    }
                if (got.gameObject.tag == "item")
                    got.GetComponent<Item>().doSth();
                if (got.gameObject.tag == "item1")
                    got.GetComponent<Item1>().doSth();
                if (got.gameObject.tag == "box")
                    got.GetComponent<Box>().doSth();
            }
        }
    }

    public void AttackDown()
    {
        if (beAttack || die || jumpState == 0 || onSp) { }
        else
        {
            onAttack = true;
            _rigidbody.velocity = Vector2.zero;
            gravity = false;
            gravityTimer = 0;

            if (SE[2] != null)
                playSE(SE[2]);
            animator.SetTrigger("Attack4");

            Collider2D[] list = Physics2D.OverlapCapsuleAll(transform.position + new Vector3(turn == 'L' ? -1 : 1, 0.5f, 0), new Vector2(AttackRange, AttackRange), CapsuleDirection2D.Vertical, 0);
            foreach (var got in list)
            {
                if (got.gameObject.tag == "Enemy")
                {
                    Enemy go = got.gameObject.GetComponent<Enemy>();
                    BattleManager.instance.PlayerCombo();
                    if (!go.die && !go.inDefend)
                    {
                        go.takeDamage(AttackValue);
                        if (go.canUp)
                        {
                            go.gravity = true;
                            go.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(turn == 'L' ? -AttackForce : AttackForce, -25);
                            if (go.canAttacked)
                                go.lie();
                        }
                        changeSp(getSp);
                        if (get)
                            changeHp(5);
                    }
                }
                if (got.gameObject.tag == "item")
                    got.GetComponent<Item>().doSth();
                if (got.gameObject.tag == "item1")
                    got.GetComponent<Item1>().doSth();
                if (got.gameObject.tag == "box")
                    got.GetComponent<Box>().doSth();
            }
            gravity = true;
            _rigidbody.velocity = new Vector2(0, -30);
            Collider2D[] list2 = Physics2D.OverlapCapsuleAll(transform.position + new Vector3(turn == 'L' ? 0 : 0, 0, 0), new Vector2(AttackRange + 2, AttackRange), CapsuleDirection2D.Horizontal, 0);
            foreach (var got in list2)
            {
                if (got.gameObject.tag == "Enemy")
                {
                    Enemy go = got.gameObject.GetComponent<Enemy>();
                    if (!go.die && !go.inDefend)
                    {
                        go.takeDamage(AttackValue);
                        if (go.canUp)
                            go.lie();
                        changeSp(getSp);
                    }
                }
                if (got.gameObject.tag == "item")
                    got.GetComponent<Item>().doSth();
                if (got.gameObject.tag == "item1")
                    got.GetComponent<Item1>().doSth();
                if (got.gameObject.tag == "box")
                    got.GetComponent<Box>().doSth();
            }
        }
    }

    //===================================特殊技能=================================
    public void AttackS1()
    {
        if (beAttack || die|| onSp) { }
        else
        {
            if (Sp >= 40)
            {
                beAttack = false;
                animator.SetBool("beAttack", false);
                beAttackTimer = 0;

                _rigidbody.velocity = Vector2.zero;
                gravity = false;
                gravityTimer = 0;
                changeSp(-40);
                StartCoroutine(AS1());
            }
        }
    }

    IEnumerator AS1()
    {
        animator.SetTrigger("sp1");
        BattleManager.instance.pauseEnemy(true);
        Camera.main.GetComponent<CameraFollow>().changeCamera(true);
        yield return new WaitForSeconds(0.5f);
        Camera.main.GetComponent<CameraFollow>().changeCamera(false);
        BattleManager.instance.pauseEnemy(false);
        yield return new WaitForSeconds(0.1f);
        if (SE[2] != null)
            playSE(SE[2]);
        _rigidbody.velocity = new Vector2(turn=='L'? -3*MoveSpeed :3 *MoveSpeed, 0);
        yield return new WaitForSeconds(0.2f);
        Collider2D[] list = Physics2D.OverlapCapsuleAll(transform.position, new Vector2(12, 4), CapsuleDirection2D.Horizontal, 0);
        int i = 0;
        foreach (var got in list)
        {
            if (got.gameObject.tag == "Enemy")
            {
                Enemy go = got.gameObject.GetComponent<Enemy>();
                if (!go.die && !go.inDefend)
                {
                    go.takeDamage(2*AttackValue);
                    if (go.canUp)
                    {
                        go.gravity = true;
                        go.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 2f * AttackForce);
                    }
                }
                i++;
            }
            if (got.gameObject.tag == "item")
                got.GetComponent<Item>().doSth();
            if (got.gameObject.tag == "item1")
                got.GetComponent<Item1>().doSth();
            if (got.gameObject.tag == "box")
                got.GetComponent<Box>().doSth();
        }
        yield return new WaitForSeconds(0.2f);
        Collider2D[] list2 = Physics2D.OverlapCapsuleAll(transform.position, new Vector2(12, 4), CapsuleDirection2D.Horizontal, 0);
        foreach (var got in list2)
        {
            if (got.gameObject.tag == "Enemy")
            {
                Enemy go = got.gameObject.GetComponent<Enemy>();
                if (!go.die && !go.inDefend)
                {
                    go.takeDamage(2*AttackValue);
                    if (go.canUp)
                    {
                        go.gravity = true;
                        go.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 2f * AttackForce);
                    }
                }
            }
            if (got.gameObject.tag == "item")
                got.GetComponent<Item>().doSth();
            if (got.gameObject.tag == "item1")
                got.GetComponent<Item1>().doSth();
            if (got.gameObject.tag == "box")
                got.GetComponent<Box>().doSth();
        }
        yield return new WaitForSeconds(0.2f);
        Collider2D[] list3 = Physics2D.OverlapCapsuleAll(transform.position, new Vector2(12, 4), CapsuleDirection2D.Horizontal, 0);
        foreach (var got in list3)
        {
            if (got.gameObject.tag == "Enemy")
            {
                Enemy go = got.gameObject.GetComponent<Enemy>();
                if (!go.die && !go.inDefend)
                {
                    go.takeDamage(3*AttackValue);
                    if (go.canUp)
                    {
                        go.gravity = true;
                        go.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 2f * AttackForce);
                    }
                }
                i++;
            }
            if (got.gameObject.tag == "item")
                got.GetComponent<Item>().doSth();
            if (got.gameObject.tag == "item1")
                got.GetComponent<Item1>().doSth();
            if (got.gameObject.tag == "box")
                got.GetComponent<Box>().doSth();
        }
        if (i >= 10)
        {
            if (!gameManager.instance.ach.Contains("13"))
                gameManager.instance.getAchievemrnt(13);
        }
        yield return new WaitForSeconds(0.1f);
        _rigidbody.velocity = Vector2.zero;
    }

    public void AttackS2()
    {
        if (beAttack || die || onSp) { }
        else
        {
            if (Sp >= 40)
            {
                beAttack = false;
                animator.SetBool("beAttack", false);
                beAttackTimer = 0;

                _rigidbody.velocity = Vector2.zero;
                changeSp(-40);
                StartCoroutine(AS2());
            }
        }
    }

    IEnumerator AS2()
    {
        animator.SetTrigger("sp2");
        yield return new WaitForSeconds(0.15f);
        _rigidbody.velocity = new Vector2(3, 30);
        yield return new WaitForSeconds(0.25f);
        gravity = false;
        _rigidbody.velocity = Vector2.zero;
        gravityTimer = 0;
        BattleManager.instance.pauseEnemy(true);
        Camera.main.GetComponent<CameraFollow>().changeCamera(true);
        yield return new WaitForSeconds(0.5f);
        Camera.main.GetComponent<CameraFollow>().changeCamera(false);
        BattleManager.instance.pauseEnemy(false);
        if (SE[2] != null)
            playSE(SE[2]);
        _rigidbody.velocity = new Vector2(3, -30);
        yield return new WaitForSeconds(0.15f);
        Collider2D[] list = Physics2D.OverlapCapsuleAll(transform.position, new Vector2(15, 6), CapsuleDirection2D.Horizontal, 0);
        int i= 0;
        foreach (var got in list)
        {
            if (got.gameObject.tag == "Enemy")
            {
                Enemy go = got.gameObject.GetComponent<Enemy>();
                if (!go.die && !go.inDefend)
                {
                    go.takeDamage(3 * AttackValue);
                    if (go.canUp)
                    {
                        go.gravity = true;
                        go.lie();
                        go.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(go.turn=='L'?-AttackForce : AttackForce, AttackForce);
                    }
                }
                i++;
            }
            if (got.gameObject.tag == "item")
                got.GetComponent<Item>().doSth();
            if (got.gameObject.tag == "item1")
                got.GetComponent<Item1>().doSth();
            if (got.gameObject.tag == "box")
                got.GetComponent<Box>().doSth();
        }
        if (i >= 10)
        {
            if (!gameManager.instance.ach.Contains("13"))
                gameManager.instance.getAchievemrnt(13);
        }
        yield return new WaitForSeconds(0.25f);
        _rigidbody.velocity = Vector2.zero;
    }

    public void AttackS3()
    {
        if (beAttack || die || onSp) { }
        else
        {
            if (Sp >= 40)
            {
                beAttack = false;
                animator.SetBool("beAttack", false);
                beAttackTimer = 0;

                _rigidbody.velocity = Vector2.zero;
                changeSp(-40);
                StartCoroutine(AS3());
            }
        }
    }

    IEnumerator AS3()
    {
        animator.SetTrigger("sp3");
        BattleManager.instance.pauseEnemy(true);
        Camera.main.GetComponent<CameraFollow>().changeCamera(true);
        yield return new WaitForSeconds(0.4f);
        Camera.main.GetComponent<CameraFollow>().changeCamera(false);
        BattleManager.instance.pauseEnemy(false);
        gravity = false;
        _rigidbody.velocity = Vector2.zero;
        gravityTimer = 0;
        yield return new WaitForSeconds(0.05f);
        if (SE[2] != null)
            playSE(SE[2]);
        Collider2D[] list = Physics2D.OverlapCapsuleAll(transform.position + new Vector3(turn == 'L' ? -1 : 1, 0.5f, 0), new Vector2(6, 5), CapsuleDirection2D.Horizontal, 0);
        int i = 0;
        foreach (var got in list)
        {
            if (got.gameObject.tag == "Enemy")
            {
                Enemy go = got.gameObject.GetComponent<Enemy>();
                if (!go.die && !go.inDefend)
                {
                    go.takeDamage(3 * AttackValue);
                    if (go.canAttacked)
                        go.gameObject.GetComponent<Rigidbody2D>().velocity += new Vector2(turn == 'L' ? -1 : 1, 1);
                }
                i++;
            }
            if (got.gameObject.tag == "item")
                got.GetComponent<Item>().doSth();
            if (got.gameObject.tag == "item1")
                got.GetComponent<Item1>().doSth();
            if (got.gameObject.tag == "box")
                got.GetComponent<Box>().doSth();
        }
        yield return new WaitForSeconds(0.25f);
        Collider2D[] list2 = Physics2D.OverlapCapsuleAll(transform.position + new Vector3(turn == 'L' ? -1 : 1, 0.5f, 0), new Vector2(6, 5), CapsuleDirection2D.Horizontal, 0);
        foreach (var got in list2)
        {
            if (got.gameObject.tag == "Enemy")
            {
                Enemy go = got.gameObject.GetComponent<Enemy>();
                if (!go.die && !go.inDefend)
                {
                    go.takeDamage(3 * AttackValue);
                    if (go.canAttacked)
                        go.gameObject.GetComponent<Rigidbody2D>().velocity += new Vector2(turn == 'L' ? -1 : 1, 1);
                }
                i++;
            }
            if (got.gameObject.tag == "item")
                got.GetComponent<Item>().doSth();
            if (got.gameObject.tag == "item1")
                got.GetComponent<Item1>().doSth();
            if (got.gameObject.tag == "box")
                got.GetComponent<Box>().doSth();
        }
        yield return new WaitForSeconds(0.45f);
        Collider2D[] list3 = Physics2D.OverlapCapsuleAll(transform.position + new Vector3(turn == 'L' ? -1 : 1, 0.5f, 0), new Vector2(6, 5), CapsuleDirection2D.Horizontal, 0);
        foreach (var got in list3)
        {
            if (got.gameObject.tag == "Enemy")
            {
                Enemy go = got.gameObject.GetComponent<Enemy>();
                if (!go.die && !go.inDefend)
                {
                    go.takeDamage(4 * AttackValue);
                        if (go.canAttacked)
                            go.lie();
                }
                i++;
            }
            if (got.gameObject.tag == "item")
                got.GetComponent<Item>().doSth();
            if (got.gameObject.tag == "item1")
                got.GetComponent<Item1>().doSth();
            if (got.gameObject.tag == "box")
                got.GetComponent<Box>().doSth();
        }
        _rigidbody.velocity = Vector2.zero;
        if (i >= 10)
        {
            if (!gameManager.instance.ach.Contains("13"))
                gameManager.instance.getAchievemrnt(13);
        }
    }

    public void addBuff(int i)
    {
        if (beAttack || die || onSp) { }
        else
        {
            if (Sp >= 30)
            {
                beAttack = false;
                animator.SetBool("beAttack", false);
                beAttackTimer = 0;

                changeSp(-30);
                animator.SetTrigger("buff" + i);
                gameManager.instance.checkBuff(i, true);
            }
        }
    }

    //正为加负为减
    public void changeHp(int hp)
    {
        if (Hp + hp <= MaxHp && Hp + hp >= 0)
            Hp += hp;
        else if (Hp + hp <= 0)
            Hp = 0;
        else
            Hp = MaxHp;
    }
    public void changeSp(int sp)
    {
        if (Sp + sp <= MaxSp && Sp + sp >= 0)
            Sp += sp;
        else if (Sp + sp <= 0)
            Sp = 0;
        else
            Sp = MaxSp;
    }

    public void takeDamage(int damage)
    {
        if (!die && !onSp)
        {
            if (canAttacked)
            {
                if(gameManager.instance.gravity)
                    gravity = true;
                beAttack = true;
                beAttackTimer = 0;
                animator.SetBool("beAttack", true);
            }
            if ((damage > 0))
            {
                float i = Random.Range(0f, 1f);
                if (i >= miss + luck / 3)
                {
                    damage -= DefendValue;
                    if (damage > 0)
                    {
                        if (gameManager.instance.debug)
                            changeHp(-1);
                        else if (gameManager.instance.noBreak && damage>20)
                            changeHp(-20);
                        else
                            changeHp(-damage);
                        BattleManager.instance.PlayerAttacked();
                        if (getSpirit)
                        {
                            float k = Random.Range(0f, 1f);
                            if (k < 0.1f+luck / 3)
                                gameManager.instance.spirit += 10;
                        }
                        if (AllHp)
                        {
                            float k = Random.Range(0f, 1f);
                            if (k < 0.05f + luck / 4)
                                Hp = MaxHp;
                        }
                        if (AllSp)
                        {
                            float k = Random.Range(0f, 1f);
                            if (k < 0.05f + luck / 4)
                                Sp = MaxSp;
                        }
                    }
                    else
                        damage = 0;
                }
                else
                    damage = -999999;
            }
            GameObject damageGo = Instantiate(damageText, transform.position + new Vector3(0, 1, 0), Quaternion.identity) as GameObject;
            damageGo.GetComponent<DamageText>().Value = damage;
            damageGo.GetComponent<DamageText>().ifMe=true;
            if (Hp <= 0)
            {
                playSE(down);
                animator.SetBool("beAttack", false);
                animator.SetTrigger("Die");
                die = true;
                BattleManager.instance.PlayerDie();
            }
        }
    }

    public void playSE(AudioClip clip)
    {
        audio.clip = clip;
        audio.Play();
    }

    public void forBottomTrigger(string choose)
    {
        if (choose == "can")
        {
            if (!die)
            {
                jumpState = 0;
                if (gravity)
                    animator.SetBool("isInAir", false);
            }
        }
        else
        {
            if (!die)
            {
                jumpState = 1;
                if (gravity)
                    animator.SetBool("isInAir", true);
            }
        }
    }
}

