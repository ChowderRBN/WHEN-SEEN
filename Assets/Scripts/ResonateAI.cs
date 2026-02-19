using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class ResonateAI : MonoBehaviour
{
    [Header("References")]
    public Transform target;
    private NavMeshAgent agent;

    [Header("Movement")]
    public float wanderSpeed = 2f;
    public float chaseSpeed = 6f;
    public float fleeSpeed = 8f;

    [Header("Detection")]
    public float hearingThreshold = 5f;
    public float chaseThreshold = 8f;
    public float maxHearingDistance = 30f;

    [Header("Kill Settings")]
    public float killRange = 2f;

    [Header("Patrol Behavior")]
    public bool isPatrolling = true;
    public float patrolRadius = 15f;
    private Vector3 patrolCenter;

    private enum State { Idle, Investigating, Chasing, Fleeing }
    private State currentState = State.Idle;

    private Vector3 investigatePosition;
    private float currentAlertLevel = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Find player
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }

        agent.speed = wanderSpeed;
        patrolCenter = transform.position;
    }

    void Update()
    {
        if (target == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        switch (currentState)
        {
            case State.Idle:
                if (isPatrolling)
                {
                    Patrol();
                }
                else
                {
                    Wander();
                }
                break;

            case State.Investigating:
                // Move to last heard noise position
                if (agent.remainingDistance < 1f)
                {
                    // Reached investigation point, go back to idle
                    currentState = State.Idle;
                    currentAlertLevel = 0f;
                }
                break;

            case State.Chasing:
                // Chase player directly
                agent.SetDestination(target.position);

                // Kill player if in range
                if (distanceToPlayer <= killRange)
                {
                    KillPlayer();
                }
                break;

            case State.Fleeing:
                // Fleeing handled by coroutine
                // Check if reached flee destination
                if (agent.remainingDistance < 1f)
                {
                    Debug.Log("Resonate reached flee destination");
                }
                break;
        }
    }

    public void HearNoise(Vector3 noiseOrigin, float noiseLevel)
    {
        float distance = Vector3.Distance(transform.position, noiseOrigin);

        // Ignore if too far
        if (distance > maxHearingDistance) return;

        // Distance affects perceived noise
        float perceivedNoise = noiseLevel * (1f - (distance / maxHearingDistance));
        currentAlertLevel = Mathf.Max(currentAlertLevel, perceivedNoise);

        if (currentState == State.Fleeing) return; // Don't interrupt fleeing

        if (currentAlertLevel >= chaseThreshold)
        {
            // High noise - start chasing
            StartChasing();
        }
        else if (currentAlertLevel >= hearingThreshold)
        {
            // Medium noise - investigate
            StartInvestigating(noiseOrigin);
        }
    }

    void StartInvestigating(Vector3 position)
    {
        currentState = State.Investigating;
        investigatePosition = position;
        agent.speed = wanderSpeed * 1.5f;
        agent.SetDestination(investigatePosition);

        Debug.Log("Resonate is investigating noise...");
    }

    void StartChasing()
    {
        currentState = State.Chasing;
        agent.speed = chaseSpeed;
        agent.SetDestination(target.position);

        Debug.Log("Resonate is chasing!");
    }

    public void Flee(Vector3 screamOrigin, float fleeDistance)
    {
        Debug.Log($"Resonate.Flee called! From {screamOrigin}, distance {fleeDistance}");

        currentState = State.Fleeing;
        currentAlertLevel = 0f;

        // Calculate flee direction (away from scream)
        Vector3 fleeDirection = (transform.position - screamOrigin).normalized;
        Vector3 fleeTarget = transform.position + fleeDirection * fleeDistance;

        agent.speed = fleeSpeed;

        // Try to find valid NavMesh position
        NavMeshHit hit;
        if (NavMesh.SamplePosition(fleeTarget, out hit, fleeDistance, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            Debug.Log($"Resonate fleeing to {hit.position}!");
        }
        else
        {
            // If can't find valid position, just run in opposite direction
            agent.SetDestination(fleeTarget);
            Debug.Log($"Resonate fleeing (no NavMesh) to {fleeTarget}!");
        }

        StartCoroutine(CalmDownAfterSeconds(5f));
    }

    IEnumerator CalmDownAfterSeconds(float seconds)
    {
        Debug.Log($"Resonate will calm down in {seconds} seconds");
        yield return new WaitForSeconds(seconds);

        currentState = State.Idle;
        currentAlertLevel = 0f;
        agent.speed = wanderSpeed;

        Debug.Log("Resonate calmed down");
    }

    void Patrol()
    {
        if (!agent.hasPath || agent.remainingDistance < 1f)
        {
            Vector3 randomPoint = patrolCenter + Random.insideUnitSphere * patrolRadius;
            randomPoint.y = patrolCenter.y;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, patrolRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
    }

    void Wander()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 10f;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void KillPlayer()
    {
        Debug.Log("PLAYER KILLED!");

        PlayerDeath playerDeath = target.GetComponent<PlayerDeath>();
        if (playerDeath != null)
        {
            playerDeath.Kill();
        }
        else
        {
            // Fallback: reload scene if no PlayerDeath script
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }
    }

    // For backwards compatibility with echo waves
    public void Alert(Vector3 waveOrigin, float waveRadius)
    {
        float dist = Vector3.Distance(transform.position, waveOrigin);
        if (dist <= waveRadius)
        {
            HearNoise(waveOrigin, 8f);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxHearingDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, killRange);

        if (currentState == State.Chasing && target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, target.position);
        }

        if (currentState == State.Fleeing)
        {
            Gizmos.color = Color.blue;
            if (agent != null && agent.hasPath)
            {
                Gizmos.DrawLine(transform.position, agent.destination);
            }
        }
    }
}