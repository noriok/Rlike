using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BuildScript
{
    static string DefineSymbol { get { return "DEBUG"; } }
    static string BundleVersion { get { return "0.0.1"; } }
    static int BundleVersionCode { get { return 1; } }
    static string ProductName { get { return "rlike"; } }
    static string BundleIdentifier { get { return "com.noriok.dev.rogue"; } }
    static bool UseIl2cpp { get { return true; } }

    /// <summary>
    /// Assets/Scenes 直下の *. unity ファイルすべてがビルド対象。
    /// </summary>
    /// <returns></returns>
    static string[] GetScenes()
    {
        var assetsPath = UnityEngine.Application.dataPath;

        // Assets/Scenes 直下にある *.unity は全部ビルド対象。
        var scenesPath = System.IO.Path.Combine(assetsPath, "Scenes");
        var scenes = System.IO.Directory.GetFiles(scenesPath, "*.unity");

        // _Base を先頭にするためソート
        var scenesList = scenes.ToList();
        scenesList.Sort();
        scenes = scenesList.ToArray();

        return scenes;
    }

    public static void PerformWebBuild()
    {
        PlayerSettings.productName = "rlike_web";
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebPlayer, DefineSymbol);
        BuildPipeline.BuildPlayer(GetScenes(), "WebPlayer.unity3d",
                                    BuildTarget.WebPlayer, BuildOptions.Development);
    }

    public static void PerformIOSBuild()
    {
        PlayerSettings.bundleVersion = BundleVersion;
        PlayerSettings.iOS.buildNumber = BundleVersionCode.ToString();
        PlayerSettings.productName = ProductName;
        PlayerSettings.iPhoneBundleIdentifier = BundleIdentifier;
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, DefineSymbol);
        PlayerSettings.iOS.scriptCallOptimization = ScriptCallOptimizationLevel.FastButNoExceptions;
        PlayerSettings.accelerometerFrequency = 0;
        PlayerSettings.iOS.targetResolution = iOSTargetResolution.ResolutionAutoQuality;

        var buildOpts = BuildOptions.Development;
        if (UseIl2cpp)
        {
            PlayerSettings.apiCompatibilityLevel = ApiCompatibilityLevel.NET_2_0_Subset;
            PlayerSettings.SetPropertyInt("ScriptingBackend", 1, BuildTargetGroup.iOS); // IL2CPP
            buildOpts |= BuildOptions.Il2CPP;
            PlayerSettings.SetPropertyInt("Architecture", 2, BuildTargetGroup.iOS); // Universal
        }
        else
        {
            PlayerSettings.apiCompatibilityLevel = ApiCompatibilityLevel.NET_2_0_Subset;
            PlayerSettings.SetPropertyInt("ScriptingBackend", 0, BuildTargetGroup.iOS); // Mono
            PlayerSettings.SetPropertyInt("Architecture", 0, BuildTargetGroup.iOS); // Armv7
        }

        BuildPipeline.BuildPlayer(GetScenes(), "iOSXCodeProject", BuildTarget.iOS, buildOpts);
    }

    public static void PerformAndroidBuild()
    {
        // バイナリのバージョンを設定.
        PlayerSettings.Android.bundleVersionCode = BundleVersionCode;
        PlayerSettings.bundleVersion = BundleVersion;
        PlayerSettings.productName = ProductName;
        PlayerSettings.bundleIdentifier = BundleIdentifier;

        // Other Build Settings
        PlayerSettings.Android.forceInternetPermission = true;
        PlayerSettings.accelerometerFrequency = 0;

        // PUBLISHING SETTINGS
        // Key Store
        PlayerSettings.Android.keystoreName = "PublishFiles/rlike.keystore";
        PlayerSettings.Android.keystorePass = "rlikekeystorepassword";			// パスワード.

        // Key Alias
        PlayerSettings.Android.keyaliasName = "rlike";
        PlayerSettings.Android.keyaliasPass = "rlikekeypassword";				// パスワード.

        // シンボルの定義
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, DefineSymbol);

        // Build
        BuildPipeline.BuildPlayer(GetScenes(), ProductName + ".apk", BuildTarget.Android, BuildOptions.Development);
    }

    public static void PerformWindowsBuild()
    {
        PlayerSettings.productName = ProductName;
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, DefineSymbol);
        BuildPipeline.BuildPlayer(GetScenes(), "./" + ProductName + ".exe", BuildTarget.StandaloneWindows, BuildOptions.Development);
    }
    public static void PerformMacBuild()
    {
        PlayerSettings.productName = ProductName;
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, DefineSymbol);
        BuildPipeline.BuildPlayer(GetScenes(), "./" + ProductName + ".app", BuildTarget.StandaloneOSXUniversal, BuildOptions.Development);
    }

}
