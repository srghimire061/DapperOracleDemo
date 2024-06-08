using Dapper;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DapperOracleDemo
{
    public class OracleDynamicParameters : SqlMapper.IDynamicParameters
    {
        private readonly DynamicParameters dynamicParameters = new DynamicParameters();

        private readonly List<OracleParameter> oracleParameters = new List<OracleParameter>();

        public void Add(string name, OracleDbType oracleDbType, ParameterDirection direction, object value = null, int? size = null)
        {
            OracleParameter item = ((!size.HasValue) ? new OracleParameter(name, oracleDbType, value, direction) : new OracleParameter(name, oracleDbType, size.Value, value, direction));
            oracleParameters.Add(item);
        }

        public void Add(string name, OracleDbType oracleDbType, ParameterDirection direction)
        {
            OracleParameter item = new OracleParameter(name, oracleDbType, direction);
            oracleParameters.Add(item);
        }

        public void AddParameters(IDbCommand command, SqlMapper.Identity identity)
        {
            ((SqlMapper.IDynamicParameters)dynamicParameters).AddParameters(command, identity);
            (command as OracleCommand)?.Parameters.AddRange(oracleParameters.ToArray());
        }

        public T Get<T>(string name)
        {
            var parameter = oracleParameters.SingleOrDefault(t => t.ParameterName == name);
            object val = parameter?.Value;
            if (val == DBNull.Value)
            {
                if (default(T) != null)
                {
                    throw new ApplicationException("Attempting to cast a DBNull to a non nullable type! Note that out/return parameters will not have updated values until the data stream completes (after the 'foreach' for Query(..., buffered: false), or after the GridReader has been disposed for QueryMultiple)");
                }
                return default;
            }
            return (T)val;
        }

        public T Get<T>(int index)
        {
            var parameter = oracleParameters[index];
            object val = parameter?.Value;
            if (val == DBNull.Value)
            {
                if (default(T) != null)
                {
                    throw new ApplicationException("Attempting to cast a DBNull to a non nullable type! Note that out/return parameters will not have updated values until the data stream completes (after the 'foreach' for Query(..., buffered: false), or after the GridReader has been disposed for QueryMultiple)");
                }
                return default;
            }
            return (T)val;
        }
    }
}
