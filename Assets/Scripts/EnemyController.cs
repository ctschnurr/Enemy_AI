using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    // enums and variables for the enemy/enemies themselves:
    public enum State
    {
        patrolling,
        chasing,
        tracking,
        searching,
        attacking,
        retreating
    }

    public State state = State.patrolling;
    public UnityEngine.AI.NavMeshAgent enemyAgent;
    GameObject enemyObject;
    GameObject enemyColor;
    Transform parent;
    Transform enemyBase;

    // variables for the patrol waypoints
    Transform nextWaypoint;
    Transform waypoint1;
    Transform waypoint2;
    Transform waypoint3;
    Transform waypoint4;
    public List<Transform> waypointList;

    // variables for player tracking
    GameObject player;
    Vector3 playerPosition = new Vector3(0, 0, 0);
    Vector3 playerLastPosition = new Vector3(0, 0, 0);
    float playerDist;

    Vector3 targetDirection;

    float speed = 50;
    float slowRotate = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        // find the waypoint objects in the Unity scene and associate them with the waypoint variables
        waypoint1 = GameObject.Find("waypoint1").transform;
        waypoint2 = GameObject.Find("waypoint2").transform;
        waypoint3 = GameObject.Find("waypoint3").transform;
        waypoint4 = GameObject.Find("waypoint4").transform;

        // assign the first waypoint to be used to the reusable variable
        nextWaypoint = waypoint1;

        // add the variables to their list
        waypointList.Add(waypoint1);
        waypointList.Add(waypoint2);
        waypointList.Add(waypoint3);
        waypointList.Add(waypoint4);

        // find the enemyBase object in the Unity scene and associate the variable with it
        parent = transform.parent;
        enemyBase = parent.Find("enemyBase").GetComponent<Transform>();

        // find the player object and associate it to the variable
        player = GameObject.Find("Player").gameObject;

        // find the enemy game object's navmesh agent component and assign it to the variable
        enemyAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        enemyObject = transform.gameObject;

        // get the component of the enemy prefab that changes color with state changes
        enemyColor = transform.Find("Color").gameObject;

        slowRotate = slowRotate * Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        // grab player's current position
        playerPosition = player.transform.position;

        switch (state)
        {
            // enemy patrolls the waypoints
            case State.patrolling:
                enemyAgent.SetDestination(nextWaypoint.position);
                if (Vector3.Distance(nextWaypoint.position, transform.position) < 1) nextWaypoint = NextWaypoint(nextWaypoint);
                if (Vector3.Distance(playerPosition, transform.position) < 3) SetState(State.chasing);
                break;

            // chase the player until in attack range
            case State.chasing:
                enemyAgent.SetDestination(playerPosition);
                if (Vector3.Distance(playerPosition, transform.position) < 2) SetState(State.attacking);
                break;

            // if the player exits the line of sight he'll track to the location he last saw the player and then search the area
            case State.tracking:
                enemyAgent.SetDestination(playerLastPosition);
                if (Vector3.Distance(playerLastPosition, transform.position) < 1) SetState(State.searching);
                break;

            // enemy rotates to face the player and stays in attack range unless the player leaves range, then will give chase
            case State.attacking:
                targetDirection = playerLastPosition - transform.position;
                Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, slowRotate, 0.0f);
                transform.rotation = Quaternion.LookRotation(newDirection);
                if (Vector3.Distance(playerPosition, transform.position) > 2)
                {
                    enemyAgent.isStopped = false;
                    SetState(State.chasing);
                }
                break;

            // after searching, if the player was not found the enemy will return to start position
            case State.retreating:
                enemyAgent.SetDestination(enemyBase.position);
                if (Vector3.Distance(enemyBase.position, transform.position) < 1) SetState(State.patrolling);
                if (Vector3.Distance(playerPosition, transform.position) < 3) SetState(State.chasing);
                break;

            // once the enemy tracks the player to their last seen position they search by scanning the area unless player is detected
            case State.searching:
                transform.Rotate(Vector3.up * (speed * Time.deltaTime));
                if (Vector3.Distance(playerPosition, transform.position) < 3) SetState(State.chasing);
                break;

        }
    }

    // this method switches the state and makes any other associated changes, like the enemy mood indicator color
    public void SetState(State input)
    {
        CancelInvoke();
        if (enemyAgent.isStopped == true) enemyAgent.isStopped = false;
        switch (input)
        {
            case State.patrolling:
                state = State.patrolling;
                enemyColor.GetComponent<Renderer>().material.color = Color.green;
                break;

            case State.chasing:
                state = State.chasing;
                enemyColor.GetComponent<Renderer>().material.color = Color.yellow;
                break;

            case State.tracking:
                playerLastPosition = player.transform.position;
                state = State.tracking;
                enemyColor.GetComponent<Renderer>().material.color = Color.blue;
                break;

            case State.attacking:
                playerLastPosition = player.transform.position;
                enemyAgent.isStopped = true;
                enemyColor.GetComponent<Renderer>().material.color = Color.red;
                state = State.attacking;
                break;

            case State.retreating:
                enemyColor.GetComponent<Renderer>().material.color = Color.black;
                state = State.retreating;
                break;

            case State.searching:
                enemyAgent.isStopped = true;
                Invoke("StopSearching", 8);
                enemyColor.GetComponent<Renderer>().material.color = Color.white;
                state = State.searching;
                break;
        }
    }

    void StopSearching()
    {
        if (state == State.searching)
        {
            enemyAgent.isStopped = false;
            SetState(State.retreating);
        }
    }

    // when the enemy reaches a waypoint, this method removes the waypoint from the top of the list and re-adds it to the bottom, then returns the waypoint now on the top of the list
    Transform NextWaypoint(Transform input)
    {
        waypointList.Remove(input);
        waypointList.Add(input);
        
        return waypointList[0];
    }

}
