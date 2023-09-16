using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates { GUARD, PATROL, CHASE, DEAD };

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStats))]
public class EnemyController : MonoBehaviour, IEndGameObserver
{
    public AudioControl Audio;
    private EnemyStates enemyStates;
    public NavMeshAgent agent;
    private Animator anim;
    public CharacterStats characterStats;
    private Collider coll;

    [Header("Basic Settings")]
    public float sightRadius;
    public bool isGuard;
    private float speed;
    protected GameObject attackTarget;
    public float lookAtTime;
    private float remainLookAtTime;
    private float lastAttackTime;
    private Quaternion guardRotation;
    public bool isBoss;

    [Header("Patrol State")]
    public float patrolRange;
    private Vector3 wayPoint;
    private Vector3 guardPos;

    // bool judge animation
    bool isWalk;
    bool isChase;
    bool isFollow;
    public bool isDead;
    bool playerDead;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        coll = GetComponent<Collider>();
        speed = agent.speed;
        guardPos = transform.position;
        guardRotation = transform.rotation;
        remainLookAtTime = lookAtTime;
        Audio = GameObject.FindWithTag("Audio").GetComponent<AudioControl>();
    }

    void Start()
    {
        if (isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            enemyStates = EnemyStates.PATROL;
            GetNewWayPoint();
        }
        //FIXME: delete when switch scene
        GameManager.Instance.AddObserver(this);

        //scale enemy stats according to current level
        LevelUp();
    }

    //FIXME: add when switch scene

    /*    void OnEnable()
        {
            GameManager.Instance.AddObserver(this);
        }*/

    void OnDisable()
    {
        if (!GameManager.IsInitialized)
            return;
        GameManager.Instance.RemoveObserver(this);
        if (isBoss)
        {
            GameManager.Instance.bossAlive = false;
            Debug.Log("Boss Died!");
        }
        if (enemyStates != EnemyStates.DEAD)
            return;

        if (isBoss)
        {
            //TODO : Boss Died Audio
            Inventory.instance.AddItem(characterStats.CurrentLevel);
        }
        else
        {
            //20 % chance for minion item drop
            if (Random.Range(0, 100) >= 80)
            {
                Inventory.instance.AddItem();
            }
        }

    }

    void Update()
    {
        if (characterStats.CurrentHealth == 0)
            isDead = true;

        if (!playerDead)
        {
            SwitchAnimation();
            SwitchStates();
            lastAttackTime -= Time.deltaTime;

        }
    }

    void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStats.isCritical);
        anim.SetBool("Death", isDead);
    }

    void SwitchStates()
    {
        if (isDead)
            enemyStates = EnemyStates.DEAD;

        // if find player, change to chase
        else if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
        }

        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                isChase = false;

                if (transform.position != guardPos)
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;

                    if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
                    {
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
                    }
                }
                break;
            case EnemyStates.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f;

                // judge if enemy arrive the random guard point or not
                if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    if (remainLookAtTime > 0)
                        remainLookAtTime -= Time.deltaTime;
                    else
                        GetNewWayPoint();
                }
                else
                {
                    isWalk = true;
                    agent.destination = wayPoint;
                }

                break;
            case EnemyStates.CHASE:
                // chase player, if player out of range, back to last status
                isWalk = false;
                isChase = true;
                agent.speed = speed;
                if (!FoundPlayer())
                {
                    isFollow = false;
                    if (remainLookAtTime > 0)
                    {
                        agent.destination = transform.position;
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else if (isGuard)
                        enemyStates = EnemyStates.GUARD;
                    else
                        enemyStates = EnemyStates.PATROL;
                }
                else
                {
                    isFollow = true;
                    agent.isStopped = false;
                    agent.destination = attackTarget.transform.position;
                }

                if (TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;
                    agent.isStopped = true;

                    if (lastAttackTime < 0)
                    {
                        lastAttackTime = characterStats.attackData.coolDown;

                        // judge critical
                        characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;
                        // execute attack
                        Attack();
                    }
                }

                break;
            case EnemyStates.DEAD:
                coll.enabled = false;
                agent.radius = 0;
                // Destroy(gameObject, 2f);

                break;
        }
    }

    void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if (TargetInAttackRange())
        {
            // normal attack animation
            anim.SetTrigger("Attack");
        }
        if (TargetInSkillRange())
        {
            // skill attack animation
            anim.SetTrigger("Skill");
        }
    }

    void destroy()
    {
        GameManager.Instance.playerStats.GainXP(characterStats.CurrentLevel *10);
        Destroy(gameObject);
    }

    bool FoundPlayer()
    {
        // judge radius
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);

        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }

    bool TargetInAttackRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.attackRange;
        else
            return false;
    }

    bool TargetInSkillRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.skillRange;
        else
            return false;
    }

    void GetNewWayPoint()
    {
        remainLookAtTime = lookAtTime;

        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);

        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z + randomZ);

        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }

    //Animation Event

    void Hit()
    {
        //TODO : put player punched audio here
        Audio.EnemyHit();
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }

    public void EndNotify()
    {
        // win animation
        // stop all movement
        // stop agent
        playerDead = true;
        anim.SetBool("Win", true);
        isChase = false;
        isWalk = false;
        attackTarget = null;

    }

    void LevelUp()
    {
        characterStats.CurrentLevel += GameManager.gameLevel;
        if (isBoss)
        {
            characterStats.CurrentLevel += GameManager.gameLevel * 2;
            characterStats.attackData.minDamage += GameManager.gameLevel * 4;
            characterStats.attackData.maxDamage += GameManager.gameLevel * 4;
            characterStats.CurrentHealth += GameManager.gameLevel * 20;
            characterStats.MaxHealth += GameManager.gameLevel * 20;
        }
        else
        {
            characterStats.CurrentLevel += GameManager.gameLevel;
            characterStats.attackData.minDamage += GameManager.gameLevel * 2;
            characterStats.attackData.maxDamage += GameManager.gameLevel * 2;
            characterStats.CurrentHealth += GameManager.gameLevel * 10;
            characterStats.MaxHealth += GameManager.gameLevel * 10;
        }
    }
}
