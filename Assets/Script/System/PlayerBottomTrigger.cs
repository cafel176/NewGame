using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBottomTrigger : MonoBehaviour {
    [SerializeField]
    private GameObject player;

    private void OnTriggerStay2D(Collider2D other)
    {
        player.GetComponent<Player>().forBottomTrigger("can");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        player.GetComponent<Player>().forBottomTrigger("cant");
    }
}
