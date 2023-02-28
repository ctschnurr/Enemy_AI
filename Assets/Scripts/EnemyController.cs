using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    enum State
    {
        patrolling,
        chasing,
        searching,
        attacking,
        retreating
    }

    static State state = State.patrolling;

    public static Vector3 target = new Vector3(0, 0, 0);
    // public static Vector3 waypoint = new Vector3(0, 0, 0);

    public List<Transform> waypoints;

    public UnityEngine.AI.NavMeshAgent enemy;
    float dist;

    GameObject player;
    Transform waypoint;
    Transform waypoint1;
    Transform waypoint2;
    Transform waypoint3;
    Transform waypoint4;


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

        player = GameObject.Find("Player").gameObject;
        UnityEngine.AI.NavMeshAgent enemy = GetComponent<UnityEngine.AI.NavMeshAgent>();


    }

    // Update is called once per frame
    void Update()
    {
        // target = player.transform.position;
        // dist = Vector3.Distance(target, transform.position);

        switch (state)
        {
            case State.patrolling:
                enemy.SetDestination(waypoint.position);
                if (GetDistance(waypoint.position, transform.position) < 1) waypoint = NextWaypoint(waypoint);
                break;

        }


        // if (dist < 5 && dist > 3)
        // {
        //     enemy.SetDestination(target);
        // }

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
