using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastDev.UiFramework

{
    public class TipPanel : PanelBase
{


    public override void Refresh(object data)
    {
        string str = data as string;


        base.Refresh(data);
    }


}
}