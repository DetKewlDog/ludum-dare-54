#if UNITY_EDITOR
using System.Diagnostics;
using UnityEditor;
public class CmdWindow : Editor
{
    [MenuItem("Tools/Open Command Prompt %`")]
    public static void OpenConsole() => Process.Start(new ProcessStartInfo("cmd.exe"));
}
#endif