using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public abstract class TowerBase : MonoBehaviour
{
    // 타워 스탯 관련
    [HideInInspector] public int basicDamage = 10; // 타워 기본 데미지
    protected float attackSpeed = 1.0f; // 타워 기본 공격속도

    // 타워 설치 및 업글 관련
    [Header ("타워 설치")] [Space (10f)]
    [Tooltip ("타워 설치 비용")] public int towerBuildPrice; // 타워 설치비용
    [HideInInspector] public int towerLv = 1; // 타워 레벨
    [HideInInspector] public int towerUpgradeBasicPrice = 100; // 타워 업그레이드 기본비용

    // 타워 타겟 관련 
    protected Transform target; // 타겟 몬스터
    protected bool isTarget = false; // 타겟이 설정되었는지 체크
    private CancellationTokenSource token; // 현재 공격중인 유니태스크 토큰

    // 타워 공격 관련
    protected Coroutine attackCoroutine; // 현재 실행 중인 공격 코루틴
    [Header ("타워 공격")] [Space (10f)] [SerializeField] [Tooltip ("타워 무기 발사위치")] protected List<GameObject> atkPos; // 타워 무기 발사위치
    [SerializeField] [Tooltip ("타워 무기 애니메이션")] protected List<Animator> towerAnim; // 타워 애니메이션
    protected WaitForSeconds halfSeconds = new WaitForSeconds(0.5f); // 대기시간

    // 타워 타입 관련
    [Header ("타워 타입")] [Space (10f)] [Tooltip ("타워 타입")] public TowerType towerType; // 타워 타입
    [Tooltip ("타워 무기 타입")] public PoolManager.TowerWeaponType towerWeaponType; // 타워 무기 타입
    [SerializeField] [Tooltip ("타워 사운드 타입")] protected SoundType soundType; // 타워 사운드 타입

    // 타워 스탯 초기화
    protected void InitTower(int dmg = 10, float speed = 1.0f, int upgradePrice = 100)
    {
        basicDamage = dmg;
        attackSpeed = speed;
        towerUpgradeBasicPrice = upgradePrice;
    }

    // 타겟 설정
    // 타겟이 범위를 나갔거나 죽으면 재타겟팅
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && (!isTarget || target.GetComponent<Enemy>().isDead)) TargetEnemy(other.transform);
    }

    // 타겟 나감
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform == target) isTarget = false; // 타겟이 설정되지 않은 상태
    }

    // 타겟 설정

    // 코루틴
    // private void TargetEnemy(Transform enemy)
    // {
    //     target = enemy; // 타겟 설정
    //     isTarget = true; // 타겟이 설정된 상태
    //     if (attackCoroutine != null) StopCoroutine(attackCoroutine); // 이전 공격 코루틴 중지
    //     attackCoroutine = StartCoroutine(Attack()); // 새로운 공격 코루틴 시작
    // }

    // 유니태스크
    private void TargetEnemy(Transform enemy)
    {
        target = enemy; // 타겟 설정
        isTarget = true; // 타겟이 설정된 상태
        if (token != null && !token.Token.IsCancellationRequested) token.Cancel(); // 이전 공격 유니태스크 중지
        token = new CancellationTokenSource(); // 토큰 생성
        Attack(token.Token).Forget(); // 새로운 공격 유니태스크 시작
    }

    // 타겟 공격

    // 코루틴
    //protected abstract IEnumerator Attack();

    // 유니태스크
    protected virtual async UniTaskVoid Attack(CancellationToken tok) {}

    // 무기 발사
    protected abstract void Shot();

    // 몬스터 처리
    protected virtual void MonsterInteraction() {}
}
