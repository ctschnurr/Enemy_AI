using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum State
    {
        patrolling,
        chasing,
        searching,
        attacking,
        retreating
    }

    public State state = State.patrolling;

    Vector3 playerPosition = new Vector3(0, 0, 0);
    Vector3 playerLastPosition = new Vector3(0, 0, 0);

    public UnityEngine.AI.NavMeshAgent enemy;
    float playerDist;

    GameObject player;
    GameObject enemyColor;

    Transform waypoint;

    Transform waypoint1;
    Transform waypoint2;
    Transform waypoint3;
    Transform waypoint4;

    public List<Transform> waypoints;

    Transform enemyBase;

    // Start is called before the first frame update
    void Start()
    {
        waypoint1 = GameObject.Find("waypoint1").transform;
        waypoint2 = GameObject.Find("waypoint2").transform;
        waypoint3 = GameObject.Find("waypoint3").transform;
        waypoint4 = GameObject.Find("waypoint4").transform;

        waypoint = waypoint1;

        waypoints.Add(waypoint1);
        waypoints.Add(waypoint2);
        waypoints.Add(waypoint3);
        waypoints.Add(waypoint4);

        enemyBase = GameObject.Find("enemyBase").transform;

        player = GameObject.Find("Player").gameObject;
        UnityEngine.AI.NavMeshAgent enemy = GetComponent<UnityEngine.AI.NavMeshAgent>();

        enemyColor = transform.Find("Color").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition = player.transform.position;

        switch (state)
        {
            case State.patrolling:
                enemy.SetDestination(waypoint.position);
                if (GetDistance(waypoint.position, transform.position) < 1) waypoint = NextWaypoint(waypoint);

                if (GetDistance(playerPosition, transform.position) < 6) SetState(State.chasing);
                break;

            case State.chasing:
                enemy.SetDestination(playerPosition);

                if (GetDistance(playerPosition, transform.position) > 8) SetState(State.searching);
                if (GetDistance(playerPosition, transform.position) < 2) SetState(State.attacking);
                break;

            case State.searching:
                enemy.SetDestination(playerLastPosition);

                if (GetDistance(playerLastPosition, transform.position) < 1) SetState(State.retreating);
                if (GetDistance(playerPosition, transform.position) < 8) SetState(State.chasing);

                break;

            case State.attacking:
                enemy.isStopped = true;
                if (GetDistance(playerPosition, transform.position) > 2)
                {
                    enemy.isStopped = false;
                    SetState(State.chasing);
                }
                break;

            case State.retreating:
                enemy.SetDestination(enemyBase.position);
                if (GetDistance(enemyBase.position, transform.position) < 1) SetState(State.patrolling);
                if (GetDistance(playerPosition, transform.position) < 6) SetState(State.chasing);
                break;
        }

    }

    void SetState(State input)
    {
        switch (input)
        {
            case State.patrolling:
                // SetWaypoints();
                state = State.patrolling;
                enemyColor.GetComponent<Renderer>().material.color = Color.green;
                break;

            case State.chasing:
                state = State.chasing;
                enemyColor.GetComponent<Renderer>().material.color = Color.yellow;
                break;

            case State.searching:
                playerLastPosition = player.transform.position;
                state = State.searching;
                enemyColor.GetComponent<Renderer>().material.color = Color.blue;
                break;

            case State.attacking:
                enemyColor.GetComponent<Renderer>().material.color = Color.red;
                state = State.attacking;
                break;

            case State.retreating:
                enemyColor.GetComponent<Renderer>().material.color = Color.white;
                state = State.retreating;
                break;
        }


    }

    float GetDistance(Vector3 end, Vector3 start)
    {
        return Vector3.Distance(end, start);
    }

    Transform NextWaypoint(Transform input)
    {
        waypoints.Remove(input);
        waypoints.Add(input);
        

        return waypoints[0];
    }

}
