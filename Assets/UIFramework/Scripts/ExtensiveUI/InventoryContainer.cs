using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastDev.UiFramework
{
    public class InventoryContainer : WidgetBase
    {

        //ʵ��һ�����߹����Ķ�����Ż�

        [SerializeField] private GameObject item, itemnContainer;

        public override void Init(object data = null)
        {
            base.Init(data);
        }

        public override void Refresh(object data = null)
        {
            base.Refresh(data);
        }

        private void ShowFilteredIntes(int index)
        {

        }

    }
}