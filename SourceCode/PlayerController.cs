using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float jumpPower;


    //bool값
    bool isJump;
    bool isLeftAttack;
    bool firstAttack;
    bool shieldCoolTime;
    bool jumpSkillOn;
    bool attackSkillOn;

    List<GameObject> contactMonsters;
    List<GameObject> contactCollisionMonsters;
    GameObject removeMonster;

    [SerializeField]
    GameObject jumpSkillEffect;
    Rigidbody2D rigid;
    Animator animator;

    CircleCollider2D circleCollider;
    void Start()
    {
        InitSetting();

    }


    void InitSetting()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        contactMonsters = new List<GameObject>();
        contactCollisionMonsters = new List<GameObject>();
        circleCollider = GetComponentInChildren<CircleCollider2D>();
        UIController.Instance.jumpAction += Jump;
        UIController.Instance.attackAction += Attack;
        UIController.Instance.jumpSkillAction += JumpSkill;
        UIController.Instance.attackSkillAction += AttackSkill;
        UIController.Instance.initAction += InitPlayer;
    }


    void InitPlayer()
    {
        StopAllCoroutines();
        jumpSkillOn = false;
        jumpSkillEffect.SetActive(false);
        circleCollider.radius = 1.6f;
        attackSkillOn = false;
        shieldCoolTime = false;
        firstAttack = false;
    }

    void Jump()
    {
        if (!isJump)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            //animator.SetBool("isJump", true);
            //isJump = true;

            GameManager.Instance.JumpCount();

        }

    }


    void Attack()
    {
        if (!firstAttack)
        {
            firstAttack = true;
            StartCoroutine(CheckFirstAttack());
        }

        if (attackSkillOn)
        {
            Vector2 tempPos = transform.position;
            tempPos.y += 1;
            MonsterPool.Instance.get(ObjectFlag.AttackSkill, tempPos);
        }
        //첫공격 모션
        if (!isLeftAttack)
        {
            animator.SetTrigger("isLeftAttack");
            isLeftAttack = true;

        }
        else //연속 공격 시 두번 째 모션
        {
            animator.SetTrigger("isRightAttack");
            isLeftAttack = false;
        }

        //몬스터 제거
        if (contactMonsters.Count > 0)
        {
            //임시 이후 풀링으로 바꿈

            removeMonster = contactMonsters[0];
            contactMonsters.RemoveAt(0);
            if (removeMonster != null)
            {
                MonsterPool.Instance.Set(removeMonster, ObjectFlag.Monster);
                GameManager.Instance.DestroyMonster();
                AudioController.Instance.PlayOnce();

                if (!attackSkillOn)
                    GameManager.Instance.AttackCount();
            }
        }
    }

    //공격 멈춤 체크
    IEnumerator CheckFirstAttack()
    {
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("LeftAttack"));
       yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")|| animator.GetCurrentAnimatorStateInfo(0).IsName("Jump"));
        firstAttack = false;
        isLeftAttack = false;
        Debug.Log(111);

    }



    //점프 스킬
    public void JumpSkill()
    {
        StartCoroutine(JumpSkillCo());
        IEnumerator JumpSkillCo()
        {

            isJump = true;
            jumpSkillOn = true; 
            jumpSkillEffect.SetActive(true);

            rigid.velocity = Vector2.zero;
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);

            for (int i = 0; i < contactMonsters.Count; i++)
            {
                
                MonsterPool.Instance.Set(contactMonsters[i], ObjectFlag.Monster);
                GameManager.Instance.DestroyMonster();
            }
            
            contactMonsters.Clear();

            yield return new WaitUntil(()=>!isJump);

            jumpSkillOn = false;
            jumpSkillEffect.SetActive(false);


        }
    }

    public void AttackSkill()
    {
        StartCoroutine(AttackRangeExtend());
        IEnumerator AttackRangeExtend()
        {
            attackSkillOn = true;
            circleCollider.radius = 5f;
              yield return new WaitForSeconds(5f);
            circleCollider.radius = 1.6f;
            attackSkillOn = false;
        }

    }

    //쉴드 쿨타임
    void ShieldCoolTime()
    {
        StartCoroutine(CoolTime());
        IEnumerator CoolTime()
        {
            shieldCoolTime = true;
            yield return StartCoroutine(UIController.Instance.ShieldCoolTime());

            shieldCoolTime = false;
        }
    }

    //쉴드 없을 때 충돌
    void NoShieldCollsion()
    {
        if (isJump)//공중에서 맞을 때
        {
            rigid.AddForce(Vector2.down * jumpPower, ForceMode2D.Impulse);
           
        }
        else//땅에서 맞을 때
        {
      
           GameManager.Instance.Die(); 
            Debug.Log("Die");

        }
    }

    //몬스터 밀치기
    void MonsterPushBack()
    {
        MonsterController[] monsterControllers = MonsterPool.Instance.monsterControllers.ToArray();

        int count = monsterControllers.Length;
        for (int i = 0; i < count; i++)
        {
            monsterControllers[i].PushBack();
        }
    
    }

    //부딪히는 경우
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            if (contactCollisionMonsters.Count > 0)
            {
           
                GameManager.Instance.Die();
            }
            animator.SetBool("isJump", false);
            isJump = false;
        }
        else if (collision.gameObject.CompareTag("Monster"))
        {
            contactCollisionMonsters.Add(collision.gameObject);

            if (!shieldCoolTime)
            {
                MonsterPushBack();
                ShieldCoolTime();
            }
            else
            {
                NoShieldCollsion();
            }

        }

    }

    

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            animator.SetBool("isJump", true);
            isJump = true;
        }
        else if (collision.gameObject.CompareTag("Monster"))
        {
            contactCollisionMonsters.Remove(collision.gameObject);

        }

    }


        private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster"))
        {
            if (jumpSkillOn)
            {
                AudioController.Instance.PlayOnce();
                MonsterPool.Instance.Set(other.gameObject, ObjectFlag.Monster);
                GameManager.Instance.DestroyMonster();

                return;
            }

            contactMonsters.Add(other.gameObject);
        }
    }



    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Monster"))
        {
            contactMonsters.Remove(other.gameObject);
        }
    }



}
