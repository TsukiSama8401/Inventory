using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using WebERP;
using UsersComponent;
using System.Data.Common;

namespace Inventory
{
    /// <summary>
    ///InventoryCheck 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    [System.Web.Script.Services.ScriptService]
    public class InventoryCheck : System.Web.Services.WebService
    {
        public class InventoryGroup
        {
            public string DeptID { get; set; }
            public string DeptName { get; set; }
            public int Amount { get; set; }
        }

        /// 取得未盤清單 group by 科室
        [WebMethod]
        public InventoryGroup[] GetUnCheckInventoryGroupByDept()
        {
            TPECHDBService tpech = TPECHDBService.getInstance();
            DbConnection conn = tpech.GetTPECHConnection("B");
            conn.Open();

            string sql = "SELECT a.CURKEEP_DEPT,b.dept_name,count(*) cnt FROM VW_ASSCHKM a,sysdepm b where a.HOSP_ID = 'B' and a.CURKEEP_DEPT = b.dept_code and a.CHK_RESULT = 'N' group by a.CURKEEP_DEPT,b.dept_name";
            DbCommand commamd = conn.CreateCommand();
            commamd.CommandText = sql;
            commamd.CommandType = System.Data.CommandType.Text;

            List<InventoryGroup> temp = new List<InventoryGroup>();
            temp.Clear();

            try
            {
                DbDataReader reader = commamd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        InventoryGroup data = new InventoryGroup();
                        data.DeptID = reader["CURKEEP_DEPT"].ToString();
                        data.DeptName = reader["DEPT_NAME"].ToString();
                        data.Amount = Convert.ToInt16(reader["CNT"]);
                        temp.Add(data);
                    }
                }
                reader.Close();
            }
            finally
            {
                conn.Close();
            }
            InventoryGroup[] inv = temp.ToArray();
            return inv;
        }

        public class Inventories
        {
            public string HOSP_ID { get; set; } //院區 代碼
            public string OWNER { get; set; } //所有權人 代碼
            public string ASS_NO { get; set; } //財產編號
            public string ASS_SERNO { get; set; } //財產分號
            public string QR_CODE { get { return $"{HOSP_ID}{OWNER}{ASS_NO}{ASS_SERNO}"; } } //QR Code
            public string CURKEEP_DEPT { get; set; } //保管單位 代號
            public string CURKEEP_DEPT_NAME { get; set; } //保管單位 名稱
            public string CURKEEPER { get; set; } //保管人 姓名
            public string CURPLACE { get; set; } //存放地點 名稱
            public string BUY_DATE { get; set; } //民國購置日期
            public string ASS_PNAME { get; set; } //財產名稱
        }

        [WebMethod]
        public Inventories[] GetUnCheckInventory(string deptId)
        {
            TPECHDBService tpech = TPECHDBService.getInstance();
            DbConnection conn = tpech.GetTPECHConnection("B");
            conn.Open();

            string sql = "SELECT a.HOSP_ID,a.OWNER,a.ASS_NO,a.ASS_SERNO,a.CURKEEP_DEPT,a.CURKEEP_DEPT,b.dept_name CURKEEP_DEPT_NAME,a.CURKEEPER,a.CURPLACE,a.BUY_DATE,a.ASS_PNAME FROM VW_ASSCHKM a,sysdepm b where a.HOSP_ID = 'B' and a.CURKEEP_DEPT = :DEPTID and a.CURKEEP_DEPT = b.dept_code and a.CHK_RESULT = 'N'";
            DbCommand commamd = conn.CreateCommand();
            commamd.CommandText = sql;
            commamd.CommandType = System.Data.CommandType.Text;

            DbParameter param = commamd.CreateParameter();
            param.Value = deptId;
            param.DbType = System.Data.DbType.String;
            param.ParameterName = ":DEPTID";
            commamd.Parameters.Add(param);

            List<Inventories> temp = new List<Inventories>();
            temp.Clear();

            try
            {
                DbDataReader reader = commamd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Inventories data = new Inventories();
                        data.HOSP_ID = reader["HOSP_ID"].ToString();
                        data.OWNER = reader["OWNER"].ToString();
                        data.ASS_NO = reader["ASS_NO"].ToString();
                        data.ASS_SERNO = reader["ASS_SERNO"].ToString();
                        data.CURKEEP_DEPT = reader["CURKEEP_DEPT"].ToString();
                        data.CURKEEP_DEPT_NAME = reader["CURKEEP_DEPT_NAME"].ToString();
                        data.CURKEEPER = reader["CURKEEPER"].ToString();
                        data.CURPLACE = reader["CURPLACE"].ToString();
                        data.BUY_DATE = reader["BUY_DATE"].ToString();
                        data.ASS_PNAME = reader["ASS_PNAME"].ToString();
                        temp.Add(data);
                    }
                }
                reader.Close();
            }
            finally
            {
                conn.Close();
            }
            Inventories[] uncheck = temp.ToArray();
            return uncheck;
        }

    }
}
