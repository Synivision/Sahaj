using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code.Utilities
{
    public class FileServices
    {

        // 
        private static string[] _resourcesList;

        public static void CreateResourcesList(string saveToPath)
        {
            var resourceListText = "{" + Environment.NewLine;

            var directories = GetDirectoryNamesFromFolder("Assets/Resources");
            foreach (var directory in directories)
            {
                resourceListText = AddFolderToList(directory, resourceListText, 1);
            }

            resourceListText += "}";
            _resourcesList = resourceListText.Split('\n');

            File.WriteAllText(saveToPath, resourceListText);
        }

        private static string AddFolderToList(FileSystemInfo folder, string list, int tabCount = 0)
        {
            /* Structure of folders:
            * 
            * FolderName
            * {
            *      SubFolder
            *      {
            *          File1
            *      }
            *      File1
            *      File2
            * }
            * */

            var folders = GetDirectoryNamesFromFolder(folder.FullName);
            var files = GetFileNamesFromFolder(folder.FullName, ".png", ".json", ".jpg", ".ttf", ".ogg", ".mp3", ".wav", ".flac", ".mat");

            for (var i = 0; i < tabCount; i++)
                list += "\t";

            list += folder.Name + Environment.NewLine;

            for (var i = 0; i < tabCount; i++)
                list += "\t";

            list += "{" + Environment.NewLine;

            if (folders.Count == 0 && files.Count == 0)
            {
                // No files or folders in this folder, end the folder and return
                for (var i = 0; i < tabCount; i++)
                    list += "\t";
                list += "}" + Environment.NewLine;
                return list;
            }
            else if (folders.Count > 0)
            {
                // put all folders in
                foreach (var _folder in folders)
                {
                    list += AddFolderToList(_folder, "", tabCount + 1);
                }

                if (files.Count > 0)
                {
                    // put all files in
                    foreach (var file in files)
                    {
                        for (var i = 0; i <= tabCount; i++)
                            list += "\t";
                        list += file.Substring(file.LastIndexOf("\\", StringComparison.Ordinal) + 1, file.Length - file.LastIndexOf("\\", StringComparison.Ordinal) - 1) + Environment.NewLine;
                    }
                }

                for (var i = 0; i < tabCount; i++)
                    list += "\t";
                list += "}" + Environment.NewLine;
                return list;
            }
            else
            {
                // No more folders, but files exist
                // put all files in
                foreach (var file in files)
                {
                    for (var i = 0; i <= tabCount; i++)
                        list += "\t";
                    list += file.Substring(file.LastIndexOf("\\", System.StringComparison.Ordinal) + 1, file.Length - file.LastIndexOf("\\", System.StringComparison.Ordinal) - 1) + Environment.NewLine;
                }

                for (var i = 0; i < tabCount; i++)
                    list += "\t";
                list += "}" + Environment.NewLine;
                return list;
            }
        }

        public static void LoadResourcesList(string resourcesListPath)
        {
            _resourcesList = LoadTextAssetFromResources(resourcesListPath).Split('\n');
        }

        // include the fullstop in the extension target! also, pass null if you dont care what extension
        public static List<DirectoryInfo> GetDirectoryNamesFromFolder(string targetFolder)
        {
            var info = new DirectoryInfo(targetFolder);
            return info.GetDirectories().ToList();
        }

        public static List<string> GetFileNamesFromFolder(string targetFolder, params string[] extensions)
        {
            var fileNames = new List<string>();

            var info = new DirectoryInfo(targetFolder);
            var fileInfo = info.GetFiles();
            foreach (var file in fileInfo)
            {
                if (extensions == null || extensions.Length == 0)
                    fileNames.Add(file.Name);
                else if (extensions.Contains(file.Extension))
                    fileNames.Add(file.Name);
            }

            return fileNames;
        }

        public static List<string> GetResourceDirectories(string targetFolder)
        {
            var directoryList = GetNestedResourcesFolders(targetFolder);

            for (var i = 0; i < directoryList.Count(); i++)
                directoryList[i] = targetFolder + "/" + directoryList[i];

            return directoryList;
        }

        private static List<string> GetNestedResourcesFolders(string targetFolder, int line = 1)
        {
            List<string> NestedFolders = new List<string>();
            string[] splitPath = targetFolder.Split('/');
            var nestLevel = 0;

            string currentLine = "";
            do
            {
                currentLine = _resourcesList[line++];
                currentLine = currentLine.Trim(new char[] { '\r', '\n', '\t' });
                if (currentLine == "{")
                {
                    nestLevel++;
                    while (nestLevel != 0)
                    {
                        currentLine = _resourcesList[line++];
                        currentLine = currentLine.Trim(new char[] { '\r', '\n', '\t' });
                        if (currentLine == "}")
                            nestLevel--;
                        else if (currentLine == "{")
                            nestLevel++;
                    }
                }
                else if (currentLine == "}")
                {
                    nestLevel--;
                }
                else if (targetFolder == "")
                {
                    NestedFolders.Add(currentLine);
                }
                else if (currentLine == splitPath[0])
                {
                    if (splitPath.Count() > 1)
                        NestedFolders = GetNestedResourcesFolders(targetFolder.Substring(targetFolder.IndexOf('/') + 1), line + 1);
                    else
                        NestedFolders = GetNestedResourcesFolders("", line + 1);

                    break;
                }

            } while (line < _resourcesList.Count() && nestLevel >= 0);

            return NestedFolders;
        }

        public static List<string> GetResourceFiles(string targetFolder, params string[] extensions)
        {
            var directoryList = GetNestedResourcesFiles(targetFolder, 1, extensions);

            for (var i = 0; i < directoryList.Count(); i++)
            {
                directoryList[i] = directoryList[i].Substring(0, directoryList[i].LastIndexOf('.'));
                directoryList[i] = targetFolder + "/" + directoryList[i];
            }

            return directoryList;
        }

        private static List<string> GetNestedResourcesFiles(string targetFolder, int line = 1, params string[] extensions)
        {
            List<string> NestedFolders = new List<string>();
            string[] splitPath = targetFolder.Split('/');
            var nestLevel = 0;

            string currentLine = "";
            do
            {
                currentLine = _resourcesList[line++];
                currentLine = currentLine.Trim(new char[] { '\r', '\n', '\t' });
                if (currentLine == "{")
                {
                    nestLevel++;
                    while (nestLevel != 0)
                    {
                        currentLine = _resourcesList[line++];
                        currentLine = currentLine.Trim(new char[] { '\r', '\n', '\t' });
                        if (currentLine == "}")
                            nestLevel--;
                        else if (currentLine == "{")
                            nestLevel++;
                    }
                }
                else if (currentLine == "}")
                {
                    nestLevel--;
                }
                else if (targetFolder == "")
                {
                    if (currentLine.Contains(".")) // this implies it's a file name and not a folder
                    {
                        var extension = currentLine.Substring(currentLine.LastIndexOf('.') + 1);
                        if (extensions == null || extensions.Contains(extension))
                            NestedFolders.Add(currentLine);
                    }
                }
                else if (currentLine == splitPath[0])
                {
                    if (splitPath.Count() > 1)
                        NestedFolders = GetNestedResourcesFolders(targetFolder.Substring(targetFolder.IndexOf('/') + 1), line + 1);
                    else
                        NestedFolders = GetNestedResourcesFolders("", line + 1);

                    break;
                }

            } while (line < _resourcesList.Count() && nestLevel >= 0);

            return NestedFolders;
        }

        public static string GetEndOfResourcePath(string resourceFolderPath)
        {
            if (resourceFolderPath.Last() == '\\')
                resourceFolderPath = resourceFolderPath.Substring(0, resourceFolderPath.Length - 1);

            return resourceFolderPath.Substring(resourceFolderPath.LastIndexOf('/') + 1);
        }

        public static string GetEndOfPersistentDataPath(string resourceFolderPath)
        {
            return resourceFolderPath.Substring(resourceFolderPath.LastIndexOf('\\') + 1);
        }

        public static string TrimResourcePathOfPersistentDataPath(string resourceFolderPath)
        {
            return resourceFolderPath.Substring(resourceFolderPath.LastIndexOf(Application.persistentDataPath) + Application.persistentDataPath.Length + 1);
        }

        public static void ClearPersistentDataFile(string targetFile)
        {
            if (targetFile == null)
                return;

            var fileName = Application.persistentDataPath + "/" + targetFile;
            var directoryName = fileName.Substring(0, fileName.LastIndexOf('/'));
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            File.WriteAllText(fileName, string.Empty);
            //File.Delete(targetFile);
            //File.CreateText(targetFile);
        }

        public static void OverwritePersistentDataFile(string targetFile, string data)
        {
            if (targetFile == null)
                return;

            ClearPersistentDataFile(targetFile);
            var fileName = Application.persistentDataPath + "/" + targetFile;
            var file = new StreamWriter(fileName, false);
            file.Write(data);

            file.Close();
        }

        public static void ClearResourceDataFile(string targetFile)
        {
#if UNITY_EDITOR
            if (targetFile == null)
                return;

            //Debug.Log("clear file at " + Environment.CurrentDirectory + "/Assets/Resources/" + targetFile);

            var fileName = Environment.CurrentDirectory + "/Assets/Resources/" + targetFile;
            var directoryName = fileName.Substring(0, fileName.LastIndexOf('/'));
            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);

            File.WriteAllText(fileName, string.Empty);
#endif
        }

        public static void OverwriteResourceDataFile(string targetFile, string data)
        {
#if UNITY_EDITOR
            if (targetFile == null)
                return;

            //Debug.Log("overwriting file at " + Environment.CurrentDirectory + "/Assets/Resources/" + targetFile);

            ClearResourceDataFile(targetFile);
            var fileName = Environment.CurrentDirectory + "/Assets/Resources/" + targetFile;
            var file = new StreamWriter(fileName, false);
            file.Write(data);

            file.Close();
#endif
        }

        public static void AppendToFile(string targetFile, string data)
        {
            if (targetFile == null)
                return;

            var fileName = Application.persistentDataPath + "/" + targetFile;
            if (!File.Exists(fileName))
            {
                OverwritePersistentDataFile(targetFile, data);
                return;
            }
            var file = new StreamWriter(fileName, true);
            file.Write(data);

            file.Close();
        }

        public static List<string> GetDirectoriesAtPersistentDataPathLocation(string targetLocation)
        {
            var properLocation = Application.persistentDataPath + "/" + targetLocation;
            var directoryInfo = new DirectoryInfo(properLocation);

            if (!directoryInfo.Exists) return null;

            return directoryInfo.GetDirectories().Select(directory => directory.FullName).ToList();
        }

        public static List<string> GetFilesAtPersistentDataPathLocation(string targetLocation)
        {
            var properLocation = Application.persistentDataPath + "/" + targetLocation;
            var directoryInfo = new DirectoryInfo(properLocation);

            if (!directoryInfo.Exists) return null;

            return directoryInfo.GetFiles().Select(file => file.FullName).ToList();
        }

        public static void DeleteLocation(string targetLocation)
        {
            var properLocation = Application.persistentDataPath + "/" + targetLocation;

            if (File.Exists(properLocation))
                File.Delete(properLocation);

            if (Directory.Exists(properLocation))
                Directory.Delete(properLocation, true);
        }

        public static string LoadTextFileFromPersistentDataPath(string targetFile)
        {
            if (targetFile == null)
                return null;

            var fileName = Application.persistentDataPath + "/" + targetFile;
            if (!File.Exists(fileName))
                return null;

            string fileData;
            using (var reader = new StreamReader(fileName, Encoding.Default))
            {
                fileData = reader.ReadToEnd();
            }

            return fileData;
        }

        public static Sprite CreateSpriteFromTexture(Texture2D texture)
        {
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100);
            sprite.name = texture.name;

            return sprite;
        }

        // Need to remove non-resource components
        public static string LoadTextAssetFromResources(string targetResource)
        {
            var resourcePath = targetResource;

            var textFile = Resources.Load(resourcePath, typeof(TextAsset)) as TextAsset;
            if (!textFile)
                return null;

            return textFile.text;
        }

        public static Material LoadMaterialResources(string targetMaterial)
        {
            string resourcePath = targetMaterial;

            var material = Resources.Load(resourcePath, typeof(Material)) as Material;

            if (!material)
                return null;

            material.name = GetEndOfResourcePath(resourcePath);

            return material;
        }

        public static Texture2D LoadTextureResource(string targetResource)
        {
            var resourcePath = targetResource;

            var texture = Resources.Load(resourcePath, typeof(Texture2D)) as Texture2D;
            if (!texture)
                return null;

            texture.name = GetEndOfResourcePath(resourcePath);
            texture.filterMode = FilterMode.Point;
            texture.anisoLevel = 0;
            //texture.wrapMode = TextureWrapMode.Clamp;

            return texture;
        }

        public static AudioClip LoadAudioResource(string targetResource)
        {
            var resourcePath = targetResource;

            var sound = Resources.Load(resourcePath, typeof(AudioClip)) as AudioClip;
            if (!sound)
                return null;

            sound.name = GetEndOfResourcePath(resourcePath);

            return sound;
        }
    }
}
