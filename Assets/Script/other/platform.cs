using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class platform : MonoBehaviour {

    private PlatformEffector2D floor;
    private float timer = 0;

	void Start () {
        floor = GetComponent<PlatformEffector2D>();
	}
	
	void Update () {
        if (gameManager.instance.downPlatform)
        {
            floor.useColliderMask = true;
            timer += Time.deltaTime;
            if (timer > 0.5f)
            {
                floor.useColliderMask = false;
                gameManager.instance.downPlatform = false;
                timer =0;
            }
        }
	}
}
