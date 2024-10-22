using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace FastDev.UiFramework
{
    public interface IRewardBoxPropData
    {
        public int Type { get; set; }//0为默认icon-string
        public Sprite Icon { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public object CustomData { get; set; }

        public Action<object> OnClickCall { get; set; }
    }
    public class PropRewardBox : ItemWithPoolBase
    {
        //public GameObject ClickGoPanel;
        public void Refresh(List<IRewardBoxPropData> lst)
        {
            ClearPool();

            foreach (var i in lst)
            {
                switch (i.Type)
                {
                    case 0://默认的自己写
                        var ins = LoadOneItemInPool();
                        //ins.GetComponent<EquipmentButton>().PureShowRefresh(_d);
                        //ins.GetComponent<EquipmentButton>().countTxt.gameObject.SetActive(true);
                        ins.SetActive(true);

                        break;
                }

            }

            //实例参考↓↓↓
            //for (int i = 0; i < equi.Count; i++)
            //{
            //    var _d = equi[i];
            //     }
            //for (int i = 0; i < pro.Count; i++)
            //{
            //    var _d = pro[i];
            //    var ins = LoadOneItemInPool();
            //    ins.GetComponent<EquipmentButton>().RefreshSimple(_d, (x, y) =>
            //    {
            //        if (x is PlayerPropData p)
            //        {
            //            UIManager.GetUIAnimationPanel().ShowPropShortMsg(p.PropName, p.Desc, y);
            //        }
            //    });
            //    ins.GetComponent<EquipmentButton>().countTxt.gameObject.SetActive(true);
            //    ins.SetActive(true); 
            //}
            transform.localScale = Vector3.one;
        }

        private void Update()
        {
            //if (Input.GetMouseButtonDown(0))
            //{
            //    Destroy(this.gameObject);
            //}
            if (Input.GetMouseButtonDown(0))
            {
                // 创建一条从摄像机指向鼠标位置的射线
                PointerEventData pointerData = new PointerEventData(EventSystem.current);
                pointerData.position = Input.mousePosition;

                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);

                bool needKill = true;
                foreach (RaycastResult result in results)
                {
                    if (result.gameObject == itemContainer)
                    {
                        needKill = false;
                        break;
                    }
                }

                if (needKill)
                {
                    Kill();
                }
            }

        }

        public void Kill()
        {
            Destroy(this.gameObject);

        }
    }
}