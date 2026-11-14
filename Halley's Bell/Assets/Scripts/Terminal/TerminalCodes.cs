using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class TerminalCode
{
    [SerializeField] public string Name;
    [SerializeField] public string Code;
    [SerializeField] public string Mssg;

    public TerminalCode(string name, string code, string mssg)
    {
        Name = name;
        Code = code;
        Mssg = mssg;
    }
}

[CreateAssetMenu(fileName = "Terminal Codes", menuName = "ScriptableObject/TerminalCodes")]
public class TerminalCodes : ScriptableObject
{
    public List<TerminalCode> codes = new();
}

