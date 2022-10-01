using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace PlayerInfoViewer.Configuration
{
    internal class PluginConfig
    {
        public static PluginConfig Instance { get; set; }
        // BSIPAが値の変更を検出し、自動的に設定を保存したい場合は、'virtual'でなければなりません。
        public virtual int DateChangeTime { get; set; } = 3;     //日付変更する時刻(24時間表記)
        public virtual int IntervalTime { get; set; } = 3;       //日付変更に必要なインターバル時間
        public virtual bool ViewPlayCount { get; set; } = true;
        public virtual bool ViewRankPP { get; set; } = true;
        public virtual bool ViewPlayerStatistics { get; set; } = true;
        public virtual float ViewFontSize { get; set; } = 12f;
        public virtual float ViewYoffset { get; set; } = 0;
        public virtual string LastPlayTime { get; set; } = null; //最後に起動した時間
        public virtual bool LastPlayerInfoNoGet { get; set; } = false;  //前回記録のScoreSaber記録が取得できなかったとき
        public virtual string LastGetTime { get; set; } = null;  //前回記録のScoreSaber取得時間
        public virtual string LastGetStatisticsTime { get; set; } = null; //前回記録のStatistics取得時間
        public virtual float LastPP { get; set; } = 0;
        public virtual int LastRank { get; set; } = 0;
        public virtual int LastCountryRank { get; set; } = 0;
        public virtual long LastTotalScore { get; set; } = 0;
        public virtual long LastTotalRankedScore { get; set; } = 0;
        public virtual float LastAverageRankedAccuracy { get; set; } = 0;
        public virtual int LastTotalPlayCount { get; set; } = 0;
        public virtual int LastRankedPlayCount { get; set; } = 0;
        public virtual int LastReplaysWatched { get; set; } = 0;
        public virtual int LastPlayedLevelsCount { get; set; } = 0;
        public virtual int LastClearedLevelsCount { get; set; } = 0;
        public virtual int LastFailedLevelsCount { get; set; } = 0;
        public virtual int LastFullComboCount { get; set; } = 0;
        public virtual float LastTimePlayed { get; set; } = 0;
        public virtual int LastHandDistanceTravelled { get; set; } = 0;
        public virtual float LastHeadDistanceTravelled { get; set; } = 0;
        public virtual float BeforePP { get; set; } = 0;
        public virtual float NowPP { get; set; } = 0;
        /// <summary>
        /// これは、BSIPAが設定ファイルを読み込むたびに（ファイルの変更が検出されたときを含めて）呼び出されます。
        /// </summary>
        public virtual void OnReload()
        {
            // 設定ファイルを読み込んだ後の処理を行う。
        }

        /// <summary>
        /// これを呼び出すと、BSIPAに設定ファイルの更新を強制します。 これは、ファイルが変更されたことをBSIPAが検出した場合にも呼び出されます。
        /// </summary>
        public virtual void Changed()
        {
            // 設定が変更されたときに何かをします。
        }

        /// <summary>
        /// これを呼び出して、BSIPAに値を<paramref name ="other"/>からこの構成にコピーさせます。
        /// </summary>
        public virtual void CopyFrom(PluginConfig other)
        {
            // このインスタンスのメンバーは他から移入されました
        }
    }
}
