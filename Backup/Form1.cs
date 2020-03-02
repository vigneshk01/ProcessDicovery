using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
//Added New Namespaces....
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml;
using System.IO;

namespace WinApplWatcher
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Timer timer1;
		private System.ComponentModel.IContainer components;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
		private System.Windows.Forms.DataGrid dataGrid1;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem3;
		
		
		#region Variables Declaration
		public static string appName,prevvalue;
		public static    Stack applnames;
		public static    Hashtable applhash;
		public static   DateTime  applfocustime;
		public static   string appltitle;
		public static Form1 form1;
		public static string tempstr;
		public TimeSpan applfocusinterval;
		public DateTime logintime;
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

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
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
			applnames = new Stack();
			applhash  = new Hashtable();
			form1= new Form1();
			Application.Run(form1);
		}
		private void timer1_Tick(object sender, System.EventArgs e)
		{
			//This is used to monitor and save active application's  details in Hashtable for future saving in xml file...
			try
			{
				bool isNewAppl = false;
				IntPtr hwnd =APIFuncs.getforegroundWindow();
				Int32 pid = APIFuncs.GetWindowProcessID(hwnd);
				Process p = Process.GetProcessById(pid);
				appName = p.ProcessName;
				appltitle = APIFuncs.ActiveApplTitle().Trim().Replace("\0","");        
				if(!applnames.Contains(appltitle+"$$$!!!"+appName))
				{
					applnames.Push(appltitle+"$$$!!!"+appName);
					applhash.Add(appltitle+"$$$!!!"+appName,0);
					isNewAppl = true;
				}
				if(prevvalue!=(appltitle+"$$$!!!"+appName))
				{
					IDictionaryEnumerator en = applhash.GetEnumerator();
					applfocusinterval = DateTime.Now.Subtract(applfocustime);
					while (en.MoveNext())
					{
						if(en.Key.ToString() == prevvalue)
						{
							double prevseconds =Convert.ToDouble(en.Value);
							applhash.Remove(prevvalue);
							applhash.Add(prevvalue,(applfocusinterval.TotalSeconds+prevseconds));
							break;
						}
					}
				    prevvalue= appltitle+"$$$!!!"+appName;
					applfocustime = DateTime.Now;
				}
				if(isNewAppl)
				applfocustime = DateTime.Now;
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message+":"+ex.StackTrace);
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
				System.Diagnostics.EventLog.WriteEntry("Application Watcher Total Time Details",timeinterval.Hours+" Hrs "+timeinterval.Minutes+" Mins",System.Diagnostics.EventLogEntryType.Information);
				MessageBox.Show("Actual Time Spent :"+timeinterval.Hours+" Hrs "+timeinterval.Minutes+" Mins","Application Watcher Total Time Details");

				StreamReader freader = new StreamReader(@"c:\appldetails.xml");
				XmlTextReader xmlreader = new XmlTextReader(freader);
				string tottime = "";
				while(xmlreader.Read())
				{
					if(xmlreader.NodeType== XmlNodeType.Element && xmlreader.Name=="TotalSeconds")
					{
						tottime += ";"+xmlreader.ReadInnerXml().ToString(); 
					}
				}
				string[] tottimes = tottime.Split(';');
				long totsecs = 0;
				foreach(string str in tottimes)
				{
					if(str != string.Empty)
					{
						if(str.IndexOf("Seconds") != -1)
						{
							totsecs += Convert.ToInt64(str.Substring(0,str.Length-8));

						}
						else
						{
							totsecs += Convert.ToInt64(str.Substring(0,str.Length-8)) * 60;
						}
					}
				}
				MessageBox.Show((totsecs/60)+" Minutes");
				showdetailsinIE();
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		#region User-defined Methods...
		private void showdetailsinIE()
		{
			//To create XSL file,if it is not existing....
			if(!File.Exists(@"c:\appl_xsl.xsl"))
			{
				File.Create(@"c:\appl_xsl.xsl").Close();
				string xslcontents ="<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?><xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\"><xsl:template match=\"/\"> <html> <body>  <h2>My Applications Details</h2>  <table border=\"1\"> <tr bgcolor=\"#9acd32\">  <th>Window Title</th>  <th>Process Name</th>  <th>Total Time</th> </tr> <xsl:for-each select=\"ApplDetails/Application_Info\"><xsl:sort select=\"ApplicationName\"/> <tr>  <td><xsl:value-of select=\"ProcessName\"/></td>  <td><xsl:value-of select=\"ApplicationName\"/></td>  <td><xsl:value-of select=\"TotalSeconds\"/></td> </tr> </xsl:for-each>  </table> </body> </html></xsl:template></xsl:stylesheet>";
				StreamWriter xslwriter = new StreamWriter(@"c:\appl_xsl.xsl");
				xslwriter.Write(xslcontents);
				xslwriter.Flush();
				xslwriter.Close();
			}
			//TO show the contents of xml file in IE with a proper xsl....
			System.Diagnostics.Process ie = new Process();
			System.Diagnostics.ProcessStartInfo ieinfo = new ProcessStartInfo(@"C:\Program Files\Internet Explorer\iexplore.exe",@"c:\appldetails.xml");
			ie.StartInfo = ieinfo;
			bool started =	ie.Start();
			Application.Exit();
		}
		private void TestFocusedChanged()
		{
			//This is used to handle hashtable,if its length is 1.It means number of active applications is only one....
			try
			{
				if(applhash.Count == 1)
				{
					IDictionaryEnumerator en = applhash.GetEnumerator();
					applfocusinterval = DateTime.Now.Subtract(applfocustime);
					
					while (en.MoveNext())
					{
						if(en.Key.ToString() == appltitle+"$$$!!!"+appName)
						{
							applhash.Remove(appltitle+"$$$!!!"+appName);
							applhash.Add(appltitle+"$$$!!!"+appName,applfocusinterval.TotalSeconds);
							break;
						}
					}
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		private void BindGrid()
		{
			//This is used to bind grid with update contents of xml file....
			SaveandShowDetails();
			DataSet ds = new DataSet();
			ds.ReadXml(@"c:\appldetails.xml");
			dataGrid1.DataSource = ds;
		}
		private void SaveandShowDetails()
		{
			//This is used to save contents of hashtable in a xml file....
			try
			{
				TestFocusedChanged();
				System.IO.StreamWriter writer = new System.IO.StreamWriter(@"c:\appldetails.xml",false);
				IDictionaryEnumerator en = applhash.GetEnumerator();
				writer.Write("<?xml version=\"1.0\"?>");
				writer.WriteLine("");
				writer.Write("<?xml-stylesheet type=\"text/xsl\" href=\"appl_xsl.xsl\"?>");
				writer.WriteLine("");
				writer.Write("<ApplDetails>");
				while (en.MoveNext())
				{
					if(!en.Value.ToString().Trim().StartsWith("0"))
					{
						writer.Write("<Application_Info>");
						writer.Write("<ProcessName>");	
						string processname =	"<![CDATA["+en.Key.ToString().Trim().Substring(0,en.Key.ToString().Trim().LastIndexOf("$$$!!!")).Trim()+"]]>";
						processname        = processname.Replace("\0","");
						writer.Write(processname);
						writer.Write("</ProcessName>");			
				
						writer.Write("<ApplicationName>");
						string applname = "<![CDATA["+en.Key.ToString().Trim().Substring(en.Key.ToString().Trim().LastIndexOf("$$$!!!")+6).Trim()+"]]>";
				
						writer.Write(applname);
						writer.Write("</ApplicationName>");			
				
						writer.Write("<TotalSeconds>");			
						if((Convert.ToDouble(en.Value)/60) < 1)
						{
							writer.Write(Convert.ToInt32((Convert.ToDouble(en.Value)))+" Seconds");
						}
						else
						{
							writer.Write(Convert.ToInt32((Convert.ToDouble(en.Value))/60)+" Minutes");
						}
						writer.Write("</TotalSeconds>");
						writer.Write("</Application_Info>");
					}
				}
				writer.Write("</ApplDetails>");
				writer.Flush();
				writer.Close();
			}
			catch(Exception ex)
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
				if(form1.Visible == true)
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
			catch{}
		}
		private void Form1_Load(object sender, System.EventArgs e)
		{
			form1.Visible = false;
			notifyIcon1.Text = "Application Watcher is in Invisible Mode";
			logintime = DateTime.Now;
			form1.Text = "Login Time is at :"+DateTime.Now.ToLongTimeString();
			if(!System.IO.File.Exists(@"c:\appldetails.xml"))
			{
				System.IO.File.Create(@"c:\appldetails.xml").Close();
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
			System.Diagnostics.EventLog.WriteEntry("Application Watcher Total Time Details",timeinterval.Hours+" Hrs "+timeinterval.Minutes+" Mins",System.Diagnostics.EventLogEntryType.Information);
			MessageBox.Show("Actual Time Spent :"+timeinterval.Hours+" Hrs "+timeinterval.Minutes+" Mins","Application Watcher Total Time Details");

			StreamReader freader = new StreamReader(@"c:\appldetails.xml");
			XmlTextReader xmlreader = new XmlTextReader(freader);
			string tottime = "";
			while(xmlreader.Read())
			{
				if(xmlreader.NodeType== XmlNodeType.Element && xmlreader.Name=="TotalSeconds")
				{
					tottime += ";"+xmlreader.ReadInnerXml().ToString(); 
				}
			}
			string[] tottimes = tottime.Split(';');
			long totsecs = 0;
			foreach(string str in tottimes)
			{
				if(str != string.Empty)
				{
					if(str.IndexOf("Seconds") != -1)
					{
						totsecs += Convert.ToInt64(str.Substring(0,str.Length-8));

					}
					else
					{
						totsecs += Convert.ToInt64(str.Substring(0,str.Length-8)) * 60;
					}
				}
			}
			MessageBox.Show((totsecs/60)+" Minutes");
			showdetailsinIE();
		}
	}
}
