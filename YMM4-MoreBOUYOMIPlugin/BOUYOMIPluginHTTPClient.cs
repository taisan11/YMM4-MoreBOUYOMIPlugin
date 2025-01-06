using System.Text.Json;
using System.Net.Http;
using System.IO;
using Windows.Win32.Foundation;

public class Voice {
    public int id { get; set; }
    public string kind { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public string alias { get; set; } = string.Empty;
}

public class VoiceListResponse {
    public required List<Voice> voiceList { get; set; }
}

public static class BOUYOMIPluginHTTPClient {
    public static async Task<Boolean>TalkAsync(string port, string text, int speed, int volume, int voice, int tone, string filename)
    {
        System.Console.WriteLine("リクエスト送るよ!!");
        try
        {
            string url = $"http://localhost:{port}/speak?text={Uri.EscapeDataString(text)}&speed={speed}&volume={volume}&voice={voice}&tone={tone}&filename={Uri.EscapeDataString(filename + ".wav")}";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            System.Console.WriteLine(response.ToString());
            string responseBody = await response.Content.ReadAsStringAsync();
            System.Console.WriteLine(responseBody);

            string sourceFile = filename + ".wav";
            string destFile = filename;
            int retryCount = 0;
            int maxRetries = 500;
            int delay = 10; // 500ms

            while (!File.Exists(sourceFile) && retryCount < maxRetries)
            {
                await Task.Delay(delay);
                retryCount++;
            }

            if (File.Exists(sourceFile))
            {
                File.Move(sourceFile, destFile);
            }
            else
            {
                throw new FileNotFoundException($"Could not find file '{sourceFile}' after {maxRetries} retries.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return true;
    }

    private static async Task<VoiceListResponse?> GetVoiceListAsync(string port)
    {
        using HttpClient client = new HttpClient();
        try
        {
            string url = $"http://localhost:{port}/getvoicelist";
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            VoiceListResponse? voiceList = JsonSerializer.Deserialize<VoiceListResponse>(responseBody);
            return voiceList;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public static async Task<string[]> GetCharacterNamesAsync(string port)
    {
        VoiceListResponse? voiceListResponse = await GetVoiceListAsync(port);
        if (voiceListResponse != null)
        {
            return voiceListResponse.voiceList.Select(voice => voice.name).ToArray();
        }
        return Array.Empty<string>();
    }

    public static async Task<int?> Name2IDAsync(string name, string port)
    {
        VoiceListResponse? voiceListResponse = await GetVoiceListAsync(port);
        if (voiceListResponse != null)
        {
            Voice? voice = voiceListResponse.voiceList.FirstOrDefault(v => v.name == name);
            if (voice != null)
            {
                return voice.id;
            }
        }
        return null;
    }
}
