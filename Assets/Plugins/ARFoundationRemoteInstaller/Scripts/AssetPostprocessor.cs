using System.Linq;


namespace ARFoundationRemote.Editor {
    internal class AssetPostprocessor : UnityEditor.AssetPostprocessor {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
            if (importedAssets.Any(_ => _.Contains("ARFoundationRemoteInstaller") && _.Contains($"{nameof(AssetPostprocessor)}.cs"))) {
                ARFoundationRemoteInstaller.log("OnPostprocessAllAssets");
                if (!ARFoundationRemoteInstaller.isInstalled) {
                    ARFoundationRemoteInstaller.installPlugin_internal();
                }
                ARFoundationRemoteInstaller.OnImport();
            }
        }
    }
}
