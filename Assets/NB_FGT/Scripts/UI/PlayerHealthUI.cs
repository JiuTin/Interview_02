using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NB_FGT.HealthData;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField]private Image _healthImage;

    private void OnEnable()
    {
        GameEventManager.MainInstance.AddEventListening<CharacterHealthInfo>("更新生命值UI", UpdateHealthImage);
    }

    private void OnDisable()
    {
        GameEventManager.MainInstance.RemoveEvent<CharacterHealthInfo>("更新生命值UI", UpdateHealthImage);
    }

    public void UpdateHealthImage(CharacterHealthInfo healthInfo)
    {
        _healthImage.fillAmount = healthInfo.CurrentHP / healthInfo.MaxHP;
    }
}
