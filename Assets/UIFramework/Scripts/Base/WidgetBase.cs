using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastDev.UiFramework
{
    public class WidgetBase : MonoBehaviour
{

    //支持空刷
    public virtual void Init(object data = null)
    {

    }
    //支持空刷    /// 在打开的前提（已经保存数据）下空刷新，
    public virtual void Refresh(object data = null)
    {

    }



}
}