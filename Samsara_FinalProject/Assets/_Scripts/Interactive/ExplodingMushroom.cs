using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingMushroom : MonoBehaviour
{


    const float CHECKING_DISTANCE = 5f;
    const float FADE_TIME = 2f;

    float checkDistance;
    GameObject player;
    float timer;

    Vector3 origScale;

    [Tooltip("If no material is specified here, it will choose the default material of the first child component of this object")]
    public Material origMat;
    Material playerNearMat;
    ParticleSystem myParticle;
    bool canExplode;

    bool StartExpanding;
    bool ShrinkLight;

    BoxCollider myCollider;
    Vector3 origBoxSize;
    Light myLight;

    public MeshRenderer[] rends;

    // Use this for initialization
    void Start()
    {
        origScale = transform.localScale;
        myCollider = GetComponent<BoxCollider>();
        origBoxSize = myCollider.size;
        myParticle = GetComponentInChildren<ParticleSystem>();
        myParticle.Stop();
        StartCoroutine(LoadGameElements());
        origMat = new Material(GetComponentInChildren<MeshRenderer>().material);
        playerNearMat = new Material(origMat);
        playerNearMat.color = new Color(playerNearMat.color.a + 0.5f, playerNearMat.color.g, playerNearMat.color.b);
        rends = GetComponentsInChildren<MeshRenderer>();
        myLight = GetComponent<Light>();
        myLight.enabled = false;
        canExplode = true;
    }

    IEnumerator LoadGameElements()
    {
        yield return new WaitForSeconds(0.2f);
        player = GameController.Instance.GetLevelGameObjectByTag(GameObjectTag.Player);
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            checkDistance = Vector3.Distance(player.transform.position, transform.position);
            if (checkDistance <= CHECKING_DISTANCE && !ShrinkLight)
            {
                timer += Time.deltaTime;
                foreach (MeshRenderer r in rends)
                {
                    r.material.Lerp(r.material, playerNearMat, Time.deltaTime * FADE_TIME); //change to danger color
                }
                //if (!myLight.enabled)
                //{
                //    myLight.enabled = true;
                //}

                //if (myLight.intensity <= 10f)
                //    myLight.intensity += 0.5f;
                //myLight.range += timer;
                transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(2, 2, 2), Time.deltaTime * FADE_TIME); // increase in size
                myCollider.size = Vector3.Lerp(myCollider.size, new Vector3(2, 2, 2), Time.deltaTime * FADE_TIME);


                if (timer >= 1.5f && canExplode) // player has 2 seconds to get away before explosion
                {
                    canExplode = false;
                    timer = 0; // reset so the coroutine doesn't get called multiple times
                    StartCoroutine(StartExplosion());
                }

            }
            else
            {
                timer = 0;
                foreach (MeshRenderer r in rends)
                {
                    r.material.Lerp(r.material, origMat, Time.deltaTime * FADE_TIME); //change to original color
                }
                transform.localScale = Vector3.Lerp(transform.localScale, origScale, Time.deltaTime * FADE_TIME);
                myCollider.size = Vector3.Lerp(myCollider.size, origBoxSize, Time.deltaTime * FADE_TIME);
            }

            //if (StartExpanding)
            //{
            //    timer += Time.deltaTime;
            //    foreach (MeshRenderer r in rends)
            //    {
            //        r.material.Lerp(r.material, playerNearMat, Time.deltaTime * FADE_TIME); //change to danger color
            //    }
            //    if (!myLight.enabled)
            //    {
            //        myLight.enabled = true;
            //    }

            //    if (myLight.intensity <= 10f)
            //    myLight.intensity += 0.5f;
            //    //myLight.range += timer;
            //    transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(2, 2, 2), Time.deltaTime * FADE_TIME); // increase in size
            //    myCollider.size = Vector3.Lerp(myCollider.size, new Vector3(2, 2, 2), Time.deltaTime * FADE_TIME);


            //    if (timer >= 1.5f && canExplode) // player has 2 seconds to get away before explosion
            //    {
            //        canExplode = false;
            //        timer = 0; // reset so the coroutine doesn't get called multiple times
            //        StartCoroutine(StartExplosion());
            //    }

                
            //}
            //else
            //{
            //    if (ShrinkLight)
            //    {
            //        myLight.intensity -= 1.5f;
            //    }
            //}
        }
    }

    IEnumerator StartExplosion()
    {
        myParticle.Play();
        foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>())
        {
            mesh.enabled = false;
        }
        if (Vector3.Distance(player.transform.position, transform.position) <= CHECKING_DISTANCE)
            player.GetComponent<PlayerStatController>().TakeDamage(2);

        foreach (BasicEnemyController item in FindObjectsOfType<BasicEnemyController>())
        {
            if (Vector3.Distance(item.transform.position, transform.position) <= CHECKING_DISTANCE)
                item.CurrentHealth -= 2;
        }

        myCollider.enabled = false;
        //ShrinkLight = true;
        //StartExpanding = false;
        yield return new WaitForSeconds(2.5f);
        gameObject.SetActive(false);
        yield return null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Player")
        {
            canExplode = false;
            timer = 0;
            StartCoroutine(StartExplosion());
        }
    }
}
