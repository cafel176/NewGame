using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePos : MonoBehaviour {

    public Transform target;
    public bool cameraActive;
    public bool changeCameraRange;

    public float minX;
    public float maxX;
    public float maxY;

    private GameObject player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag!=null && other.gameObject.tag=="Player")
        {
            player = other.gameObject;
            StartCoroutine(eventMove());
        }
    }

    IEnumerator eventMove()
    {
        gameManager.instance.beBlack(0.02f);
        yield return new WaitForSeconds(1);
        player.transform.position = target.position;
        Camera.main.GetComponent<CameraFollow>().setPlayerPos();
        Camera.main.GetComponent<CameraFollow>().Active = cameraActive;
        if(changeCameraRange)
        {
            Camera.main.GetComponent<CameraFollow>().minX = minX;
            Camera.main.GetComponent<CameraFollow>().maxX = maxX;
            Camera.main.GetComponent<CameraFollow>().maxY = maxY;
        }
        gameManager.instance.beWhite(0.02f);
        yield return new WaitForSeconds(1);
    }
}
