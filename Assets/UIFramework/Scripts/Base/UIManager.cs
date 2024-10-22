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
        // ��ʾ�����
        Main = 1, 


       
        RewardPanel = 9001, // ����������� 
        UIAnimationPanel = 9002, // UI ������壬ֵΪ 9002 
        ChoicePanel = 9003, // ѡ����壬ֵΪ 9003
        WarningPanel = 9004,

        LoadingPanel =9091,//�ȴ��������

    }

    //��ת���涨λ
    public enum UiLocatePartEnum
    {
        SHOP_Chapture, 
    }
    //���о�̬����
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
                    //�л���
                    newLoad = false;
                    script = PanelCache[panelName];
                }
                else
                {
                    //���������һ��
                    //Debug.LogError("�����������򿪹���������ΪĳЩԭ��missing��:" + panelName.ToString());
                }
            }

            if (newLoad)
            {
                var pref = Resources.Load<GameObject>(PanelPrefabPath + panelName.ToString());
                var ins = GameObject.Instantiate(pref);
              
                //todo---���ü��뵽ĳ��transform����
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
            //����ر���panel�ڽ��д����˴����������ϱ�һЩ����

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
            //��������destroy������Ի�����������������
            if (TryGetPanel(panelName, out var p))
            {
               GameObject.Destroy(p.gameObject);
                PanelCache.Remove(panelName);
            }

        }
         

        //��ȡ�����ڣ����������ʾ���Ż᷵��
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
                    //Debug.LogError("�����������򿪹���������ΪĳЩԭ��missing��:" + panelName.ToString());
                }
            }

            return false;
        }  
     

    }
    //����ҵ��ui
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

        //����ui������
        public static UIAnimationPanel GetUIAnimationPanel()
        {
            OpenPanel(PanelName.UIAnimationPanel);

            if (TryGetPanel(PanelName.UIAnimationPanel, out var p))
            {
                if (p is UIAnimationPanel ui)
                    return ui;
                else
                    Debug.LogError("�����˴���1");
            }
            //Debug.LogError("�����˴���2");
            return null;
        }

        //�����н���
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
        //��λ��ĳ��uiģ�����
        public static void LocatedUiPart(UiLocatePartEnum pos)
        {
            string str = pos.ToString();
            //�̵���
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

