using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using LTP.DataModels;

namespace LTP.DataAccess
{
    public class StateDao : BaseDao
    {
        private static State CreateState(SqlDataReader reader)
        {
            return new State
            {
                StateId = Convert.ToInt32(reader["state_id"]),
                StateCode = reader["state_code"].ToString()
            };
        }

        public ExecuteResult<List<State>> GetAll()
        {
            try
            {
                var results = new List<State>();
                using (var connection = CreateConnection())
                {
                    using (var command = new SqlCommand("uspStatesList", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        connection.Open();
                        var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            results.Add(CreateState(reader));
                        }
                    }
                }

                return new ExecuteResult<List<State>>
                {
                    Data = results,
                    Succeeded = true
                };
            }
            catch (Exception exception)
            {
                return new ExecuteResult<List<State>>
                {
                    Succeeded = false,
                    Message = exception.Message
                };
            }
        }
    }
}