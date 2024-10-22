 
namespace UIUtils
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Serialization;
    using UnityEngine.UI;

    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class ScrollView : ScrollRect
    {
        [Tooltip("Ĭ��item�ߴ�")]
        public Vector2 defaultItemSize;

        [Tooltip("item��ģ��")]
        public RectTransform itemTemplate;

        public float buttomOffset = 0;
        // 0001
        protected const int flagScrollDirection = 1;

        [SerializeField]
        [FormerlySerializedAs("m_layoutType")]
        protected ItemLayoutType layoutType = ItemLayoutType.Vertical;

        // ֻ����4���ٽ�index
        protected int[] criticalItemIndex = new int[4];

        // callbacks for items
        protected Action<int, RectTransform> updateFunc;
        protected Func<int, Vector2> itemSizeFunc;
        protected Func<int> itemCountFunc;
        protected Func<int, RectTransform> itemGetFunc;
        protected Action<RectTransform> itemRecycleFunc;

        private readonly List<ScrollItemWithRect> managedItems = new List<ScrollItemWithRect>();

        private Rect refRect;

        // resource management
        private SimpleObjPool<RectTransform> itemPool = null;

        private int dataCount = 0;

        [Tooltip("��ʼ��ʱ����item����")]
        [SerializeField]
        private int poolSize;

        // status
        private bool initialized = false;
        private int willUpdateData = 0;

        private Vector3[] viewWorldConers = new Vector3[4];
        private Vector3[] rectCorners = new Vector3[2];

        private bool applicationIsQuitting;

        // for hide and show
        public enum ItemLayoutType
        {
            // ���һλ��ʾ��������
            Vertical = 0b0001,                   // 0001
            Horizontal = 0b0010,                 // 0010
            VerticalThenHorizontal = 0b0100,     // 0100
            HorizontalThenVertical = 0b0101,     // 0101
        }

        public virtual void SetUpdateFunc(Action<int, RectTransform> func)
        {
            this.updateFunc = func;
        }

        public virtual void SetItemSizeFunc(Func<int, Vector2> func)
        {
            this.itemSizeFunc = func;
        }

        public virtual void SetItemCountFunc(Func<int> func)
        {
            this.itemCountFunc = func;
        }

        public void SetItemGetAndRecycleFunc(Func<int, RectTransform> getFunc, Action<RectTransform> recycleFunc)
        {
            if (getFunc != null && recycleFunc != null)
            {
                this.itemGetFunc = getFunc;
                this.itemRecycleFunc = recycleFunc;
            }
            else
            {
                this.itemGetFunc = null;
                this.itemRecycleFunc = null;
            }
        }

        public void ResetAllDelegates()
        {
            this.SetUpdateFunc(null);
            this.SetItemSizeFunc(null);
            this.SetItemCountFunc(null);
            this.SetItemGetAndRecycleFunc(null, null);
        }

        public void UpdateData(bool immediately = true)
        {
            if (immediately)
            {
                this.willUpdateData |= 3; // 0011
                this.InternalUpdateData();
            }
            else
            {
                if (this.willUpdateData == 0 && this.IsActive())
                {
                    this.StartCoroutine(this.DelayUpdateData());
                }

                this.willUpdateData |= 3;
            }
        }

        public void UpdateDataIncrementally(bool immediately = true)
        {
            if (immediately)
            {
                this.willUpdateData |= 1; // 0001
                this.InternalUpdateData();
            }
            else
            {
                if (this.willUpdateData == 0)
                {
                    this.StartCoroutine(this.DelayUpdateData());
                }

                this.willUpdateData |= 1;
            }
        }

        public void ScrollTo(int index)
        {
            this.InternalScrollTo(index);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (this.willUpdateData != 0)
            {
                this.StartCoroutine(this.DelayUpdateData());
            }
        }

        protected override void OnDisable()
        {
            this.initialized = false;
            base.OnDisable();
        }

        protected virtual void InternalScrollTo(int index)
        {
         
            index = Mathf.Clamp(index, 0, this.dataCount - 1);
            if (index < 0)
                return;
            if (managedItems.Count <= 0)
                return;
            if (managedItems.Count <= index)
                return;
            
            this.EnsureItemRect(index);
            Rect r = this.managedItems[index].rect;
            var dir = (int)this.layoutType & flagScrollDirection;
            if (dir == 1)
            {
                // vertical
                var value = 1 - (-r.yMax / (this.content.sizeDelta.y - this.refRect.height));
                value = Mathf.Clamp01(value);
                this.SetNormalizedPosition(value, 1);
            }
            else
            {
                // horizontal
                var value = r.xMin / (this.content.sizeDelta.x - this.refRect.width);
                value = Mathf.Clamp01(value);
                this.SetNormalizedPosition(value, 0);
            }
        }

        protected override void SetContentAnchoredPosition(Vector2 position)
        {
            base.SetContentAnchoredPosition(position);

            if (this.willUpdateData != 0)
            {
                return;
            }

            this.UpdateCriticalItems();
        }

        protected override void SetNormalizedPosition(float value, int axis)
        {
            base.SetNormalizedPosition(value, axis);

            if (this.willUpdateData != 0)
            {
                return;
            }

            this.ResetCriticalItems();
        }

        protected void EnsureItemRect(int index)
        {
            if (index < 0)
                return;
            if (managedItems.Count <= 0)
                return; 
            if (managedItems.Count <= index)
                return;
            //Debug.LogError($"index {index},conmt:{managedItems.Count}");
            if (!this.managedItems[index].rectDirty)
            {
                // �Ѿ��Ǹɾ�����
                return;
            }

            ScrollItemWithRect firstItem = this.managedItems[0];
            if (firstItem.rectDirty)
            {
                Vector2 firstSize = this.GetItemSize(0);
                firstItem.rect = CreateWithLeftTopAndSize(Vector2.zero, firstSize);
                firstItem.rectDirty = false;
            }

            // ��ǰitem֮ǰ��������Ѹ��µ�rect
            var nearestClean = 0;
            for (var i = index; i >= 0; --i)
            {
                if (!this.managedItems[i].rectDirty)
                {
                    nearestClean = i;
                    break;
                }
            }

            // ��Ҫ���� �� nearestClean �� index �ĳߴ�
            Rect nearestCleanRect = this.managedItems[nearestClean].rect;
            Vector2 curPos = GetLeftTop(nearestCleanRect);
            Vector2 size = nearestCleanRect.size;
            this.MovePos(ref curPos, size);

            for (var i = nearestClean + 1; i <= index; i++)
            {
                size = this.GetItemSize(i);
                this.managedItems[i].rect = CreateWithLeftTopAndSize(curPos, size);
                this.managedItems[i].rectDirty = false;
                this.MovePos(ref curPos, size);
            }

            var range = new Vector2(Mathf.Abs(curPos.x), Mathf.Abs(curPos.y));
            switch (this.layoutType)
            {
                case ItemLayoutType.VerticalThenHorizontal:
                    range.x += size.x;
                    range.y = this.refRect.height;
                    break;
                case ItemLayoutType.HorizontalThenVertical:
                    range.x = this.refRect.width;
                    if (curPos.x != 0)
                    {
                        range.y += size.y;
                    }

                    break;
                default:
                    break;
            }
            range.y += buttomOffset;
            //Debug.LogError(range);
            this.content.sizeDelta = range;
        }

        protected override void OnDestroy()
        {
            if (this.itemPool != null)
            {
                this.itemPool.Purge();
            }
        }

        protected Rect GetItemLocalRect(int index)
        {
            if (index >= 0 && index < this.dataCount)
            {
                this.EnsureItemRect(index);
                return this.managedItems[index].rect;
            }

            return (Rect)default;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            var dir = (int)this.layoutType & flagScrollDirection;
            if (dir == 1)
            {
                // vertical
                if (this.horizontalScrollbar != null)
                {
                    this.horizontalScrollbar.gameObject.SetActive(false);
                    this.horizontalScrollbar = null;
                }
            }
            else
            {
                // horizontal
                if (this.verticalScrollbar != null)
                {
                    this.verticalScrollbar.gameObject.SetActive(false);
                    this.verticalScrollbar = null;
                }
            }

            base.OnValidate();
        }
#endif

        private static Vector2 GetLeftTop(Rect rect)
        {
            Vector2 ret = rect.position;
            ret.y += rect.size.y;
            return ret;
        }

        private static Rect CreateWithLeftTopAndSize(Vector2 leftTop, Vector2 size)
        {
            Vector2 leftBottom = leftTop - new Vector2(0, size.y);
            return new Rect(leftBottom, size);
        }

        private IEnumerator DelayUpdateData()
        {
            yield return new WaitForEndOfFrame();

            this.InternalUpdateData();
        }

        private void InternalUpdateData()
        {
            if (!this.IsActive())
            {
                this.willUpdateData |= 3;
                return;
            }

            if (!this.initialized)
            {
                this.InitScrollView();
            }

            this.CheckDataCountChange();

            this.ResetCriticalItems();

            this.willUpdateData = 0;
        }

        protected virtual void CheckDataCountChange()
        {
            var newDataCount = 0;

            if (this.itemCountFunc != null)
            {
                newDataCount = this.itemCountFunc();
            }

            var keepOldItems = (this.willUpdateData & 2) == 0;

            if (newDataCount != this.managedItems.Count)
            {
                if (this.managedItems.Count < newDataCount)
                {
                    // ����
                    if (!keepOldItems)
                    {
                        foreach (var itemWithRect in this.managedItems)
                        {
                            // ��������rect
                            itemWithRect.rectDirty = true;
                        }
                    }

                    while (this.managedItems.Count < newDataCount)
                    {
                        this.managedItems.Add(new ScrollItemWithRect());
                    }
                }
                else
                {
                    // ���� ������λ ����GC
                    for (int i = 0, count = this.managedItems.Count; i < count; ++i)
                    {
                        if (i < newDataCount)
                        {
                            // ��������rect
                            if (!keepOldItems)
                            {
                                this.managedItems[i].rectDirty = true;
                            }

                            if (i == newDataCount - 1)
                            {
                                this.managedItems[i].rectDirty = true;
                            }
                        }

                        // �������� �������item
                        if (i >= newDataCount)
                        {
                            this.managedItems[i].rectDirty = true;
                            if (this.managedItems[i].item != null)
                            {
                                this.RecycleOldItem(this.managedItems[i].item);
                                this.managedItems[i].item = null;
                            }
                        }
                    }
                }
            }
            else
            {
                if (!keepOldItems)
                {
                    for (int i = 0, count = this.managedItems.Count; i < count; ++i)
                    {
                        // ��������rect
                        this.managedItems[i].rectDirty = true;
                    }
                }
            }

            this.dataCount = newDataCount;
        }

        private void ResetCriticalItems()
        {
            bool hasItem, shouldShow;
            int firstIndex = -1, lastIndex = -1;

            for (var i = 0; i < this.dataCount; i++)
            {
                hasItem = this.managedItems[i].item != null;
                shouldShow = this.ShouldItemSeenAtIndex(i);

                if (shouldShow)
                {
                    if (firstIndex == -1)
                    {
                        firstIndex = i;
                    }

                    lastIndex = i;
                }

                if (hasItem && shouldShow)
                {
                    // Ӧ��ʾ������ʾ
                    this.SetDataForItemAtIndex(this.managedItems[i].item, i);
                    continue;
                }

                if (hasItem == shouldShow)
                {
                    // ��Ӧ��ʾ��δ��ʾ
                    // if (firstIndex != -1)
                    // {
                    //     // �Ѿ�����������Ҫ��ʾ���� ��ߵ�������
                    //     break;
                    // }
                    continue;
                }

                if (hasItem && !shouldShow)
                {
                    // ������ʾ ������
                    this.RecycleOldItem(this.managedItems[i].item);
                    this.managedItems[i].item = null;
                    continue;
                }

                if (shouldShow && !hasItem)
                {
                    // ��Ҫ��ʾ ����û��
                    RectTransform item = this.GetNewItem(i);
                    this.OnGetItemForDataIndex(item, i);
                    this.managedItems[i].item = item;
                    continue;
                }
            }

            // content.localPosition = Vector2.zero;
            this.criticalItemIndex[CriticalItemType.UpToHide] = firstIndex;
            this.criticalItemIndex[CriticalItemType.DownToHide] = lastIndex;
            this.criticalItemIndex[CriticalItemType.UpToShow] = Mathf.Max(firstIndex - 1, 0);
            this.criticalItemIndex[CriticalItemType.DownToShow] = Mathf.Min(lastIndex + 1, this.dataCount - 1);
        }

        private RectTransform GetCriticalItem(int type)
        {
            var index = this.criticalItemIndex[type];
            if (index >= 0 && index < this.dataCount)
            {
                return this.managedItems[index].item;
            }

            return null;
        }

        private void UpdateCriticalItems()
        {
            var dirty = true;

            while (dirty)
            {
                dirty = false;

                for (int i = CriticalItemType.UpToHide; i <= CriticalItemType.DownToShow; i++)
                {
                    if (i <= CriticalItemType.DownToHide)
                    {
                        // �����뿪�ɼ������item
                        dirty = dirty || this.CheckAndHideItem(i);
                    }
                    else
                    {
                        // ��ʾ����ɼ������item
                        dirty = dirty || this.CheckAndShowItem(i);
                    }
                }
            }
        }

        private bool CheckAndHideItem(int criticalItemType)
        {
            RectTransform item = this.GetCriticalItem(criticalItemType);
            var criticalIndex = this.criticalItemIndex[criticalItemType];
            if (item != null && !this.ShouldItemSeenAtIndex(criticalIndex))
            {
                this.RecycleOldItem(item);
                this.managedItems[criticalIndex].item = null;

                if (criticalItemType == CriticalItemType.UpToHide)
                {
                    // ����������һ��
                    this.criticalItemIndex[criticalItemType + 2] = Mathf.Max(criticalIndex, this.criticalItemIndex[criticalItemType + 2]);
                    this.criticalItemIndex[criticalItemType]++;
                }
                else
                {
                    // ����������һ��
                    this.criticalItemIndex[criticalItemType + 2] = Mathf.Min(criticalIndex, this.criticalItemIndex[criticalItemType + 2]);
                    this.criticalItemIndex[criticalItemType]--;
                }

                this.criticalItemIndex[criticalItemType] = Mathf.Clamp(this.criticalItemIndex[criticalItemType], 0, this.dataCount - 1);

                if (this.criticalItemIndex[CriticalItemType.UpToHide] > this.criticalItemIndex[CriticalItemType.DownToHide])
                {
                    // żȻ����� ��ק����һ��
                    this.ResetCriticalItems();
                    return false;
                }

                return true;
            }

            return false;
        }

        private bool CheckAndShowItem(int criticalItemType)
        {
            RectTransform item = this.GetCriticalItem(criticalItemType);
            var criticalIndex = this.criticalItemIndex[criticalItemType];

            if (item == null && this.ShouldItemSeenAtIndex(criticalIndex))
            {
                RectTransform newItem = this.GetNewItem(criticalIndex);
                this.OnGetItemForDataIndex(newItem, criticalIndex);
                this.managedItems[criticalIndex].item = newItem;

                if (criticalItemType == CriticalItemType.UpToShow)
                {
                    // ������ʾ��һ��
                    this.criticalItemIndex[criticalItemType - 2] = Mathf.Min(criticalIndex, this.criticalItemIndex[criticalItemType - 2]);
                    this.criticalItemIndex[criticalItemType]--;
                }
                else
                {
                    // ������ʾ��һ��
                    this.criticalItemIndex[criticalItemType - 2] = Mathf.Max(criticalIndex, this.criticalItemIndex[criticalItemType - 2]);
                    this.criticalItemIndex[criticalItemType]++;
                }

                this.criticalItemIndex[criticalItemType] = Mathf.Clamp(this.criticalItemIndex[criticalItemType], 0, this.dataCount - 1);

                if (this.criticalItemIndex[CriticalItemType.UpToShow] >= this.criticalItemIndex[CriticalItemType.DownToShow])
                {
                    // żȻ����� ��ק����һ��
                    this.ResetCriticalItems();
                    return false;
                }

                return true;
            }

            return false;
        }

        private bool ShouldItemSeenAtIndex(int index)
        {
            if (index < 0 || index >= this.dataCount)
            {
                return false;
            }

            this.EnsureItemRect(index);
            return new Rect(this.refRect.position - this.content.anchoredPosition, this.refRect.size).Overlaps(this.managedItems[index].rect);
        }

        private bool ShouldItemFullySeenAtIndex(int index)
        {
            if (index < 0 || index >= this.dataCount)
            {
                return false;
            }

            this.EnsureItemRect(index);
            return this.IsRectContains(new Rect(this.refRect.position - this.content.anchoredPosition, this.refRect.size), this.managedItems[index].rect);
        }

        private bool IsRectContains(Rect outRect, Rect inRect, bool bothDimensions = false)
        {
            if (bothDimensions)
            {
                var xContains = (outRect.xMax >= inRect.xMax) && (outRect.xMin <= inRect.xMin);
                var yContains = (outRect.yMax >= inRect.yMax) && (outRect.yMin <= inRect.yMin);
                return xContains && yContains;
            }
            else
            {
                var dir = (int)this.layoutType & flagScrollDirection;
                if (dir == 1)
                {
                    // ��ֱ���� ֻ����y��
                    return (outRect.yMax >= inRect.yMax) && (outRect.yMin <= inRect.yMin);
                }
                else
                {
                    // = 0
                    // ˮƽ���� ֻ����x��
                    return (outRect.xMax >= inRect.xMax) && (outRect.xMin <= inRect.xMin);
                }
            }
        }

        private void InitPool()
        {
            var poolNode = new GameObject("SCROLL_POOL");
            poolNode.SetActive(false);
            poolNode.transform.SetParent(this.transform, false);
            this.itemPool = new SimpleObjPool<RectTransform>(
                this.poolSize,
                (RectTransform item) =>
                {
                    item.transform.SetParent(poolNode.transform, false);
                },
                () =>
                {
                    GameObject itemObj = Instantiate(this.itemTemplate.gameObject);
                    RectTransform item = itemObj.GetComponent<RectTransform>();
                    itemObj.transform.SetParent(poolNode.transform, false);

                    item.anchorMin = Vector2.up;
                    item.anchorMax = Vector2.up;
                    item.pivot = Vector2.zero;

                    itemObj.SetActive(true);
                    return item;
                },
                (RectTransform item) =>
                {
                    if (!applicationIsQuitting)
                    {
                        item.transform.SetParent(null, false);
                        Destroy(item.gameObject);
                    }
                });
        }

        private void OnGetItemForDataIndex(RectTransform item, int index)
        {
            this.SetDataForItemAtIndex(item, index);
            item.transform.SetParent(this.content, false);
        }

        private void SetDataForItemAtIndex(RectTransform item, int index)
        {
            if (this.updateFunc != null)
            {
                this.updateFunc(index, item);
            }

            this.SetPosForItemAtIndex(item, index);
        }

        private void SetPosForItemAtIndex(RectTransform item, int index)
        {
            this.EnsureItemRect(index);
            Rect r = this.managedItems[index].rect;
            item.localPosition = r.position;
            item.sizeDelta = r.size;
        }

        private Vector2 GetItemSize(int index)
        {
            if (index >= 0 && index <= this.dataCount)
            {
                if (this.itemSizeFunc != null)
                {
                    return this.itemSizeFunc(index);
                }
            }

            return this.defaultItemSize;
        }

        private RectTransform GetNewItem(int index)
        {
            RectTransform item;
            if (this.itemGetFunc != null)
            {
                item = this.itemGetFunc(index);
            }
            else
            {
                item = this.itemPool.Get();
            }

            return item;
        }

        private void RecycleOldItem(RectTransform item)
        {
            if (this.itemRecycleFunc != null)
            {
                this.itemRecycleFunc(item);
            }
            else
            {
                this.itemPool.Recycle(item);
            }
        }

        private void InitScrollView()
        {
            this.initialized = true;

            // ��������������ԭScrollRect�Ĺ�������
            var dir = (int)this.layoutType & flagScrollDirection;
            this.vertical = dir == 1;
            this.horizontal = dir == 0;

            this.content.pivot = Vector2.up;
            this.content.anchorMin = Vector2.up;
            this.content.anchorMax = Vector2.up;
            this.content.anchoredPosition = Vector2.zero;

            this.InitPool();
            this.UpdateRefRect();
        }

        // refRect����Content�ڵ��µ� viewport�� rect
        private void UpdateRefRect()
        {
            /*
             *  WorldCorners
             *
             *    1 ------- 2
             *    |         |
             *    |         |
             *    0 ------- 3
             *
             */

            if (!CanvasUpdateRegistry.IsRebuildingLayout())
            {
                Canvas.ForceUpdateCanvases();
            }

            this.viewRect.GetWorldCorners(this.viewWorldConers);
            this.rectCorners[0] = this.content.transform.InverseTransformPoint(this.viewWorldConers[0]);
            this.rectCorners[1] = this.content.transform.InverseTransformPoint(this.viewWorldConers[2]);
            this.refRect = new Rect((Vector2)this.rectCorners[0] - this.content.anchoredPosition, this.rectCorners[1] - this.rectCorners[0]);
        }

        private void MovePos(ref Vector2 pos, Vector2 size)
        {
            // ע�� ���е�rect�������½�Ϊ��׼
            switch (this.layoutType)
            {
                case ItemLayoutType.Vertical:
                    // ��ֱ���� �����ƶ�
                    pos.y -= size.y;
                    break;
                case ItemLayoutType.Horizontal:
                    // ˮƽ���� �����ƶ�
                    pos.x += size.x;
                    break;
                case ItemLayoutType.VerticalThenHorizontal:
                    pos.y -= size.y;
                    if (pos.y - size.y < -this.refRect.height)
                    {
                        pos.y = 0;
                        pos.x += size.x;
                    }

                    break;
                case ItemLayoutType.HorizontalThenVertical:

                    pos.x += size.x;
                    if (pos.x + size.x > this.refRect.width)
                    {
                        pos.x = 0;
                        pos.y -= size.y;
                    }

                    break;
                default:
                    break;
            }
        }

        private void OnApplicationQuit()
        {
            applicationIsQuitting = true;
        }

        // const int ���� enum ���� (int)��(CriticalItemType)ת��
        protected static class CriticalItemType
        {
            public static byte UpToHide = 0;
            public static byte DownToHide = 1;
            public static byte UpToShow = 2;
            public static byte DownToShow = 3;
        }

        private class ScrollItemWithRect
        {
            // scroll item ���ϵ� RectTransform���
            public RectTransform item;

            // scroll item ��scrollview�е�λ��
            public Rect rect;

            // rect �Ƿ���Ҫ����
            public bool rectDirty = true;
        }
    }
}
