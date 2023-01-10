using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HordeMemberUIController : MonoBehaviour
{
    public string id = "";
    public TMP_Text name;
    public TMP_Text status;
    public TMP_Text energy;

    public Button mineJobButton;
    public Button harvestJobButton;
    public Button chopJobButton;


    public void SetJob(MemberJob job)
    {
        HordeManager.Instance.UpdateMemberJob(id, job);
    }
    
    public void SetJobToMining()
    {
        SetJob(MemberJob.Mine);
    }
    
    public void SetJobToHarvesting()
    {
        SetJob(MemberJob.Harvest);
    }
    
    public void SetJobToChopping()
    {
        SetJob(MemberJob.Chop);
    }
}