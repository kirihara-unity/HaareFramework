using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class AddressableLoader : MonoBehaviour
{
    [Header("UI")] public GameObject waitMessage;
    public GameObject downMessage;
    public Slider downSlider;
    public Text sizeInfoText;
    public Text downValText;

    [Header("Label")] public AssetLabelReference defaultLabel;

    private long patchSize;
    private Dictionary<string, long> patchMap = new Dictionary<string, long>();

    void Start()
    {
        waitMessage.SetActive(true);
        downMessage.SetActive(false);

        StartCoroutine(InitAddressable());
        StartCoroutine(CheckUpdateFiles());
    }

    IEnumerator InitAddressable()
    {
        var init = Addressables.InitializeAsync();
        yield return init;
    }
    
    private string GetFileSize(long byteCnt)
    {
        string size = "0 Bytes";

        if (byteCnt >= 1073741824.0)
        {
            size = string.Format("{0:##.##}", byteCnt / 1073741824.0) + " GB";
        }
        else if (byteCnt >= 1048576.0)
        {
            size = string.Format("{0:##.##}", byteCnt / 1048576.0) + " MB";
        }
        else if (byteCnt >= 1024.0)
        {
            size = string.Format("{0:##.##}", byteCnt / 1024.0) + " KB";
        }
        else if (byteCnt > 0 && byteCnt < 1024.0)
        {
            size = byteCnt.ToString() + " Bytes";
        }

        return size;
    }

    public void Button_Down()
    {
        StartCoroutine(PatchFiles());
    }
    
    IEnumerator CheckUpdateFiles()
    {
        var labels = new List<string>() { defaultLabel.labelString };
        patchSize = default;

        foreach (var label in labels)
        {
            var handle = Addressables.GetDownloadSizeAsync(label);

            yield return handle;

            patchSize += handle.Result;
        }

        if (patchSize > decimal.Zero)
        {
            waitMessage.SetActive(false);
            downMessage.SetActive(true);

            sizeInfoText.text = GetFileSize(patchSize);
        }
        else
        {
            downValText.text = "100%";
            downSlider.value = 1;
            yield return new WaitForSeconds(2f);
            //LoadingManager.LoadScene("Main");
        }
    }
    //Download
    IEnumerator PatchFiles()
    {
        var labels = new List<string>() { defaultLabel.labelString };
        patchSize = default;

        foreach (var label in labels)
        {
            var handle = Addressables.GetDownloadSizeAsync(label);

            yield return handle;

            if (handle.Result != decimal.Zero)
            {
                StartCoroutine(DownLoadLabel(label));
            }
        }

        yield return CheckDownLoad();
    }

    IEnumerator DownLoadLabel(string label)
    {
        patchMap.Add(label, 0);

        var handle = Addressables.DownloadDependenciesAsync(label, true);

        while (!handle.IsDone)
        {
            patchMap[label] = handle.GetDownloadStatus().DownloadedBytes;
            yield return new WaitForEndOfFrame();
        }

        patchMap[label] = handle.GetDownloadStatus().TotalBytes;
        Addressables.Release(handle);
    }

    IEnumerator CheckDownLoad()
    {
        var total = 0f;
        downValText.text = "0%";

        while (true)
        {
            total += patchMap.Sum(tmp => tmp.Value);

            downSlider.value = total / patchSize;
            downValText.text = (int)(downSlider.value * 100) + "%";

            if (total == patchSize)
            {
                //LoadingManager.LoadScene("Main");
                break;
            }

            total = 0f;
            yield return new WaitForEndOfFrame();
        }
    }
}