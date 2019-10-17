using System.IO;

namespace Yuuki.FileIO
{
    public class FileIO
    {
        public enum FileIODesc
        {
            //既に存在していれば処理しない
            Default,
            //上書き
            Overwrite,
            //追記
            Append,
        }

        public void CreateFile(string filePath, string content, FileIODesc desc = FileIODesc.Default)
        {
            //フォルダ作成
            string dirPath = Path.GetDirectoryName(filePath);
            if (dirPath != string.Empty && !Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }
            //既に存在
            if (File.Exists(filePath))
            {
                switch (desc)
                {
                    case FileIODesc.Default: return;
                    case FileIODesc.Overwrite:
                        {
                            File.Delete(filePath);
                        }
                        break;
                    case FileIODesc.Append: break;
                    default:
                        break;
                }
            }
            //作成
            using (StreamWriter sw = new StreamWriter(filePath, true))
            {
                sw.Write(content);
                sw.Close();
            }
        }

        public bool CreateFile(string filePath, string[] contents, bool isOverride = false)
        {
            //フォルダ作成
            string dirPath = Path.GetDirectoryName(filePath);
            if (dirPath != string.Empty && !Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }
            //既に存在
            if (File.Exists(filePath)) { return false; }
            //作成
            using (StreamWriter sw = new StreamWriter(filePath, true))
            {
                foreach (var it in contents) { sw.WriteLine(it); }
                sw.Close();
            }
            return true;
        }

        /// <summary>
        /// ディレクトリ作成
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns>既に存在すれば作成せずにfalse</returns>
        public bool CreateDirectory(string directoryPath)
        {
            if (Directory.Exists(directoryPath)) { return false; }
            if (Directory.CreateDirectory(directoryPath) == null) { return false; }
            return true;
        }

        public string GetContents(string filePath)
        {
            if (!File.Exists(filePath)) { return string.Empty; }
            string contents = string.Empty;
            using (var sr = new StreamReader(filePath))
            {
                contents = sr.ReadToEnd();
                sr.Close();
            }
            return contents;
        }

    }
}
