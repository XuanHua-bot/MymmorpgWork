using Common;
using GameServer.Entities;
using GameServer.Services;
using Network;
using SkillBridge.Message;

namespace GameServer.Managers
{
    class EquipManager : Singleton<EquipManager>
    {
        public Result EquipItem(NetConnection<NetSession> sender, int slot, int itemId, bool isEquip)
        {
            Character character = sender.Session.Character;
            if (!character.ItemManager.Items.ContainsKey(itemId))////校验角色身上有无装备
            {
                return Result.Failed;
            }

            UpdateEquip(character.Data.Equips, slot, itemId, isEquip);
            
            DBService.Instance.Save();
            return Result.Success;
        }

        // 通过指针操作修改角色的装备数据
        unsafe void UpdateEquip(byte[] equipData, int slot, int itemId, bool isEquip)
        {
            //fixed 语句固定 equipData 字节数组的内存地址
            fixed (byte * pt = equipData)//pt 是指向 equipData 起始地址的 byte 类型指针。
            {
                int* slotid = (int*)(pt + slot * sizeof(int));//获取槽位指针
                if (isEquip)
                {
                    *slotid = itemId;
                }
                else //isEquip 是false  则脱装备，情空 地址内的槽位中的 装备id
                {
                    *slotid = 0;
                }
            }
        }
    }
}