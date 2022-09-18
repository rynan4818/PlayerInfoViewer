# PlayerInfoViewer

このBeatSaberプラグインは、ScoreSaberのプレイヤー情報のうちScoreSaber modで表示されない項目を追加表示します。

また、ランクアップした順位や、増加したプレイカウント、増えたppなど当日に変化のあった差分も表示します。

This BeatSaber plug-in additionally displays items of ScoreSaber player information that are not displayed by the ScoreSaber mod.

It also displays differences that have changed on the day, such as ranks increased, play counts increased, pp increased, etc.

![image](https://user-images.githubusercontent.com/14249877/189532098-d18e19f2-f866-429d-b0b8-40421dd5586c.png)

表示内容は今後拡張していくつもりです。

私の[ScoreSaberRanking](https://github.com/rynan4818/ScoreSaberRanking)のランク情報とかも表示できるようにするつもりです。

あと、表示がテキトーなので、もっとカッコよく表示したい。

We intend to expand the contents of the display in the future.

I intend to be able to display my [ScoreSaberRanking](https://github.com/rynan4818/ScoreSaberRanking) rank information and so on.

Also, I would like to make the display more cool, since it is very plain.

# インストール方法 (How to Install)
1. [LeaderboardCore](https://github.com/rithik-b/LeaderboardCore)に依存するので、Mod Assistantでインストールして下さい。[BeatLeader](https://github.com/BeatLeader/beatleader-mod) modをインストールしている人は既に導入されています。
2. [リリースページ](https://github.com/rynan4818/PlayerInfoViewer/releases)から最新のPlayerInfoViewerのリリースをダウンロードします。
3. ダウンロードしたzipファイルを`Beat Saber`フォルダに解凍して、`Plugin`フォルダに`PlayerInfoViewer.dll`ファイルをコピーします。

※デフォルトではAM3時に日付が変更されます。深夜帯にプレイする方は、下記の設定値を調整して下さい。

1. depends on [LeaderboardCore](https://github.com/rithik-b/LeaderboardCore), please install it with Mod Assistant. If you have installed [BeatLeader](https://github.com/BeatLeader/beatleader-mod) mod, it is already installed.
2. download the latest PlayerInfoViewer release from [Release Page](https://github.com/rynan4818/PlayerInfoViewer/releases).
3. unzip the downloaded zip file into the `Beat Saber` folder and copy the `Plugin` folder with the `PlayerInfoViewer.dll` file.

※Date changes at 3 AM by default. For those who play during the late night, please adjust the setting values below.

# 設定について (About Settings)
`Beat Saber\UserData`フォルダの`PlayerInfoViewer.json`が設定ファイルです。

| 項目 | デフォルト値 | 説明 |
|------|--------------|------|
| DateChangeTime | 3 | 日付が変更される時刻(hour)です。3 → AM3時 |
| IntervalTime | 3 | 前回プレイ時刻から、この時間経過しないと日付が更新されません。 3 → 3時間以上経過 |

そのうちゲーム内から変更できるようにします。

The configuration file is `PlayerInfoViewer.json` in the `Beat Saber\UserData` folder.

| Item | Default Value | Description |
|------|---------------|-------------|
| DateChangeTime | 3 | The time (hour) at which the date will be changed. 3 → AM3:00 |
| IntervalTime | 3 | The date will not be updated until this amount of time has elapsed since the last time it was played. 3 → More than 3 hours have elapsed |

We will soon be able to change this from within the game.
