using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public GameObject target;
    public GameObject enemy;

    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.tag == "Player") 
        //{
        //    var component = enemy.AddComponent<ChaseController>();
        //    component.target = target.transform;

        //    Destroy(this);
        //}
    }
}
