using System.Diagnostics;

class Program
{
    static void Main(string[] args) //input file, output file
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: <input file> <output file>");
            return;
        }

        string filePath = args[0];
        byte[] targetBytes = new byte[] { 0x10, 0x2A, 0x11, 0x00 }; // Example: replace with your sequence
        byte[] fileBytes = File.ReadAllBytes(filePath);
        int index = FindSequence(fileBytes, targetBytes);

        if (index >= 0)
        {
            byte[] newBytes = new byte[fileBytes.Length - index];
            Array.Copy(fileBytes, index, newBytes, 0, newBytes.Length);

            File.WriteAllBytes("temp_" + filePath, newBytes);

            Console.WriteLine("File cleaned successfully.");
            filePath = "temp_" + filePath;
            Console.WriteLine("Sending to XenosRecomp...");
            var process = new Process();
            process.StartInfo.FileName = "XenosRecomp/XenosRecomp.exe";
            process.StartInfo.Arguments = $"{filePath} {args[1]} XenosRecomp/shader_common.h";
            process.Start();
            process.WaitForExit();
            Console.WriteLine("Tidying up...");
            File.Delete(filePath);
            Console.WriteLine("Finished!");
        }
        else
        {
            Console.WriteLine("Byte sequence not found.");
        }
    }

    static int FindSequence(byte[] source, byte[] sequence)
    {
        for (int i = 0; i <= source.Length - sequence.Length; i++)
        {
            bool match = true;
            for (int j = 0; j < sequence.Length; j++)
            {
                if (source[i + j] != sequence[j])
                {
                    match = false;
                    break;
                }
            }
            if (match)
                return i;
        }
        return -1;
    }
}
