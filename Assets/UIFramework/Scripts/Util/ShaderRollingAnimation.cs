using UnityEngine;
using UnityEngine.UI;
namespace FastDev.UiFramework
{
	public class ShaderRollingAnimation : MonoBehaviour
	{
        private Material mat;
        private Vector2 originOff;
        private void OnEnable()
        {
            mat = GetComponent<Image>().material;
            offseter = 0;
            if (mat != null)
            {
                originOff = GetComponent<Image>().material.mainTextureOffset;
            }

        }
        float offseter = 0;
        void Update()
        {
            if (mat != null)
            {
                offseter += Time.deltaTime / 10 * 0.8f;
                //mat.mainTextureOffset = new Vector2(0, 0);
                mat.mainTextureOffset = new Vector2(-offseter, -offseter) + originOff;
                //Debug.Log(Time.deltaTime);
                if (offseter > 60000.0) { offseter = 0; }
            }
            else
            {
            }
        }

    }
}