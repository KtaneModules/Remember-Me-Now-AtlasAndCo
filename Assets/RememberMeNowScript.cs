using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

public class TemplateScript : MonoBehaviour
{
    [SerializeField] private KMBombInfo bombRef;
    [SerializeField] private KMAudio audioRef;

    [SerializeField] private KMSelectable[] buttons;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake()
    {
        //NEEDED, DONT TOUCH
        moduleId = moduleIdCounter++;

        //Gives each selectable object a function
        foreach (KMSelectable button in buttons)
        {
            button.OnInteract += delegate () { PressButton(button); return false; };
        }
    }

    void Start()
    {
        
    }

    //When a button is pushed
    void PressButton(KMSelectable pressedButton)
    {
        if (pressedButton.name == "Button1")
        {

        }
        else if (pressedButton.name == "Button2")
        {

        }
        else if (pressedButton.name == "Button3")
        {

        }
    }
}
