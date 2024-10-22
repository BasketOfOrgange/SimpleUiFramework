
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FastDev.UiFramework
{
    public class ChiocePanelData
    {

        public string content;
        public string title;
        public string okString;
        public string noString;

        public bool isTip;
        public UnityAction okDelegate;
        public UnityAction noDelegate;

        public ChiocePanelData(string content, UnityAction okAc, UnityAction noAc = null)
        {
            this.content = content;
            this.okDelegate = okAc;
            this.noDelegate = noAc;
        }

        public ChiocePanelData(string content, string title, string okString, string noString, bool isTip, UnityAction okDelegate, UnityAction noDelegate)
        {
            this.content = content;
            this.title = title;
            this.okString = okString;
            this.noString = noString;
            this.isTip = isTip;
            this.okDelegate = okDelegate;
            this.noDelegate = noDelegate;
        }
    }
    public class ChoicePanel : PanelBase
    {
        public Text contentTxt;
        public Text titleTxt;
        public Text okTxt;
        public Text noTxt;
        public Button okBtn;
        public Button noBtn;
        public Button closeBtn;

        public override void Refresh(object data)
        {
            ChiocePanelData d = data as ChiocePanelData;


            contentTxt.text = d.content;
            if (!string.IsNullOrEmpty(d.title))
            {
                titleTxt.text = d.title;
            }
            if (!string.IsNullOrEmpty(d.okString))
            {
                okTxt.text = d.okString;
            }
            if (!string.IsNullOrEmpty(d.noString))
            {
                noTxt.text = d.noString;
            }


            okBtn.onClick.RemoveAllListeners();
            okBtn.onClick.AddListener(d.okDelegate);

            noBtn.onClick.RemoveAllListeners();
            closeBtn.onClick.RemoveAllListeners();
            if (d.noDelegate != null)
                noBtn.onClick.AddListener(d.noDelegate);

            okBtn.onClick.AddListener(() => { Close(); });
            noBtn.onClick.AddListener(() => { Close(); });
            closeBtn.onClick.AddListener(() => { Close(); });
            base.Refresh(data);
        }

        public override void Close(bool immidiately = false)
        {
            okBtn.transform.localScale = Vector3.one;
            noBtn.transform.localScale = Vector3.one;
            base.Close(immidiately);
        }
    }
}