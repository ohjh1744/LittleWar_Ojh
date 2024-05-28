using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class MagicTowerBase : TowerBase
{
    // 매직 타워 무기 충돌 이펙트
    [Tooltip ("매직타워 무기 이펙트 타입")] public PoolManager.TowerWeaponEffectType towerWeaponEffectType;

    // 풀링한 무기 충돌 이펙트 저장
    [HideInInspector] public List<GameObject> towerWeaponEffectPrefabs = new List<GameObject>();

    // 타겟 공격
    // 매직타워1,4 데미지 높지만 몬스터 제어기능 X
    // 매직타워2,3 데미지 낮지만 몬스터 제어기능 O

    // 코루틴
    // protected override IEnumerator Attack()
    // {
    //     while (isTarget)
    //     {
    //         // 공격속도만큼 대기
    //         yield return new WaitForSeconds(attackSpeed);

    //         // 사운드
    //         SoundManager.Instance.PlaySFX(soundType);

    //         // 매직타워시간만 잠시 대기 후
    //         yield return soundType == SoundType.매직타워시간 || soundType == SoundType.매직타워불 ? new WaitForSeconds(0.75f) : null;

    //         // 애니메이션
    //         for(int i = 0; i < towerAnim.Count; i++) towerAnim[i].SetTrigger("atkTrig");

    //         // 매직 타워 충돌 이펙트 초기화
    //         MagicTowerEffectInit();

    //         // 타워 무기 충돌 이펙트
    //         GameObject towerWeaponEffect = PoolManager.Instance.GetTowerWeaponEffect(towerWeaponEffectType);
    //         towerWeaponEffect.transform.position = target.transform.position + transform.up * 14f;
    //         towerWeaponEffectPrefabs.Add(towerWeaponEffect);

    //         // 발사
    //         Shot();

    //         // 잠시 대기 후
    //         yield return halfSeconds;

    //         // 매직 타워 충돌 이펙트 초기화
    //         MagicTowerEffectInit();
    //     }
    // }

    // 유니태스크
    protected override async UniTaskVoid Attack(CancellationToken tok)
    {
        while (!tok.IsCancellationRequested && isTarget)
        {
            // 공격속도만큼 대기
            await UniTask.Delay(TimeSpan.FromSeconds(attackSpeed), cancellationToken: tok);

            // 사운드
            SoundManager.Instance.PlaySFX(soundType);

            // 매직타워시간만 잠시 대기 후
            await (soundType == SoundType.매직타워시간 || soundType == SoundType.매직타워불 ? UniTask.Delay(TimeSpan.FromSeconds(0.75f), cancellationToken: tok) : UniTask.CompletedTask);

            // 애니메이션
            for(int i = 0; i < towerAnim.Count; i++) towerAnim[i].SetTrigger("atkTrig");

            // 매직 타워 충돌 이펙트 초기화
            MagicTowerEffectInit();

            // 타워 무기 충돌 이펙트
            GameObject towerWeaponEffect = PoolManager.Instance.GetTowerWeaponEffect(towerWeaponEffectType);
            towerWeaponEffect.transform.position = target.transform.position + transform.up * 14f;
            towerWeaponEffectPrefabs.Add(towerWeaponEffect);

            // 발사
            Shot();

            // 잠시 대기 후
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: tok);

            // 매직 타워 충돌 이펙트 초기화
            MagicTowerEffectInit();
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
            towerWeaponRigid.velocity = direction * 15f;

            // 무기 발사 각도
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
            towerWeapon.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // 몬스터 체력 감소
            MonsterInteraction();
        }
    }

    // 몬스터 처리
    protected override void MonsterInteraction()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(target.position, 6f);

        // 스플래쉬 처리
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy")) hit.GetComponent<Enemy>().hp -= hit.gameObject == target.gameObject ? basicDamage : basicDamage / 2;
        }
    }

    // 매직 타워 충돌 이펙트 초기화
    private void MagicTowerEffectInit()
    {
        for(int i = 0; i < towerWeaponEffectPrefabs.Count; i++) towerWeaponEffectPrefabs[i].gameObject.SetActive(false);

        towerWeaponEffectPrefabs.Clear();
    }
}
