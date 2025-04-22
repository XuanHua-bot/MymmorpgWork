using System.Collections.Generic;
using GameServer.Models;
using UnityEngine.Experimental.PlayerLoop;

namespace GameServer.Managers
{
    /// <summary>
    /// 刷怪管理器
    /// </summary>
     class SpawnManager
    {
        private List<Spawner> Rules = new List<Spawner>();
        private Map Map;

        public void Init(Map map)
        {
            this.Map = map;//获取当前地图
            
            if (DataManager.Instance.SpawnRules.ContainsKey(map.Define.ID))//读取刷怪规则表   当前地图
            {
                foreach (var define in DataManager.Instance.SpawnRules[map.Define.ID].Values)
                {
                    ////刷怪规则 创建为刷怪器
                        this.Rules.Add(new Spawner(define,this.Map));// 刷怪规则、地图
                }
            }
        }

        public void Update()
        {
            if (Rules.Count ==0)
            {
                return;
            }

            for (int i = 0; i < Rules.Count; i++)//有刷怪规则 则调用规则的update
            {
                this.Rules[i].Update();
            }
        }
    }
}