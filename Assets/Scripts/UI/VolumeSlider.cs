using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VolumeSlider : MonoBehaviour
{
    public TMP_Text ValueText;

    void OnEnable()
    {
        this.gameObject.GetComponent<Slider>().value = GameManager.Volume;
    }

    public void OnSliderChanged(float value)
    {
        GameManager.Volume = value;
        ValueText.text = value.ToString();
    }

}
