using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using Cysharp.Threading.Tasks;
using System.Threading;

public class Enemy : MonoBehaviour
{

    [Header("Enemy Stats")]
    public int hp = 100;  //체력
    public int power;     // 데미지
    public int bossPower;  // 보스스킬데미지
    public float moveSpeed; // 이동속도


    private Rigidbody2D rigid;
    private Animator anim;
    private GameObject hitTarget; //몬스터 공격타겟
    private MovePoints movePoints; // 이동포인트위치 저장
    private GameObject target; //Enemy 이동타겟
    private int nextPosition = 0; // 다음 이동포인트를 가르키는 변수
    private bool isAttack = false; // 공격하는지 체크
    private int soundNum = 0; // enemy sound 변수 
    private CancellationTokenSource tokenDead; // 유니태스크 죽음함수 취소 토큰
    private CancellationTokenSource tokenAttack; // 유니태스크 공격함수 취소 토큰
 

    //다른 스크립트에서 참조하는 변수들
    [HideInInspector]public int maxHp; // 최대체력
    [HideInInspector]public int dotDamage; // 몬스터한테 입힐 도트 데미지
    [HideInInspector]public int enemyGold; // 골드량
    [HideInInspector]public float originSpeed = 0.1f; // 몬스터 원래 속도
    [HideInInspector]public int movePointNum;  // 1~4가지 동선 결정.
    [HideInInspector]public bool isDot = false; // 도트딜을 입는 상태인지 체크
    [HideInInspector]public bool isDead = false; // 죽었는지 체크
    [HideInInspector]public int bossAttackNum;  //보스 공격횟수 체크
    [HideInInspector]public CancellationTokenSource tokenDotDamage; // 유니태스크 도트데미지함수 취소 토큰


    void Awake()
    {
        dotDamage = hp / 100;
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        movePoints = GameObject.Find("MovePoints").gameObject.GetComponent<MovePoints>();
        EnemyCheckForSound();
    }
    void Start()
    {
        tokenDead = new ();
        tokenAttack = new();
        //  StartCoroutine(DotDamaged()); // 도트 데미지
        Uni_DotDamaged();
    }

    void Update()
    {
        CancelForUni();
        if (!isAttack &&  !isDead)
        {
            EnemyMove();
        }

        if(hp <= 0)
        {
            isDead = true;
            //StartCoroutine("EnemyDead");
            Uni_EnemyDead();
        }
    }

    void FixedUpdate()
    {
        if (!isDead)
        {
            Scan();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {

        ChangeMovepTarget(collision);

    }
    void OnEnable()
    {
        tokenDead = new();
        tokenAttack = new();
        tokenDotDamage = new();
    }


    //공격타겟 스캔
    void Scan()
    {
        Vector2 v2 = rigid.velocity.normalized;
        //Debug.DrawRay(rigid.position, Vector2.right * 1, new Color(0, 1, 0));
        RaycastHit2D rayHit_obstacle = Physics2D.Raycast(rigid.position, Vector2.right, 1f, LayerMask.GetMask("Hit"));

        try
        {
            if (rayHit_obstacle.collider != null)
            {

                hitTarget = rayHit_obstacle.transform.gameObject;
                if (!isAttack)
                {
                    //StartCoroutine("EnemyAttack", hitTarget);
                    Uni_EnemyAttack(hitTarget);
                }
            }
            else if (rayHit_obstacle.collider == null)
            {
                hitTarget = null;
                RemoveObstacle(hitTarget);
            }
        }
        catch
        {

        }


    }
    //몬스터 이동
    void EnemyMove()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, moveSpeed);
    }

    // 몬스터 spawn시 첫이동타겟설정
    public void FirstMoveTarget()
    {
        switch (movePointNum)
        {
            case 1:
                target = GameObject.Find("MovePoints").transform.Find("move_point6").gameObject;
                break;
            case 2:
                target = GameObject.Find("MovePoints").transform.Find("move_point7").gameObject;
                break;
            case 3:
                target = GameObject.Find("MovePoints").transform.Find("move_point3").gameObject;
                break;
            case 4:
                target = GameObject.Find("MovePoints").transform.Find("move_point1").gameObject;
                break;
        }
    }

    // 경로마다 이동타겟 변경
    void ChangeMovepTarget(Collider2D collision)
    {
        try
        {
            if (collision.gameObject.tag == "MovePoints")
            {
                if (movePointNum == 1 && collision.gameObject == movePoints.movepoints_1[nextPosition])
                {
                    target = movePoints.movepoints_1[nextPosition + 1];
                    nextPosition++;
                }
                else if (movePointNum == 2 && collision.gameObject == movePoints.movepoints_2[nextPosition])
                {
                    target = movePoints.movepoints_2[nextPosition + 1];
                    nextPosition++;
                }
                else if (movePointNum == 3 && collision.gameObject == movePoints.movepoints_3[nextPosition])
                {
                    target = movePoints.movepoints_3[nextPosition + 1];
                    nextPosition++;
                }
                else if (movePointNum == 4 && collision.gameObject == movePoints.movepoints_4[nextPosition])
                {
                    target = movePoints.movepoints_4[nextPosition + 1];
                    nextPosition++;
                }
            }
        }
        catch
        {
            
        }
    }

    // 공격 후 타겟 제거
    void RemoveObstacle(GameObject hit_object)
    {
        anim.SetBool("isAttack", false);
        isAttack = false;
        if(hit_object != null)
        {
            hit_object.gameObject.SetActive(false);
        }
    }

    //유니태스크용 cancel토큰 초기화
    void CancelForUni()
    {
        if (isDead)
        {
            tokenAttack.Cancel();
        }

    }
 

    //Enmey , Boss별 sound_num 할당
    private void EnemyCheckForSound()
    {
        for (int i = 1; i <= 10; i++)
        {
            if (gameObject.name == "Enemy" + i + "(Clone)")
            {
                soundNum = i;
            }
        }

        if (soundNum == 0)
        {
            for (int i = 1; i <= 2; i++)
            {
                if (gameObject.name == "Boss" + i + "(Clone)")
                    soundNum = i + 10;
            }
        }
    }
    //Enmey 공격, 죽을 시 소리
    private void EnemySound()
    {
        if (isDead)
        {
            switch (soundNum)
            {
                case 1:
                    SoundManager.Instance.PlaySFX(SoundType.Enemy1_Death);
                    break;
                case 2:
                    SoundManager.Instance.PlaySFX(SoundType.Enemy2_Death);
                    break;
                case 3:
                    SoundManager.Instance.PlaySFX(SoundType.Enemy3_Death);
                    break;
                case 4:
                    SoundManager.Instance.PlaySFX(SoundType.Enemy4_Death);
                    break;
                case 5:
                    SoundManager.Instance.PlaySFX(SoundType.Enemy5_Death);
                    break;
                case 6:
                    SoundManager.Instance.PlaySFX(SoundType.Enemy6_Death);
                    break;
                case 7:
                    SoundManager.Instance.PlaySFX(SoundType.Enemy7_Death);
                    break;
                case 8:
                    SoundManager.Instance.PlaySFX(SoundType.Enemy8_Death);
                    break;
                case 9:
                    SoundManager.Instance.PlaySFX(SoundType.Enemy9_Death);
                    break;
                case 10:
                    SoundManager.Instance.PlaySFX(SoundType.Enemy10_Death);
                    break;
                case 11:
                    SoundManager.Instance.PlaySFX(SoundType.보스1_Death);
                    break;
                case 12:
                    SoundManager.Instance.PlaySFX(SoundType.보스2_Death);
                    break;
            }
        }
        else if (isAttack)
        {
            if (bossAttackNum == 5)
            {
                switch (soundNum)
                {
                    case 11: SoundManager.Instance.PlaySFX(SoundType.보스1_Skill); break;
                    case 12: SoundManager.Instance.PlaySFX(SoundType.보스2_Skill); break;
                }
            }
            else
            {
                SoundManager.Instance.PlaySFX(SoundType.Enemy_Hit);
            }
        }


    }
    //보스 스테이지 UI용 확인
    private void BossCheck()
    {
        if (enemyGold == 2000 || enemyGold == 10000)
        {
            GameManager.Instance.bossHpPanel.SetActive(false);
            GameManager.Instance.isBossStage = false;
        }

        // 마지막 보스 킬 시 게임 승리
        if (enemyGold == 10000)
        {
            GameManager.Instance._isGameOver = true;
            GameManager.Instance.PauseGameBtn();
            GameManager.Instance.gameWinPanel.SetActive(true);
            GameManager.Instance.DisableClick(true);
        }
    }

    //체력0되면 Enemy는 알아서 fasle 되면서 풀로 들어감.
    IEnumerator EnemyDead()
    {
        if (isDead)
        {

            anim.SetBool("isAttack", false);
            anim.SetBool("isSkill1", false);
            anim.SetBool("isDead", true);
            rigid.velocity = Vector2.zero;

            yield return new WaitForSeconds(0.3f);

            EnemySound();
            anim.SetBool("isDead", false);
            gameObject.SetActive(false);
            isDead = false;
            isDot = false; // 죽으면 도트딜 없는 상태로
            hp = maxHp;
            bossAttackNum = 0;
            nextPosition = 0;
            moveSpeed = originSpeed; // 다시 원래 속도로
            GoldManager.Instance.AcquireGold(enemyGold); // 골드 증가

            BossCheck();
        }
    }
    private async UniTask Uni_EnemyDead()
    {
        if (isDead && !tokenDead.IsCancellationRequested)
        {

            anim.SetBool("isAttack", false);
            anim.SetBool("isSkill1", false);
            anim.SetBool("isDead", true);
            rigid.velocity = Vector2.zero;

      
            await UniTask.Delay(TimeSpan.FromSeconds(0.3f), cancellationToken: tokenDead.Token);
            tokenDead.Cancel();

            EnemySound();
            anim.SetBool("isDead", false);
            gameObject.SetActive(false);
            isDead = false;
            isDot = false; // 죽으면 도트딜 없는 상태로
            hp = maxHp;
            bossAttackNum = 0;
            nextPosition = 0;
            moveSpeed = originSpeed; // 다시 원래 속도로
            GoldManager.Instance.AcquireGold(enemyGold); // 골드 증가
            tokenDead = new CancellationTokenSource();
            BossCheck();

            
        }
    }

    //Enemy 공격 및 보스 스킬공격
    IEnumerator EnemyAttack(GameObject hitObject)
    {
        anim.SetBool("isAttack", true);
        isAttack = true;
        rigid.velocity = Vector2.zero;



        if (hitObject.gameObject.tag == "Stone")
        {
            while (!isDead) // ������ ���߿� �״� ��쵵 ������ �ֱ⿡ ����Ȯ��.
            {
                bossAttackNum++;
                if (bossAttackNum == 5 && gameObject.layer == 10) //layer 10 Boss
                {
                    if (gameObject.GetComponent<Boss>().isSkill == false)
                    {
                        gameObject.GetComponent<Boss>().isSkill = true;
                        gameObject.GetComponent<Boss>().BossSkill1(bossPower, hitObject);
                        EnemySound();
                    }
                    bossAttackNum = 0;
                    continue;
                }
                EnemySound();

                hitObject.GetComponent<Stone>().stoneHP -= power;
                if (hitObject.GetComponent<Stone>().stoneHP <= 0)
                {
                    RemoveObstacle(hitObject);
                    break;
                }
                yield return new WaitForSeconds(0.5f);

            }
        }

        


        else if (hitObject.gameObject.tag == "Nexus")
        {
            while (!GameManager.Instance._isGameOver && !isDead)  //때리는 도중에 죽을수 있기 때문에 isdead변수 추가.
            {
                bossAttackNum++;
                if (bossAttackNum == 5 && gameObject.layer == 10) //layer 10 Boss
                {
                    if (gameObject.GetComponent<Boss>().isSkill == false)
                    {
                        gameObject.GetComponent<Boss>().isSkill = true;
                        gameObject.GetComponent<Boss>().BossSkill1(0, hitObject); // 0으로 해서 대미지 x 애니메니션 실행만
                        EnemySound();
                        GameManager.Instance.NexusDamaged(bossPower);
                    }
                    bossAttackNum = 0;
                    continue;
                }
                EnemySound();
                GameManager.Instance.NexusDamaged(power);
                yield return new WaitForSeconds(0.5f);
            }
        }

    }

    private async UniTask Uni_EnemyAttack(GameObject hitObject)
    {
        anim.SetBool("isAttack", true);
        isAttack = true;
        rigid.velocity = Vector2.zero;

        if (hitObject.gameObject.tag == "Stone")
        {
            while (!isDead && !tokenAttack.IsCancellationRequested) // ������ ���߿� �״� ��쵵 ������ �ֱ⿡ ����Ȯ��.
            {
                bossAttackNum++;
                if (bossAttackNum == 5 && gameObject.layer == 10) //layer 10 Boss
                {
                    if (gameObject.GetComponent<Boss>().isSkill == false)
                    {
                        gameObject.GetComponent<Boss>().isSkill = true;
                        gameObject.GetComponent<Boss>().BossSkill1(bossPower, hitObject);
                        EnemySound();
                    }
                    bossAttackNum = 0;
                    continue;
                }
                EnemySound();

                hitObject.GetComponent<Stone>().stoneHP -= power;
                if (hitObject.GetComponent<Stone>().stoneHP <= 0)
                {
                    RemoveObstacle(hitObject);
                    break;
                }
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f) , cancellationToken: tokenAttack.Token);

            }
        }




        else if (hitObject.gameObject.tag == "Nexus")
        {
            while (!GameManager.Instance._isGameOver && !isDead && !tokenAttack.IsCancellationRequested)  //때리는 도중에 죽을수 있기 때문에 isdead변수 추가.
            {
                bossAttackNum++;
                if (bossAttackNum == 5 && gameObject.layer == 10) //layer 10 Boss
                {
                    if (gameObject.GetComponent<Boss>().isSkill == false)
                    {
                        gameObject.GetComponent<Boss>().isSkill = true;
                        gameObject.GetComponent<Boss>().BossSkill1(0, hitObject); // 0으로 해서 대미지 x 애니메니션 실행만
                        EnemySound();
                        GameManager.Instance.NexusDamaged(bossPower);
                    }
                    bossAttackNum = 0;
                    continue;
                }
                EnemySound();
                GameManager.Instance.NexusDamaged(power);
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: tokenAttack.Token);
            }
        }
    }

    // 도트데미지 : 체력 1%씩 감소
    IEnumerator DotDamaged()
    {
        while (true)
        {
            if(isDot)
            {
                hp -= dotDamage;
                yield return new WaitForSeconds(0.1f);

                continue;
            }

            yield return null;
        }
    }

    private async UniTask Uni_DotDamaged()
    {
        while (true)
        {
            if (isDot && !tokenDotDamage.IsCancellationRequested)
            {
                hp -= dotDamage;
                await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: tokenDotDamage.Token);

                continue;
            }

            await UniTask.Yield();
        }
    }
    
}
