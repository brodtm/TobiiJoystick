using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.CodeDom.Compiler;
using System.Globalization;


namespace Utils
{
    /// <summary>
    /// Ini file utitity functions
    /// 
    ///Usage:
    ///string section, key, inival;
    /// 
    ///section = "General";
    ///
    ///	key = "AxesSoftwareFile";
    ///inival = sGetPrivateProfileString(section, key,inifile, "");
    ///uFirmWareFile = CleanString(inival);
    ///			
    ///key = "HomeYoffset";
    ///inival = sGetPrivateProfileString(section, key,inifile, "28.4");
    ///HomeYoffset = Convert.ToDouble (CleanString(inival)); 
    ///			
    ///key = "AxesSoftwareFile";
    ///inival = uFirmWareFile;
    ///WritePrivateProfileString(section, key, inival, inifile);
    ///key = "HomeXoffset";
    ///inival = HomeXoffset.ToString("f6");
    ///WritePrivateProfileString(section, key, inival, inifile);
    ///
    ///
    /// 
    ///</summary>
    public class Ini
    {
        #region WIN API
        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileStringA")]
        static extern int GetPrivateProfileString(string lpApplicationName, string lpKeyName, string lpDefault, [MarshalAs(UnmanagedType.LPArray)] byte[] lpReturnedString, int nSize, string lpFileName);
        //		static extern int GetPrivateProfileString(string lpApplicationName, string lpKeyName , string lpDefault, string lpReturnedString , int nSize , string lpFileName );
        /// <summary>
        /// Write a line to ini file
        /// </summary>
        /// <param name="SectionName">[Section] of ini file</param>
        /// <param name="lpKeyName">key</param>
        /// <param name="lpString">string to write</param>
        /// <param name="lpFileName">ini file</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", EntryPoint = "WritePrivateProfileStringA")]
         static extern int _WritePrivateProfileString(string SectionName, string lpKeyName, string lpString, string lpFileName);
        [DllImport("kernel32.dll", EntryPoint = "WritePrivateProfileSectionA")]
         static extern int _WritePrivateProfileSection(string SectionName, string lpString, string lpFileName);
        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileSectionA")]
        static extern int GetPrivateProfileSection(string SectionName, [MarshalAs(UnmanagedType.LPArray)] byte[] lpReturnedString, int nSize, string lpFileName);
        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileSectionNamesA")]
        static extern int GetPrivateProfileSectionNames([MarshalAs(UnmanagedType.LPArray)] byte[] lpReturnedString, int nSize, string lpFileName);
        #endregion


        public static int WritePrivateProfileString(string SectionName, string lpKeyName, string lpString, string lpFileName)
        {
            lock (IniSerializer.sm_locker)
            {
                return _WritePrivateProfileString(SectionName, lpKeyName, lpString, lpFileName);
            }
        }
        public static int WritePrivateProfileSection(string SectionName, string lpString, string lpFileName)
        {
            lock (IniSerializer.sm_locker)
            {
                return _WritePrivateProfileSection(SectionName, lpString, lpFileName);
            }
        }

        /// <summary>
        /// Deletes a line from the ini file
        /// </summary>
        /// <param name="SectionName"></param>
        /// <param name="lpKeyName"></param>
        /// <param name="lpFileName"></param>
        /// <returns></returns>
        public static int DeletePrivateProfileString(string SectionName, string lpKeyName, string lpFileName)
        {
            return WritePrivateProfileString(SectionName, lpKeyName, null, lpFileName);
        }
        /// <summary>
        /// Cleans a string, removing everything after a ;
        /// </summary>
        /// <param name="srcstr">original string</param>
        /// <returns>cleaned string</returns>
        public static string CleanString(string srcstr)
        {
            int colonpos;
            string outstr;
            colonpos = srcstr.IndexOf(';');
            if (colonpos > 0)
            {
                outstr = srcstr.Substring(0, colonpos);
            }
            else
                outstr = srcstr;
            return outstr;
        }

        /// <summary>
        /// Reads a string from an ini file
        /// </summary>
        /// <param name="rsSection">[Section]</param>
        /// <param name="rsKey">key string</param>
        /// <param name="rsFile">ini file</param>
        /// <param name="rsDefault">default value if not in ini file</param>
        /// <returns></returns>
        public static string sGetPrivateProfileString(string rsSection, string rsKey, string rsFile, string rsDefault)
        {
            lock (IniSerializer.sm_locker)
            {
                const int nMAX_STRING_LEN = 32767;
                byte[] sKeyValue = new Byte[nMAX_STRING_LEN];
                string outstr;
                int lCharacters;
                //get it from the file
                lCharacters = Ini.GetPrivateProfileString(rsSection, rsKey, rsDefault, sKeyValue, nMAX_STRING_LEN - 1, rsFile);
                //trim the exess
                outstr = System.Text.Encoding.ASCII.GetString(sKeyValue);
                return outstr.Substring(0, lCharacters).Trim();
            }
        }

        /// <summary>
        /// Gets a string array of all of the lines in an ini file [Section]
        /// </summary>
        /// <param name="rsSection">[Section]</param>
        /// <param name="rsFile">ini file</param>
        /// <returns>string array</returns>
        public static string[] sGetPrivateProfileSection(string rsSection, string rsFile)
        {
            lock (IniSerializer.sm_locker)
            {
                const int nMAX_STRING_LEN = 32767;
                byte[] sKeyValue = new Byte[nMAX_STRING_LEN];
                string outstr;
                int lCharacters;
                //get it from the file
                lCharacters = GetPrivateProfileSection(rsSection, sKeyValue, nMAX_STRING_LEN - 1, rsFile);
                //trim the exess
                outstr = System.Text.Encoding.ASCII.GetString(sKeyValue);
                outstr = outstr.Substring(0, lCharacters);
                string[] parts = outstr.Split(new char[] { (char)0 });
                return parts;
            }
        }
        /// <summary>
        /// Gets a list of the keys in a section
        /// </summary>
        /// <param name="rsSection"></param>
        /// <param name="rsFile"></param>
        /// <returns></returns>
        public static string[] sGetPrivateProfileSectionKeys(string rsSection, string rsFile)
        {
            string[] lines = sGetPrivateProfileSection(rsSection, rsFile);
            List<string> outlines = new List<string>();
            foreach (string line in lines)
            {
                string[] parts = line.Split(new char[] { '=' });
                if (parts.Length > 1)
                {
                    if (parts[0].StartsWith("#") || parts[0].StartsWith("//") || parts[0].StartsWith(";"))
                    {
                        //this is a comment, so we can ignore
                    }
                    else
                    {
                        outlines.Add(parts[0].Trim());
                    }
                }
            }
            return outlines.ToArray();
        }

        /// <summary>
        /// Gets a string array of the section names in an ini file
        /// </summary>
        /// <param name="rsFile">ini file</param>
        /// <returns>array of section names</returns>
        public static string[] sGetPrivateProfileSectionNames(string rsFile)
        {
            lock (IniSerializer.sm_locker)
            {
                const int nMAX_STRING_LEN = 32767;
                byte[] sKeyValue = new Byte[nMAX_STRING_LEN];
                string outstr;
                int lCharacters;
                //get it from the file
                lCharacters = GetPrivateProfileSectionNames(sKeyValue, nMAX_STRING_LEN - 1, rsFile);
                //trim the exess
                outstr = System.Text.Encoding.ASCII.GetString(sKeyValue);
                outstr = outstr.Substring(0, lCharacters);
                string[] parts = outstr.Split(new char[] { (char)0 });
                return parts;
            }
        }

        public static void ClearPrivateProfileSection(string rsSection, string rsFile)
        {
            WritePrivateProfileSection(rsSection, "", rsFile);
        }
    }

    public class DeepSaver
    {
        static bool sm_savedefaultvalues = false;
        /// <summary>
        /// If false, then when saving properties that have the DefaultValueAttribute,
        /// and have a value that is the same as the one specified by the DefaultValueAttribute attribute, 
        /// the entry is not written to the ini file.
        /// </summary>
        public static bool SaveDefaultValues
        {
            get { return sm_savedefaultvalues; }
            set { sm_savedefaultvalues = value; }
        }
        public static void Save(object o, string topsection, string file)
        {
            Save(o, topsection, file, true);
        }
        public static void Save(object o, string topsection, string file, bool makebackupfile)
        {
            lock (IniSerializer.sm_locker)
            {
                FileInfo fi = new FileInfo(file);
                if (fi.Exists)
                {
                    FileAttributes fattr = File.GetAttributes(file);
                    fattr &= ~FileAttributes.ReadOnly;
                    File.SetAttributes(file, fattr);
                }
                if (Directory.Exists(Path.GetDirectoryName(file)) == false)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(file));
                }
                //get a temp file
                using (TempFileCollection tempfiles = new TempFileCollection(fi.DirectoryName, false))
                {
                    string tempfile = tempfiles.AddExtension(fi.Extension, false);
                    if (fi.Exists)
                    {
                        File.Copy(file, tempfile);
                        FileAttributes tattr = File.GetAttributes(tempfile);
                        tattr &= ~FileAttributes.ReadOnly;
                        File.SetAttributes(tempfile, tattr);
                    }
                    _Save(o, topsection, tempfile);


                    //make a backup of the original file
                    DateTime now = DateTime.Now;
                    string datesuffix = string.Format("{0:00}{1:00}{2:00}{3:00}{4:00}", now.Month, now.Day, now.Year, now.Hour, now.Minute);
                    //make a backup of the original file
                    string backupfile = Path.Combine(BackupFolder.Path, Path.GetFileName(file) + ".BACKUP" + datesuffix);
                    if (fi.Exists)
                    {
                        // check for read only flag, remove it if needed
                        if (makebackupfile)
                        {
                            if (File.Exists(backupfile))
                            {
                                FileAttributes attr = File.GetAttributes(backupfile);
                                attr &= ~FileAttributes.ReadOnly;
                                File.SetAttributes(backupfile, attr);
                            }
                            File.Copy(file, backupfile, true);
                            File.SetAttributes(backupfile, FileAttributes.ReadOnly);
                        }
                        fi.Delete();
                    }
                    //if we got here, then we should be OK
                    if (File.Exists(tempfile))
                        File.Move(tempfile, file);
                    tempfiles.Delete();
                }
            }
        }

        /// <summary>
        /// Save an object to ini file, except for the properties marked with the DontSerializeAttribute.
        /// </summary>
        /// <param name="o">object to save</param>
        /// <param name="section">[Section] to save to</param>
        /// <param name="file">ini file name</param>
        /// <param name="makebackupfile">makes a backup file</param>
        public static void _Save(object o, string section, string file)
        {
            lock (IniSerializer.sm_locker)
            {
                //get a temp file
                string tempfile = file;

                if (o == null) return;
                Type t = o.GetType();
                PropertyInfo[] pis = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField);//

                foreach (PropertyInfo pi in pis)
                {
                    try
                    {
                        DontSerializeAttribute[] fields = (DontSerializeAttribute[])pi.GetCustomAttributes(typeof(DontSerializeAttribute), true);
                        if (fields.Length == 0)
                        {
                            bool hassetandget = pi.CanRead & pi.CanWrite;
                            object v = pi.GetValue(o, null);
                            Type tval = v.GetType();
                            string key = pi.Name;
                            {
                                if ((v.GetType().IsPrimitive) || (v.GetType().IsEnum))
                                {
                                    if (hassetandget)
                                    {
                                        string inival = StringConverter.ConvertToString(v);
                                        string defval = "";
                                        DefaultValueAttribute[] defvalfields = (DefaultValueAttribute[])pi.GetCustomAttributes(typeof(DefaultValueAttribute), true);
                                        if (defvalfields.Length > 0)
                                        {
                                            if (defvalfields[0].Value != null)
                                            {
                                                defval = StringConverter.ConvertToString(defvalfields[0].Value);
                                            }
                                        }
                                        string curinival = Ini.sGetPrivateProfileString(section, key, tempfile, defval);
                                        if ((curinival != inival) || SaveDefaultValues)
                                        {
                                            Ini.WritePrivateProfileString(section, key, inival, tempfile);
                                        }
                                    }
                                }
                                else
                                {
                                    _Save(v, section + "." + key, file);
                                }
                            }
                        }
                    }
                    catch (Exception exc) { MsgBox.ShowException(exc); }
                }
            }
        }


        /// <summary>
        /// Load an object from ini file, except for the properties marked with the DontSerializeAttribute.
        /// </summary>
        /// <param name="o">object to save</param>
        /// <param name="section">[Section] to save to</param>
        /// <param name="file">ini file name</param>
        /// <param name="makebackupfile">makes a backup file</param>
        public static void Load(object o, string section, string file)
        {
            lock (IniSerializer.sm_locker)
            {
                //get a temp file
                string tempfile = file;

                if (o == null) return;
                Type t = o.GetType();
                PropertyInfo[] pis = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField);//

                foreach (PropertyInfo pi in pis)
                {
                    try
                    {
                        DontSerializeAttribute[] fields = (DontSerializeAttribute[])pi.GetCustomAttributes(typeof(DontSerializeAttribute), true);
                        if (fields.Length == 0)
                        {
                            bool hassetandget = pi.CanRead & pi.CanWrite;
                            object v = pi.GetValue(o, null);
                            string key = pi.Name;
                            {
                                if ((v.GetType().IsPrimitive) || (v.GetType().IsEnum))
                                {
                                    if (hassetandget)
                                    {
                                        string inival = v.ToString();
                                        inival = Ini.sGetPrivateProfileString(section, key, file, "");
                                        if (inival.Length > 0)
                                        {
                                            try
                                            {
                                                Type proptype = v.GetType();
                                                v = StringConverter.ConvertFromString(inival, proptype);
                                                if (pi.CanWrite)
                                                    pi.SetValue(o, v, null);
                                            }
                                            catch { }
                                        }
                                    }
                                }
                                else
                                {
                                    Load(v, section + "." + key, file);
                                }
                            }
                        }
                    }
                    catch (Exception exc) { MsgBox.ShowException(exc); }
                }
            }
        }

    }

    /// <summary>
    /// Class for saving an object to an ini file
    /// </summary>
    public class IniSerializer
    {
        public static object sm_locker = new object();
        static bool sm_savedefaultvalues = false;
        /// <summary>
        /// If false, then when saving properties that have the DefaultValueAttribute,
        /// and have a value that is the same as the one specified by the DefaultValueAttribute attribute, 
        /// the entry is not written to the ini file.
        /// </summary>
        public static bool SaveDefaultValues
        {
            get { return sm_savedefaultvalues; }
            set { sm_savedefaultvalues = value; }
        }
        /// <summary>
        /// Save an object to ini file, except for the properties marked with the DontSerializeAttribute.
        /// </summary>
        /// <param name="o">object to save</param>
        /// <param name="section">[Section] to save to</param>
        /// <param name="file">ini file name</param>
        public static void Save(object o, string section, string file)
        {
            Save(o, section, file, true);
        }



        /// <summary>
        /// Save an object to ini file, except for the properties marked with the DontSerializeAttribute.
        /// </summary>
        /// <param name="o">object to save</param>
        /// <param name="section">[Section] to save to</param>
        /// <param name="file">ini file name</param>
        /// <param name="makebackupfile">makes a backup file</param>
        public static void Save(object o, string section, string file, bool makebackupfile)
        {
            lock (sm_locker)
            {
                if (Directory.Exists(Path.GetDirectoryName(file)) == false)
                {
                    try
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(file));
                    }
                    catch (Exception ex)
                    {

                        throw new ApplicationException(string.Format("Unable to create directory '{0}'.", Path.GetDirectoryName(file)), ex);
                    }
                }

                bool changesToIni = false;
                FileInfo fi = new FileInfo(file);
                if (fi.Exists)
                {
                    FileAttributes fattr = File.GetAttributes(file);
                    if ((fattr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        fattr &= ~FileAttributes.ReadOnly;
                        File.SetAttributes(file, fattr);
                    }
                }
                //get a temp file
                using (TempFileCollection tempfiles = new TempFileCollection(fi.DirectoryName, false))
                {
                    string tempfile = tempfiles.AddExtension(fi.Extension, false);
                    //if (fi.Exists)
                    //{
                    //    File.Copy(file, tempfile);
                    //    FileAttributes tattr = File.GetAttributes(tempfile);
                    //    tattr &= ~FileAttributes.ReadOnly;
                    //    File.SetAttributes(tempfile, tattr);
                    //}



                    Type t = o.GetType();
                    PropertyInfo[] pis = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField);

                    Dictionary<string, string> existingEntries = GetIniSectionDictionary(section, file);

                    foreach (PropertyInfo pi in pis)
                    {
                        try
                        {
                            DontSerializeAttribute[] fields = (DontSerializeAttribute[])pi.GetCustomAttributes(typeof(DontSerializeAttribute), true);
                            if (fields.Length == 0)
                            {
                                bool hassetandget = pi.CanRead & pi.CanWrite;
                                //if (hassetandget)
                                {
                                    object v = pi.GetValue(o, null);
                                    string key = pi.Name;
                                    string inival = StringConverter.ConvertToString(v);
                                    string defval = "";
                                    DefaultValueAttribute[] defvalfields = (DefaultValueAttribute[])pi.GetCustomAttributes(typeof(DefaultValueAttribute), true);
                                    if (defvalfields.Length > 0)
                                    {
                                        if (defvalfields[0].Value != null)
                                        {
                                            defval = StringConverter.ConvertToString(defvalfields[0].Value);
                                        }
                                    }
                                    string curinival = GetIniSectionValuefromDictionary(existingEntries, key, defval);//Ini.sGetPrivateProfileString(section, key, tempfile, defval);
                                    if ((curinival != inival) || SaveDefaultValues)
                                    {
                                        if (changesToIni == false)
                                        {
                                            changesToIni = true;
                                            if (fi.Exists)
                                            {
                                                File.Copy(file, tempfile);
                                                FileAttributes tattr = File.GetAttributes(tempfile);
                                                if ((tattr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                                                {
                                                    tattr &= ~FileAttributes.ReadOnly;
                                                    File.SetAttributes(tempfile, tattr);
                                                }
                                            }
                                        }
                                        Ini.WritePrivateProfileString(section, key, inival, tempfile);
                                    }
                                }
                            }
                        }
                        catch (Exception exc) { MsgBox.ShowException(exc); }
                    }

                    if (changesToIni)
                    {
                        //make a backup of the original file
                        DateTime now = DateTime.Now;
                        string datesuffix = string.Format("{0:00}{1:00}{2:00}{3:00}{4:00}", now.Month, now.Day, now.Year, now.Hour, now.Minute);
                        //make a backup of the original file
                        string backupfile = Path.Combine(BackupFolder.Path, Path.GetFileName(file) + ".BACKUP" + datesuffix);
                        if (fi.Exists)
                        {
                            // check for read only flag, remove it if needed
                            if (makebackupfile)
                            {
                                if (File.Exists(backupfile))
                                {
                                    FileAttributes attr = File.GetAttributes(backupfile);
                                    attr &= ~FileAttributes.ReadOnly;
                                    File.SetAttributes(backupfile, attr);
                                }
                                File.Copy(file, backupfile, true);
                                File.SetAttributes(backupfile, FileAttributes.ReadOnly);
                            }
                            fi.Delete();
                        }
                        //if we got here, then we should be OK
                        if (File.Exists(tempfile))
                            File.Move(tempfile, file);
                        tempfiles.Delete();
                    }
                }
            }
        }

        public static void Save(object o, string section, string[] propertynames, string file)
        {
            Save(o, section, propertynames, file, true);
        }


        /// <summary>
        /// Save an object to ini file
        /// </summary>
        /// <param name="o">object to save</param>
        /// <param name="section">[Section] to save to</param>
        /// <param name="propertynames">array of properties to save</param>
        /// <param name="file">ini file name</param>
        public static void Save(object o, string section, string[] propertynames, string file, bool makebackupfile)
        {
            lock (sm_locker)
            {
                FileInfo fi = new FileInfo(file);
                if (fi.Exists)
                {
                    FileAttributes fattr = File.GetAttributes(file);
                    fattr &= ~FileAttributes.ReadOnly;
                    File.SetAttributes(file, fattr);
                }
                if (Directory.Exists(Path.GetDirectoryName(file)) == false)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(file));
                }
                //get a temp file
                using (TempFileCollection tempfiles = new TempFileCollection(fi.DirectoryName, false))
                {
                    string tempfile = tempfiles.AddExtension(fi.Extension, false);
                    if (fi.Exists)
                    {
                        File.Copy(file, tempfile);
                        FileAttributes tattr = File.GetAttributes(tempfile);
                        tattr &= ~FileAttributes.ReadOnly;
                        File.SetAttributes(tempfile, tattr);
                    }

                    Type t = o.GetType();
                    ArrayList pis = new ArrayList();

                    foreach (string s in propertynames)
                    {
                        PropertyInfo propinfo = t.GetProperty(s);
                        if (propinfo != null)
                            pis.Add(propinfo);
                    }

                    Dictionary<string, string> existingEntries = GetIniSectionDictionary(section, file);
                    foreach (PropertyInfo pi in pis)
                    {
                        bool hassetandget = pi.CanRead & pi.CanWrite;
                        //if (hassetandget)
                        {
                            object v = pi.GetValue(o, null);
                            string key = pi.Name;
                            string inival = StringConverter.ConvertToString(v);//v.ToString();
                            string defval = "";
                            DefaultValueAttribute[] defvalfields = (DefaultValueAttribute[])pi.GetCustomAttributes(typeof(DefaultValueAttribute), true);
                            if (defvalfields.Length > 0)
                            {
                                if (defvalfields[0].Value != null)
                                {
                                    defval = StringConverter.ConvertToString(defvalfields[0].Value);
                                }
                            }
                            string curinival = GetIniSectionValuefromDictionary(existingEntries, key, defval);//Ini.sGetPrivateProfileString(section, key, tempfile, defval);
                            if ((curinival != inival) || SaveDefaultValues)
                            {
                                Ini.WritePrivateProfileString(section, key, inival, tempfile);
                            }
                        }
                    }
                    DateTime now = DateTime.Now;
                    string datesuffix = string.Format("{0:00}{1:00}{2:00}{3:00}{4:00}", now.Month, now.Day, now.Year, now.Hour, now.Minute);
                    //make a backup of the original file
                    string backupfile = Path.Combine(BackupFolder.Path, Path.GetFileName(file) + ".BACKUP" + datesuffix);
                    if (fi.Exists)
                    {
                        // check for read only flag, remove it if needed
                        if (makebackupfile)
                        {
                            if (File.Exists(backupfile))
                            {
                                FileAttributes attr = File.GetAttributes(backupfile);
                                attr &= ~FileAttributes.ReadOnly;
                                File.SetAttributes(backupfile, attr);
                            }
                            File.Copy(file, backupfile, true);
                            File.SetAttributes(backupfile, FileAttributes.ReadOnly);
                        }
                        fi.Delete();
                    }
                    //if we got here, then we should be OK
                    if (File.Exists(tempfile))
                        File.Move(tempfile, file);
                    tempfiles.Delete();
                }
            }
        }

        /// <summary>
        /// Load an objects properties from an ini file, 
        /// </summary>
        /// <param name="o">object to load</param>
        /// <param name="section">[Section] of ini file</param>
        /// <param name="propertynames">array of property names to load</param>
        /// <param name="file">ini file name</param>
        /// <returns>true if the {Section] exists, otherwise false</returns>
        public static bool Load(object o, string section, string[] propertynames, string file)
        {
            lock (sm_locker)
            {
                //string[] sectionlines = Ini.sGetPrivateProfileSection(section, file);
                //if (sectionlines.Length == 0) return false;
                //if (sectionlines.Length == 1)
                //{
                //    if (sectionlines[0].Trim().Length == 0)
                //        return false;
                //}
                Dictionary<string, string> existingEntries = GetIniSectionDictionary(section, file);
                if (existingEntries.Count == 0) return false;
                if (existingEntries.Count == 1)
                {
                    foreach (string k in existingEntries.Keys)
                    {
                        if (k.Trim().Length == 0)
                            return false;
                    }
                }


                Type t = o.GetType();
                ArrayList pis = new ArrayList();

                foreach (string s in propertynames)
                {
                    PropertyInfo propinfo = t.GetProperty(s, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField);
                    if (propinfo != null)
                        pis.Add(propinfo);
                }
                foreach (PropertyInfo pi in pis)
                {
                    bool hassetandget = pi.CanRead & pi.CanWrite;
                    //if (hassetandget)
                    {
                        object v = pi.GetValue(o, null);
                        string key = pi.Name;
                        string inival = v.ToString();
                        inival = GetIniSectionValuefromDictionary(existingEntries, key, ""); //Ini.sGetPrivateProfileString(section, key, file, "");
                        if (inival.Length > 0)
                        {
                            try
                            {
                                Type proptype = v.GetType();
                                v = StringConverter.ConvertFromString(inival, proptype);
                                if (pi.CanWrite)
                                    pi.SetValue(o, v, null);
                            }
                            catch { }
                        }
                    }
                }
                return true;
            }
        }

        public static string GetIniSectionValuefromDictionary(Dictionary<string, string> dict, string key, string defaultvalue)
        {
            string ukey = key.ToUpper();
            if (dict.ContainsKey(ukey))
                return dict[ukey];
            else
                return defaultvalue;
        }
        public static Dictionary<string, string> GetIniSectionDictionary(string section, string inifile)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string[] loadernames = Ini.sGetPrivateProfileSection(section, inifile);

            foreach (string entry in loadernames)
            {
                try
                {
                int equalspos = entry.IndexOf('=');
                if (equalspos > 0)
                {
                    string key = entry.Substring(0, equalspos);
                    string val = "";
                    if (entry.Length > equalspos + 1)
                    {
                        val = entry.Substring(equalspos + 1);
                    }
                    //else
                    //{
                    //    Console.WriteLine("");
                    //}

                    if (key.StartsWith("#") || key.StartsWith("//") || key.StartsWith(";"))
                    {
                        //this is a comment, so we can ignore
                    }
                    else
                    {
                            if (dict.ContainsKey(key.Trim().ToUpper()) == false)//just in case there is a duplicate entry, take the first value only
                            {
                                dict.Add(key.Trim().ToUpper(), val.Trim());
                    }
                        }
                    }
                }
                catch
                {
                }
                //string[] parts = entry.Split(new char[] { '=' });
                //if (parts.Length > 1)
                //{
                //    if (parts[0].StartsWith("#") || parts[0].StartsWith("//") || parts[0].StartsWith(";"))
                //    {
                //        //this is a comment, so we can ignore
                //    }
                //    else
                //    {
                //        dict.Add(parts[0].Trim().ToUpper(), parts[1].Trim());
                //    }
                //}
            }
            return dict;
        }

        /// <summary>
        ///  Load an objects properties from an ini file,
        ///  except for the properties marked with the DontSerializeAttribute.
        /// </summary>
        /// <param name="o">object to load</param>
        /// <param name="section">[Section] of ini file</param>
        /// <param name="file">ini file name</param>
        /// <returns>true if the {Section] exists, otherwise false</returns>
        //public static bool Load(object o, string section, string file)
        //{

        //    Wrapper wrapper = null; ///this wrapper section is used to deal with the case when the object o is actually a structure;
        //    ///We cannot pass the object back as a value since it is a struct; Therefore, it should have been wrapped in the 
        //    ///wrapper before calling this function
        //    if (o != null)
        //    {
        //        if (o is Wrapper)
        //        {
        //            wrapper = o as Wrapper;
        //            o = wrapper.wrappedobject;
        //        }
        //    }
        //    lock (sm_locker)
        //    {
        //        //string[] sectionlines = Ini.sGetPrivateProfileSection(section, file);
        //        //if (sectionlines.Length == 0) return false;
        //        //if (sectionlines.Length == 1)
        //        //{
        //        //    if (sectionlines[0].Trim().Length == 0)
        //        //        return false;
        //        //}
        //        Dictionary<string, string> existingEntries = GetIniSectionDictionary(section, file);
        //        if (existingEntries.Count == 0) return false;
        //        if (existingEntries.Count == 1)
        //        {
        //            foreach (string k in existingEntries.Keys)
        //            {
        //                if (k.Trim().Length == 0)
        //                    return false;
        //            }
        //        }

        //        Type t = o.GetType();
        //        PropertyInfo[] pis = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField);
        //        foreach (PropertyInfo pi in pis)
        //        {
        //            bool hassetandget = pi.CanRead & pi.CanWrite;
        //            //if (hassetandget)
        //            {
        //                try
        //                {
        //                    DontSerializeAttribute[] fields = (DontSerializeAttribute[])pi.GetCustomAttributes(typeof(DontSerializeAttribute), true);
        //                    if (fields.Length == 0)
        //                    {
        //                        object v = pi.GetValue(o, null);
        //                        string key = pi.Name;
        //                        string inival = v.ToString();
        //                        inival = GetIniSectionValuefromDictionary( existingEntries, key, ""); //Ini.sGetPrivateProfileString(section, key, file, "");
        //                        if (inival.Length > 0)
        //                        {
        //                            try
        //                            {
        //                                Type proptype = v.GetType();
        //                                v = StringConverter.ConvertFromString(inival, proptype);
        //                                if (pi.CanWrite)
        //                                    pi.SetValue(o, v, null);
        //                            }
        //                            catch //(Exception exc) 
        //                            { }
        //                        }
        //                    }
        //                }
        //                catch (Exception exc)
        //                {
        //                    MsgBox.ShowException(exc);
        //                }
        //            }
        //        }
        //        if (wrapper != null)
        //        {
        //            wrapper.wrappedobject = o;
        //        }
        //        return true;
        //    }

        //}
    }
    /// <summary>
    /// This attribute can be applied to any properties that should not be serialized with the IniSerializer
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DontSerializeAttribute : Attribute
    {
        public DontSerializeAttribute()
        {
        }
    }

    /// <summary>
    /// A handly class to convert virtually any object to or from a string
    /// </summary>
    public class StringConverter
    {
        /// <summary>
        /// Convert an object to a string
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string ConvertToString(object o)
        {
            if (o is DateTime)
            {
                DateTime dt = (DateTime)o;
                //string s = dt.ToString("G", DateTimeFormatInfo.InvariantInfo);
                string s = dt.ToString("o");
                return s;
            }
            else if (o is float)
            {
                float f = (float)o;
                string s = f.ToString("R");
                return s;
            }
            else if (o is double)
            {
                double d = (double)o;
                string s = d.ToString("R");
                return s;
            }
            else
            {
                TypeConverter typeconv = TypeDescriptor.GetConverter(o);
                return typeconv.ConvertToString(o);
            }
        }
        /// <summary>
        /// Convert a string to an object
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static object ConvertFromString(string s, Type t)
        {
            if (t == typeof(DateTime))
            {
                try
                {
                    DateTime v = DateTime.Parse(s, null, DateTimeStyles.RoundtripKind);
                    return v;
                }
                catch
                {
                    try
                    {
                        TypeConverter typeconv = TypeDescriptor.GetConverter(t);
                        return typeconv.ConvertFromString(s);
                    }
                    catch
                    {
                        try
                        {
                            DateTime v = DateTime.Parse(s);
                            return v;
                        }
                        catch
                        {
                            //try breaking the value in 2 parts if possible, in the case of the form "09/30/2009 17:32:25"
                            string[] parts = s.Split(new char[] { ' ' });
                            if (parts.Length > 1)
                            {
                                //get the date part
                                DateTime vd = DateTime.Parse(parts[0]);
                                //also try to get the time part
                                try
                                {
                                    DateTime vh = DateTime.Parse(parts[1]);
                                }
                                catch
                                {
                                }
                                return vd;
                            }
                            DateTime v = DateTime.Parse(s, null, DateTimeStyles.RoundtripKind | DateTimeStyles.AllowWhiteSpaces);
                            return v;

                        }
                    }
                }
            }
            else if (t == typeof(float))
            {
                float f = float.Parse(s, CultureInfo.InvariantCulture);
                return f;
            }
            else if (t == typeof(double))
            {
                double d = double.Parse(s, CultureInfo.InvariantCulture); 
                return d;
            }
            else
            {
                TypeConverter typeconv = TypeDescriptor.GetConverter(t);
                return typeconv.ConvertFromString(s);
            }
        }
    }

    public class FileBackup
    {
        /// <summary>
        /// Backup a file with a time-date stamp to the Backup folder
        /// </summary>
        /// <param name="file"></param>
        public static void Backup(string file)
        {
            FileInfo fi = new FileInfo(file);
            //make a backup of the original file
            DateTime now = DateTime.Now;
            string datesuffix = string.Format("{0:00}{1:00}{2:00}{3:00}{4:00}", now.Month, now.Day, now.Year, now.Hour, now.Minute);
            //make a backup of the original file
            string backupfile = Path.Combine(BackupFolder.Path, Path.GetFileName(file) + ".BACKUP" + datesuffix);
            if (fi.Exists)
            {
                if (File.Exists(backupfile))
                {
                    FileAttributes attr = File.GetAttributes(backupfile);
                    attr &= ~FileAttributes.ReadOnly;
                    File.SetAttributes(backupfile, attr);

                    File.Delete(backupfile);
                }
                File.Copy(file, backupfile);
            }
        }
    }
}