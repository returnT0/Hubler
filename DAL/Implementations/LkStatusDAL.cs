﻿using System.Data;
using Dapper;
using Dapper.Oracle;
using Hubler.DAL.Interfaces;
using Hubler.DAL.Models;

namespace Hubler.DAL.Implementations;

public class LkStatusDAL : ILkStatusDAL
    {
        public LkStatus GetById(int id)
        {
            using (var connection = DBConnection.GetConnection())
            {
                var parameters = new OracleDynamicParameters();
                parameters.Add("p_id", id, OracleMappingType.Int32);
                parameters.Add("p_statusname", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output);

                connection.Execute("GET_STATUS_BY_ID", parameters, commandType: CommandType.StoredProcedure);

                return new LkStatus
                {
                    Id = id,
                    StatusName = parameters.Get<string>("p_statusname")
                };
            }
        }

        public void Insert(LkStatus status)
        {
            using (var connection = DBConnection.GetConnection())
            {
                var parameters = new OracleDynamicParameters();
                parameters.Add("p_statusname", status.StatusName, OracleMappingType.Varchar2);

                connection.Execute("INSERT_STATUS", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public void Update(LkStatus status)
        {
            using (var connection = DBConnection.GetConnection())
            {
                var parameters = new OracleDynamicParameters();
                parameters.Add("p_id", status.Id, OracleMappingType.Int32);
                parameters.Add("p_statusname", status.StatusName, OracleMappingType.Varchar2);

                connection.Execute("UPDATE_STATUS", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public void Delete(int id)
        {
            using (var connection = DBConnection.GetConnection())
            {
                var parameters = new OracleDynamicParameters();
                parameters.Add("p_id", id, OracleMappingType.Int32);

                connection.Execute("DELETE_STATUS", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public IEnumerable<LkStatus> GetAll()
        {
            using (var connection = DBConnection.GetConnection())
            {
                using (var multi = connection.QueryMultiple("GET_ALL_STATUSES", commandType: CommandType.StoredProcedure))
                {
                    return multi.Read<LkStatus>();
                }
            }
        }
    }