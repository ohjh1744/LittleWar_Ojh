using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class SoundManager : SingletonBehaviour<SoundManager>
{
	[System.Serializable]
	public class Sound
	{
		public SoundType SoundType;
		public AudioClip Clip;
	}

	[SerializeField]
	private List<Sound> _bgms = null;
	[SerializeField]
	private List<Sound> _sfxs = null;
	[SerializeField]
	private AudioSource _bgmPlayer = null;
	public List<AudioSource> SfxPlayers = new List<AudioSource>();

	private Dictionary<SoundType, AudioClip> _sfxDictionary;

	private Queue<AudioSource> _sfxQueue = new Queue<AudioSource>();

	public float bgmVolume, sfxVolume; // 배경음 볼륨 및 효과음 볼륨

    public GameObject buttonSound; // UI 버튼 사운드

	protected override void Awake()
	{
		base.Awake();

		// SFX ��ųʸ� �ʱ�ȭ
		_sfxDictionary = _sfxs.ToDictionary(s => s.SoundType, s => s.Clip);
	}

	private void Start()
	{
		_bgmPlayer = gameObject.AddComponent<AudioSource>();

        // 볼륨 초기화
        bgmVolume = 0.3f; // 0.5
        sfxVolume = 1f;

		// SFX �÷��̾� �� ���� �ʱ⿡ �����ϰ� ����Ʈ�� �߰�
		for (int i = 0; i < 20; i++)
		{
			AudioSource sfxPlayer = gameObject.AddComponent<AudioSource>();
			SfxPlayers.Add(sfxPlayer);
			_sfxQueue.Enqueue(sfxPlayer);
		}

		// 처음은 메뉴 BGM 재생
		// 게임씬으로 가면 일반 BGM 재생
		// 보스가 나오면 각 보스 BGM 재생
		PlayBGM(SoundType.메뉴BGM);
	}

	// 배경음 테스트
	private void Update()
	{
        if (Input.GetKeyDown(KeyCode.Alpha1)) PlayBGM(SoundType.메뉴BGM);
        if (Input.GetKeyDown(KeyCode.Alpha2)) PlayBGM(SoundType.일반BGM);
        if (Input.GetKeyDown(KeyCode.Alpha3)) PlayBGM(SoundType.보스BGM1);
        if (Input.GetKeyDown(KeyCode.Alpha4)) PlayBGM(SoundType.보스BGM2);
        if (Input.GetKeyDown(KeyCode.Alpha5)) PlayBGM(SoundType.보스BGM3);
        if (Input.GetKeyDown(KeyCode.Alpha6)) PlayBGM(SoundType.보스BGM4);
	}

	public void PlayBGM(SoundType soundType)
	{
		var bgm = _bgms.First(b => b.SoundType == soundType);
		_bgmPlayer.clip = bgm.Clip;
		_bgmPlayer.volume = bgmVolume; // 볼륨 조절 bgmVolume
		_bgmPlayer.loop = true;
		_bgmPlayer.Play();
	}

	public void StopBGM()
	{
		_bgmPlayer.Stop();
	}

	// 코루틴
	// public void PlaySFX(SoundType soundType)
	// {
	// 	if (_sfxDictionary.TryGetValue(soundType, out AudioClip clip))
	// 	{
	// 		AudioSource sfxPlayer = GetAvailableSFXPlayer();
	// 		sfxPlayer.clip = clip;
	// 		sfxPlayer.volume = sfxVolume; // 볼륨 조절 sfxVolume
	// 		sfxPlayer.Play();

	// 		// 사운드 재생이 끝나면 풀에 반환
	// 		StartCoroutine(ReturnSFXPlayerWhenFinished(sfxPlayer, clip.length));
	// 	}
	// }

	// 유니태스크
	public void PlaySFX(SoundType soundType)
	{
		if (_sfxDictionary.TryGetValue(soundType, out AudioClip clip))
		{
			AudioSource sfxPlayer = GetAvailableSFXPlayer();
			sfxPlayer.clip = clip;
			sfxPlayer.volume = sfxVolume; // 볼륨 조절 sfxVolume
			sfxPlayer.Play();

			// 사운드 재생이 끝나면 풀에 반환
			ReturnSFXPlayerWhenFinished(sfxPlayer, clip.length).Forget();
		}
	}

	// public void PlaySFX(SoundType soundType, float volume = 1.0f)
	// {
	// 	if (_sfxDictionary.TryGetValue(soundType, out AudioClip clip))
	// 	{
	// 		AudioSource sfxPlayer = GetAvailableSFXPlayer();
	// 		sfxPlayer.clip = clip;
	// 		sfxPlayer.volume = volume;
	// 		sfxPlayer.Play();
	// 	}
	// }

	// public void PlaySFX(SoundType soundType, float volume = 1.0f, float delay = 1.0f)
	// {
	// 	if (_sfxDictionary.TryGetValue(soundType, out AudioClip clip))
	// 	{
	// 		AudioSource sfxPlayer = GetAvailableSFXPlayer();
	// 		sfxPlayer.clip = clip;
	// 		sfxPlayer.volume = volume;
	// 		sfxPlayer.PlayDelayed(delay); // delay�� �Ŀ� ���
	// 	}
	// }

	private AudioSource GetAvailableSFXPlayer()
	{
		if (_sfxQueue.Count > 0)
		{
			return _sfxQueue.Dequeue();
		}
		else
		{
			// �� �÷��̾ �����ϰ� ����Ʈ�� �߰�
			AudioSource newSFXPlayer = gameObject.AddComponent<AudioSource>();
			SfxPlayers.Add(newSFXPlayer);
			return newSFXPlayer;
		}
	}

	// ���� �÷��̰� ������ ȣ���Ͽ� ť�� �ٽ� �ֱ�
	public void ReturnSFXPlayerToQueue(AudioSource sfxPlayer)
	{
		_sfxQueue.Enqueue(sfxPlayer);
	}

	// 사운드 재생이 끝나면 풀에 반환

	// 코루틴
	// private IEnumerator ReturnSFXPlayerWhenFinished(AudioSource sfxPlayer, float delay)
	// {
	// 	// 사운드 재생 시간만큼 대기
	// 	yield return new WaitForSeconds(delay);

	// 	// 사운드 재생이 끝났으니 큐에 반환
	// 	ReturnSFXPlayerToQueue(sfxPlayer);
	// }

	// 유니태스크
	private async UniTaskVoid ReturnSFXPlayerWhenFinished(AudioSource sfxPlayer, float delay)
	{
		// 사운드 재생 시간만큼 대기
		await UniTask.Delay(TimeSpan.FromSeconds(delay));

		// 사운드 재생이 끝났으니 큐에 반환
		ReturnSFXPlayerToQueue(sfxPlayer);
	}

    // 배경음 볼륨 조절
    public void SetBgmVolume(float volume)
    {
        // 슬라이더 값에따라 볼륨 적용
        _bgmPlayer.volume = volume;

        // 슬라이더 값을 변수에 저장해서 배경음악을 실행할때마다 볼륨을 지정
        bgmVolume = volume;
    }

    // 효과음 볼륨 조절
    public void SetSfxVolume(float volume)
    {
        // 슬라이더 값을 변수에 저장해서 효과음을 실행할때마다 볼륨을 지정
        sfxVolume = volume;
    }

    // UI 버튼 사운드
    public void OnBtnClickSound()
    {
        buttonSound.gameObject.SetActive(true);
        buttonSound.GetComponent<AudioSource>().volume = sfxVolume;
    }
}
public enum SoundType
{
	아쳐타워123화살, 아쳐타워4화살,
	매직타워불, 매직타워얼음, 매직타워전기, 매직타워시간,
	스톤타워불, 스톤타워돌,
	메뉴BGM, 일반BGM, 보스BGM1, 보스BGM2, 보스BGM3, 보스BGM4,
	보스1_Death,보스2_Death,보스1_Skill,보스2_Skill,Enemy1_Death, Enemy2_Death, Enemy3_Death, Enemy4_Death, Enemy5_Death, Enemy6_Death, Enemy7_Death, Enemy8_Death, Enemy9_Death, Enemy10_Death,Enemy_Hit
}
