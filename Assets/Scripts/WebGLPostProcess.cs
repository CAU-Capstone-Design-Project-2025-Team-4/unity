using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Prism3D
{
    public static class WebGLPostProcess
    {
        [PostProcessBuild]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            if (target != BuildTarget.WebGL) return;

            var buildFolder = Path.Combine(pathToBuiltProject, "Build");
            
            File.Move(Path.Combine(buildFolder, "Build.data.br"), Path.Combine(buildFolder, "unity.data.br"));
            File.Move(Path.Combine(buildFolder, "Build.framework.js.br"), Path.Combine(buildFolder, "unity.framework.js.br"));
            File.Move(Path.Combine(buildFolder, "Build.wasm.br"), Path.Combine(buildFolder, "unity.wasm.br"));
            File.Move(Path.Combine(buildFolder, "Build.loader.js"), Path.Combine(buildFolder, "unity.loader.js"));
        }
    }
}