using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class ItemSelectEvent : UnityEvent<ListView.ListViewItem>//触发时会传递一个 ListView.ListViewItem 类型的参数
                                         //列表项的内部类
{

}

public class ListView : MonoBehaviour
{
    //UnityEvent可在Inspector中可视化配置
    public UnityAction<ListViewItem> onItemSelected;// 公开的事件委托
    public class ListViewItem : MonoBehaviour, IPointerClickHandler//响应指针点击事件
    {
        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                onSelected(selected);
            }
        }
        public virtual void onSelected(bool selected)
        {

        }

        public ListView owner;// 主列表视图类 管理多个 ListViewItem

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!this.selected)
            {
                this.Selected = true;
            }
            if (owner != null && owner.SelectedItem != this)//委托不为空，则调用该委托并传入新的选中项。
            {
                owner.SelectedItem = this;
            }
        }
    }

    List<ListViewItem> items = new List<ListViewItem>();

    private ListViewItem selectedItem = null;
    public ListViewItem SelectedItem
    {
        get { return selectedItem; }
        private set
        {
            if (selectedItem != null && selectedItem != value)
            {
                selectedItem.Selected = false;
            }
            selectedItem = value;
            if (onItemSelected != null)
                onItemSelected.Invoke((ListViewItem)value);
        }
    }

    public void AddItem(ListViewItem item)
    {
        item.owner = this;
        this.items.Add(item);
    }

    public void RemoveAll()
    {
        foreach (var it in items)
        {
            Destroy(it.gameObject);
        }
        items.Clear();
    }
}
