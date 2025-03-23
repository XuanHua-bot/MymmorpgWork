using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities;
using SkillBridge.Message;

namespace Managers
{
    interface IEntityNotify
    {
        void OnEntityRemoved();
        void OnEntityChaged(Entity entity);
        void OnEntityEvent(EntityEvent @event);
    }
    class EntityManager:Singleton<EntityManager>
    {

        Dictionary<int, Entity> entities = new Dictionary<int, Entity>();
        Dictionary<int, IEntityNotify> notifiers = new Dictionary<int, IEntityNotify>();

        public void RegisterEnityChangeNotify(int entifyId, IEntityNotify notify)
        {
            this.notifiers[entifyId] = notify;
        }

        public void AddEntity(Entity enity)
        {
            entities[enity.entityId] = enity;
        }

        public void RemoveEntity(NEntity entity)
        {
            this.entities.Remove(entity.Id);
            if (notifiers.ContainsKey(entity.Id))
            {
                notifiers[entity.Id].OnEntityRemoved();
                notifiers.Remove(entity.Id);
            }
        }

        internal void OnEntitySync(NEntitySync data)
        {
            Entity entity = null;

            entities.TryGetValue(data.Id, out entity);
            if (entity!=null)
            {
                if (data.Entity != null)
                    entity.EntityData = data.Entity;
                if (notifiers.ContainsKey(data.Id))
                {
                    notifiers[entity.entityId].OnEntityChaged(entity);
                    notifiers[entity.entityId].OnEntityEvent(data.Event);
                }
            }
        }
    }
}
