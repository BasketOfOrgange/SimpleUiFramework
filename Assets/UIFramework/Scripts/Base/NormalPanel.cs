using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
 
using UnityEngine;
using UnityEngine.UI;

namespace FastDev.UiFramework
{
    public class NormalPanel : PanelBase
    {
        //�Դ�һ������غ͹رհ�ť

        public GameObject? item, itemContainer;

        [SerializeField] protected Button closeButton; // �رհ�ť
        [SerializeField] protected Button? closeButtonBg; // ��������رհ�ť
        public override void Init(object data)
        {


            if (closeButtonBg != null)
            {
                closeButtonBg.onClick.RemoveAllListeners();
                closeButtonBg.onClick.AddListener(OnClickClose);
            }
            base.Init(data);
        }
        public override void Refresh(object data)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(OnClickClose);

            //InterRegisteHttpBack();
            base.Refresh(data);
        }


        private List<GameObject> _pool;
        private List<GameObject> Pool
        {
            get
            {
                if (_pool == null)
                    _pool = new List<GameObject>();
                return _pool;
            }
        }
        protected List<GameObject> GetPool { get { return Pool; } }
        protected virtual void ClearPool()
        {
            Pool.ForEach(t => t.gameObject.SetActive(false));

            if(item!=null)
            {
                if (item.activeSelf)
                    item.SetActive(false);
            }
        }
        protected GameObject LoadOneItemInPool(bool isSetTrue = false, bool defaulPutInContainer = true)
        {
            foreach (var i in Pool)
            {
                if (i.activeSelf == false)
                {
                    if (isSetTrue)
                    {
                        i.SetActive(true);
                    }
                    return i;
                }
            }
            GameObject ins = null;
            if (defaulPutInContainer)
            {
                ins = Instantiate(item, itemContainer.transform);
                Pool.Add(ins);
            }
            else
            {
                ins = Instantiate(item);
                Pool.Add(ins);
            }
            if (isSetTrue)
            {
                ins.SetActive(true);
            }
            return ins;

        }

        protected virtual void OnClickClose()
        { 
            closeButton.transform.localScale = Vector3.one;
            Close();
        }

        protected string GetConfigString(int id)
        {

            return "";
            //var d = ConfigManager.m_tables.CopywritingConfig.Get(id);
            //if (d != null)
            //    return d.Copywriting;
            //else
            //{
            //    return id.ToString();
            //    Debug.LogError("���߻���" + id);
            //}
        }


        #region HTTP
        //ע��ص�
        //protected List<SimpleHttpObserver> observers;

        ////�̳�ֻ��ע��д���һ������
        //protected virtual void RegisteHttpBack() { }
        
        //protected void AddOneRegist(string name,Action Call)
        //{
        //    SimpleHttpObserver o = new SimpleHttpObserver(name, Call);
        //    Facade.Instance.RegisterObserver(name, o); 
        //    observers.Add(o);
        //}
      
        //private void InterRegisteHttpBack()
        //{
        //    if (observers == null)
        //    {
        //        observers = new List<SimpleHttpObserver>();
        //        RegisteHttpBack();
        //    }
        //}

        //private void ClearObservers()
        //{
        //    //�Ƴ�http����
        //    if (observers != null)
        //    {
        //        foreach (var i in observers) { Facade.Instance.RemoveObserver(i.observerString, i); }
        //    }
        //}

        //public override void Close(bool immidiately = false)
        //{
        //    ClearObservers();
        //    observers = null;
        //    base.Close(immidiately);
        //}
        #endregion


    }
}