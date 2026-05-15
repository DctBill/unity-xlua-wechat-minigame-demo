using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

/// <summary>
/// 使用 YooAsset 标准流程：初始化 → 请求版本 → 更新清单 → <see cref="ResourcePackage.CreateResourceDownloader"/> 边玩边下。
/// 下载走 Unity 侧网络（如 WebGL 下 UnityWebRequest），不再使用微信 WX.DownloadFile。
/// </summary>
public class YooAssetPlayAndDownloadDemo : MonoBehaviour
{
    [Tooltip("与 YooAsset 构建时的包裹名一致")]
    public string packageName = "DefaultPackage";

    [Tooltip("远端资源根 URL，不要以 / 结尾；清单与 bundle 由 Yoo 按 fileName 拼接在此根路径下")]
    public string remoteCdnRoot = "https://127.0.0.1/CDN/WebGL/v1.0";

    [Tooltip("下载器最大并发数")]
    public int downloadingMaxNumber = 10;

    [Tooltip("失败重试次数")]
    public int failedTryAgain = 3;

    [Tooltip("可选，显示下载进度文案")]
    public Text statusText;

    void Awake()
    {
        Application.runInBackground = true;
    }

    IEnumerator Start()
    {
        YooAssets.Initialize();

        var package = YooAssets.TryGetPackage(packageName);
        if (package == null)
            package = YooAssets.CreatePackage(packageName);

        InitializationOperation initOp = null;

#if UNITY_EDITOR
        var buildResult = EditorSimulateModeHelper.SimulateBuild(packageName);
        var editorParams = new EditorSimulateModeParameters();
        editorParams.EditorFileSystemParameters =
            FileSystemParameters.CreateDefaultEditorFileSystemParameters(buildResult.PackageRootDirectory);
        initOp = package.InitializeAsync(editorParams);
#elif UNITY_WEBGL
        var root = remoteCdnRoot.TrimEnd('/');
        var remote = new RemoteServices(root, root);
        var webParams = new WebPlayModeParameters();
        webParams.WebServerFileSystemParameters = FileSystemParameters.CreateDefaultWebServerFileSystemParameters();
        webParams.WebRemoteFileSystemParameters =
            FileSystemParameters.CreateDefaultWebRemoteFileSystemParameters(remote);
        initOp = package.InitializeAsync(webParams);
#else
        var hostRoot = remoteCdnRoot.TrimEnd('/');
        var hostRemote = new RemoteServices(hostRoot, hostRoot);
        var hostParams = new HostPlayModeParameters();
        hostParams.BuildinFileSystemParameters = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
        hostParams.CacheFileSystemParameters =
            FileSystemParameters.CreateDefaultCacheFileSystemParameters(hostRemote);
        initOp = package.InitializeAsync(hostParams);
#endif

        yield return initOp;
        if (initOp.Status != EOperationStatus.Succeed)
        {
            SetStatus("初始化失败: " + initOp.Error);
            yield break;
        }

        YooAssets.SetDefaultPackage(package);

        var versionOp = package.RequestPackageVersionAsync();
        yield return versionOp;
        if (versionOp.Status != EOperationStatus.Succeed)
        {
            SetStatus("请求版本失败: " + versionOp.Error);
            yield break;
        }

        var manifestOp = package.UpdatePackageManifestAsync(versionOp.PackageVersion);
        yield return manifestOp;
        if (manifestOp.Status != EOperationStatus.Succeed)
        {
            SetStatus("更新清单失败: " + manifestOp.Error);
            yield break;
        }

        var downloader = package.CreateResourceDownloader(downloadingMaxNumber, failedTryAgain);
        if (downloader.TotalDownloadCount == 0)
        {
            SetStatus("无需下载（已与清单一致）");
            yield break;
        }

        downloader.DownloadUpdateCallback = data =>
            SetStatus($"下载 {data.CurrentDownloadCount}/{data.TotalDownloadCount}  {data.Progress:P0}");
        downloader.DownloadErrorCallback = data =>
            Debug.LogWarning($"[YooAsset] {data.FileName}: {data.ErrorInfo}");

        downloader.BeginDownload();
        yield return downloader;

        SetStatus(downloader.Status == EOperationStatus.Succeed
            ? "下载完成"
            : "下载失败: " + downloader.Error);
    }

    void Update()
    {
        transform.Rotate(Vector3.up, 60f * Time.deltaTime, Space.World);
    }

    void SetStatus(string msg)
    {
        Debug.Log("[YooAssetPlayAndDownloadDemo] " + msg);
        if (statusText != null)
            statusText.text = msg;
    }

    sealed class RemoteServices : IRemoteServices
    {
        readonly string _main;
        readonly string _fallback;

        public RemoteServices(string main, string fallback)
        {
            _main = main;
            _fallback = fallback;
        }

        public string GetRemoteMainURL(string fileName) => $"{_main}/{fileName}";

        public string GetRemoteFallbackURL(string fileName) => $"{_fallback}/{fileName}";
    }
}
