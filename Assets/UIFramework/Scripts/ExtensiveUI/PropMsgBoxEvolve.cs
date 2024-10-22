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

        //��ť���
        public Button btn;
        public Image needIcon;
        public Text needCnt;

        public GameObject trigger;//��Ĺرհ�ť
        public GameObject Sizer;//�ؼ��Ĵ�С��������Ƿ��ڵ���⣬�ر�

        //ҵ���߼�\
        private void SetDataTrans(object d)
        {
            //����װ�Լ���ҵ������
            if (d is string str)
            {

            }
        }


        //������ݣ��Ǳ�Ҫ���ġ�������������������

        //����ť
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


        //��������ť
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
        //��������ס�����ö�λ��
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
                if (leftX < 0)//��Ҫ���ƽ�����Ļ��Χ
                {
                    moveDistance = -leftX;
                }
                if (rightX > Screen.width)//��Ҫ���ƽ�����Ļ��Χ
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
            // ���������������
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