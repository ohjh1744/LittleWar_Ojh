using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameSetController : MonoBehaviour
{
    
    public Slider bgmSlider; // ����� �����̴�
    public Slider sfxSlider; // ȿ���� �����̴�

    private void OnAwake()
    {
        bgmSlider.onValueChanged.AddListener(SoundManager.Instance.SetBgmVolume); // ����� ���� �̺�Ʈ������ ���
        sfxSlider.onValueChanged.AddListener(SoundManager.Instance.SetSfxVolume); // ȿ���� ���� �̺�Ʈ������ ���
    }

    public void ReturnToMenuScene()
    {
        SceneManager.LoadScene("MenuScene");
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
