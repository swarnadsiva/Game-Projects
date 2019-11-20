using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTileController : MonoBehaviour
{

    public bool PlayerOnTile;
    public bool StayChangedColor;
    [Tooltip("Check if the tile is visible to the player")]
    public bool IsInvisible;

    [Tooltip("The material to change to when the player steps on this tile.")]
    public Material playerOnTileMaterial;
    [Tooltip("Normal tile material if this is a visible tile.")]
    public Material regularMaterial;
    [Tooltip("If the tile is supposed to be hidden and not visible to the player, this should be the same material for the ground area.")]
    public Material invisibleMaterial; 

    public const float COLOR_CHANGE_SPEED = 2f;
    private Renderer rend;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    // Use this for initialization
    void Start()
    {
        PlayerOnTile = false;
        StayChangedColor = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (!StayChangedColor)
        {

            if (PlayerOnTile)
            {
                rend.material.Lerp(rend.material, playerOnTileMaterial, Time.deltaTime * COLOR_CHANGE_SPEED);

            }
            else
            {
                if (IsInvisible)
                {
                    // go back to the original material 
                    rend.material.Lerp(rend.material, invisibleMaterial, Time.deltaTime * COLOR_CHANGE_SPEED);

                }
                else
                {
                    // go back to the original material 
                    rend.material.Lerp(rend.material, regularMaterial, Time.deltaTime * COLOR_CHANGE_SPEED);
                }

            }
        }
        else
        {
            rend.material.Lerp(rend.material, playerOnTileMaterial, Time.deltaTime * COLOR_CHANGE_SPEED);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GameObjectTag.Player.ToString()))
        {
            PlayerOnTile = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(GameObjectTag.Player.ToString()))
        {
            PlayerOnTile = false;
        }
    }
}
