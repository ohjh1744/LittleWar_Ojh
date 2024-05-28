using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : SingletonBehaviour<PoolManager>
{
	#region
	public enum EnemyType
	{
		Enemy1,     // 1�� �������� ���׹�~ 40�� �������� ���׹�
		Enemy2,    Enemy3,     Enemy4,     Enemy5,      
		Enemy6,    Enemy7,     Enemy8,     Enemy9,      
		Enemy10,   Enemy11,    Enemy12,    Enemy13,      
		Enemy14,   Enemy15,    Enemy16,    Enemy17,      
		Enemy18,   Enemy19,    Enemy20,    Enemy21,     
		Enemy22,   Enemy23,    Enemy24,    Enemy25,     
		Enemy26,   Enemy27,    Enemy28,    Enemy29,      
		Enemy30,   Enemy31,    Enemy32,    Enemy33,      
		Enemy34,   Enemy35,    Enemy36,    Enemy37,      
		Enemy38,   Enemy39,    Enemy40,
		MiddleBoss, LastBoss
	}

	[System.Serializable]
	public class EnemyPrefab
	{   //���׹� ������ Ŭ����
		public EnemyType type;
		public GameObject prefab;
	}

	// 타워 무기 타입
	public enum TowerWeaponType
	{
		ArcherTower123, ArcherTower4Lv12, ArcherTower4Lv3,
		MagicTower1, MagicTower2, MagicTower3, MagicTower4,
		StoneTower1, StoneTower2
	}

	// 타워 무기 프리팹
	[System.Serializable]
	public class TowerWeaponPrefab
	{   //���׹� ������ Ŭ����
		public TowerWeaponType type;
		public GameObject prefab;
	}

	// 타워 무기 이펙트 타입
	public enum TowerWeaponEffectType
	{
		MagicTower1, MagicTower2, MagicTower3, MagicTower4
	}

	// 타워 무기 이펙트 프리팹
	[System.Serializable]
	public class TowerWeaponEffectPrefab
	{   //���׹� ������ Ŭ����
		public TowerWeaponEffectType type;
		public GameObject prefab;
	}

	#endregion
	public List<EnemyPrefab> enemyPrefabs; //���׹� ������ ����Ʈ
	private Dictionary<EnemyType, List<GameObject>> enemiesPool; //�� ���׹� ���� ��ųʸ�
	
	// 타워 무기 맵핑
	public List<TowerWeaponPrefab> towerWeaponPrefabs;
	private Dictionary<TowerWeaponType, List<GameObject>> towerWeaponsPool;

	// 타워 무기 이펙트 맵핑
	public List<TowerWeaponEffectPrefab> towerWeaponEffectPrefabs;
	private Dictionary<TowerWeaponEffectType, List<GameObject>> towerWeaponEffectsPool;

	/*�ν����Ϳ��� �Ҵ� -> �ʱ�ȭ
	public GameObject[] EnemyPrefabs; //���׹�������
    public GameObject[] BossPrefabs;  //����������
    public GameObject[] TowerPrefabs; //Ÿ��������
	public GameObject[] EffectPrefabs;  //����Ʈ ������


	private List<GameObject>[] enemyPool; //���׹̴��� ����Ʈ
	private List<GameObject>[] bossPool; //���׹̴��� ����Ʈ
	private List<GameObject>[] towerPool; //���׹̴��� ����Ʈ
	private List<GameObject>[] effectPool; //����Ʈ���� ����Ʈ*/

	protected override void Awake()
    {
        base.Awake();
		
        //Ǯ�� �迭�� ���� ���� �ʱ�ȭ => ���׹�, ����, Ÿ��, ����Ʈ

		// ��ųʸ� �ʱ�ȭ
		enemiesPool = new Dictionary<EnemyType, List<GameObject>>();

		// �� ���׹� �����տ� ���� Ǯ �ʱ�ȭ
		foreach (EnemyPrefab enemyPrefab in enemyPrefabs)
		{
			List<GameObject> pool = new List<GameObject>();
			for (int i = 0; i < 30; i++) // �� ���׹� Ÿ�Ժ��� 30���� �����Ͽ� Ǯ�� �߰�
			{
				GameObject enemyObject = Instantiate(enemyPrefab.prefab);
				enemyObject.SetActive(false); // Ȱ��ȭ���� ���� ���·� ����
				pool.Add(enemyObject);
			}
			enemiesPool.Add(enemyPrefab.type, pool); // ���׹� Ÿ�԰� �ش� Ÿ���� Ǯ�� ��ųʸ��� �߰�

		}

		// 타워 무기 맵핑
		towerWeaponsPool = new Dictionary<TowerWeaponType, List<GameObject>>();

		foreach (TowerWeaponPrefab towerWeaponPrefab in towerWeaponPrefabs)
		{
			List<GameObject> pool = new List<GameObject>();

			for (int i = 0; i < 30; i++)
			{
				GameObject towerWeaponObject = Instantiate(towerWeaponPrefab.prefab);
				towerWeaponObject.SetActive(false);
				pool.Add(towerWeaponObject);
			}

			towerWeaponsPool.Add(towerWeaponPrefab.type, pool);
		}

		// 타워 무기 이펙트 맵핑
		towerWeaponEffectsPool = new Dictionary<TowerWeaponEffectType, List<GameObject>>();

		foreach (TowerWeaponEffectPrefab towerWeaponEffectPrefab in towerWeaponEffectPrefabs)
		{
			List<GameObject> pool = new List<GameObject>();

			for (int i = 0; i < 30; i++)
			{
				GameObject towerWeaponEffectObject = Instantiate(towerWeaponEffectPrefab.prefab);
				towerWeaponEffectObject.SetActive(false);
				pool.Add(towerWeaponEffectObject);
			}

			towerWeaponEffectsPool.Add(towerWeaponEffectPrefab.type, pool);
		}
	
		/*bossPool = new List<GameObject>[EnemyPrefabs.Length];  //4��
		towerPool = new List<GameObject>[EnemyPrefabs.Length];  //8�� �������� 3�� =>24��
		effectPool=new List<GameObject>[EffectPrefabs.Length];  //8��*/
	}

	public GameObject GetEnemy(EnemyType type)
	{
		GameObject selectedEnemy = null;
		List<GameObject> enemyList;
		if (enemiesPool.TryGetValue(type, out enemyList))
		{
			foreach (GameObject enemy in enemyList)
			{
				if (!enemy.activeSelf)
				{
					selectedEnemy = enemy;
					selectedEnemy.SetActive(true);
					break;
				}
			}
		}
		else
		{
			enemyList = new List<GameObject>();
			enemiesPool.Add(type, enemyList);
		}

		if (selectedEnemy == null)
		{
			// Ǯ���� ��� ������ ���� ������ ���� �����Ͽ� �߰�
			selectedEnemy = Instantiate(GetEnemyPrefab(type), transform);
			selectedEnemy.transform.parent = this.transform;
			enemyList.Add(selectedEnemy);
		}

		return selectedEnemy;
	}

	// 타워 무기 가져오기
	public GameObject GetTowerWeapon(TowerWeaponType type)
	{
		GameObject selectedTowerWeapon = null;
		List<GameObject> towerWeaponList;
		if (towerWeaponsPool.TryGetValue(type, out towerWeaponList))
		{
			foreach (GameObject towerWeapon in towerWeaponList)
			{
				if (!towerWeapon.activeSelf)
				{
					selectedTowerWeapon = towerWeapon;
					selectedTowerWeapon.SetActive(true);
					break;
				}
			}
		}
		else
		{
			towerWeaponList = new List<GameObject>();
			towerWeaponsPool.Add(type, towerWeaponList);
		}

		if (selectedTowerWeapon == null)
		{
			selectedTowerWeapon = Instantiate(GetTowerWeapon(type), transform);
			selectedTowerWeapon.transform.parent = this.transform;
			towerWeaponList.Add(selectedTowerWeapon);
		}

		return selectedTowerWeapon;
	}

	// 타워 무기 이펙트 가져오기
	public GameObject GetTowerWeaponEffect(TowerWeaponEffectType type)
	{
		GameObject selectedTowerWeaponEffect = null;
		List<GameObject> towerWeaponEffectList;
		if (towerWeaponEffectsPool.TryGetValue(type, out towerWeaponEffectList))
		{
			foreach (GameObject towerWeaponEffect in towerWeaponEffectList)
			{
				if (!towerWeaponEffect.activeSelf)
				{
					selectedTowerWeaponEffect = towerWeaponEffect;
					selectedTowerWeaponEffect.SetActive(true);
					break;
				}
			}
		}
		else
		{
			towerWeaponEffectList = new List<GameObject>();
			towerWeaponEffectsPool.Add(type, towerWeaponEffectList);
		}

		if (selectedTowerWeaponEffect == null)
		{
			selectedTowerWeaponEffect = Instantiate(GetTowerWeaponEffect(type), transform);
			selectedTowerWeaponEffect.transform.parent = this.transform;
			towerWeaponEffectList.Add(selectedTowerWeaponEffect);
		}

		return selectedTowerWeaponEffect;
	}

	private GameObject GetEnemyPrefab(EnemyType type)
	{
		foreach (EnemyPrefab enemyPrefab in enemyPrefabs)
		{
			if (enemyPrefab.type == type)
			{
				return enemyPrefab.prefab;
			}
		}
		return null;
	}
}
