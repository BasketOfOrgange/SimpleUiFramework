using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastDev.UiFramework
{
    public class ItemWithPoolBase : MonoBehaviour
    {
        public GameObject? item, itemContainer;

        protected List<GameObject> _pool;
        protected List<GameObject> Pool
        {
            get
            {
                if (_pool == null)
                    _pool = new List<GameObject>();
                return _pool;
            }
        }

        protected virtual void ClearPool()
        {
            Pool.ForEach(t => t.gameObject.SetActive(false));
            if (item != null)
            {
                item.SetActive(false);
            }
        }
        protected GameObject LoadOneItemInPool(bool isSetTrue = false)
        {
            foreach (var i in Pool)
            {
                if (i.activeSelf == false)
                {
                    if (isSetTrue)
                    {
                        i.SetActive(true);
                    }
                    return i;
                }
            }

            var ins = Instantiate(item, itemContainer.transform);
            Pool.Add(ins);
            if (isSetTrue)
            {
                ins.SetActive(true);
            }
            return ins;

        }


    }
}