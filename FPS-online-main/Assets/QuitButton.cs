using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class QuitButton : MonoBehaviour
{
    // Attach this method to the button's OnClick event in the Unity Editor
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit(); // This will only work in a built application, not in the Unity Editor
    }
}
