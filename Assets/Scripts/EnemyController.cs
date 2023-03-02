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

    public static Vector3 playerPos = new Vector3(0, 0, 0);
    public static Vector3 playerLastPos = new Vector3(0, 0, 0);

    public List<Transform> waypoints;
    public List<Vector3> roomSensors;
    public List<Vector3> roomSensorsQueue;
    public List<Vector3> searchTheseRooms;

    public UnityEngine.AI.NavMeshAgent enemy;
    float playerDist;

    GameObject player;
    GameObject enemyColor;

    float playerLastX;
    float playerLastY;
    float playerLastZ;

    Transform waypoint;

    Transform waypoint1;
    Transform waypoint2;
    Transform waypoint3;
    Transform waypoint4;

    Vector3 nextRoom = new Vector3(0, 0, 0);

    Vector3 roomSensor1 = new Vector3(0, 0, 0);
    Vector3 roomSensor2 = new Vector3(0, 0, 0);
    Vector3 roomSensor3 = new Vector3(0, 0, 0);
    Vector3 roomSensor4 = new Vector3(0, 0, 0);

    Vector3 roomSensor5 = new Vector3(0, 0, 0);
    Vector3 roomSensor6 = new Vector3(0, 0, 0);
    Vector3 roomSensor7 = new Vector3(0, 0, 0);
    Vector3 roomSensor8 = new Vector3(0, 0, 0);

    Vector3 roomSensor9 = new Vector3(0, 0, 0);
    Vector3 roomSensor10 = new Vector3(0, 0, 0);
    Vector3 roomSensor11 = new Vector3(0, 0, 0);
    Vector3 roomSensor12 = new Vector3(0, 0, 0);

    Vector3 roomSensor13 = new Vector3(0, 0, 0);
    Vector3 roomSensor14 = new Vector3(0, 0, 0);
    Vector3 roomSensor15 = new Vector3(0, 0, 0);
    Vector3 roomSensor16 = new Vector3(0, 0, 0);


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

        roomSensor1 = GameObject.Find("roomSensor1").transform.position;
        roomSensor2 = GameObject.Find("roomSensor2").transform.position;
        roomSensor3 = GameObject.Find("roomSensor3").transform.position;
        roomSensor4 = GameObject.Find("roomSensor4").transform.position;

        roomSensor5 = GameObject.Find("roomSensor5").transform.position;
        roomSensor6 = GameObject.Find("roomSensor6").transform.position;
        roomSensor7 = GameObject.Find("roomSensor7").transform.position;
        roomSensor8 = GameObject.Find("roomSensor8").transform.position;

        roomSensor9 = GameObject.Find("roomSensor9").transform.position;
        roomSensor10 = GameObject.Find("roomSensor10").transform.position;
        roomSensor11 = GameObject.Find("roomSensor11").transform.position;
        roomSensor12 = GameObject.Find("roomSensor12").transform.position;

        roomSensor13 = GameObject.Find("roomSensor13").transform.position;
        roomSensor14 = GameObject.Find("roomSensor14").transform.position;
        roomSensor15 = GameObject.Find("roomSensor15").transform.position;
        roomSensor16 = GameObject.Find("roomSensor16").transform.position;

        roomSensors.Add(roomSensor1);
        roomSensors.Add(roomSensor2);
        roomSensors.Add(roomSensor3);
        roomSensors.Add(roomSensor4);

        roomSensors.Add(roomSensor5);
        roomSensors.Add(roomSensor6);
        roomSensors.Add(roomSensor7);
        roomSensors.Add(roomSensor8);

        roomSensors.Add(roomSensor9);
        roomSensors.Add(roomSensor10);
        roomSensors.Add(roomSensor11);
        roomSensors.Add(roomSensor12);

        roomSensors.Add(roomSensor13);
        roomSensors.Add(roomSensor14);
        roomSensors.Add(roomSensor15);
        roomSensors.Add(roomSensor16);

        player = GameObject.Find("Player").gameObject;
        UnityEngine.AI.NavMeshAgent enemy = GetComponent<UnityEngine.AI.NavMeshAgent>();

        enemyColor = transform.Find("Color").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = player.transform.position;
        // dist = Vector3.Distance(target, transform.position);

        switch (state)
        {
            case State.patrolling:
                enemy.SetDestination(waypoint.position);
                if (GetDistance(waypoint.position, transform.position) < 1) waypoint = NextWaypoint(waypoint);

                if (GetDistance(playerPos, transform.position) < 5) SetState(State.chasing);
                break;

            case State.chasing:
                enemy.SetDestination(playerPos);

                if (GetDistance(playerPos, transform.position) > 8) SetState(State.searching);
                break;

            case State.searching:
                enemy.SetDestination(nextRoom);
                if (GetDistance(nextRoom, transform.position) < 1) nextRoom = NextRoom(nextRoom);
                if (nextRoom == transform.position) SetState(State.patrolling);
                if (GetDistance(playerPos, transform.position) < 5) SetState(State.chasing);

                break;
        }

    }

    Vector3 SavePlayerPos()
    {
        playerLastX = player.transform.position.x;
        playerLastY = player.transform.position.y;
        playerLastZ = player.transform.position.z;
        
        playerLastPos = new Vector3(playerLastX, playerLastY, playerLastZ);
        
        return playerLastPos;

        
    }

    void CalculateNearestRoom()
    {
        Vector3 nearest = new Vector3(0, 0, 0);
        float minDist = Mathf.Infinity;

        foreach (Vector3 room in roomSensorsQueue)
        {
            float dist = GetDistance(room, transform.position);
            if (dist < minDist)
            {
                nearest = room;
                minDist = dist;
            }
        }

        searchTheseRooms.Add(nearest);
        roomSensorsQueue.Remove(nearest);
    }

    void CalculateSearchRoute()
    {
        searchTheseRooms.Clear();
        roomSensorsQueue = roomSensors;

        playerLastPos = player.transform.position;
        searchTheseRooms.Add(playerLastPos);

        while (searchTheseRooms.Count < 4)
        {
            CalculateNearestRoom();
        }


        nextRoom = searchTheseRooms[0];
    }

    void SetState(State input)
    {
        switch (input)
        {
            case State.patrolling:
                state = State.patrolling;
                enemyColor.GetComponent<Renderer>().material.color = Color.green;
                break;

            case State.chasing:
                state = State.chasing;
                enemyColor.GetComponent<Renderer>().material.color = Color.red;
                break;

            case State.searching:
                CalculateSearchRoute();
                state = State.searching;
                enemyColor.GetComponent<Renderer>().material.color = Color.yellow;
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

    Vector3 NextRoom(Vector3 input)
    {
        searchTheseRooms.Remove(input);
        if (searchTheseRooms.Count == 0) return transform.position;
        else return searchTheseRooms[0];
    }


}
