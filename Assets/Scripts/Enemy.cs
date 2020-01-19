using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {
    [Range(0f, 360f)]
    public float fieldOfView = 45f;
    public float sightRadius = 10f;
    public float chaseTimeout = 5f;
    public int maxHealth = 20;
    public Slider healthBar;
    public Transform weaponPivot;
    public EnemyWeapon currentWeapon;
    int _health;
    public int health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;
            healthBar.value = value / (float)maxHealth;
            SetChase(true);
            if (value < 1)
            {
                Gameplay.instance.enemyCount--;
                Destroy(gameObject);
            }
        }
    }
    NavMeshAgent agent;
    Transform trans;
    Transform charTrans;
    bool isPatrol;
    bool isChase;
    float chaseTime;

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    Vector3 res;
    public void Patrol()
    {
        if (RandomPoint(trans.position, 8f, out res))
        {
            agent.SetDestination(res);
        }
    }

    void SetChase(bool val)
    {
        if (val)
        {
            isPatrol = false;
            isChase = true;
            agent.updateRotation = false;
            chaseTime = 0;
            agent.stoppingDistance = 2f;
            fieldOfView = 100f;
        } else
        {
            isChase = false;
            isPatrol = true;
            chaseTime = 0;
            fieldOfView = 45f;
            agent.stoppingDistance = 0.1f;
        }
    }

	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();
        charTrans = Character.instance.transform;
        trans = transform;
        agent.updatePosition = true;
        agent.updateRotation = true;
        isPatrol = true;
        isChase = false;
        health = maxHealth;
	}

    Vector3 dir;
    Vector3 dirToPlayer;
    float dstToPlayer;
    bool IsPlayerInSight()
    {
        dir = (charTrans.position - trans.position);
        dirToPlayer = dir.normalized;
        dstToPlayer = dir.magnitude;
        if(dstToPlayer < sightRadius)
        {
            if (Vector3.Angle(trans.forward, dirToPlayer) < fieldOfView / 2f)
            {
                if(!Physics.Raycast(trans.position, dirToPlayer, dstToPlayer, 4))
                {
                    return true;
                }
            }
        }
        return false;
    }

    Quaternion lookRotation;
	// Update is called once per frame
	void Update ()
    {
        if (Gameplay.instance.isLose) return;
        if (IsPlayerInSight())
        {
            if (isChase)
            {
                lookRotation = Quaternion.LookRotation(dirToPlayer);
                trans.rotation = Quaternion.Slerp(trans.rotation, lookRotation, Time.deltaTime * 10f);
                if (dstToPlayer < 10f)
                {
                    currentWeapon.Shoot();
                }
            }
            SetChase(true);
        } else
        {
            if (isChase)
            {
                agent.updateRotation = true;
                chaseTime += Time.deltaTime;
                if (chaseTime > chaseTimeout)
                {
                    SetChase(false);
                }
            }
        }
        if (isChase)
        {
            if (dstToPlayer < 3f)
                agent.isStopped = true;
            else
                agent.isStopped = false;
                agent.SetDestination(charTrans.position);
        }
        if (!isPatrol) return;
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    Patrol();
                }
            }
        }
    }

    public void AssignWeapon(EnemyWeapon weapon)
    {
        EnemyWeapon e = Instantiate(weapon, weaponPivot);
        currentWeapon = e;
    }
}
