using System.Text;
using static Rpm_Lab4.Adapter;
using static Rpm_Lab4.Composite;
using static Rpm_Lab4.Facade;

class Program
{
    static void Main(string[] args)
    {
        // 1. Composite
        var localRoot = new FolderItem("LocalRoot");
        var docsFolder = new FolderItem("Documents", localRoot);
        var picsFolder = new FolderItem("Pictures", localRoot);

        var reportFile = new FileItem("report.txt", 1024, docsFolder, Encoding.UTF8.GetBytes("Отчет"));
        var photoFile = new FileItem("vacation.jpg", 2048, picsFolder, new byte[] { 0xFF, 0xD8 });

        docsFolder.Add(reportFile);
        picsFolder.Add(photoFile);
        localRoot.Add(docsFolder);
        localRoot.Add(picsFolder);

        Console.WriteLine($"1. Размер системы: {localRoot.GetSize()} байт");

        // 2. Adapter
        IFileSystem localFs = new CompositeFileSystemAdapter(localRoot, "LOCAL");
        IFileSystem cloudFs = new CompositeFileSystemAdapter(new FolderItem("CloudRoot"), "CLOUD");

        Console.WriteLine("\n2. Тест адаптера:");
        Console.WriteLine($"Файлы в корне: {string.Join(", ", localFs.ListItems("/LocalRoot"))}");

        // 3. Facade
        SyncFacade facade = new SyncFacade(localFs, cloudFs);

        Console.WriteLine("\n3. Работа фасада:");
        facade.SyncFolder("/LocalRoot/Documents", "/CloudRoot/BackupDocs");
        facade.Backup("/LocalRoot", "/CloudRoot/FullBackup");

    }
}

