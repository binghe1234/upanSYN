using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2_upan
{
    public partial class Form1 : Form
    {
        public const int WM_DEVICECHANGE = 0x219;
        public const int DBT_DEVICEARRIVAL = 0x8000;
        public const int DBT_CONFIGCHANGECANCELED = 0x0019;
        public const int DBT_CONFIGCHANGED = 0x0018;
        public const int DBT_CUSTOMEVENT = 0x8006;
        public const int DBT_DEVICEQUERYREMOVE = 0x8001;
        public const int DBT_DEVICEQUERYREMOVEFAILED = 0x8002;
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        public const int DBT_DEVICEREMOVEPENDING = 0x8003;
        public const int DBT_DEVICETYPESPECIFIC = 0x8005;
        public const int DBT_DEVNODES_CHANGED = 0x0007;
        public const int DBT_QUERYCHANGECONFIG = 0x0017;
        public const int DBT_USERDEFINED = 0xFFFF;
        
        public int filecount = 0;
        List<string> onlyYingPanFiles = new List<string> ();
        List<string> onlyUPanFiles = new List<string>();
        List<string> onlyYingPanDirs = new List<string>();
        List<string> onlyUPanDirs = new List<string>();

        List<string> GongGongYingPanFiles = new List<string>();
        List<string> GongGongUPanFiles = new List<string>();        
        List<string> GongGongYingPanDirs = new List<string>();
        List<string> GongGongUPanDirs = new List<string>();
        List<string> allYingPanFiles = new List<string>();
        IniFiles ini = new IniFiles(Application.StartupPath + @"\MyConfig.INI");//Application.StartupPath只适用于winform窗体程序
        public delegate void DelegateUpdateListView(string mulu, string s);
        public delegate void DelegateUpdateBtn();

        string UPanpath = null;
        string YingPanpath = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string s;
            //notifyIcon1.Visible = false;
            //去掉标记以实现完全隐身状态（此状态只能从任务管理器处关闭程序）
            //TransparencyKey = Color.White  ;
            //Opacity = 0;
            
            //checkDirectory("D:/Music", "E:/Music");
            s = panfu();
            if(s=="")
            {
                textBox1.Text = "请插入U盘并选择目录";
                textBox1.Enabled = false;
                button1.Enabled = false;
                timer1.Enabled = true;
                
            }
            else
            {

                textBox1.Enabled = true ;
                textBox1.Text = s;
                button1.Enabled = true;
                textBox2.Text = "请选择硬盘目录";
                if (ini.ExistINIFile())//验证是否存在文件，存在就读取
                {
                    if (ini.IniReadValue("目录", "U盘")!="" && ini.IniReadValue("目录", "硬盘")!="")
                    {
                        textBox1.Text = ini.IniReadValue("目录", "U盘");
                        textBox2.Text = ini.IniReadValue("目录", "硬盘");
                    }
                    
                }

                UPanpath = Application.StartupPath + @"\UPanPath.ini";
                if (File.Exists(UPanpath))
                {
                    //读取配置文件，并加载到combobox选项中，默认选中第一个选项
                    //StreamReader sr = new StreamReader(path,Encoding.Default);

                    StreamReader sr = new StreamReader(UPanpath, true);

                    while (sr.Peek() > 0)
                    {
                        comboBox_UPan.Items.Add(sr.ReadLine());
                    }
                    sr.Close();
                    //选中combobox第一个
                    comboBox_UPan.Text = (string)comboBox_UPan.Items[0];
                }
                
                YingPanpath = Application.StartupPath + @"\YingPanPath.ini";
                if (File.Exists(YingPanpath))
                {
                    //读取配置文件，并加载到combobox选项中，默认选中第一个选项
                    //StreamReader sr = new StreamReader(path,Encoding.Default);

                    StreamReader sr = new StreamReader(YingPanpath, true);

                    while (sr.Peek() > 0)
                    {
                        comboBox_YingPan.Items.Add(sr.ReadLine());
                    }
                    sr.Close();
                    //选中combobox第一个
                    comboBox_YingPan.Text = (string)comboBox_YingPan.Items[0];
                }
                
                //comboBox_UPan.Items.Add("1.");
                //comboBox_UPan.Items.Add("2.");
                //comboBox_UPan.Items.Add("3.");
                //comboBox_UPan.Items.Add("4.");
                //comboBox_UPan.SelectedIndex = 0;

                //comboBox_YingPan.Items.Add("1.");
                //comboBox_YingPan.Items.Add("2.");
                //comboBox_YingPan.Items.Add("3.");
                //comboBox_YingPan.Items.Add("4.");
                //comboBox_YingPan.SelectedIndex = 1;

            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (textBox1.Text== "请插入U盘并选择目录")
            {
                ini.IniWriteValue("目录", "U盘", "");
            }
            else
            {
                ini.IniWriteValue("目录", "U盘", textBox1.Text);
            }
            if (textBox2.Text == "请选择硬盘目录")
            {
                ini.IniWriteValue("目录", "硬盘", "");
            }
            else
            {
                ini.IniWriteValue("目录", "硬盘", textBox2.Text);
            }

            StreamWriter sw = new StreamWriter(UPanpath);
            for (int i = 0; i < comboBox_UPan.Items.Count; i++)
            {
                sw.WriteLine(comboBox_UPan.Items[i]);
            }
            sw.Close();
            StreamWriter sw1 = new StreamWriter(YingPanpath);
            for (int i = 0; i < comboBox_YingPan.Items.Count; i++)
            {
                sw1.WriteLine(comboBox_YingPan.Items[i]);
            }
            sw1.Close();
        }
        
        FileInfo[] filesYingpan;
        DirectoryInfo[] directsYingPan;
        string UPan = "", YingPan = "";

        //获得u盘下所有文件夹名
        public void getDirectoryUtoY(Object obj)//Object obj  string DirectoryPath
        {
            //搜索U盘下所有子目录            
            DirectoryInfo root = new DirectoryInfo((string )obj); //获取当前目录
            FileInfo[] fis = root.GetFiles();//获取当前目录中的文件
            DirectoryInfo[] directs = root.GetDirectories();//获取当前目录中的文件夹

            UPan = root.FullName;
            //MessageBox.Show("U盘全路径字符串：" + i, "UtoY");
            YingPan = UPan.Remove(0, textBox1.Text.Length);
            //MessageBox.Show("U盘截取后的字符串：" + y, "UtoY");
            YingPan = textBox2.Text + YingPan;
            //MessageBox.Show("截取后的字符串：" + y, "UtoY");
            
            DirectoryInfo rootYingPan = new DirectoryInfo(YingPan); //获取当前目录
            //若硬盘上目录不存在，则创建。
            try
            {
                if (Directory.Exists(rootYingPan.FullName))
                {
                    filesYingpan = rootYingPan.GetFiles();//获取当前硬盘目录中的文件
                    directsYingPan = rootYingPan.GetDirectories();//获取当前硬盘目录中的文件夹
                }
                else
                {
                    
                    Directory.CreateDirectory(rootYingPan.FullName);
                    filesYingpan = rootYingPan.GetFiles();//获取当前目录中的文件
                    directsYingPan = rootYingPan.GetDirectories();//获取当前目录中的文件夹
                    tianchong(rootYingPan.FullName, "硬盘创建目录");
                }
            }
            catch (Exception)
            {

                
            }
            

            //将当前U盘目录中的的文件夹加入list

            Console.WriteLine("当前U盘文件夹<{0}>内的文件夹：", root.FullName );
            foreach (DirectoryInfo directoryInfo  in directs )
            {
                Console.WriteLine("{0}", directoryInfo.Name );
                GongGongUPanDirs.Add(directoryInfo.Name  );
            }
            Console.WriteLine("————————————————————————————————————\n共计{0}个\n", GongGongUPanDirs.Count);

            //将当前硬盘目录中的的文件夹加入list

            Console.WriteLine("当前硬盘文件夹<{0}>内的文件夹：", rootYingPan.FullName );
            foreach (DirectoryInfo directoryInfo in directsYingPan)
            {
                Console.WriteLine("{0}", directoryInfo.Name );
                GongGongYingPanDirs.Add(directoryInfo.Name );
            }
            Console.WriteLine("————————————————————————————————————\n共计{0}个\n", GongGongYingPanDirs.Count);

            //求解list文件夹的差集
            IEnumerable<string> onlyUPanDirsIE = GongGongUPanDirs.Except(GongGongYingPanDirs);
            onlyUPanDirs = onlyUPanDirsIE.ToList<string>();

            IEnumerable<string> onlyYingPanDirsIE = GongGongYingPanDirs.Except(GongGongUPanDirs);
            onlyYingPanDirs = onlyYingPanDirsIE.ToList<string>();

            Console.WriteLine("仅在U盘文件夹<{0}>内的文件夹：", root.FullName);
            foreach (string  item in onlyUPanDirs )
            {
                Console.WriteLine("{0}", item );
            }
            Console.WriteLine("————————————————————————————————————\n共计{0}个\n", onlyUPanDirs.Count );

            Console.WriteLine("仅在硬盘文件夹<{0}>内的文件夹：", rootYingPan.FullName);
            //若U盘上目录不存在，则创建。
            foreach (string  item in onlyYingPanDirs)
            {
                try
                {
                    Console.WriteLine("{0}", item);
                    Directory.CreateDirectory(UPan + @"\" + item);
                    tianchong(UPan + @"\" + item, "U盘创建目录");
                    
                }
                catch (Exception)
                {                    
                }
                
            }
            Console.WriteLine("————————————————————————————————————\n共计{0}个\n", onlyYingPanDirs.Count);           
            

            //将当前U盘目录中的文件加入list
            Console.WriteLine("当前U盘文件夹<{0}>内的文件：\n", root.FullName);
            foreach (FileInfo fileInfo in fis)
            {
                Console.WriteLine("{0}", fileInfo.Name);
                GongGongUPanFiles.Add(fileInfo.Name);
            }
            Console.WriteLine("————————————————————————————————————\n共计{0}个\n", GongGongUPanFiles.Count);

            //将当前硬盘同目录中的文件加入list

            Console.WriteLine("当前硬盘文件夹<{0}>内的文件：\n", rootYingPan.FullName);
            foreach (FileInfo fileInfo in filesYingpan)
            {
                Console.WriteLine("{0}", fileInfo.Name);
                GongGongYingPanFiles.Add(fileInfo.Name);
            }
            Console.WriteLine("————————————————————————————————————\n共计{0}个\n", GongGongYingPanFiles.Count);

            //求解list文件的差集
            IEnumerable<string> onlyUPanFileIE = GongGongUPanFiles.Except(GongGongYingPanFiles);
            onlyUPanFiles = onlyUPanFileIE.ToList<string>();

            IEnumerable<string> onlyYingPanFileIE = GongGongYingPanFiles.Except(GongGongUPanFiles);
            onlyYingPanFiles = onlyYingPanFileIE.ToList<string>();

            Console.WriteLine("仅在U盘文件夹<{0}>内的文件：", root.FullName);
            foreach (string item in onlyUPanFiles)
            {
                try
                {
                    Console.WriteLine("{0}", item);
                    File.Copy(root.FullName + @"\" + item, rootYingPan.FullName + @"\" + item, true);
                    Console.WriteLine("仅在U盘文件夹<{0}>内的文件{1}到硬盘复制成功！", root.FullName, item);
                    tianchong(rootYingPan.FullName + @"\" + item, "U盘文件到硬盘复制成功！");
                    
                }
                catch (Exception)
                {                    
                }
                
            }
            Console.WriteLine("————————————————————————————————————\n共计{0}个\n", onlyUPanFiles.Count);


            Console.WriteLine("仅在硬盘文件夹<{0}>内的文件：", rootYingPan.FullName);            
            foreach (string item in onlyYingPanFiles)
            {
                try
                {
                    Console.WriteLine("{0}", item);
                    File.Copy(rootYingPan.FullName + @"\" + item, root.FullName + @"\" + item, true);
                    Console.WriteLine("仅在硬盘文件夹<{0}>内的文件{1}到U盘复制成功！", rootYingPan.FullName, item);
                    tianchong(root.FullName + @"\" + item, "硬盘文件到U盘复制成功！");
                    
                }
                catch (Exception)
                {                    
                }
                
                //Directory.CreateDirectory(UPan + @"\" + item);
            }
            Console.WriteLine("————————————————————————————————————\n共计{0}个\n", onlyYingPanFiles.Count);

            IEnumerable<string> allYingPanFileIE = GongGongYingPanFiles.Intersect(GongGongUPanFiles);
            allYingPanFiles = allYingPanFileIE.ToList<string>();
            foreach (string  item in allYingPanFiles)
            {
                FileInfo UPan = new FileInfo(root.FullName + @"\" + item);
                FileInfo YingPan = new FileInfo(rootYingPan.FullName + @"\" + item);
                copyFiles(UPan, YingPan );
            }

            GongGongUPanFiles.Clear();
            GongGongUPanDirs.Clear();
            GongGongYingPanFiles.Clear();
            GongGongYingPanDirs.Clear();
            onlyUPanDirs.Clear();
            onlyYingPanDirs.Clear();
            onlyUPanFiles.Clear();
            onlyYingPanFiles.Clear();

            //递归遍历子目录
            foreach (DirectoryInfo d in root.GetDirectories())
            {
                getDirectoryUtoY(d.FullName);
            }
            
        }
        

        //复制文件到TargetPath
        private void copyFiles(FileInfo upanfile, FileInfo yingpanfile)//U盘到硬盘拷贝
        {//自定义被复制文件后缀名
            //MessageBox.Show("时间差="+ (upanfile.LastWriteTimeUtc  - yingpanfile.LastWriteTimeUtc).Hours );
            
            TimeSpan span = upanfile.LastWriteTimeUtc - yingpanfile.LastWriteTimeUtc;

                if (span.TotalHours > 12)
                {
                    try
                    {
                        //MessageBox.Show(upanfile.FullName + "\nU盘修改时间：" + upanfile.LastWriteTimeUtc.ToString() + "(最新)" + "\n硬盘修改时间：" + yingpanfile.LastWriteTimeUtc.ToString() + "\n时间差=" + span.TotalHours);
                        //MessageBox.Show("U盘文件是后修改的:" + upanfile.Name);
                        File.Copy(upanfile.FullName, yingpanfile.FullName, true);
                        //MessageBox.Show("从U盘到硬盘拷贝完成！");
                        tianchong(yingpanfile.FullName, "U盘2硬盘更新");
                    
                }
                    catch (Exception)
                    {                        
                    }
                    
                }
                if (span.TotalHours < -12)
                {
                    try
                    {
                        //MessageBox.Show(yingpanfile.FullName + "\nU盘修改时间：" + upanfile.LastWriteTimeUtc.ToString() + "\n" + "硬盘修改时间：" + yingpanfile.LastWriteTimeUtc.ToString() + "（最新）\n时间差=" + span.TotalHours);
                        //MessageBox.Show("硬盘文件是后修改的:" + yingpanfile.Name);
                        File.Copy(yingpanfile.FullName, upanfile.FullName, true);
                        //MessageBox.Show("从硬盘到U盘拷贝完成！");
                        tianchong(upanfile.FullName, "硬盘2U盘更新");
                    
                }
                    catch (Exception)
                    {                       
                    }
                    
                }
        }

        //获取硬盘事件
        //protected override void WndProc(ref Message m)
        //{
        //    try
        //    {
        //        if (m.Msg == WM_DEVICECHANGE)//获取消息的 ID 号。
        //        {
        //            switch (m.WParam.ToInt32())//获取消息的 System.Windows.Forms.Message.WParam 字段。
        //            {
        //                case WM_DEVICECHANGE:
        //                    break;
        //                case DBT_DEVICEARRIVAL://U盘插入
        //                    DriveInfo[] s = DriveInfo.GetDrives();
        //                    foreach (DriveInfo drive in s)
        //                    {
        //                        Console.WriteLine(drive.DriveType + "驱动类型");
        //                        if (drive.DriveType == DriveType.Removable)
        //                        {
        //                            Console.WriteLine(drive.Name.ToString() + "驱动类型2");
        //                            //MessageBox.Show("有U盘插入！");
        //                            DriveInfo info = new DriveInfo(drive.Name.ToString());

        //                            //MessageBox.Show("盘符：" + drive.Name.ToString() + "   " + "名称：" + info.VolumeLabel);

        //                            Console.WriteLine(info.VolumeLabel + "驱动类型3");
        //                            if (info.VolumeLabel.Equals("金士顿"))
        //                            {
        //                                textBox1.Enabled = true;
        //                                textBox1.Text = drive.Name.ToString();

        //                                //getDirectoryUtoY(textBox1.Text);
        //                                //getDirectoryYtoU(textBox2.Text, false);
        //                                //tianchong("", "同步完成！");

        //                            ///*
        //                            // * 名字叫SANDISK的U盘一旦插入将自动剪切path1内的所有文件到U盘内
        //                            //而不会将此U盘内的文件复制到path1内
        //                            //*/
        //                            //path2 = drive.Name.ToString() + "/Music";
        //                            //Directory.CreateDirectory(drive.Name.ToString() + "/Music");
        //                            //getDirectoryUtoY(path1, true);
        //                            //DirectoryInfo root = new DirectoryInfo(path1);
        //                            //checkDirectory("D:/Music", "E:/Music");
        //                            ////删除文件夹及文件夹内所有文件
        //                            //foreach (FileInfo f in root.GetFiles())
        //                            //{
        //                            //    //File.Delete(f.FullName);
        //                            //}
        //                            //Directory.Delete(path1);

        //                            break;
        //                            }
        //                            else
        //                            {
        //                                //其它U盘一旦插入电脑则将自动复制copyFiles类中已定义后缀名的U盘文件到path1中
        //                                getDirectoryUtoY(textBox1.Text);
        //                                //getDirectoryYtoU(textBox2.Text, false);
        //                                tianchong("", "同步完成！");
        //                            }
        //                            break;
        //                        }
        //                    }
        //                    break;
        //                case DBT_CONFIGCHANGECANCELED:
        //                    break;
        //                case DBT_CONFIGCHANGED:
        //                    break;
        //                case DBT_CUSTOMEVENT:
        //                    break;
        //                case DBT_DEVICEQUERYREMOVE:
        //                    break;
        //                case DBT_DEVICEQUERYREMOVEFAILED:
        //                    break;
        //                case DBT_DEVICEREMOVECOMPLETE:   //U盘卸载 
        //                    break;
        //                case DBT_DEVICEREMOVEPENDING:
        //                    break;
        //                case DBT_DEVICETYPESPECIFIC:
        //                    break;
        //                case DBT_DEVNODES_CHANGED:
        //                    break;
        //                case DBT_QUERYCHANGECONFIG:
        //                    break;
        //                case DBT_USERDEFINED:
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    base.WndProc(ref m);
        //}

        public void TongBu_UtoY(Object obj)//给程序加个壳，这样可以判定线程结束。
        {
            getDirectoryUtoY(obj);
            tianchong("", "同步完成！");
            Btn();            
        }

        private void TongBuBtn_Click(object sender, EventArgs e)//点击按钮 同步目录和文件
        {
            bool a = true;
            if ((textBox1.Text  !="" || textBox1.Text  != "请插入U盘并选择目录") && (textBox2 .Text !="" || textBox2.Text != "请选择硬盘目录"))
            {
                //tianchong("", "开始同步！");
                TongBuBtn.Enabled = false;
                Thread thread = new Thread(new ParameterizedThreadStart(TongBu_UtoY));
                thread.Start(textBox1.Text);//给方法传值
                //getDirectoryUtoY(textBox1.Text);
                
            } 
            else
            {
                MessageBox.Show("请选择需要同步的目录！");
            }
            
        }

        private void Btn()//线程委托程序，在子线程中修改按钮值。
        {

            if (this.InvokeRequired)
            {
                this.BeginInvoke(new DelegateUpdateBtn(Btn));
                return;
            }
            else
            {
                TongBuBtn.Enabled = true;
            }




        }

        private void button1_Click(object sender, EventArgs e) //点击按钮 设置U盘同步目录
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            path.SelectedPath = ini.IniReadValue("目录", "U盘");
            path.ShowDialog();
            this.textBox1.Text = path.SelectedPath;

            StreamWriter sw = new StreamWriter(UPanpath, true);
            if (!(comboBox_UPan.Items.Contains(textBox1 .Text)))//comboBox_UPan.Text
            {
                sw.WriteLine(textBox1.Text);
                comboBox_UPan.Items.Insert(0, textBox1.Text);
                comboBox_UPan.Text = (string)comboBox_UPan.Items[0];
            }
            else
            {
                comboBox_UPan.Items .RemoveAt ( comboBox_UPan.FindString(textBox1.Text ));
                comboBox_UPan.Items.Insert(0, textBox1.Text);
                comboBox_UPan.Text = (string)comboBox_UPan.Items[0];
            }
            sw.Close();
        }


        private void button2_Click(object sender, EventArgs e)//点击按钮 设置硬盘同步目录
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            //path.SelectedPath = @"F:\BaiduYunDownload\化学文件\"; 
            path.SelectedPath = ini.IniReadValue("目录", "硬盘");
            path.ShowDialog();
            this.textBox2.Text = path.SelectedPath;

            StreamWriter sw = new StreamWriter(YingPanpath, true);
            if (!(comboBox_YingPan.Items.Contains(textBox2.Text)))
            {
                sw.WriteLine(textBox2.Text);
                comboBox_YingPan.Items.Insert(0, textBox2.Text);
                comboBox_YingPan.Text = (string)comboBox_YingPan.Items[0];
            }
            else
            {
                comboBox_YingPan.Items.RemoveAt(comboBox_YingPan.FindString(textBox2.Text));
                comboBox_YingPan.Items.Insert(0, textBox2.Text);
                comboBox_YingPan.Text = (string)comboBox_YingPan.Items[0];
            }
            sw.Close();
        }        
        //清空listbox
        private void button4_Click(object sender, EventArgs e)//点击按钮 清空listbox
        {
            listView1.Items .Clear ();            
        }


        static int it = 2;
        //填充listbox
        private  void  tianchong(string mulu,string s)
        {

            if (this.InvokeRequired)
            {
                this.BeginInvoke(new DelegateUpdateListView(tianchong),mulu ,s );
                return;
            }
            else
            {
                this.listView1.BeginUpdate();  //数据更新，UI暂时挂起，直到EndUpdate绘制控件，可以有效避免闪烁并大大提高加载速度 
                ListViewItem lvi = new ListViewItem();
                //lvi.ImageIndex = i;     //通过与imageList绑定，显示imageList中第i项图标 
                if (it % 2 == 0)
                {
                    lvi.BackColor = Color.Moccasin;
                    lvi.ForeColor = Color.MidnightBlue; //Teal
                }
                else
                {
                    lvi.BackColor = Color.NavajoWhite;
                    lvi.ForeColor = Color.MidnightBlue;
                }

                lvi.Text = mulu;
                lvi.SubItems.Add(s);
                this.listView1.Items.Add(lvi);
                this.listView1.EnsureVisible(this.listView1.Items.Count - 1);
                this.listView1.EndUpdate();  //结束数据处理，UI界面一次性绘制
                it++;
            }


            

        }//可以实现在子线程中填充listbox ，委托函数



        private string panfu()
        {
            string s="";
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
            {
                //判断是不是U盘
                if (d.DriveType == DriveType.Removable)
                {
                    s = d.Name;
                }
            }
            return s;
        }
        //定时器事件：检查是否有U盘插入，直到有U盘插入时，停止事件发生
        private void JianChaUPan(object sender, EventArgs e)
        {
            string s = "";
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
            {
                //判断是不是U盘
                if (d.DriveType == DriveType.Removable)
                {
                    s = d.Name;
                    textBox1.Enabled = true;
                    textBox1.Text = s ;
                    timer1.Enabled = false;
                    if (ini.ExistINIFile())//验证是否存在文件，存在就读取
                    {
                        if (ini.IniReadValue("目录", "U盘") != "" && ini.IniReadValue("目录", "硬盘") != "")
                        {
                            textBox1.Text = ini.IniReadValue("目录", "U盘");
                            textBox2.Text = ini.IniReadValue("目录", "硬盘");
                        }

                    }
                }
            }
        }        


    }
    public class IniFiles
    {
        public string inipath;

        //声明API函数

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        /// <summary> 
        /// 构造方法 
        /// </summary> 
        /// <param name="INIPath">文件路径</param> 
        public IniFiles(string INIPath)
        {
            inipath = INIPath;
        }

        public IniFiles() { }

        /// <summary> 
        /// 写入INI文件 
        /// </summary> 
        /// <param name="Section">项目名称(如 [TypeName] )</param> 
        /// <param name="Key">键</param> 
        /// <param name="Value">值</param> 
        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.inipath);
        }
        /// <summary> 
        /// 读出INI文件 
        /// </summary> 
        /// <param name="Section">项目名称(如 [TypeName] )</param> 
        /// <param name="Key">键</param> 
        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(500);
            int i = GetPrivateProfileString(Section, Key, "", temp, 500, this.inipath);
            return temp.ToString();
        }
        /// <summary> 
        /// 验证文件是否存在 
        /// </summary> 
        /// <returns>布尔值</returns> 
        public bool ExistINIFile()
        {
            return File.Exists(inipath);
        }
    }

    public  class UpanStream
    {

    }
}                           
