using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class Fire : MonoBehaviour
{
    [HideInInspector] public int fireDamage; // 불 데미지

    // 스톤타워 1 불
    // 도트딜 부여

    // 코루틴
    // private void OnTriggerStay2D(Collider2D other)
    // {
    //     if (other.CompareTag("Enemy"))
    //     {
    //         Enemy enemy = other.GetComponent<Enemy>();
    //         if(!enemy.isDot) StartCoroutine(Dot(enemy));
    //     }
    // }

    // 유니태스크
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if(!enemy.isDot) Dot(enemy).Forget();
        }
    }

    // 코루틴
    // private IEnumerator Dot(Enemy enemy)
    // {
    //     enemy.dotDamage = enemy.maxHp / 100 * fireDamage; // 도트딜

    //     enemy.isDot = true; // 적용

    //     yield return new WaitForSeconds(5f); // 지속시간

    //     enemy.isDot = false; // 해제
    // }

    // 유니태스크
    private async UniTaskVoid Dot(Enemy enemy)
    {
        enemy.dotDamage = enemy.maxHp / 100 * fireDamage; // 도트딜

        enemy.isDot = true; // 적용

        await UniTask.Delay(TimeSpan.FromSeconds(5f)); // 지속시간

        enemy.isDot = false; // 해제
        enemy.tokenDotDamage.Cancel(); // 유니태스크용 dotdamage 함수 취소.
    }
}
