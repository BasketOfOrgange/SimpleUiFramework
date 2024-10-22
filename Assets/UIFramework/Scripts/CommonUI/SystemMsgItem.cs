using UnityEngine;
using UnityEngine.UI;


namespace FastDev.UiFramework
{
    public class SystemMsgItem : MonoBehaviour
    {
        public Text txt;
        Vector3 targetPos;
        private float speed=10f;
        public void Init(string content)
        {
            targetPos = transform.position + new Vector3(0, 300, 0);
            transform.localScale = Vector3.one * 0.5f;

            txt.text = content;
            Destroy(gameObject, 2f);
        }

        void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed);// transform.position + new Vector3(0,1,0);

            if (transform.localScale.x < 1)
            {
                Vector3 next = transform.localScale + new Vector3(0.05f, 0.05f, 0.05f);
                transform.localScale = next;
            }
        }
    }
}