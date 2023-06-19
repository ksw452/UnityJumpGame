using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public static float forceValue;
    public Rigidbody2D rigid;
    ConstantForce2D force;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        force = GetComponent<ConstantForce2D>();

    }


    private void OnEnable()
    {
        force.force = new Vector2(0,forceValue);
    }

    public void PushBack()
    {
        rigid.velocity = Vector2.zero;
        rigid.AddForce(Vector2.up*50, ForceMode2D.Impulse);
       


    }
}
