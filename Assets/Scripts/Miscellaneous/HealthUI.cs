using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Slider healthSlider;
    public void SetTextToSliderValue()
    {
        healthText.text = ((int)(healthSlider.value * 100)).ToString() + "%";
    }
}
