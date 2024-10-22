using FastDev.UiFramework;
using UnityEngine;
public class SampleUiFramework : MonoBehaviour
{
    public void ShowIntro()
    {
        UIManager.GetUIAnimationPanel().ShowPropShortMsg("ţ��", "ţ����ţ����˾", GameObject.Find("mm").GetComponent<RectTransform>());
    }
    public void ShowIntro2()
    {
        UIManager.GetUIAnimationPanel().ShowPropShortWithBtnMsg("ţ��", "ţ����ţ����˾", "ţ��+1", GameObject.Find("xx").transform.position, null, null);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { UIManager.OpenLoading(); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { UIManager.CloseLoading(); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { UIManager.ShowSystemPop("���ţ����"); }
    }
}