using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyController;

public class EnemySightTrigger : MonoBehaviour
{
    GameObject parent;
    EnemyController enemy;

    // Start is called before the first frame update
    void Start()
    {
        enemy = transform.parent.GetComponent<EnemyController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            enemy.SetState(State.chasing);
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            enemy.SetState(State.tracking);
        }
    }
}
