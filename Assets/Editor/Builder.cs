using UnityEditor;
using System.Linq;

public class Builder {
    public static void BuildWebGL() {
        var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
        BuildPipeline.BuildPlayer(scenes, "docs", BuildTarget.WebGL, BuildOptions.None);
    }
}
