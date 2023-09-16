using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CameraSlider : MonoBehaviour
{
    public TMP_Text ValueText;

    void OnEnable()
    {
        this.gameObject.GetComponent<Slider>().value = GameManager.CameraSensitivity;
    }
    
    public void OnSliderChanged(float value)
    {
        GameManager.CameraSensitivity = value;
        ValueText.text = value.ToString();
    }

}
