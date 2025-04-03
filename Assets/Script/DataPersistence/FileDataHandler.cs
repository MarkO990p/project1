using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";
    private bool useEncryption = false;
    private string backupExtension = ".bak";  // สำหรับไฟล์สำรอง

    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
    }

    // ฟังก์ชันโหลดข้อมูล
    public GameData Load(string profileId, bool allowRestoreFromBackup = true)
    {
        if (profileId == null) { return null; }

        string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
        GameData loadedData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                if (useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
                loadedData.completedScenes = loadedData.completedScenes ?? new List<string>();
            }
            catch (Exception e)
            {
                if (allowRestoreFromBackup)
                {
                    Debug.LogWarning("Failed to load data file. Attempting to roll back.");
                    bool rollbackSuccess = AttemptRollback(fullPath);
                    if (rollbackSuccess)
                    {
                        loadedData = Load(profileId, false);
                    }
                }
                else
                {
                    Debug.LogError("Error loading file: " + e);
                }
            }
        }
        return loadedData;
    }

    // ฟังก์ชันบันทึกข้อมูล
    public void Save(GameData data, string profileId)
    {
        if (profileId == null) { return; }

        string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            string dataToStore = JsonUtility.ToJson(data, true);

            if (useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }

            // สำรองข้อมูล
            GameData verifiedData = Load(profileId);
            if (verifiedData != null)
            {
                File.Copy(fullPath, fullPath + backupExtension, true);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving data: " + e);
        }
    }

    // ฟังก์ชันสำหรับลบข้อมูลโปรไฟล์
    public void Delete(string profileId)
    {
        string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
        string backupPath = fullPath + backupExtension; // เพิ่มบรรทัดนี้

        try
        {
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath); // ลบไฟล์หลัก
            }

            // ลบไฟล์สำรอง (ถ้ามี)
            if (File.Exists(backupPath))
            {
                File.Delete(backupPath);
                Debug.Log($"Deleted backup data for {profileId}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to delete data for {profileId}: {e.Message}");
        }
    }

    // ฟังก์ชันสำหรับการโหลดข้อมูลทั้งหมดของโปรไฟล์
    public Dictionary<string, GameData> LoadAllProfiles()
    {
        Dictionary<string, GameData> profilesDictionary = new Dictionary<string, GameData>();

        // ลูปผ่านทุกโปรไฟล์ในโฟลเดอร์
        IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(dataDirPath).EnumerateDirectories();
        foreach (var dirInfo in dirInfos)
        {
            string profileId = dirInfo.Name;
            string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);

            // ตรวจสอบว่าไฟล์ข้อมูลโปรไฟล์มีอยู่
            if (File.Exists(fullPath))
            {
                try
                {
                    // โหลดข้อมูลโปรไฟล์
                    GameData profileData = Load(profileId);
                    if (profileData != null)
                    {
                        profilesDictionary.Add(profileId, profileData);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to load profile data for {profileId}: {e.Message}");
                }
            }
        }

        return profilesDictionary;
    }

    // ฟังก์ชันสำหรับการกู้คืนไฟล์จากสำรอง
    private bool AttemptRollback(string fullPath)
    {
        string backupFilePath = fullPath + backupExtension;
        if (File.Exists(backupFilePath))
        {
            File.Copy(backupFilePath, fullPath, true);  // กู้คืนจากไฟล์สำรอง
            Debug.Log($"Rolled back to backup: {fullPath}");
            return true;
        }
        return false;
    }

    // ฟังก์ชันการเข้ารหัสและถอดรหัส
    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ 123);  // ใช้ XOR เป็นการเข้ารหัส/ถอดรหัส
        }
        return modifiedData;
    }

    public string GetMostRecentlyUpdatedProfileId()
    {
        string mostRecentProfileId = null;
        DateTime mostRecentTime = DateTime.MinValue;

        // ลูปผ่านทุกโปรไฟล์ในโฟลเดอร์
        IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(dataDirPath).EnumerateDirectories();
        foreach (var dirInfo in dirInfos)
        {
            string profileId = dirInfo.Name;
            string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);

            if (File.Exists(fullPath))
            {
                try
                {
                    // โหลดข้อมูลโปรไฟล์
                    GameData profileData = Load(profileId);  // โหลดข้อมูลของโปรไฟล์จากไฟล์
                    if (profileData != null)
                    {
                        DateTime profileLastUpdated = DateTime.FromBinary(profileData.lastUpdated);  // ตรวจสอบเวลาที่อัปเดตล่าสุด
                        if (profileLastUpdated > mostRecentTime)
                        {
                            mostRecentTime = profileLastUpdated;
                            mostRecentProfileId = profileId;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to load profile data for {profileId}: {e.Message}");
                }
            }
        }

        return mostRecentProfileId;
    }

}
