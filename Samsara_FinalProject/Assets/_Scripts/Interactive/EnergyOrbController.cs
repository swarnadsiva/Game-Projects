using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyOrbController : MonoBehaviour
{
    public bool CanPickUp;
    public bool CastRay;
    public bool CanHit;
    public bool InStatue = false;
    ParticleSystem hitParticles;
    public ParticleSystem[] statueParticles;

    Vector3 localForward;

    void Start()
    {
        if (!InStatue)
        {
            CanPickUp = true;
            CastRay = false;
            CanHit = true;

            foreach (ParticleSystem part in statueParticles)
            {
                part.Stop();
            }

        }
        else
        {
            CanPickUp = false;
            CastRay = true;
            CanHit = true;
        }
    }

    private void OnEnable()
    {
        hitParticles = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CastRay)
        {
            if (CanHit)
            {
                localForward = transform.TransformVector(transform.worldToLocalMatrix.MultiplyVector(transform.forward));
                RaycastHit outHit;
                if (Physics.Raycast(transform.position, localForward, out outHit))
                {
                    if (outHit.collider.gameObject.tag == GameObjectTag.LightCatcher.ToString())
                    {
                        outHit.collider.gameObject.GetComponent<LightCatcher>().HitNumber++;
                        CanHit = false;
                    }
                }
            }
            foreach (ParticleSystem item in statueParticles)
            {
                if (item.isStopped)
                {
                    item.Play();
                }
            }
        }
        else
        {
            hitParticles.Stop();
        }

    }
}
