using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Data;

namespace DapperOracleDemo
{
    public class Program
    {
        static void Main(string[] args)
        {
            var inDto = new EmployeeInputDto
            {
                NAME = "SURYA RAJ GHIMIRE",
                ADDRESS = "KATHMANDU, NEPAL",
                DEPARTMENT = "RESEARCH & DEVELOPMENT",
                POSITION = "SOFTWARE ENGINEER"
            };

            var resp = SaveEmployeeDetails(inDto);
            var outDto = new EmployeeOutputDto
            {
                EMP_ID = resp.ID is null ? null : (string)resp.ID,
                ERROR = resp.ErrorMsg.ToString()
            };

            Console.WriteLine("Employee id: " + outDto.EMP_ID);
            Console.WriteLine("Error: " + outDto.ERROR);
            Console.ReadLine();
        }

        static SpReturnModel SaveEmployeeDetails(EmployeeInputDto dto)
        {
            var oraDao = new OracleDataAccessObject(Config.OracleDBConnectionString);
            var oracleParam = new OracleDynamicParameters();
            oracleParam.Add("V_NAME", OracleDbType.Varchar2, ParameterDirection.Input, dto.NAME);
            oracleParam.Add("V_ADDRESS", OracleDbType.Varchar2, ParameterDirection.Input, dto.ADDRESS);
            oracleParam.Add("V_DEPARTMENT", OracleDbType.Varchar2, ParameterDirection.Input, dto.DEPARTMENT);
            oracleParam.Add("V_POSITION", OracleDbType.Varchar2, ParameterDirection.Input, dto.POSITION);

            oracleParam.Add("O_EMP_ID", OracleDbType.Varchar2, ParameterDirection.Output, size: 20);
            oracleParam.Add("O_ERROR", OracleDbType.Varchar2, ParameterDirection.Output, size: 2000);

            var oracleQuery = Config.SP_SAVE_EMPLOYEE_DETAILS;
            oraDao.OracleSProcWithParam(oracleQuery, oracleParam);

            return GetSpOutParamResult(oracleParam, "O_EMP_ID");
        }

        static SpReturnModel GetSpOutParamResult(OracleDynamicParameters param, string idParam = "")
        {
            SpReturnModel retMdl = new SpReturnModel()
            {
                ID = !string.IsNullOrEmpty(idParam) ? param.Get<dynamic>(idParam) : 0,
                ErrorMsg = param.Get<OracleString>("O_ERROR")
            };

            return retMdl;
        }
    }

    static class Config
    {
        public static string OracleDBConnectionString => "Data Source=192.168.29.1:1521/DEMODB;User Id=DEMO;Password=demo@123;";
        public static string SP_SAVE_EMPLOYEE_DETAILS = "DEMO.SAVE_EMPLOYEE_DETAILS";
    }
}
