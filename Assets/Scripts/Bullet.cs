using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public float maxDamage = 20f;
    public float speed = 90f;
    float time;
    float damage;
    Transform trans;
    
    private void Start()
    {
        trans = transform;
        time = 0;
        Destroy(gameObject, 2f);
    }

    float dist;
    RaycastHit hit;
    GameObject go;
    private void Update()
    {
        if (Gameplay.instance.isLose) return;
        time += Time.deltaTime;
        dist = speed * Time.deltaTime;
        damage = Mathf.SmoothStep(maxDamage, 0f, time / 2f);
        if (Physics.Raycast(trans.position, trans.forward, out hit, dist))
        {
            go = hit.transform.gameObject;
            switch (go.tag)
            {
                case "Enemy":
                    Enemy e = go.GetComponent<Enemy>();
                    e.health -= (int)damage;
                    break;
            }
            Destroy(gameObject);
        }
        trans.position += trans.forward * dist;
    }
}
