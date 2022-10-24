using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using UnityEngine.UI;

public class RememberMeNowScript : MonoBehaviour
{
    [SerializeField] private KMBombInfo bombRef;
    [SerializeField] private KMBombModule bombModuleRef;
    [SerializeField] private KMAudio audioRef;

    [SerializeField] private KMSelectable[] buttons;
    [SerializeField] private TextMesh displayText;
    [SerializeField] private TextMesh timerText;
    [SerializeField] private GameObject[] ledArray;
    [SerializeField] private Material[] lightMaterials;
    private string[] colourStrings = new string[] {"Red", "Blue", "Yellow", "Green", "Orange", "Purple", "Off"};
    private string[] lightColours = new string[] {"Off", "Off"};
    private Color[] colours = new Color[] {Color.red, Color.blue, Color.yellow, Color.green, new Color(1f, 0.6f, 0f), new Color(1f, 0f, 1f)};

    private List<int> acceptedNumbers = new List<int>();
    private List<int> rejectedNumbers = new List<int>();

    private Coroutine timerRef;

    private int stage = 1;
    private bool isValidNumber;
    private int amountOfSameModules = 0;

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
        print(bombRef.GetTime());
        timerRef = StartCoroutine(NewNumberAndTimer());
    }

    private void RestartTimer()
    {
        timerText.text = "---";
        StopCoroutine(timerRef);
        timerRef = StartCoroutine(NewNumberAndTimer());
    }

    IEnumerator NewNumberAndTimer()
    {
        lightColours[0] = colourStrings[6];
        ledArray[0].GetComponent<MeshRenderer>().material = lightMaterials[6];
        ledArray[0].GetComponentInChildren<Light>().color = Color.black;
        lightColours[1] = colourStrings[6];
        ledArray[1].GetComponent<MeshRenderer>().material = lightMaterials[6];
        ledArray[1].GetComponentInChildren<Light>().color = Color.black;

        for (int i = 0; i < 4; i++)
        {
            displayText.text = displayText.text.Remove(i, 1).Insert(i, "-");

            yield return new WaitForSecondsRealtime(0.5f);
        }

        for (int i = 0; i < 4; i++)
        {
            displayText.text = displayText.text.Remove(i, 1).Insert(i, UnityEngine.Random.Range(0, 10).ToString());

            yield return new WaitForSecondsRealtime(0.5f);
        }

        //LED handler
        int temp1 = UnityEngine.Random.Range(0, 6);
        lightColours[0] = colourStrings[temp1];
        ledArray[0].GetComponent<MeshRenderer>().material = lightMaterials[temp1];
        ledArray[0].GetComponentInChildren<Light>().color = colours[temp1];
        if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
        {
            int temp2 = UnityEngine.Random.Range(0, 6);
            while (temp2 == temp1)
            {
                temp2 = UnityEngine.Random.Range(0, 6);
            }

            lightColours[1] = colourStrings[temp2];
            ledArray[1].GetComponent<MeshRenderer>().material = lightMaterials[temp2];
            ledArray[1].GetComponentInChildren<Light>().color = colours[temp2];
        }

        //validity handler
        switch (lightColours[0])
        {
            case "Off":
                isValidNumber = true;
                break;
            case "Red":
                if (rejectedNumbers.Count > 0)
                {
                    isValidNumber = rejectedNumbers[rejectedNumbers.Count - 1] > int.Parse(displayText.text);
                }
                else
                {
                    isValidNumber = false;
                }
                break;
            case "Blue":
                isValidNumber = bombRef.GetStrikes() == 0;
                break;
            case "Yellow":
                isValidNumber = bombRef.GetTime() < 306f;
                break;
            case "Green":
                isValidNumber = bombRef.GetSolvedModuleIDs().Count >= 1;
                break;
            case "Orange":
                isValidNumber = acceptedNumbers.Count % 2 == 0;
                break;
            case "Purple":
                isValidNumber = lightColours[1] != "Off";
                break;
        }

        //Logs the amount of RMN modules are on the bomb
        if (amountOfSameModules == 0)
        {
            for (int i = 0; i < bombRef.GetSolvableModuleNames().Count; i++)
            {
                if (bombRef.GetModuleNames()[i] == "Remember Me Now")
                {
                    amountOfSameModules++;
                }
            }
        }

        timerText.text = (amountOfSameModules * 30).ToString();

        if (timerText.text.Length == 2)
        {
            timerText.text = "0" + timerText.text;
        }
        else if (timerText.text.Length == 1)
        {
            timerText.text = "00" + timerText.text;
        }

        while (true)
        {
            yield return new WaitForSecondsRealtime(1f);

            timerText.text = (int.Parse(timerText.text) - 1).ToString();

            if (timerText.text.Length == 2)
            {
                timerText.text = "0" + timerText.text;
            }
            else if (timerText.text.Length == 1)
            {
                timerText.text = "00" + timerText.text;
            }

            if (timerText.text == "000")
            {
                bombModuleRef.HandleStrike();
                RestartTimer();
            }
        }
    }


    //When a button is pushed
    void PressButton(KMSelectable pressedButton)
    {
        if (pressedButton.name == "Accept_Button")
        {
            if (stage == 1 && timerText.text != "---")
            {
                if (!isValidNumber)
                {
                    bombModuleRef.HandleStrike();
                }
                acceptedNumbers.Add(int.Parse(displayText.text));
                RestartTimer();
                pressedButton.AddInteractionPunch(1f);
            }
            else if (stage == 2)
            {

            }
        }
        else if (pressedButton.name == "Reject_Button")
        {
            if (stage == 1 && timerText.text != "---")
            {
                if (isValidNumber)
                {
                    bombModuleRef.HandleStrike();
                }
                rejectedNumbers.Add(int.Parse(displayText.text));
                RestartTimer();
                pressedButton.AddInteractionPunch(1f);
            }
            else if (stage == 2)
            {

            }
        }
    }
}
