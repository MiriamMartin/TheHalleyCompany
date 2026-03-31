using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Terminal : MonoBehaviour, ButtonInterface
{

    [Header("SONAR")]
    public Image sonar;

    [Header("TEXT")]
    public TMP_Text terminalText;
    public TMP_Text display;
    private float textSpeed = 0.07f;
    private bool typingMssg = false;

    private Coroutine idle;
    private bool idleRunning = false;

    private Coroutine displayMssg;
    private float fadeDuration = 1f;

    [Header("NUMPAD")]
    public GameObject Numpad;
    public AudioSource clickPressed;
    public AudioSource clickReleased;
    private bool keyPressed = false;

    private string bootPasscode = "1234";
    private bool bootedPass = false;

    [Header("INCIDENT")]
    public GameObject TerminalCanvas;
    private bool isTerminalOn = false;
    public List<string> radioMessages = new List<string>(); // might change to a file later, but since we only have like 6 messages this works fine
    private int radioMessageIndex = 0;
    private bool started = false;

    [Header("STARTUP")]
    public Radio rad;  // lets us 'connect' radio once password entered

    // Start is called before the first frame update
    void Start()
    {
        terminalText.text = ">";
        //setMssgIndex();
        if (Checkpoints.Instance.getCheckpoint() > 0)
        {
            started = true;
            fixMessages();
        }
    }

    // Update is called once per frame
    void Update()
    {
        SonarRotate();

        if (!typingMssg && !idleRunning)
        {
            idleRunning = true;
            idle = StartCoroutine(IdleType());
        }
        if (Depth.Instance.tunedKeith && !started)  // handles first message
        {
            typeMessage(radioMessages[radioMessageIndex]);
            radioMessageIndex++;
            started = true;
        }
        if (Depth.Instance.radioTrigger || Input.GetKeyDown(KeyCode.K))  // handles all subsequent messages [Delete K input, just for testing]
        {
            typeMessage(radioMessages[radioMessageIndex]);
            radioMessageIndex++;
        }
    }

    void OnMouseOver()
    {
        Scroll("none");
    }

    
    // ========================= SONAR =========================

    public void SonarRotate()
    {
        sonar.transform.Rotate(0, 0, -20f * Time.deltaTime);
    }

    // ========================= TEXT =========================

    public void typeMessage(string mssg)
    {
        // types mssg on terminal screen

        stopIdling();
        typingMssg = true;

        addNewline();

        StartCoroutine(typingEffect(mssg));
        displayMssg = StartCoroutine(displayTranscribing());
    }

    public IEnumerator typingEffect(string mssg)
    {
        // typing effect

        for (int i = 0; i < mssg.Length; i++)
        {
            terminalText.text += mssg[i];
            yield return new WaitForSeconds(textSpeed);
        }

        addNewline();

        StopCoroutine(displayMssg);
        display.text = "";
        typingMssg = false;
    }

    public IEnumerator displayTranscribing()
    {
        display.text = "[ TRANSCRIBING MESSAGE ]";

        while (true)
        {
            yield return StartCoroutine(FadeAlpha(1f, 0.2f));
            yield return StartCoroutine(FadeAlpha(0.2f, 1f));
        }
    }

    public IEnumerator FadeAlpha(float start, float end)
    {
        float timer = 0f;
        Color c = display.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            c.a = Mathf.Lerp(start, end, timer / fadeDuration);
            display.color = c;
            yield return null;
        }
    }

    public void addNewline()
    {
        if (terminalText.text[terminalText.text.Length - 1] != '>')
        {
            terminalText.text += "\n\n>";  // adds space before new message
        }
    }
    public IEnumerator IdleType()
    {
        // Plays the idle animation (flashing underscore)

        terminalText.text += "_";
        yield return new WaitForSeconds(1f);
        Delete(1);
        yield return new WaitForSeconds(1f);
        idleRunning = false;
    }

    public void CheckLastChar()
    {
        // checks if last char is leftover from Idle Anim, if so removes it.

        if (terminalText.text.Length > 0 && terminalText.text[terminalText.text.Length - 1] == '_')
        {
            Delete(1);
        }
    }
    
    public void typeNumpad(string num)
    {
        // types input from the NumPad

        if (num == "Enter")
        {
            CheckLastChar();
            if (CheckPasscode(bootPasscode))
            {
                if (!bootedPass)  // if first time entering password
                {
                    terminalText.text += "\n\n>PASSWORD CORRECT: BOOTING STARTUP PROTOCOL...\n\n> ";
                    rad.ConnectRadio();  // turns radio on
                    terminalText.text += "Hello hello, Operator.\nTune the Radio to 100 whenever you're ready :^D.\n\n> ";
                    bootedPass = true;
                }
                else  // if entering password when already booted systems / started descent
                {
                    terminalText.text += "\n\n>DESCENT IN PROGRESS\n\n> ";
                }
            }
            else
            {
                terminalText.text += "\n\n>PASSWORD INCORRECT\n\n>";
            }

            ShiftLines(2, -1);

        }
        else if (num == "Del")
        {
            Delete(1);
        }
        else
        {
            stopIdling();
            terminalText.text += num;
        }
    }

    public bool CheckPasscode(string psc)
    {
        if (terminalText.text.Length >= psc.Length && terminalText.text.Substring(terminalText.text.Length - psc.Length) == psc)
        {
            return true;
        }
        else { return false; }
    }
        
    public void Scroll(string dir)
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput > 0f || dir == "up")
        {
            ShiftLines(1, -1);
        }
        else if (scrollInput < 0f || dir == "down")
        {
            ShiftLines(1, 1);
        }
    }

    public void ShiftLines(int numLines, int dir)
    {
        terminalText.transform.localPosition += new Vector3(dir * 82f * numLines, 0, 0);
    }

    public void Delete(int numChars)
    {
        if (terminalText.text.Length > numChars - 1 && terminalText.text[terminalText.text.Length - 1] != '>')
        {
            terminalText.text = terminalText.text.Substring(0, terminalText.text.Length - numChars);
        }
        
    }

    public void stopIdling()
    {
        StopCoroutine(idle);
        idleRunning = false;
        CheckLastChar();
    }

    // ========================= Numpad =========================

    public IEnumerator animateOnClick(Transform key)
    {
        // 'Animates' Numpad button press

        key.transform.position += Vector3.down * 0.015f;
        yield return new WaitForSeconds(0.5f);
        key.transform.position += Vector3.up * 0.015f;
    }


    public void Button(bool mouseDown, string message)
    {
        //if (!mouseDown || PauseManager.Instance.getIsPaused()) return;  // only on mousedown, and not while paused

        // For On / Off Button

        if (message == "TerminalPower")
        {
            if (mouseDown && isTerminalOn)
            {
                TerminalOff();
            }
            else if (mouseDown && !isTerminalOn)
            {
                TerminalOn();
            }

            return;
        }

        // For Numpad Keys

        Transform key = Numpad.transform.Find(message);  // gets the key matching the mssg's transform

        if (mouseDown && !keyPressed && !typingMssg)
        {
            keyPressed = true;
            clickPressed.Play();
            key.transform.position += Vector3.down * 0.015f;
            if (isTerminalOn) { typeNumpad(message); }  // only types nums when terminal is on
        }
        else if (keyPressed)
        {
            key.transform.position += Vector3.up * 0.015f;
            //clickReleased.Play();
            keyPressed = false;
        }
    }

    // ========================= Incident Stuff =========================

    public void TerminalOn()
    {
        TerminalCanvas.SetActive(true);
        this.GetComponent<AudioSource>().Play(); // play ambient whirring when on
        isTerminalOn = true;
    }
    public void TerminalOff()
    {
        TerminalCanvas.SetActive(false);
        this.GetComponent<AudioSource>().Stop(); // ambient whirring off
        isTerminalOn = false;
    }

    public bool TerminalPowerStatus()
    {
        return isTerminalOn;
    }


    // ====================== Checkpoints =======================
    public void fixMessages()
    {
        // types all message before next audio clip

        while (radioMessageIndex < getMssgNum())
        {
            terminalText.text += radioMessages[radioMessageIndex];
            terminalText.text += "\n\n>";
            radioMessageIndex++;
        }
    }

    public int getMssgNum()
    {
        int chkptNum = Checkpoints.Instance.getCheckpoint();

        if (chkptNum == 0) { return 0; }
        else if (chkptNum == 1) {  return 1; }
        else if (chkptNum == 2) {  return 1; }
        else if (chkptNum == 3) {  return 3; }
        else if (chkptNum == 4) {  return 4; }
        else if (chkptNum == 5) {  return 6; }
        else if (chkptNum == 6) { return 7; }
        else { return 8; }
    }

}
