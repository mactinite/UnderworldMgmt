using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonBed : MonoBehaviour
{

    [SerializeField] public bool occupied = false;
    [SerializeField] public string owner = "Unassigned";
    
    [SerializeField] private float energyRechargeRate = 1f;
    public Transform rallyPoint;

    private float maxEnergy = 1f;
    [SerializeField] private float currentEnergy = 1f;
    private float sleepTime = 0f;
    
    
    // Start is called before the first frame update
    void Start()
    {
        HordeManager.Instance.AddRandomMember();
    }

    // Update is called once per frame
    void Update()
    {
        if (occupied)
        {
            if (sleepTime == 0)
            {
                maxEnergy = HordeManager.Instance.hordeMembers[owner].maxEnergy;
                currentEnergy = HordeManager.Instance.hordeMembers[owner].energy;
            }

            sleepTime += Time.deltaTime;
            currentEnergy += energyRechargeRate * Time.deltaTime;

            currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
            HordeManager.Instance.UpdateMemberEnergy(owner, currentEnergy);

            if (currentEnergy >= maxEnergy)
            {
                HordeManager.Instance.UpdateMemberEnergy(owner, maxEnergy);
            }
        }
        else
        {
            sleepTime = 0;
        }
    }
}
