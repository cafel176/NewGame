using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thread : MonoBehaviour {

    public int damage=10;
    public Vector2 target;
    public float speed=7;

    private Rigidbody2D body;
    private float timer = 0;
    private int attack = 0;

	void Start() {
        body = this.gameObject.GetComponent<Rigidbody2D>();
	}
	
	void Update () {
        timer += Time.deltaTime;
        if (timer > 4)
            Destroy(this.gameObject);

            float r = VectorAngle(new Vector2(-1, 0), target);
            transform.rotation = Quaternion.Euler(0, 0, r);
            body.velocity = target.normalized*speed;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.GetComponent<Player>().takeDamage(damage);
        }
    }

     public float VectorAngle(Vector2 from, Vector2 to)
     {
         float angle;
        
         Vector3 cross = Vector3.Cross(from, to);
         angle = Vector2.Angle(from, to);
         return cross.z < 0 ? -angle : angle;
     }
}
