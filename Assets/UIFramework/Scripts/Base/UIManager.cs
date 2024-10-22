using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace FastDev.UiFramework
{

    public enum PanelName
    {
        Null = 0,
        // 表示主面板
        Main = 1, 


       
        RewardPanel = 9001, // 奖励弹窗面板 
        UIAnimationPanel = 9002, // UI 动画面板，值为 9002 
        ChoicePanel = 9003, // 选择面板，值为 9003
        WarningPanel = 9004,

        LoadingPanel =9091,//等待网络界面

    }

    //跳转界面定位
    public enum UiLocatePartEnum
    {
        SHOP_Chapture, 
    }
    //进行静态管理
    public partial class UIManager 
    {
        private const string PanelPrefabPath = "PanelPrefab/";
         
        private static Dictionary<PanelName, PanelBase> PanelCache
        {
            get
            {
                if (_panelCache == null)
                    _panelCache = new Dictionary<PanelName, PanelBase>();
                return _panelCache;
            }
        }
        private static Dictionary<PanelName, PanelBase> _panelCache;
         
        public static void OpenPanel(PanelName panelName, object data = null)
        {
            bool newLoad = true;
            PanelBase script = null;
            if (PanelCache.ContainsKey(panelName))
            {
                if (PanelCache[panelName] != null)
                {
                    //有缓存
                    newLoad = false;
                    script = PanelCache[panelName];
                }
                else
                {
                    //会从新生成一个
                    //Debug.LogError("缓存中曾经打开过，不过因为某些原因missing了:" + panelName.ToString());
                }
            }

            if (newLoad)
            {
                var pref = Resources.Load<GameObject>(PanelPrefabPath + panelName.ToString());
                var ins = GameObject.Instantiate(pref);
              
                //todo---设置加入到某个transform子下
                if (PanelCache.ContainsKey(panelName))
                    PanelCache[panelName] = ins.GetComponent<PanelBase>();
                else
                    PanelCache.Add(panelName, ins.GetComponent<PanelBase>());
                script = ins.GetComponent<PanelBase>();
                script.Init(data);
            }
            script.Refresh(data);



        }

        public static void ClosePanel(PanelName panelName, bool respwanInPool = true)
        {
            //具体关闭在panel内进行处理，此处可以用来上报一些数据

            if (respwanInPool)
            {
                if (PanelCache.ContainsKey(panelName) == false)
                    Debug.LogError("cant find in dic:"+panelName.ToString());
                PanelCache[panelName].gameObject.SetActive(false);
            }
            else
            {
                KillPanel(panelName);
            }

        }
        private static void KillPanel(PanelName panelName)
        {
            //对面板进行destroy，例如对话面板走这里进行销毁
            if (TryGetPanel(panelName, out var p))
            {
               GameObject.Destroy(p.gameObject);
                PanelCache.Remove(panelName);
            }

        }
         

        //获取面板入口，如果正在显示，才会返回
        public static bool TryGetPanel(PanelName panelName, out PanelBase resp)
        {

            resp = null;
            if (PanelCache.ContainsKey(panelName))
            {
                if (PanelCache[panelName] != null)
                {
                    resp = PanelCache[panelName];
                    return true;
                }
                else
                {
                    //Debug.LogError("缓存中曾经打开过，不过因为某些原因missing了:" + panelName.ToString());
                }
            }

            return false;
        }  
     

    }
    //内置业务ui
    public partial class UIManager
    {
        public static void ShowSystemPop(string str)
        {
            GetUIAnimationPanel().ShowSystemMsg(str);
        }
        public static void ShowChoicePanel(string title, string content, UnityAction okCall, string okString, string noString, UnityAction noCall = null)
        {
            ChiocePanelData d = new ChiocePanelData(content, okCall, noCall);
            d.title = title;
            d.okString = okString;
            d.noString = noString;

            UIManager.OpenPanel(PanelName.ChoicePanel, d);
        }

        public static void ShowWarningChoicePanel(string title, string content, UnityAction okCall, string okString, string noString, UnityAction noCall = null)
        {
            ChiocePanelData d = new ChiocePanelData(content, okCall, noCall);
            d.title = title;
            d.okString = okString;
            d.noString = noString;

            UIManager.OpenPanel(PanelName.WarningPanel, d);
        }

        //飞行ui动画等
        public static UIAnimationPanel GetUIAnimationPanel()
        {
            OpenPanel(PanelName.UIAnimationPanel);

            if (TryGetPanel(PanelName.UIAnimationPanel, out var p))
            {
                if (p is UIAnimationPanel ui)
                    return ui;
                else
                    Debug.LogError("出现了错误1");
            }
            //Debug.LogError("出现了错误2");
            return null;
        }

        //加载中界面
        public static void OpenLoading(string str = "")
        {
            if (string.IsNullOrEmpty(str))
                OpenPanel(PanelName.LoadingPanel);
            else
                OpenPanel(PanelName.LoadingPanel, str);

        }

        public static void CloseLoading()
        {
            if (TryGetPanel(PanelName.LoadingPanel, out var d)) { d.Close(); }
        }
        //定位到某个ui模块界面
        public static void LocatedUiPart(UiLocatePartEnum pos)
        {
            string str = pos.ToString();
            //商店类
            if (str.Contains("SHOP"))
            {
                if (UIManager.TryGetPanel(PanelName.Null, out var m))
                {
                    //m.GetShopWidget().AimToPart(pos);
                }
            }
            else if (str.Contains("???"))
            {

            }
        }


    }
}

