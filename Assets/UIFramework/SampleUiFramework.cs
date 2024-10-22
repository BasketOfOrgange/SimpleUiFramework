using FastDev.UiFramework;
using UnityEngine;
public class SampleUiFramework : MonoBehaviour
{
    public void ShowIntro()
    {
        UIManager.GetUIAnimationPanel().ShowPropShortMsg("牛批", "牛批人牛批公司", GameObject.Find("mm").GetComponent<RectTransform>());
    }
    public void ShowIntro2()
    {
        UIManager.GetUIAnimationPanel().ShowPropShortWithBtnMsg("牛批", "牛批人牛批公司", "牛批+1", GameObject.Find("xx").transform.position, null, null);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { UIManager.OpenLoading(); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { UIManager.CloseLoading(); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { UIManager.ShowSystemPop("你好牛批啊"); }
    }
}