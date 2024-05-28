using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    Animator anim;

    //스킬 쓰는지 확인
    public bool isSkill;
    void Start()
    {
        anim = GetComponent<Animator>();
    }
    
    public void BossSkill1(int power , GameObject hitObject)
    {
        
        anim.SetBool("isSkill1", true);
        if (hitObject.gameObject.tag == "Stone")
        {
            hitObject.GetComponent<Stone>().stoneHP -= power;
        }

        Invoke("StopSkill", 0.5f);
    }

    private void StopSkill()
    {
        anim.SetBool("isSkill1", false);
        isSkill = false;
    }

}
