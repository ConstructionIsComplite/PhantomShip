using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    [Header("Text References")]
    [SerializeField] TMP_Text magazineText;
    [SerializeField] TMP_Text totalAmmoText;

    [Header("Reload UI")]
    [SerializeField] Image reloadProgressBar;
    [SerializeField] GameObject reloadIndicator;

    [Header("Visual Settings")]
    [SerializeField] Color normalColor = Color.white;
    [SerializeField] Color lowAmmoColor = Color.red;
    [SerializeField] float lowAmmoThreshold = 0.3f;
    [SerializeField] float pulseSpeed = 2f;

    private WeaponController weapon;
    private RectTransform magazineTransform;
    private Vector3 initialScale;

    void Start()
    {
        weapon = FindObjectOfType<WeaponController>();
        magazineTransform = magazineText.GetComponent<RectTransform>();
        initialScale = magazineTransform.localScale;

        SubscribeToEvents();
        InitializeUI();
    }

    void SubscribeToEvents()
    {
        weapon.OnMagazineChanged.AddListener(UpdateMagazine);
        weapon.OnTotalAmmoChanged.AddListener(UpdateTotalAmmo);
        weapon.OnReloadProgressChanged.AddListener(UpdateReloadProgress);
        weapon.OnReloadStart.AddListener(ShowReloadIndicator);
        weapon.OnReloadComplete.AddListener(HideReloadIndicator);
    }

    void InitializeUI()
    {
        UpdateMagazine(weapon.CurrentMagazine);
        UpdateTotalAmmo(weapon.TotalAmmo);
        reloadProgressBar.fillAmount = 0f;
        reloadIndicator.SetActive(false);
    }

    void UpdateMagazine(int amount)
    {
        magazineText.text = $"{amount}";
        UpdateMagazineColor(amount);
        HandleLowAmmoPulse(amount);
    }

    void UpdateMagazineColor(int amount)
    {
        float fillRatio = (float)amount / weapon.MaxMagazine;
        magazineText.color = Color.Lerp(
            lowAmmoColor,
            normalColor,
            fillRatio / lowAmmoThreshold
        );
    }

    void HandleLowAmmoPulse(int amount)
    {
        if ((float)amount / weapon.MaxMagazine < lowAmmoThreshold)
        {
            float pulse = Mathf.PingPong(Time.time * pulseSpeed, 0.2f);
            magazineTransform.localScale = initialScale * (1f + pulse);
        }
        else
        {
            magazineTransform.localScale = initialScale;
        }
    }

    void UpdateTotalAmmo(int amount)
    {
        totalAmmoText.text = $"{amount}";
    }

    void UpdateReloadProgress(float progress)
    {
        reloadProgressBar.fillAmount = progress;
    }

    void ShowReloadIndicator()
    {
        reloadIndicator.SetActive(true);
    }

    void HideReloadIndicator()
    {
        reloadIndicator.SetActive(false);
    }

    void OnDestroy()
    {
        weapon.OnMagazineChanged.RemoveListener(UpdateMagazine);
        weapon.OnTotalAmmoChanged.RemoveListener(UpdateTotalAmmo);
        weapon.OnReloadProgressChanged.RemoveListener(UpdateReloadProgress);
        weapon.OnReloadStart.RemoveListener(ShowReloadIndicator);
        weapon.OnReloadComplete.RemoveListener(HideReloadIndicator);
    }
}
