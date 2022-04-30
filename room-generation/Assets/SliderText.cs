using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderText : MonoBehaviour
{
    private void Start() {
        Slider slider = GetComponentInParent<Slider>();
        Text text = GetComponent<Text>();
        text.text = slider.value.ToString();
        slider.onValueChanged.AddListener((value) => {text.text = value.ToString();});
    }
}
