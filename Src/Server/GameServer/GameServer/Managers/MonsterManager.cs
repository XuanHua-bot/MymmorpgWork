using System.Collections.Generic;
using GameServer.Entities;
using GameServer.Models;
using SkillBridge.Message;

namespace GameServer.Managers
{
     class MonsterManager
    {
        private Map Map;//标记当前所属地图
        public Dictionary<int, Monster> Monsters = new Dictionary<int, Monster>();//管理所有怪物

        public void Init(Map map)
        {
            this.Map = map;//存储 Map Map;//标记当前所属地图
        }

        internal Monster Creat(int spawnMonID, int spawnLevel, NVector3 position, NVector3 direction)
        {
            // 1、new 一个monster  2、交给怪物管理器 3、设置属性 4、添加到字典 5、怪物进入（以实现 玩家能否看到）  类似玩家进入 6、返回
            Monster monster = new Monster(spawnMonID, spawnLevel, position, direction);
            EnityManager.Instance.AddEntity(this.Map.ID,monster);
            monster.Id = monster.entityId;
            monster.Info.EntityId = monster.entityId;//因为怪物没有数据库id 所以直接使用entityId
            monster.Info.mapId = this.Map.ID;
            Monsters[monster.Id] = monster;

            this.Map.MonsterEnter(monster);//怪物进入地图
            return monster;
        }
    }
}