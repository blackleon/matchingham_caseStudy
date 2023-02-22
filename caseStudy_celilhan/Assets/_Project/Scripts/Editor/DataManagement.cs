using System.IO;
using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Editor
{
    public class DataManagement
    {
        [MenuItem("intLeon/Delete Data")]
        public static void DeleteData()
        {
            PlayerPrefs.DeleteAll();
            var files = Directory.GetFiles(Application.persistentDataPath, "*Data", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
                File.Delete(file);
            
            Debug.Log("Deleted Data");
        }

        [MenuItem("intLeon/Open Folder")]
        public static void OpenDataPath()
        {
            Debug.Log("Opening Data Path");
            Application.OpenURL(Application.persistentDataPath);
        }
    }
}