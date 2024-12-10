using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class PostBuild_Favicon : IPostprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPostprocessBuild(BuildReport report)
    {
        string favicoPath = EditorUtility.OpenFilePanel("Select favicon file:", Directory.GetCurrentDirectory(), "ico");
        string destinationPath = $"{report.summary.outputPath}\\TemplateData\\favicon.ico";

        File.Delete(destinationPath);
        File.Copy(favicoPath, $"{report.summary.outputPath}\\TemplateData\\favicon.ico");
    }
}
