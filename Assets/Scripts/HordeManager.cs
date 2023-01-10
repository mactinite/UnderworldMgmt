using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class HordeManager : MonoBehaviour
{
    public static HordeManager Instance = null;
    
    public int hordeSize = 0;

    public TextAsset DemonNamesTextAsset;
    private string[] names;
    public GameObject memberInstancePrefab;
    public List<GameObject> memberInstances = new();
    public Dictionary<string, Member> hordeMembers = new();

    public Action<string, Member> OnHordeChange;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        
    }

    public void Start()
    {
        char[] archDelim = new char[] { '\r', '\n' };
        names = DemonNamesTextAsset.text.Split(archDelim, StringSplitOptions.RemoveEmptyEntries);
    }

    public Member GetMember(string ID)
    {
        if (hordeMembers.ContainsKey(ID))
        {
            return hordeMembers[ID];
        }

        return null;
    }

    private string GetRandomName()
    {
        return names[Random.Range(0, names.Length)];
    }

    public void AddRandomMember()
    {
        AddMember(GetRandomName());
    }

    public void AddMember(string name)
    {
        Member newMember = new Member()
        {
            energy = 10,
            maxEnergy = 10,
            name = name,
            job = MemberJob.Mine
        };

        hordeMembers.Add(newMember.ID, newMember);
        
        GameObject memberInstance = Instantiate(memberInstancePrefab, transform.position + (Vector3)(UnityEngine.Random.insideUnitCircle * 3f), Quaternion.identity);
        DemonBrain demonBrain = memberInstance.GetComponent<DemonBrain>();
        demonBrain.ID = newMember.ID;
        
        memberInstances.Add(memberInstance);
        OnHordeChange?.Invoke(newMember.ID, newMember);
    }

    public void UpdateMemberStatus(string id, string status)
    {
        if (hordeMembers.TryGetValue(id, out var member))
        {
            member.status = status;
            hordeMembers[id] = member;
            OnHordeChange?.Invoke(id, member);
            return;
        }
        
        Debug.LogWarning($"Member with id:({id}) does not exist");
    }
    
    public void UpdateMemberJob(string id, MemberJob job)
    {
        if (hordeMembers.TryGetValue(id, out var member))
        {
            member.job = job;
            hordeMembers[id] = member;
            OnHordeChange?.Invoke(id, member);
            return;
        }
        
        Debug.LogWarning($"Member with id:({id}) does not exist");
    }

    public void UpdateMemberEnergy(string id, float newEnergy)
    {
        if (hordeMembers.TryGetValue(id, out var member))
        {
            member.energy = newEnergy;
            hordeMembers[id] = member;
            OnHordeChange?.Invoke(id, member);
            return;
        }
        
        Debug.LogWarning($"Member with id:({id}) does not exist");
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 3f);
    }
}

[Serializable]
public class Member
{
    public string name = "Bob";
    public string status = "Idle";
    public MemberJob job = MemberJob.Mine;
    public float energy = 10f;
    public float maxEnergy = 10f;
    public string ID => name + GetHashCode();

}

[Serializable]
public enum MemberJob {
    Mine,
    Harvest,
    Chop
}
