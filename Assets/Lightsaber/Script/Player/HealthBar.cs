using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSlider(float amount)
    {
        healthSlider.value = amount;


    }
    public void SetSliderMax(float amount)
    {
        healthSlider.maxValue = amount;
        healthSlider.value = amount;
    }
}
