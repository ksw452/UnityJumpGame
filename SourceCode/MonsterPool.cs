using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ObjectFlag
{
    Monster,
    MonsterBombEffect,
    AttackSkill
}
public class MonsterPool : MonoBehaviour
{

    //싱글톤
    public static MonsterPool Instance;
    //오브젝트 개수
    public int[] initCount;

    //몬스터 오브젝트 스크립트
    [HideInInspector]
    public List<MonsterController> monsterControllers;


    // 오브젝트 프리팹
    [SerializeField]
    GameObject[] cpyObject;

    //오브젝트 리스트
    public List<Queue<GameObject>> queList = new List<Queue<GameObject>>();


    


    private void init(int count, GameObject gb, int flag)
    {

        for (int i = 0; i < count; i++)
        {

            GameObject tempGB = GameObject.Instantiate(gb, this.transform);
            tempGB.gameObject.SetActive(false);
            queList[flag].Enqueue(tempGB);
        }

    }

    public void Reset()
    {
        int length = monsterControllers.Count;
        for (int i = 0; i < length; i++)
        {

            Set(monsterControllers[0].gameObject,ObjectFlag.Monster);

        }
        monsterControllers.Clear();

        
        
    }


    public GameObject get(ObjectFlag flag, Vector3 pos)
    {
        GameObject tempGB;
        int index = (int)flag;


        if (queList[index].Count > 0)
        {

            tempGB = queList[index].Dequeue();

            tempGB.transform.SetParent(null);
            tempGB.SetActive(true);

        }
        else
        {
            tempGB = GameObject.Instantiate(cpyObject[index], this.transform);
            tempGB.transform.SetParent(null);

        }
        //몬스터일 경우
        if (flag == ObjectFlag.Monster)
        {
            monsterControllers.Add(tempGB.GetComponent<MonsterController>());
        }

        tempGB.transform.position = pos;


        return tempGB;

    }


    public void Set(GameObject gb, ObjectFlag flag)
    {
        int index = (int)flag;
        gb.gameObject.SetActive(false);
        gb.transform.SetParent(this.transform);
        queList[index].Enqueue(gb);

        //몬스터일 경우
        if (flag == ObjectFlag.Monster)
        {
            monsterControllers.Remove(gb.GetComponent<MonsterController>());
            get(ObjectFlag.MonsterBombEffect, gb.transform.position);
        }

    }

    void Awake()
    {
        if(Instance== null)
            Instance = this;

        monsterControllers = new List<MonsterController>();

        for (int i = 0; i < cpyObject.Length; i++)
        {
            queList.Add(new Queue<GameObject>());

            init(initCount[i], cpyObject[i], i);
        }

    }
}
