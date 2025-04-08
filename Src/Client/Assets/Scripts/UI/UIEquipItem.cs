using System;
using Managers;
using Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class UIEquipItem: MonoBehaviour,IPointerClickHandler //指针点击处理器
    {
        public Image icon;
        public Text title;
        public Text level;
        public Text limitClass;
        public Text LimitCategory;

        public Image background;
        public Sprite normalBg;
        public Sprite selectedBg;

        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                this.background.overrideSprite = selected ? selectedBg : normalBg;
            }
        }
        
        public int index { get; set; }
        private UICharEquip owner;

        private Item item;

        private bool isEquiped = false;

        public void SetEquipItem(int idx, Item item,UICharEquip owner,bool equiped)
        {
            this.owner = owner;
            this.index = idx;
            this.item = item;
            this.isEquiped = equiped;
            if (this.title != null) this.title.text = this.item.Define.Name;
            if (this.level != null) this.level.text = item.Define.Level.ToString();
            if (this.limitClass != null) this.limitClass.text = item.Define.LimitClass.ToString();
            if (this.LimitCategory != null) this.LimitCategory.text = item.Define.Category;
            if (this.icon != null) this.icon.overrideSprite = Resloader.Load < Sprite >(this.item.Define.Icon);
            
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (this.isEquiped) //装备了  就卸下
            {
                UnEquip();
            }
            else
            {
                if (this.selected)//选择状态
                {
                    DoEquip();//装备
                    this.Selected = false;//取消选择
                }
                else//非选择状态
                {
                    this.Selected = true;//设置为选择状态
                }
            }
        }

        private void DoEquip()
        {
            var msg = MessageBox.Show(string.Format("Darling~ 确定要装备 [{0}] 吗？", this.item.Define.Name), "确认", MessageBoxType.Confirm);
            msg.OnYes = () =>
            {
                var oldEquip = EquipManager.Instance.GetEqiup(item.EquipInfo.Slot);
                if (oldEquip!=null)
                {
                    var newmsg = MessageBox.Show(string.Format("Darling~ 确定要替换 [{0}] 吗？", oldEquip.Define.Name), "确认", MessageBoxType.Confirm);
                    newmsg.OnYes = () =>
                    {
                        this.owner.DoEquip(this.item);
                    };
                }
                else
                {
                    this.owner.DoEquip(this.item);
                }
            };

        }

        void UnEquip()
        {
            //messageBox show 有返回值
            var msg = MessageBox.Show(string.Format("Darling~ 确定要取下 [{0}] 吗？", this.item.Define.Name), "确认", MessageBoxType.Confirm);
            msg.OnYes = () =>
            {
                //调用  uicharEquip
                this.owner.UnEquip(this.item);
            };

        }
    }
}