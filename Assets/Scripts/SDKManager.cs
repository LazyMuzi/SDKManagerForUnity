using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;

public class SDKManager : EditorWindow
{
    private bool _isImported = false; // Logger 임포트 여부 확인
    private string _packageUrl;
    private string _clonePath;

    [MenuItem("Tools/SDK Manager")]
    public static void ShowWindow()
    {
        GetWindow<SDKManager>("SDK Manager");
    }

    private void OnGUI()
    {
        GUILayout.Label("SDK Manager", EditorStyles.boldLabel);

        #region Custom
        GUILayout.Label("Package URL (e.g., git@github.com:...):");
        _packageUrl = GUILayout.TextField(_packageUrl);

        // 클론 경로 입력 필드
        GUILayout.Label("Clone Path (relative to Assets):");
        _clonePath = GUILayout.TextField(_clonePath);

        // 다운로드 및 임포트 버튼
        if (GUILayout.Button("Clone and Import Package"))
        {
            if (!string.IsNullOrEmpty(_packageUrl) && !string.IsNullOrEmpty(_clonePath))
            {
                CloneAndImportPackage(_packageUrl, _clonePath);
            }
            else
            {
                Debug.LogError("Package URL or Clone Path is empty. Please enter valid values.");
            }
        }
        #endregion  // 직접 입력
        
        GUILayout.Space(20);

        #region Membership
        GUILayout.Label("Membership");
        GUILayout.BeginHorizontal();
        // 임포트 여부 갱신
        _isImported = Directory.Exists(Path.Combine(Application.dataPath, PathConstant.MembershipPath)) && !CheckEmpty(PathConstant.MembershipPath);

        // 다운로드 및 임포트 버튼
        GUI.enabled = !_isImported; // 이미 임포트된 경우 버튼 비활성화
        if (GUILayout.Button("Import Membership"))
        {
            CloneAndImportPackage(PathConstant.MembershipURL, PathConstant.MembershipPath);
        }

        // 삭제 버튼
        GUI.enabled = _isImported; // 임포트된 경우에만 버튼 활성화
        if (GUILayout.Button("Delete", GUILayout.Width(47)))
        {
            DeletePackage(PathConstant.MembershipPath);
        }
        GUILayout.EndHorizontal();
        #endregion  // Membership
        
        GUILayout.Space(5);

        #region OnePush
        GUILayout.Label("OnePush");
        GUILayout.BeginHorizontal();
        _isImported = Directory.Exists(Path.Combine(Application.dataPath, PathConstant.OnePushPath)) && !CheckEmpty(PathConstant.MembershipPath);

        GUI.enabled = !_isImported;
        if (GUILayout.Button("Import OnePush"))
        {
            CloneAndImportPackage(PathConstant.OnePushURL, PathConstant.OnePushPath);
        }

        GUI.enabled = _isImported;
        if (GUILayout.Button("Delete", GUILayout.Width(47)))
        {
            DeletePackage(PathConstant.OnePushPath);
        }
        GUILayout.EndHorizontal();
        #endregion  // OnePush
        
        GUILayout.Space(5);

        #region MobileNativePopup
        GUILayout.Label("MobileNativePopup");
        GUILayout.BeginHorizontal();
        _isImported = Directory.Exists(Path.Combine(Application.dataPath, PathConstant.MobileNativePopupPath)) && !CheckEmpty(PathConstant.MembershipPath);

        GUI.enabled = !_isImported; 
        if (GUILayout.Button("Import MobileNativePopup"))
        {
            CloneAndImportPackage(PathConstant.MobileNativePopupURL, PathConstant.MobileNativePopupPath);
        }
        
        GUI.enabled = _isImported; 
        if (GUILayout.Button("Delete", GUILayout.Width(47)))
        {
            DeletePackage(PathConstant.MobileNativePopupPath);
        }
        GUILayout.EndHorizontal();
        #endregion  // MobileNativePopup
        
        GUILayout.Space(5);

        #region EventPopup
        GUILayout.Label("EventPopup");
        GUILayout.BeginHorizontal();
        _isImported = Directory.Exists(Path.Combine(Application.dataPath, PathConstant.EventPopupPath)) && !CheckEmpty(PathConstant.MembershipPath);

        GUI.enabled = !_isImported; 
        if (GUILayout.Button("Import EventPopup"))
        {
            CloneAndImportPackage(PathConstant.EventPopupURL, PathConstant.EventPopupPath);
        }
        
        GUI.enabled = _isImported; 
        if (GUILayout.Button("Delete", GUILayout.Width(47)))
        {
            DeletePackage(PathConstant.EventPopupPath);
        }
        GUILayout.EndHorizontal();
        #endregion  // EventPopup
        
        GUILayout.Space(5);

        #region PurchasePopup
        GUILayout.Label("PurchasePopup");
        GUILayout.BeginHorizontal();
        _isImported = Directory.Exists(Path.Combine(Application.dataPath, PathConstant.PurchasePopupPath)) && !CheckEmpty(PathConstant.MembershipPath);

        GUI.enabled = !_isImported; 
        if (GUILayout.Button("Import PurchasePopup"))
        {
            CloneAndImportPackage(PathConstant.PurchasePopupURL, PathConstant.PurchasePopupPath);
        }
        
        GUI.enabled = _isImported; 
        if (GUILayout.Button("Delete", GUILayout.Width(47)))
        {
            DeletePackage(PathConstant.PurchasePopupPath);
        }
        GUILayout.EndHorizontal();
        #endregion  // PurchasePopup
        
        GUILayout.Space(5);
   
        #region I2
        GUILayout.Label("I2");
        GUILayout.BeginHorizontal();
        _isImported = Directory.Exists(Path.Combine(Application.dataPath, PathConstant.I2Path)) && !CheckEmpty(PathConstant.I2Path);

        GUI.enabled = !_isImported; 
        if (GUILayout.Button("Import PurchasePopup"))
        {
            CloneAndImportPackage(PathConstant.I2URL, PathConstant.I2Path);
        }
        
        GUI.enabled = _isImported; 
        if (GUILayout.Button("Delete", GUILayout.Width(47)))
        {
            DeletePackage(PathConstant.I2Path);
        }
        GUILayout.EndHorizontal();
        #endregion  // I2
        
        // 버튼 상태 초기화
        GUI.enabled = true;
    }
    
    private static bool CheckForUpdates(string relativePath)
    {
        string absolutePath = Path.Combine(Application.dataPath, relativePath);
        if (!Directory.Exists(absolutePath)) return false;
        try
        {
            // Git fetch 실행
            Process gitProcess = new Process();
            gitProcess.StartInfo.FileName = "git";
            gitProcess.StartInfo.Arguments = "fetch";
            gitProcess.StartInfo.WorkingDirectory = absolutePath;
            gitProcess.StartInfo.CreateNoWindow = true;
            gitProcess.StartInfo.UseShellExecute = false;
            gitProcess.StartInfo.RedirectStandardOutput = true;
            gitProcess.StartInfo.RedirectStandardError = true;
            gitProcess.Start();
            gitProcess.WaitForExit();
            // 로컬과 원격 브랜치 비교
            Process statusProcess = new Process();
            statusProcess.StartInfo.FileName = "git";
            statusProcess.StartInfo.Arguments = "status -uno";
            statusProcess.StartInfo.WorkingDirectory = absolutePath;
            statusProcess.StartInfo.CreateNoWindow = true;
            statusProcess.StartInfo.UseShellExecute = false;
            statusProcess.StartInfo.RedirectStandardOutput = true;
            statusProcess.StartInfo.RedirectStandardError = true;
            statusProcess.Start();
            string output = statusProcess.StandardOutput.ReadToEnd();
            statusProcess.WaitForExit();
            return output.Contains("behind"); // 원격 저장소와 비교하여 뒤처진 경우 업데이트 필요
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error checking for updates: " + ex.Message);
            return false;
        }
    }

    private static void CloneAndImportPackage(string url, string relativePath)
    {
        string absolutePath = Path.Combine(Application.dataPath, relativePath);

        try
        {
            // 폴더가 이미 존재하면 삭제
            if (Directory.Exists(absolutePath))
            {
                Directory.Delete(absolutePath, true);
                Debug.Log("Existing folder deleted: " + absolutePath);
            }

            // Git 클론 실행
            Process gitProcess = new Process();
            gitProcess.StartInfo.FileName = "git";
            gitProcess.StartInfo.Arguments = $"clone {url} \"{absolutePath}\"";
            gitProcess.StartInfo.CreateNoWindow = true;
            gitProcess.StartInfo.UseShellExecute = false;
            gitProcess.StartInfo.RedirectStandardOutput = true;
            gitProcess.StartInfo.RedirectStandardError = true;

            gitProcess.Start();

            string output = gitProcess.StandardOutput.ReadToEnd();
            string error = gitProcess.StandardError.ReadToEnd();

            gitProcess.WaitForExit();

            if (gitProcess.ExitCode == 0)
            {
                Debug.Log("Git clone successful:\n" + output);
            }
            else
            {
                Debug.LogError("Git clone failed:\n" + error);
                return;
            }

            // 유니티 에셋 임포트
            AssetDatabase.Refresh();
            Debug.Log("Assets imported successfully.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error cloning or importing package: " + ex.Message);
        }
    }

    private static void DeletePackage(string relativePath)
    {
        string absolutePath = Path.Combine(Application.dataPath, relativePath);

        try
        {
            if (Directory.Exists(absolutePath))
            {
                // 폴더 삭제
                Directory.Delete(absolutePath, true);
                Debug.Log("Package successfully deleted: " + absolutePath);

                // 유니티 에셋 데이터베이스 갱신
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogWarning("Package folder does not exist: " + absolutePath);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error deleting package: " + ex.Message);
        }
    }

    private static bool CheckEmpty(string relativePath)
    {
        var path = Path.Combine(Application.dataPath, relativePath);
        return Directory.GetFiles(path).Length.Equals(0) && Directory.GetDirectories(path).Length.Equals(0);
    }
}