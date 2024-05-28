using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class ArcherTowerBase : TowerBase
{
    // 타겟 공격
    // 아쳐타워1,2,3 단일공격
    // 아쳐타워1,2 연사공격 X
    // 아쳐타워3 연사공격 O
    // 아쳐타워4 연사공격 X 광역공격 O

    // 코루틴
    // protected override IEnumerator Attack()
    // {
    //     while (isTarget)
    //     {
    //         // 공격속도만큼 대기
    //         yield return new WaitForSeconds(attackSpeed);

    //         // 발사
    //         Shot();

    //         // 사운드
    //         SoundManager.Instance.PlaySFX(soundType);
    //     }
    // }

    // 유니태스크
    protected override async UniTaskVoid Attack(CancellationToken tok)
    {
        while (!tok.IsCancellationRequested && isTarget)
        {
            // 공격속도만큼 대기
            await UniTask.Delay(TimeSpan.FromSeconds(attackSpeed), cancellationToken: tok);

            // 발사
            Shot();

            // 사운드
            SoundManager.Instance.PlaySFX(soundType);
        }
    }

    // 무기 발사
    protected override void Shot()
    {
        // 타워 무기 발사위치 개수만큼
        for(int i = 0; i < atkPos.Count; i++)
        {
            // 타워 무기 가져오기
            GameObject towerWeapon = PoolManager.Instance.GetTowerWeapon(towerWeaponType);
            Rigidbody2D towerWeaponRigid = towerWeapon.GetComponent<Rigidbody2D>();

            // 위치 및 회전 초기화
            towerWeapon.transform.position = atkPos[i].transform.position;
            towerWeapon.transform.rotation = towerWeapon.transform.rotation;

            // 타워 무기 발사
            Vector2 direction = (target.position - towerWeapon.transform.position).normalized;
            towerWeaponRigid.velocity = direction * 45f;

            // 무기 발사 각도
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
            if(towerWeapon.CompareTag("ArcherWeapon123")) angle -= 45;
            towerWeapon.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // 몬스터 체력 감소
            MonsterInteraction();

            // 타워 애니메이션
            if(towerWeapon.CompareTag("ArcherWeapon123")) towerAnim[i].SetTrigger("atkTrig");
        }
    }

    // 몬스터 처리
    protected override void MonsterInteraction()
    {
        // 단일 처리
        target.GetComponent<Enemy>().hp -= basicDamage;
        Debug.Log(basicDamage);
    }
}
