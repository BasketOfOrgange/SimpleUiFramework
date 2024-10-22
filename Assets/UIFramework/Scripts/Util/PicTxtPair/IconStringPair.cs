using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconStringPair : MonoBehaviour
{
    public Image img;
    public Text txt;
    public void Refresh(Sprite sp ,string content)
    {
        img.sprite = sp;
        txt.text = content;
    }
}
