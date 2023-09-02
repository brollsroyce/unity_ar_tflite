# Installation
1. Install **AR Foundation >= 3.0.1** and XR providers (**ARKit XR Plugin, ARCore XR Plugin**, etc.) via Package Manager.
2. **Enable** providers in *'Project Settings/XR Plug-in Management'*.   
Please choose providers compatible with your AR Foundation version.  
3. Check that your project compiles and works correctly on your AR device.  
4. Import the plugin into your project.  
The plugin will install itself automatically after importing it to Unity.  
If automatic installation fails, please fix all Console errors, then select an *'Assets/Plugins/ARFoundationRemoteInstaller/Installer.asset'* object and press the *'Install Plugin'* button.  ).  
5. Add this folder to version control ignore list (.gitignore will be modified automatically):  
*'Assets/Plugins/ARFoundationRemoteInstaller'*  
If your repository is **private** AND you've purchased the **license** for every team member who will use the plugin, only then you can add the plugin's source code to the version control system.  
More info about multiple licenses (seats) [here](https://support.unity3d.com/hc/en-us/articles/208601846).  
6. Enable *'**AR Foundation Remote**'* provider in *'Project Settings/XR Plug-in Management/PC, Mac & Linux Standalone'*.  
   If you can't enable this checkbox, please [delete](https://forum.unity.com/threads/xr-plugin-management-cant-enable-plugin.961806/#post-6440849) the file *'Assets/XR/Loaders/AR Foundation Remote Loader.asset'* and try again.
7. Apply [modifications](https://github.com/KirillKuzyk/AR-Foundation-Remote-support/discussions/11) to your project if you're using one of these features:
   - ARKit Mesh Classification.
   - ARKit Face Blendshapes.
   - ARKit World Map.
   - Compass/Location Services (GPS)/Gyroscope.
8. (Optional) Install samples by pressing the *'Installer/**Import Samples**'* button.
            
# How to use
1. Switch your project to the **appropriate** build target (iOS/Android) and press the *'Installer/**Install AR Companion app**'* button.   
2. Run '**AR Companion**' app on your AR device.  
3. Enter 'AR Companion' app's IP in *'Project Settings/XR Plug-in Management/AR Foundation Remote/**AR Companion App IP**'*.  
4. Run AR scene of your choice in the Editor or use one from samples.  
If the connection to your AR device fails, please try to configure Static IP Configuration in Wi-Fi settings or try wired connection (see FAQ section below):  
iOS: https://www.mobi-pos.com/web/guide/settings/static-ip-configuration  
Android: https://service.uoregon.edu/TDClient/2030/Portal/KB/ArticleDet?ID=33742  
5. Leave an **honest review** on the Asset Store and the forum :)  

## Location Services (GPS), Gyroscope, and Compass
To enable Location Services (GPS), Gyroscope, and Compass, please add this line on top of every script that uses UnityEngine.Input:  
`using Input = ARFoundationRemote.Input;`

#### Location Services (GPS) setup
The Location Permission is requested automatically by Unity if a codebase references the UnityEngine.LocationService or UnityEngine.LocationServiceStatus class. To reference these classes in the AR Companion app, please do the following:
1. Add the *'ENABLE_AR_FOUNDATION_REMOTE_LOCATION_SERVICES'* define to *'Project Settings/Player/Scripting Define Symbols'*.
2. Make a new build of the AR Companion app.

#### Compass setup for iOS
Compass on iOS works only after the Location Permission was granted. Please see the 'Location Services (GPS) setup' section.

# FAQ: 
Didn't find the answer in the FAQ section? Ask your question [here](https://github.com/KirillKuzyk/AR-Foundation-Remote-support/discussions). 

**How to update the plugin to a newer version?**  
1. Press the 'Installer/Un-embed AR Foundation Package' button if you previously embedded the AR Foundation package.  
2. Press the 'Installer/Un-install Plugin' button.  
3. Delete the folder 'Assets/Plugins/ARFoundationRemoteInstaller'.
4. Import new plugin version.
5. Make a new build of the AR Companion app by pressing 'Installer/Install AR Companion App' button.  

**How to set up the plugin for multiple team members or Unity Cloud Build?**  
Before adding the plugin to your repository, please check that:
- Your git repository is **private**.
- You've purchased a plugin license **for every** team member who will use the plugin.

Then:
1. Ensure the AR Foundation package is embedded.
2. Push the embedded AR Foundation package to your VCS (version control system) by adding the folder:  
'Packages/com.unity.xr.arfoundation@x.x.x'.
3. Add the 'Plugin/ARFoundationRemoteInstaller' folder to your VCS. If you're using git, press the 'Installer/Add plugin to git' button. 
4. You can also add the 'Plugin/ARFoundationRemoteInstaller/Resources/Settings.asset' to ignore list so multiple people in a team can use their own version of settings.

**How to install the plugin on multiple development machines in an open-source project?**  
Please don't commit plugin files to your public repository. Instead, install the plugin on every development machine separately after purchasing the additional licenses: https://support.unity3d.com/hc/en-us/articles/208601846  

**How to update the AR Foundation to a newer version?**
1. Un-embed the AR Foundation package by pressing the 'Installer/Un-embed AR Foundation'.
2. Install new AR Foundation version via Package Manager window.
3. Press the 'Installer/Embed AR Foundation Package'.
4. Make a new build of the AR Companion app.

**How to temporarily disable the plugin?**  
Please disable the *'**AR Foundation Remote**'* provider in *'Project Settings/XR Plug-in Management/PC, Mac & Linux Standalone'*.

**Can I build the AR Companion app from another (or newly created) project to speed up build time?**  
Yes! Please ensure that your projects:
 - Have the same version of Unity and the same AR packages (AR Foundation, AR Subsystems, etc.). The easiest way to ensure that packages are the same is to copy the 'Packages/manifest.json' file. 
 - Have the same color space. 
 - Use the same 'Active Input Handling'.
 - Use the same Render Pipeline settings.

**How to connect an AR device to Unity Editor by wire?**  
 - iOS + macOS: One of the IP addresses displayed in the AR Companion app is an IP of a wired connection.            
 Please disable Wi-Fi, and there should be one IP left that corresponds to the wired connection.  
 - iOS + Windows: https://forum.unity.com/threads/ar-foundation-editor-remote-test-and-debug-your-ar-project-in-the-editor.898433/page-9#post-6849035  
 - Android + macOS/Windows:  
   Launch the AR Companion app on your Android device. Open the adb tool and execute the following command:  
   adb forward tcp:44819 tcp:44819  
   Where 44819 is a port from the plugin's setting.  
   This command will forward any localhost connection to the connected Android device.  
   More info: https://developer.android.com/studio/command-line/adb#forwardports  
   Enter the localhost IP (127.0.0.1) in the plugin's settings instead of the Wi-Fi IP displayed in the companion app.      

**How to check if the plugin is installed to decouple my project from the plugin?**
1. Create and ***.asmdef** file in your project.
2. Add a **version define** (for example, AR_FOUNDATION_REMOTE_INSTALLED) to created *.asmdef file ([example](https://forum.unity.com/threads/ar-foundation-editor-remote-test-and-debug-your-ar-project-in-the-editor.898433/page-7#post-6561910)).
3. Now you can wrap a plugin-specific code inside an #if/#endif block:
```    
#if AR_FOUNDATION_REMOTE_INSTALLED  
// plugin-specific code goes here  
#endif
```

**My Universal Render Pipeline (URP) project is not working correctly with the plugin.**  
Please check that you've completed all the URP setup steps and make a new build of the AR Companion app: https://forum.unity.com/threads/arfoundation-4-uwrp7-3-unity2019-4-not-working-black-screen-and-no-tracking.915527/#post-6374280

**Image Tracking on Android is working unreliably.**  
The plugin sends reference images from Editor and adds them on AR device at runtime with ScheduleAddImageJob().        While on iOS everything works perfectly fine, image tracking on Android with AR Foundation 4.0.x works unreliably.   
        https://github.com/Unity-Technologies/arfoundation-samples/issues/359   
        https://github.com/Unity-Technologies/arfoundation-samples/issues/586  
 Solutions: 
 - Please use ARCore 3.x.x or ARCore 4.1.x. 
 - Please ensure you've added all your image libraries to 'Embedded Image Libraries' before building the AR Companion app.     

**I'm trying to run AR Foundation Samples scenes, and input remoting doesn't work in SimpleAR and PlaneOcclusion scenes.**  
These scenes use PlaceOnPlane.cs script, which has #if UNITY_EDITOR define. Please remove this define to enable touch input remoting and simulation.  

**How to change video resolution?**  
To change Editor camera background settings, please go to 'Project Settings/XR Plug-in Management/AR Foundation Remote'. Setting a higher resolution scale will result in higher latency and lower frames-per-second.     

**Can I write a custom background video shader and test in the Editor?**
 - iOS + Windows Unity Editor: Make a copy of ARKitBackgroundWindows.shader and modify it as you wish. 
 - iOS + macOS Unity Editor: Make a copy of ARKitBackground.shader and modify it as you wish. Please enable 'Project Settings/Player/PC, Mac & .../Other Settings/Rendering/Metal Editor Support'. 
 - Android: Unity Editor doesn't support shader features specific for Android (GL_OES_EGL_image_external_essl3, samplerExternalOES), so to test your custom video shader in Editor,    this shader should be compatible with both Android and Unity Editor.
            But, as far as I know, it's not possible to write such a shader because Windows/macOS Unity Editor can't run GLSLPROGRAM shaders.  
  
**How to build the AR Companion app with Unity Cloud Build?**
1. Please ensure that your project is closed-source.
2. Add the plugin to your version control system, see 'How to set up the plugin for multiple team members or Unity Cloud Build' section?
3. Go to the Unity Cloud Build console.
4. Setup new build target.
5. Choose the same Unity version as your project uses.
6. Disable 'Auto-build' as your want to build the AR Companion only once.
7. Go to advanced settings.
8. Enable 'Yes, I'd like to make Development builds'.
9. Add 'AR_COMPANION' to Scripting Define Symbols.
10. Add 'Assets/Plugins/ARFoundationRemoteInstaller/Resources/ARCompanion.unity' to Scene List.
11. Build the created target in UCB.

# How to uninstall
1. Press the 'Installer/Delete AR Companion app build folder' button.  
2. Press the 'Installer/Un-embed AR Foundation Package' button.
3. Press the 'Installer/Un-install Plugin' button.
4. Delete the folder: Assets/Plugins/ARFoundationRemoteInstaller.  

# AR Foundation Remote 2.0
Want even more awesome features?  
Upgrade to [**AR Foundation Remote 2.0**](https://assetstore.unity.com/packages/slug/201106) at a discount.
