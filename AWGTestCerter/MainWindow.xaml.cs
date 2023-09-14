using AWGTestCerter.Common;
using AWGTestCerter.Remote;
using CommonLibrary.Enum;
using CommonLibrary.EnumHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;


namespace AWGTestCerter
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            Init();
        }
        private int _1652ASocketID = 0;
        private int _1652BSocketID = 1;
        private int _nrxSocketID = 4;
        private int _mxr604aSocketID = 5;
        private int _mso254aSocketID = 6;
        private int _fsw50SocketID = 7;
        private int _fsmr50SocketID = 8;

        private BackgroundWorker backgroundWorkerPowerCali;      //DC-2GHz幅度校准子任务

        string[] CaliLineStr    = new string[200];
    
  

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            ChannelComoBox = CHANNEL.CHA_1;
            Check_NRX = true;
            Check_Awg1652B = true;
    
            return;
        }

        /// <summary>
        /// 滤波器类型
        /// </summary>
        private CHANNEL _channelComoBox;

        /// <summary>
        /// 滤波器类型
        /// </summary>
        public CHANNEL ChannelComoBox
        {
            get { return _channelComoBox; }
            set
            {
                _channelComoBox = value;
                OnPropertyChanged("ChannelComoBox");
            }
        }

        public Dictionary<CHANNEL, string> ChannelComoBoxDictionary
        {
            get
            {
               return System.Enum.GetValues(typeof(CHANNEL)).Cast<CHANNEL>().ToDictionary(item => item, item => (EnumHelper.GetEnumDescription(item)).ToString());
            }
        }

        /// <summary>
        ///
        /// </summary>
        private bool _check_Awg1652B;

        /// <summary>
        ///  
        /// </summary>
        public bool Check_Awg1652B
        {
            get
            {
                return _check_Awg1652B;
            }
            set
            {
                _check_Awg1652B = value;
                if (_check_Awg1652B == true)
                {
                    Check_Awg1652A = false;
                }
                OnPropertyChanged("Check_Awg1652B");
            }
        }


        /// <summary>
        ///
        /// </summary>
        private bool _check_Awg1652A;

        /// <summary>
        ///  
        /// </summary>
        public bool Check_Awg1652A
        {
            get
            {
                return _check_Awg1652A;
            }
            set
            {
                _check_Awg1652A = value;
                if(_check_Awg1652A  == true)
                {
                    Check_Awg1652B = false;
                }
                OnPropertyChanged("Check_Awg1652A");
            }
        }

        /// <summary>
        ///
        /// </summary>
        private bool _check_NRX;

        /// <summary>
        ///  
        /// </summary>
        public bool Check_NRX
        {
            get
            {
                return _check_NRX;
            }
            set
            {
                _check_NRX = value;
                if(_check_NRX == true)
                {
                    Check_MXR604A = false;
                    Check_MSO254A = false;
                    Check_FSW50 = false;
                    Check_FSMR50 = false;
                }
                OnPropertyChanged("Check_NRX");
            }
        }


        /// <summary>
        ///
        /// </summary>
        private bool _check_MXR604A;

        /// <summary>
        ///  
        /// </summary>
        public bool Check_MXR604A
        {
            get
            {
                return _check_MXR604A;
            }
            set
            {
                _check_MXR604A = value;
                if (_check_MXR604A == true)
                {
                    Check_NRX = false;
                    Check_MSO254A = false;
                    Check_FSW50 = false;
                    Check_FSMR50 = false;
                }
                OnPropertyChanged("Check_MXR604A");
            }
        }

        /// <summary>
        ///
        /// </summary>
        private bool _check_MSO254A;

        /// <summary>
        ///  
        /// </summary>
        public bool Check_MSO254A
        {
            get
            {
                return _check_MSO254A;
            }
            set
            {
                _check_MSO254A = value;
                if (_check_MSO254A == true)
                {
                    Check_NRX = false;
                    Check_MXR604A = false;
                    Check_FSW50 = false;
                    Check_FSMR50 = false;
                }
                OnPropertyChanged("Check_MSO254A");
            }
        }

        /// <summary>
        ///
        /// </summary>
        private bool _check_FSW50;

        /// <summary>
        ///  
        /// </summary>
        public bool Check_FSW50
        {
            get
            {
                return _check_FSW50;
            }
            set
            {
                _check_FSW50 = value;
                if (_check_FSW50 == true)
                {
                    Check_NRX = false;
                    Check_MXR604A = false;
                    Check_MSO254A = false;
                    Check_FSMR50 = false;
                }
                OnPropertyChanged("Check_FSW50");
            }
        }

        /// <summary>
        ///
        /// </summary>
        private bool _check_FSMR50;

        /// <summary>
        ///  
        /// </summary>
        public bool Check_FSMR50
        {
            get
            {
                return _check_FSMR50;
            }
            set
            {
                _check_FSMR50 = value;
                if (_check_FSMR50 == true)
                {
                    Check_NRX = false;
                    Check_MXR604A = false;
                    Check_MSO254A = false;
                    Check_FSW50 = false;
                }
                OnPropertyChanged("Check_FSMR50");
            }
        }

        /// <summary>
        /// 创建幅度校准子任务
        /// </summary>
        public void CreateBackgroundWorkerPowerCali()
        {
            backgroundWorkerPowerCali = new BackgroundWorker();
            backgroundWorkerPowerCali.WorkerReportsProgress = true;
            backgroundWorkerPowerCali.WorkerSupportsCancellation = true;
            backgroundWorkerPowerCali.DoWork += backgroundWorkerPowerCali_DoWork;
            backgroundWorkerPowerCali.ProgressChanged += backgroundWorkerPowerCali_ProgressChanged;
            backgroundWorkerPowerCali.RunWorkerCompleted += backgroundWorkerPowerCali_Completed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ReadData"></param>
        private void Clear(double[,] array)
        {
            int i, j;
            int row = array.GetLength(0);//获取维数，这里指行数
            int col = array.GetLength(1);//获取指定维度中的元素个数，这里也就是列数了
            for (i = 0; i < row; i++)
            {
                for (j = 0; j < col; j++)
                {
                    array[i, j] = 0 - 200.0;
                }
            }
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="couple"></param>
        /// <param name="vpp"></param>
        /// <param name="maxVpp"></param>
        /// <param name="minVpp"></param>
        private void GetMaxMin_1652B_Vpp(int couple, double vpp, out double maxVpp, out double minVpp)
        {
            double deltaVpp = 0;
            maxVpp = 0;
            minVpp = 0;
            if (couple == TywCommon.COUPLETYPE_DC)
            {
                deltaVpp = 0.05 * vpp + 0.035;//5%,35mvpp
                maxVpp = vpp + deltaVpp;
                minVpp = vpp - deltaVpp;
            }
            if (couple == TywCommon.COUPLETYPE_AC)
            {
                if (vpp < 0.70001)
                {
                    deltaVpp = 0.05 * vpp + 0.035;//5%,35mvpp
                    maxVpp = vpp + deltaVpp;
                    minVpp = vpp - deltaVpp;
                }
                else
                {
                    deltaVpp = 0.05 * vpp + 0.05;//5%,50mvpp
                    maxVpp = vpp + deltaVpp;
                    minVpp = vpp - deltaVpp;
                }
            }
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vpp"></param>
        /// <param name="maxVpp"></param>
        /// <param name="minVpp"></param>
        private void GetMaxMin_1652A_Vpp(double vpp, out double maxVpp, out double minVpp)
        {
            double deltaVpp = 0;
            maxVpp = 0;
            minVpp = 0;         
            if (vpp < 2.0001)
            {
                deltaVpp = 0.05 * vpp + 0.030;//5%,30mvpp
                maxVpp = vpp + deltaVpp;
                minVpp = vpp - deltaVpp;
            }
            else if(vpp < 3.0001)
            {
                deltaVpp = 0.05 * vpp + 0.050;//5%,50mvpp
                maxVpp = vpp + deltaVpp;
                minVpp = vpp - deltaVpp;
            }
            else
            {
                deltaVpp = 0.05 * vpp + 0.1;//5%,100mvpp
                maxVpp = vpp + deltaVpp;
                minVpp = vpp - deltaVpp;
            }    
           return;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="awg1652x">1652A or 1652B, 0表示1652A，1表示1652B</param>
        /// <param name="who">1652通道</param>
        private void GetVppByNRX(BackgroundWorker _worker, int awg1652x, int who)
        {
            int i;
            double maxValue = 10;
            double minValue = 0 - 50;
            double d, vpp = 0.05, centerFreq = 0.0;
            double _nrxFreq = 0.0;
            double _awg1652_freq = 0.0;
            int chgFreqSleepTime = 500;
            string cmdStr = "";
            List<double> freqSlotList = new List<double>();
            List<double> vppSlotList = new List<double>();
            List<double> getPowerList = new List<double>();
            string srcFile = @"1652A_Slot.txt";
            string saveFileName;
            string[] tBuffer;
            int time_out = 500;
            int Count = 61 + 88;
 
            if (awg1652x == 0)
            {
                srcFile = @"1652A_Slot.txt";
                saveFileName = string.Format("D:\\AWG1652A_CH{0}_dBm.csv", (who + 1).ToString());
                //程控1652A，配置初始状态              
                cmdStr = string.Format("AWGC:RF:CH{0} ON", (who + 1).ToString());
                MyClientSocketLAN.OnSendMsg(cmdStr, _1652ASocketID);//射频通道开关
                Thread.Sleep(500);
                MyClientSocketLAN.OnSendMsg("AWGC:RUN ON", _1652ASocketID);//播放使能
                Thread.Sleep(500);           
                MyClientSocketLAN.OnSendMsg("AWGC:MODE DDFS", _1652ASocketID);//模式切换
                Thread.Sleep(500);
                cmdStr = string.Format("AWGC:CHA {0}", who.ToString());
                MyClientSocketLAN.OnSendMsg(cmdStr, _1652ASocketID);//通道切换
                Thread.Sleep(500);
                MyClientSocketLAN.OnSendMsg("DDFS:WAVE SIN", _1652ASocketID);//波形为正弦
                Thread.Sleep(500);
                MyClientSocketLAN.OnSendMsg("DDFS:SIN:FREQ 10MHz", _1652ASocketID);
                Thread.Sleep(500);
                MyClientSocketLAN.OnSendMsg("CHS:OUT:POW 0.5", _1652ASocketID);
                Thread.Sleep(500);

                //NRX功率计载波频率
                MyClientSocketLAN.SetNRXCarrierFreq(50000000.0, _nrxSocketID);
                Thread.Sleep(chgFreqSleepTime);

                using (FileStream fr = new FileStream(srcFile, FileMode.Open))
                {
                    StreamReader sr = new StreamReader(fr);
                    while (!sr.EndOfStream)
                    {
                        cmdStr = sr.ReadLine();
                        tBuffer = cmdStr.Split(',');
                        if (tBuffer.Length == 2)
                        {
                            freqSlotList.Add(Convert.ToDouble(tBuffer[0]));//中心频率
                            vppSlotList.Add(Convert.ToDouble(tBuffer[1]));//峰峰值
                        }
                    }
                    sr.Close();
                    fr.Close();
                }
                Count = freqSlotList.Count;
                MyClientSocketLAN.OnSendMsg("CHS:OUT:DCVOF 0.0", _1652ASocketID);
                cmdStr = string.Format("AWGC:CHA {0}", who.ToString());
                MyClientSocketLAN.OnSendMsg(cmdStr, _1652ASocketID);//通道切换
                Thread.Sleep(500);
                MyClientSocketLAN.OnSendMsg("CHS:OUT:DCVOF 0.0", _1652ASocketID);
                Thread.Sleep(300);
                Count = freqSlotList.Count;
                for (i = 0; i < freqSlotList.Count; i++)
                {
                    centerFreq = freqSlotList[i];
                    vpp = vppSlotList[i];
                    //设置1652A频率和峰峰值
                    if (i == 0)
                    {
                        _awg1652_freq = centerFreq;
                        cmdStr = string.Format("DDFS:SIN:FREQ {0}", centerFreq.ToString("0.000"));
                        MyClientSocketLAN.OnSendMsg(cmdStr, _1652ASocketID);//信号频率
                        Thread.Sleep(500);
                    }
                    else
                    {
                        if (Math.Abs(centerFreq - _awg1652_freq) > 10.0)
                        {
                            _awg1652_freq = centerFreq;
                            cmdStr = string.Format("DDFS:SIN:FREQ {0}", centerFreq.ToString("0.000"));
                            MyClientSocketLAN.OnSendMsg(cmdStr, _1652ASocketID);//信号频率
                            Thread.Sleep(500);
                        }
                    }
                    cmdStr = string.Format("CHS:OUT:POW {0}", vpp.ToString("0.000"));
                    MyClientSocketLAN.OnSendMsg(cmdStr, _1652ASocketID);//发指令
                    Thread.Sleep(500);//等待回读
                    //NRX功率计载波频率                
                    if(i == 0)
                    {
                        _nrxFreq = centerFreq;
                        MyClientSocketLAN.SetNRXCarrierFreq(_nrxFreq, _nrxSocketID);
                    }
                    else
                    {
                        if(Math.Abs(centerFreq- _nrxFreq)>1000000.0)
                        {
                            _nrxFreq = centerFreq;
                            MyClientSocketLAN.SetNRXCarrierFreq(_nrxFreq, _nrxSocketID);
                        }
                    }
                    Thread.Sleep(chgFreqSleepTime);
                    //读取功率值
                    MyClientSocketLAN.GetNRXPower1(out d, time_out, _nrxSocketID);
                    if (i == 0)
                    {
                        Thread.Sleep(2000);
                        MyClientSocketLAN.GetNRXPower1(out d, time_out, _nrxSocketID);
                    }
                    getPowerList.Add(d);
                    _worker.ReportProgress(100 * i / Count);//显示校准进度 
                }
                //保存数据文件
                ParseData.ParseData.DeleteFile(saveFileName);
                using (FileStream fw = new FileStream(saveFileName, FileMode.Create))
                {
                    StreamWriter sw = new StreamWriter(fw);//写.csv或.txt文件
                    cmdStr = string.Format("Freq(MHz),SetValue(dBm),CH{0},MaxValue(dBm),MinValue(dBm)\r\n", (who + 1).ToString());
                    sw.Write(cmdStr);
                    for (i = 0; i < getPowerList.Count; i++)
                    {
                        GetMaxMin_1652A_Vpp(vppSlotList[i], out maxValue, out minValue);
                        maxValue = ConvertFunc.VppToDbm(maxValue);
                        minValue = ConvertFunc.VppToDbm(minValue);
                        cmdStr = string.Format(" {0}MHz,{1},{2},{3},{4}\r\n", ( freqSlotList[i]/1000000.0).ToString("0.0"),
                                                                                 ConvertFunc.VppToDbm(vppSlotList[i]).ToString("0.00"),
                                                                                 getPowerList[i].ToString("0.00"),
                                                                                 maxValue.ToString("0.00"),
                                                                                 minValue.ToString("0.00")); 
                        sw.Write(cmdStr);
                    }
                    sw.Close();
                    fw.Close();
                }

            }
            if (awg1652x == 1)
            {              
                //程控1652B，配置初始状态              
                cmdStr = string.Format("AWGC:RF:CH{0} ON", (who + 1).ToString());
                MyClientSocketLAN.OnSendMsg(cmdStr, _1652BSocketID);//射频通道开关
                Thread.Sleep(500);
                MyClientSocketLAN.OnSendMsg("AWGC:RUN ON", _1652BSocketID);//播放使能
                Thread.Sleep(500);       
                MyClientSocketLAN.OnSendMsg("AWGC:MODE DDFS", _1652BSocketID);//模式切换
                Thread.Sleep(500);
                cmdStr = string.Format("AWGC:CHA {0}", who.ToString());
                MyClientSocketLAN.OnSendMsg(cmdStr, _1652BSocketID);//通道切换
                Thread.Sleep(500);
                MyClientSocketLAN.OnSendMsg("DDFS:WAVE SIN", _1652BSocketID);//波形为正弦
                Thread.Sleep(500);
                MyClientSocketLAN.OnSendMsg("DDFS:SIN:FREQ 10MHz", _1652BSocketID);
                Thread.Sleep(500);
                MyClientSocketLAN.OnSendMsg("CHS:OUT:POW 0.5", _1652BSocketID);
                Thread.Sleep(500);

                //NRX功率计载波频率
                MyClientSocketLAN.SetNRXCarrierFreq(50000000.0, _nrxSocketID);
                Thread.Sleep(chgFreqSleepTime);

                //------------------： DC耦合
                //------------------： DC耦合
                freqSlotList.Clear();
                vppSlotList.Clear();
                getPowerList.Clear();
                srcFile = @"1652B_DC_Slot.txt";
                saveFileName = string.Format("D:\\AWG1652B_DC_CH{0}_dBm.csv", (who + 1).ToString());
                using (FileStream fr = new FileStream(srcFile, FileMode.Open))
                {
                    StreamReader sr = new StreamReader(fr);
                    while (!sr.EndOfStream)
                    {
                        cmdStr = sr.ReadLine();
                        tBuffer = cmdStr.Split(',');
                        if (tBuffer.Length == 2)
                        {
                            freqSlotList.Add(Convert.ToDouble(tBuffer[0]));//中心频率
                            vppSlotList.Add(Convert.ToDouble(tBuffer[1]));//峰峰值                          
                        }
                    }
                    sr.Close();
                    fr.Close();
                }

                MyClientSocketLAN.OnSendMsg("CHS:OUT:COUP DC", _1652BSocketID);
                Thread.Sleep(300);
                cmdStr = string.Format("AWGC:CHA {0}", who.ToString());
                MyClientSocketLAN.OnSendMsg(cmdStr, _1652BSocketID);//通道切换
                Thread.Sleep(500);
                MyClientSocketLAN.OnSendMsg("CHS:OUT:COUP DC", _1652BSocketID);
                Thread.Sleep(300);
                MyClientSocketLAN.OnSendMsg("CHS:OUT:DCVOF 0.0", _1652BSocketID);
                Thread.Sleep(300);
                time_out = 400;
                for (i = 0; i < freqSlotList.Count; i++)
                {
                    centerFreq = freqSlotList[i];
                    vpp = vppSlotList[i];
                    //设置1652B频率和峰峰值
                    if(i == 0)
                    {
                        _awg1652_freq = centerFreq;
                        cmdStr = string.Format("DDFS:SIN:FREQ {0}", centerFreq.ToString("0.000"));
                        MyClientSocketLAN.OnSendMsg(cmdStr, _1652BSocketID);//信号频率
                        Thread.Sleep(500);
                    }
                    else
                    {
                        if(Math.Abs(centerFreq - _awg1652_freq) > 10.0)
                        {
                            _awg1652_freq = centerFreq;
                            cmdStr = string.Format("DDFS:SIN:FREQ {0}", centerFreq.ToString("0.000"));
                            MyClientSocketLAN.OnSendMsg(cmdStr, _1652BSocketID);//信号频率
                            Thread.Sleep(500);
                        }
                    }                
                    cmdStr = string.Format("CHS:OUT:POW {0}", vpp.ToString("0.000"));
                    MyClientSocketLAN.OnSendMsg(cmdStr, _1652BSocketID);//发指令
                    Thread.Sleep(500);//等待回读
                    //NRX功率计载波频率
                    if (i == 0)
                    {
                        _nrxFreq = centerFreq;
                        MyClientSocketLAN.SetNRXCarrierFreq(_nrxFreq, _nrxSocketID);
                    }
                    else
                    {
                        if (Math.Abs(centerFreq - _nrxFreq) > 1000000.0)
                        {
                            _nrxFreq = centerFreq;
                            MyClientSocketLAN.SetNRXCarrierFreq(_nrxFreq, _nrxSocketID);
                        }
                    }
                    Thread.Sleep(chgFreqSleepTime);
                    //读取功率值
                    MyClientSocketLAN.GetNRXPower1(out d, time_out, _nrxSocketID);
                    if( i == 0)
                    {
                        Thread.Sleep(2000);
                        MyClientSocketLAN.GetNRXPower1(out d, time_out, _nrxSocketID);
                    }
                    getPowerList.Add(d);
                    _worker.ReportProgress(100 * i / Count);//显示校准进度     
                }
                ParseData.ParseData.DeleteFile(saveFileName);
                using (FileStream fw = new FileStream(saveFileName, FileMode.Create))
                {
                    StreamWriter sw = new StreamWriter(fw);//写.csv或.txt文件
                    cmdStr = string.Format("CoupleType,Freq(MHz),SetValue(dBm),CH{0},MaxValue(dBm),MinValue(dBm)\r\n", (who + 1).ToString());
                    sw.Write(cmdStr);
                    //DC
                    sw.Write("DC\r\n");
                    for (i = 0; i < getPowerList.Count; i++)
                    {                    
                            GetMaxMin_1652B_Vpp(TywCommon.COUPLETYPE_DC, vppSlotList[i], out maxValue, out minValue);
                            maxValue = ConvertFunc.VppToDbm(maxValue);
                            minValue = ConvertFunc.VppToDbm(minValue);
                            cmdStr = string.Format(" ,{0}MHz,{1},{2},{3},{4}\r\n",  (freqSlotList[i] / 1000000.0).ToString("0.0"),
                                                                                     ConvertFunc.VppToDbm(vppSlotList[i]).ToString("0.00"),
                                                                                     getPowerList[i].ToString("0.00"),
                                                                                     maxValue.ToString("0.00"),
                                                                                     minValue.ToString("0.00")); ;
                            sw.Write(cmdStr);

                    }
                    sw.Close();
                    fw.Close();
                }
               
                //------------------： AC耦合
                //------------------： AC耦合
                freqSlotList.Clear();
                vppSlotList.Clear();
                getPowerList.Clear();
                srcFile = @"1652B_AC_Slot.txt";
                saveFileName = string.Format("D:\\AWG1652B_AC_CH{0}_dBm.csv", (who + 1).ToString());
                using (FileStream fr = new FileStream(srcFile, FileMode.Open))
                {
                    StreamReader sr = new StreamReader(fr);
                    while (!sr.EndOfStream)
                    {
                        cmdStr = sr.ReadLine();
                        tBuffer = cmdStr.Split(',');
                        if (tBuffer.Length == 2)
                        {
                            freqSlotList.Add(Convert.ToDouble(tBuffer[0]));//中心频率
                            vppSlotList.Add(Convert.ToDouble(tBuffer[1]));//峰峰值
                        }
                    }
                    sr.Close();
                    fr.Close();
                }
                MyClientSocketLAN.OnSendMsg("CHS:OUT:COUP AC", _1652BSocketID);
                MyClientSocketLAN.OnSendMsg("CHS:OUT:DCVOF 0.0", _1652BSocketID);
                Thread.Sleep(300);
                time_out = 400;
                for (i = 0; i < freqSlotList.Count; i++)
                {
                    centerFreq = freqSlotList[i];
                    vpp = vppSlotList[i];
                    //设置1652B频率和峰峰值
                    if (i == 0)
                    {
                        _awg1652_freq = centerFreq;
                        cmdStr = string.Format("DDFS:SIN:FREQ {0}", centerFreq.ToString("0.000"));
                        MyClientSocketLAN.OnSendMsg(cmdStr, _1652BSocketID);//信号频率
                        Thread.Sleep(500);
                    }
                    else
                    {
                        if (Math.Abs(centerFreq - _awg1652_freq) > 10.0)
                        {
                            _awg1652_freq = centerFreq;
                            cmdStr = string.Format("DDFS:SIN:FREQ {0}", centerFreq.ToString("0.000"));
                            MyClientSocketLAN.OnSendMsg(cmdStr, _1652BSocketID);//信号频率
                            Thread.Sleep(500);
                        }
                    }                
                    cmdStr = string.Format("CHS:OUT:POW {0}", vpp.ToString("0.000"));
                    MyClientSocketLAN.OnSendMsg(cmdStr, _1652BSocketID);//发指令
                    Thread.Sleep(500);//等待回读
                    //NRX功率计载波频率
                    if (i == 0)
                    {
                        _nrxFreq = centerFreq;
                        MyClientSocketLAN.SetNRXCarrierFreq(_nrxFreq, _nrxSocketID);
                    }
                    else
                    {
                        if (Math.Abs(centerFreq - _nrxFreq) > 1000000.0)
                        {
                            _nrxFreq = centerFreq;
                            MyClientSocketLAN.SetNRXCarrierFreq(_nrxFreq, _nrxSocketID);
                        }
                    }
                    Thread.Sleep(chgFreqSleepTime);
                    //读取功率值
                    MyClientSocketLAN.GetNRXPower1(out d, time_out, _nrxSocketID);
                    if (i == 0)
                    {
                        Thread.Sleep(2000);
                        MyClientSocketLAN.GetNRXPower1(out d, time_out, _nrxSocketID);
                    }
                    getPowerList.Add(d);
                    _worker.ReportProgress(100 * (i + 61)/ Count);//显示校准进度     
                }
                ParseData.ParseData.DeleteFile(saveFileName);
                using (FileStream fw = new FileStream(saveFileName, FileMode.Create))
                {
                    StreamWriter sw = new StreamWriter(fw);//写.csv或.txt文件
                    cmdStr = string.Format("CoupleType,Freq(MHz),SetValue(dBm),CH{0},MaxValue(dBm),MinValue(dBm)\r\n", (who + 1).ToString());
                    sw.Write(cmdStr);
                    //AC
                    sw.Write("AC\r\n");
                    for (i = 0; i < getPowerList.Count; i++)
                    {
                        GetMaxMin_1652B_Vpp(TywCommon.COUPLETYPE_DC, vppSlotList[i], out maxValue, out minValue);
                        maxValue = ConvertFunc.VppToDbm(maxValue);
                        minValue = ConvertFunc.VppToDbm(minValue);
                        cmdStr = string.Format(" ,{0}MHz,{1},{2},{3},{4}\r\n",  (freqSlotList[i] / 1000000.0).ToString("0.0"),
                                                                                 ConvertFunc.VppToDbm(vppSlotList[i]).ToString("0.00"),
                                                                                 getPowerList[i].ToString("0.00"),
                                                                                 maxValue.ToString("0.00"),
                                                                                 minValue.ToString("0.00")); ;
                        sw.Write(cmdStr);

                    }
                    sw.Close();
                    fw.Close();
                }
            }
            return;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="awg1652x">1652A or 1652B, 0表示1652A，1表示1652B</param>
        /// <param name="who">1652通道</param>
        private void GetVppByMXR604A(BackgroundWorker _worker, int awg1652x, int who)
        {
            int i;
            double maxValue = 10;
            double minValue = 0 - 50;
            double d, vpp = 0.05, centerFreq = 0.0;
            double _awg1652_freq = 0.0;
            string cmdStr = "";
            List<double> freqSlotList = new List<double>();
            List<double> vppSlotList = new List<double>();
            List<double> getPowerList = new List<double>();
            string srcFile = @"1652A_Slot.txt";
            string saveFileName;
            string[] tBuffer;
            int Count = 61 + 88;
            double yScale = 0.0;
            double xScale = 0.0;

            if (awg1652x == 0)
            {
                //发程控命令，初始化1652B                  
                cmdStr = string.Format("AWGC:RF:CH{0} ON", (who + 1).ToString());
                MyClientSocketLAN.OnSendMsg(cmdStr, _1652ASocketID);//射频通道开关
                Thread.Sleep(500);
                MyClientSocketLAN.OnSendMsg("AWGC:RUN ON", _1652ASocketID);//播放使能
                Thread.Sleep(500);            
                MyClientSocketLAN.OnSendMsg("AWGC:MODE DDFS", _1652ASocketID);//模式切换
                Thread.Sleep(500);
                cmdStr = string.Format("AWGC:CHA {0}", who.ToString());
                MyClientSocketLAN.OnSendMsg(cmdStr, _1652ASocketID);//通道切换
                Thread.Sleep(500);
                MyClientSocketLAN.OnSendMsg("DDFS:WAVE SIN", _1652ASocketID);//波形为正弦
                Thread.Sleep(500);
                MyClientSocketLAN.OnSendMsg("DDFS:SIN:FREQ 10MHz", _1652ASocketID);
                Thread.Sleep(500);
                MyClientSocketLAN.OnSendMsg("CHS:OUT:POW 0.5", _1652ASocketID);
                Thread.Sleep(500);
                //示波器初始化
                MyClientSocketLAN.RstMXR604A(_mxr604aSocketID);
                MyClientSocketLAN.OnSendMsg(":CHAN1:INP DC50", _mxr604aSocketID); //50欧姆阻抗
                Thread.Sleep(1000);
                MyClientSocketLAN.InitMXR604AOneChannel(1, _mxr604aSocketID);

                freqSlotList.Clear();
                vppSlotList.Clear();
                getPowerList.Clear();
                srcFile = @"1652A_Slot.txt";
                saveFileName = string.Format("D:\\AWG1652A_CH{0}_Vpp.csv", (who + 1).ToString());
                using (FileStream fr = new FileStream(srcFile, FileMode.Open))
                {
                    StreamReader sr = new StreamReader(fr);
                    while (!sr.EndOfStream)
                    {
                        cmdStr = sr.ReadLine();
                        tBuffer = cmdStr.Split(',');
                        if (tBuffer.Length == 2)
                        {
                            freqSlotList.Add(Convert.ToDouble(tBuffer[0]));//中心频率
                            vppSlotList.Add(Convert.ToDouble(tBuffer[1]));//峰峰值
                        }
                    }
                    sr.Close();
                    fr.Close();
                }
                MyClientSocketLAN.OnSendMsg("CHS:OUT:DCVOF 0.0", _1652ASocketID);
                cmdStr = string.Format("AWGC:CHA {0}", who.ToString());
                MyClientSocketLAN.OnSendMsg(cmdStr, _1652ASocketID);//通道切换
                Thread.Sleep(500);
                MyClientSocketLAN.OnSendMsg("CHS:OUT:DCVOF 0.0", _1652ASocketID);
                Thread.Sleep(300);
                Count = freqSlotList.Count;
                for (i = 0; i < freqSlotList.Count; i++)
                {
                    centerFreq = freqSlotList[i];
                    vpp = vppSlotList[i];
                    //调整示波器
                    xScale = (1.0 / centerFreq) / 2.5;
                    MyClientSocketLAN.SetMXR604ATime(xScale, _mxr604aSocketID);//调整时基 
                    Thread.Sleep(500);
                    if (i == 0)  yScale = vppSlotList[i] / 2.0;
                    else         yScale = vppSlotList[i] / 6.0;
                    MyClientSocketLAN.SetMXR604AYScale(yScale, 1, _mxr604aSocketID);//调整垂直分辨率
                    Thread.Sleep(1000);//等待回读                                       
                    //设置1652A频率和峰峰值
                    if (i == 0)
                    {
                        _awg1652_freq = centerFreq;
                        cmdStr = string.Format("DDFS:SIN:FREQ {0}", centerFreq.ToString("0.000"));
                        MyClientSocketLAN.OnSendMsg(cmdStr, _1652ASocketID);//信号频率
                        Thread.Sleep(500);
                    }
                    else
                    {
                        if (Math.Abs(centerFreq - _awg1652_freq) > 10.0)
                        {
                            _awg1652_freq = centerFreq;
                            cmdStr = string.Format("DDFS:SIN:FREQ {0}", centerFreq.ToString("0.000"));
                            MyClientSocketLAN.OnSendMsg(cmdStr, _1652ASocketID);//信号频率
                            Thread.Sleep(500);
                        }
                    }
                    cmdStr = string.Format("CHS:OUT:POW {0}", vpp.ToString("0.000"));
                    MyClientSocketLAN.OnSendMsg(cmdStr, _1652ASocketID);//发指令
                    Thread.Sleep(500);//等待回读                
                    MyClientSocketLAN.GetMXR604AVpp(out d, 1, _mxr604aSocketID);
                    getPowerList.Add(d);
                    _worker.ReportProgress(100 * i / Count);//显示校准进度        
                }
                ParseData.ParseData.DeleteFile(saveFileName);
                using (FileStream fw = new FileStream(saveFileName, FileMode.Create))
                {
                    StreamWriter sw = new StreamWriter(fw);//写.csv或.txt文件
                    cmdStr = string.Format("Freq(MHz),SetValue(Vpp),CH{0},MaxValue(Vpp),MinValue(Vpp)\r\n", (who + 1).ToString());
                    sw.Write(cmdStr);
                    for (i = 0; i < getPowerList.Count; i++)
                    {
                        GetMaxMin_1652A_Vpp(vppSlotList[i], out maxValue, out minValue);
                        cmdStr = string.Format("{0}MHz,{1},{2},{3},{4}\r\n",   (freqSlotList[i] / 1000000.0).ToString("0.0"),
                                                                                 vppSlotList[i].ToString("0.000"),
                                                                                  getPowerList[i].ToString("0.000"),
                                                                                  maxValue.ToString("0.000"),
                                                                                  minValue.ToString("0.000")); ;
                        sw.Write(cmdStr);

                    }
                    sw.Close();
                    fw.Close();
                }


            }
            if(awg1652x == 1)
            {
                //发程控命令，初始化1652B                  
                cmdStr = string.Format("AWGC:RF:CH{0} ON", (who + 1).ToString());
                MyClientSocketLAN.OnSendMsg(cmdStr, _1652BSocketID);//射频通道开关
                Thread.Sleep(500);
                MyClientSocketLAN.OnSendMsg("AWGC:RUN ON", _1652BSocketID);//播放使能
                Thread.Sleep(500);        
                MyClientSocketLAN.OnSendMsg("AWGC:MODE DDFS", _1652BSocketID);//模式切换
                Thread.Sleep(500);
                cmdStr = string.Format("AWGC:CHA {0}", who.ToString());
                MyClientSocketLAN.OnSendMsg(cmdStr, _1652BSocketID);//通道切换
                Thread.Sleep(500);
                MyClientSocketLAN.OnSendMsg("DDFS:WAVE SIN", _1652BSocketID);//波形为正弦
                Thread.Sleep(500);
                MyClientSocketLAN.OnSendMsg("DDFS:SIN:FREQ 10MHz", _1652BSocketID);
                Thread.Sleep(500);
                MyClientSocketLAN.OnSendMsg("CHS:OUT:POW 0.5", _1652BSocketID);
                Thread.Sleep(500);
                //示波器初始化
                MyClientSocketLAN.RstMXR604A(_mxr604aSocketID);
                MyClientSocketLAN.OnSendMsg(":CHAN1:INP DC50", _mxr604aSocketID); //50欧姆阻抗
                Thread.Sleep(1000);
                MyClientSocketLAN.InitMXR604AOneChannel(1, _mxr604aSocketID);

                //------------------： DC耦合
                //------------------： DC耦合    
                freqSlotList.Clear();
                vppSlotList.Clear();
                getPowerList.Clear();
                srcFile = @"1652B_DC_Slot.txt";
                saveFileName = string.Format("D:\\AWG1652B_DC_CH{0}_Vpp.csv", (who + 1).ToString());
                using (FileStream fr = new FileStream(srcFile, FileMode.Open))
                {
                    StreamReader sr = new StreamReader(fr);
                    while (!sr.EndOfStream)
                    {
                        cmdStr = sr.ReadLine();
                        tBuffer = cmdStr.Split(',');
                        if (tBuffer.Length == 2)
                        {
                            freqSlotList.Add(Convert.ToDouble(tBuffer[0]));//中心频率
                            vppSlotList.Add(Convert.ToDouble(tBuffer[1]));//峰峰值
                        }
                    }
                    sr.Close();
                    fr.Close();
                }
                MyClientSocketLAN.OnSendMsg("CHS:OUT:COUP DC", _1652BSocketID);
                Thread.Sleep(300);
                cmdStr = string.Format("AWGC:CHA {0}", who.ToString());
                MyClientSocketLAN.OnSendMsg(cmdStr, _1652BSocketID);//通道切换
                Thread.Sleep(500);
                MyClientSocketLAN.OnSendMsg("CHS:OUT:COUP DC", _1652BSocketID);
                Thread.Sleep(300);
                MyClientSocketLAN.OnSendMsg("CHS:OUT:DCVOF 0.0", _1652BSocketID);
                Thread.Sleep(300);
                for (i = 0; i < freqSlotList.Count; i++)
                {
                    centerFreq = freqSlotList[i];
                    vpp = vppSlotList[i];
                    //调整示波器
                    xScale = (1.0 / centerFreq) / 2.5;
                    MyClientSocketLAN.SetMXR604ATime(xScale, _mxr604aSocketID);//调整时基 
                    Thread.Sleep(500);
                    if (i == 0) yScale = vppSlotList[i] / 4.0;
                    else yScale = vppSlotList[i] / 6.0;
                    MyClientSocketLAN.SetMXR604AYScale(yScale, 1, _mxr604aSocketID);//调整垂直分辨率
                    Thread.Sleep(1000);//等待回读                
                    //设置1652B频率和峰峰值
                    if (i == 0)
                    {
                        _awg1652_freq = centerFreq;
                        cmdStr = string.Format("DDFS:SIN:FREQ {0}", centerFreq.ToString("0.000"));
                        MyClientSocketLAN.OnSendMsg(cmdStr, _1652BSocketID);//信号频率
                        Thread.Sleep(500);
                    }
                    else
                    {
                        if (Math.Abs(centerFreq - _awg1652_freq) > 10.0)
                        {
                            _awg1652_freq = centerFreq;
                            cmdStr = string.Format("DDFS:SIN:FREQ {0}", centerFreq.ToString("0.000"));
                            MyClientSocketLAN.OnSendMsg(cmdStr, _1652BSocketID);//信号频率
                            Thread.Sleep(500);
                        }
                    }
                    cmdStr = string.Format("CHS:OUT:POW {0}", vpp.ToString("0.000"));
                    MyClientSocketLAN.OnSendMsg(cmdStr, _1652BSocketID);//发指令
                    if( i == 0) Thread.Sleep(3000);//等待回读
                    else        Thread.Sleep(1000);//等待回读
                    MyClientSocketLAN.GetMXR604AVpp(out d, 1, _mxr604aSocketID);
                    getPowerList.Add(d);
                    _worker.ReportProgress(100 * i / Count);//显示校准进度        
                }
                ParseData.ParseData.DeleteFile(saveFileName);
                using (FileStream fw = new FileStream(saveFileName, FileMode.Create))
                {
                    StreamWriter sw = new StreamWriter(fw);//写.csv或.txt文件
                    cmdStr = string.Format("CoupleType,Freq(MHz),SetValue(Vpp),CH{0},MaxValue(Vpp),MinValue(Vpp)\r\n", (who + 1).ToString());
                    sw.Write(cmdStr);
                    //DC
                    sw.Write("DC\r\n");
                    for (i = 0; i < getPowerList.Count; i++)
                    {
                        GetMaxMin_1652B_Vpp(TywCommon.COUPLETYPE_DC, vppSlotList[i], out maxValue, out minValue);                       
                        cmdStr = string.Format(" ,{0}MHz,{1},{2},{3},{4}\r\n", (freqSlotList[i] / 1000000.0).ToString("0.0"),
                                                                                vppSlotList[i].ToString("0.000"),
                                                                                getPowerList[i].ToString("0.000"),
                                                                                maxValue.ToString("0.000"),
                                                                                minValue.ToString("0.000")); ;
                        sw.Write(cmdStr);

                    }
                    sw.Close();
                    fw.Close();
                }
                //------------------： AC耦合
                //------------------： AC耦合    
                freqSlotList.Clear();
                vppSlotList.Clear();
                getPowerList.Clear();
                srcFile = @"1652B_AC_Slot.txt";
                saveFileName = string.Format("D:\\AWG1652B_AC_CH{0}_Vpp.csv", (who + 1).ToString());
                using (FileStream fr = new FileStream(srcFile, FileMode.Open))
                {
                    StreamReader sr = new StreamReader(fr);
                    while (!sr.EndOfStream)
                    {
                        cmdStr = sr.ReadLine();
                        tBuffer = cmdStr.Split(',');
                        if (tBuffer.Length == 2)
                        {
                            freqSlotList.Add(Convert.ToDouble(tBuffer[0]));//中心频率
                            vppSlotList.Add(Convert.ToDouble(tBuffer[1]));//峰峰值
                        }
                    }
                    sr.Close();
                    fr.Close();
                }
                MyClientSocketLAN.OnSendMsg("CHS:OUT:COUP AC", _1652BSocketID);
                MyClientSocketLAN.OnSendMsg("CHS:OUT:DCVOF 0.0", _1652BSocketID);
                Thread.Sleep(300);
                for (i = 0; i < freqSlotList.Count; i++)
                {
                    centerFreq = freqSlotList[i];
                    vpp = vppSlotList[i];
                    //调整示波器
                    xScale = (1.0 / centerFreq) / 2.5;
                    MyClientSocketLAN.SetMXR604ATime(xScale, _mxr604aSocketID);//调整时基 
                    Thread.Sleep(500);
                    if (i == 0) yScale = vppSlotList[i] / 4.0;
                    else yScale = vppSlotList[i] / 6.0;
                    MyClientSocketLAN.SetMXR604AYScale(yScale, 1, _mxr604aSocketID);//调整垂直分辨率
                    Thread.Sleep(1000);//等待回读
                   
                    //设置1652B频率和峰峰值
                    if (i == 0)
                    {
                        _awg1652_freq = centerFreq;
                        cmdStr = string.Format("DDFS:SIN:FREQ {0}", centerFreq.ToString("0.000"));
                        MyClientSocketLAN.OnSendMsg(cmdStr, _1652BSocketID);//信号频率
                        Thread.Sleep(500);
                    }
                    else
                    {
                        if (Math.Abs(centerFreq - _awg1652_freq) > 10.0)
                        {
                            _awg1652_freq = centerFreq;
                            cmdStr = string.Format("DDFS:SIN:FREQ {0}", centerFreq.ToString("0.000"));
                            MyClientSocketLAN.OnSendMsg(cmdStr, _1652BSocketID);//信号频率
                            Thread.Sleep(500);
                        }
                    }
                    cmdStr = string.Format("CHS:OUT:POW {0}", vpp.ToString("0.000"));
                    MyClientSocketLAN.OnSendMsg(cmdStr, _1652BSocketID);//发指令
                    if (i == 0) Thread.Sleep(3000);//等待回读
                    else Thread.Sleep(1000);//等待回读
                    MyClientSocketLAN.GetMXR604AVpp(out d, 1, _mxr604aSocketID);
                    getPowerList.Add(d);
                    _worker.ReportProgress(100 * (i+61) / Count);//显示校准进度        
                }
                ParseData.ParseData.DeleteFile(saveFileName);
                using (FileStream fw = new FileStream(saveFileName, FileMode.Create))
                {
                    StreamWriter sw = new StreamWriter(fw);//写.csv或.txt文件
                    cmdStr = string.Format("CoupleType,Freq(MHz),SetValue(Vpp),CH{0},MaxValue(Vpp),MinValue(Vpp)\r\n", (who + 1).ToString());
                    sw.Write(cmdStr);
                    //AC
                    sw.Write("AC\r\n");
                    for (i = 0; i < getPowerList.Count; i++)
                    {
                        GetMaxMin_1652B_Vpp(TywCommon.COUPLETYPE_DC, vppSlotList[i], out maxValue, out minValue);                    
                        cmdStr = string.Format(" ,{0}MHz,{1},{2},{3},{4}\r\n", (freqSlotList[i] / 1000000.0).ToString("0.0"),
                                                                                vppSlotList[i].ToString("0.000"),
                                                                                getPowerList[i].ToString("0.000"),
                                                                                 maxValue.ToString("0.000"),
                                                                                 minValue.ToString("0.000")); ;
                        sw.Write(cmdStr);

                    }
                    sw.Close();
                    fw.Close();
                }
            }
            return;
        }

         /// <summary>
         /// DC-2GHz幅度测试子任务
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
         private void backgroundWorkerPowerCali_DoWork(object sender, DoWorkEventArgs e)
         {
            int awg1652x = 0;
            int who = (int)ChannelComoBox;
            bool []IsConnect = new bool [10];           
            BackgroundWorker _worker = sender as BackgroundWorker;
            if(Check_Awg1652A ==  true)
            {
                awg1652x = 0;
            }
            if (Check_Awg1652B == true)
            {
                awg1652x = 1;
            }      
            if (_worker.CancellationPending == true)
            {
                e.Cancel = true;
            }
            else
            {           
                if (Check_NRX == true)
                {
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        //LAN连接建立:
                        if(Check_Awg1652A == true)
                        {
                            IsConnect[_1652ASocketID] = MyClientSocketLAN.OnConnectSSocket(Awg1652A_Ip.Text, "4000", _1652ASocketID);
                        }
                        else if(Check_Awg1652B == true)
                        {
                            IsConnect[_1652BSocketID] = MyClientSocketLAN.OnConnectSSocket(Awg1652B_Ip.Text, "4000", _1652BSocketID);
                        }                     
                        IsConnect[_nrxSocketID] = MyClientSocketLAN.OnConnectSSocket  (NRX_Ip.Text, NRX_Port.Text, _nrxSocketID);
                    }));
                    if (Check_Awg1652A == true)
                    {
                        if (IsConnect[_1652ASocketID] == false)
                        {
                            MessageBox.Show("1652A连接失败!");
                            return;
                        }
                    }
                    else if (Check_Awg1652B == true)
                    {
                        if (IsConnect[_1652BSocketID] == false)
                        {
                            MessageBox.Show("1652B连接失败!");
                            return;
                        }
                    }                   
                    if (IsConnect[_nrxSocketID] == false )
                    {
                        MessageBox.Show("NRX功率计仪器连接失败!");
                        return;
                    }
                    SetShowMsg("NRX读取功率中...", 1);
                    GetVppByNRX(_worker, awg1652x, who);
                }

                if (Check_MXR604A == true)
                {
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        //LAN连接建立:
                        if (Check_Awg1652A == true)
                        {
                            IsConnect[_1652ASocketID] = MyClientSocketLAN.OnConnectSSocket(Awg1652A_Ip.Text, "4000", _1652ASocketID);
                        }
                        else if (Check_Awg1652B == true)
                        {
                            IsConnect[_1652BSocketID] = MyClientSocketLAN.OnConnectSSocket(Awg1652B_Ip.Text, "4000", _1652BSocketID);
                        }
                        IsConnect[_mxr604aSocketID] = MyClientSocketLAN.OnConnectSSocket(MXR604A_Ip.Text, MXR604A_Port.Text, _mxr604aSocketID);
                    }));
                    if (Check_Awg1652A == true)
                    {
                        if (IsConnect[_1652ASocketID] == false)
                        {
                            MessageBox.Show("1652A连接失败!");
                            return;
                        }
                    }
                    else if (Check_Awg1652B == true)
                    {
                        if (IsConnect[_1652BSocketID] == false)
                        {
                            MessageBox.Show("1652B连接失败!");
                            return;
                        }
                    }
                    if (IsConnect[_mxr604aSocketID] == false)
                    {
                        MessageBox.Show("MXR604A示波器连接失败!");
                        return;
                    }
                    SetShowMsg("MXR604A读取中...", 1);
                    GetVppByMXR604A(_worker, awg1652x, who);
                }

            }
            return;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorkerPowerCali_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Progress.Instance().ProgressValue = e.ProgressPercentage;
            SetPercentValue((double)e.ProgressPercentage);//界面显示校准进度
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorkerPowerCali_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                SetShowMsg("Cancel!", 0);//取消!
            }
            else if (e.Error != null)
            {
                SetShowMsg("Error:" + e.Error.Message, 0);//错误:
            }
            else
            {
                SetShowMsg("");
            }
            // Close the AlertForm
            SetPercentValue(0);
            return;
        }

        /// 设置执行百分比
        /// </summary>
        /// <param name="percentV"></param>
        public void SetPercentValue(double percentV)
        {
            this.Dispatcher.BeginInvoke(new Action(() => {
                TXT_percent.Value = percentV;
            }));
            return;
        }

        /// <summary>
        /// 设置显示提示
        /// </summary>
        /// <param name="showMsg"></param>
        public void SetShowMsg(string showMsg, int _color = 0)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                labelResult.Content = showMsg;
                if (_color == 0)
                {
                    labelResult.Foreground = new SolidColorBrush(Colors.Red);
                }
                else if (_color == 1)
                {
                    labelResult.Foreground = new SolidColorBrush(Colors.Green);
                }
            }));
            return;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Start_Click(object sender, RoutedEventArgs e)
        {
            //携带信息的事件
            var send = new object[4];
            send[0] = 10.5;
            send[1] = 'A';
            send[2] = 32;
            send[3] = "test string";

           
          
            CreateBackgroundWorkerPowerCali();
            backgroundWorkerPowerCali.RunWorkerAsync(send);
        

            return;
        }

        public event PropertyChangedEventHandler PropertyChanged;


        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        
    }
}
