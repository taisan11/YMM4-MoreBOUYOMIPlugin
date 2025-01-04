using YukkuriMovieMaker.Plugin;
using static BOUYOMIPluginHTTPClient;

namespace MoreBOUYOMIVoicePlugin {
    /// <summary>
    /// 設定クラス
    /// Speaker一覧をキャッシュする
    /// </summary>
    public class MoreBOUYOMIVoiceSettings : SettingsBase<MoreBOUYOMIVoiceSettings> {
        public MoreBOUYOMIVoiceSettingsViewModel viewModel = new MoreBOUYOMIVoiceSettingsViewModel();
        /// <summary>
        /// 設定のカテゴリ
        /// ボイス設定はSettingsCategory.Voice
        /// HasSettingViewがfalseの場合は実装しなくても良い
        /// </summary>
        public override SettingsCategory Category => SettingsCategory.Voice;

        /// <summary>
        /// 設定の名前
        /// HasSettingViewがfalseの場合は実装しなくても良い
        /// </summary>
        public override string Name => "棒読みちゃん";

        /// <summary>
        /// 設定用Viewを持つかどうか
        /// </summary>
        public override bool HasSettingView => true;

        /// <summary>
        /// 設定View (WPFのUserControl)
        /// </summary>
        public override object SettingView => new MoreBOUYOMIVoiceSettingView { DataContext = viewModel };

        bool isCached = false;
        string[] speakers = Array.Empty<string>();
        public bool IsCached { get => isCached; set => Set(ref isCached, value); }
        public string[] Speakers { get => speakers; set => Set(ref speakers, value); }

        /// <summary>
        /// 設定の読み込み後に呼ばれる
        /// </summary>
        public override void Initialize()
        {

        }

        /// <summary>
        /// 話者一覧を更新する
        /// </summary>
        public async Task UpdateSpeakersAsync()
        {
            IsCached = true;
            string[] predefinedSpeakers = await BOUYOMIPluginHTTPClient.GetCharacterNamesAsync(viewModel.PortNumber);
            Speakers = predefinedSpeakers;
        }
    }
}