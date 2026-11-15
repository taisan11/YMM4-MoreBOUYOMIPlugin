using System.IO;
using System.Net.Http;
using System.Text.Json;

public class Voice
{
    public int id { get; set; }
    public string kind { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public string alias { get; set; } = string.Empty;
}

public class VoiceListResponse
{
    public required List<Voice> voiceList { get; set; }
}

public static class BOUYOMIPluginHTTPClient
{
    // 通常モードのサーバー側タイムアウト（30秒）より長く待ち、504の応答を受け取る。
    private static readonly HttpClient client = new() { Timeout = TimeSpan.FromSeconds(45) };

    public static async Task TalkAsync(
        string port, string text, int speed, int volume, int voice, int tone, string filename, bool useRawMode)
    {
        string destination = Path.GetFullPath(filename);
        string directory = Path.GetDirectoryName(destination)!;
        Directory.CreateDirectory(directory);
        // YMM4の出力先の拡張子にかかわらず、APIには必ず一意な.wavパスを渡す。
        string source = Path.Combine(directory, $".morebouyomi-{Guid.NewGuid():N}.wav");
        string endpoint = useRawMode ? "speakraw" : "speak";
        string url = $"http://localhost:{port}/{endpoint}?text={Uri.EscapeDataString(text)}"
            + $"&speed={speed}&voice={voice}&filename={Uri.EscapeDataString(source)}";
        // rawは音量・音程の加工に対応していない。省略してAPIの既定値を使う。
        if (!useRawMode)
            url += $"&volume={volume}&tone={tone}";

        try
        {
            using HttpResponseMessage response = await client.GetAsync(url);
            await EnsureSuccessAsync(response, endpoint);
            // APIの成功応答はWAV保存完了後に返るため、ファイルのポーリングは不要。
            if (!File.Exists(source) || new FileInfo(source).Length == 0)
                throw new IOException($"棒読みちゃんがWAVファイルを保存しませんでした: {source}");

            File.Move(source, destination, overwrite: true);
        }
        finally
        {
            // 自分が指定した一時ファイルだけを削除する。サーバー側の作業ファイルには触れない。
            File.Delete(source);
        }
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, string endpoint)
    {
        if (response.IsSuccessStatusCode)
            return;

        string detail = await response.Content.ReadAsStringAsync();
        string hint = endpoint == "speakraw"
            ? " rawモードはAquesTalk系の声専用です。その他の声では設定からrawモードを無効にしてください。"
            : string.Empty;
        throw new HttpRequestException(
            $"棒読みちゃん /{endpoint} が失敗しました（HTTP {(int)response.StatusCode}）。{hint}\n{detail}",
            null, response.StatusCode);
    }

    private static async Task<VoiceListResponse> GetVoiceListAsync(string port)
    {
        using HttpResponseMessage response = await client.GetAsync($"http://localhost:{port}/getvoicelist");
        await EnsureSuccessAsync(response, "getvoicelist");
        string responseBody = await response.Content.ReadAsStringAsync();
        VoiceListResponse? voiceList = JsonSerializer.Deserialize<VoiceListResponse>(responseBody);
        if (voiceList?.voiceList is null)
            throw new InvalidDataException("棒読みちゃんから取得した話者一覧の形式が不正です。");
        return voiceList;
    }

    public static async Task<string[]> GetCharacterNamesAsync(string port)
    {
        VoiceListResponse voiceListResponse = await GetVoiceListAsync(port);
        return voiceListResponse.voiceList.Select(voice => voice.name).ToArray();
    }

    public static async Task<int?> Name2IDAsync(string name, string port)
    {
        VoiceListResponse voiceListResponse = await GetVoiceListAsync(port);
        return voiceListResponse.voiceList.FirstOrDefault(voice => voice.name == name)?.id;
    }
}
