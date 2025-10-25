using UnityEngine;
using UnityEngine.UI;

public class UI_BaseHPBar : MonoBehaviour
{
    public BaseHealth baseHealth;
    public Slider slider;

    void Awake()
    {
        if (!slider) slider = GetComponent<Slider>();
    }

    void OnEnable()
    {
        if (!baseHealth) baseHealth = FindObjectOfType<BaseHealth>();
        if (baseHealth != null)
        {
            baseHealth.OnHPChanged -= OnHPChanged;
            baseHealth.OnHPChanged += OnHPChanged;
            OnHPChanged(baseHealth.CurrentRatio);
        }
    }

    void OnDisable()
    {
        if (baseHealth != null) baseHealth.OnHPChanged -= OnHPChanged;
    }

    void OnHPChanged(float ratio)
    {
        if (slider) slider.value = ratio;
    }
}
