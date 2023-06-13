using System;
using System.IO;
using BlahEditor.Attributes;
using UnityEngine;

namespace BlahEditor.Screenshoter
{
public class EditorScreenshoter : MonoBehaviour
{
    void Start()
    {
        if (!Application.isEditor)
            Destroy(gameObject);
    }

#if UNITY_EDITOR
    
    [Info("В запущенной игре, можно сделать скрин, нажав F12. В незапущенной игре - по кнопке ниже.")]
    [SerializeField, Button("Take Screenshot", nameof(TakeScreenshot))]
    private bool _editorTakeScreenshotButton;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
            TakeScreenshot();
    }

    private void TakeScreenshot()
    {
        string path = Application.dataPath;
        path = Path.GetDirectoryName(path);
        path = Path.Join(path, "Screenshots");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        string screenshotName = $"Screenshot_{DateTime.Now.ToString()}"
                                .Replace(' ', '_')
                                .Replace(':', '-')
                                .Replace('.', '-');
        ScreenCapture.CaptureScreenshot($"Screenshots/{screenshotName}.png");
        Debug.Log($"Screenshot taken, name: {screenshotName}");
    }
#endif
}
}
