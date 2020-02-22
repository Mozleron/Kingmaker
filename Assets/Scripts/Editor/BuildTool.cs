// (C) king.com Ltd 2018

using UnityEditor;
using UnityEngine;

public static class BuildTool 
{
	[MenuItem("CI/Build/Android")]
	static void BuildAndroid()
	{
        BuildForPlatform(BuildTarget.Android, BuildOptions.None, "build.apk");
	}

    static void BuildForPlatform(BuildTarget target, BuildOptions options, string ending = "")
    {   
        // Get filename.
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("User Aborted");
            return;
        }
            
        string[] levels = new string[] { "Assets/Scenes/Example.unity" };

        BuildPipeline.BuildPlayer(levels, path + ending, target, options);
    }
}
