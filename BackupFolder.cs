using System;
using System.IO;

namespace Utils
{
    public class BackupFolder
    {
        static string sm_backuppath = "";
        public static string Path
        {
            get
            {
                if (sm_backuppath == null) sm_backuppath = "";

                return sm_backuppath;
            }
            set
            {
                sm_backuppath = value;
            }
        }

        public static void BackupFile(string file)
        {                    //make a backup of the original file
            DateTime now = DateTime.Now;
            string datesuffix = string.Format("{0:00}{1:00}{2:00}{3:00}{4:00}{5:00}{6:00}", now.Month, now.Day, now.Year, now.Hour, now.Minute, now.Second, now.Millisecond);
            //make a backup of the original file
            string backfolder = System.IO.Path.GetDirectoryName(file);
            if (BackupFolder.Path.Length > 0)
            {
                if (Directory.Exists(BackupFolder.Path) == false)
                    Directory.CreateDirectory(BackupFolder.Path);
                backfolder = BackupFolder.Path;
            }
            string backupfile = System.IO.Path.Combine(backfolder, System.IO.Path.GetFileName(file) + ".BACKUP" + datesuffix);
            FileInfo fi = new FileInfo(file);
            if (fi.Exists)
            {
                // check for read only flag, remove it if needed
                if (File.Exists(backupfile))
                {
                    FileAttributes attr = File.GetAttributes(backupfile);
                    attr &= ~FileAttributes.ReadOnly;
                    File.SetAttributes(backupfile, attr);
                }
                File.Copy(file, backupfile, true);
                File.SetAttributes(backupfile, FileAttributes.ReadOnly);
            }
    }
}
}