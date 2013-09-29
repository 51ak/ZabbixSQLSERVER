using System;
using System.Runtime.InteropServices;
using System.Text;

/// <summary>
/// Windows 任务管理器登录用户信息
/// author:Stone_W
/// date:2011.1.14
/// </summary>
public class ComputerLoginUserInfo
{
    #region 本机连接用户信息API封装
    public class TSControl
    {
        [DllImport("wtsapi32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool WTSEnumerateSessions(int hServer, int Reserved,
            int Version, ref long ppSessionInfo, ref int pCount);
        [DllImport("wtsapi32.dll")]
        public static extern void WTSFreeMemory(System.IntPtr pMemory);
        [DllImport("wtsapi32.dll")]
        public static extern bool WTSLogoffSession(int hServer, long SessionId, bool bWait);
        [DllImport("Wtsapi32.dll")]
        public static extern bool WTSQuerySessionInformation(
        System.IntPtr hServer, int sessionId, WTSInfoClass wtsInfoClass,out StringBuilder ppBuffer, out int pBytesReturned);
        public enum WTSInfoClass
        {
            WTSInitialProgram,
            WTSApplicationName,
            WTSWorkingDirectory,
            WTSOEMId,
            WTSSessionId,
            WTSUserName,
            WTSWinStationName,
            WTSDomainName,
            WTSConnectState,
            WTSClientBuildNumber,
            WTSClientName,
            WTSClientDirectory,
            WTSClientProductId,
            WTSClientHardwareId,
            WTSClientAddress,
            WTSClientDisplay,
            WTSClientProtocolType
        }
        public enum WTS_CONNECTSTATE_CLASS
        {
            WTSActive,
            WTSConnected,
            WTSConnectQuery,
            WTSShadow,
            WTSDisconnected,
            WTSIdle,
            WTSListen,
            WTSReset,
            WTSDown,
            WTSInit,
        }

        public struct WTS_SESSION_INFO
        {
            public int SessionID;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pWinStationName;
            public WTS_CONNECTSTATE_CLASS state;
        }

        public static WTS_SESSION_INFO[] SessionEnumeration()
        {
            //Set handle of terminal server as the current terminal server
            int hServer = 0;
            bool RetVal;
            long lpBuffer = 0;
            int Count = 0;
            long p;
            WTS_SESSION_INFO Session_Info = new WTS_SESSION_INFO();
            WTS_SESSION_INFO[] arrSessionInfo;
            RetVal = WTSEnumerateSessions(hServer, 0, 1, ref lpBuffer, ref Count);
            arrSessionInfo = new WTS_SESSION_INFO[0];
            if (RetVal)
            {
                arrSessionInfo = new WTS_SESSION_INFO[Count];
                int i;
                p = lpBuffer;
                for (i = 0; i < Count; i++)
                {
                    arrSessionInfo[i] = (WTS_SESSION_INFO)Marshal.PtrToStructure(new IntPtr(p),
                        Session_Info.GetType());
                    p += Marshal.SizeOf(Session_Info.GetType());
                }
                WTSFreeMemory(new IntPtr(lpBuffer));
            }
            else
            {
                //Insert Error Reaction Here  
            }
            return arrSessionInfo;
        }
    }
    #endregion

    public System.Collections.Generic.List<ComputerLoginUserInfoModel> ComputerLoginUserInfoList;
    public ComputerLoginUserInfo()
    {
        #region 查询代码
        TSControl.WTS_SESSION_INFO[] pSessionInfo = TSControl.SessionEnumeration();
        ComputerLoginUserInfoModel cum = null;
        ComputerLoginUserInfoList = new System.Collections.Generic.List<ComputerLoginUserInfoModel>();
        for (int i = 0; i < pSessionInfo.Length; i++)
        {
            if ("RDP-Tcp" != pSessionInfo[i].pWinStationName)
            {
                try
                {
                    int count = 0;
                    IntPtr buffer = IntPtr.Zero;
                    StringBuilder userName = new StringBuilder();           // 用户名
                    StringBuilder clientUser = new StringBuilder();         // 客户端名
                    StringBuilder stateType = new StringBuilder();          // 会话类型

                    bool userNameBool = TSControl.WTSQuerySessionInformation(IntPtr.Zero,
                        pSessionInfo[i].SessionID, TSControl.WTSInfoClass.WTSUserName,
                        out userName, out count);
                    bool clientUserBool = TSControl.WTSQuerySessionInformation(IntPtr.Zero,
                        pSessionInfo[i].SessionID, TSControl.WTSInfoClass.WTSClientName,
                        out clientUser, out count);
                    bool stateTypeBool = TSControl.WTSQuerySessionInformation(IntPtr.Zero,
                        pSessionInfo[i].SessionID, TSControl.WTSInfoClass.WTSWinStationName,
                        out stateType, out count);
                    if (userNameBool && clientUserBool && stateTypeBool)
                    {
                        cum = new ComputerLoginUserInfoModel();
                        cum.UserName = userName.ToString();
                        cum.ClientUserName = clientUser.ToString();
                        cum.SessionType = stateType.ToString();
                    }
                    ComputerLoginUserInfoList.Add(cum);
                }
                catch (Exception ex) { }
            }
        }
        #endregion
    }



}
public class ComputerLoginUserInfoModel
{
    #region 用户信息字段
    private string userName;
    private string clientUserName;
    private string sessionType;

    /// <summary>
    /// 会话类型
    /// </summary>
    public string SessionType
    {
        get { return sessionType; }
        set { sessionType = value; }
    }
    /// <summary>
    /// 客户端名
    /// </summary>
    public string ClientUserName
    {
        get { return clientUserName; }
        set { clientUserName = value; }
    }
    /// <summary>
    /// 登录用户名
    /// </summary>
    public string UserName
    {
        get { return userName; }
        set { userName = value; }
    }
    #endregion
}