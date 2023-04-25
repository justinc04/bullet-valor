using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Settings : MonoBehaviour
{
    public static Settings Instance;

    public GameObject settingsMenu;

    [SerializeField] Slider sensitivitySlider;
    [SerializeField] Slider volumeSlider;

    [SerializeField] TMP_Text sensitivityValueText;
    [SerializeField] TMP_Text volumeValueText;

    public float mouseSensitivity;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity");
        volumeSlider.value = PlayerPrefs.GetInt("Volume");
        UpdateSensitivity();
        UpdateVolume();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenCloseSettings();
        }
    }

    public void OpenCloseSettings()
    {
        settingsMenu.SetActive(!settingsMenu.activeSelf);

        if (settingsMenu.activeSelf || ShopManager.Instance.shop.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void UpdateSensitivity()
    {
        PlayerPrefs.SetFloat("Sensitivity", sensitivitySlider.value);
        mouseSensitivity = sensitivitySlider.value;
        sensitivityValueText.text = sensitivitySlider.value.ToString("F2");
    }

    public void UpdateVolume()
    {
        PlayerPrefs.SetInt("Volume", (int)volumeSlider.value);
        AudioListener.volume = volumeSlider.value / 100f;
        volumeValueText.text = volumeSlider.value.ToString();
    }
}
