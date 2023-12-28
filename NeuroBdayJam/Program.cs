using NeuroBdayJam.Util;
using System.Diagnostics;
using System.Text;
namespace NeuroBdayJam;

internal class Program {
    static void Main(string[] args) {
        ConsoleControl.Hide();

        Log.OnLog += (msg, type) => Console.WriteLine(msg);
        Log.OnLog += (msg, type) => Debug.WriteLine(msg);

        try {
            if (args.Length > 0) {
                for (int i = 0; i < args.Length; i++) {
                    if (!args[i].StartsWith("--"))
                        continue;

                    if (args[i] == "--debug")
                        Application.DRAW_DEBUG = true;
                }
            }

            //Application.DRAW_DEBUG = true;  // TODO
            Application.Initialize();
            Application.Start();
        } catch (Exception e) {
            StringBuilder sb = new StringBuilder();

            Exception? ex = e;
            while (ex != null) {
                sb.AppendLine(ex.ToString());
                ex = ex.InnerException;
            }

            sb.AppendLine();
            sb.AppendLine(e.StackTrace);

            Log.WriteLine(sb.ToString(), eLogType.Error);

            throw;
        }

    }
}
