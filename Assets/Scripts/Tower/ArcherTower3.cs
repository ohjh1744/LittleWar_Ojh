using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class ArcherTower3 : ArcherTowerBase
{
    // 스탯 조정
    private void Awake()
    {
        InitTower(70, 0.5f, 500);
    }

    // 아쳐타워3
    // 50% 확률로 연사공격

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

    //         // 연사
    //         if (Random.value < 0.5f)
    //         {
    //             yield return new WaitForSeconds(attackSpeed * 0.5f);
    //             Shot();
    //             SoundManager.Instance.PlaySFX(soundType);
    //         }
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

            // 연사
            if (UnityEngine.Random.value < 0.5f)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(attackSpeed * 0.5f), cancellationToken: tok);
                Shot();
                SoundManager.Instance.PlaySFX(soundType);
            }
        }
    }
}
