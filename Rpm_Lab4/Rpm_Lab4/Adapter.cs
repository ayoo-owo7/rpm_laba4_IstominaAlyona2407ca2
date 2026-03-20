using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Rpm_Lab4.Composite;

namespace Rpm_Lab4
{
    public class Adapter
    {
        public interface IFileSystem
        {
            string SystemName { get; }
            List<string> ListItems(string path);
            byte[] ReadFile(string path);
            void WriteFile(string path, byte[] data);
            void DeleteItem(string path);
            bool Exists(string path);
        }

        public class CompositeFileSystemAdapter : IFileSystem
        {
            private readonly FolderItem _root;
            public string SystemName { get; }

            public CompositeFileSystemAdapter(FolderItem root, string systemName)
            {
                _root = root;
                SystemName = systemName;
            }

            private FileSystemItem GetItemByPath(string path)
            {
                string cleanPath = path.TrimStart('/');
                if (string.IsNullOrEmpty(cleanPath)) return _root;

                string[] parts = cleanPath.Split('/');
                FileSystemItem current = _root;

                int startIndex = 0;
                if (parts.Length > 0 && parts[0] == _root.Name)
                {
                    startIndex = 1;
                }

                for (int i = startIndex; i < parts.Length; i++)
                {
                    if (current is FolderItem folder)
                    {
                        current = folder.FindByName(parts[i]);
                        if (current == null) return null;
                    }
                    else
                    {
                        return null;
                    }
                }
                return current;
            }

            public List<string> ListItems(string path)
            {
                var item = GetItemByPath(path);
                if (item is FolderItem folder)
                {
                    return folder.GetChildren().Select(c => c.Name).ToList();
                }
                return new List<string>();
            }

            public byte[] ReadFile(string path)
            {
                var item = GetItemByPath(path);
                if (item is FileItem file)
                {
                    Console.WriteLine($"[{SystemName}] Чтение файла: {path}");
                    return file.Content;
                }
                throw new FileNotFoundException($"Файл не найден: {path}", path);
            }

            public void WriteFile(string path, byte[] data)
            {
                var item = GetItemByPath(path);

                if (item is FileItem existingFile)
                {
                    Console.WriteLine($"[{SystemName}] Обновление файла: {path}");
                }
                else
                {
                    string fileName = path.Substring(path.LastIndexOf('/') + 1);
                    string parentPath = path.Substring(0, path.LastIndexOf('/'));

                    var parentObj = GetItemByPath(parentPath);
                    if (parentObj is FolderItem parentFolder)
                    {
                        var newFile = new FileItem(fileName, data.Length, parentFolder, data);
                        parentFolder.Add(newFile);
                        Console.WriteLine($"[{SystemName}] Создан файл: {path}");
                    }
                    else
                    {
                        throw new Exception($"Не удалось найти родительскую папку для {path}");
                    }
                }
            }

            public void DeleteItem(string path)
            {
                var item = GetItemByPath(path);
                if (item != null && item.Parent != null)
                {
                    if (item.Parent is FolderItem parent)
                    {
                        parent.Remove(item);
                        Console.WriteLine($"[{SystemName}] Удалено: {path}");
                    }
                }
                else
                {
                    throw new FileNotFoundException($"Не удалось удалить: {path}");
                }
            }

            public bool Exists(string path)
            {
                return GetItemByPath(path) != null;
            }
        }

    }
}
