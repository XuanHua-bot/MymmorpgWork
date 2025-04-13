using Common.Data;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Models
{
    public class Quest
    {
        /// <summary>
        /// QuestDefine 服务器中定义的 任务表格信息
        /// </summary>
        public QuestDefine Define;//本地配置信息
        /// <summary>
        /// NQuestInfo 服务端客户端交互用  任务 id，对应数据库id,任务状态,任务目标//列表
        /// </summary>
        public NQuestInfo Info;//网络配置信息  如果 任务没有接 则不会出现在网络配置中   以缓解服务器压力
        
        public Quest()
        {

        }

        public Quest(NQuestInfo info)
        {
            this.Info = info;
            this.Define = DataManager.Instance.Quests[info.QuestId];
        }

        public Quest(QuestDefine define)
        {
            this.Define = define;
            this.Info = null;
        }

        //public string GetTypeName()
        //{
        //    return EnumUtil.GetEnumDescription(this.Define.Type);
        //}
    }
}
