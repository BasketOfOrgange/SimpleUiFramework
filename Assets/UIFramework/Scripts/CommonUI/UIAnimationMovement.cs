using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastDev.UiFramework
{
    public class UIAnimationMovement : MonoBehaviour
    {
        private bool isStart = false;
        Vector3 startPos, endPos;
        float moveTime;
        Action call;

        public void Init(RectTransform start, RectTransform end, float time, Action onfinish)
        {
            isStart = false;

            startPos = start.position;
            endPos = end.position;
            moveTime = time;
            call = onfinish;

            isStart = true;

            //if (transform.localScale != Vector3.one)
            //    transform.localScale = Vector3.one;
        }
        float delayKillTime;
        public void Init(Vector3 start, Vector3 end, float time, Action onfinish, float delayKill = 0f)
        {
            isStart = false;

            startPos = start;
            endPos = end;
            moveTime = time;
            call = onfinish;
            delayKillTime = delayKill;
            isStart = true;

            //if (transform.localScale != Vector3.one)
            //    transform.localScale = Vector3.one;
        }
        private void Update()
        {
            if (isStart)
            {
                //transform.position = Vector3.Lerp(transform.position, endPos, moveTime*Time.deltaTime);
                transform.position = Vector3.MoveTowards(transform.position, endPos, moveTime);
                if (Vector3.Distance(transform.position, endPos) < 0.1f)
                {
                    call?.Invoke();
                    isStart = false;
                    if (delayKillTime > 0)
                        Invoke("DelayKill", delayKillTime);
                    else
                        Destroy(this.gameObject);
                }
            }
        }
        private void FixedUpdate()
        {

        }

        void DelayKill()
        {
            Destroy(this.gameObject);
        }
    }
}