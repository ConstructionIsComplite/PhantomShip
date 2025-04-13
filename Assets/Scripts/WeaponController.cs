using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class AmmoEvent : UnityEvent<int> { }

public class WeaponController : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] AudioClip fireSound;
    [SerializeField] AudioClip reloadSound;
    [SerializeField] float fireRate = 0.2f;
    [SerializeField] float projectileSpeed = 25f;

    [Header("Ammo Settings")]
    [SerializeField] int maxMagazine = 30;
    [SerializeField] int maxTotalAmmo = 150;
    [SerializeField] float reloadTime = 1.5f;

    [Header("Animation")]
    [SerializeField] Animator animator;

    [Header("Events")]
    public AmmoEvent OnMagazineChanged = new AmmoEvent();
    public AmmoEvent OnTotalAmmoChanged = new AmmoEvent();
    public UnityEvent<float> OnReloadProgressChanged = new UnityEvent<float>();
    public UnityEvent OnReloadStart = new UnityEvent();
    public UnityEvent OnReloadComplete = new UnityEvent();

    private int currentMagazine;
    private int currentTotalAmmo;
    private bool isReloading;
    private float reloadTimer;
    private float nextFireTime;

    public int CurrentMagazine => currentMagazine;
    public int MaxMagazine => maxMagazine;
    public int TotalAmmo => currentTotalAmmo;

    void Start()
    {
        currentMagazine = maxMagazine;
        currentTotalAmmo = maxTotalAmmo;
        UpdateAmmoUI();
    }

    void Update()
    {
        HandleShooting();
        HandleReloadInput();
        UpdateReloadProgress();
    }

    void HandleShooting()
    {
        if (CanShoot())
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
            currentMagazine--;
            UpdateAmmoUI();
        }
    }

    bool CanShoot()
    {
        return Input.GetButton("Fire1")
            && Time.time >= nextFireTime
            && currentMagazine > 0
            && !isReloading;
    }

    void Shoot()
    {
        animator.SetTrigger("Attack");
        Vector3 shootDirection = CalculateShootDirection();
        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(shootDirection)
        );

        projectile.GetComponent<Projectile>().Initialize(shootDirection, projectileSpeed);
        
        SoundEvents.OnPlayerCharacterEffect(fireSound);
    }

    Vector3 CalculateShootDirection()
    {
        Plane horizontalPlane = new Plane(Vector3.up, firePoint.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        return horizontalPlane.Raycast(ray, out float distance)
            ? (ray.GetPoint(distance) - firePoint.position).normalized
            : Camera.main.transform.forward;
    }

    void HandleReloadInput()
    {
        if (Input.GetKeyDown(KeyCode.R)) StartReload();
    }

    public void StartReload()
    {
        if (!isReloading && NeedsReload() && HasAmmoToReload())
        {
            SoundEvents.OnPlayerCharacterEffect(reloadSound);
            isReloading = true;
            reloadTimer = 0f;
            OnReloadStart?.Invoke();
        }
    }

    bool NeedsReload() => currentMagazine < maxMagazine;
    bool HasAmmoToReload() => currentTotalAmmo > 0;

    void UpdateReloadProgress()
    {
        if (!isReloading) return;

        reloadTimer += Time.deltaTime;
        float progress = Mathf.Clamp01(reloadTimer / reloadTime);
        OnReloadProgressChanged?.Invoke(progress);

        if (progress >= 1f) FinishReload();
    }

    void FinishReload()
    {
        int ammoNeeded = maxMagazine - currentMagazine;
        int ammoToAdd = Mathf.Min(ammoNeeded, currentTotalAmmo);

        currentMagazine += ammoToAdd;
        currentTotalAmmo -= ammoToAdd;

        isReloading = false;
        UpdateAmmoUI();
        OnReloadComplete?.Invoke();
    }

    void UpdateAmmoUI()
    {
        OnMagazineChanged?.Invoke(currentMagazine);
        OnTotalAmmoChanged?.Invoke(currentTotalAmmo);
    }

    public void AddAmmo(int amount)
    {
        currentTotalAmmo = Mathf.Min(currentTotalAmmo + amount, maxTotalAmmo);
        UpdateAmmoUI();
    }
}
