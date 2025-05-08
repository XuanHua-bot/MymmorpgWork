using Common.Data;
using GameServer.Core;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entities
{
    class CharacterBase : Entity
    {

        public int Id { get; set; }
        public string Name { get { return this.Info.Name; } }

        public NCharacterInfo Info;
        public CharacterDefine Define;

        public CharacterBase(Vector3Int pos, Vector3Int dir) : base(pos, dir)
        {

        }

        public CharacterBase(CharacterType type, int configId, int level, Vector3Int pos, Vector3Int dir) :
           base(pos, dir)
        {
            this.Info = new NCharacterInfo();
            this.Info.Type = type;
            this.Info.Level = level;
            this.Info.ConfigId = configId;
            this.Info.Entity = this.EntityData;
            this.Info.EntityId = this.entityId;
            this.Define = DataManager.Instance.Characters[this.Info.ConfigId];
            this.Info.Name = this.Define.Name;
         
            //int key = this.Info.Tid;
            // 检查键是否存在
            /*if (DataManager.Instance.Characters.ContainsKey(key))
            {
                this.Define = DataManager.Instance.Characters[key];
                this.Info.Name = this.Define.Name;
            }
            else
            {
                // 处理键不存在的情况，输出日志
                Console.WriteLine($"Key {key} not found in the Characters dictionary.");
                // 你可以根据实际情况进行其他处理，比如抛出异常或者设置默认值
            }*/
        }
    }
}