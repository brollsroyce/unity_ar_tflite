# unity_ar_tflite
Unity project with AR plane detection and anchor placement along with ssd object detection via tflite

detection of objects via a Machine Learning (ML) model as well as Augmented Reality (AR) implementation of plane detection, anchor placement and distance measurement. Two separate projects were created, with different outcomes. The outcomes along with descriptions of what can be adjusted to tailor the project to specific requirements for any application are also mentioned in this document, thus providing a framework for using AR+ML. The functionality explained was tested and working on two different Android mobile devices: Samsung s23 Ultra and OnePlus 5T

Unity
Unity version 2021.3.16f1 was used to create the AR+AI framework. 
The plugins used were:
-	[AR: ARFoundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.2/manual/index.html)
-	[ML: tf-lite-unity-sample](https://github.com/asus4/tf-lite-unity-sample)

	![Framework (1)](https://github.com/brollsroyce/unity_ar_tflite/assets/50242741/7ddce113-885e-47b5-a8ed-29b76142fed4)
The image above shows an outline of the framework created in Unity, providing anchor placement and distance measurement through 2 modes. Let’s go through all the steps:

## Ground Plane Detection
AR Foundation has its ground plane detection feature which can be added to the AR Session Origin object in the scene Hierarchy. For it to work properly, it is recommended to also have an AR Raycast Manager and an AR Anchor Manager component attached to the same object, in order to place anchors at the point on the ground that a screen position on a mobile device points to.
Anchors. I created a ScreenToWorldAnchors.cs script which is also attached to the same AR Session Origin object. This component handles the creation & removal of anchors (public functions that can be used in other scripts too) and distance measurement. In this component, the objects required to visualize the anchors placed on the ground plane and the text object that displays the eventually detected distance text between the anchors can also be set.

 ![Screenshot 2023-08-31 112509](https://github.com/brollsroyce/unity_ar_tflite/assets/50242741/3ec5391a-04bb-422a-9d15-b7e15aa510a8)

## TFLITE implementation
The CameraImageSample.cs script gets the raw camera frames which the tflite ssd model can run on. The SsdSpecific.cs script generates the results on each camera frame and displays it on the screen. It also provides the screen position which is essential for placement of anchors via the ScreenToWorldAnchors.cs script. 
 
 ![Screenshot 2023-09-02 161118](https://github.com/brollsroyce/unity_ar_tflite/assets/50242741/50f1cf12-d4d1-4eff-bf04-37f761033ee1)

## Mode Handler
The ModeHandler.cs script handles toggling between the 2 modes: Manual and Automatic. 
In the manual mode, the user can manually place anchors on a detected plane and distance measurement occurs automatically. The first anchor placed is the one for Object 1 (Eg: golf hole) while the corresponding anchors are for object 2 (eg: golf balls).
In the automatic mode, the two objects are detected automatically and appropriate anchors are spawned onto them. There can either be automatic spawning of anchors or spawning of anchors at the detected location upon the press of a button. This can be set in the SsdSpecific component.
