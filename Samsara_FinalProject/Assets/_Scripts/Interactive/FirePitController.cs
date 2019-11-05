using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePitController : MonoBehaviour
{

    ParticleSystem[] particles;

    bool SpoutingFire = true;

    // Use this for initialization
    void Start()
    {
        particles = GetComponentsInChildren<ParticleSystem>();
       
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == GameObjectTag.Player.ToString() && SpoutingFire)
        {
            other.GetComponent<PlayerStatController>().TakeDamage(1);
        }

        if (other.gameObject.tag == GameObjectTag.Block.ToString())
        {
            foreach(ParticleSystem part in particles)
            {
                SpoutingFire = false;
                part.Stop();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == GameObjectTag.Block.ToString())
        {
            foreach (ParticleSystem part in particles)
            {
                SpoutingFire = true;
                part.Play();
            }
        }
    }
}
