# PlayerInfoViewer

このBeatSaberプラグインは、ScoreSaberのプレイヤー情報のうちScoreSaber modで表示されない項目を追加表示します。
また、ランクアップした順位や、増加したプレイカウント、増えたppなど当日に変化のあった差分も表示します。

![image](https://user-images.githubusercontent.com/14249877/189532098-d18e19f2-f866-429d-b0b8-40421dd5586c.png)

表示内容は今後拡張していくつもりです。

私の[ScoreSaberRanking](https://github.com/rynan4818/ScoreSaberRanking)のランク情報とかも表示できるようにするつもりです。

あと、表示がテキトーなので、もっとカッコよく表示したい。

# インストール方法
1. [LeaderboardCore](https://github.com/rithik-b/LeaderboardCore)に依存するので、Mod Assistantでインストールして下さい。[BeatLeader](https://github.com/BeatLeader/beatleader-mod) modをインストールしている人は既に導入されています。
2. [リリースページ](https://github.com/rynan4818/PlayerInfoViewer/releases)から最新のPlayerInfoViewerのリリースをダウンロードします。
3. ダウンロードしたzipファイルを`Beat Saber`フォルダに解凍して、`Plugin`フォルダに`PlayerInfoViewer.dll`ファイルをコピーします。

※デフォルトではAM3時に日付が変更されます。深夜帯にプレイする方は、下記の設定値を調整して下さい。

# 設定について
`Beat Saber\UserData`フォルダの`PlayerInfoViewer.json`が設定ファイルです。

| 項目 | デフォルト値 | 説明 |
|------|--------------|------|
| DateChangeTime | 3 | 日付が変更される時刻(hour)です。3 → AM3時 |
| IntervalTime | 3 | 前回プレイ時刻から、この時間経過しないと日付が更新されません。 3 → 3時間以上経過 |

そのうちゲーム内から変更できるようにします。
