using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{
    public List<GameObject> SelectableButtons;
    int Selected;
    bool CanSelect = true;

    IEnumerator DelaySelect()
    {
        SelectableButtons[Selected].GetComponent<Button>().Select();
        yield return new WaitForSeconds(1f);
        CanSelect = true;
    }

    IEnumerator MenuChange(int input)
    {
        print(Selected);

        if (input < 0 && Selected < SelectableButtons.Count - 1)
        {
            Selected++;
        }
        else if (input > 0 && Selected > 0)
        {
            Selected--;
        }
        StartCoroutine(DelaySelect());
        yield return null;
    }

    public static bool CheckPause()
    {
        switch (GameController.Instance.currentGameOptions.inputSetting)
        {
            case InputType.Controller:
                return (Input.GetKeyDown(KeyCode.JoystickButton9)); // options button
            case InputType.Keyboard:
                return (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape));
            default:
                return false;
        }
    }

    public static bool CheckGodMode()
    {
        switch (GameController.Instance.currentGameOptions.inputSetting)
        {
            case InputType.Controller:
                return (Input.GetKeyDown(KeyCode.JoystickButton8)); // Share button
            case InputType.Keyboard:
                return Input.GetKeyDown(KeyCode.G);
            default:
                return false;
        }
    }

    public static bool CheckCanPickupObject()
    {
        switch (GameController.Instance.currentGameOptions.inputSetting)
        {
            case InputType.Controller:
                return (Input.GetKeyDown(KeyCode.JoystickButton1)); // X button
            case InputType.Keyboard:
                return (Input.GetKeyDown(KeyCode.Q));
                
            default:
                return false;
        }
    }

    public static bool CheckClimb()
    {
        switch (GameController.Instance.currentGameOptions.inputSetting)
        {
            case InputType.Controller:
                return (Input.GetKeyDown(KeyCode.JoystickButton1)); //X button
            case InputType.Keyboard:
                return Input.GetKeyDown(KeyCode.E);
            default:
                return false;
        }
    }

    public static bool CheckCanPlaceOrb()
    {
        switch (GameController.Instance.currentGameOptions.inputSetting)
        {
            case InputType.Controller:
                return (Input.GetKeyDown(KeyCode.JoystickButton1)); // X button
            case InputType.Keyboard:
                return Input.GetKeyDown(KeyCode.Space);
            default:
                return false;
        }
    }

    public static bool CheckCanRotateStatue()
    {
        switch (GameController.Instance.currentGameOptions.inputSetting)
        {
            case InputType.Controller:
                return (Input.GetKeyDown(KeyCode.JoystickButton1)); // X button
            case InputType.Keyboard:
                return Input.GetKeyDown(KeyCode.Q);
            default:
                return false;
        }
    }

    public static bool CheckLightAttack()
    {
        switch (GameController.Instance.currentGameOptions.inputSetting)
        {
            case InputType.Controller:
                return (Input.GetKeyDown(KeyCode.JoystickButton0)); // Square button
            case InputType.Keyboard:
                return (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButton(0));
            default:
                return false;
        }
    }

    public static bool CheckHeavyAttack()
    {
        switch (GameController.Instance.currentGameOptions.inputSetting)
        {
            case InputType.Controller:
                return (Input.GetKeyDown(KeyCode.JoystickButton3)); // Triangle button
            case InputType.Keyboard:
                return (Input.GetKey(KeyCode.C) || Input.GetMouseButton(1));
            default:
                return false;
        }
    }

    public static bool CheckLockOnTarget()
    {
        switch (GameController.Instance.currentGameOptions.inputSetting)
        {
            case InputType.Controller:
                return (Input.GetKey(KeyCode.JoystickButton4)); // L1 button
            case InputType.Keyboard:
                return Input.GetKey(KeyCode.E);
            default:
                return false;
        }
    }


    public static bool CheckRangedAttack()
    {
        switch (GameController.Instance.currentGameOptions.inputSetting)
        {
            case InputType.Controller:
                return (Input.GetKeyDown(KeyCode.JoystickButton5)); // R1 button
            case InputType.Keyboard:
                return Input.GetKey(KeyCode.R);
            default:
                return false;
        }
    }

    public static bool CheckDropObject()
    {
        switch (GameController.Instance.currentGameOptions.inputSetting)
        {
            case InputType.Controller:
                return (Input.GetKeyDown(KeyCode.JoystickButton2)); // Circle button
            case InputType.Keyboard:
                return Input.GetKeyDown(KeyCode.Space);
            default:
                return false;
        }
       
    }

    public static bool CheckDodge()
    {
        switch (GameController.Instance.currentGameOptions.inputSetting)
        {
            case InputType.Controller:
                return (Input.GetKeyDown(KeyCode.JoystickButton11)); // click right joystick
            case InputType.Keyboard:
                return Input.GetKeyDown(KeyCode.Z);
            default:
                return false;
        }

    }

    public static bool CheckTeleport()
    {
        switch (GameController.Instance.currentGameOptions.inputSetting)
        {
            case InputType.Controller:
                return (Input.GetKeyDown(KeyCode.JoystickButton1)); // X button
            case InputType.Keyboard:
                return Input.GetKeyDown(KeyCode.Q);
            default:
                return false;
        }
    }

    public static bool CheckCloseDialogueMessage()
    {
        switch (GameController.Instance.currentGameOptions.inputSetting)
        {
            case InputType.Controller:
                return (Input.GetKeyDown(KeyCode.JoystickButton1)); // X button
            case InputType.Keyboard:
                return (Input.GetKeyDown(KeyCode.Space)); 
            default:
                return false;
        }
    }

    public static bool CheckSelectRightPauseMenu()
    {
        //only use for controller input!
        if (GameController.Instance.currentGameOptions.inputSetting == InputType.Controller)
        {
            return (Input.GetKeyDown(KeyCode.JoystickButton5)); // R1
        }
        else
        {
            return false;
        }
    }

    public static bool CheckSelectLeftPauseMenu()
    {
        //only use for controller input!
        if (GameController.Instance.currentGameOptions.inputSetting == InputType.Controller)
        {
            return (Input.GetKeyDown(KeyCode.JoystickButton4)); // L1
        }
        else
        {
            return false;
        }
    }

    public static bool CheckMenuGoBack()
    {
        switch (GameController.Instance.currentGameOptions.inputSetting)
        {
            case InputType.Controller:
                return (Input.GetKeyDown(KeyCode.JoystickButton2)); // Circle button
            case InputType.Keyboard:
                return (Input.GetKeyDown(KeyCode.Escape));
            default:
                return false;
        }
    }

    public static bool CheckSkipCutscene()
    {
        switch (GameController.Instance.currentGameOptions.inputSetting)
        {
            case InputType.Controller:
                return (Input.GetKeyDown(KeyCode.JoystickButton1)); // X button
            case InputType.Keyboard:
                return (Input.GetKeyDown(KeyCode.Space));
            default:
                return false;
        }
    }
}
