///////////////////////////////////////////////////////////////////////////////////////
// RELAY COMMAND STYLE
// Default CMD : 05 5A 03 00 01 00 10
// RELAY CMD : 05 5A 0B 00 01 0D 05 06 05 5A 03 00 01 00 10
///////////////////////////////////////////////////////////////////////////////////////


// definitions
#define xHDX2910
#define xHDX2914
#define xHDX2918
#define xENABLE_CTRL_NFC // wsjung.add.170921 : nfc polling before start
#define xNFC_LOCK // wsjung.add.171011 : nfc lock function
#define HDX2936
#define xATH_CKS30TW
#define ATH_CKS50TW2
#define ENABLE_WIRELESS_TEST
#define repeat_testx

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using System.Media;
using FTD2XX_NET; // wsjung.add.170517 : FT234XD gpio test
using USBInterface; // wsjung.add.180110 : usbhid control

namespace threadTest01
{
    public partial class Form1 : Form
    {
        public const byte USB_HOST_DATA_CMD_START_CONNECTION = 0x00;
        public const byte USB_HOST_DATA_CMD_DISCONNECT = 0x01;
        public const byte USB_HOST_DATA_CMD_GET_REMOTE_NAME = 0x02;
        public const byte USB_HOST_DATA_CMD_GET_REMOTE_BD_ADDRESS = 0x03;
        public const byte USB_HOST_DATA_CMD_GET_REMOTE_VERSION = 0x04;
        public const byte USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD = 0x05;
        public const byte USB_HOST_DATA_CMD_SEND_REALTEK_SPP_CMD = 0x06;

        public const byte USB_DEVICE_DATA_RSP_HS_CONNECTED = 0x00;
        public const byte USB_DEVICE_DATA_RSP_HS_CONNECTING_ATTEMPT = 0x01;
        public const byte USB_DEVICE_DATA_RSP_HS_REMOTE_BD_ADDR = 0x02;
        public const byte USB_DEVICE_DATA_RSP_HS_REMOTE_NAME = 0x03;
        public const byte USB_DEVICE_DATA_RSP_HS_VERSION_INFO = 0x04;
        public const byte USB_DEVICE_DATA_RSP_HS_SPP_RX = 0x05;

#if true
        public const byte RACE_CHANNEL = 0x05;
        public const byte RACE_TYPE_RESP = 0x5B;
        public const byte RACE_TYPE_NOTI = 0x5D;
        public const int RACE_ID_CMDRELAY_AVA_DST = 0x0D00;
        public const int RACE_ID_CMDRELAY_PASS_TO_DST = 0x0D01;
        public const int RACE_ID_READ_FULLKEY = 0x0A00;
        public const int RACE_ID_READ_BATTERY = 0x0CD6;
        public const int RACE_ID_READ_AE_INFO = 0x1C09;

        public static bool needProcessMoreData = false;
        public static bool needProcessDstData = false;
        public static byte destination_id = 0x00;

        public static bool requestAgentBattery = false;
        public static bool requestPartnerBattery = false;
        public static bool requestAgentAEInfo = false;
        public static bool requestPartnerAEInfo = false;
#endif

#if (ENABLE_WIRELESS_TEST)
        public static bool HeadsetConnected = false; // wsjung.add.180110
        public static bool CheckModelName = false;
        public static bool CheckModelNamePri = false;
        public static bool CheckModelNameSec = false;
        public static bool CheckDefaultBDPri = false;
        public static bool CheckDefaultBDSec = false;
        public static bool CheckBdAddress = false;
        public static bool CheckBdAddressPri = false;
        public static bool CheckBdAddressSec = false;
        public static bool CheckBdRange = false;
        public static bool CheckFwVersion = false;
        public static bool CheckFwVersionPri = false;
        public static bool CheckFwVersionSec = false;
        public static bool CheckAncWritePri = false;
        public static bool CheckAncWriteSec = false;
        public static bool CheckFwRegion = false;
        public static bool WriteFwColorPri = false;
        public static bool WriteFwColorSec = false;
        public static bool CheckFwColorPri = false;
        public static bool CheckFwColorSec = false;
        public static bool CheckFactoryReset = false;
        public static bool CheckShippingPoweroffPri = false;
        public static bool CheckShippingPoweroffSec = false;
        public static bool CheckAvaDest = false;
        public static bool CheckBatLevel = false; // yspark.add.220223

        public static bool flag_CheckDefaultBDPri = false;
        public static bool flag_CheckDefaultBDSec = false;


        static public int AncWriteCheckDataPri;
        static public int AncWriteCheckDataSec;
        static public int autotest_count = 0;

        static public byte dutBd01;
        static public byte dutBd02;
        static public byte dutBd03;
        static public byte dutBd04;
        static public byte dutBd05;
        static public byte dutBd06;

        static public byte[] dutModelNameByte = new Byte[30];
        static public byte[] dutModelNamePriByte = new Byte[30];
        static public byte[] dutModelNameSecByte = new Byte[30];

        /* Default */
        static public string dutBdNap;
        static public string dutBdUap;
        static public string dutBdLap;
#if ATH_CKS30TW || ATH_CKS50TW2

        static public byte dutBd01Pri;
        static public byte dutBd02Pri;
        static public byte dutBd03Pri;
        static public byte dutBd04Pri;
        static public byte dutBd05Pri;
        static public byte dutBd06Pri;

        static public byte dutBd01Sec;
        static public byte dutBd02Sec;
        static public byte dutBd03Sec;
        static public byte dutBd04Sec;
        static public byte dutBd05Sec;
        static public byte dutBd06Sec;

        static public string dutBdNapPri;
        static public string dutBdUapPri;
        static public string dutBdLapPri;
        static public string dutBdNapSec;
        static public string dutBdUapSec;
        static public string dutBdLapSec;

        static public string dutFullBdAddressPri;
        static public string dutFullBdAddressSec;

        static public byte batLevelPri;
        static public byte batLevelSec;
#endif

        static public string dutFullBdAddress;
        static public string dutModelName;
        static public string dutModelNamePri;
        static public string dutModelNameSec;
        static public string dutVersionMajor;
        static public string dutVersionMinor;
        static public string dutVersionRevision;
        static public string dutFullVersion;
        static public string dutFullVersionPri;
        static public string dutFullVersionSec;
        static public string dutFwColorPri;
        static public string dutFwColorSec;

        static public byte dutRegion;
        static public byte dutFactoryReset;

        public static DeviceScanner scanner = null;
        public static USBDevice dev = null;

        public static System.Timers.Timer timer = null;
#endif
        static public string gDutName;
        static public string gSwVersion;
        static public string gManufactureDate;

        static public string gDefaultBd;
        static public string gDefaultBdNap;
        static public string gDefaultBdUap;
        static public string gDefaultBdLap;

        static public string gBdNap;
        static public string gBdUap;
        static public string gBdLapStart;
        static public string gBdLapEnd;

#if false
        static public string gBdLapWriteNext;
#endif

        static public string gLineInfoA;
        static public string gLineInfoB;

        static public string gFwRegion; // wsjung.add.170405
        static public string gFwColor; // yspark.add.240206 : using color check


#if false
        public string comPort;
#endif

        static public string gAppName;

#if false
        static public string gCheckOnly; // wsjung.add.170510
#endif

        static public string gSoundNg1 = System.IO.Directory.GetCurrentDirectory() + "\\" + "sound\\" + "ng_merge.wav";
        //static public string gSoundNg2 = System.IO.Directory.GetCurrentDirectory() + "\\" + "sound\\" + "ng2.wav";

        public string pathLogFile;
        public string pathDupeLogFile;

        public UInt32 totalCount = 0;
        public UInt32 passCount = 0;
        public UInt32 failCount = 0;

        public string[] ngDesc = new string[20];
        static public string typeNg = "";

        static public int gCountSequence = 0;
        public string[] gtestSequence = new string[20];
        public string[] gTestValue = new string[20];
        public string[] gNgType = new string[20];

        static public int gLatestDupeRow = 0;

        string pathConfigFile;

        public Form1()
        {
            InitializeComponent();
        }

        private void initDataGridView()
        {
            dataGridView1.ColumnCount = 5;
            dataGridView1.ColumnHeadersVisible = true;

            DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();

            columnHeaderStyle.BackColor = Color.Blue;
            columnHeaderStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle = columnHeaderStyle;

            dataGridView1.Columns[0].Width = 50;
            dataGridView1.Columns[1].Width = 300;
            dataGridView1.Columns[2].Width = 200;
            dataGridView1.Columns[3].Width = 200;
            dataGridView1.Columns[4].Width = 200;

            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[0].Name = "No.";
            dataGridView1.Columns[1].Name = "Item";
            dataGridView1.Columns[2].Name = "Config";
            dataGridView1.Columns[3].Name = "Value";
            dataGridView1.Columns[4].Name = "Result";
        }

        private void initDataGridView2()
        {
            dataGridView2.ColumnCount = 4;
            dataGridView2.ColumnHeadersVisible = true;

            DataGridViewCellStyle columnHeaderStyle2 = new DataGridViewCellStyle();

            columnHeaderStyle2.BackColor = Color.Blue;
            columnHeaderStyle2.Font = new Font("Arial", 8, FontStyle.Bold);
            dataGridView2.ColumnHeadersDefaultCellStyle = columnHeaderStyle2;

            dataGridView2.Columns[0].Width = 50;
            dataGridView2.Columns[1].Width = 250;
            dataGridView2.Columns[2].Width = 130;
            dataGridView2.Columns[3].Width = 130;

            dataGridView2.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView2.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridView2.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView2.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[0].Name = "No.";
            dataGridView2.Columns[1].Name = "BD";
            dataGridView2.Columns[2].Name = "Result";
            dataGridView2.Columns[3].Name = "Dupe";
        }

        private void saveDupeData()
        {

        }

        private void setRegenOn()
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;

            // Create new instance of the FTDI device class
            FTDI myFtdiDevice = new FTDI();

            ftStatus = myFtdiDevice.OpenByIndex(0);

            ftStatus = myFtdiDevice.SetBitMode(0xF1, 0x20);
            Thread.Sleep(200);

            ftStatus = myFtdiDevice.SetBitMode(0x00, 0x00);

            ftStatus = myFtdiDevice.Close();
        }

        private void setRegenOff()
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;

            // Create new instance of the FTDI device class
            FTDI myFtdiDevice = new FTDI();

            ftStatus = myFtdiDevice.OpenByIndex(0);

            ftStatus = myFtdiDevice.SetBitMode(0xF0, 0x20);
            Thread.Sleep(200);

            ftStatus = myFtdiDevice.SetBitMode(0x00, 0x00);

            ftStatus = myFtdiDevice.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            initDataGridView();
            initDataGridView2();

            loadConfig();

            // set mainform name
            this.Text = gDutName + " WIRELESS COLOR WRITE" + " V" + this.ProductVersion;
           
            checkLogFile();
            checkDupeLogFile();

            // update count
            tb_totalCount.Text = totalCount.ToString();
            tb_passCount.Text = passCount.ToString();

            failCount = totalCount - passCount;
            tb_failCount.Text = (totalCount - passCount).ToString();

            // show region
            lbRegion.Text = gFwColor;
            if (gFwColor == "BK")
            {
                lbRegion.BackColor = Color.Black;
                lbRegion.ForeColor = Color.White;
            }
            else if (gFwColor == "GR")
            {
                lbRegion.BackColor = Color.DarkGreen;
                lbRegion.ForeColor = Color.White;
            }
            else if (gFwColor == "BG")
            {
                lbRegion.BackColor = Color.Beige;
                lbRegion.ForeColor = Color.Black;
            }
                

            btnStart.Focus();

#if false
            if(gCheckOnly == "YES")
            {
                setRegenOn(); // wsjung.add.170619 : test
            }
#endif

#if (ENABLE_WIRELESS_TEST)
            if (!initUsbHid())
            {
                MessageBox.Show("Cannot load USB Audio Dongle");
                Close();
            }
#endif
            
        }

        private void clearDisp()
        {
            for (int row = 0; row < Form1.gCountSequence; row++)
            {
                dataGridView1.Rows[row].Cells[3].Value = "-";
                dataGridView1.Rows[row].Cells[4].Value = "-";
                dataGridView1.Rows[row].DefaultCellStyle.BackColor = Color.Empty;
            }

            {
                dataGridView2.Rows[gLatestDupeRow].DefaultCellStyle.BackColor = Color.Empty;
            }

            lbResult.Text = "--";
            lbResult.ForeColor = Color.Empty;
            lbResult.BackColor = Color.Empty;
        }

        private void clearFlag()
        {
            HeadsetConnected = false; // wsjung.add.180110
            CheckModelName = false;
            CheckModelNamePri = false;
            CheckModelNameSec = false;
            CheckDefaultBDPri = false;
            CheckDefaultBDSec = false;
            CheckBdAddress = false;
            CheckBdAddressPri = false;
            CheckBdAddressSec = false;
            CheckBdRange = false;
            CheckFwVersion = false;
            CheckFwVersionPri = false;
            CheckFwVersionSec = false;
            CheckAncWritePri = false;
            CheckAncWriteSec = false;
            CheckShippingPoweroffPri = false;
            CheckShippingPoweroffSec = false;
            CheckFwRegion = false;
            WriteFwColorPri = false;
            WriteFwColorSec = false;
            CheckFwColorPri = false;
            CheckFwColorSec = false;
            CheckFactoryReset = false;
            CheckAvaDest = false;
        }

        private void NgSound()
        {
            SoundPlayer sound1 = new SoundPlayer(gSoundNg1);
            //SoundPlayer sound2 = new SoundPlayer(gSoundNg2);

            sound1.Play();
            //sound2.PlaySync();
        }

        public void btnStart_Click(object sender, EventArgs e)
        {
            clearDisp();
            clearFlag();

            backWork2 bg = new backWork2(this);
            Thread workerThread = new Thread(bg.DoWork);
            workerThread.Start();
        }
        public void auto_test()
        {
            Thread.Sleep(2500);            

            btnStart.Invoke(new MethodInvoker(delegate ()
            {
                btnStart.PerformClick();
            }));
           
        }

#if (ENABLE_WIRELESS_TEST)
        public bool initUsbHid()
        {
            int flag_ng = 0;

            // setup a scanner before hand
            scanner = new DeviceScanner(0x0a12, 0x1004);
            scanner.DeviceArrived += enter;
            scanner.DeviceRemoved += exit;
            scanner.StartAsyncScan();
            Console.WriteLine("asd");
            scanner.StopAsyncScan();

            try
            {
                // this can all happen inside a using(...) statement
                dev = new USBDevice(0x0a12, 0x1004, 0x0001, 0xFFA0, null, false, 101);

                //Console.WriteLine(dev.Description());

                dev.SetNonBlock();

                // add handle for data read
                dev.InputReportArrivedEvent += handle;
                // after adding the handle start reading
                dev.StartAsyncRead();
            }
            catch
            {
                flag_ng = 1;
            }

            if (flag_ng == 1) { return false; }
            else { return true; }
        }

/* 200406.wsjung.test
USB_HOST_COMMAND_USB_CRESYN_SPECIFIC => 11 (b)

USB HOST to DONGLE
USB_HOST_DATA_CMD_START_CONNECTION, 0
USB_HOST_DATA_CMD_DISCONNECT, 1
USB_HOST_DATA_CMD_GET_REMOTE_NAME, 2
USB_HOST_DATA_CMD_GET_REMOTE_BD_ADDRESS, 3
USB_HOST_DATA_CMD_GET_REMOTE_VERSION, 4
USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD 5

DONGLE to USB HOST
USB_DEVICE_DATA_RSP_HS_CONNECTED, 0
USB_DEVICE_DATA_RSP_HS_CONNECTING_ATTEMPT, 1
USB_DEVICE_DATA_RSP_HS_REMOTE_BD_ADDR, 2
USB_DEVICE_DATA_RSP_HS_REMOTE_NAME, 3
USB_DEVICE_DATA_RSP_HS_VERSION_INFO 4
*/

        public static void handle(object s, USBInterface.ReportEventArgs a)
        {
            if (a.Data[0] == 0x02 && a.Data[1] == 0x0b && a.Data[2] == USB_DEVICE_DATA_RSP_HS_CONNECTED) // check connected
            {
                if (!HeadsetConnected)
                    HeadsetConnected = true;
            }
#if false
            else if (a.Data[0] == 0x02 && a.Data[1] == 0x0b && a.Data[2] == 0x03 && a.Data[3] == 0x01) // check disconnected
            {
                if (HeadsetConnected)
                    HeadsetConnected = false;
            }
#endif
            else if (a.Data[0] == 0x02 && a.Data[1] == 0x0b && a.Data[2] == USB_DEVICE_DATA_RSP_HS_REMOTE_BD_ADDR) // check remote bd address            
            {
#if ATH_CKS30TW || ATH_CKS50TW2
                if (!CheckBdAddress)
                {
                    dutBd06 = a.Data[3];
                    dutBd05 = a.Data[4];
                    dutBd04 = a.Data[5];
                    dutBd03 = a.Data[6];
                    dutBd02 = a.Data[7];
                    dutBd01 = a.Data[8];

                    dutBdNap = dutBd01.ToString("X2") + dutBd02.ToString("X2");
                    dutBdUap = dutBd03.ToString("X2");
                    dutBdLap = dutBd04.ToString("X2") + dutBd05.ToString("X2") + dutBd06.ToString("X2");

                    dutFullBdAddress = dutBdNap + dutBdUap + dutBdLap;

                    CheckBdAddress = true;
                }
#else
                if (!CheckBdAddress)
                {
                    dutBd06 = a.Data[3];
                    dutBd05 = a.Data[4];
                    dutBd04 = a.Data[5];
                    dutBd03 = a.Data[6];
                    dutBd02 = a.Data[7];
                    dutBd01 = a.Data[8];

                    dutBdNap = dutBd01.ToString("X2") + dutBd02.ToString("X2");
                    dutBdUap = dutBd03.ToString("X2");
                    dutBdLap = dutBd04.ToString("X2") + dutBd05.ToString("X2") + dutBd06.ToString("X2");

                    dutFullBdAddress = dutBdNap + dutBdUap + dutBdLap;

                    CheckBdAddress = true;
                }
#endif
            }
            else if (a.Data[0] == 0x02 && a.Data[1] == 0x0b && a.Data[2] == USB_DEVICE_DATA_RSP_HS_REMOTE_NAME) // check remote name
            {
                if (!CheckModelName)
                {
                    int x = 0;
                    for (int i = 0; i < 30; i++)
                    {
                        dutModelNameByte[i] = a.Data[i + 3];
                        if (a.Data[i + 3] == '\0') { x = i;  break; }
                    }
                    //dutModelName = dutModelNameByte.ToString();
                    dutModelName = Encoding.UTF8.GetString(dutModelNameByte, 0, x);
                    CheckModelName = true;
                }
                
            }
            else if (a.Data[0] == 0x02 && a.Data[1] == 0x0b && a.Data[2] == 0x06) // check version and region
            {
                if (!CheckFwVersion)
                {
                    dutVersionMajor = a.Data[3].ToString("X1");
                    dutVersionMinor = a.Data[4].ToString("X1");
                    dutVersionRevision = a.Data[5].ToString("X2");
                    dutFullVersion = dutVersionMajor + dutVersionMinor + dutVersionRevision;
                    dutRegion = a.Data[6];

                    CheckFwVersion = true;
                }
                
            }
            else if (a.Data[0] == 0x02 && a.Data[1] == 0x0b && a.Data[2] == 0x07) // check factory reset
            {
                if (!CheckFactoryReset)
                {
                    dutFactoryReset = a.Data[3];
                    CheckFactoryReset = true;
                }
            }
#if true // 200406.wsjung.test
            //////////////////////////////
            // vaild data : a.Data[4] //
            //////////////////////////////
            else if (a.Data[0] == 0x02 && a.Data[1] == 0x0b && a.Data[2] == USB_DEVICE_DATA_RSP_HS_SPP_RX) // SPP Test
            {
                
                Console.WriteLine("_____[wsjung] SPP RX Received!!!!");
                if(a.Data[3] != 0)
                {                    
                    int size = a.Data[3];
                    for(int i=0; i<size; i++)
                    {
                        Console.Write("{0:X2}", a.Data[i + 4]);
                        Console.Write(" ");   
                    }
                    Console.WriteLine("");
                    
                    
                    byte[] dataForProc = new byte[size];
                    //Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));
                    Buffer.BlockCopy(a.Data, 4, dataForProc, 0, size);
                    procRaceCmdData(dataForProc);

#if ATH_CKS50TW2
                    if (a.Data[9] == 0x00 && a.Data[10] == 0x04) // Check FW Version
                    {
                        /*if (!CheckFwVersion)
                        {                            
                            dutFullVersion = Convert.ToString((a.Data[11]/16)*1000 +(a.Data[11]%16)*100 + (a.Data[12]/16)*10 + a.Data[12]%16);
                            CheckFwVersion = true;                            
                        } */
                        if (!CheckFwVersionPri)
                        {
                            dutFullVersionPri = Convert.ToString((a.Data[11] / 16) * 1000 + (a.Data[11] % 16) * 100 + (a.Data[12] / 16) * 10 + a.Data[12] % 16);
                            CheckFwVersionPri = true;
                        }
                        else if (!CheckFwVersionSec)
                        {
                            dutFullVersionSec = Convert.ToString((a.Data[13] / 16) * 1000 + (a.Data[13] % 16) * 100 + (a.Data[14] / 16) * 10 + a.Data[14] % 16);
                            CheckFwVersionSec = true;
                        }
                    }
                    else if (a.Data[9] == 0x00 && a.Data[10] == 0x0A) // Shipping power off Primary
                    {
                        if (!CheckShippingPoweroffPri)
                        {
                            CheckShippingPoweroffPri = true;
                        }
                    }
                    else if (a.Data[17] == 0x00 && a.Data[18] == 0x0A) // Shipping power off Secondary
                    {
                        if (!CheckShippingPoweroffSec)
                        {
                            CheckShippingPoweroffSec = true;
                        }
                    }
                    else if (a.Data[6] == 0x15 && a.Data[9] == 0x00 && a.Data[10] == 0x10) // Check Model Name Primary
                    {
                        if (!CheckModelNamePri)
                        {
                            int x = 0;
                            for (int i = 0; i < 30; i++)
                            {
                                dutModelNamePriByte[i] = a.Data[i + 11];
                                if (a.Data[i + 11] == '\0') { x = i; break; }
                            }
                            //dutModelName = dutModelNameByte.ToString();
                            dutModelNamePri = Encoding.UTF8.GetString(dutModelNamePriByte, 0, x);
                            CheckModelNamePri = true;
                        }
                    }
                    else if (a.Data[14] == 0x15 && a.Data[17] == 0x00 && a.Data[18] == 0x10) // Check Model Name Secondary
                    {
                        if (!CheckModelNameSec)
                        {
                            int x = 0;
                            for (int i = 0; i < 30; i++)
                            {
                                dutModelNameSecByte[i] = a.Data[i + 19];
                                if (a.Data[i + 19] == '\0') { x = i; break; }
                            }
                            //dutModelName = dutModelNameByte.ToString();
                            dutModelNameSec = Encoding.UTF8.GetString(dutModelNameSecByte, 0, x);
                            CheckModelNameSec = true;
                        }
                    }
                    else if (a.Data[8] == 0x06 && a.Data[9] == 0x0E) // Check ANC Parameter Write Primary, Default parameter = 0x00 / After write != 0x00
                    {
                        AncWriteCheckDataPri = a.Data[12] + a.Data[13] + a.Data[14] + a.Data[15] + a.Data[16] + a.Data[17] + a.Data[18] + a.Data[19];

                        if (!CheckAncWritePri)
                        {
                            if (a.Data[10] == 0x00 && AncWriteCheckDataPri == 0x00)
                            {
                                Console.WriteLine("Check ANC Parameter Primary send is OK");
                                Console.WriteLine("Check ANC Parameter Primary is FAIL");

                                CheckAncWritePri = false;
                            }
                            else if (a.Data[10] == 0x00 && AncWriteCheckDataPri != 0x00)
                            {
                                Console.WriteLine("Check ANC Parameter Primary send is OK");
                                Console.WriteLine("Check ANC Parameter Primary is OK");
                                CheckAncWritePri = true;
                            }
                        }
                    }
                    else if (a.Data[16] == 0x06 && a.Data[17] == 0x0E) // Check ANC Parameter Write Secondary, Default parameter = 0x00 / After write != 0x00
                    {

                        AncWriteCheckDataSec = a.Data[20] + a.Data[21] + a.Data[22] + a.Data[23] + a.Data[24] + a.Data[25] + a.Data[26] + a.Data[27];

                        if (!CheckAncWriteSec)
                        {
                            if (a.Data[18] == 0x00 && AncWriteCheckDataSec == 0x00)
                            {
                                Console.WriteLine("Check ANC Parameter Secondary send is OK");
                                Console.WriteLine("Check ANC Parameter Secondary is FAIL");
                                CheckAncWriteSec = false;
                            }
                            else if (a.Data[18] == 0x00 && AncWriteCheckDataSec != 0x00)
                            {
                                Console.WriteLine("Check ANC Parameter Secondary send is OK");
                                Console.WriteLine("Check ANC Parameter Secondary is OK");
                                CheckAncWriteSec = true;
                            }
                        }
                    }
                    else if (a.Data[6] == 0x03 && a.Data[8] == 0x01 && a.Data[9] == 0x0A)
                    {
                        if(!WriteFwColorPri)
                        {
                            Console.WriteLine("Color Write Primary is ok");
                            WriteFwColorPri = true;
                        }
                    }
                    else if (a.Data[6] == 0x03 && a.Data[8] == 0x01 && a.Data[9] == 0x0D)
                    {
                        if(!WriteFwColorSec)
                        {
                            Console.WriteLine("Color Write Secondary is ok");
                            WriteFwColorSec = true;
                        }
                    }
                    else if (a.Data[9]== 0x0A && a.Data[10] == 0x01 && a.Data[11] == 0x00)
                    {
                        if(!CheckFwColorPri)
                        {                            
                            if(a.Data[12] == 0x00)
                            {
                                dutFwColorPri = "BK";
                                Console.WriteLine("Color Check Primary is ok");
                                CheckFwColorPri = true;
                            }
                            else if(a.Data[12] == 0x01)
                            {
                                dutFwColorPri = "GR";
                                Console.WriteLine("Color Check Primary is ok");
                                CheckFwColorPri = true;
                            }
                            else if (a.Data[12] == 0x02)
                            {
                                dutFwColorPri = "BG";
                                Console.WriteLine("Color Check Primary is ok");
                                CheckFwColorPri = true;
                            }
                            else
                            {
                                dutFwColorPri = "NG";
                                Console.WriteLine("Color Check Primary is fail");
                                CheckFwColorPri = false;
                            }
                        }
                    }
                    else if (a.Data[17] == 0x0A && a.Data[18] == 0x01 && a.Data[19] == 0x00)
                    {
                        if (!CheckFwColorSec)
                        {
                            if (a.Data[20] == 0x00)
                            {
                                dutFwColorSec = "BK";
                                Console.WriteLine("Color Check Primary is ok");
                                CheckFwColorSec = true;
                            }
                            else if (a.Data[20] == 0x01)
                            {
                                dutFwColorSec = "GR";
                                Console.WriteLine("Color Check Primary is ok");
                                CheckFwColorSec = true;
                            }
                            else if (a.Data[20] == 0x02)
                            {
                                dutFwColorSec = "BG";
                                Console.WriteLine("Color Check Primary is ok");
                                CheckFwColorSec = true;
                            }
                            else
                            {
                                dutFwColorSec = "NG";
                                Console.WriteLine("Color Check Primary is fail");
                                CheckFwColorSec = false;
                            }
                        }
                    }

#else
                    if (a.Data[17] == 0x04 && a.Data[18] == 0x2A) // Check FW Version
                    {
                        if (!CheckFwVersion)
                        {
                            dutFullVersion = Convert.ToString(a.Data[19] * 1000 + a.Data[20] * 100 + a.Data[21] * 10 + a.Data[22]);
                            CheckFwVersion = true;
                        }
                    }
#endif
                    else if (a.Data[9] == 0x00 && a.Data[10] == 0x05) // check battery level
                    {
                        if (!CheckBatLevel)
                        {
                            batLevelPri = a.Data[11];
                            batLevelSec = a.Data[12];

                            CheckBatLevel = true;
                        }
                    }
                    else if (a.Data[9] == 0x00 && a.Data[10] == 0x07 && flag_CheckDefaultBDPri)
                    {
                        if (!CheckDefaultBDPri)
                        {
                            dutBd06Pri = a.Data[11];
                            dutBd05Pri = a.Data[12];
                            dutBd04Pri = a.Data[13];
                            dutBd03Pri = a.Data[14];
                            dutBd02Pri = a.Data[15];
                            dutBd01Pri = a.Data[16];

                            dutBd06Sec = a.Data[17];
                            dutBd05Sec = a.Data[18];
                            dutBd04Sec = a.Data[19];
                            dutBd03Sec = a.Data[20];
                            dutBd02Sec = a.Data[21];
                            dutBd01Sec = a.Data[22];

                            dutBdNapPri = dutBd01Pri.ToString("X2") + dutBd02Pri.ToString("X2");
                            dutBdUapPri = dutBd03Pri.ToString("X2");
                            dutBdLapPri = dutBd04Pri.ToString("X2") + dutBd05Pri.ToString("X2") + dutBd06Pri.ToString("X2");

                            dutBdNapSec = dutBd01Sec.ToString("X2") + dutBd02Sec.ToString("X2");
                            dutBdUapSec = dutBd03Sec.ToString("X2");
                            dutBdLapSec = dutBd04Sec.ToString("X2") + dutBd05Sec.ToString("X2") + dutBd06Sec.ToString("X2");

                            dutFullBdAddressPri = dutBdNapPri + dutBdUapPri + dutBdLapPri;
                            dutFullBdAddressSec = dutBdNapSec + dutBdUapSec + dutBdLapSec;

                            CheckDefaultBDPri = true;
                            CheckDefaultBDSec = true;
                            CheckBdAddress = true;
                            flag_CheckDefaultBDPri = false;
                            flag_CheckDefaultBDSec = false;
                        }

                    }
                    else if (a.Data[9] == 0x00 && a.Data[10] == 0x07) // Check BD Address
                    {
                        if (!CheckBdRange)
                        {
                            dutBd06Pri = a.Data[11];
                            dutBd05Pri = a.Data[12];
                            dutBd04Pri = a.Data[13];
                            dutBd03Pri = a.Data[14];
                            dutBd02Pri = a.Data[15];
                            dutBd01Pri = a.Data[16];

                            dutBd06Sec = a.Data[17];
                            dutBd05Sec = a.Data[18];
                            dutBd04Sec = a.Data[19];
                            dutBd03Sec = a.Data[20];
                            dutBd02Sec = a.Data[21];
                            dutBd01Sec = a.Data[22];

                            dutBdNapPri = dutBd01Pri.ToString("X2") + dutBd02Pri.ToString("X2");
                            dutBdUapPri = dutBd03Pri.ToString("X2");
                            dutBdLapPri = dutBd04Pri.ToString("X2") + dutBd05Pri.ToString("X2") + dutBd06Pri.ToString("X2");

                            dutBdNapSec = dutBd01Sec.ToString("X2") + dutBd02Sec.ToString("X2");
                            dutBdUapSec = dutBd03Sec.ToString("X2");
                            dutBdLapSec = dutBd04Sec.ToString("X2") + dutBd05Sec.ToString("X2") + dutBd06Sec.ToString("X2");

                            dutFullBdAddressPri = dutBdNapPri + dutBdUapPri + dutBdLapPri;
                            dutFullBdAddressSec = dutBdNapSec + dutBdUapSec + dutBdLapSec;

                            CheckBdRange = true;
                        }
                    }                     
                }                


#if false // 201006.wsjung.test
                byte RaceChannel = 0;
                byte RaceType = 0;
                int RaceId = 0;
                int RespLength = 0;

                RaceChannel = a.Data[4];
                RaceType = a.Data[5];

                RaceId |= ((int)(a.Data[8] << 0));
                RaceId |= ((int)(a.Data[9] << 8));

                RespLength = a.Data[6] + (a.Data[7] * 255);

                if (RaceChannel == RACE_CHANNEL)
                {
                    switch (RaceType)
                    {
                        case RACE_TYPE_RESP: //
                            switch (RaceId)
                            {
                                case RACE_ID_READ_FULLKEY:
                                    break;
                            } // switch(RaceId)
                            break;
                        case RACE_TYPE_NOTI:
                            switch (RaceId)
                            {
                                case RACE_ID_READ_FULLKEY:
                                    break;
                                case RACE_ID_READ_BATTERY:
                                    if(a.Data[10] == 0x00) // status : OK
                                    {
                                        if(a.Data[11] == 0x00) // agent
                                        {
                                            Console.WriteLine("_____[wsjung] got agent battery level : {0}%", a.Data[12]);
                                        }
                                        else if(a.Data[11] == 0x01) // partner
                                        {
                                            Console.WriteLine("_____[wsjung] got partner battery level : {0}%", a.Data[12]);
                                        }
                                    }
                                    break;
                            }
                            break;
                    } // switch(RaceType)
                }
#endif

#if false
                if(a.Data[4] == 0x05 && a.Data[5] == 0x5B && a.Data[6] == 0x03 && a.Data[7] == 0x00 && a.Data[8] == 0x03 && a.Data[9] == 0x0A && a.Data[10] == 0x01)
                {
                    Form1.dev.StartAsyncRead();
                    byte[] data = new byte[50];
                    byte[] cmdData = new byte[] { 0x05, 0x5A, 0x12, 0x00, 0x01, 0x0A, 0x02, 0xF2, 0x41, 0x54, 0x48, 0x2D, 0x53, 0x51, 0x31, 0x54, 0x57, 0x31, 0x31, 0x31, 0x31, 0x33 };
                    data[0] = 0x02;
                    data[1] = 0x0b;
                    data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
                    data[3] = (byte)Buffer.ByteLength(cmdData);
                    Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

                    Form1.dev.Write(data);
                }
#endif
            }
#endif
                }
        
        public static void enter(object s, EventArgs a)
        {
            Console.WriteLine("device arrived");
        }
        public static void exit(object s, EventArgs a)
        {
            Console.WriteLine("device removed");
        }

#if true // 201007.wsjung.test
        public static void procRaceCmdData(byte[] race_data)
        {
            byte RaceChannel = 0;
            byte RaceType = 0;
            int RaceId = 0;
            int RespLength = 0;
            int DataLength = 0;
            int size = 0;

            RaceChannel = race_data[0];
            RaceType = race_data[1];

            RaceId |= ((int)(race_data[4] << 0));
            RaceId |= ((int)(race_data[5] << 8));

            RespLength = race_data[2] + (race_data[3] * 255);

            DataLength = Buffer.ByteLength(race_data);

            needProcessDstData = false;

            if (RaceChannel == RACE_CHANNEL)
            {
                if (DataLength == RespLength + 4)
                {
                    needProcessMoreData = false;
                }
                else if (DataLength > RespLength + 4)
                {
                    needProcessMoreData = true;
                }

                switch (RaceType)
                {
                    case RACE_TYPE_RESP: //
                        switch (RaceId)
                        {
                            case RACE_ID_READ_FULLKEY:
                                break;
                            case RACE_ID_CMDRELAY_AVA_DST:
                                if (DataLength > 9)
                                {
                                    destination_id = race_data[9];
                                    CheckAvaDest = true;
                                }
                                break;
                        } // switch(RaceId)
                        break;
                    case RACE_TYPE_NOTI:
                        switch (RaceId)
                        {
                            case RACE_ID_READ_FULLKEY:
                                break;
                            case RACE_ID_READ_BATTERY:
                                if (race_data[6] == 0x00) // status : OK
                                {
#if false
                                    if (race_data[7] == 0x00) // agent
                                    {
                                        Console.WriteLine("_____[wsjung] got agent battery level : {0}%", race_data[8]);
                                    }
                                    else if (race_data[7] == 0x01) // partner
                                    {
                                        Console.WriteLine("_____[wsjung] got partner battery level : {0}%", race_data[8]);
                                    }
#endif
                                    if (requestAgentBattery)
                                    {
                                        Console.WriteLine("_____[wsjung] got agent battery level : {0}%", race_data[8]);
                                    }
                                    else if (requestPartnerBattery)
                                    {
                                        Console.WriteLine("_____[wsjung] got partner battery level : {0}%", race_data[8]);
                                    }

                                    requestAgentBattery = requestPartnerBattery = false;
                                }
                                break;
                            case RACE_ID_READ_AE_INFO:
                                if (race_data[6] == 0x00) // status : OK
                                {
                                    if(requestAgentAEInfo)
                                    {
                                        Console.WriteLine("_____[wsjung] got Agent FW Version : {0}{1}{2}{3}", race_data[11], race_data[12], race_data[13], race_data[14]);
                                    }
                                    else if (requestPartnerAEInfo)
                                    {
                                        Console.WriteLine("_____[wsjung] got Partner FW Version : {0}{1}{2}{3}", race_data[11], race_data[12], race_data[13], race_data[14]);
                                    }

                                    requestAgentAEInfo = requestPartnerAEInfo = false;
                                }
                                break;
                            case RACE_ID_CMDRELAY_PASS_TO_DST: // get this by Notification mean, agent send command to partner to get partner information
                                if (RespLength + 4 == DataLength)
                                {
                                    if(race_data[6] == 0x05 && race_data[7] == destination_id && race_data[8] == 0x05 && race_data[9] == 0x5D)
                                    {
                                        needProcessDstData = true;
                                    }
                                }
                                break;
                        }
                        break;
                } // switch(RaceType)

                // processing more data
                if(needProcessMoreData)
                {
                    size = DataLength - (RespLength + 4);
                    byte[] dataForProcMoreData = new byte[size];
                    //Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));
                    Buffer.BlockCopy(race_data, (RespLength + 4), dataForProcMoreData, 0, size);
                    procRaceCmdData(dataForProcMoreData);
                }

                if(needProcessDstData)
                {
                    size = DataLength - 8;
                    byte[] dataForProcDstData = new byte[size];
                    Buffer.BlockCopy(race_data, 8, dataForProcDstData, 0, size);
                    procRaceCmdData(dataForProcDstData);
                }
            }
        }
#endif
#endif

                #region delegates
                delegate void writeDupeDataDelegate(string bd); // dupe bd write
        public void writeDupeData(string bd)
        {
            if (InvokeRequired)
            {
                writeDupeDataDelegate writeDupeDel = new writeDupeDataDelegate(writeDupeData);
                Invoke(writeDupeDel, bd);
            }
            else
            {
                // add dupe data to datagrid2
                string[] rowrow = { passCount.ToString(), bd, "OK", "0" };
                dataGridView2.Rows.Add(rowrow);

                // add dupe data to log
                writeDupeLog(passCount.ToString(), bd, "OK", "0");
            }
        }

        delegate bool findDupeDataDelegate(string bd); // dupe bd find
        public bool findDupeData(string bd)
        {
            if (InvokeRequired)
            {
                findDupeDataDelegate findDupeDel = new findDupeDataDelegate(findDupeData);
                return (bool)Invoke(findDupeDel, bd);
            }
            else
            {
                int count = 0;
                int row = 0;
                bool isDupe = false;

                count = dataGridView2.Rows.Count - 1;

                if(count != 0)
                {
                    for (row = 0; row < count; row++)
                    {
                        string ttt = dataGridView2.Rows[row].Cells[1].Value.ToString();
                        if (bd == dataGridView2.Rows[row].Cells[1].Value.ToString())
                        {
                            // focus?
                            dataGridView2.FirstDisplayedScrollingRowIndex = dataGridView2.Rows[row].Index;

                            // change dupe count
                            string strTempCount = dataGridView2.Rows[row].Cells[3].Value.ToString();
                            dataGridView2.Rows[row].Cells[3].Value = (Convert.ToInt32(strTempCount) + 1).ToString();

                            dataGridView2.Rows[row].DefaultCellStyle.BackColor = Color.Red;
                            gLatestDupeRow = row;
                            isDupe = true;
                            break;
                        }
                    }
                }
                
                if (!isDupe) { return false; }
                else { return true; }
            }

        }

        delegate void showResultDelegate(int result, string bd); // show final result and write log
        public void showResult(int result, string bd)
        {
            if(InvokeRequired)
            {
                showResultDelegate resultDel = new showResultDelegate(showResult);
                Invoke(resultDel, result, bd);
            }
            else 
            {                
                if (result == 1) // flag_ng == 1
                {
                    // write log
                    writeLog(bd, "FAIL", typeNg);

                    totalCount++;
                    failCount++;
                    
                    tb_totalCount.Text = totalCount.ToString();
                    tb_failCount.Text = failCount.ToString();

                    lbResult.Text = "FAIL";
                    //lbResult.ForeColor = Color.Red;
                    lbResult.BackColor = Color.Red;

                    NgSound();
                }
                else
                {
                    // write log
                    writeLog(bd, "PASS", "-");

                    totalCount++;
                    passCount++;

                    tb_totalCount.Text = totalCount.ToString();
                    tb_passCount.Text = passCount.ToString();

                    lbResult.Text = "PASS";
                    //lbResult.ForeColor = Color.SkyBlue;
                    //lbResult.BackColor = Color.LightBlue;
                    lbResult.BackColor = Color.Blue;
                }
            }
        }

        delegate void toggleBtnDelegate(Boolean x); // start button control
        public void toggleBtn(Boolean x)
        {
            if(InvokeRequired)
            {
                toggleBtnDelegate btnDel = new toggleBtnDelegate(toggleBtn);
                Invoke(btnDel, x);
            }
            else
            {
                if (x == true) { btnStart.Enabled = true; btnStart.Focus(); }
                else { btnStart.Enabled = false; }
            }
        }

        delegate void ShowErrorDelegates(string s); // temp
        public void showError(string s)
        {
            if (InvokeRequired)
            {
                ShowErrorDelegates eDel = new ShowErrorDelegates(showError);
                Invoke(eDel, s);
            }
            else
            {
                this.label1.Text = s;
                this.label1.ForeColor = Color.Red;
            }
        }

        delegate void ShowDelegate2(int row, string result); // show progress and result
        public void ShowProgress2(int row, string result)
        {
            if (InvokeRequired)
            {
                ShowDelegate2 del = new ShowDelegate2(ShowProgress2);
                //또는 ShowDelegate del = p => ShowProgress(p);
                Invoke(del, row, result);
            }
            else
            {
                //progressBar1.Value = pct;
                dataGridView1.Rows[row].Cells[3].Value = gTestValue[row];
                dataGridView1.Rows[row].Cells[4].Value = result;

                if (result == "FAIL")
                {
                    dataGridView1.Rows[row].DefaultCellStyle.BackColor = Color.Red;
                    typeNg = gNgType[row];
                }
                else if (result == "PASS")
                {
                    dataGridView1.Rows[row].DefaultCellStyle.BackColor = Color.SkyBlue;
                }
            }
        }

#if true
        delegate void ShowProgressIndicatorDelegate(int row, bool x); // wsjung.add.190208 : test
        public void ShowProgressIndicator(int row, bool x)
        {
            if(InvokeRequired)
            {
                ShowProgressIndicatorDelegate del = new ShowProgressIndicatorDelegate(ShowProgressIndicator);
                Invoke(del, row, x);
            }
            else
            {
                if(x == true)
                {
                    dataGridView1.Rows[row].Cells[3].Value = dataGridView1.Rows[row].Cells[3].Value + ">";
                }
                else
                {
                    dataGridView1.Rows[row].Cells[3].Value = "";
                }
                
            }
        }
#endif
        #endregion
      
        #region config_relative
        private void loadConfig()
        {
            // using ini style config
            pathConfigFile = System.IO.Directory.GetCurrentDirectory() + "\\" + "config\\" + "config.ini";
            string tempString;

            IniReadWrite IniReader = new IniReadWrite();

            tempString = IniReader.IniReadValue("CONFIG", "MODEL_NAME", pathConfigFile);
            gDutName = tempString;

            tempString = IniReader.IniReadValue("CONFIG", "SW_VERSION", pathConfigFile);
            gSwVersion = tempString;

            tempString = IniReader.IniReadValue("CONFIG", "MANUFACTURE_DATE", pathConfigFile);
            gManufactureDate = tempString;

            tempString = IniReader.IniReadValue("CONFIG", "DEFAULT_BD_NAP", pathConfigFile);
            gDefaultBdNap = tempString;

            tempString = IniReader.IniReadValue("CONFIG", "DEFAULT_BD_UAP", pathConfigFile);
            gDefaultBdUap = tempString;

            tempString = IniReader.IniReadValue("CONFIG", "DEFAULT_BD_LAP", pathConfigFile);
            gDefaultBdLap = tempString;

            gDefaultBd = gDefaultBdNap + gDefaultBdUap + gDefaultBdLap;

            tempString = IniReader.IniReadValue("CONFIG", "BD_NAP", pathConfigFile);
            gBdNap = tempString;

            tempString = IniReader.IniReadValue("CONFIG", "BD_UAP", pathConfigFile);
            gBdUap = tempString;

            tempString = IniReader.IniReadValue("CONFIG", "BD_LAP_START", pathConfigFile);
            gBdLapStart = tempString;

            tempString = IniReader.IniReadValue("CONFIG", "BD_LAP_END", pathConfigFile);
            gBdLapEnd = tempString;

#if false
            tempString = IniReader.IniReadValue("CONFIG", "BD_LAP_WRITE_NEXT", pathConfigFile);
            gBdLapWriteNext = tempString;

            tempString = IniReader.IniReadValue("CONFIG", "PORT", pathConfigFile);
            comPort = tempString;
#endif

            tempString = IniReader.IniReadValue("CONFIG", "LINE_INFO_A", pathConfigFile);
            gLineInfoA = tempString;

            tempString = IniReader.IniReadValue("CONFIG", "LINE_INFO_B", pathConfigFile);
            gLineInfoB = tempString;

            tempString = IniReader.IniReadValue("CONFIG", "APPNAME", pathConfigFile);
            gAppName = tempString;
            lb_AppName.Text = gAppName + " V" + this.ProductVersion; ;
            lb_AppName.BackColor = Color.LightYellow;
            lb_AppName.ForeColor = Color.Green;

            tempString = IniReader.IniReadValue("CONFIG", "FW_REGION", pathConfigFile); // wsjung.add.170405
            gFwRegion = tempString;

            tempString = IniReader.IniReadValue("CONFIG", "FW_COLOR", pathConfigFile);
            gFwColor = tempString;

#if false
            tempString = IniReader.IniReadValue("CONFIG", "CHECK_ONLY", pathConfigFile); // wsjung.add.170510
            gCheckOnly = tempString; // YES or NO
#endif


            // setup test list
            // max item of sequence is 20
            string tempProc = "";
            for(int i=0; i<20; i++)
            {
                tempProc = String.Format("TEST_{0}", i.ToString("D2"));
              //updateStatus(String.Format("High Temp(): {0}", count.ToString()));
                tempString = IniReader.IniReadValue("SEQUENCE", tempProc, pathConfigFile);
                if(tempString == "END")
                {
                    gCountSequence = i;
                    break;
                }
                else
                {
                    gtestSequence[i] = tempString;
                }
            }

            // display test list
            for(int j=0; j<gCountSequence; j++)
            {
                if (gtestSequence[j] == "CHECK_MODEL_NAME")
                {
                    string[] rowInfo = { j.ToString(), gtestSequence[j], gDutName, "-", "-" };
                    dataGridView1.Rows.Add(rowInfo);
                }
                else if (gtestSequence[j] == "CHECK_MODEL_NAME_PRI")
                {
                    string[] rowInfo = { j.ToString(), gtestSequence[j], gDutName, "-", "-" };
                    dataGridView1.Rows.Add(rowInfo);
                }
                else if (gtestSequence[j] == "CHECK_MODEL_NAME_SEC")
                {
                    string[] rowInfo = { j.ToString(), gtestSequence[j], gDutName, "-", "-" };
                    dataGridView1.Rows.Add(rowInfo);
                }
                else if (gtestSequence[j] == "CHECK_FW_VERSION")
                {
                    string[] rowInfo = { j.ToString(), gtestSequence[j], gSwVersion, "-", "-" };
                    dataGridView1.Rows.Add(rowInfo);
                }
                else if (gtestSequence[j] == "CHECK_FW_VERSION_PRI")
                {
                    string[] rowInfo = { j.ToString(), gtestSequence[j], gSwVersion, "-", "-" };
                    dataGridView1.Rows.Add(rowInfo);
                }
                else if (gtestSequence[j] == "CHECK_FW_VERSION_SEC")
                {
                    string[] rowInfo = { j.ToString(), gtestSequence[j], gSwVersion, "-", "-" };
                    dataGridView1.Rows.Add(rowInfo);
                }
                else if (gtestSequence[j] == "CHECK_ANC_WRITE_PRI")
                {
                    string[] rowInfo = { j.ToString(), gtestSequence[j], "-", "-", "-" };
                    dataGridView1.Rows.Add(rowInfo);
                }
                else if (gtestSequence[j] == "CHECK_ANC_WRITE_SEC")
                {
                    string[] rowInfo = { j.ToString(), gtestSequence[j], "-", "-", "-" };
                    dataGridView1.Rows.Add(rowInfo);
                }
                else if (gtestSequence[j] == "CHECK_BD_DEFAULT_PRI")
                {
                    string[] rowInfo = { j.ToString(), gtestSequence[j], gDefaultBd, "-", "-" };
                    dataGridView1.Rows.Add(rowInfo);
                }
                else if (gtestSequence[j] == "CHECK_BD_DEFAULT_SEC")
                {
                    string[] rowInfo = { j.ToString(), gtestSequence[j], gDefaultBd, "-", "-" };
                    dataGridView1.Rows.Add(rowInfo);
                }
                else if (gtestSequence[j] == "CHECK_BD_RANGE_PRI")
                {
                    string[] rowInfo = { j.ToString(), gtestSequence[j], String.Format("{0} ~ {1}", gBdLapStart, gBdLapEnd), "-", "-" };
                    dataGridView1.Rows.Add(rowInfo);
                }
                else if (gtestSequence[j] == "CHECK_BD_RANGE_SEC")
                {
                    string[] rowInfo = { j.ToString(), gtestSequence[j], String.Format("{0} ~ {1}", gBdLapStart, gBdLapEnd), "-", "-" };
                    dataGridView1.Rows.Add(rowInfo);
                }
                else if (gtestSequence[j] == "CHECK_REGION")
                {
                    string[] rowInfo = { j.ToString(), gtestSequence[j], gFwRegion, "-", "-" };
                    dataGridView1.Rows.Add(rowInfo);
                }
                else if (gtestSequence[j] == "WRITE_COLOR_PRI")
                {
                    string[] rowInfo = { j.ToString(), gtestSequence[j], gFwColor, "-", "-" };
                    dataGridView1.Rows.Add(rowInfo);
                }
                else if (gtestSequence[j] == "WRITE_COLOR_SEC")
                {
                    string[] rowInfo = { j.ToString(), gtestSequence[j], gFwColor, "-", "-" };
                    dataGridView1.Rows.Add(rowInfo);
                }
                else if (gtestSequence[j] == "CHECK_COLOR_PRI")
                {
                    string[] rowInfo = { j.ToString(), gtestSequence[j], gFwColor, "-", "-" };
                    dataGridView1.Rows.Add(rowInfo);
                }
                else if (gtestSequence[j] == "CHECK_COLOR_SEC")
                {
                    string[] rowInfo = { j.ToString(), gtestSequence[j], gFwColor, "-", "-" };
                    dataGridView1.Rows.Add(rowInfo);
                }

#if false
                else if (gtestSequence[j] == "WRITE_REGION")
                {
                    string[] rowInfo = { j.ToString(), gtestSequence[j], gFwRegion, "-", "-" };
                    dataGridView1.Rows.Add(rowInfo);
                }
#endif
                else
                {
                    string[] rowInfo = { j.ToString(), gtestSequence[j], "-", "-", "-" };
                    dataGridView1.Rows.Add(rowInfo);
                }
            }

            // check bd setting range
            // check bd range
            UInt32 tempConfigLapStart = Convert.ToUInt32(Form1.gBdLapStart, 16);
            UInt32 tempConfigLapEnd = Convert.ToUInt32(Form1.gBdLapEnd, 16);
#if false
            UInt32 tempLap = Convert.ToUInt32(Form1.gBdLapWriteNext, 16);
#endif

#if false
            if ((tempLap > tempConfigLapEnd) || (tempLap < tempConfigLapStart))
#endif
#if true
            if (tempConfigLapEnd <= tempConfigLapStart)
#endif
            {
                MessageBox.Show("Check BD Address Range Config");
                Close();
            }

        }
        #endregion

        #region log_relative
        private void writeLog(string bd, string result, string faildesc)
        {
#if true
            string date = DateTime.Now.ToString("yy-MM-dd HH:mm:ss");

            // open stream
            StreamWriter sw = new StreamWriter(pathLogFile, true, Encoding.Unicode);

            // write measured data
            sw.WriteLine("{0}\t{1}\t{2}\t{3}", date, bd, result, faildesc);
            // close stream
            sw.Close();
#endif

#if false
            string date = DateTime.Now.ToString("yy-MM-dd");
            string newLog = String.Format("{0}, {1}, {2}, {3}", date, bd, result, faildesc);
            string[] lines = System.IO.File.ReadAllLines(pathLogFile);

            lines[1] = newLog;
            System.IO.File.WriteAllLines(pathLogFile, lines);
#endif
        }

        private bool checkLogFile()
        {
            DateTime today = DateTime.Now;
            //string strDatePrefix = string.Format("{0:YY-MM-DD}", today);
            string strDatePrefix = today.ToString("yy-MM-dd");
            string strLogFile = strDatePrefix + "_" + gDutName + "-log.csv";

            // set file path
            pathLogFile = System.IO.Directory.GetCurrentDirectory() + "\\" + "log\\" + strLogFile;

            if (File.Exists(pathLogFile))
            {
                // re-set test Cound
                StreamReader sr = new StreamReader(pathLogFile);
                string line;
                UInt32 lineCount = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    lineCount++;
                }
                sr.Close();
                totalCount = lineCount - 1;

                return true;
            }
            else
            {
                //make new log file
                StreamWriter sw = new StreamWriter(pathLogFile, true, Encoding.Unicode);

                // write basic data (index)
                sw.WriteLine("date" + "\t" + "bd" + "\t" + "result" + "\t" + "ng description");
#if false
                sw.WriteLine("--" + "," + "--" + "," + "--" + "," + "--");
#endif
                sw.Close();
                totalCount = 0;

                return false;
            }
        }

        private void writeDupeLog(string num, string bd, string result, string dupeCount)
        {
            string date = DateTime.Now.ToString("yy-MM-dd");

            // open stream
            StreamWriter swd = new StreamWriter(pathDupeLogFile, true, Encoding.Unicode);


            // write measured data
            swd.WriteLine("{0},{1},{2},{3}", num, bd, result, dupeCount);
            // close stream
            swd.Close();
        }

        private bool checkDupeLogFile()
        {
            DateTime today = DateTime.Now;
            string strDatePrefix = today.ToString("yy-MM-dd");
            string strLogFile = strDatePrefix + "_" + gDutName + "-dupelog.dat";

            // set file path
            pathDupeLogFile = System.IO.Directory.GetCurrentDirectory() + "\\" + "log\\" + strLogFile;

            if (File.Exists(pathDupeLogFile))
            {
                // re-set test Cound
                StreamReader srd = new StreamReader(pathDupeLogFile);
                string line;
                UInt32 lineCount = 0;
                while ((line = srd.ReadLine()) != null)
                {
                    lineCount++;
                    string[] rowrow = line.Split(',');
                    dataGridView2.Rows.Add(rowrow);
                }
                srd.Close();

                passCount = lineCount;

                return true;
            }
            else
            {
                //make new log file
                StreamWriter swd = new StreamWriter(pathDupeLogFile, true, Encoding.Unicode);

                // write basic data (index)
                //swd.WriteLine("date" + "," + "bd" + "," + "result" + "," + "ng description");
                swd.Close();

                return false;
            }
        }
        #endregion

        private void Form1_FormClosed_1(object sender, FormClosedEventArgs e)
        {
            if (dev != null)
                dev.Dispose();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            procTestOpenPort2();
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();
            byte[] data = new byte[19];

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_DISCONNECT;

            dev.Write(data);

            HeadsetConnected = false;
            tbResultConnect.Text = "";
        }

        private void btnEnableAiroThrough_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();

            byte[] data = new byte[50];
            byte[] cmdData = new byte[] { 0x05, 0x5A, 0x04, 0x00, 0x01, 0x11, 0x8B, 0x00 };

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

            dev.Write(data);
        }


        private void btnClaim_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();

            byte[] data = new byte[50];
            byte[] cmdData = new byte[] { 0x05, 0x5A, 0x04, 0x00, 0x03, 0x0A, 0xE0, 0x00 };

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

            dev.Write(data);
        }

        private void btnChangeATOutGain_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();

            byte[] data = new byte[50];
            byte[] cmdData = new byte[] { 0x05, 0x5A, 0x26, 0x00, 0x01, 0x0A, 0x16, 0xE0, 0x10, 0x00, 0x47, 0x00, 0x47, 0x00, 0x47, 0x00, 0x47, 0x00, 0x47, 0x00, 0x47, 0x00, 0x47, 0x00, 0x47, 0x00, 0x47, 0x00, 0x47, 0x00, 0x47, 0x00, 0x47, 0x00, 0x47, 0x00, 0x47, 0x00, 0x47, 0x00, 0x47, 0x00 };

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

            dev.Write(data);
        }

        private void btnChangeATOutGain2_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();

            byte[] data = new byte[50];
            byte[] cmdData = new byte[] { 0x05, 0x5A, 0x26, 0x00, 0x01, 0x0A, 0x16, 0xE0, 0x10, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x6F, 0x00 };

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

            dev.Write(data);
        }

        private void btnRoleChange_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();

            byte[] data = new byte[50];
            byte[] cmdData = new byte[] { 0x05, 0x5A, 0x04, 0x00, 0x01, 0x11, 0x9D, 0x00 };

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

            dev.Write(data);
        }

        private void btnGetAEInfo_Click(object sender, EventArgs e)
        {
            requestAgentAEInfo = true;

            dev.StartAsyncRead();

            byte[] data = new byte[50];
            byte[] cmdData = new byte[] { 0x05, 0x5A, 0x04, 0x00, 0x09, 0x1C, 0x01, 0xFF };

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

            dev.Write(data);
        }

        private void btnGetPartnerAEInfo_Click(object sender, EventArgs e)
        {
            requestPartnerAEInfo = true;

            dev.StartAsyncRead();

            byte[] data = new byte[50];
            byte[] cmdData = new byte[] { 0x05, 0x5A, 0x0C, 0x00, 0x01, 0x0D, 0x05, 0x00, 0x05, 0x5A, 0x04, 0x00, 0x09, 0x1C, 0x01, 0xFF };
            cmdData[7] = destination_id;

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

            dev.Write(data);
        }

        private void btnGetAgentBattery_Click(object sender, EventArgs e)
        {
            requestAgentBattery = true;

            dev.StartAsyncRead();

            byte[] data = new byte[50];
            //byte[] cmdData = new byte[] { 0x05, 0x5A, 0x03, 0x00, 0xD6, 0x0C, 0x00 };
            byte[] cmdData = new byte[] { 0x05, 0x5A, 0x06, 0x00, 0x00, 0x0A, 0x00, 0xF4, 0x40, 0x00 };

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

            dev.Write(data);
        }
        
        private void btnGetPartnerBattery_Click(object sender, EventArgs e)
        {
            requestPartnerBattery = true;

            dev.StartAsyncRead();

            byte[] data = new byte[50];
            //byte[] cmdData = new byte[] { 0x05, 0x5A, 0x03, 0x00, 0xD6, 0x0C, 0x01 };
            byte[] cmdData = new byte[] { 0x05, 0x5A, 0x0B, 0x00, 0x01, 0x0D, 0x05, 0x00, 0x05, 0x5A, 0x03, 0x00, 0xD6, 0x0C, 0x00 };
            cmdData[7] = destination_id;

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

            dev.Write(data);
        }

        private void btnGetAudioChannel_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();

            byte[] data = new byte[50];
            byte[] cmdData = new byte[] { 0x05, 0x5A, 0x02, 0x00, 0xC2, 0x01 };

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

            dev.Write(data);
        }

        private void btnGetPartnerAudioChannel_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();

            byte[] data = new byte[50];
            byte[] cmdData = new byte[] { 0x05, 0x5A, 0x0A, 0x00, 0x01, 0x0D, 0x05, 0x06, 0x05, 0x5A, 0x02, 0x00, 0xC2, 0x01 };
            cmdData[7] = destination_id;

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

            dev.Write(data);
        }

        private void btnGetAvaDst_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();

            byte[] data = new byte[50];
            byte[] cmdData = new byte[] { 0x05, 0x5A, 0x02, 0x00, 0x00, 0x0D };

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

            dev.Write(data);
        }
#if true
        private bool procTestOpenPort2() // check connection
        {
            int flag_ng = 0;
            int Count = 20;

            try
            {
                dev.StartAsyncRead();

                byte[] data = new byte[19];

                data[0] = 0x02;
                data[1] = 0x0b;
                data[2] = 0x00;

                dev.Write(data);

                while (Count > 0)
                {
                    dev.StartAsyncRead();
                    Thread.Sleep(1000);
                    Count--;

                    if (Form1.HeadsetConnected)
                    {
                        Console.WriteLine("Headset Connected");
                        tbResultConnect.Text = "Connected";
                        break;
                    }
                }
            }
            //catch (System.Exception ex)
            catch
            {
                flag_ng = 1;
            }
            finally
            {
                if (Count == 0 || !Form1.HeadsetConnected)
                {
                    flag_ng = 1;
                }
            }

            if (flag_ng == 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        
        private void btnGetFWVersion_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();
            byte a = 0;
            byte[] data = new byte[50];
            byte[] cmdData = new byte[] { 0x05, 0x5A, 0x03, 0x00, 0x01, 0x00, 0x04 };
            
            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));            
            dev.Write(data);            
        }

        private void btnGetBDAddr_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();
            byte a = 0;
            byte[] data = new byte[50];
            //byte[] cmdData = new byte[] { 0xAA, a, 0x04, 0x00, 0x00, 0x08, 0x00, 0x9B };
            byte[] cmdData = new byte[] { 0x05, 0x5A, 0x03, 0x00, 0x01, 0x00, 0x07 };

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

            dev.Write(data);
        }

        private void btnGetFWMode_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();
            byte a = 0;
            byte[] data = new byte[50];
            byte[] cmdData = new byte[] { 0xAA, a, 0x04, 0x00, 0x00, 0x08, 0x00, 0x90 };

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

            dev.Write(data);
        }

        private void btnGetRWSStatus_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();
            byte a = 0;
            byte[] data = new byte[50];
            byte[] cmdData = new byte[] { 0xAA, a, 0x03, 0x00, 0x18, 0x00, 0x00 };

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

            dev.Write(data);
        }

        private void btnGetBATStatus_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();
            byte a = 0;
            byte[] data = new byte[50];
            byte[] cmdData = new byte[] { 0xAA, a, 0x03, 0x00, 0x18, 0x00, 0x02 };

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

            dev.Write(data);
        }

        private void btnGetAPTGain_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();
            byte a = 0;
            byte[] data = new byte[50];
            byte[] cmdData = new byte[] { 0xAA, a, 0x04, 0x00, 0x00, 0x08, 0x00, 0x97 };

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

            dev.Write(data);
        }

        private void btnGetGFPSID_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();
            byte a = 0;
            byte[] data = new byte[50];
            byte[] cmdData = new byte[] { 0xAA, a, 0x04, 0x00, 0x00, 0x08, 0x00, 0x4c };

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

            dev.Write(data);
        }

        private void btnGetGFPSPublickey_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();
            byte a = 0;
            byte[] data = new byte[100];
            byte[] cmdData = new byte[] { 0xAA, a, 0x04, 0x00, 0x00, 0x08, 0x00, 0x93 };

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

            dev.Write(data);
        }

        private void btnGetGFPSPrivatekey_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();
            byte a = 0;
            byte[] data = new byte[100];
            byte[] cmdData = new byte[] { 0xAA, a, 0x04, 0x00, 0x00, 0x08, 0x00, 0x95 };

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

            dev.Write(data);
        }
        
        private void btnSetFactorymode_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();
            byte a = 0;
            byte[] data = new byte[50];
            byte[] cmdData = new byte[] { 0xAA, a, 0x05, 0x00, 0x00, 0x08, 0x00, 0x91, 0x00 };

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

            dev.Write(data);
        }

        private void btnSetUserMode_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();
            byte a = 0;
            byte[] data = new byte[50];
            byte[] cmdData = new byte[] { 0xAA, a, 0x05, 0x00, 0x00, 0x08, 0x00, 0x91, 0x01 };

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

            dev.Write(data);
        }

        private void btnFactoryReset_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();
            byte a = 0;
            byte[] data = new byte[50];
            byte[] cmdData = new byte[] { 0xAA, a, 0x04, 0x00, 0x04, 0x00, 0x00, 0x5A};

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

            dev.Write(data);
        }

        private void btnEnterDUTMode_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();
            byte a = 0;
            byte[] data = new byte[50];
            byte[] cmdData = new byte[] { 0xAA, a, 0x04, 0x00, 0x04, 0x00, 0x00, 0x5C };

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

            dev.Write(data);
        }

        private void btnPoweroff_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();
            byte a = 0;
            byte[] data = new byte[50];
            byte[] cmdData = new byte[] { 0xAA, a, 0x04, 0x00, 0x04, 0x00, 0x00, 0x56 };

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

            dev.Write(data);
        }

        private void btnAPTModetoggle_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();
            byte a = 0;
            byte[] data = new byte[50];
            byte[] cmdData = new byte[] { 0xAA, a, 0x04, 0x00, 0x04, 0x00, 0x00, 0x65 };

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

            dev.Write(data);
        }

        private void btnGetFWVersionSec_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();
            byte a = 0;
            byte[] data = new byte[50];
            byte[] cmdData = new byte[] { 0xAA, a, 0x03, 0x00, 0x08, 0x03, 0x01 };

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));
            dev.Write(data);
        }

        private void btnSetGFPSID_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();
            byte a = 0;
            byte[] data = new byte[50];
            byte[] cmdData = new byte[] { 0xAA, a, 0x03, 0x00, 0x08, 0x03, 0x01 };

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));
            dev.Write(data);
        }

        private void btnSetGFPSPublickey_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();
            byte a = 0;
            byte[] data = new byte[50];
            byte[] cmdData = new byte[] { 0xAA, a, 0x03, 0x00, 0x08, 0x03, 0x01 };

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));
            dev.Write(data);
        }

        private void btnSetGFPSPrivatekey_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();
            byte a = 0;
            byte[] data = new byte[50];
            byte[] cmdData = new byte[] { 0xAA, a, 0x03, 0x00, 0x08, 0x03, 0x01 };

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));
            dev.Write(data);
        }

        private void btnTest1_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();
            byte a = 0;
            byte[] data = new byte[50];
            byte[] cmdData = new byte[] { 0x05, 0x5A, 0x05, 0x00, 0x01, 0x0A, 0x00, 0xFF, 0x00 };

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));
            dev.Write(data);
        }

        private void btnTest2_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();
            byte a = 0;
            byte[] data = new byte[50];
            byte[] cmdData = new byte[] { 0x05, 0x5A, 0x05, 0x00, 0x01, 0x0A, 0x00, 0xFF, 0x01 };

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));
            dev.Write(data);
        }

        private void btnTest3_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();
            byte a = 0;
            byte[] data = new byte[50];
            byte[] cmdData = new byte[] { 0x05, 0x5A, 0x05, 0x00, 0x01, 0x0A, 0x00, 0xFF, 0x02 };

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));
            dev.Write(data);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();
            byte a = 0;
            byte[] data = new byte[50];
            byte[] cmdData = new byte[] { 0x05, 0x5A, 0x0D, 0x00, 0x01, 0x0D, 0x05, 0x06, 0x05, 0x5A, 0x05, 0x00, 0x01, 0x0A, 0x00, 0xFF, 0x00 };

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));
            dev.Write(data);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();
            byte a = 0;
            byte[] data = new byte[50];
            byte[] cmdData = new byte[] { 0x05, 0x5A, 0x0D, 0x00, 0x01, 0x0D, 0x05, 0x06, 0x05, 0x5A, 0x05, 0x00, 0x01, 0x0A, 0x00, 0xFF, 0x01 };

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));
            dev.Write(data);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dev.StartAsyncRead();
            byte a = 0;
            byte[] data = new byte[50];
            byte[] cmdData = new byte[] { 0x05, 0x5A, 0x0D, 0x00, 0x01, 0x0D, 0x05, 0x06, 0x05, 0x5A, 0x05, 0x00, 0x01, 0x0A, 0x00, 0xFF, 0x02 };

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));
            dev.Write(data);
        }

#endif
    }

    class backWork2
    {
        // import dll
        [DllImport("felicaDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool fnSetData(byte[] addr, int sizeofAddr, byte[] localName, int sizeofLocalName);

        [DllImport("felicaDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool fnReadData(byte[] addr, int sizeofAddr, byte[] localName, int sizeofLocalName);

        [DllImport("felicaDll.dll", CallingConvention = CallingConvention.Cdecl)] // wsjung.add.170921 : nfc polling before start
        public static extern bool fnPolling();

#if (NFC_LOCK) && (!HDX2914)
        [DllImport("felicaDll.dll", CallingConvention = CallingConvention.Cdecl)] // wsjung.add.171011 : nfc lock
        public static extern bool fnSetLock();
#endif

        Form1 mainForm;

        string bdFromDUT;
        string nameFromDUT;
        string fwVersionInfoFromDUT;
        byte[] bdAddress = new Byte[6];

        string tempNap;
        string tempUap;
        string tempLap;

        byte tempBatPri;
        byte tempBatSec;

        public backWork2(Form1 frm)
        {
            mainForm = frm;
        }

        public void DoWork() // sequence base
        {
            int flag_ng = 0;

#if false
            int Count = 20;
#endif

            mainForm.toggleBtn(false);

#if (ENABLE_CTRL_NFC) // wsjung.add.170921 : nfc polling before start
            fnPolling();
            Thread.Sleep(100);
            fnPolling();
            Thread.Sleep(100);
#endif


#if false // test code
            if(flag_ng != 1)
            {
                Form1.dev.StartAsyncRead();

                byte[] tempBd = new byte[6];
                byte[] data = new byte[19];

                data[0] = 0x02;
                data[1] = 0x0b;
                data[2] = 0x00;

                Form1.dev.Write(data);

                while (Count > 0)
                {
                    Form1.dev.StartAsyncRead();
                    Thread.Sleep(1000);
                    Count--;

                    if (Form1.HeadsetConnected)
                    {
                        Console.WriteLine("Headset Connected");
                        break;
                    }
                }

                if(Count == 0 || !Form1.HeadsetConnected)
                {
                    flag_ng = 1;
                }
            }

            Count = 20;

            if(flag_ng != 1)
            {
                byte[] tempBd = new byte[6];
                byte[] data = new byte[19];

                data[0] = 0x02;
                data[1] = 0x0b;
                data[2] = 0x04;

                Form1.dev.Write(data);

                while (Count > 0)
                {
                    Form1.dev.StartAsyncRead();
                    Thread.Sleep(1000);
                    Count--;

                    if (Form1.CheckBdAddress)
                    {
                        Console.WriteLine("check remote bd address");
                        break;
                    }
                }

                if (Count == 0 || !Form1.CheckBdAddress)
                {
                    flag_ng = 1;
                }
            }

            Count = 20;

#if true
            if (flag_ng != 1)
            {
                byte[] tempBd = new byte[6];
                byte[] data = new byte[19];

                data[0] = 0x02;
                data[1] = 0x0b;
                data[2] = 0x03;

                Form1.dev.Write(data);

                while (Count > 0)
                {
                    Form1.dev.StartAsyncRead();
                    Thread.Sleep(1000);
                    Count--;

                    if (Form1.CheckModelName)
                    {
                        Console.WriteLine("check remote model name");
                        break;
                    }
                }

                if (Count == 0 || !Form1.CheckModelName)
                {
                    flag_ng = 1;
                }
            }
#endif

            Count = 20;

            if (flag_ng != 1)
            {
                byte[] tempBd = new byte[6];
                byte[] data = new byte[19];

                data[0] = 0x02;
                data[1] = 0x0b;
                data[2] = 0x05;

                Form1.dev.Write(data);

                while (Count > 0)
                {
                    Form1.dev.StartAsyncRead();
                    Thread.Sleep(1000);
                    Count--;

                    if (Form1.CheckFwVersion)
                    {
                        Console.WriteLine("check remote fw version");
                        break;
                    }
                }

                if (Count == 0 || !Form1.CheckFwVersion)
                {
                    flag_ng = 1;
                }
            }

            Form1.dev.StartAsyncRead();

            byte[] tempBd1 = new byte[6];
            byte[] data1 = new byte[19];

            data1[0] = 0x02;
            data1[1] = 0x0b;
            data1[2] = 0x02;

            Form1.dev.Write(data1);
#endif

#if true
            // start sequence
            for (int i = 0; i < Form1.gCountSequence; i++ )
            {
                if (procTestSequence(mainForm.gtestSequence[i], i) == false)
                {
                    flag_ng = 1;
                    mainForm.ShowProgress2(i, "FAIL");
                    break;
                }
                else
                {
                    mainForm.ShowProgress2(i, "PASS");
                }
            }
#endif

#if true // disconnect -> factory reset (for ATH-CKS50TW2, when send disconnect command, earbuds enter strange state)
            Form1.dev.StartAsyncRead();

            byte[] data = new byte[50];
            byte[] cmdData = new byte[] { 0x05, 0x5A, 0x03, 0x00, 0x01, 0x00, 0x09 };

            data[0] = 0x02;
            data[1] = 0x0b;
            data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
            data[3] = (byte)Buffer.ByteLength(cmdData);
            Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

            Form1.dev.Write(data);

#endif

#if true
            // show final result
            //mainForm.showResult(flag_ng, Form1.dutFullBdAddress);
            mainForm.showResult(flag_ng, Form1.dutFullBdAddressPri + " / " + Form1.dutFullBdAddressSec);
#endif

#if true
            // write dupe data (pass only)
            if (flag_ng == 0)
            {
                {
                    //mainForm.writeDupeData(Form1.dutFullBdAddress);
                    mainForm.writeDupeData(Form1.dutFullBdAddressPri + " / " + Form1.dutFullBdAddressSec);
                }
            }
#endif

            mainForm.toggleBtn(true);

#if repeat_test
            mainForm.auto_test();
#endif
            //Form1.autotest_count++;            
        }

        private bool validateBd(string xnap, string xuap, string xlap, string type)
        {
            int flag_ng = 0;

            switch (type)
            {
                case "check_default_for_check":
                    {
                        if ((Form1.gDefaultBdNap == xnap) && (Form1.gDefaultBdUap == xuap) && (Form1.gDefaultBdLap == xlap)) { flag_ng = 1; }
                    }
                    break;
                case "check_default_for_write":
                    {
                        if ((Form1.gDefaultBdNap != xnap) || (Form1.gDefaultBdUap != xuap) || (Form1.gDefaultBdLap != xlap)) { flag_ng = 1; }
                    }
                    break;
                case "check_range":
                    {
                        // convert
                        UInt16 tempConfigNap = Convert.ToUInt16(Form1.gBdNap, 16);
                        Byte tempConfigUap = Convert.ToByte(Form1.gBdUap, 16);
                        UInt32 tempConfigLapStart = Convert.ToUInt32(Form1.gBdLapStart, 16);
                        UInt32 tempConfigLapEnd = Convert.ToUInt32(Form1.gBdLapEnd, 16);

                        UInt16 tempNap = Convert.ToUInt16(xnap, 16);
                        Byte tempUap = Convert.ToByte(xuap, 16);
                        UInt32 tempLap = Convert.ToUInt32(xlap, 16);

                        if ((tempLap < tempConfigLapStart) || (tempLap > tempConfigLapEnd) || (tempNap != tempConfigNap) || (tempUap != tempConfigUap)) { flag_ng = 1; }
                    }
                    break;             
                default:
                    {

                    }
                    break;
            }

            if (flag_ng == 1) { return false; }
            else { return true; }
        }

        private bool checkBatLevel(byte batpri, byte batsec)
        {            
            if (batpri < 50 || batsec < 50) { return false; }
            else { return true; }
        }
        private bool checkBD(string bd)
        {
            return true;
        }

        private bool checkName(string name)
        {
            if (name == Form1.gDutName) { return true; }
            else { return false; }
        }
        private bool checkColor(string color)
        {
            if (color == Form1.gFwColor) { return true; }
            else { return false; }
            
        }

        private bool checkSwVer(string swVersion)
        {
            if (swVersion == Form1.gSwVersion) { return true; }
            else { return false; }
        }

        private bool procTestSequence(string seqName, int index)
        {
#if false // sequence list
TEST_CONNECT
CHECK_BD_DEFAULT_FOR_CHECK
CHECK_BD_RANGE_PRI
CHECK_BD_RANGE_SEC
CHECK_BD_DUPE
CHECK_BAT_LEVEL
CHECK_MODEL_NAME
CHECK_FW_VERSION
CHECK_REGION
CHECK_FACTORY_RESET


WRITE_NFC
CHECK_NFC

END
#endif
            bool retVal = false;

            if (seqName == "TEST_CONNECT") { retVal = procTestOpenPort(index); }
            else if (seqName == "CHECK_BD_DEFAULT_FOR_CHECK") { retVal = procTestCheckBdDefaultForCheck(index); }
            else if (seqName == "CHECK_BD_DEFAULT_PRI") { retVal = procTestCheckBdDefaultForCheckPri(index); }
            else if (seqName == "CHECK_BD_DEFAULT_SEC") { retVal = procTestCheckBdDefaultForCheckSec(index); }
            else if (seqName == "CHECK_BD_RANGE_PRI") { retVal = procTestCheckBdRangePri(index); }
            else if (seqName == "CHECK_BD_RANGE_SEC") { retVal = procTestCheckBdRangeSec(index); }
            else if (seqName == "CHECK_BAT_LEVEL") { retVal = procTestCheckBatLevel(index); }
            else if (seqName == "CHECK_MODEL_NAME") { retVal = procTestCheckModelName(index); }
            else if (seqName == "CHECK_MODEL_NAME_PRI") { retVal = procTestCheckModelNamePri(index); }
            else if (seqName == "CHECK_MODEL_NAME_SEC") { retVal = procTestCheckModelNameSec(index); }
            else if (seqName == "CHECK_FW_VERSION") { retVal = procTestCheckFwVersion(index); }
            else if (seqName == "CHECK_FW_VERSION_PRI") { retVal = procTestCheckFwVersionPri(index); }
            else if (seqName == "CHECK_FW_VERSION_SEC") { retVal = procTestCheckFwVersionSec(index); }
            else if (seqName == "CHECK_ANC_WRITE_PRI") { retVal = procTestCheckAncWritePri(index);}
            else if (seqName == "CHECK_ANC_WRITE_SEC") { retVal = procTestCheckAncWriteSec(index); }
            else if (seqName == "WRITE_NFC") { retVal = procTestWriteNfc(index); }
            else if (seqName == "CHECK_NFC") { retVal = procTestReadNfc(index); }
            else if (seqName == "CHECK_BD_DUPE") { retVal = procTestCheckDupe(index); }
            else if (seqName == "CHECK_FACTORY_RESET") { retVal = procTestCheckFactoryReset(index); }
            else if (seqName == "SHIPPING_POWEROFF_PRI") { retVal = procTestShippingPoweroffPri(index); }
            else if (seqName == "SHIPPING_POWEROFF_SEC") { retVal = procTestShippingPoweroffSec(index); }
            else if (seqName == "CHECK_REGION") { retVal = procTestCheckRegion(index); } // wsjung.add.170405
            else if (seqName == "WRITE_COLOR_PRI") { retVal = procTestWriteColorPri(index); }
            else if (seqName == "WRITE_COLOR_SEC") { retVal = procTestWriteColorSec(index); }
            else if (seqName == "CHECK_COLOR_PRI") { retVal = procTestCheckColorPri(index); }
            else if (seqName == "CHECK_COLOR_SEC") { retVal = procTestCheckColorSec(index); }
            else if (seqName == "CHECK_AVA_DEST") { retVal = procTestCheckAvaDest(index); }

            return retVal;
        }
        
        private bool procTestWriteColorPri(int index)
        {
            int flag_ng = 0;
            int Count = 20;

            try
            {
                Form1.dev.StartAsyncRead();

                byte a = 0;
                byte[] data = new byte[50];
                if(Form1.gFwColor == "BK")
                {
                    byte[] cmdData = new byte[] { 0x05, 0x5A, 0x05, 0x00, 0x01, 0x0A, 0x00, 0xFF, 0x00 };
                    

                    data[0] = 0x02;
                    data[1] = 0x0b;
                    data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
                    data[3] = (byte)Buffer.ByteLength(cmdData);
                    Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

                }
                else if (Form1.gFwColor == "GR")
                {
                    byte[] cmdData = new byte[] { 0x05, 0x5A, 0x05, 0x00, 0x01, 0x0A, 0x00, 0xFF, 0x01 };
                    
                    data[0] = 0x02;
                    data[1] = 0x0b;
                    data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
                    data[3] = (byte)Buffer.ByteLength(cmdData);
                    Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

                }
                else if (Form1.gFwColor == "BG")
                {
                    byte[] cmdData = new byte[] { 0x05, 0x5A, 0x05, 0x00, 0x01, 0x0A, 0x00, 0xFF, 0x02 };
                    
                    data[0] = 0x02;
                    data[1] = 0x0b;
                    data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
                    data[3] = (byte)Buffer.ByteLength(cmdData);
                    Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

                }

                //byte[] cmdData = new byte[] { 0x05, 0x5A, 0x03, 0x00, 0x01, 0x00, 0x10 };

                Form1.dev.Write(data);

                while (Count > 0)
                {
                    mainForm.ShowProgressIndicator(index, true);

                    Form1.dev.StartAsyncRead();
                    Thread.Sleep(1000);
                    Count--;

                    if (Form1.WriteFwColorPri)
                    {
                        Console.WriteLine("WriteFwColorPri");
                        break;
                    }
                }
            }
            //catch (System.Exception ex)
            catch
            {
                flag_ng = 1;
            }
            finally
            {
                if (Count == 0 || !Form1.WriteFwColorPri)
                {
                    flag_ng = 1;
                }

                if (flag_ng != 1)
                {
                   //
                }
            }

            mainForm.ShowProgressIndicator(index, false);

            if (flag_ng == 1)
            {
                mainForm.gNgType[index] = "Write Color Pri Fail!";
                return false;
            }
            else
            {
                mainForm.gTestValue[index] = Form1.gFwColor;
                return true;
            }

        }
        private bool procTestWriteColorSec(int index)
        {
            int flag_ng = 0;
            int Count = 20;

            try
            {
                Form1.dev.StartAsyncRead();

                byte a = 0;
                byte[] data = new byte[50];
                if (Form1.gFwColor == "BK")
                {                    
                    byte[] cmdData = new byte[] { 0x05, 0x5A, 0x0D, 0x00, 0x01, 0x0D, 0x05, 0x06, 0x05, 0x5A, 0x05, 0x00, 0x01, 0x0A, 0x00, 0xFF, 0x00 };

                    data[0] = 0x02;
                    data[1] = 0x0b;
                    data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
                    data[3] = (byte)Buffer.ByteLength(cmdData);
                    Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

                }
                else if (Form1.gFwColor == "GR")
                {                    
                    byte[] cmdData = new byte[] { 0x05, 0x5A, 0x0D, 0x00, 0x01, 0x0D, 0x05, 0x06, 0x05, 0x5A, 0x05, 0x00, 0x01, 0x0A, 0x00, 0xFF, 0x01 };
                    data[0] = 0x02;
                    data[1] = 0x0b;
                    data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
                    data[3] = (byte)Buffer.ByteLength(cmdData);
                    Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

                }
                else if (Form1.gFwColor == "BG")
                {
                    
                    byte[] cmdData = new byte[] { 0x05, 0x5A, 0x0D, 0x00, 0x01, 0x0D, 0x05, 0x06, 0x05, 0x5A, 0x05, 0x00, 0x01, 0x0A, 0x00, 0xFF, 0x02 };
                    data[0] = 0x02;
                    data[1] = 0x0b;
                    data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
                    data[3] = (byte)Buffer.ByteLength(cmdData);
                    Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

                }

                //byte[] cmdData = new byte[] { 0x05, 0x5A, 0x03, 0x00, 0x01, 0x00, 0x10 };

                Form1.dev.Write(data);

                while (Count > 0)
                {
                    mainForm.ShowProgressIndicator(index, true);

                    Form1.dev.StartAsyncRead();
                    Thread.Sleep(1000);
                    Count--;

                    if (Form1.WriteFwColorSec)
                    {
                        Console.WriteLine("WriteFwColorSec");
                        break;
                    }
                }
            }
            //catch (System.Exception ex)
            catch
            {
                flag_ng = 1;
            }
            finally
            {
                if (Count == 0 || !Form1.WriteFwColorSec)
                {
                    flag_ng = 1;
                }

                if (flag_ng != 1)
                {
                    //
                }
            }

            mainForm.ShowProgressIndicator(index, false);

            if (flag_ng == 1)
            {
                mainForm.gNgType[index] = "Write Color Sec Fail!";
                return false;
            }
            else
            {
                mainForm.gTestValue[index] = Form1.gFwColor;
                return true;
            }
        }

        private bool procTestCheckColorPri(int index)
        {
            int flag_ng = 0;
            int Count = 20;

            try
            {
                Form1.dev.StartAsyncRead();

                byte a = 0;
                byte[] data = new byte[50];

                byte[] cmdData = new byte[] { 0x05, 0x5A, 0x06, 0x00, 0x00, 0x0A, 0x00, 0xFF, 0x01, 0x00 };

                data[0] = 0x02;
                data[1] = 0x0b;
                data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
                data[3] = (byte)Buffer.ByteLength(cmdData);
                Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

                Form1.dev.Write(data);

                while (Count > 0)
                {
                    mainForm.ShowProgressIndicator(index, true);

                    Form1.dev.StartAsyncRead();
                    Thread.Sleep(1000);
                    Count--;

                    if (Form1.CheckFwColorPri)
                    {
                        Console.WriteLine("CheckFwColorPri");
                        break;
                    }
                }
            }
            //catch (System.Exception ex)
            catch
            {
                flag_ng = 1;
            }
            finally
            {
                if (Count == 0 || !Form1.CheckFwColorPri)
                {
                    flag_ng = 1;
                }

                if (flag_ng != 1)
                {
                    mainForm.gTestValue[index] = Form1.dutFwColorPri;
                    if (!checkColor(Form1.dutFwColorPri)) { flag_ng = 1; }
                }
            }

            mainForm.ShowProgressIndicator(index, false);

            if (flag_ng == 1)
            {
                mainForm.gNgType[index] = "Check FW Color Primary fail!";
                return false;
            }
            else
            {
                return true;
            }

        }
        private bool procTestCheckColorSec(int index)
        {
            int flag_ng = 0;
            int Count = 20;

            try
            {
                Form1.dev.StartAsyncRead();

                byte a = 0;
                byte[] data = new byte[50];

                byte[] cmdData = new byte[] { 0x05, 0x5A, 0x0E, 0x00, 0x01, 0x0D, 0x05, 0x06, 0x05, 0x5A, 0x06, 0x00, 0x00, 0x0A, 0x00, 0xFF, 0x01, 0x00 };

                data[0] = 0x02;
                data[1] = 0x0b;
                data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
                data[3] = (byte)Buffer.ByteLength(cmdData);
                Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

                Form1.dev.Write(data);

                while (Count > 0)
                {
                    mainForm.ShowProgressIndicator(index, true);

                    Form1.dev.StartAsyncRead();
                    Thread.Sleep(1000);
                    Count--;

                    if (Form1.CheckFwColorPri)
                    {
                        Console.WriteLine("CheckFwColorSec");
                        break;
                    }
                }
            }
            //catch (System.Exception ex)
            catch
            {
                flag_ng = 1;
            }
            finally
            {
                if (Count == 0 || !Form1.CheckFwColorPri)
                {
                    flag_ng = 1;
                }

                if (flag_ng != 1)
                {
                    mainForm.gTestValue[index] = Form1.dutFwColorSec;
                    if (!checkColor(Form1.dutFwColorSec)) { flag_ng = 1; }
                }
            }

            mainForm.ShowProgressIndicator(index, false);

            if (flag_ng == 1)
            {
                mainForm.gNgType[index] = "Check Fw Color Secondary fail!";
                return false;
            }
            else
            {
                return true;
            }

        }

        private bool procTestCheckAvaDest(int index)
        {
            int flag_ng = 0;
            int Count = 20;

            try
            {
                Form1.dev.StartAsyncRead();

                byte[] data = new byte[50];
                byte[] cmdData = new byte[] { 0x05, 0x5A, 0x02, 0x00, 0x00, 0x0D };

                data[0] = 0x02;
                data[1] = 0x0b;
                data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
                data[3] = (byte)Buffer.ByteLength(cmdData);
                Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

                Form1.dev.Write(data);

                while (Count > 0)
                {
                    mainForm.ShowProgressIndicator(index, true);

                    Form1.dev.StartAsyncRead();
                    Thread.Sleep(1000);
                    Count--;

                    if (Form1.CheckAvaDest)
                    {
                        Console.WriteLine("CheckAvaDest");
                        break;
                    }
                }
            }
            //catch (System.Exception ex)
            catch
            {
                flag_ng = 1;
            }
            finally
            {
                if (Count == 0 || !Form1.CheckAvaDest)
                {
                    flag_ng = 1;
                }

                if (flag_ng != 1)
                {
                    /*
                    mainForm.gTestValue[index] = Form1.dutModelName;
                    if (!checkName(Form1.dutModelName)) { flag_ng = 1; }
                     */
                }
            }

            mainForm.ShowProgressIndicator(index, false);

            if (flag_ng == 1)
            {
                mainForm.gNgType[index] = "Get Available Destination fail!";
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool procTestCheckFactoryReset(int index) // check factory reset
        {
            int flag_ng = 0;
            int Count = 20;

            try
            {
                Form1.dev.StartAsyncRead();

                byte[] data = new byte[50];
                byte[] cmdData = new byte[] { 0x05, 0x5A, 0x03, 0x00, 0x01, 0x00, 0x09 };

                data[0] = 0x02;
                data[1] = 0x0b;
                data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
                data[3] = (byte)Buffer.ByteLength(cmdData);
                Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

                Form1.dev.Write(data);

                while (Count > 0)
                {
                    mainForm.ShowProgressIndicator(index, true);

                    Form1.dev.StartAsyncRead();
                    Thread.Sleep(1000);
                    Count--;

                    if (Form1.CheckFactoryReset)
                    {
                        Console.WriteLine("CheckFactoryReset");
                        break;
                    }
                }
            }
            //catch (System.Exception ex)
            catch
            {
                flag_ng = 1;
            }
            finally
            {
                if (Count == 0 || !Form1.CheckFactoryReset)
                {
                    flag_ng = 1;
                }

                if (flag_ng != 1)
                {
                   // if (Form1.dutFactoryReset == 0x01) { flag_ng = 1; }
                }
            }

            mainForm.ShowProgressIndicator(index, false);

            if (flag_ng == 1)
            {
                mainForm.gNgType[index] = "Factory reset check fail!";
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool procTestCheckDupe(int index) // check bd dupe
        {
            int flag_ng = 0;

            try
            {
                // set test value (nothing)
                mainForm.gTestValue[index] = "-";

                // 01. bd check (default bd for write)
            }
            catch
            {
                flag_ng = 1;
            }
            finally
            {
                if (mainForm.findDupeData(Form1.dutFullBdAddressPri + " / " + Form1.dutFullBdAddressSec)) { flag_ng = 1; }
                else if (mainForm.findDupeData(Form1.dutFullBdAddressSec + " / " + Form1.dutFullBdAddressPri)) { flag_ng = 1; }
            }

            if (flag_ng == 1)
            {
                mainForm.gNgType[index] = "BD Address Dupe";
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool procTestOpenPort(int index) // check connection
        {
            int flag_ng = 0;
            int Count = 20;

            try
            {
                Form1.dev.StartAsyncRead();

                byte[] data = new byte[19];

                data[0] = 0x02;
                data[1] = 0x0b;
                data[2] = 0x00;

                Form1.dev.Write(data);

                while (Count > 0)
                {
                    mainForm.ShowProgressIndicator(index, true);

                    Form1.dev.StartAsyncRead();
                    Thread.Sleep(1000);
                    Count--;

                    if (Form1.HeadsetConnected)
                    {
                        Console.WriteLine("Headset Connected");
                        break;
                    }
                }
            }
            //catch (System.Exception ex)
            catch
            {
                flag_ng = 1;
            }
            finally
            {
                if (Count == 0 || !Form1.HeadsetConnected)
                {
                    flag_ng = 1;
                }
            }

            mainForm.ShowProgressIndicator(index, false);

            if (flag_ng == 1)
            {
                mainForm.gNgType[index] = "Connection Fail";
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool procTestWriteNfc(int index) // write nfc
        {
            // make data
            byte[] bdAddress = new Byte[6];
            //byte[] localName = Encoding.UTF8.GetBytes(nameFromDUT); // wsjung.del.170509 : for EPP (no need version)
            //byte[] localName = Encoding.UTF8.GetBytes("HDX-2888"); // wsjung.add.170509 : for EPP (model name only : HDX-2888)

#if (HDX2910)
            byte[] localName = Encoding.UTF8.GetBytes("WH-CH400"); // wsjung.edit.170724 : for HDX-2910 ET1
#endif
#if (HDX2914)
            byte[] localName = Encoding.UTF8.GetBytes("WI-C300"); // wsjung.edit.170724 : for HDX-2914 ET1
#endif
#if (HDX2918)
            byte[] localName = Encoding.UTF8.GetBytes("WH-CH500"); // wsjung.edit.170724 : for HDX-2918 ET1
#endif

#if (HDX2936)
            byte[] localName = Encoding.UTF8.GetBytes("HDX-2936");
#endif

            string a111 = bdFromDUT.Substring(0, 2);
            string a222 = bdFromDUT.Substring(2, 2);
            string a333 = bdFromDUT.Substring(4, 2);
            string a444 = bdFromDUT.Substring(6, 2);
            string a555 = bdFromDUT.Substring(8, 2);
            string a666 = bdFromDUT.Substring(10, 2);

            // re-arrange bd address
            bdAddress[0] = Convert.ToByte(a666, 16);
            bdAddress[1] = Convert.ToByte(a555, 16);
            bdAddress[2] = Convert.ToByte(a444, 16);
            bdAddress[3] = Convert.ToByte(a333, 16);
            bdAddress[4] = Convert.ToByte(a222, 16);
            bdAddress[5] = Convert.ToByte(a111, 16);

            // set test value (nothing)
            mainForm.gTestValue[index] = "-";

            if (!fnSetData(bdAddress, bdAddress.Length, localName, localName.Length))
            {
                mainForm.gNgType[index] = "Write NFC Fail";
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool procTestReadNfc(int index) // read nfc
        {
#if (NFC_LOCK) && (!HDX2914)
            int flag_ng = 0; // wsjung.add.171011
#endif

            // make data
            byte[] bdAddress = new Byte[6];
            //byte[] localName = Encoding.UTF8.GetBytes(nameFromDUT); // wsjung.del.170509 : for EPP (no need version)
            //byte[] localName = Encoding.UTF8.GetBytes("HDX-2888"); // wsjung.add.170509 : for EPP (model name only : HDX-2888)

#if (HDX2910)
            byte[] localName = Encoding.UTF8.GetBytes("WH-CH400"); // wsjung.edit.170724 : for HDX-2910 ET1
#endif
#if (HDX2914)
            byte[] localName = Encoding.UTF8.GetBytes("WI-C300"); // wsjung.edit.170724 : for HDX-2914 ET1
#endif
#if (HDX2918)
            byte[] localName = Encoding.UTF8.GetBytes("WH-CH500"); // wsjung.edit.170724 : for HDX-2918 ET1
#endif

#if (HDX2936)
            byte[] localName = Encoding.UTF8.GetBytes("HDX-2936");
#endif

            string a111 = bdFromDUT.Substring(0, 2);
            string a222 = bdFromDUT.Substring(2, 2);
            string a333 = bdFromDUT.Substring(4, 2);
            string a444 = bdFromDUT.Substring(6, 2);
            string a555 = bdFromDUT.Substring(8, 2);
            string a666 = bdFromDUT.Substring(10, 2);

            // re-arrange bd address
            bdAddress[0] = Convert.ToByte(a666, 16);
            bdAddress[1] = Convert.ToByte(a555, 16);
            bdAddress[2] = Convert.ToByte(a444, 16);
            bdAddress[3] = Convert.ToByte(a333, 16);
            bdAddress[4] = Convert.ToByte(a222, 16);
            bdAddress[5] = Convert.ToByte(a111, 16);

            // set test value (nothing)
            mainForm.gTestValue[index] = "-";

#if (NFC_LOCK) && (!HDX2914) // wsjung.edit.171011 : nfc lock
            if (!fnReadData(bdAddress, bdAddress.Length, localName, localName.Length))
            {
                mainForm.gNgType[index] = "Read NFC Fail";
                //return false; // wsjung.del.171011 : removed
                flag_ng = 1; // wsjung.del.171011 : added
            }

            if (flag_ng != 1) // wsjung.add.171011 : nfc lock
            {
                // lock nfc
                if (!fnSetLock())
                {
                    mainForm.gNgType[index] = "Lock NFC Fail";
                    flag_ng = 1; // wsjung.del.171011 : added
                }
            }

            if (flag_ng == 0) { return true; }
            else { return false; }
#else
            if (!fnReadData(bdAddress, bdAddress.Length, localName, localName.Length))
            {
                mainForm.gNgType[index] = "Read NFC Fail";
                return false; // wsjung.del.171011 : removed
            }
            else
            {
                return true;
            }
#endif
        }

        private bool procTestCheckBdDefaultForCheck(int index) // check default bd (check)
        {
            int flag_ng = 0;
            int Count = 20;

            try
            {
                Form1.dev.StartAsyncRead();

                byte[] data = new byte[19];

                data[0] = 0x02;
                data[1] = 0x0b;
                data[2] = Form1.USB_HOST_DATA_CMD_GET_REMOTE_BD_ADDRESS;

                Form1.dev.Write(data);

                while (Count > 0)
                {
                    mainForm.ShowProgressIndicator(index, true);

                    Form1.dev.StartAsyncRead();
                    Thread.Sleep(1000);
                    Count--;

                    if (Form1.CheckBdAddress)
                    {
                        Console.WriteLine("CheckBdAddress");
                        break;
                    }
                }
            }
            catch
            {
                flag_ng = 1;
            }
            finally
            {
                if (Count == 0 || !Form1.CheckBdAddress)
                {
                    flag_ng = 1;
                }

                if(flag_ng != 1)
                {
                    tempNap = Form1.dutBdNap;
                    tempUap = Form1.dutBdUap;
                    tempLap = Form1.dutBdLap;

                    // set test value (nothing)
                    mainForm.gTestValue[index] = Form1.dutFullBdAddress;

                    if (!validateBd(tempNap, tempUap, tempLap, "check_default_for_check")) { flag_ng = 1; }
                }
            }

            mainForm.ShowProgressIndicator(index, false);

            if (flag_ng == 1)
            {
                mainForm.gNgType[index] = "Check Default BD(check) Fail";
                return false;
            }
            else
            {
                return true;
            }

        }
#if ATH_CKS30TW || ATH_CKS50TW2
        private bool procTestCheckBdRangePri(int index) // check bd range (against config)
        {
            int flag_ng = 0;
            int Count = 20;

            try
            {
                // split bd string
                tempNap = Form1.dutBdNapPri;
                tempUap = Form1.dutBdUapPri;
                tempLap = Form1.dutBdLapPri;

                // set test value (nothing)
                mainForm.gTestValue[index] = Form1.dutFullBdAddressPri;
            }
            catch
            {
                flag_ng = 1;
            }
            finally
            {

                // split bd string
                tempNap = Form1.dutBdNapPri;
                tempUap = Form1.dutBdUapPri;
                tempLap = Form1.dutBdLapPri;

                // set test value (nothing)
                mainForm.gTestValue[index] = Form1.dutFullBdAddressPri;

                if (!validateBd(tempNap, tempUap, tempLap, "check_range")) { flag_ng = 1; }
            }

            if (flag_ng == 1)
            {
                mainForm.gNgType[index] = "Check BD Range Fail";
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool procTestCheckBdRangeSec(int index) // check bd range (against config)
        {            

            int flag_ng = 0;

            try
            {
                // split bd string
                tempNap = Form1.dutBdNapSec;
                tempUap = Form1.dutBdUapSec;
                tempLap = Form1.dutBdLapSec;

                // set test value (nothing)
                mainForm.gTestValue[index] = Form1.dutFullBdAddressSec;
            }
            catch
            {
                flag_ng = 1;
            }
            finally
            {
                if (!validateBd(tempNap, tempUap, tempLap, "check_range")) { flag_ng = 1; }
            }

            if (flag_ng == 1)
            {
                mainForm.gNgType[index] = "Check BD Range Fail";
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool procTestCheckBdDefaultForCheckPri(int index) // check bd range (against config)
        {
            Form1.flag_CheckDefaultBDPri = true;

            int flag_ng = 0;
            int Count = 20;

            try
            {
                Form1.dev.StartAsyncRead();
                byte a = 0;
                byte[] data = new byte[50];
                //byte[] cmdData = new byte[] { 0xAA, a, 0x04, 0x00, 0x00, 0x08, 0x00, 0x9B };
                byte[] cmdData = new byte[] { 0x05, 0x5A, 0x03, 0x00, 0x01, 0x00, 0x07 };

                data[0] = 0x02;
                data[1] = 0x0b;
                data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
                data[3] = (byte)Buffer.ByteLength(cmdData);
                Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

                Form1.dev.Write(data);

                while (Count > 0)
                {
                    mainForm.ShowProgressIndicator(index, true);

                    Form1.dev.StartAsyncRead();
                    Thread.Sleep(1000);
                    Count--;

                    if (Form1.CheckBdAddress)
                    {
                        Console.WriteLine("CheckBdAddress");
                        break;
                    }
                }

            }
            catch
            {
                flag_ng = 1;
            }
            finally
            {

                // split bd string
                tempNap = Form1.dutBdNapPri;
                tempUap = Form1.dutBdUapPri;
                tempLap = Form1.dutBdLapPri;

                // set test value (nothing)
                mainForm.gTestValue[index] = Form1.dutFullBdAddressPri;

                if (!validateBd(tempNap, tempUap, tempLap, "check_default_for_check")) { flag_ng = 1; }
            }

            if (flag_ng == 1)
            {
                mainForm.gNgType[index] = "check_default_for_check Pri Fail";
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool procTestCheckBdDefaultForCheckSec(int index) // check bd range (against config)
        {
            Form1.flag_CheckDefaultBDSec = true;

            int flag_ng = 0;
            
            try
            {
                // split bd string
                tempNap = Form1.dutBdNapSec;
                tempUap = Form1.dutBdUapSec;
                tempLap = Form1.dutBdLapSec;

                // set test value (nothing)
                mainForm.gTestValue[index] = Form1.dutFullBdAddressSec;
            }
            catch
            {
                flag_ng = 1;
            }
            finally
            {
                if (!validateBd(tempNap, tempUap, tempLap, "check_default_for_check")) { flag_ng = 1; }
            }

            if (flag_ng == 1)
            {
                mainForm.gNgType[index] = "check_default_for_check Sec Fail";
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool procTestCheckBatLevel(int index)
        {
            int flag_ng = 0;
            int Count = 20;

            try
            {
                Form1.dev.StartAsyncRead();
                byte a = 0;
                byte[] data = new byte[50];
                byte[] cmdData = new byte[] { 0x05, 0x5A, 0x03, 0x00, 0x01, 0x00, 0x05 };

                data[0] = 0x02;
                data[1] = 0x0b;
                data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
                data[3] = (byte)Buffer.ByteLength(cmdData);
                Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

                Form1.dev.Write(data);

                while (Count > 0)
                {
                    mainForm.ShowProgressIndicator(index, true);

                    Form1.dev.StartAsyncRead();
                    Thread.Sleep(1000);
                    Count--;

                    if (Form1.CheckBatLevel)
                    {
                        Console.WriteLine("CheckBatLevel");
                        break;
                    }
                }

            }
            catch
            {
                flag_ng = 1;
            }
            finally
            {                
                tempBatPri = Form1.batLevelPri;
                tempBatSec = Form1.batLevelSec;                

                // set test value (nothing)
                mainForm.gTestValue[index] = "Pri : " + Convert.ToString(Form1.batLevelPri) + " / " + "Sec : " + Convert.ToString(Form1.batLevelSec);

                if (!checkBatLevel(tempBatPri, tempBatSec) ){ flag_ng = 1; }
            }

            if (flag_ng == 1)
            {
                mainForm.gNgType[index] = "Check BD Range Fail";
                return false;
            }
            else
            {
                return true;
            }
        }
#else
         private bool procTestCheckBdRange(int index) // check bd range (against config)
        {
            int flag_ng = 0;

            try
            {
                // split bd string
                tempNap = Form1.dutBdNap;
                tempUap = Form1.dutBdUap;
                tempLap = Form1.dutBdLap;

                // set test value (nothing)
                mainForm.gTestValue[index] = Form1.dutFullBdAddress;
            }
            catch
            {
                flag_ng = 1;
            }
            finally
            {
                if (!validateBd(tempNap, tempUap, tempLap, "check_range")) { flag_ng = 1; }
            }

            if (flag_ng == 1)
            {
                mainForm.gNgType[index] = "Check BD Range Fail";
                return false;
            }
            else
            {
                return true;
            }
        }

#endif


        private bool procTestCheckModelName(int index) // check model name
        {
            int flag_ng = 0;
            int Count = 20;

            try
            {
                Form1.dev.StartAsyncRead();

                byte[] data = new byte[19];

                data[0] = 0x02;
                data[1] = 0x0b;
                data[2] = Form1.USB_HOST_DATA_CMD_GET_REMOTE_NAME;

                Form1.dev.Write(data);

                while (Count > 0)
                {
                    mainForm.ShowProgressIndicator(index, true);

                    Form1.dev.StartAsyncRead();
                    Thread.Sleep(1000);
                    Count--;

                    if (Form1.CheckModelName)
                    {
                        Console.WriteLine("CheckModelName");
                        break;
                    }
                }
            }
            //catch (System.Exception ex)
            catch
            {
                flag_ng = 1;
            }
            finally
            {
                if (Count == 0 || !Form1.CheckModelName)
                {
                    flag_ng = 1;
                }

                if(flag_ng != 1)
                {
                    mainForm.gTestValue[index] = Form1.dutModelName;
                    if (!checkName(Form1.dutModelName)) { flag_ng = 1; }
                }
            }

            mainForm.ShowProgressIndicator(index, false);

            if (flag_ng == 1)
            {
                mainForm.gNgType[index] = "Model name fail!";
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool procTestCheckModelNamePri(int index) // check model name primary
        {

            int flag_ng = 0;
            int Count = 20;
            
            try
            {
                Form1.dev.StartAsyncRead();

                byte a = 0;
                byte[] data = new byte[50];
                byte[] cmdData = new byte[] { 0x05, 0x5A, 0x03, 0x00, 0x01, 0x00, 0x10 };

                data[0] = 0x02;
                data[1] = 0x0b;
                data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
                data[3] = (byte)Buffer.ByteLength(cmdData);
                Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));

                Form1.dev.Write(data);

                while (Count > 0)
                {
                    mainForm.ShowProgressIndicator(index, true);

                    Form1.dev.StartAsyncRead();
                    Thread.Sleep(1000);
                    Count--;

                    if (Form1.CheckModelNamePri)
                    {
                        Console.WriteLine("CheckModelNamePri");
                        break;
                    }
                }
            }
            //catch (System.Exception ex)
            catch
            {
                flag_ng = 1;
            }
            finally
            {
                if (Count == 0 || !Form1.CheckModelNamePri)
                {
                    flag_ng = 1;
                }

                if (flag_ng != 1)
                {
                    mainForm.gTestValue[index] = Form1.dutModelNamePri;
                    if (!checkName(Form1.dutModelNamePri)) { flag_ng = 1; }
                }
            }

            mainForm.ShowProgressIndicator(index, false);

            if (flag_ng == 1)
            {
                mainForm.gNgType[index] = "Check Model name Primary fail!";
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool procTestCheckModelNameSec(int index) // check model name secondary
        {

            int flag_ng = 0;
            int Count = 20;

            try
            {
                Form1.dev.StartAsyncRead();

                byte a = 0;
                byte[] data = new byte[50];
                byte[] cmdData = new byte[] { 0x05, 0x5A, 0x0B, 0x00, 0x01, 0x0D, 0x05, 0xFF, 0x05, 0x5A, 0x03, 0x00, 0x01, 0x00, 0x10};

                data[0] = 0x02;
                data[1] = 0x0b;
                data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
                data[3] = (byte)Buffer.ByteLength(cmdData);
                Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));
                Form1.dev.Write(data);

                while (Count > 0)
                {
                    mainForm.ShowProgressIndicator(index, true);

                    Form1.dev.StartAsyncRead();
                    Thread.Sleep(1000);
                    Count--;

                    if (Form1.CheckModelNameSec)
                    {
                        Console.WriteLine("CheckModelNameSec");
                        break;
                    }
                }
            }
            //catch (System.Exception ex)
            catch
            {
                flag_ng = 1;
            }
            finally
            {
                if (Count == 0 || !Form1.CheckModelNameSec)
                {
                    flag_ng = 1;
                }

                if (flag_ng != 1)
                {
                    mainForm.gTestValue[index] = Form1.dutModelNameSec;
                    if (!checkName(Form1.dutModelNameSec)) { flag_ng = 1; }
                }
            }

            mainForm.ShowProgressIndicator(index, false);

            if (flag_ng == 1)
            {
                mainForm.gNgType[index] = "Check Model name Secondary fail!";
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool procTestCheckFwVersion(int index) // check fw version
        {
            //Thread.Sleep(5000);

            int flag_ng = 0;
            int Count = 20;

            try
            {
                Form1.dev.StartAsyncRead();

                byte a = 0;
                byte[] data = new byte[50];
                byte[] cmdData = new byte[] { 0x05, 0x5A, 0x03, 0x00, 0x01, 0x00, 0x04 };

                data[0] = 0x02;
                data[1] = 0x0b;
                data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
                data[3] = (byte)Buffer.ByteLength(cmdData);
                Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));
                Form1.dev.Write(data);

                while (Count > 0)
                {
                    mainForm.ShowProgressIndicator(index, true);

                    Form1.dev.StartAsyncRead();
                    Thread.Sleep(1000);
                    Count--;

                    if (Form1.CheckFwVersion)
                    {
                        Console.WriteLine("CheckFwVersion");
                        break;
                    }
                }
            }
            //catch (System.Exception ex)
            catch
            {
                flag_ng = 1;
            }
            finally
            {
                if (Count == 0 || !Form1.CheckFwVersion)
                {
                    flag_ng = 1;
                }

                if (flag_ng != 1)
                {
                    mainForm.gTestValue[index] = Form1.dutFullVersion;
                    if (!checkSwVer(Form1.dutFullVersion)) { flag_ng = 1; }
                }
            }

            mainForm.ShowProgressIndicator(index, false);

            if (flag_ng == 1)
            {
                mainForm.gNgType[index] = "FW Version fail!";
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool procTestCheckFwVersionPri(int index) // check fw version
        {
            //Thread.Sleep(5000);

            int flag_ng = 0;
            int Count = 20;

            try
            {
                Form1.dev.StartAsyncRead();

                byte a = 0;
                byte[] data = new byte[50];
                byte[] cmdData = new byte[] { 0x05, 0x5A, 0x03, 0x00, 0x01, 0x00, 0x04 };

                data[0] = 0x02;
                data[1] = 0x0b;
                data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
                data[3] = (byte)Buffer.ByteLength(cmdData);
                Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));
                Form1.dev.Write(data);

                while (Count > 0)
                {
                    mainForm.ShowProgressIndicator(index, true);

                    Form1.dev.StartAsyncRead();
                    Thread.Sleep(1000);
                    Count--;

                    if (Form1.CheckFwVersionPri)
                    {
                        Console.WriteLine("CheckFwVersionPri");
                        break;
                    }
                }
            }
            //catch (System.Exception ex)
            catch
            {
                flag_ng = 1;
            }
            finally
            {
                if (Count == 0 || !Form1.CheckFwVersionPri)
                {
                    flag_ng = 1;
                }

                if (flag_ng != 1)
                {
                    mainForm.gTestValue[index] = Form1.dutFullVersionPri;
                    if (!checkSwVer(Form1.dutFullVersionPri)) { flag_ng = 1; }
                }
            }

            mainForm.ShowProgressIndicator(index, false);

            if (flag_ng == 1)
            {
                mainForm.gNgType[index] = "FW Version Pri fail!";
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool procTestCheckFwVersionSec(int index) // check fw version
        {
            //Thread.Sleep(5000);

            int flag_ng = 0;
            int Count = 20;

            try
            {
                Form1.dev.StartAsyncRead();

                byte a = 0;
                byte[] data = new byte[50];
                byte[] cmdData = new byte[] { 0x05, 0x5A, 0x03, 0x00, 0x01, 0x00, 0x04 };

                data[0] = 0x02;
                data[1] = 0x0b;
                data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
                data[3] = (byte)Buffer.ByteLength(cmdData);
                Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));
                Form1.dev.Write(data);

                while (Count > 0)
                {
                    mainForm.ShowProgressIndicator(index, true);

                    Form1.dev.StartAsyncRead();
                    Thread.Sleep(1000);
                    Count--;

                    if (Form1.CheckFwVersionSec)
                    {
                        Console.WriteLine("CheckFwVersionSec");
                        break;
                    }
                }
            }
            //catch (System.Exception ex)
            catch
            {
                flag_ng = 1;
            }
            finally
            {
                if (Count == 0 || !Form1.CheckFwVersionSec)
                {
                    flag_ng = 1;
                }

                if (flag_ng != 1)
                {
                    mainForm.gTestValue[index] = Form1.dutFullVersionSec;
                    if (!checkSwVer(Form1.dutFullVersionSec)) { flag_ng = 1; }
                }
            }

            mainForm.ShowProgressIndicator(index, false);

            if (flag_ng == 1)
            {
                mainForm.gNgType[index] = "FW Version fail!";
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool procTestCheckAncWritePri(int index) // check ANC parameter write
        {
            //Thread.Sleep(5000);

            int flag_ng = 0;
            int Count = 20;

            try
            {
                Form1.dev.StartAsyncRead();

                byte a = 0;
                byte[] data = new byte[50];
                byte[] cmdData = new byte[] { 0x05, 0x5A, 0x04, 0x00, 0x06, 0x0E, 0x00, 0x0D };

                data[0] = 0x02;
                data[1] = 0x0b;
                data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
                data[3] = (byte)Buffer.ByteLength(cmdData);
                Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));
                Form1.dev.Write(data);

                while (Count > 0)
                {
                    mainForm.ShowProgressIndicator(index, true);

                    Form1.dev.StartAsyncRead();
                    Thread.Sleep(1000);
                    Count--;

                    if (Form1.CheckAncWritePri)
                    {
                        Console.WriteLine("CheckAncWritePri OK");
                        break;
                    }
                    else if ((!Form1.CheckAncWritePri))
                    {
                        Console.WriteLine("CheckAncWritePri NG");
                        break;
                    }
                }
            }
            //catch (System.Exception ex)
            catch
            {
                flag_ng = 1;
            }
            finally
            {
                if (Count == 0 || !Form1.CheckAncWritePri)
                {
                    flag_ng = 1;
                    mainForm.gTestValue[index] = "ANC WRITE PRI NG";
                }

                if (flag_ng != 1)
                {
                    mainForm.gTestValue[index] = "ANC WRITE PRI OK";
                }
            }

            mainForm.ShowProgressIndicator(index, false);

            if (flag_ng == 1)
            {
                mainForm.gNgType[index] = "FW Version PRI fail!";
                return false;
            }
            else
            {
                return true;
            }
        }
     
        private bool procTestCheckAncWriteSec(int index) // check ANC parameter write secondary
        {
            //Thread.Sleep(5000);

            int flag_ng = 0;
            int Count = 20;

            try
            {
                Form1.dev.StartAsyncRead();

                byte a = 0;
                byte[] data = new byte[50];
                byte[] cmdData = new byte[] { 0x05, 0x5A, 0x0C, 0x00, 0x01, 0x0D, 0x05, 0x06, 0x05, 0x5A, 0x04, 0x00, 0x06, 0x0E, 0X00, 0x0D };

                data[0] = 0x02;
                data[1] = 0x0b;
                data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
                data[3] = (byte)Buffer.ByteLength(cmdData);
                Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));
                Form1.dev.Write(data);

                while (Count > 0)
                {
                    mainForm.ShowProgressIndicator(index, true);

                    Form1.dev.StartAsyncRead();
                    Thread.Sleep(1000);
                    Count--;

                    if (Form1.CheckAncWriteSec)
                    {
                        Console.WriteLine("CheckAncWriteSec");
                        break;
                    }
                    else if ((!Form1.CheckAncWriteSec))
                    {
                        Console.WriteLine("CheckAncWriteSec NG");
                        break;
                    }
                }
            }
            //catch (System.Exception ex)
            catch
            {
                flag_ng = 1;
            }
            finally
            {
                if (Count == 0 || !Form1.CheckAncWriteSec)
                {
                    flag_ng = 1;
                    mainForm.gTestValue[index] = "ANC WRITE SEC NG";
                }

                if (flag_ng != 1)
                {
                    mainForm.gTestValue[index] = "ANC WRITE SEC OK";
                }
            }

            mainForm.ShowProgressIndicator(index, false);

            if (flag_ng == 1)
            {
                mainForm.gNgType[index] = "FW Version SEC fail!";
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool procTestShippingPoweroffPri(int index)
        {
            int flag_ng = 0;
            int Count = 20;

            try
            {
                Form1.dev.StartAsyncRead();

                byte[] data = new byte[50];
                byte[] cmdData = new byte[] { 0x05, 0x5A, 0x03, 0x00, 0x01, 0x00, 0x0A };

                data[0] = 0x02;
                data[1] = 0x0b;
                data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
                data[3] = (byte)Buffer.ByteLength(cmdData);
                Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));
                Form1.dev.Write(data);

                while (Count > 0)
                {
                    mainForm.ShowProgressIndicator(index, true);

                    Form1.dev.StartAsyncRead();
                    Thread.Sleep(1000);
                    Count--;

                    if (Form1.CheckShippingPoweroffPri)
                    {
                        Console.WriteLine("Shipping power off pri");
                        break;
                    }
                }
            }
            //catch (System.Exception ex)
            catch
            {
                flag_ng = 1;
            }
            finally
            {
                if (Count == 0 || !Form1.CheckShippingPoweroffPri)
                {
                    flag_ng = 1;
                }

                if (flag_ng != 1)
                {
                    mainForm.gTestValue[index] = "Shipping power off Pri";
                    //if (!checkSwVer(Form1.dutFullVersionSec)) { flag_ng = 1; }
                }
            }

            mainForm.ShowProgressIndicator(index, false);


            if (flag_ng == 1)
            {
                mainForm.gNgType[index] = "Shipping power off pri fail!";
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool procTestShippingPoweroffSec(int index)
        {
            int flag_ng = 0;
            int Count = 20;

            try
            {
                Form1.dev.StartAsyncRead();

                byte[] data = new byte[50];
                byte[] cmdData = new byte[] { 0x05, 0x5A, 0x0B, 0x00, 0x01, 0x0D, 0x05, 0x06, 0x05, 0x5A, 0x03, 0x00, 0x01, 0x00, 0x0A };

                data[0] = 0x02;
                data[1] = 0x0b;
                data[2] = Form1.USB_HOST_DATA_CMD_SEND_AIROHA_RACE_CMD;
                data[3] = (byte)Buffer.ByteLength(cmdData);
                Buffer.BlockCopy(cmdData, 0, data, 4, Buffer.ByteLength(cmdData));
                Form1.dev.Write(data);

                while (Count > 0)
                {
                    mainForm.ShowProgressIndicator(index, true);

                    Form1.dev.StartAsyncRead();
                    Thread.Sleep(1000);
                    Count--;

                    if (Form1.CheckShippingPoweroffSec)
                    {
                        Console.WriteLine("Shipping power off sec");
                        break;
                    }
                }
            }
            //catch (System.Exception ex)
            catch
            {
                flag_ng = 1;
            }
            finally
            {
                if (Count == 0 || !Form1.CheckShippingPoweroffSec)
                {
                    flag_ng = 1;
                }

                if (flag_ng != 1)
                {
                    mainForm.gTestValue[index] = "Shipping power off Sec";
                    //if (!checkSwVer(Form1.dutFullVersionSec)) { flag_ng = 1; }
                }
            }

            mainForm.ShowProgressIndicator(index, false);


            if (flag_ng == 1)
            {
                mainForm.gNgType[index] = "Shipping power off sec fail!";
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool procTestCheckRegion(int index) // wsjung.edit.190208 : test
        {
            int flag_ng = 0;

            try
            {
                if ((Form1.dutRegion == 0x00)) { mainForm.gTestValue[index] = "UC"; }
                else if ((Form1.dutRegion == 0x01)) { mainForm.gTestValue[index] = "CN"; }
                else if ((Form1.dutRegion == 0x02)) { mainForm.gTestValue[index] = "CE7"; }
                else { mainForm.gTestValue[index] = "NONE"; }
            }
            catch
            {
                flag_ng = 1;
            }
            finally
            {
                switch (Form1.gFwRegion)
                {
                    case "UC":
                        if (Form1.dutRegion != 0x00) { flag_ng = 1; }
                        break;
                    case "CN":
                        if (Form1.dutRegion != 0x01) { flag_ng = 1; }
                        break;
                    case "CE7":
#if (HDX2910) || (HDX2918) // wsjung.edit.170921 : for regulated and non-regulated model
                        if (!((readProdData[9] == 0x0f) && (readProdData[10] == 0x00) && (readProdData[11] == 0x0f) && (readProdData[15] == 0x02))) { flag_ng = 1; }
#endif
#if (HDX2914)
                        if (!((readProdData[9] == 0x0d) && (readProdData[10] == 0x00) && (readProdData[11] == 0x0d) && (readProdData[15] == 0x02))) { flag_ng = 1; }
#endif
                        if (Form1.dutRegion != 0x02) { flag_ng = 1; }
                        break;
                }
            }

            if (flag_ng == 1)
            {
                mainForm.gNgType[index] = "Check FW Region Fail";
                return false;
            }
            else
            {
                return true;
            }
        }

        private void setRegenOnInThread()
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;

            // Create new instance of the FTDI device class
            FTDI myFtdiDevice = new FTDI();

            ftStatus = myFtdiDevice.OpenByIndex(0);

            ftStatus = myFtdiDevice.SetBitMode(0xF1, 0x20);
            Thread.Sleep(200);

            ftStatus = myFtdiDevice.SetBitMode(0x00, 0x00);

            ftStatus = myFtdiDevice.Close();
        }

        private void setRegenOffInThread()
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;

            // Create new instance of the FTDI device class
            FTDI myFtdiDevice = new FTDI();

            ftStatus = myFtdiDevice.OpenByIndex(0);

            ftStatus = myFtdiDevice.SetBitMode(0xF0, 0x20);
            Thread.Sleep(200);

            ftStatus = myFtdiDevice.SetBitMode(0x00, 0x00);

            ftStatus = myFtdiDevice.Close();
        }

        private void resetPower()
        {
            setRegenOffInThread(); // wsjung.add.170619 : test
            Thread.Sleep(200);
            setRegenOnInThread(); // wsjung.add.170619 : test
            Thread.Sleep(200);
        }
    }
}
