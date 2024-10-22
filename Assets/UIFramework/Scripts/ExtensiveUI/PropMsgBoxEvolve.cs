using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FastDev.UiFramework
{
    public class PropMsgBoxEvolve : MonoBehaviour
    {
        public Image img;
        public Text nameTxt;
        public Text contentTxt;
        public Text additionValueTxt;

        //按钮相关
        public Button btn;
        public Image needIcon;
        public Text needCnt;

        public GameObject trigger;//大的关闭按钮
        public GameObject Sizer;//控件的大小，来检查是否在点击外，关闭

        //业务逻辑\
        private void SetDataTrans(object d)
        {
            //拆箱装自己的业务数据
            if (d is string str)
            {

            }
        }


        //框架内容，非必要不改↓↓↓↓↓↓↓↓↓↓

        //带按钮
        public void RefreshWithBtn(string title, string content, string addStr, object d, Action<object> onBtnClick)
        {
            Refresh(title, content, addStr);
            btn.gameObject.SetActive(true);
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => { onBtnClick(d); });
            btn.onClick.AddListener(() =>
            {
                SetDataTrans(d);
                Destroy(gameObject);
            });
            var need = 0;
            var have = 0;

            string needStr = need > have ? $"<color=red>{need}</color>" : $"{need}";
            needCnt.text = needStr;
        }


        //还不带按钮
        public void RefreshNoBtn(string title, string content, string addstr)
        {
            Refresh(title, content, addstr);
            btn.gameObject.SetActive(false);

        }
        private void Refresh(string title, string content, string addstr)
        {
            nameTxt.text = title;
            contentTxt.text = content;
            additionValueTxt.text = addstr;
            if (JudgmentUiInScreen(img.GetComponent<RectTransform>(), out Vector3 offset))
            {
                img.GetComponent<RectTransform>().position += offset;
            }
            transform.localScale = Vector3.one;
        }
        //呗上面遮住把他置顶位置
        bool JudgmentUiInScreen(RectTransform rect, out Vector3 offset)
        {
            bool isOutView = false;
            float moveDistance = 0;
            Vector3 worldPos = rect.transform.position;
            float leftX = worldPos.x - rect.sizeDelta.x / 2;
            float rightX = worldPos.x + rect.sizeDelta.x / 2;
            if (leftX >= 0 && rightX <= Screen.width)
            {
                offset = Vector3.zero;
            }
            else
            {
                if (leftX < 0)//需要右移进入屏幕范围
                {
                    moveDistance = -leftX;
                }
                if (rightX > Screen.width)//需要左移进入屏幕范围
                {
                    moveDistance = Screen.width - rightX;
                }
                offset = new Vector3(moveDistance, 0, 0);
                isOutView = true;
            }
            return isOutView;
        }
        private void Update()
        {
            // 如果鼠标左键被按下
            if (Input.GetMouseButtonDown(0))
            {
                PointerEventData eventData = new PointerEventData(EventSystem.current);
                eventData.position = Input.mousePosition;

                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventData, results);
                bool isOnPanel = false;
                foreach (var result in results)
                {
                    if (result.gameObject == Sizer)
                    {
                        isOnPanel = true;
                        return;
                    }
                }
                if (!isOnPanel)
                {
                    Destroy(gameObject);
                }
            }
        }

        //private string GetLanguageConfig(int Id)
        //{
        //    var c = ConfigManager.m_tables.CopywritingConfig.Get(Id);
        //    return c.Copywriting;
        //}
    }
}