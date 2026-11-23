using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{


    // REFs to gun system
    [SerializeField] private Gun gunPrefab;
    [SerializeField] private GunDefination startingGun;
    [SerializeField] private Transform gunSlot;
    [SerializeField] private Transform enemyFiringPoint;
    private Gun equippedGun;
    public enum State { Patrol, Chase, Attack }
    public State currentState = State.Patrol;

    public enum PatrolMode { Waypoints, RandomNavMesh }

    [Header("Patrol Mode")]
    public PatrolMode patrolMode = PatrolMode.Waypoints;
    public bool randomWaypointOrder = false;
    public float randomPatrolRadius = 10f;
    public Transform[] patrolPoints;


    [Header("References")]
    public Transform player;
    public Transform firePoint;

    [Header("Movement Settings")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;

    [Header("Attack Settings")]
    public float attackDistance = 1.5f;
    public float attackCooldown = 1f;
    public int damage = 20;

    [Header("Bullet Visual")]
    public GameObject bulletVisualPrefab;
    public float bulletVisualSpeed = 25f;
    public float bulletVisualLifeTime = 0.5f;

    [Header("Detection Settings")]
    public float chaseDistance = 8f;
    public float viewDistance = 10f;
    [Range(0, 360)]
    public float viewAngle = 90f;
    public LayerMask obstacleMask;

    [Header("State Materials")]
    public Material patrolMaterial;
    public Material chaseMaterial;
    public Material attackMaterial;

    private Renderer rend;
    private NavMeshAgent agent;
    private int patrolIndex = 0;
    private float attackTimer = 0f;

    private Vector3 eyeOffset = new Vector3(0, 1.5f, 0);

    private void Awake()
    {
        EquipStartingGun();
    }

    void Start()
    {
        rend = GetComponent<Renderer>();
        agent = GetComponent<NavMeshAgent>();

        rend.material = patrolMaterial;
        agent.speed = patrolSpeed;
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;

            case State.Chase:
                Chase();
                break;

            case State.Attack:
                Attack();
                break;
        }

    }

    private void EquipStartingGun()
    {
        if (equippedGun != null || gunPrefab == null ||
            startingGun == null || gunSlot == null) return;
        equippedGun = Instantiate(gunPrefab);
        equippedGun.InjectDefinition(startingGun);
        equippedGun.AttachToHolder(gunSlot, gameObject);
    }

    // ---------------- PATROL ---------------- //
    void Patrol()
    {
        SetMaterial(patrolMaterial);
        agent.speed = patrolSpeed;
        agent.isStopped = false;

        if (!agent.hasPath || agent.remainingDistance < 0.3f)
        {
            if (patrolMode == PatrolMode.Waypoints && patrolPoints.Length > 0)
            {
                if (randomWaypointOrder)
                {
                    patrolIndex = Random.Range(0, patrolPoints.Length);
                }
                else
                {
                    patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
                }

                agent.SetDestination(patrolPoints[patrolIndex].position);
            }
            else
            {
                agent.SetDestination(GetRandomNavMeshPosition());
            }
        }

        if (ShouldStartChasing())
            currentState = State.Chase;
    }

    // ---------------- CHASE ---------------- //
    void Chase()
    {
        SetMaterial(chaseMaterial);
        agent.speed = chaseSpeed;
        agent.isStopped = false;

        if (!PlayerInChaseRange() && !PlayerInViewRange())
        {
            currentState = State.Patrol;
            return;
        }

        agent.SetDestination(player.position);

        float dist = Vector3.Distance(transform.position, player.position);

        if (PlayerInViewRange() && Vector3.Distance(transform.position, player.position) <= attackDistance)
        {
            currentState = State.Attack;
        }
    }

    // ---------------- ATTACK ---------------- //
    void Attack()
    {
        SetMaterial(attackMaterial);
        agent.ResetPath();

        if (!PlayerInViewRange())
        {
            currentState = State.Chase;
            return;
        }

        Vector3 lookPos = player.position;
        lookPos.y = transform.position.y;
        transform.LookAt(lookPos);

        if (attackTimer <= 0f)
        {
            Shoot();
            attackTimer = attackCooldown;
        }
    }

    // ---------------- SHOOTING ---------------- //

    void Shoot()
    {
        //Instead use the public methoid from gun
        if (equippedGun != null && enemyFiringPoint != null)
        {
            equippedGun.Fire(firePoint.position, firePoint.rotation, bulletVisualPrefab);
        }
    }



       // ---------------- DETECTION ---------------- //
    bool PlayerInChaseRange()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > chaseDistance) return false;

        return !Physics.Raycast(
            transform.position + eyeOffset,
            (player.position - transform.position).normalized,
            dist,
            obstacleMask
        );

    }

    bool PlayerInViewRange()
    {
        Vector3 dir = player.position - transform.position;
        float dist = dir.magnitude;
        if (dist > viewDistance) return false;

        if (Vector3.Angle(transform.forward, dir) > viewAngle * 0.5f)
            return false;

        return !Physics.Raycast(
            transform.position + eyeOffset,
            dir.normalized,
            dist,
            obstacleMask
            );
    }

    bool ShouldStartChasing()
    {
        return PlayerInChaseRange() || PlayerInViewRange();
    }

    // ---------------- RANDOM NAVMESH PATROL ---------------- //
    Vector3 GetRandomNavMeshPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * randomPatrolRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, randomPatrolRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return transform.position;
    }

    // ---------------- VISUAL HELPER ---------------- //

    void SetMaterial(Material mat)
    {
        if (rend.material != mat)
            rend.material = mat;

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);

        Gizmos.color = Color.cyan;
        Vector3 left = Quaternion.Euler(0, -viewAngle / 2f, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, viewAngle / 2f, 0) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + left * viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + right * viewDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}
