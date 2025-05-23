﻿using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillBridge.Message;
using GameServer.Entities;

namespace GameServer.Managers
{
    class CharacterManager : Singleton<CharacterManager>
    {
        public Dictionary<int, Character> Characters = new Dictionary<int, Character>();

        public CharacterManager()
        {
        }

        public void Dispose()
        {
        }

        public void Init()
        {

        }

        public void Clear()
        {
            this.Characters.Clear();
        }

        public Character AddCharacter(TCharacter cha)
        {
            Character character = new Character(CharacterType.Player, cha);
            EnityManager.Instance.AddEntity(cha.MapID, character);
            character.Info.EntityId = character.entityId;
            this.Characters[character.Id] = character;
            return character;
        }


        public void RemoveCharacter(int characterId)
        {
            if (this.Characters.ContainsKey(characterId))
            {
                var cha = this.Characters[characterId];
                EnityManager.Instance.RemoveEntity(cha.Data.MapID, cha);
                this.Characters.Remove(characterId); 
            }
           
        }

        public Character GetCharacter(int CharacterId)
        {
            Character character = null;
            this.Characters.TryGetValue(CharacterId, out character);
            return character;
        }

    }
}
