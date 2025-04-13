using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] int maxHealth = 100;
    [SerializeField] bool destroyOnDeath = true;

    [Header("Effects")]
    [SerializeField] GameObject deathEffect;

    [Header("UI References")]
    [SerializeField] Slider healthSlider;
    [SerializeField] TMP_Text healthText;
    
    [SerializeField]
    private int currentHealth;

    public event System.Action<float> OnHealthChanged;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        UpdateUI();
        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        if (deathEffect) Instantiate(deathEffect, transform.position, Quaternion.identity);

        if (destroyOnDeath)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateUI();
    }

    void UpdateUI()
    {
        float healthPercentage = (float)currentHealth / maxHealth;

        // ���������� Slider
        if (healthSlider)
            healthSlider.value = healthPercentage;

        // ���������� Text
        if (healthText)
            healthText.text = $"HP: {currentHealth}/{maxHealth}";

        // ����� �������
        OnHealthChanged?.Invoke(healthPercentage);
    }
}
