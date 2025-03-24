﻿using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Managers
{
    class MiniMapManager : MonoSingleton<MiniMapManager> {
        public Transform PlayerTransform {
            get
            {
                if (User.Instance.CurrentCharacter == null)
                {
                    return null;
                }
                return User.Instance.CurrentCharacterObject.transform;
            }
        }
        public Sprite LoadCurrentMiniMap()
        {
            return Resloader.Load<Sprite>("UI/MiniMap/" + User.Instance.CurrentMapData.Name);
        }


    }
    

        
    
}
