using UnityEngine;
using UnityEngine.AI;

public class NPC_Controller : MonoBehaviour
{
    [Header("Waypoint Settings")]
    [Tooltip("Assign waypoint transforms in order")]
    public Transform[] waypoints;

    [Tooltip("Idle time at each waypoint (seconds)")]
    public float idleTime = 2f;

    private NavMeshAgent agent;
    private Animator animator;
    private int currentWaypointIndex = 0;
    private float idleTimer = 0f;

    private enum State { Walking, Idle }
    private State currentState = State.Idle;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (waypoints.Length > 0)
        {
            GoToNextWaypoint();
        }
        else
        {
            Debug.LogWarning("NPC_Controller: No waypoints assigned.");
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Walking:
                // Check if NPC has reached the destination and stopped moving
                if (!agent.pathPending &&
                    agent.remainingDistance <= agent.stoppingDistance &&
                    agent.velocity.sqrMagnitude < 0.01f)
                {
                    EnterIdleState();
                }
                break;

            case State.Idle:
                idleTimer -= Time.deltaTime;
                if (idleTimer <= 0f)
                {
                    GoToNextWaypoint();
                }
                break;
        }

        // Update animation parameters continuously for smooth blending
        animator.SetBool("isWalking", agent.velocity.sqrMagnitude > 0.01f);
    }

    void GoToNextWaypoint()
    {
        if (waypoints.Length == 0) return;

        agent.SetDestination(waypoints[currentWaypointIndex].position);
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        currentState = State.Walking;
    }

    void EnterIdleState()
    {
        agent.ResetPath();
        idleTimer = idleTime;
        currentState = State.Idle;
    }

    private void OnDrawGizmosSelected()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] && waypoints[i + 1])
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }
    }
}
