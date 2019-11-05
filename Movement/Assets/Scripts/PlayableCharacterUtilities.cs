
using UnityEngine;

public static class PlayableCharacterUtilities
{
    static Playable[] characters;
    static int current;

    public static bool isSetup = false;

    public static void Setup()
    {
        FindPlayers();
        SetCurrentPlayer();
        isSetup = true;
    }

    private static void FindPlayers()
    {
        characters = MonoBehaviour.FindObjectsOfType<Playable>();
    }

    private static void SetCurrentPlayer()
    {
        // find the current player
        bool found = false;
        int i = 0;
        while (!found && i < characters.Length)
        {
            if (characters[i].gameObject.CompareTag("Player"))
            {
                current = i;
                found = true;
            }
            else
            {
                i++;
            }
        }

        if (!found)
        {
            Debug.Log("Couldn't find the current player!");
        }

    }

    public static GameObject GetCurrentPlayer()
    {
        return characters[current].gameObject;
    }

    public static GameObject GetNextPlayable()
    {
        int next = current;
        if (current == characters.Length - 1)
        {
            next = 0;
        } else
        {
            next++;
        }
        current = next;
        return characters[current].gameObject;
    }

    public static GameObject GetPreviousPlayable()
    {
        int previous = current;
        if (current == 0)
        {
            previous = characters.Length - 1;
        }
        else
        {
            previous--;
        }
        current = previous;
        return characters[current].gameObject;

    }
}
