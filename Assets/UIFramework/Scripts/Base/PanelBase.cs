using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastDev.UiFramework
{
    public class PanelBase : MonoBehaviour
{
    /*
     Init在面板首次instaniate实例化时候调用
     Refresh在激活打开时候进行调用
     data是黑盒，在传入后进行is 拆箱，默认为空，要传入数据先看里面解析的class是什么
     【一般面板为xxxPanel,data为xxxPanelData】
     */


    public bool KillPanelWhenClose = false;
    [Header("需要配置")]
    [Tooltip("需要和预制体名字一样")]
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
    /// 重写需要后调用这个base
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