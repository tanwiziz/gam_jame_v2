using UnityEngine;
using UnityEngine.UI;
using System;

public class BaseHealth : MonoBehaviour
{
    public float maxHP = 100f;
    public float currentHP;
    public Slider hpSlider;

    public event Action<float> OnHPChanged;
    public event Action OnBaseDestroyed;

    public float CurrentRatio => maxHP <= 0 ? 0 : currentHP / maxHP;

    void Start()
    {
        currentHP = maxHP;
        if (hpSlider) hpSlider.value = 1f;
        OnHPChanged?.Invoke(CurrentRatio);
    }

    public void TakeDamage(float dmg)
    {
        currentHP -= dmg;
        if (currentHP <= 0f)
        {
            currentHP = 0f;
            if (hpSlider) hpSlider.value = 0f;
            OnHPChanged?.Invoke(0f);
            OnBaseDestroyed?.Invoke();
            return;
        }
        if (hpSlider) hpSlider.value = CurrentRatio;
        OnHPChanged?.Invoke(CurrentRatio);
    }
}
