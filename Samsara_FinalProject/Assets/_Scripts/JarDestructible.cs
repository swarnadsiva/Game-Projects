using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JarDestructible : MonoBehaviour
{
    /// <summary>
    /// Used to change objects into there destroyable form
    /// </summary>


    #region Public Member Variables
    public GameObject destroyable;

    #endregion


    #region Public Member Functions
    public  void DestroyJar()
    {
        Instantiate(destroyable, transform.position, transform.rotation);
        Destroy(gameObject);
    }
    #endregion




}
