using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace zabbix_rec
{
    /// <summary>
    /// author:Stone_W
    /// date:2010.12.20
    /// desc:进程管理类
    /// </summary>
    public class ProcessUtility
    {
        #region 进程的cpu使用率
        /// <summary>
        /// 进程的cpu使用率
        /// </summary>
        /// <param name="processName">进程的名称</param>
        /// <returns>string</returns>
        public static string GetProcessRate(string processName)
        {
            string result = String.Empty;
            try
            {
                PerformanceCounter pfc = new PerformanceCounter();      // 性能计数器
                pfc.CategoryName = "Process";                           // 指定获取计算机进程信息
                pfc.CounterName = "% Processor Time";                   // 占有率
                pfc.InstanceName = processName;                         // 指定进程   
                pfc.MachineName = ".";
                result = Math.Round(pfc.NextValue(), 2).ToString();
            }
            catch (Exception ex) { }
            return result;
        }
        #endregion

        #region 进程内存使用(单位：K)
        /// <summary>
        /// 进程内存使用(单位：K)
        /// </summary>
        /// <param name="pcs">进程实体</param>
        /// <returns>string</returns>
        public static string GetProcessDDR(string pName)
        {
            string result = String.Empty;
            if (!String.IsNullOrEmpty(pName))
            {
                try
                {
                    Process[] myProcesses = Process.GetProcesses();

                    double memsize = 0;
                    foreach (System.Diagnostics.Process myProcess in myProcesses)
                    {
                        if (myProcess.ProcessName.ToUpper() == pName.ToUpper().Trim())
                        {
                           
                            memsize+=myProcess.PrivateMemorySize;
                            break;
                        }
                    }
                    result = memsize.ToString();
                }
                catch (Exception ex) { }
            }
            return result;
        }
        #endregion

        #region 关闭进程
        /// <summary>
        /// 关闭进程
        /// </summary>
        /// <param name="pName">进程的名称</param>
        /// <returns>bool</returns>
        public static bool StopProcessByPName(string pName)
        {
            bool result = false;
            if (!String.IsNullOrEmpty(pName))
            {
                try
                {
                    Process[] myProcesses = Process.GetProcesses();
                    foreach (System.Diagnostics.Process myProcess in myProcesses)
                    {
                        if (myProcess.ProcessName.ToUpper() == pName.ToUpper().Trim())
                        {
                            myProcess.Kill();
                        }
                    }
                    result = true;
                }
                catch (Exception ex) { }
            }
            return result;

        }
        #endregion

        #region 重新启动进程
        /// <summary>
        /// 重新启动进程
        /// </summary>
        /// <param name="pName">进程名称</param>
        /// <returns>bool</returns>
        public static bool resetProcessByPName(string pName)
        {
            bool result = false;
            if (!String.IsNullOrEmpty(pName))
            {
                try
                {
                    Process[] myProcesses;
                    myProcesses = Process.GetProcessesByName(pName);
                    foreach (Process myProcess in myProcesses)
                    {
                        myProcess.Kill();
                    }
                    result = true;
                }
                catch (Exception ex) { }
            }
            return result;
        }
        #endregion
    }
}