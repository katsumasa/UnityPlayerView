# UnityPlayerView

![GitHub package.json version](https://img.shields.io/github/package-json/v/katsumasa/UnityPlayerView)　　

Viewer that plays the content displayed on the actual device in UnityEditor.

## 概要

実機の画面をUnityEditor上で表示するViewerです。

<img width="800" alt="image" src="https://user-images.githubusercontent.com/29646672/183345036-8f466447-ade5-4391-b5fd-980131b8d269.png">


## セットアップ

UnityPlayerViewのプレハブをSceneに配置してアプリケーションのビルドを行います。


## 使い方

ここではUnityEditor上での使い方を説明します。

### 起動方法

MenuからWindow->UnityPlayerViewでPlayerView Windowが起動します。

### 操作方法

#### Connect To

接続先のデバイスを指定します。接続の仕組みはUnityProfilerと共有していますので、どちらかの接続先を切り替えると、もう片方の接続先も切り替わります。

<img width="20" alt="PlayIcon" src="https://user-images.githubusercontent.com/29646672/137236748-d4c3ad04-c66c-4e42-81f4-547649720f02.png"> 再生の開始/終了</br>
<img width="20" alt="RecIcon" src="https://user-images.githubusercontent.com/29646672/137236785-25596da8-ba35-4cf9-a622-5f2e014baa8a.png"> 録画の開始/終了</br>
<img width="20" alt="ScreenShotIcon" src="https://user-images.githubusercontent.com/29646672/137236826-10a97a17-40b3-41c8-affd-d499e64e7475.png">スクリーンショットを保存する</br>
<img width="20" alt="SaveFolderIcon" src="https://user-images.githubusercontent.com/29646672/137236850-d88a79ec-0e32-46a8-97cd-d736020dd659.png">録画結果の保存先を指定する</br>

#### Enable Async GPU Readback

[Async GPU Readback](https://docs.unity3d.com/ja/2018.4/ScriptReference/Rendering.AsyncGPUReadback.html)の機能を使用して画像処理を行う為、MainThareadの負荷が軽減される場合があります。


#### Enabled Touch Event

View上でのタッチをPlayer側に通知します。(Android プラットフォーム限定)

#### Refresh Interval

画像の転送処理を行う間隔を指定します。
間隔を開けることでCPU負荷が軽減されます。

#### Record Folder

録画時にファイルを書き出す先のフォルダー

#### Record Count Max

録画に何コマ迄録画するかを指定出来ます。
このコマまで録画を行うと録画は自動的に停止します。

#### Record Count

録画内容をSeekすることが出来ます。

<img width="800" alt="UnityChoseKunDemo04" src="https://user-images.githubusercontent.com/29646672/137240645-7e4f1d5d-1214-4247-b846-971e09f852d1.gif">



## 注意事項

- Player Viewerは非常に負荷の高い処理です。Player Inspector->Screen->SetScreenからwidthとheightを調整したり、Refresh Intervalで実行間隔を調整してから再生を実行することを*オススメ*します。

## その他

要望・不具合等ありましたら[Issue](https://github.com/katsumasa/UnityPlayerView/issues)をご利用下さい。可能な限り対応します。

以上
