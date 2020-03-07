using System.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
//Added New Namespaces....
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace ProcessDiscovery
{    
    // Summary description for Form1.
    public class Form1 : System.Windows.Forms.Form
    {
        #region Form Declaration
        private System.Windows.Forms.Timer timer1;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.DataGrid dataGrid1;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem menuItem3;
        #endregion Form Declaration

        #region Variables Declaration
        public static Dictionary<string, Tuple<double, string, string,int>> applhashdict;
        public static string appName, prevvalue;
        public static Stack applnames;        
        public static DateTime applfocustime;
        public static string applstarttime;
        public static string appltitle;
        public static Form1 form1;
        public static string tempstr;
        public TimeSpan applfocusinterval;
        public DateTime logintime;
        public static string XMLfilepath;
        public static string XMLTemplatePath;
        public static string dbConStr;
        public static int activitySeqNo;
        #endregion
        public Form1()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Form1));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.dataGrid1 = new System.Windows.Forms.DataGrid();
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
            this.SuspendLayout();
            //
            // timer1
            //
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            //
            // notifyIcon1
            //
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.Click += new System.EventHandler(this.notifyIcon1_Click);
            //
            // dataGrid1
            //
            this.dataGrid1.DataMember = "";
            this.dataGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGrid1.GridLineColor = System.Drawing.SystemColors.Highlight;
            this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dataGrid1.Location = new System.Drawing.Point(0, 0);
            this.dataGrid1.Name = "dataGrid1";
            this.dataGrid1.PreferredColumnWidth = 164;
            this.dataGrid1.Size = new System.Drawing.Size(536, 398);
            this.dataGrid1.TabIndex = 0;
            //
            // mainMenu1
            //
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                      this.menuItem1});
            //
            // menuItem1
            //
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                      this.menuItem2,
                                                                                      this.menuItem3});
            this.menuItem1.Text = "&Main";
            //
            // menuItem2
            //
            this.menuItem2.Index = 0;
            this.menuItem2.Shortcut = System.Windows.Forms.Shortcut.F5;
            this.menuItem2.Text = "&Refresh";
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
            //
            // menuItem3
            //
            this.menuItem3.Index = 1;
            this.menuItem3.Text = "E&xit";
            this.menuItem3.Click += new System.EventHandler(this.menuItem3_Click);
            //
            // Form1
            //
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(536, 398);
            this.Controls.Add(this.dataGrid1);
            this.MaximizeBox = false;
            this.Menu = this.mainMenu1;
            this.Name = "Form1";
            this.ShowInTaskbar = false;
            this.Text = "Win Appl Watcher";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Closed += new System.EventHandler(this.Form1_Closed);
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        [STAThread]
        static void Main()
        {
            dbConStr = ConfigurationManager.ConnectionStrings["ProcessDiscoveryDB"].ConnectionString;
            XMLfilepath = ConfigurationManager.AppSettings["XMLFilePath"];
            XMLTemplatePath = ConfigurationManager.AppSettings["XMLFilePath"];
            applnames = new Stack();
            applhashdict = new Dictionary<string, Tuple<double, string, string, int>>();
            form1 = new Form1();
            Application.Run(form1);
        }
        private void timer1_Tick(object sender, System.EventArgs e)
        {
            //This is used to monitor and save active application's  details in Hashtable for future saving in xml file...
            try
            {
                bool isNewAppl = false;
                activitySeqNo = 1;
                IntPtr hwnd = APIFuncs.getforegroundWindow();
                Int32 pid = APIFuncs.GetWindowProcessID(hwnd);
                Process p = Process.GetProcessById(pid);
                appName = p.ProcessName;
                appltitle = APIFuncs.ActiveApplTitle().Trim().Replace("\0", "");
                if (!applnames.Contains(appltitle + "$$$!!!" + appName))
                {
                    applnames.Push(appltitle + "$$$!!!" + appName);
                    applhashdict.Add(appltitle + "$$$!!!" + appName,new Tuple<double, string, string, int>(0,DateTime.Now.ToString(),"0",activitySeqNo));
                    isNewAppl = true;
                }
                if (prevvalue != (appltitle + "$$$!!!" + appName))
                {
                    IDictionaryEnumerator eb = applhashdict.GetEnumerator();
                    applfocusinterval = DateTime.Now.Subtract(applfocustime);
                    //applstarttime = applfocustime.ToString();
                    while (eb.MoveNext())
                    {
                        if (eb.Key.ToString() == prevvalue)
                        {
                            double prevseconds = Convert.ToDouble(applhashdict[eb.Key.ToString()].Item1);
                            if (applhashdict[eb.Key.ToString()].Item2.ToString().Length > 1)
                            {
                                applstarttime = applhashdict[eb.Key.ToString()].Item2.ToString();
                            }
                            
                            applhashdict.Remove(prevvalue);
                            applhashdict.Add(prevvalue, new Tuple<double, string, string, int>(applfocusinterval.TotalSeconds + prevseconds,applstarttime.ToString(),DateTime.Now.ToString(),activitySeqNo+1));
                            try
                            {
                                using (SqlConnection con = new SqlConnection(dbConStr))
                                {
                                    using (SqlCommand cmd = new SqlCommand("uspProcessData", con))
                                    {
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        cmd.Parameters.Add("@WindowTitle", SqlDbType.VarChar).Value = appltitle;
                                        cmd.Parameters.Add("@ProcessName", SqlDbType.VarChar).Value = appName;
                                        cmd.Parameters.Add("@SequenceNo", SqlDbType.VarChar).Value = 0;
                                        cmd.Parameters.Add("@AppStartTime", SqlDbType.VarChar).Value = applstarttime.ToString();
                                        cmd.Parameters.Add("@AppStopTime", SqlDbType.VarChar).Value = DateTime.Now.ToString();
                                        cmd.Parameters.Add("@SeqStartTime", SqlDbType.VarChar).Value = 0;
                                        cmd.Parameters.Add("@SeqStopTime", SqlDbType.VarChar).Value = 0;
                                        cmd.Parameters.Add("@TotalActiveTime", SqlDbType.VarChar).Value = applfocusinterval.TotalSeconds + prevseconds;
                                        con.Open();
                                        cmd.ExecuteNonQuery();
                                        con.Close();
                                    }
                                }
                            }
                            catch(Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            break;
                        }
                    }
                    prevvalue = appltitle + "$$$!!!" + appName;
                    applfocustime = DateTime.Now;                    
                }
                if (isNewAppl)
                    applfocustime = DateTime.Now;                    
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ":" + ex.StackTrace);
            }
        }
        private void Form1_Closed(object sender, System.EventArgs e)
        {
            //This is activated on click on Exit menu item and to show actual and calculated time spent on all applications
            //opened so far and to open IE to show xml contents....
            try
            {
                SaveandShowDetails();
                TimeSpan timeinterval = DateTime.Now.Subtract(logintime);
                System.Diagnostics.EventLog.WriteEntry("Application Watcher Total Time Details", timeinterval.Hours + " Hrs " + timeinterval.Minutes + " Mins", System.Diagnostics.EventLogEntryType.Information);
                MessageBox.Show("Actual Time Spent :" + timeinterval.Hours + " Hrs " + timeinterval.Minutes + " Mins", "Application Watcher Total Time Details");

                StreamReader freader = new StreamReader(XMLfilepath);
                XmlTextReader xmlreader = new XmlTextReader(freader);
                string tottime = "";
                while (xmlreader.Read())
                {
                    if (xmlreader.NodeType == XmlNodeType.Element && xmlreader.Name == "TotalSeconds")
                    {
                        tottime += ";" + xmlreader.ReadInnerXml().ToString();
                    }
                }
                string[] tottimes = tottime.Split(';');
                long totsecs = 0;
                foreach (string str in tottimes)
                {
                    if (str != string.Empty)
                    {
                        if (str.IndexOf("Seconds") != -1)
                        {
                            totsecs += Convert.ToInt64(str.Substring(0, str.Length - 8));

                        }
                        else
                        {
                            totsecs += Convert.ToInt64(str.Substring(0, str.Length - 8)) * 60;
                        }
                    }
                }
                //MessageBox.Show((totsecs / 60) + " Minutes");
                showdetailsinIE();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region User-defined Methods...
        private void showdetailsinIE()
        {
            //To create XSL file,if it is not existing....
            if (!File.Exists(XMLTemplatePath))
            {
                File.Create(XMLTemplatePath).Close();
                string xslcontents = "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?><xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\">" +
                                     "<xsl:template match=\"/\"><html><body><h2>My Applications Details</h2><table border=\"1\"><tr bgcolor=\"#9acd32\"><th>Window Title</th>" +
                                     "<th>Process Name</th><th>Total Time</th></tr><xsl:for-each select=\"ApplDetails/Application_Info\"><xsl:sort select=\"ApplicationName\"/>" +
                                     "<tr><td><xsl:value-of select=\"ProcessName\"/></td><td><xsl:value-of select=\"ApplicationName\"/></td><td><xsl:value-of select=\"TotalSeconds\"/>" +
                                     "</td></tr></xsl:for-each></table></body></html></xsl:template></xsl:stylesheet>";
                StreamWriter xslwriter = new StreamWriter(XMLTemplatePath);
                xslwriter.Write(xslcontents);
                xslwriter.Flush();
                xslwriter.Close();
            }
            //TO show the contents of xml file in IE with a proper xsl....
            System.Diagnostics.Process ie = new Process();
            System.Diagnostics.ProcessStartInfo ieinfo = new ProcessStartInfo(@"C:\Program Files\Internet Explorer\iexplore.exe", XMLfilepath);
            ie.StartInfo = ieinfo;
            bool started = ie.Start();
            Application.Exit();
        }
        private void TestFocusedChanged()
        {
            //This is used to handle hashtable,if its length is 1.It means number of active applications is only one....
            try
            {
                if (applhashdict.Count == 1)
                {
                    IDictionaryEnumerator eb = applhashdict.GetEnumerator();
                    applfocusinterval = DateTime.Now.Subtract(applfocustime);

                    while (eb.MoveNext())
                    {
                        if (eb.Key.ToString() == appltitle + "$$$!!!" + appName)
                        {
							applhashdict.Remove(appltitle + "$$$!!!" + appName);
                            applhashdict.Add(appltitle + "$$$!!!" + appName, new Tuple<double, string, string, int>(applfocusinterval.TotalSeconds,"0", DateTime.Now.ToString(),activitySeqNo));
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void BindGrid()
        {
            //This is used to bind grid with update contents of xml file....
            SaveandShowDetails();
            DataSet ds = new DataSet();
            ds.ReadXml(XMLfilepath);
            dataGrid1.DataSource = ds;
        }
        private void SaveandShowDetails()
        {
            //This is used to save contents of hashtable in a xml file....
            try
            {
                TestFocusedChanged();
                System.IO.StreamWriter writer = new System.IO.StreamWriter(XMLfilepath, false);
                IDictionaryEnumerator eb = applhashdict.GetEnumerator();
                writer.Write("<?xml version=\"1.0\"?>");
                writer.WriteLine("");
                writer.Write("<?xml-stylesheet type=\"text/xsl\" href=\"appl_xsl.xsl\"?>");
                writer.WriteLine("");
                writer.Write("<ApplDetails>");
                while (eb.MoveNext())
                    {
                    if (!applhashdict[eb.Key.ToString()].Item1.ToString().Trim().StartsWith("0"))
                    {
                        writer.Write("<Application_Info>");
                        writer.Write("<ProcessName>");
                        string processname = "<![CDATA[" + eb.Key.ToString().Trim().Substring(0, eb.Key.ToString().Trim().LastIndexOf("$$$!!!")).Trim() + "]]>";
                        processname = processname.Replace("\0", "");
                        writer.Write(processname);
                        writer.Write("</ProcessName>");
                        writer.Write("<ApplicationName>");
                        string applname = "<![CDATA[" + eb.Key.ToString().Trim().Substring(eb.Key.ToString().Trim().LastIndexOf("$$$!!!") + 6).Trim() + "]]>";
                        writer.Write(applname);
                        writer.Write("</ApplicationName>");
                        writer.Write("<StartTime>");
                        writer.Write("<![CDATA[" + applhashdict[eb.Key.ToString()].Item2 + "]]>");
                        writer.Write("</StartTime>");
                        writer.Write("<StopTime>");
                        writer.Write("<![CDATA[" + applhashdict[eb.Key.ToString()].Item3 + "]]>");
                        writer.Write("</StopTime>");
                        writer.Write("<TotalSeconds>");
                        if ((Convert.ToDouble(applhashdict[eb.Key.ToString()].Item1) / 60) < 1)
                        {
                            writer.Write(Convert.ToInt32((Convert.ToDouble(applhashdict[eb.Key.ToString()].Item1))) + " Seconds");
                        }
                        else
                        {
                            writer.Write(Convert.ToInt32((Convert.ToDouble(applhashdict[eb.Key.ToString()].Item1)) / 60) + " Minutes");
                        }
                        writer.Write("</TotalSeconds>");
                        writer.Write("</Application_Info>");
                    }
                }
                writer.Write("</ApplDetails>");
                writer.Flush();
                writer.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion
        private void notifyIcon1_Click(object sender, System.EventArgs e)
        {
            //This is used to show and hide Form....
            try
            {
                if (form1.Visible == true)
                {
                    form1.Visible = false;
                    notifyIcon1.Text = "Application Watcher is in Invisible Mode";
                }
                else
                {
                    form1.Visible = true;
                    form1.Focus();
                    form1.WindowState = FormWindowState.Normal;
                    notifyIcon1.Text = "Application Watcher is in Visible Mode";
                    BindGrid();
                }
            }
            catch { }
        }
        private void Form1_Load(object sender, System.EventArgs e)
        {
            form1.Visible = false;
            notifyIcon1.Text = "Application Watcher is in Invisible Mode";
            logintime = DateTime.Now;
            form1.Text = "Login Time is at :" + DateTime.Now.ToLongTimeString();
            if (!System.IO.File.Exists(XMLfilepath))
            {
                System.IO.File.Create(XMLfilepath).Close();
            }
        }
        private void menuItem2_Click(object sender, System.EventArgs e)
        {
            //This is activated on click on Refresh menu item to load grid with present contents of xml file....
            BindGrid();
        }
        private void menuItem3_Click(object sender, System.EventArgs e)
        {
            //This is activated on click on Exit menu item and to show actual and calculated time spent on all applications
            //opened so far and to open IE to show xml contents....
            SaveandShowDetails();
            TimeSpan timeinterval = DateTime.Now.Subtract(logintime);
            System.Diagnostics.EventLog.WriteEntry("Application Watcher Total Time Details", timeinterval.Hours + " Hrs " + timeinterval.Minutes + " Mins", System.Diagnostics.EventLogEntryType.Information);
            //MessageBox.Show("Actual Time Spent :" + timeinterval.Hours + " Hrs " + timeinterval.Minutes + " Mins", "Application Watcher Total Time Details");

            StreamReader freader = new StreamReader(XMLfilepath);
            XmlTextReader xmlreader = new XmlTextReader(freader);
            string tottime = "";
            while (xmlreader.Read())
            {
                if (xmlreader.NodeType == XmlNodeType.Element && xmlreader.Name == "TotalSeconds")
                {
                    tottime += ";" + xmlreader.ReadInnerXml().ToString();
                }
            }
            string[] tottimes = tottime.Split(';');
            long totsecs = 0;
            foreach (string str in tottimes)
            {
                if (str != string.Empty)
                {
                    if (str.IndexOf("Seconds") != -1)
                    {
                        totsecs += Convert.ToInt64(str.Substring(0, str.Length - 8));

                    }
                    else
                    {
                        totsecs += Convert.ToInt64(str.Substring(0, str.Length - 8)) * 60;
                    }
                }
            }
            //MessageBox.Show((totsecs / 60) + " Minutes");
            showdetailsinIE();
        }
    }
}