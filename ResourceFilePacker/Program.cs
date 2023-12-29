using System.IO.Compression;
using System.Text;

namespace ResourceFilePacker;

internal class Program {
    static void Main(string[] args) {
        StringBuilder log = new StringBuilder();

        try {
            if (args.Length != 2) {
                log.AppendLine("Invalid number of arguments");
                return;
            }

            if (!Directory.Exists(args[0])) {
                log.AppendLine("Source directory does not exist");
                return;
            }

            if (!Directory.Exists(args[1]))
                Directory.CreateDirectory(args[1]);

            foreach (string dirPath in Directory.EnumerateDirectories(args[0])) {
                string dirName = Path.GetFileName(dirPath);
                string zipPath = Path.Combine(args[1], dirName + ".dat");

                if (File.Exists(zipPath))
                    File.Delete(zipPath);

                ZipFile.CreateFromDirectory(dirPath, zipPath);
            }

            log.AppendLine("Done Packing");
        } catch (Exception e) {
        } finally {
            File.WriteAllText("ResourceFilePacker_log.txt", log.ToString());
        }
    }
}
