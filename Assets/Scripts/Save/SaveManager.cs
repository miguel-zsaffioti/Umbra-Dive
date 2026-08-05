using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private const string SaveFileName = "save.dat";
    private string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

    public static bool SaveExists() => File.Exists(Path.Combine(Application.persistentDataPath, SaveFileName));

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        string encrypted = SaveEncryption.Encrypt(json);
        File.WriteAllText(SavePath, encrypted);
    }

    public SaveData Load()
    {
        if (!HasSave()) return null;

        string encrypted = File.ReadAllText(SavePath);

        try
        {
            string json = SaveEncryption.Decrypt(encrypted);
            return JsonUtility.FromJson<SaveData>(json);
        }
        catch
        {
            // Arquivo corrompido ou adulterado fora do jogo.
            Debug.LogWarning("Save inválido ou corrompido. Ignorando.");
            return null;
        }
    }

    public bool HasSave() => SaveExists();

    public void DeleteSave()
    {
        if (HasSave()) File.Delete(SavePath);
    }
}