# Session Recording and Playback

## Prepare your app for session recording and playback
Ensure your app is **deterministic** and behaves exactly the same way across different app launches. That is, your AR experience shouldn't rely on framerate-dependent or time-dependent events (animations, timers, ScrollRect moving with inertia, physics simulation, server connection, etc.). Instead, make sure your AR app only responds to AR Foundation and input events to trigger AR functionality while recording or playing back a session.  
For **example**, the 'ARWorldMapController.cs' is framerate-dependent because the file is streamed from the disk in chunks (line 179):  
`int bytesPerFrame = 1024 * 10;  `  
To make it framerate-independent, we should read the whole file at once while recording or playing back a session.

## How to record a session?
Ensure the Unity Editor connects to the AR Companion app and your AR app works correctly with your remotely connected AR device.
Then, go to the 'Project Settings/XR Plug-in Management/AR Foundation Remote/**Recording and Playback**'_ window and press the **'Record session'** button. Starting the recording will launch the currently open scene. To finish the recording, stop the scene in Unity Editor.

## How to playback the previously recorded session?
Enable the **'Play session record'** toggle, ensure there are no errors in the '**Recording and Playback**' window, and start the scene in Unity Editor.
Session records are not interchangeable between different platforms; that is, it's not possible to playback an ARCore session record in ARKit and vice-versa.

## How to select a session record?
Every scene can have multiple session records associated with it. The plugin takes the first record that matches the current build target of Unity Editor. Place the desired session record on top of the list to play it on the next scene launch.

## Can I assign a session record to a different scene?
In the general case, a session record is only compatible with the original scene. To transfer a session record from the original scene to a new one, these scenes should have identical behaviors: same AR configuration, same input response, and so on.  
To associate a scene with a session record, press the 'plus' button under the '**Scene To Record Associations**' list and set scene and record reference. Disabling the 'Respect Time Scale' setting can increase the compatibility in cases of minor scene differences.    
    
# Google Cloud Anchors
## Installation
Google implemented this feature as an external extension to AR Foundation.  
1. To use Cloud Anchors together with AR Foundation Editor Remote, please clone this branch: https://github.com/KirillKuzyk/arcore-unity-extensions.  
2. Import the cloned repo to your Unity project via '**Package Manager/Add**' package from disk... and Select the '**package.json**' file located at the repo's root folder.
3. Make a new build of the AR Companion app by pressing the 'Installer/Install AR Companion App' button.

## Persistent Cloud Anchors on Android
To test Persistent Cloud Anchors in the Editor, please go to Installer, uncheck the '**Modify App Id**' setting and make a new AR Companion app build.
>Explanation: by default, the plugin modifies the package name while making the AR Companion build so it doesn't override your real app on your AR device. 
		    But Persistent Cloud Anchors on Android require you to use OAuth 2.0 authentication with the original package name. 
		    Unchecking the 'Modify App Id' tells the plugin to use the original package name, but this will override the installed app on your AR device.
        
