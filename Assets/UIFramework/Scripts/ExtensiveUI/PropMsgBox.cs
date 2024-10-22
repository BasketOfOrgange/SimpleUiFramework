using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FastDev.UiFramework
{
    public class PropMsgBox : MonoBehaviour
    {
        public Image img;
        public Text nameTxt;
        public Text contentTxt;
        public void Refresh(string title, string content)
        {
            nameTxt.text = title;
            contentTxt.text = content;

            if (JudgmentUiInScreen(img.GetComponent<RectTransform>(), out Vector3 offset))
            {
                img.GetComponent<RectTransform>().position += offset;
            }
            else
            {
                //居中对齐
                Vector3 pos = img.GetComponent<RectTransform>().localPosition;
                pos.x = 0;
                img.GetComponent<RectTransform>().localPosition = pos;
            }
            transform.localScale = Vector3.one;
        }

        //偏移算法
        bool JudgmentUiInScreen(RectTransform rect, out Vector3 offset)
        {
            float screenSizer = Screen.height / 1600f;//this is importance,the root canvas will scale the size ,so here need time the position

            //  锚点是0-1的百分比，所以考虑*height

            bool isOutView = false;
            float moveDistance = 0;
            Vector3 worldPos = rect.transform.position;
            float leftX = worldPos.x - (rect.rect.width * screenSizer / 2);
            //float leftX = worldPos.x - (rect.sizeDelta.x / 2)-80;//mark，跟锚点没关系,-80能解决，不知其所以然  
            float rightX = worldPos.x + (rect.sizeDelta.x * screenSizer / 2);
            Debug.LogWarning(rect.gameObject.name + "?" + worldPos + $"x:{leftX}..r:{rightX}...rect.rect:{rect.rect}..rect.sizeDelta{rect.sizeDelta}.");
            if (leftX >= 0 && rightX <= Screen.width)
            {
                offset = Vector3.zero;
                Debug.LogWarning(Screen.width + " +++NONON");
            }
            else
            {
                if (leftX < 0)//需要右移进入屏幕范围
                {
                    moveDistance = -leftX;//mark
                                          //moveDistance = -leftX-80;//mark
                }
                if (rightX > Screen.width)//需要左移进入屏幕范围
                {
                    moveDistance = Screen.width - (rightX);
                }
                offset = new Vector3(moveDistance, 0, 0);
                //Debug.LogError(offset+"?" + worldPos);
                isOutView = true;
                Debug.LogWarning(Screen.width + " +++");
            }
            return isOutView;
        }
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Destroy(this.gameObject);
            }
        }
    }
}