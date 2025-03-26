using Managers;
using Models;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMiniMap : MonoBehaviour {


    public Collider MiniMapBoundingBox;
    public Image miniMap;
    public Image arrow;
    public Text mapName;
    //public Text Description;

    private Transform playerTransform;
    // Use this for initialization
    void Start() {
      
        MiniMapManager.Instance.miniMap = this;
        this.UpdateMap();
    }
    public void UpdateMap ()
    {
        this.mapName.text = User.Instance.CurrentMapData.Name;

        
        this.miniMap.overrideSprite = MiniMapManager.Instance.LoadCurrentMiniMap();
        
        //this.miniMap.overrideSprite = MiniMapManager.Instance.LoadCurrentMiniMap();

        
        //this.Description.text = User.Instance.CurrentMapData.Description;
        this.miniMap.SetNativeSize();
        this.miniMap.transform.localPosition = Vector3.zero;
        this.MiniMapBoundingBox = MiniMapManager.Instance.MinimapBoundingBox;
        this.playerTransform = null;
    }

    // Update is called once per frame
    void Update () { 

        if (playerTransform==null)
            playerTransform = MiniMapManager.Instance.PlayerTransform;
        
        if (MiniMapBoundingBox == null || playerTransform == null) return;
   
        float realWidth = MiniMapBoundingBox.bounds.size.x;
        float realHight = MiniMapBoundingBox.bounds.size.z;

        float relaX = playerTransform.position.x - MiniMapBoundingBox.bounds.min.x;
        float relaY = playerTransform.position.z - MiniMapBoundingBox.bounds.min.z;

        float pivotX = relaX / realWidth;
        float pivoY = relaY / realHight;
        this.miniMap.rectTransform.pivot = new Vector2(pivotX, pivoY);
        this.miniMap.rectTransform.localPosition = Vector2.zero;
        this.arrow.transform.eulerAngles = new Vector3(0, 0, -playerTransform.eulerAngles.y);

	}
}
