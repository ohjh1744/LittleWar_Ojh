using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerWeapon : MonoBehaviour
{
    // 타워 무기 비활성화
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Enemy") || other.CompareTag("Border")) gameObject.SetActive(false);
    }
}
