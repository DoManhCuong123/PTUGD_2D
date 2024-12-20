using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    public GameObject attackpoint;
    public float radius;
    public LayerMask Player;
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
        Collider2D[] player = Physics2D.OverlapCircleAll(attackpoint.transform.position, radius, Player);
        foreach (Collider2D player2 in player)
        {
            Debug.Log("Hit player");
            player2.GetComponentInChildren<PlayerHealth>().health -= damge;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackpoint.transform.position, radius);
    }
}
