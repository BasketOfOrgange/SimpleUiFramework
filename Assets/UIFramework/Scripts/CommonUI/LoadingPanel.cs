using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace FastDev.UiFramework
{
    public class LoadingPanel : PanelBase
    {
        public GameObject loadingGo;
        public Text intro; 
        public override void Refresh(object data)
        {
            loadingGo.SetActive(false);
            delayOpentimer = 0;
            timeSwitch = true;
            delayCloseTrigger = false ;
             
            if (data !=null)
            {
                intro.text = data as string;
            }
            base.Refresh(data);
        }
        bool timeSwitch;
        float delayOpentimer;
        float gapTime = 2f;

        bool delayCloseTrigger;
        float delayCloseTimer;

        private void Update()
        { 
            //��ȴ�ʱ��
            if (delayCloseTrigger)
            {
                delayCloseTimer -= Time.deltaTime;
                if (delayCloseTimer < 0)
                {
                    delayCloseTrigger = false;
                    Close();
                    UIManager.ShowSystemPop("������ȴ�����ʱ��");
                }
            }
            if (!timeSwitch)
                return;
            delayOpentimer += Time.deltaTime;
            if (delayOpentimer > gapTime)
            {
                loadingGo.SetActive(true);
                timeSwitch = false;

                delayCloseTimer = 20f;
                delayCloseTrigger = true;
            }
          

        }
    }
}