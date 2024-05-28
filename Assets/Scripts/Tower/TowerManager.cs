using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 타워 타입 -> 키로 사용
public enum TowerType
{
    ArcherTower1, ArcherTower2, ArcherTower3, ArcherTower4,
    MagicTower1, MagicTower2, MagicTower3, MagicTower4,
    StoneTower1, StoneTower2
}

// 각 타워 타입에 해당하는 타워 Lv1 ~ Lv3 리스트
[System.Serializable]
public class TowerLvList
{
    public List<GameObject> towerPrefabs;
}

// 타워 설치, 해제, 업그레이드 관리
public class TowerManager : MonoBehaviour
{
    // 타워 설치 관련
    [Header ("타워 설치")] [Space (10f)]
    [Tooltip ("각 타워를 설치할 위치")] public GameObject towerBuildPos; // 각 타워를 설치할 위치
    [Tooltip ("타워 설치 패널")] public GameObject towerBuildPanel; // 타워 설치 패널
    [Tooltip ("각 타워를 설치할 위치만 클릭되게")] public LayerMask towerBuildPosLayerMask; // 타워 설치 위치만 클릭

    // 타워 업글 관련
    [Header ("타워 업글")] [Space (10f)]
    [Tooltip ("타워 업글 패널")] public GameObject towerUpgradePanel; // 타워 업글 패널
    [Tooltip ("타워 업글 패널 타워타입 이미지")] public Image upgradePanelTowerImage; // 업그레이드 패널 타워 이미지
    [Tooltip ("타워 업글 패널 타워레벨 텍스트")] public TMP_Text upgradePanelTowerLvText; // 업그레이드 패널 타워 레벨 텍스트
    [Tooltip ("타워 업글 패널 업글비용 텍스트")] public TMP_Text upgradePanelTowerPriceText; // 업그레이드 패널 타워 업그레이드 비용 텍스트
    [Tooltip ("타워 업글 패널 업글스타 이미지")] public Image upgradePanelTowerStarImage; // 업그레이드 패널 타워 업글스타 이미지

    // 타워 공유 관련
    private static Transform selectedTowerBuildPos; // 선택된 타워를 설치할 위치
    private static GameObject selectedTowerPref; // 선택된 설치할 타워 프리팹
    private static bool isPanel = false; // 패널이 활성화중인지 체크

    // 타워 맵핑 관련
    [Header ("타워 맵핑")] [Space (10f)]
    [Tooltip ("각 타워 타입의 Lv1 Lv2 Lv3 타워 프리팹들")] public List<TowerLvList> towerPrefs = new List<TowerLvList>(); // 타워 프리팹들 -> 인스펙터에서 할당
    [Tooltip ("각 타워 타입에 해당하는 스프라이트들")] public List<Sprite> towerSpritePrefs = new List<Sprite>(); // 업그레이드 패널 타워 이미지 바꿀 스프라이트들 -> 인스펙터에서 할당
    [Tooltip ("타워 레벨에 따른 업글스타 스프라이트들")] public List<Sprite> towerStarPrefs = new List<Sprite>(); // 업그레이드 패널 스타 이미지 바꿀 스프라이트들 -> 인스펙터에서 할당

    private Dictionary<TowerType, TowerLvList> towers = new Dictionary<TowerType, TowerLvList>(); // (타워타입, 타입에 해당하는 타워 Lv1 ~ Lv3) 맵핑
    private Dictionary<TowerType, Sprite> towerSprites = new Dictionary<TowerType, Sprite>(); // (타워타입, 타입에 해당하는 타워 스프라이트) 맵핑
    private Dictionary<int, Sprite> towerStarSprites = new Dictionary<int, Sprite>(); // (타워레벨, 레벨에 해당하는 타워스타 스프라이트) 맵핑

    // 타워 UI 버튼 사운드
    public GameObject buttonSound;

    // 타워 맵핑
    private void Awake()
    {
        for(int i = 0; i < towerPrefs.Count; i++)
        {
            towers[(TowerType)i] = towerPrefs[i];
            towerSprites[(TowerType)i] = towerSpritePrefs[i];
        }

        for(int i = 0; i < 3; i++)
        {
            towerStarSprites[i + 1] = towerStarPrefs[i];
        }
    }

    // 설치 패널 및 업글 패널 활성화
    private void Update()
    {
        // 패널이 활성화중인지 체크
        if(isPanel) return;

        // 설치 패널 및 업글 패널 활성화
        EnterPanel();
    }

    // 설치 패널 및 업글 패널 띄우기
    private void EnterPanel()
    {
        // 마우스 왼쪽 클릭 또는 모바일 터치하면
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            // 게임 승리/패배/옵션 상태인지 체크
            if(GameManager.Instance.disableClick) return;

            // 마우스 클릭 또는 모바일 터치 위치 가져옴
            Vector3 inputPosition = Input.GetMouseButtonDown(0) ? Camera.main.ScreenToWorldPoint(Input.mousePosition) : Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            inputPosition.z = 0;

            // 히트된 설치할 위치 콜라이더 가져와서 선택된 타워 위치로 설정
            RaycastHit2D hit = Physics2D.Raycast(inputPosition, Vector2.zero, Mathf.Infinity, towerBuildPosLayerMask);

            // 히트된 콜라이더가 있는지 체크
            if(hit.collider == null) return;

            // 타워매니저 
            TowerManager towerManager = hit.collider.gameObject.GetComponent<TowerManager>();

            // 타워매니저가 있는지 체크 
            if(towerManager == null) return;

            // 히트된 콜라이더의 게임오브젝트가 클릭한 영역의 설치할 위치인지 체크
            if(!hit.collider.gameObject.Equals(towerManager.towerBuildPos)) return;

            // 선택된 타워 설치 위치 저장
            selectedTowerBuildPos = towerManager.towerBuildPos.transform;

            // 패널이 활성화된 상태
            isPanel = true;

            // 사운드
            OnBtnClickSound();

            // 설치된 상태가 아니면 설치 패널 활성화
            if(selectedTowerBuildPos.transform.childCount == 0) 
            {
                towerBuildPanel.SetActive(true);

                return;
            }

            // 설치된 상태면 업그레이드 패널 활성화
            towerUpgradePanel.SetActive(true);

            // 선택된 타워 타입으로 업그레이드 패널 갱신
            TowerBase selectedTowerBase = selectedTowerBuildPos.GetChild(0).GetComponent<TowerBase>();
            upgradePanelTowerImage.sprite = towerSprites[selectedTowerBase.towerType];
            upgradePanelTowerLvText.text = "타워 레벨 : " + selectedTowerBase.towerLv.ToString();
            upgradePanelTowerPriceText.text = selectedTowerBase.towerLv == 3 ? "" : selectedTowerBase.towerUpgradeBasicPrice.ToString();
            upgradePanelTowerStarImage.sprite = towerStarSprites[selectedTowerBase.towerLv];
        }
    }

    // 설치 패널 및 업글 패널 Exit 버튼
    public void ExitPanel(string panelName)
    {
        // 설치 패널 및 업글 패널 닫기
        if(panelName.Equals("build")) towerBuildPanel.SetActive(false);
        else towerUpgradePanel.SetActive(false);

        // 패널이 비활성화된 상태
        isPanel = false;
    }

    // 설치할 타워 선택
    public void SelectTower(int towerType)
    {
        // 설치할 타워 프리팹 선택
        selectedTowerPref = towers[(TowerType)towerType].towerPrefabs[0]; 
    }

    // 타워 설치
    public void BuildTower()
    {
        // 골드 체크
        if(!GoldManager.Instance.UseGold(selectedTowerPref.GetComponent<TowerBase>().towerBuildPrice)) return;

        // 현재 위치에 타워 생성
        GameObject buildTower = Instantiate(selectedTowerPref, selectedTowerBuildPos.position, Quaternion.identity);

        // 설치된 타워 부모를 선택된 설치 위치로
        buildTower.transform.SetParent(selectedTowerBuildPos);

        // 설치 패널 비활성화
        towerBuildPanel.SetActive(false);

        // 패널 비활성화된 상태
        isPanel = false;
    }

    // 타워 해제
    public void DeleteTower()
    {
        // 매직 타워는 이전에 생성된 이펙트 비활성화
        MagicTowerEffectInit();

        // 스톤 타워 1 불 초기화
        StoneTowerFireInit();

        // 타워베이스 가져와서 골드 얻음
        TowerBase selectedTowerBase = selectedTowerBuildPos.GetChild(0).GetComponent<TowerBase>();
        GoldManager.Instance.AcquireGold(selectedTowerBase.towerUpgradeBasicPrice / 4);

        // 자식에 있던 타워 파괴
        Destroy(selectedTowerBuildPos.GetChild(0).gameObject);

        // 업그레이드 패널 비활성화
        towerUpgradePanel.SetActive(false);

        // 패널 비활성화된 상태
        isPanel = false;
    }

    // 타워 업그레이드 
    public void TowerUpgrade()
    {
        // 이전레벨 타워베이스 가져와서
        TowerBase selectedTowerBase = selectedTowerBuildPos.GetChild(0).GetComponent<TowerBase>();

        // 최대레벨이면 리턴
        if(selectedTowerBase.towerLv == 3) return;

        // 골드 체크
        if(!GoldManager.Instance.UseGold(selectedTowerBase.towerUpgradeBasicPrice)) return;

        // 매직 타워는 이전에 생성된 이펙트 비활성화
        MagicTowerEffectInit();

        // 스톤 타워 1 불 초기화
        StoneTowerFireInit();

        // 다음레벨 타워 설치
        GameObject buildTower = Instantiate(towers[selectedTowerBase.towerType].towerPrefabs[selectedTowerBase.towerLv], selectedTowerBuildPos.position, Quaternion.identity);
        buildTower.transform.SetParent(selectedTowerBuildPos);

        // 업그레이드 타워베이스 가져와서
        TowerBase upgradeTowerBase = selectedTowerBuildPos.GetChild(1).GetComponent<TowerBase>();

        // 이전레벨 타워 스탯을 기준으로 업그레이드 타워 스탯 증가
        upgradeTowerBase.towerLv = ++selectedTowerBase.towerLv;
        upgradeTowerBase.basicDamage = selectedTowerBase.basicDamage * upgradeTowerBase.towerLv;
        upgradeTowerBase.towerUpgradeBasicPrice = selectedTowerBase.towerUpgradeBasicPrice * upgradeTowerBase.towerLv;

        // 이전레벨 타워 삭제
        Destroy(selectedTowerBuildPos.GetChild(0).gameObject);

        // 업그레이드 패널 갱신
        upgradePanelTowerLvText.text = "타워 레벨 : " + upgradeTowerBase.towerLv.ToString();
        upgradePanelTowerPriceText.text = upgradeTowerBase.towerUpgradeBasicPrice.ToString();
        if(upgradeTowerBase.towerLv == 3) upgradePanelTowerPriceText.text = "";
        upgradePanelTowerStarImage.sprite = towerStarSprites[upgradeTowerBase.towerLv];
    }

    // 매직 타워 충돌 이펙트 초기화
    private void MagicTowerEffectInit()
    {
        // 매직 타워는 이전에 생성된 이펙트 비활성화
        MagicTowerBase magicTowerBase = selectedTowerBuildPos.GetChild(0).GetComponent<MagicTowerBase>();
        if(magicTowerBase != null)
        {
            for(int i = 0; i < magicTowerBase.towerWeaponEffectPrefabs.Count; i++) magicTowerBase.towerWeaponEffectPrefabs[i].gameObject.SetActive(false);

            magicTowerBase.towerWeaponEffectPrefabs.Clear();
        }
    }

    // 스톤 타워 1 불 초기화
    private void StoneTowerFireInit()
    {
        // 스톤 타워 1은 이전에 생성된 불 비활성화
        StoneTower1 stoneTower1 = selectedTowerBuildPos.GetChild(0).GetComponent<StoneTower1>();
        if(stoneTower1 != null)
        {
            for(int i = 0; i < stoneTower1.FirePrefabs.Count; i++) stoneTower1.FirePrefabs[i].gameObject.SetActive(false);

            stoneTower1.FirePrefabs.Clear();
        }
    }

    // 타워 UI 버튼 사운드
    public void OnBtnClickSound()
    {
        buttonSound.gameObject.SetActive(true);
        buttonSound.GetComponent<AudioSource>().volume = SoundManager.Instance.sfxVolume;
    }
}
