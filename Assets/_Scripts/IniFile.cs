using System.Runtime.InteropServices;
using System.Text;

using UnityEngine;

public class IniFile
{
    private string path;
    private string fileName;

    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

    public IniFile(string _fileName)
    {
        fileName = _fileName;
        path = Application.persistentDataPath + "/" + fileName + ".ini";
    }

    public string Read(string Key, string DefaultValue, string Section = null)
    {
        var RetVal = new StringBuilder(255);
        GetPrivateProfileString(Section ?? fileName, Key, "", RetVal, 255, path);

        if (RetVal.Length == 0)
        {
            WritePrivateProfileString(Section ?? fileName, Key, DefaultValue, path);
            GetPrivateProfileString(Section ?? fileName, Key, "", RetVal, 255, path);
        }

        return RetVal.ToString();
    }

    public void Write(string Key, string Value, string Section = null)
    {
        WritePrivateProfileString(Section ?? fileName, Key, Value, path);
    }

    public void DeleteKey(string Key, string DefaultValue, string Section = null)
    {
        WritePrivateProfileString(Section ?? fileName, Key, DefaultValue, path);
    }

    public void DeleteSection(string Section = null)
    {
        Write(null, null, Section ?? fileName);
    }

    public bool KeyExists(string Key, string Section = null)
    {
        return Read(Key, Section).Length > 0;
    }
}
