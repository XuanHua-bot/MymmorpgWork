using Models;
using Services;
using SkillBridge.Message;

namespace Managers
{
    public class EquipManager: Singleton<EquipManager>
    {
        public delegate void OnEquipChangeHandler();

        public event OnEquipChangeHandler OnEquipChanged;

        public Item[] Equips = new Item[(int)EquipSlot.SlotMax];//维护了七个格子

        private byte[] Data;//intList 用byte  来维护转换   服务端发送

        unsafe public void Init(byte[] data)
        {
            this.Data = data; //获取服务端背包格子的字节
            this.ParseEquipData(data);//解析服务器发来的 字节
        }

        public bool Contains(int equipId) //检查有没有穿什么道具
        {
            for (int i = 0; i < this.Equips.Length; i++)
            {
                if (Equips[i]!=null && Equips[i].Id == equipId)
                {
                    return true;
                }

               
            }
            return false;
        }

        public Item GetEqiup(EquipSlot slot) //  查询 格子 中放的啥道具
        {
            return Equips[(int)slot];
        }

        /// <summary>
        /// 用来解析服务器 发送来 的字节
        /// </summary>
        /// <param name="data"></param>
        unsafe void ParseEquipData(byte[] data)
        {
            fixed (byte* pt = this.Data)
            {
                for (int i = 0; i < this.Equips.Length; i++)
                {
                     int itemId = *(int*)(pt + i * sizeof(int));
                     if (itemId >0)
                     {
                         Equips[i] = ItemManager.Instance.Items[itemId];
                     }
                     else
                     {
                         Equips[i] = null;
                     }
                    
                }
            }
        }//解析服务器的 字节 =》 装备信息

        /// <summary>
        /// 用来转换 装备数组=》 字节
        /// </summary>
        /// <returns></returns>
        unsafe public byte[] GetEquipData()
        {
            fixed(byte* pt = Data)
            {
                for (int i = 0; i < (int)EquipSlot.SlotMax; i++)
                {
                    int* itemid = (int*)(pt + i * sizeof(int));
                    if (Equips[i] == null)
                    {
                        *itemid = 0;
                    }
                    else
                    {
                        *itemid = Equips[i].Id;
                    }
                }

                return this.Data;
            }
        }//装备信息 =》 字节

        
        
        public void EquipItem(Item equip)//请求备道具
        {
            ItemService.Instance.sendEquipItem(equip, true);
        }
        public void UnEquipItem(Item equip)//请求脱道具
        {
            ItemService.Instance.sendEquipItem(equip, false);
        }
        public void OnEquipItem(Item equip)
        {
            if (this.Equips[(int)equip.EquipInfo.Slot]!= null && this.Equips[(int)equip.EquipInfo.Slot].Id==equip.Id)
            {
              return;
            }

            this.Equips[(int)equip.EquipInfo.Slot] = ItemManager.Instance.Items[equip.Id];
            if (OnEquipChanged!=null)
            {
                OnEquipChanged();
            }
        }

         public void OnUnEquipItem( EquipSlot slot)
        {
            if (this.Equips[(int)slot]!=null)
            {
                this.Equips[(int)slot] = null;
                if (OnEquipChanged !=null)
                {
                    OnEquipChanged();
                }
            }
        }

        
    }
}