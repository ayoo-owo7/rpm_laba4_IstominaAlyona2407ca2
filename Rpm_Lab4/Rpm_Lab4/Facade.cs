using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Rpm_Lab4.Adapter;

namespace Rpm_Lab4
{
    public class Facade
    {
        public class SyncFacade
        {
            private readonly IFileSystem _sourceFS;
            private readonly IFileSystem _targetFS;

            public SyncFacade(IFileSystem source, IFileSystem target)
            {
                _sourceFS = source;
                _targetFS = target;
            }

            public void SyncFolder(string sourcePath, string targetPath)
            {
                Console.WriteLine($"Начало синхронизации: {_sourceFS.SystemName}:{sourcePath} -> {_targetFS.SystemName}:{targetPath}");

                if (!_sourceFS.Exists(sourcePath))
                {
                    Console.WriteLine("Источник не найден.");
                    return;
                }

                var items = _sourceFS.ListItems(sourcePath);

                foreach (var itemName in items)
                {
                    string srcItemPath = CombinePath(sourcePath, itemName);
                    string tgtItemPath = CombinePath(targetPath, itemName);


                    try
                    {
                        var data = _sourceFS.ReadFile(srcItemPath);


                        if (!_targetFS.Exists(tgtItemPath))
                        {
                            _targetFS.WriteFile(tgtItemPath, data);
                        }
                        else
                        {
                            Console.WriteLine($"Файл {itemName} уже существует в цели.");
                        }
                    }
                    catch (FileNotFoundException)
                    {

                        SyncFolder(srcItemPath, tgtItemPath);
                    }
                }

                Console.WriteLine("Синхронизация завершена успешно.");
            }

            public void Backup(string sourcePath, string backupPath)
            {
                Console.WriteLine($"Начало резервного копирования: {sourcePath} -> {backupPath}");

                if (!_sourceFS.Exists(sourcePath))
                {
                    Console.WriteLine("Источник для бэкапа не найден.");
                    return;
                }

                var items = _sourceFS.ListItems(sourcePath);

                foreach (var itemName in items)
                {
                    string srcItemPath = CombinePath(sourcePath, itemName);
                    string bakItemPath = CombinePath(backupPath, itemName);

                    try
                    {
                        var data = _sourceFS.ReadFile(srcItemPath);
                        _targetFS.WriteFile(bakItemPath, data);
                        Console.WriteLine($"Сохранено: {itemName}");
                    }
                    catch (FileNotFoundException)
                    {
                        Backup(srcItemPath, bakItemPath);
                    }
                }

                Console.WriteLine("Резервное копирование завершено.");
            }

            private string CombinePath(string path, string name)
            {
                if (path.EndsWith("/")) return path + name;
                return path + "/" + name;
            }
        }
    }
}
