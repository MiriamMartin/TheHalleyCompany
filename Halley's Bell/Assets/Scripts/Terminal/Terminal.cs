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
    private float textSpeed = 0.065f;
    private bool typingMssg = false;

    private Coroutine idle;
    private bool idleRunning = false;

    private Coroutine displayMssg;
    private float fadeDuration = 1f;

    private int numLines = 0;
    private float lineHeight = 107.4219f;
    private int totalLineOffset = 4;
    private float addOffset = 0f;

    [Header("NUMPAD")]
    public GameObject Numpad;
    public AudioSource clickPressed;
    private bool keyPressed = false;

    private string bootPasscode = "5437";
    private bool bootedPass = false;

    [Header("INCIDENT")]
    private bool isTerminalOn = false;
    public List<string> radioMessages = new List<string>(); // might change to a file later, but since we only have like 6 messages this works fine
    private int radioMessageIndex = 0;
    private bool started = false;

    [Header("STARTUP")]
    public Radio rad;  // lets us 'connect' radio once password entered
    public GameObject TermBootCanv;

    [Header("SCREENS")]
    public Camera TermCam;  // for hallucination
    public GameObject TerminalCanvas;  // current canvas
    public GameObject TermTextCanv;
    public GameObject TermDepthCanv;
    public GameObject TermSonarCanv;

    [Header("AUDIO")]
    public AudioSource bootupAudio;
    public AudioSource whirringAudio;
    public AudioSource messageBeepAudio;
    public List<AudioClip> messageTones = new List<AudioClip>();  // tone[0] = radio, tone[1] = terminal bark

    [Header("BARKS")]
    public List<string> storyBarks = new List<string>();
    public List<int> storyBarkTriggers = new List<int>();
    private int barksIndex = 0;
    private string lineBRK = "-_-_-_-_-_-_-_-_-_-_-_-_-_-_";

    [Header("BLACKOUT")]
    private bool canTurnOn = true;

    // Start is called before the first frame update
    void Start()
    {
        if (Checkpoints.Instance.getCheckpoint() > 0)
        {
            terminalText.text = ">";
            started = true;
            bootedPass = true;
            fixMessages();
            barksIndex = SetBarkIndex();
        }
        else
        {
            terminalText.text = ">Enter Passcode: "; // Enter passcode so easier to see when starting?
        }
    }

    // Update is called once per frame
    void Update()
    {
        SonarRotate();
        CheckBarks();

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
        if (Depth.Instance.radioTrigger)  // handles all subsequent messages
        {
            Depth.Instance.radioTrigger = false;
            messageBeepAudio.clip = messageTones[0];
            typeMessage(radioMessages[radioMessageIndex]);
            radioMessageIndex++;
        }

        if (Depth.Instance.runBlackout && !Depth.Instance.FirstBlackoutDone)
        {
            canTurnOn = false;
            TerminalOff();
        }

        if (Depth.Instance.FirstBlackoutDone && !canTurnOn)
        {
            canTurnOn = true;
        }

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

        PlayMessageBeep();
        yield return new WaitUntil(() => !messageBeepAudio.isPlaying);

        for (int i = 0; i < mssg.Length; i++)
        {
            terminalText.text += mssg[i];
            AutoScroll();
            yield return new WaitForSeconds(textSpeed);
        }

        addNewline();
        ShiftLines(2, -1);

        StopCoroutine(displayMssg);
        display.text = "";
        typingMssg = false;
    }

    public void AutoScroll()
    {
        if (!isTerminalOn) { return; }

        int nowLines = terminalText.textInfo.lineCount;

        float scrolledLines = Mathf.Abs(Mathf.Floor(terminalText.transform.localPosition.x / lineHeight));

        if (nowLines > 9 && numLines != nowLines && (scrolledLines < (numLines - totalLineOffset)))
        {
            ShiftLines(1, -1);
            numLines++;
        }
        else if (numLines != nowLines)
        {
            numLines++;
        }
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
        else if (num == "up" || num == "down")
        {
            // ignore
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
        //  Scroll up or down

        if (TerminalCanvas != TermTextCanv) { return; }  // no scroll if on Depth or Sonar screen

        if (dir == "up")
        {
            ShiftLines(1, 1);
        }
        else if (dir == "down") 
        {
            ShiftLines(1, -1);
        }
    }

    public void ShiftLines(int numLines, int dir)
    {
        terminalText.transform.localPosition += new Vector3(dir * (lineHeight + addOffset) * numLines, 0, 0);
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
            else if (mouseDown && !isTerminalOn && canTurnOn)
            {
                TerminalOn();
            }

            return;
        }

        // For Numpad Keys

        Transform key = Numpad.transform.Find(message);  // gets the key matching the mssg's transform

        if (mouseDown && !keyPressed)
        {
            keyPressed = true;
            clickPressed.Play();
            key.transform.position += Vector3.down * 0.015f;
            if (isTerminalOn && !typingMssg) { typeNumpad(message); }  // only types nums when terminal is on
        }
        else if (keyPressed)
        {
            key.transform.position += Vector3.up * 0.015f;
            keyPressed = false;
        }
    }

    // ========================= Incident Stuff =========================

    public void TerminalOn()
    {
        bootupAudio.Play();
        TerminalCanvas = TermTextCanv; // ALWAYS bootup on text screen
        StartCoroutine(BootSeq());
    }

    public IEnumerator BootSeq()
    {
        TermBootCanv.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        TermBootCanv.SetActive(false);
        TerminalCanvas.SetActive(true);
        isTerminalOn = true;
        whirringAudio.Play();
    }

    public void TerminalOff()
    {
        TerminalCanvas.SetActive(false);
        whirringAudio.Stop();
        isTerminalOn = false;
    }

    public bool TerminalPowerStatus()
    {
        return isTerminalOn;
    }

    // ====================== screens =======================

    public void ChangeScreens(string scrn)
    {
        if (!isTerminalOn) { return; }  // only change screens when terminal is on

        if (scrn == "text" && TerminalCanvas != TermTextCanv)
        {   
            TerminalCanvas = TermTextCanv;
            TermTextCanv.SetActive(true);
            TermDepthCanv.SetActive(false);
            TermSonarCanv.SetActive(false);
        }
        else if (scrn == "depth" && TerminalCanvas != TermDepthCanv)
        {
            TerminalCanvas = TermDepthCanv;
            TermDepthCanv.SetActive(true);
            TermTextCanv.SetActive(false);
            TermSonarCanv.SetActive(false);
        }
        else if (scrn == "sonar" && TerminalCanvas != TermSonarCanv)
        {
            TerminalCanvas = TermSonarCanv;
            TermSonarCanv.SetActive(true);
            TermTextCanv.SetActive(false);
            TermDepthCanv.SetActive(false);
        }
    }

    // ====================== Barks =======================

    public void CheckBarks()
    {
        if (Depth.Instance.getDepth() >= storyBarkTriggers[barksIndex] && !typingMssg)
        {
            messageBeepAudio.clip = messageTones[1];
            messageBeepAudio.Play();

            var tt = terminalText.text;

            if (tt.Substring(tt.Length - 2) == ">_")
            {
                terminalText.text = tt.Remove(terminalText.text.Length - 1);
                terminalText.text = tt.Remove(terminalText.text.Length - 1);
            }
            else if (tt.Substring(tt.Length - 1) == ">")
            {
                terminalText.text = tt.Remove(terminalText.text.Length - 1);
            }
            else
            {
                terminalText.text += "\n";
            }

            terminalText.text += lineBRK + "\n\n";
            terminalText.text += ">" + storyBarks[barksIndex];
            terminalText.text += "\n\n" + lineBRK + "\n\n>";
            
            barksIndex++;
        }
    }

    public int SetBarkIndex()
    {
        int chkptNum = Checkpoints.Instance.getCheckpoint();

        if (chkptNum == 0) { return 0; }
        else if (chkptNum == 1) { return 0; }
        else if (chkptNum == 2) { return 1; }
        else if (chkptNum == 3) { return 2; }
        else if (chkptNum == 4) { return 3; }
        else if (chkptNum == 5) { return 4; }
        else if (chkptNum == 6) { return 5; }
        else { return 8; }
    }

    // ====================== sounds =======================

    public void PlayMessageBeep()
    {
        messageBeepAudio.Play();
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
