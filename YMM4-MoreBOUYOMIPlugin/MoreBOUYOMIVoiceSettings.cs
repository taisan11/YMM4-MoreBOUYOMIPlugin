using Newtonsoft.Json;
using YukkuriMovieMaker.Plugin;

namespace MoreBOUYOMIVoicePlugin
{
    /// <summary>
    /// 接続設定と話者一覧のキャッシュ。保存・読み込みはYMM4のSettingsBaseに任せる。
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class MoreBOUYOMIVoiceSettings : SettingsBase<MoreBOUYOMIVoiceSettings>
    {
        private string wavPortNumber = "50088";
        private string bouyomiExecutablePath = string.Empty;
        private bool useRawMode = true;
        private bool isCached;
        private string[] speakers = Array.Empty<string>();

        public override SettingsCategory Category => SettingsCategory.Voice;
        public override string Name => "棒読みちゃん";
        public override bool HasSettingView => true;
        public override object SettingView => new MoreBOUYOMIVoiceSettingView { DataContext = this };

        [JsonProperty]
        public string WavPortNumber
        {
            get => wavPortNumber;
            set
            {
                if (Set(ref wavPortNumber, value))
                    IsCached = false;
            }
        }

        [JsonProperty]
        public string BouyomiExecutablePath
        {
            get => bouyomiExecutablePath;
            set => Set(ref bouyomiExecutablePath, value ?? string.Empty);
        }

        [JsonProperty]
        public bool UseRawMode
        {
            get => useRawMode;
            set => Set(ref useRawMode, value);
        }

        [JsonProperty]
        public bool IsCached { get => isCached; set => Set(ref isCached, value); }

        [JsonProperty]
        public string[] Speakers { get => speakers; set => Set(ref speakers, value ?? Array.Empty<string>()); }

        public override void Initialize()
        {
        }

        /// <summary>
        /// 取得が成功してからキャッシュを差し替え、YMM4の設定ファイルへ保存する。
        /// </summary>
        public async Task UpdateSpeakersAsync()
        {
            await BouyomiProcessLauncher.EnsureRunningAsync(BouyomiExecutablePath, WavPortNumber);
            string[] updatedSpeakers = await BOUYOMIPluginHTTPClient.GetCharacterNamesAsync(WavPortNumber);
            Speakers = updatedSpeakers;
            IsCached = true;
            Save();
        }
    }
}
