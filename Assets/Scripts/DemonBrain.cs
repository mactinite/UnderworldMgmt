using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using Unity.Collections;
using UnityEngine;

public class DemonBrain : MonoBehaviour
{
    [ReadOnly] public string ID;
    public Member memberData;
    [ReadOnly] public MemberJob job;
    [SerializeField] private AIPath pathfinder;
    [SerializeField, ReadOnly] private Status status = Status.Idle;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private DemonBed ownedBed;
    [SerializeField] private float workEfficiency = 1;
    private Vector2 _currentDestination = Vector2.zero;
    private DemonBed _bedTarget;
    private ResourceGenerator _workTarget;
    private DestroyableResource _harvestTarget;

    private float _workTime = 0;
    private float _sleepTime = 0;


    public enum Status
    {
        Idle,
        Sleeping,
        GoingToWork,
        GoingToBed,
        Working,
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        job = HordeManager.Instance.hordeMembers[ID].job;
        HordeManager.Instance.OnHordeChange += OnHordeChange;
    }

    private void OnHordeChange(string id, Member newData)
    {
        if (id == ID)
        {
            memberData = newData;
            job = memberData.job;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (pathfinder.desiredVelocity.magnitude > 0f)
        {
            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }

        sprite.flipX = pathfinder.desiredVelocity.x < 0;

        if (status == Status.Idle)
        {
            if (HordeManager.Instance.hordeMembers[ID].energy > 0)
            {
                _workTarget = GetAvailableWork();

                if (_workTarget != null)
                {
                    status = Status.GoingToWork;
                    return;
                }
                else
                {
                    _harvestTarget = GetHarvestableNode();
                    status = Status.GoingToWork;
                    return;
                }
            }
            else
            {
                _bedTarget = GetAvailableBed();

                if (_bedTarget != null)
                {
                    _bedTarget.owner = ID;
                    status = Status.GoingToBed;
                    return;
                }
            }

            // Go to player if idle and nothing to do
            HordeManager.Instance.UpdateMemberStatus(ID, $"Idle");
            _currentDestination = PlayerControllerInput.Player.transform.position;
            pathfinder.destination = _currentDestination;
            return;
        }



        Working();

        
        if (status == Status.GoingToBed)
        {
            _currentDestination = _bedTarget.rallyPoint.position;
            pathfinder.destination = _currentDestination;
            // if the owner changes to another demon while we're on the way, go back to idle
            if (_bedTarget.owner != ID)
            {
                status = Status.Idle;
            }
            else if (gameObject.FastDistance(_currentDestination) < 3f)
            {
                status = Status.Sleeping;
                _sleepTime = 0;
                return;
            }
            else
            {
                HordeManager.Instance.UpdateMemberStatus(ID, "Going to Bed");
            }
        }
        
        if (status == Status.Sleeping)
        {
            if (_sleepTime == 0)
            {
                
                pathfinder.enabled = false;
                transform.position = _bedTarget.transform.position;
                _bedTarget.occupied = true;
                animator.SetBool("Sleeping", true);
                HordeManager.Instance.UpdateMemberStatus(ID, $"Sleeping");
            }

            _sleepTime += Time.deltaTime;

            if (HordeManager.Instance.hordeMembers[ID].energy >= HordeManager.Instance.hordeMembers[ID].maxEnergy)
            {
                _sleepTime = 0;
                animator.SetBool("Sleeping", false);
                transform.position = _bedTarget.rallyPoint.position;
                pathfinder.enabled = true;
                _bedTarget.occupied = false;
                status = Status.Idle;
                return;
            }
        }
    }

    private void Working()
    {
        
        if (status == Status.GoingToWork)
        {
            if (_workTarget)
            {
                _currentDestination = _workTarget.rallyPoint.position;
                pathfinder.destination = _currentDestination;
                if ((_workTarget.reserved && _workTarget.occupantID != ID) || _workTarget.requiredJob != job)
                {
                    status = Status.Idle;
                }
            }
            else if (_harvestTarget)
            {
                _currentDestination = _harvestTarget.transform.position;
                pathfinder.destination = _currentDestination;
            }
            else
            {
                status = Status.Idle;
            }



            // if the target becomes occupied by someone that's not us, go back to idle


            if (gameObject.FastDistance(_currentDestination) < 3f)
            {
                status = Status.Working;
                _workTime = 0;
            }
            else
            {
                HordeManager.Instance.UpdateMemberStatus(ID, "Going to Work");
            }
        }
        
        if (status == Status.Working && _workTarget != null)
        {
            if (_workTime == 0)
            {
                _workTarget.reserved = true;
                _workTarget.occupantID = ID;

                animator.SetBool("IsAttacking", true);

                pathfinder.enabled = false;
                transform.position = _workTarget.rallyPoint.position;
                HordeManager.Instance.UpdateMemberStatus(ID, $"{_workTarget.verb}");
            }

            var currentEnergy = HordeManager.Instance.hordeMembers[ID].energy;


            HordeManager.Instance.UpdateMemberEnergy(ID, currentEnergy - (workEfficiency * Time.deltaTime));

            _workTime += Time.deltaTime;
            _workTarget.WorkTick();

            if (HordeManager.Instance.hordeMembers[ID].energy <= 0 || _workTarget.requiredJob != job)
            {
                _workTarget.reserved = false;
                _workTarget.occupantID = "";
                animator.SetBool("IsAttacking", false);
                pathfinder.enabled = true;
                transform.position = _workTarget.rallyPoint.position;
                status = Status.Idle;
                _workTarget = null;

            }
        }
        else if (status == Status.Working && _workTarget == null && _harvestTarget != null)
        {
            if (_workTime == 0)
            {
                pathfinder.enabled = false;
                HordeManager.Instance.UpdateMemberStatus(ID, $"{_harvestTarget.verb}");
                animator.SetBool("IsAttacking", true);
            }

            var currentEnergy = HordeManager.Instance.hordeMembers[ID].energy;
            _harvestTarget.Damage(2);
            HordeManager.Instance.UpdateMemberEnergy(ID, currentEnergy - (workEfficiency * Time.deltaTime));
            _workTime += Time.deltaTime;

            if (HordeManager.Instance.hordeMembers[ID].energy <= 0)
            {
                animator.SetBool("IsAttacking", false);
                _harvestTarget = null;
                pathfinder.enabled = true;
                status = Status.Idle;
            }
        }
        else if(HordeManager.Instance.hordeMembers[ID].energy > 0 && status == Status.Working)
        {
            animator.SetBool("IsAttacking", false);
            _harvestTarget = null;
            _workTarget = null;
            pathfinder.enabled = true;
            status = Status.Idle;
        }
    }

    void Hide()
    {
        gameObject.SetLayerRecursively(LayerMask.NameToLayer("Hidden"));
    }

    void Show()
    {
        gameObject.SetLayerRecursively(LayerMask.NameToLayer("Default"));
    }

    public DemonBed GetAvailableBed()
    {
        var availableBeds = GameObject.FindObjectsOfType<DemonBed>()
            .Where(bed => bed.owner == ID || bed.owner == "Unassigned");

        return GetBestBed(availableBeds.ToArray());
    }

    DemonBed GetBestBed(DemonBed[] beds)
    {
        DemonBed tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (DemonBed bed in beds)
        {
            if (bed.owner == ID) return bed;
            float dist = Vector3.Distance(bed.transform.position, currentPos);
            if (dist < minDist)
            {
                tMin = bed;
                minDist = dist;
            }
        }

        return tMin;
    }

    public ResourceGenerator GetAvailableWork()
    {
        var generators = GameObject.FindObjectsOfType<ResourceGenerator>()
            .Where(generator => generator.requiredJob == job && !generator.reserved).ToArray();


        return GetClosestResourceGenerator(generators);
    }

    public DestroyableResource GetHarvestableNode()
    {
        var resourceNodes = GameObject.FindObjectsOfType<DestroyableResource>()
            .Where(generator => generator.jobRequired == job).ToArray();

        return GetClosestDestroyableResource(resourceNodes);
    }

    DestroyableResource GetClosestDestroyableResource(DestroyableResource[] resourceGenerators)
    {
        DestroyableResource tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (DestroyableResource g in resourceGenerators)
        {
            float dist = Vector3.Distance(g.transform.position, currentPos);
            if (dist < minDist)
            {
                tMin = g;
                minDist = dist;
            }
        }

        return tMin;
    }

    ResourceGenerator GetClosestResourceGenerator(ResourceGenerator[] resourceGenerators)
    {
        ResourceGenerator tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (ResourceGenerator g in resourceGenerators)
        {
            float dist = Vector3.Distance(g.transform.position, currentPos);
            if (dist < minDist)
            {
                tMin = g;
                minDist = dist;
            }
        }

        return tMin;
    }
}