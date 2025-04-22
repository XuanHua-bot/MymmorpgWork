using System;
using Common;
using Common.Data;
using GameServer.Models;
using UnityEngine;

namespace GameServer.Managers
{
     class Spawner
    {
        public SpawnRuleDefine Define { get; set; }//每一个刷怪器对应 一个 刷怪规则 和 地图
        private Map Map;


        /// <summary>
        /// 刷新时间
        /// </summary>
        private float spawnTime = 0;

        /// <summary>
        /// 消失时间
        /// </summary>
        private float unspawnTime = 0;

        private bool spawned = false;

        private SpawnPointDefine spawnPoint = null;

        public Spawner(SpawnRuleDefine define, Map map)// 加载刷怪点
        {
            this.Define = define;
            this.Map = map;
            if (DataManager.Instance.SpawnPoints[this.Map.ID].ContainsKey(this.Define.SpawnPoint))//是否存在 该刷怪点
            {
                spawnPoint = DataManager.Instance.SpawnPoints[this.Map.ID][this.Define.SpawnPoint];
            }
            else
            {
                Log.ErrorFormat("SpawnRuele[{0}] SpawnPoint[{1}] not existed",this.Define,this.spawnPoint);
            }
        }
        
        
        
        public void Update()
        {
            if (this.CanSpawn())// canSpawn 为true  则 执行 spawn()
            {
                this.Spawn();
            }
        }

        bool CanSpawn()
        {
            if (this.spawned)
            {
                return false; 
            }
            //怪物 被杀死的时间
            if(this.unspawnTime + this.Define.SpawnPeriod > Time.time)
            {
                return false;
            }
            return true;
        }

        public void Spawn()
        {
            this.spawned = true;
            Log.InfoFormat("Map[{0}]Spawn[{1} : Mon :{2},Lv:{3}] At Point:{4}",this.Define.MapID,this.Define.ID,this.Define.SpawnMonID,this.Define.SpawnLevel,this.Define.SpawnPoint);
            this.Map.MonsterManager.Creat(this.Define.SpawnMonID,this.Define.SpawnLevel,this.spawnPoint.Position,this.spawnPoint.Direction);
        }
    }
}