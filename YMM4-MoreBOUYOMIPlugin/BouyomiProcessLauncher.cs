using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;

namespace MoreBOUYOMIVoicePlugin;

internal static class BouyomiProcessLauncher
{
    private static readonly SemaphoreSlim launchGate = new(1, 1);

    public static async Task EnsureRunningAsync(string executablePath, string port)
    {
        // 実行ファイル未指定なら、従来どおり手動起動した棒読みちゃんへ接続する。
        if (string.IsNullOrWhiteSpace(executablePath))
            return;

        string fullPath = Path.GetFullPath(executablePath);
        if (!File.Exists(fullPath))
            throw new FileNotFoundException("棒読みちゃんの実行ファイルが見つかりません。設定を確認してください。", fullPath);
        if (!string.Equals(Path.GetExtension(fullPath), ".exe", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("棒読みちゃんの実行ファイル（.exe）を指定してください。");

        if (!int.TryParse(port, out int portNumber) || portNumber is < 1 or > 65535)
            throw new ArgumentException($"HTTPプラグインのポート番号が不正です: {port}");

        await launchGate.WaitAsync();
        try
        {
            if (!IsRunning(fullPath))
            {
                using var process = Process.Start(new ProcessStartInfo(fullPath)
                {
                    WorkingDirectory = Path.GetDirectoryName(fullPath)!,
                    UseShellExecute = true,
                });
                if (process is null)
                    throw new InvalidOperationException("棒読みちゃんを起動できませんでした。");
            }

            // 起動直後にAPIへ送信して失敗しないよう、必要なポートの待ち受けを待つ。
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            try
            {
                while (true)
                {
                    using var connection = new TcpClient();
                    try
                    {
                        await connection.ConnectAsync("localhost", portNumber, timeout.Token);
                        break;
                    }
                    catch (SocketException)
                    {
                        await Task.Delay(200, timeout.Token);
                    }
                }
            }
            catch (OperationCanceledException) when (timeout.IsCancellationRequested)
            {
                throw new TimeoutException("棒読みちゃんのAPIに15秒以内に接続できませんでした。起動状態、HTTPプラグイン、ポート番号を確認してください。");
            }
        }
        finally
        {
            launchGate.Release();
        }
    }

    private static bool IsRunning(string fullPath)
    {
        bool found = false;
        foreach (var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(fullPath)))
        {
            using (process)
            {
                try
                {
                    if (string.Equals(process.MainModule?.FileName, fullPath, StringComparison.OrdinalIgnoreCase))
                        found = true;
                }
                catch (Win32Exception)
                {
                    // 権限不足でパスを取得できない同名プロセスは、二重起動を避けるため起動済みとみなす。
                    found = true;
                }
                catch (InvalidOperationException)
                {
                    // 確認中に終了したプロセスは無視する。
                }
            }
        }
        return found;
    }
}
