using UnityEditor;

public class BuildMenu : EditorWindow
{
    [MenuItem("Build/BuildWebPlayer")]
    static void BuildWebPlayer()
    {
        BuildScript.PerformWebBuild();
    }
    [MenuItem("Build/iOS")]
    static void PerformIOSBuild()
    {
        BuildScript.PerformIOSBuild();
    }
    [MenuItem("Build/android")]
    static void PerformAndroidBuild()
    {
        BuildScript.PerformAndroidBuild();
    }
    [MenuItem("Build/windows")]
    static void PerformWindowsBuild()
    {
        BuildScript.PerformWindowsBuild();
    }
    [MenuItem("Build/OSX")]
    static void PerformMacBuild()
    {
        BuildScript.PerformMacBuild();
    }
}