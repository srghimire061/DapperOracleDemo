using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace DapperOracleDemo
{
    public class OracleDataAccessObject
    {
        private readonly string _ConnectionString;

        public OracleDataAccessObject(string ConnectionString)
        {
            _ConnectionString = ConnectionString;
        }

        private IDbConnection GetOracleConnection()
        {
            return new OracleConnection(_ConnectionString);
        }

        public dynamic OracleSProcWithParam(string sql, OracleDynamicParameters param)
        {
            using IDbConnection cnn = GetOracleConnection();
            CommandType? commandType = CommandType.StoredProcedure;
            return cnn.Execute(sql, param, null, null, commandType);
        }
    }
}
