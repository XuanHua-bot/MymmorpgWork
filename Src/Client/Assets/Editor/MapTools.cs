using Common.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapTools
{
    [MenuItem("Map Tools/Export Teleporters")]
    public static void ExportTeleporters()
    {
        DataManager.Instance.Load();

        Scene current = EditorSceneManager.GetActiveScene();
        string currentScene = current.name;
        if (current.isDirty)
        {
            EditorUtility.DisplayDialog("Warning Warning Warning Youuuu!", "传送点提示：请先保存当前场景喵~", "确定");
            return;
        }

        List<TeleporterObject> allTelepoters = new List<TeleporterObject>();

       
        foreach (var map in DataManager.Instance.Maps) //循环处理数据管理器中定义的每个地图（MapDefine），准备导出其传送点。
        {
            string sceneFile = "Assets/Levels/" + map.Value.Resource + ".unity";//根据地图名字生成 路径
            if (!System.IO.File.Exists(sceneFile))//检查场景文件是否存在
            {
                Debug.LogWarningFormat("Scene {0} not existed!", sceneFile);
                continue;
            }
            EditorSceneManager.OpenScene(sceneFile, OpenSceneMode.Single);//打开场景
            TeleporterObject[] teleporters = GameObject.FindObjectsOfType<TeleporterObject>();//找到所有传送点脚本
            foreach (var teleporter in teleporters)
            {
                if (!DataManager.Instance.Teleporters.ContainsKey(teleporter.ID))//检查当前传送点的 ID 是否在数据配置中定义
                {
                    EditorUtility.DisplayDialog("错误！！！", string.Format("地图:{0}中配置的 teleporter:{1}号传送嗲不存在！", map.Value.Resource, teleporter.ID),"确定");

                    return;
                }

                TeleporterDefine def = DataManager.Instance.Teleporters[teleporter.ID];
                if (def.MapID!=map.Value.ID)
                {
                    EditorUtility.DisplayDialog("错误", string.Format("地图:{0}中配置的Teleporter{1}不存在！", map.Value.Resource, teleporter.ID), "确定");
                    return;
                }
                def.Position = GameObjectTool.WorldToLogicN(teleporter.transform.position);
                def.Direction = GameObjectTool.WorldToLogicN(teleporter.transform.forward);
            }
        }
        DataManager.Instance.SaveTeleporters();
        EditorSceneManager.OpenScene("Assets/Levels/" + currentScene + ".unity");
        EditorUtility.DisplayDialog("提示", "传送点导出完毕了喵", "确定");
    }

    [MenuItem("Map Tools/Export SpawnPoints")]
    public static void ExportSpawnPoints()
    {
        DataManager.Instance.Load();

        Scene current = EditorSceneManager.GetActiveScene();
        string currentScence = current.name;
        if (current.isDirty)
        {
            EditorUtility.DisplayDialog("Warning Warning Warning Youuuu!", "刷怪点提示：请先保存当前场景喵~", "确定");
            return;
        }
        if (DataManager.Instance.SpawnPoints == null)
            DataManager.Instance.SpawnPoints = new Dictionary<int, Dictionary<int, SpawnPointDefine>>();
        foreach (var map in DataManager.Instance.Maps)
        {
            string scenceFile = "Assets/Levels/" + map.Value.Resource + ".unity";//根据地图名字生成 路径
            if (!System.IO.File.Exists(scenceFile))
            {
                Debug.LogWarningFormat("Scence{0} not existed!", scenceFile);
                continue;
            }
            EditorSceneManager.OpenScene(scenceFile, OpenSceneMode.Single);

            SpawnPoint[] spawnpoints = GameObject.FindObjectsOfType<SpawnPoint>();

            if (!DataManager.Instance.SpawnPoints.ContainsKey(map.Value.ID))
            {
                DataManager.Instance.SpawnPoints[map.Value.ID] = new Dictionary<int, SpawnPointDefine>();
            }
            foreach (var sp in spawnpoints)
            {
                if (!DataManager.Instance.SpawnPoints[map.Value.ID].ContainsKey(sp.ID))
                {
                    DataManager.Instance.SpawnPoints[map.Value.ID][sp.ID] = new SpawnPointDefine();
                }

                SpawnPointDefine def = DataManager.Instance.SpawnPoints[map.Value.ID][sp.ID];
                def.ID = sp.ID;
                def.MapID = map.Value.ID;
                def.Position = GameObjectTool.WorldToLogicN(sp.transform.position);
                def.Direction = GameObjectTool.WorldToLogicN(sp.transform.forward);

              

                
            }
     
        }
        DataManager.Instance.SaveTeleporters();
        EditorSceneManager.OpenScene("Assets/Levels/" + currentScence + ".unity");
        EditorUtility.DisplayDialog("提示", "刷怪点导出完毕了喵", "确定");
    }
     
}
