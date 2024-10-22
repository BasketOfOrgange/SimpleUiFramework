using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconStringPairList : MonoBehaviour
{
    public struct SpriteStringPair
    {
        public Sprite sp;
        public string content;
    }
     
    public void Clear()
    {
        ClearPool();
    }
    public void Refresh(List<SpriteStringPair> lst )
    {
        ClearPool();
        foreach (var i in lst)
        {
            var ins = LoadOneItemInPool();
            ins.gameObject.SetActive(true);
            ins.GetComponent<IconStringPair>().Refresh(i.sp,i.content); 
        }

    }

    #region pool
    public GameObject? item, itemContainer;

    private List<GameObject> _pool;
    private List<GameObject> Pool
    {
        get
        {
            if (_pool == null)
                _pool = new List<GameObject>();
            return _pool;
        }
    }

    protected void ClearPool()
    {
        Pool.ForEach(t => t.gameObject.SetActive(false));
        if (item != null)
        {
            item.SetActive(false);
        }
    }
    protected GameObject LoadOneItemInPool()
    {
        foreach (var i in Pool)
        {
            if (i.activeSelf == false)
            {
                return i;
            }
        }

        var ins = Instantiate(item, itemContainer.transform);
        Pool.Add(ins);
        return ins;

    }

#endregion
}
