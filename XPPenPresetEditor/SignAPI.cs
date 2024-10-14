using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace SignAPI
{
    //event type
    public enum EventType
    {
        EventType_Pen = 1,
        EventType_Key = 2,
        EventType_Eraser = 3,
        EventType_Wheel = 4,
        EventType_ALL = 0xfe
    }


    //pen status
    public enum PenStatus
    {
        PenStatus_Hover,
        PenStatus_Down,
        PenStatus_Move,
        PenStatus_Up,
        PenStatus_Leave
    }

    //key status
    public enum KeyStatus
    {
        KeyStatus_Up,
        KeyStatus_Down
    }

    //connection status
    public enum DeviceStatus
    {
        DeviceStatus_Disconnected,
        DeviceStatus_Connected,
        DeviceStatus_Sleep,
        DeviceStatus_Awake
    }

    //touch status
    public enum TouchStatus
    {
        TouchStatus_Up,
        TouchStatus_Down,
        TouchStatus_Move
    }

    ////run mode
    public enum DeviceRunMode
    {
        DeviceRunMode_Mouse = 1,                    //system mouse
        DeviceRunMode_Pen = 2,                      //pen data
        DeviceRunMode_MousePen = 3,                 //system mouse and pen data
        DeviceRunMode_StdPen = 4                    //standard pen
    }

    public enum ErrorCode : int
    {
        ERR_OK = 0,
        ERR_DEVICE_NOTFOUND = -1,
        ERR_DEVICE_OPENFAIL = -2,
        ERR_DEVICE_NOTCONNECTED = -3,
        ERR_INVALIDPARAM = -101,
        ERR_NOSUPPORTED = -102
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
        public int Width { get { return right - left; } }
        public int Height { get { return bottom - top; } }
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct AXIS
    {
        public int min;
        public int max;
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct TABLET_DEVICEINFO
    {
        public AXIS axisX;                  //x
        public AXIS axisY;                  //y
        public int pressure;               //pressure
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public char[] vendor;               //verdor name
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public char[] product;              //product name
        public uint version;                //driver name
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public char[] serialnum;            //device serial number
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct DATAPACKET
    {
        public EventType eventtype;             //event type			4 bytes
        public ushort physical_key;             //physical key value	2 bytes
        public ushort virtual_key;              //virtual key value		2 bytes
        public KeyStatus keystatus;             //key status			4 bytes
        public PenStatus penstatus;             //pen status			4 bytes
        public int x;                        //x	value				4 bytes
        public int y;                        //y	value				4 bytes
        public int pressure;                 //pressure				4 bytes
        public short wheel_direction;           //wheel			        2 bytes
        public ushort button;                   //pen button            2 bytes;
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct STATUSPACKET
    {
        public int penAlive;
        public int penBattery;
        public int status;      //0  DISCONNECTED  1 CONNECTED  2 SLEEP  3 AWAKE
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct TOUCHDATA
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public TouchStatus[] status;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public uint[] x;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public uint[] y;
    }

    //data callback
    public delegate int DATAPACKETPRCO(DATAPACKET pktObj);
    //status notify callback
    public delegate int DEVNOTIFYPROC(STATUSPACKET status);
    //touch callback
    public delegate int TOUCHPROC(TOUCHDATA td);
    class Win32SignAPI
    {
        //Initialize the device application environment.
        [DllImport("libSign.dll", EntryPoint = "signInitialize")]
        public static extern int signInitialize();
        //Close the device application environment.
        [DllImport("libSign.dll", EntryPoint = "signClean")]
        public static extern void signClean();
        //Find available tablet devices
        [DllImport("libSign.dll", EntryPoint = "signGetDeviceStatus")]
        public static extern int signGetDeviceStatus();
        //Open a usable device.
        [DllImport("libSign.dll", EntryPoint = "signOpenDevice")]
        public static extern int signOpenDevice();
        //Close device.
        [DllImport("libSign.dll", EntryPoint = "signCloseDevice")]
        public static extern int signCloseDevice();
        //Get information about the device.
        [DllImport("libSign.dll", EntryPoint = "signGetDeviceInfo", CharSet = CharSet.Ansi)]
        public static extern int signGetDeviceInfo(ref TABLET_DEVICEINFO devInfo);

        //Register a data callback function.
        [DllImport("libSign.dll", EntryPoint = "signRegisterDataCallBack")]
        public static extern int signRegisterDataCallBack(DATAPACKETPRCO packDataProc);
        //反注册数据回调
        [DllImport("libSign.dll", EntryPoint = "signUnregisterDataCallBack")]
        public static extern void signUnregisterDataCallBack(int handler);

        //Register a status callback function.
        [DllImport("libSign.dll", EntryPoint = "signRegisterDevNotifyCallBack")]
        public static extern int signRegisterDevNotifyCallBack(DEVNOTIFYPROC devNotifyProc);
        //Unregister a data callback function.
        [DllImport("libSign.dll", EntryPoint = "signUnregisterDevNotifyCallBack")]
        public static extern void signUnregisterDevNotifyCallBack(int handler);

        //Register a touch callback function.
        [DllImport("libSign.dll", EntryPoint = "signRegisterTouchCallBack")]
        public static extern int signRegisterTouchCallBack(TOUCHPROC touchProc);
        //Unregister a touch callback function.
        [DllImport("libSign.dll", EntryPoint = "signUnregisterTouchCallBack")]
        public static extern void signUnregisterTouchCallBack(int handler);

        //Find available display device working area
        [DllImport("libSign.dll", EntryPoint = "signGetScreenRect")]
        public static extern int signGetScreenRect(ref RECT srceenRect);
        //control mouse
        [DllImport("libSign.dll", EntryPoint = "signMouseControl")]
        public static extern int signMouseControl(bool enabled);
        //change the device run mode
        [DllImport("libSign.dll", EntryPoint = "signChangeDeviceMode")]
        public static extern int signChangeDeviceMode(int mode);
        //
        [DllImport("libSign.dll", EntryPoint = "signSetExtendDisplay")]
        public static extern void signSetExtendDisplay(bool enabled);
    }
}
