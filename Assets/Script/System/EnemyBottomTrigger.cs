using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBottomTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject enemy;

    private void OnTriggerStay2D(Collider2D other)
    {
        enemy.GetComponent<Enemy>().forBottomTrigger("can");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        enemy.GetComponent<Enemy>().forBottomTrigger("cant");
    }
}
