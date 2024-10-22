using System.Collections;
using System.Collections.Generic;
 
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class UIToggleAudio : MonoBehaviour
{
    private void OnEnable()
    {
        var btn = GetComponent<Toggle>();
        btn.onValueChanged.RemoveListener(AddAudio);
        btn.onValueChanged.AddListener(AddAudio);
    }
    private void AddAudio(bool isOn)
    {
        //AudioManager.PlaySound_UI("Ui_CommonClick");
    }
}
