using System;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace Rename_Revit_Files
{
    public partial class frmMain : Form
    {

        //string[] fileEntries;
        //string foldername;
        //decimal hyphenNum;

        public frmMain()
        {
            InitializeComponent();
        }

        Project myProject = new Project();

        private void Form1_Load(object sender, EventArgs e)
        {
            var version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
            
            this.Text = String.Format("Rename PDF Files | V {0} | 08/15/2012", version.ToString());
            this.btnRename.Enabled = false;
            this.ckbRenameFiles.Enabled = myProject.ReadyToWrite; ;
            this.lblReport.Text = "";            
            toolTip1.SetToolTip(this.numUpDown, "Select number of hyphens.\r\nxxx-xxx-xxx-E45-xxx.pdf that would be 3 hyphens");            
        }

        private void btnDialog_Click(object sender, EventArgs e)
        {
           
            this.folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.Desktop;
            DialogResult result = this.folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                myProject.FolderPath = folderBrowserDialog1.SelectedPath;
                this.txbDialog.Text = myProject.FolderPath;

                if (myProject.FolderPath != "")
                {
                    myProject.FilteredFiles.Clear();
                    this.listBox2.DataSource = null;
                    // Process the list of files found in the directory. 
                    myProject.SourceFiles.Clear(); //reset
                    myProject.SourceFiles.AddRange(Directory.GetFiles(myProject.FolderPath, "*.pdf")); //get only pdf files
                                       
                    // do something with fileName
                    this.listBox1.DataSource = null;
                    this.listBox1.DataSource = myProject.SourceFiles;
                    this.btnRename.Enabled = true;

                    if (this.numUpDown.Value == 1)
                    {
                        if (myProject.SourceFiles.Count != 0)
                        {
                            if (HyphenNum(myProject.SourceFiles[0]) != 0)
                            {
                                myProject.NumHyphens = HyphenNum(myProject.SourceFiles[0]);
                                this.numUpDown.Value = myProject.NumHyphens;
                            }
                            else 
                            {
                                myProject.NumHyphens = 1;
                                this.numUpDown.Value = myProject.NumHyphens;
                            }
                        }
                    }
                }           
            }
        }

        private void btnRename_Click(object sender, EventArgs e)
        {
            string[] fileNameArray;           

            int totalFiles = myProject.SourceFiles.Count;
            int pdfFiles = 0;
            int errorsFiles = 0;
            int totalRenamedFiles = 0;

            myProject.FilteredFiles.Clear();

            foreach (string fileName in myProject.SourceFiles)
            {
                if (Path.GetFileName(fileName).Contains(" - "))
                {
                    pdfFiles++;
                    fileNameArray = Regex.Split(Path.GetFileNameWithoutExtension(fileName), " - ");
                    
                    //making sure that the hyphen number is not higher than the array
                    if (myProject.NumHyphens > fileNameArray.Count()) myProject.NumHyphens = fileNameArray.Count();
                    this.numUpDown.Value = myProject.NumHyphens;

                    myProject.FilteredFiles.Add(myProject.Prefix + fileNameArray[(int)myProject.NumHyphens - 1] + ".pdf");

                    if (myProject.ReadyToWrite == true)
                    {
                        int error;
                        error = Project.File_rename(fileName, myProject.FolderPath + "\\" + myProject.Prefix + fileNameArray[(int)myProject.NumHyphens - 1] + ".pdf");
                        if (error != 1)
                        {
                            MessageBox.Show("ERROR renaming file -> " + myProject.Prefix + fileNameArray[(int)myProject.NumHyphens - 1] + ".pdf");
                            errorsFiles++;
                        }
                        else
                        {
                            totalRenamedFiles++;
                        }
                    }                                       
                }
                else
                {
                    myProject.FilteredFiles.Add(Path.GetFileName(fileName));
                }
            }

            this.listBox2.DataSource = null;
            this.listBox2.DataSource = myProject.FilteredFiles;

            this.ckbRenameFiles.Enabled = true;            
            lblReport.Text = string.Format("Total Files = {0} | Total PDF files ready to be renamed = {1} | Total writting errors = {2} | Total files renamed = {3}", totalFiles, pdfFiles, errorsFiles, totalRenamedFiles);
        }

                     

        private decimal HyphenNum(string filename)
        {
            int count = Regex.Matches(Path.GetFileNameWithoutExtension(filename), " - ").Count;

            if (count != 0)
            {
                return count;
            }                
            else
            {
                return 0;
            }            
        }

        private void numUpDown_ValueChanged(object sender, EventArgs e)
        {
            myProject.NumHyphens= (int)this.numUpDown.Value;
        }

        private void txbPrefix_TextChanged(object sender, EventArgs e)
        {
            myProject.Prefix = txbPrefix.Text;
        }

        private void ckbRenameFiles_CheckedChanged(object sender, EventArgs e)
        {
            myProject.ReadyToWrite = ckbRenameFiles.Checked;
        }  
      
    }
}
