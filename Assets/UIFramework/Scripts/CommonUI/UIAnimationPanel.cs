using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace FastDev.UiFramework
{
    public interface IFlyableUi { }
    public class UIAnimationPanel : PanelBase
    {

        public GameObject equipmentIconItem;
        public GameObject _goRoot;
        public GameObject _systemMsgItem;

        public GameObject _propMsgItem;
        public GameObject _propMsgEvolveItem;

        public GameObject goRoot { get { if (_goRoot == null) { _goRoot = gameObject; } return _goRoot; } }
        public GameObject systemMsgItem { get { if (_systemMsgItem == null) { _systemMsgItem = Resources.Load<GameObject>("PanelPrefab/Prop/systemMsgItem"); } return _systemMsgItem; } }
        public GameObject propMsgItem { get { if (_propMsgItem == null) { _propMsgItem = Resources.Load<GameObject>("PanelPrefab/Prop/propMsgBox"); } return _propMsgItem; } }
        public GameObject propMsgEvolveItem { get { if (_propMsgEvolveItem == null) { _propMsgEvolveItem = Resources.Load<GameObject>("PanelPrefab/Prop/propMsgBox_Evolve"); } return _propMsgEvolveItem; } }
   
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_content"></param>
        /// <param name="senderTrans">点击的item位置</param>
        /// <param name="containBtn">是否可以升级</param>
        /// <param name="onClickCall"></param>
        public void ShowPropShortWithBtnMsg(string _name, string _content, string addStr, Vector3 initPos, object data, Action<object> onClickCall, bool containBtn = false)
        {

            var ins = Instantiate(propMsgEvolveItem, initPos, Quaternion.identity);
            ins.transform.SetParent(goRoot.transform);
            if (containBtn)
            {
                ins.GetComponent<PropMsgBoxEvolve>().RefreshWithBtn(_name, _content, addStr, data, onClickCall);
            }
            else
            {
                ins.GetComponent<PropMsgBoxEvolve>().RefreshNoBtn(_name, _content, addStr); 
            }

            ins.gameObject.SetActive(true);
        }
        public void ShowPropShortMsg(string _name, string _content, RectTransform senderTrans)
        {
            Vector3 initPos = senderTrans.transform.position;
            initPos.y = initPos.y + (senderTrans.sizeDelta.y / 2);
            var ins = Instantiate(propMsgItem, initPos, Quaternion.identity);
            ins.transform.SetParent(goRoot.transform);
            ins.GetComponent<PropMsgBox>().Refresh(_name, _content);

            ins.gameObject.SetActive(true);

        }

        //飞行icon 
        public IFlyableUi MovePropItem(RectTransform start, RectTransform end, Action onFinished, float moveSpeed = 80f)
        {
            var ins = Instantiate(equipmentIconItem, start.transform.position, Quaternion.identity);
            ins.GetComponent<UIAnimationMovement>().Init(start, end, moveSpeed, onFinished);
            ins.transform.parent = goRoot.transform;
            ins.gameObject.SetActive(true);

            return ins.GetComponent<IFlyableUi>();
        }

        //散落飞行动画
        //public void ShowGetRewardFlyAnimation(List<PlayerPropData> lst)
        //{
        //    return;
        //    if (rewardAnimator == null)
        //    {
        //        rewardAnimator = Instantiate(rewardAnimatorPrefab, goRoot.transform);
        //    }

        //    rewardAnimator.GetComponent<RewardAnimatior>().Show(lst);

        //}

        public void ShowSystemMsg(string str)
        {
            if (isShowing && str == lastMsg)
                return;

            var ins = Instantiate(systemMsgItem, goRoot.transform); 
            ins.GetComponent<SystemMsgItem>().Init(str);

            ins.gameObject.SetActive(true);

            isShowing = true;
            systemMsgtimer = 0;
            lastMsg = str;

        }

        bool isShowing = false;
        string lastMsg = "";
        float systemMsgtimer = 0;
        private void Update()
        {
            if (!isShowing)
                return;
            systemMsgtimer += Time.deltaTime;
            if (systemMsgtimer > 0.1)
            {
                isShowing = false;
            }
        }
    }
}