using System.Collections;
using System.Collections.Generic;
 
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonAudio : MonoBehaviour
{
    public string audioName = "Ui_CommonClick";
    private void OnEnable()
    {
        var btn = GetComponent<Button>();
        btn.onClick.RemoveListener(AddAudio);
        btn.onClick.AddListener(AddAudio);
    }
    private void AddAudio()
    {
        //AudioManager.PlaySound_UI(audioName);

    }
      
    public void CheckAdd()
    {
        var btn = GetComponent<Button>();
        btn.onClick.RemoveListener(AddAudio);
        btn.onClick.AddListener(AddAudio);
    }
}
