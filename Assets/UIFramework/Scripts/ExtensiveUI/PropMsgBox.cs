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
                //���ж���
                Vector3 pos = img.GetComponent<RectTransform>().localPosition;
                pos.x = 0;
                img.GetComponent<RectTransform>().localPosition = pos;
            }
            transform.localScale = Vector3.one;
        }

        //ƫ���㷨
        bool JudgmentUiInScreen(RectTransform rect, out Vector3 offset)
        {
            float screenSizer = Screen.height / 1600f;//this is importance,the root canvas will scale the size ,so here need time the position

            //  ê����0-1�İٷֱȣ����Կ���*height

            bool isOutView = false;
            float moveDistance = 0;
            Vector3 worldPos = rect.transform.position;
            float leftX = worldPos.x - (rect.rect.width * screenSizer / 2);
            //float leftX = worldPos.x - (rect.sizeDelta.x / 2)-80;//mark����ê��û��ϵ,-80�ܽ������֪������Ȼ  
            float rightX = worldPos.x + (rect.sizeDelta.x * screenSizer / 2);
            Debug.LogWarning(rect.gameObject.name + "?" + worldPos + $"x:{leftX}..r:{rightX}...rect.rect:{rect.rect}..rect.sizeDelta{rect.sizeDelta}.");
            if (leftX >= 0 && rightX <= Screen.width)
            {
                offset = Vector3.zero;
                Debug.LogWarning(Screen.width + " +++NONON");
            }
            else
            {
                if (leftX < 0)//��Ҫ���ƽ�����Ļ��Χ
                {
                    moveDistance = -leftX;//mark
                                          //moveDistance = -leftX-80;//mark
                }
                if (rightX > Screen.width)//��Ҫ���ƽ�����Ļ��Χ
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