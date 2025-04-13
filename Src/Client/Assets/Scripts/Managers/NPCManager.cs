using Common.Data;
using System.Collections.Generic;

namespace Managers
{
    class NPCManager :Singleton<NPCManager>
    {
        public delegate bool NpcActionHandler(NpcDefine npc);

        Dictionary<NpcFunction, NpcActionHandler> eventMap = new Dictionary<NpcFunction, NpcActionHandler>();

        
        //调用shopManager中注册的 方法
        public void RegisterNpcEvent(NpcFunction function,NpcActionHandler action)
        {
            if (!eventMap.ContainsKey(function))
            {
                eventMap[function] = action;
            }
            else
                eventMap[function] += action;
        }

        public NpcDefine GetNpcDefine(int npcID) //返回给调用者datanpc
        {
            NpcDefine npc = null;
            DataManager.Instance.NPCs.TryGetValue(npcID, out npc);
            return npc;
        }

        public bool Interactive(int npcId)//与npc交互前 先判断是否存在
        {
            if (DataManager.Instance.NPCs.ContainsKey(npcId))
            {
                var npc = DataManager.Instance.NPCs[npcId];
                return Interactive(npc);//存在 便执行交互
            }
            return false;
        }
        public bool Interactive(NpcDefine npc)// 传入npc类型 并进行 任务类型分支
        {
            //检查有无对话
            if (DoTaskInteractive(npc))
            {
                return true;
            }
            else if (npc.Type == NpcType.Functional)
            {
                return DoFunctionInteractive(npc);//调用事件
            }
            return false;
        }

        

        private bool DoTaskInteractive(NpcDefine npc)
        {
            var status = QuestManager.Instance.GetQuestStatusByNpc(npc.ID);//获取状态
            if (status == NpcQuestStatus.None)
            {
                return false;
            }

            return QuestManager.Instance.OpenNpcQuest(npc.ID);//有状态 则打开任务
        }

        private bool DoFunctionInteractive(NpcDefine npc)
        {
            if (npc.Type != NpcType.Functional)//验证 NPC 类型
            {
                return false;
            }
            if (!eventMap.ContainsKey(npc.Function))//验证功能是否注册
            {
                return false;
            }

            //npc.Function 是当前 NPC 的功能类型（如 NpcFunction.OpenShop）  
            //返回值是bool 代表 openShop是否成功执行
            return eventMap[npc.Function](npc);//从字典 eventMap 中找到该功能对应的委托（处理函数），传入 NPC 数据 npc 并执行。
        }


    }
}
