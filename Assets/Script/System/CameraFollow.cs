using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//相机跟随
public class CameraFollow : MonoBehaviour {

    public float minX;
    public float maxX;
    public float maxY;
    public bool Active = false;
    public GameObject player;
    public GameObject camera2;
    public float followSpeed=20;

    private void Update()
    {
        if (Active)
        {
            Vector2 result = Vector2.Lerp(transform.position, player.transform.position+new Vector3(0,2,0), followSpeed * Time.deltaTime);
            if(result.x>minX && result.x<maxX)
                if (result.y <= maxY)
                    transform.position = new Vector3(result.x, result.y, transform.position.z);
                else
                    transform.position = new Vector3(result.x, maxY, transform.position.z);
            else if (result.x <= minX)
                if (result.y <= maxY)
                    transform.position = new Vector3(minX, result.y, transform.position.z);
                else
                    transform.position = new Vector3(minX, maxY, transform.position.z);
            else if (result.x >= maxX)
                if (result.y <= maxY)
                    transform.position = new Vector3(maxX, result.y, transform.position.z);
                else
                    transform.position = new Vector3(maxX, maxY, transform.position.z);

        }
    }

    public void setPlayerPos()
    {
        float i = player.transform.position.x;
        float j = player.transform.position.y;
        if (i < minX)
            i = minX;
        else if (i > maxX)
            i = maxX;
        if (j > maxY-2)
            j = maxY;
        else
            j += 2;
        transform.position = new Vector3(i, j, transform.position.z);
    }

    public void changeCamera(bool take)
    {
        camera2.SetActive(take);
    }
}
