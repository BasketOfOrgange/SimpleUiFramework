using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UIElements;
using UnityEngine.Events;
using Button = UnityEngine.UI.Button;
namespace FastDev.UiFramework
{
    [RequireComponent(typeof(UnityEngine.UI.Button))]
    public class ButtonClickMaintainTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private float pressStartTime;
        private bool OnTrigger { get; set; }
        private float timer;
        private Action call;
        private UnityAction onDwoncall, onUpCall, onUpCallWhenTriggeredCall;

        private bool IsActive = false;
        public void SetActive(bool a)
        {
            IsActive = a;
        }
        public void Init(float gap_timer, Action callWhenTimerTrigge, UnityAction onDown = null, UnityAction onUp = null, UnityAction onUpCallWhenTriggered = null)
        {
            OnTrigger = false;
            pressStartTime = gap_timer;
            call = callWhenTimerTrigge;
            onDwoncall = onDown;
            onUpCall = onUp;
            onUpCallWhenTriggeredCall = onUpCallWhenTriggered;
        }

        private bool OnFirstCallTrigger;//第一次执行连续点击
        private void Update()
        {
            if (IsActive == false)
                return;
            if (OnTrigger)
            {
                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    if (OnFirstCallTrigger == false)
                    {
                        onDwoncall?.Invoke();
                        OnFirstCallTrigger = true;

                    }
                    timer = pressStartTime;
                    call?.Invoke();
                    Debug.Log("call timer");

                }
            }
            //Debug.LogError(OnTrigger + " " + timer);
        }



        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            OnTrigger = false;
            timer = pressStartTime;
            Debug.Log("up");
            //触发第一次后才会up
            if (OnFirstCallTrigger)
                onUpCallWhenTriggeredCall?.Invoke();

            onUpCall?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            timer = pressStartTime;
            OnTrigger = true;
            OnFirstCallTrigger = false;
            Debug.Log("down");
        }
    }
}