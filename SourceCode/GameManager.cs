using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int round = 1;
    public int monsterCount = 0;
    public float time = 0;
    public float jumpCount = 0;
    public float attackCount = 0;
    bool isStart;
    [SerializeField]
    Transform monsterPosTr;
    Vector2 monsterPos;


    private void Awake()
    {
        Application.targetFrameRate = 60;
        if (Instance == null)
        {
            Instance = this;
        }

        monsterPos = monsterPosTr.position;
    }


    public void Die()
    {

        isStart = false;
        Time.timeScale = 0f;
        UIController.Instance.OnRestartPanel();
       
    }

    //점수 세기
    IEnumerator CountTime()
    {
        isStart = true;
        while (isStart)
        {
            time += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        
        
        }
        
    }

    //초기화
    public void InitValue()
    {
        StopAllCoroutines();
        round = 1;
        monsterCount = 0;
        time = 0;
        jumpCount = 0;
        attackCount = 0;
        MonsterPool.Instance.Reset();
    }

    public void CreateMonster()
    {
        if (!isStart)
        {

            StartCoroutine(CountTime());
        
        }

        StartCoroutine(Create());
        IEnumerator Create()
        {
            int length = round * 10;
            monsterCount = length;

            MonsterController.forceValue = 0.005f * round;
            for (int i = 0; i < length; i++)
            {
                MonsterPool.Instance.get(ObjectFlag.Monster, monsterPos);
                yield return new WaitForSeconds(1f/round);
            }

            round++;
        }
    }
    public void DestroyMonster()
    {

        monsterCount--;
        if (monsterCount <= 0)
        {
            UIController.Instance.OnRoundPanel();
        
        }

    }

    public void JumpCount()
    {
        jumpCount += 0.2f;

        UIController.Instance.jumpSkillImage.fillAmount = jumpCount;

        if (jumpCount >= 1f)
        {
            jumpCount = 0f;
            UIController.Instance.jumpSlider.gameObject.SetActive(true);
        }

    }

    public void AttackCount()
    {
        attackCount += 0.1f;

        UIController.Instance.attackSkillImage.fillAmount = attackCount;
        if (attackCount >= 1f)
        {
            attackCount = 0f;
            UIController.Instance.attackSlider.gameObject.SetActive(true);
        }

    }




}
