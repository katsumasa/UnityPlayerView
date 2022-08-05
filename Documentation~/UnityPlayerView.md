# UnityPlayerView

<img width="800" alt="PlayerView" src="https://user-images.githubusercontent.com/29646672/137237372-637a0a77-5913-4bfc-835e-03737e0a5013.png">

## Overview

Viewer that plays the content displayed on the actual device in UnityEditor.

## Launch Method

From Menu, choose Window->UnityPlayerView. The PlayerView Window shows up.

## How to operate

### Connect To

Specify the device you want to connect. The connection mechanism is shared with UnityProfiler, so when you switch to one of them, the other one will switch as well.

<img width="20" alt="PlayIcon" src="https://user-images.githubusercontent.com/29646672/137236748-d4c3ad04-c66c-4e42-81f4-547649720f02.png">　Play Begin/End</br>
<img width="20" alt="RecIcon" src="https://user-images.githubusercontent.com/29646672/137236785-25596da8-ba35-4cf9-a622-5f2e014baa8a.png">　Record Begin/End</br>
<img width="20" alt="ScreenShotIcon" src="https://user-images.githubusercontent.com/29646672/137236826-10a97a17-40b3-41c8-affd-d499e64e7475.png">　Save Screenshot</br>
<img width="20" alt="SaveFolderIcon" src="https://user-images.githubusercontent.com/29646672/137236850-d88a79ec-0e32-46a8-97cd-d736020dd659.png">　Specify the path of the recording results</br>

### Enable Async GPU Readback

If you check this box, you will be able to use [Async GPU Readback](https://docs.unity3d.com/2018.4/Documentation/ScriptReference/Rendering.AsyncGPUReadback.html) to process images. This may reduce the load of the MainTharead.

### Reflesh Interval

Specify the process interval images being transfered.
By giving interval, it may lead to reducing CPU load.

### Record Folder

Folder where the recorded results will export to.

### Record Count Max

Specify which frame to start record.
The recording will automatically stop once you have set the frame.

### Record Count

You can seek the recorded result.

<img width="800" alt="UnityChoseKunDemo04" src="https://user-images.githubusercontent.com/29646672/137240645-7e4f1d5d-1214-4247-b846-971e09f852d1.gif">

## Warning

- This is a very high-load process.
- We  __recommend__ you to adjust the width and height in SetScreen or skip frame and press Play.
