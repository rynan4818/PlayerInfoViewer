# PlayerInfoViewer

このBeatSaberプラグインは、ScoreSaberのプレイヤー情報のうちScoreSaber modで表示されない項目を追加表示します。

また、ランクアップした順位や、増加したプレイカウント及び[ScoreSaberRanking](https://github.com/rynan4818/ScoreSaberRanking)のランク表示、増えたpp、PLAYER STATISTICSなど当日に変化のあった差分も表示します。

[HeadDistanceTravelled](https://github.com/denpadokei/HeadDistanceTravelled)を使用している場合は、当日の増加HDTも表示します。

This BeatSaber plug-in additionally displays items of ScoreSaber player information that are not displayed by the ScoreSaber mod.

It also displays the ranks that have increased, the increased play counts and [ScoreSaberRanking](https://github.com/rynan4818/ScoreSaberRanking) ranks, the increased pp, PLAYER STATISTICS, and other differences that have changed on the day of the event. The differences of the changes on the day are also displayed.

If [HeadDistanceTravelled](https://github.com/denpadokei/HeadDistanceTravelled) is used, the increased HDT for the day is also displayed.

![image](https://user-images.githubusercontent.com/14249877/193413697-f5aadd39-aef8-40b5-a3af-59a03a7f320c.png)

表示内容は今後拡張していくつもりです。

あと、表示がテキトーなので、もっとカッコよく表示したい。

We intend to expand the contents of the display in the future.

Also, I would like to make the display more cool, since it is very plain.

# インストール方法 (How to Install)
1. [LeaderboardCore](https://github.com/rithik-b/LeaderboardCore)に依存するので、Mod Assistantでインストールして下さい。[BeatLeader](https://github.com/BeatLeader/beatleader-mod) modをインストールしている人は既に導入されています。
2. [リリースページ](https://github.com/rynan4818/PlayerInfoViewer/releases)から最新のPlayerInfoViewerのリリースをダウンロードします。
3. ダウンロードしたzipファイルを`Beat Saber`フォルダに解凍して、`Plugin`フォルダに`PlayerInfoViewer.dll`ファイルをコピーします。

※デフォルトではAM3時に日付が変更されます。深夜帯にプレイする方は、下記の設定を調整して下さい。

1. depends on [LeaderboardCore](https://github.com/rithik-b/LeaderboardCore), please install it with Mod Assistant. If you have installed [BeatLeader](https://github.com/BeatLeader/beatleader-mod) mod, it is already installed.
2. download the latest PlayerInfoViewer release from [Release Page](https://github.com/rynan4818/PlayerInfoViewer/releases).
3. unzip the downloaded zip file into the `Beat Saber` folder and copy the `Plugin` folder with the `PlayerInfoViewer.dll` file.

※Date changes at 3 AM by default. For those who play during the late night, please adjust the setting values below.

# 設定について (About Settings)
![image](https://user-images.githubusercontent.com/14249877/190883803-ec218fbc-ee05-4e93-85c2-94d0ac65113c.png)

| 項目 | デフォルト値 | 説明 |
|------|--------------|------|
| Date Change Time | 3 | 日付が変更される時刻(hour)です。3 → AM3時 |
| Interval Time | 3 | 前回プレイ時刻から、この時間経過しないと日付が更新されません。 3 → 3時間以上経過 |
| Rank PP | ON | ScoreSaberのランクやPPを表示します |
| Play Count | ON | ScoreSaberのプレイカウントを表示します |
| Player Statistics | ON | BeatSaberのPLAYER STATISTICSを表示します |
| Font Size | 14 | 表示文字のフォントサイズです |
| Y Offset | 0 | 表示文字の高さオフセットです |

| Item | Default Value | Description |
|------|---------------|-------------|
| Date Change Time | 3 | The time (hour) at which the date will be changed. 3 → AM3:00 |
| Interval Time | 3 | The date will not be updated until this amount of time has elapsed since the last time it was played. 3 → More than 3 hours have elapsed |
| Rank PP | ON | Displays ScoreSaber rank and PP |
| Play Count | ON | Displays ScoreSaber play counts |
| Player Statistics | ON | Displays BeatSaber's PLAYER STATISTICS |
| Font Size | 14 | The font size of the displayed text |
| Y Offset | 0 | Display character height offset |
