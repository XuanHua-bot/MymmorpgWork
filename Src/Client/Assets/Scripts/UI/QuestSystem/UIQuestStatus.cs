using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.QuestSystem
{
    public class UIQuestStatus: MonoBehaviour
    {
        public Image[] StatusImages;
        

        

        private NpcQuestStatus questStatus;
        
        
        
        public void SetQuestStatus(NpcQuestStatus status)
        {
            this.questStatus = status;
            for (int i = 0; i < 4; i++)
            {
                if (this.StatusImages[i]!=null)
                {
                    this.StatusImages[i].gameObject.SetActive(i == (int)status);
                }
            }
        }
        
    }
    
    
}