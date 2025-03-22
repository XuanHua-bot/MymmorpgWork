using Common;
using GameServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class EnityManager:Singleton<EnityManager>
    {
        private int idx = 0;
        public List<Entity> AllEnities = new List<Entity>();
        public Dictionary<int, List<Entity>> MapEnities = new Dictionary<int, List<Entity>>();

        public void AddEntity(int mapId,Entity entity)
        {
            AllEnities.Add(entity);

            entity.EntityData.Id = ++this.idx;

            List<Entity> entities = null;

            if (!MapEnities.TryGetValue(mapId,out entities))
            {
                entities = new List<Entity>();
                MapEnities[mapId] = entities;
            }
            entities.Add(entity);
            
        }

        public void RemoveEntity(int mapId,Entity entity)
        {
            this.AllEnities.Remove(entity);
            this.MapEnities[mapId].Remove(entity);
        }
    }
}
