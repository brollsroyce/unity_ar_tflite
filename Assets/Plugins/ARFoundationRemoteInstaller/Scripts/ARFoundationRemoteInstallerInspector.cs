#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;
using UnityEditor.PackageManager.UI;
#if AR_FOUNDATION_REMOTE_INSTALLED
using ARFoundationRemote.RuntimeEditor;
#endif


namespace ARFoundationRemote.Editor {
    [CustomEditor(typeof(ARFoundationRemoteInstaller), true), CanEditMultipleObjects]
    internal class ARFoundationRemoteInstallerInspector : UnityEditor.Editor {
        bool isARFoundationEmbedded;
        static bool? isPluginAddedToIgnore;

        void OnEnable() {
            isARFoundationEmbedded = ARFoundationRemoteInstaller.getArf()?.source == PackageSource.Embedded;
            #if AR_FOUNDATION_REMOTE_INSTALLED
            if (isARFoundationEmbedded) {
                FixesForEditorSupport.Apply();
            }
            
            if (InstallerSettings.AddGitIgnore) {
                GitRepoCheck.AddGitIgnore();
                InstallerSettings.AddGitIgnore = false;
            }
            
            isPluginAddedToIgnore = GitRepoCheck.IsPluginAddedToIgnore();
            #endif
            ARFoundationRemoteInstaller.CheckExamplesFolder();
        }

        public override void OnInspectorGUI() {
            if (EditorApplication.isCompiling) {
                return;
            }
            
            DrawDefaultInspector();
            showMethodsInInspector(targets);
        }

        void showMethodsInInspector(params Object[] inspectorTargets) {
            GUILayout.Space(8);
            var instance = inspectorTargets.First() as ARFoundationRemoteInstaller;
            Assert.IsNotNull(instance);

            if (!ARFoundationRemoteInstaller.isInstalled) {
                if (GUILayout.Button("Install Plugin")) {
                    ARFoundationRemoteInstaller.installPlugin_internal();
                }
            } else {
                GUILayout.Space(16);
                GUILayout.Label("AR Companion app", EditorStyles.boldLabel);
                var boldButtonStyle = new GUIStyle(GUI.skin.button) {fontStyle = FontStyle.Bold};
                if (GUILayout.Button("Install AR Companion App", boldButtonStyle)) {
                    CompanionAppInstaller.BuildAndRun(instance.installerSettings);
                }

                if (GUILayout.Button("Build AR Companion and show in folder", new GUIStyle(GUI.skin.button))) {
                    CompanionAppInstaller.Build(instance.installerSettings);
                }

                if (GUILayout.Button("Delete AR Companion app build folder")) {
                    CompanionAppInstaller.DeleteCompanionAppBuildFolder();
                }

                GUILayout.Space(8);
                if (GUILayout.Button("Open Settings", boldButtonStyle)) {
                    #if AR_FOUNDATION_REMOTE_INSTALLED
                    SettingsService.OpenProjectSettings($"Project/XR Plug-in Management/{Runtime.Constants.packageName}");
                    #endif
                }
                
                GUILayout.Space(16);
                GUILayout.Label("Installation", EditorStyles.boldLabel);
                if (GUILayout.Button("Import Samples", boldButtonStyle)) {
                    EditorUtility.DisplayDialog(ARFoundationRemoteInstaller.displayName, $"Please select the '{ARFoundationRemoteInstaller.displayName}' in 'Package Manager', then press the 'Samples/Import' button at the bottom of the plugin's description.", "Ok");
                    Window.Open(ARFoundationRemoteInstaller.pluginId);
                }
                if (isARFoundationEmbedded) {
                    if (GUILayout.Button("Un-embed AR Foundation Package")) {
                        ARFoundationRemoteInstaller.UnEmbedARFoundation();
                    }
                } else {
                    #if !AR_FOUNDATION_5_0_OR_NEWER
                    if (GUILayout.Button("Embed AR Foundation (optional)")) {
                        ARFoundationRemoteInstaller.EmbedARFoundation();
                    }
                    #endif
                    
                    if (GUILayout.Button("Un-install Plugin", new GUIStyle(GUI.skin.button) {normal = {textColor = Color.red}})) {
                        ARFoundationRemoteInstaller.UnInstallPlugin();
                    }
                }
            }

            #if AR_FOUNDATION_REMOTE_INSTALLED
            if (isPluginAddedToIgnore != null) {
                GUILayout.Space(16);
                if (isPluginAddedToIgnore == true) {
                    GUILayout.Label("How to add the plugin to git repository?", EditorStyles.boldLabel);
                    GUILayout.Label("Before adding the plugin to your git repository, please check that:\n" +
                                    "- You've purchased a plugin license for every team member who will use the plugin.\n" +
                                    "- Your git repository is private.", 
                        new GUIStyle(GUI.skin.label) {wordWrap = true, richText = true, margin = new RectOffset(4, 4, 4, 4)});
                    if (GUILayout.Button("Add plugin to git")) {
                        if (GitRepoCheck.RemovePluginFromGitIgnore()) {
                            isPluginAddedToIgnore = false;
                        }
                    }
                } else {
                    GUILayout.Label("Git integration", EditorStyles.boldLabel);
                    if (GUILayout.Button("Ignore plugin in git repository")) {
                        isPluginAddedToIgnore = true;
                        GitRepoCheck.AddGitIgnore();
                    }
                }
            }

            ARFoundationRemoteLoaderSettingsInspector.TryDrawVersionUpgrade();
            #endif
            
            /*GUILayout.Space(100);
            if (GUILayout.Button("DEBUG")) {
            }*/
        }
    }
}
#endif // UNITY_EDITOR
