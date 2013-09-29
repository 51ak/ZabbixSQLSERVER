using System;
using System.Collections.Generic;
using System.Text;
using ajaxpr02;
using System.Data;
using System.Data.SqlClient;

namespace zabbix_rec
{
    class Program
    {
        //class begin
        static void Main(string[] args)
        {        
            string rscmd = args[0].ToLower();
            string returnvalue;
            switch (rscmd)
            {
                case "test":
                    {
                        //测试 
                        returnvalue = "I'm fine!";
                        break;
                    }
                case "pwd":
                    {
                        //密文获得，输入明文得密码串  pwd zhangwjcmc
                        string oldstr = args[1];
                        returnvalue = GetPasswordStr(oldstr);
                        break;
                    }                
                case "sqlproc":
                    {
                        //运行存储过程 例： sqlproc 0 0 zhangwj 4zg/BgsyIZe7GALUDUc2h4+MBojIziCKT7TPO752uro= Dba_Pfdata usp_dba_wkf_lockcount_get db
                        string port = args[1];
                        string instance = args[2];
                        string uid = args[3];
                        string password = args[4];//加密后的密码串，会在运行时被解出来
                        string dbname = args[5];
                        string rstsq = args[6];

                        string rspara = "";
                        if (args.Length > 7)
                        {
                            rspara = args[7];
                        }
                        returnvalue = GetSqlProcValue(port, instance, uid, password, dbname, rstsq, rspara);                       
                        break;
                    }
                case "sysloginuser":
                    {
                        //得到WINDOWS当前登录用户名和使用的客户端，多个用户用分号隔开
                        returnvalue = GetLoginUser();
                        break;
                    }
                case "processcpu":
                    {
                        //得到指定进程的CPU占用 例  processcpu rsyncd
                        string processname = args[1];
                        returnvalue = ProcessUtility.GetProcessRate(processname);
                        break;
                    }
                case "processmem":
                    {  
                        //得到指定进程的内存占用
                        string processname = args[1];
                        returnvalue = ProcessUtility.GetProcessDDR(processname);
                        break;
                    }
                case "processkill":
                    {
                        //杀掉指定进程
                        string processname = args[1];
                        returnvalue = ProcessUtility.StopProcessByPName(processname).ToString();
                        break;
                    }
                case "processrestart":
                    {
                        //重启指定进程
                        string processname = args[1];
                        returnvalue = ProcessUtility.resetProcessByPName(processname).ToString();
                        break;
                    }
                default:
                    {
                        returnvalue = rscmd+"rscmd is not find-51ak";
                        break;
                    }
            }
            Console.Write(returnvalue);
        }
        #region 过程块
        /// <summary>
        /// 字符串加密后的密文
        /// </summary>
        /// <param name="oldstr">原文</param>
        /// <returns></returns>
        protected static string GetPasswordStr(string oldstr)
        {
            pwd_ pwds = new pwd_("8jd72jdi", "j9ks8726");
            return(pwds.Encode(oldstr + "@51ak"));

        }

        /// <summary>
        /// 调用存储过程返回值 
        /// </summary>
        /// <param name="port">端口号:1433,0,def 会被忽略</param>
        /// <param name="instance">实例名：MSSQLSERVER，0，def  会被忽略</param>
        /// <param name="uid">SQL登录用户名</param>
        /// <param name="password">SQL登录密码</param>
        /// <param name="dbname">数据库</param>
        /// <param name="rstsq">运行的usp开头的存储过程</param>
        /// <param name="rspara">存储过程参数竖线分隔多个参数</param>
        /// <returns>一个字符串（可能是数字），有些过程反回的DATATABLE会被拼成一个分号分隔的长字符串</returns>
        protected static string GetSqlProcValue(string port, string instance, string uid, string password, string dbname, string rstsq, string rspara)
        {
            string mess = "";

            //1.处理密文
            if (password.Length > 2)
            {
                pwd_ pwds = new pwd_("8jd72jdi", "j9ks8726");
                password = pwds.Decode(password + "");
            }
            else{
                return "password [" + password + "] is too short !--51ak";
            }
            if (password.Contains("@51ak"))
            {
                password=password.Replace("@51ak", "");
            }
            else
            {
                return "password [" + password + "] is unrecognized !--51ak";
            }
            //2.处理端口
            if (port == "0" || port == "def" || port == "1433")
            {
                port = "";
            }
            else
            {
                int cport = c_.c_int(port);
                if (cport > 0)
                {
                    port = "," + cport.ToString();
                }
                else
                {
                    return "Sql port " + port + "unrecognized !--51ak";
                }
            }

            //2.处理实例
            if (instance == "0" || instance == "def" || instance == "MSSQLSERVER")
            {
                instance = "";
            }
            else
            {
                instance = "\\" + instance.ToString();               
            }
            string connstr = string.Format("server={0}{1}{2};uid={3};password={4};database={5};", "127.0.0.1", port, instance, uid, password, dbname);

            try
            {
                if (rstsq.Contains("usp_zabbix_discovery"))
                {
                    sql_ ooo = new sql_(connstr);
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = rstsq;
                    DataTable dt = ooo.get_datatable(cmd);
                    StringBuilder sb_s1 = new StringBuilder();
                    int allcount = dt.Rows.Count;
                    if (allcount > 0)
                    {
                        sb_s1.Append("{\"data\":[");
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            sb_s1.Append("{\"{#FSNAME}\":\"" + dt.Rows[i][0].ToString() + "\",\"{#FSTYPE}\":\"" + dt.Rows[i][1].ToString() + "\"}");
                            if (i < allcount - 1)
                            {
                                sb_s1.Append(",");
                            }

                        }
                        sb_s1.Append(" ]}");
                    }

                    mess = sb_s1.ToString();

                    //

                }
                else if (rstsq.Contains("usp"))
                {
                    sql_ ooo = new sql_(connstr);
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = rstsq;
                    if (rspara.Length > 0 && rspara != "null")
                    {
                        SqlParameter ccc = new SqlParameter("@para", SqlDbType.VarChar, 1000);
                        ccc.Value=rspara;
                        cmd.Parameters.Add(ccc);
                    }
                    DataTable dt = ooo.get_datatable(cmd);

                    StringBuilder sb_s1 = new StringBuilder();
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (i > 0)
                            {
                                sb_s1.Append(",");
                            }
                            sb_s1.Append(dt.Rows[i][0].ToString());
                        }
                    }
                    mess = sb_s1.ToString();
                }
                else
                {
                    return "rstsq is unrecognized !--51ak";
                }
            }
            catch (Exception ed)
            {
                return ed.Message;
            }

            return mess;
        }
        
        /// <summary>
        /// 得到系统当前正在登录的用户名及连接客户端
        /// </summary>
        /// <returns></returns>
        protected static string GetLoginUser()
        {
            StringBuilder sb_user = new StringBuilder();
            ComputerLoginUserInfo uif = new ComputerLoginUserInfo();
            foreach (ComputerLoginUserInfoModel item in uif.ComputerLoginUserInfoList)
            {
                sb_user.AppendFormat("{0},{1},{2};",item.UserName, item.SessionType, item.ClientUserName);
            }
            return sb_user.ToString();
        }


        #endregion
        
        //end of class
    }
}
