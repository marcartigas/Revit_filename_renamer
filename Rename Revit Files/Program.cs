using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace Rename_Revit_Files
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }
    }


    public class Project
    {
        //Constructor
        public Project()
        {
            ReadyToWrite = false;
            NumHyphens = 0;
            Prefix = "";
        }


        //Properties
        public string FolderPath { get; set; }
        public string Prefix { get; set; }
        public decimal NumHyphens { get; set; }
        public bool ReadyToWrite { get; set; }
       
        private List<string> _sourceFiles = new List<string>();
        public List<string> SourceFiles
        {
            get { return _sourceFiles; }
            set { _sourceFiles = value; }        
        }

        private List<string> _filteredFiles = new List<string>();
        public List<string> FilteredFiles
        {
            get { return _filteredFiles; }
            set { _filteredFiles = value; }
        }
        

        //Methods
        public static int File_rename(string FilepathOld, string FilepathNew)
        {
            FileInfo fi = new FileInfo(FilepathOld);

            if (fi.Exists)
            {
                try
                {
                    fi.MoveTo(FilepathNew);
                }
                catch (InvalidCastException e)
                {
                    return 0;
                }
            }
            return 1;
        }    
    }


}
