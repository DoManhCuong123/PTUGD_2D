using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPlayer : MonoBehaviour
{
    public GameObject attackpoint;
    public float radius;
    public LayerMask Enemy;
    public float damge;


    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void attack()
    {
        Collider2D[] enemy = Physics2D.OverlapCircleAll(attackpoint.transform.position, radius,Enemy);

        foreach (Collider2D enemyObject in enemy)
        {
            Debug.Log("Hit enemy");
            enemyObject.GetComponent<MonsterHealth>().health -= damge;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackpoint.transform.position, radius);
    }
}
