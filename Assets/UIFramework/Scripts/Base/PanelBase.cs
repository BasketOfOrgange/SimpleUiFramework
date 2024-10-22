using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastDev.UiFramework
{
    public class PanelBase : MonoBehaviour
{
    /*
     Init������״�instaniateʵ����ʱ�����
     Refresh�ڼ����ʱ����е���
     data�ǺںУ��ڴ�������is ���䣬Ĭ��Ϊ�գ�Ҫ���������ȿ����������class��ʲô
     ��һ�����ΪxxxPanel,dataΪxxxPanelData��
     */


    public bool KillPanelWhenClose = false;
    [Header("��Ҫ����")]
    [Tooltip("��Ҫ��Ԥ��������һ��")]
    public PanelName m_PanelName;
    public bool IsOpen { get; private set; }

    protected object internalData;
    public virtual void Init(object data)
    {
        internalData = data;
    }


    public virtual void Refresh(object data)
    {
        internalData = data;
        IsOpen = false;
        if (gameObject.activeSelf == false)
            gameObject.SetActive(true);
    }

    /// <summary>
    /// ��д��Ҫ��������base
    /// </summary>
    /// <param name="immidiately"></param>
    public virtual void Close(bool immidiately = false)
    {
        IsOpen = false;

        if (KillPanelWhenClose)
        {
            Kill();
        }
        else
        {
            UIManager.ClosePanel(m_PanelName);
        } 
    }

    public virtual void Kill(bool immidiately = false)
    {
        IsOpen = false;
        UIManager.ClosePanel(m_PanelName, false);
    }


    protected void ShowTipPop(string str)
    {

    }
    protected void ShowPop(string str)
    { 
    }

    protected void ShowChoicePop(string content, Action OnOkClick, Action OnNoClick = null)
    {

    }

}
}