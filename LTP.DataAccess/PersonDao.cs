using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using LTP.DataModels;

namespace LTP.DataAccess
{
    public class PersonDao : BaseDao
    {
        private static PersonSearchResult CreatePersonSearchResult(SqlDataReader reader)
        {
            return new PersonSearchResult
            {
                PersonId = Convert.ToInt32(reader["person_id"]),
                FirstName = reader["first_name"].ToString(),
                LastName = reader["last_name"].ToString(),
                StateId = Convert.ToInt32(reader["state_id"]),
                StateCode = reader["state_code"].ToString(),
                Gender = Convert.ToChar(reader["gender"]),
                DateOfBirth = Convert.ToDateTime(reader["dob"])
            };
        }

        public ExecuteResult<List<PersonSearchResult>> Search(PersonSearchCriteria criteria, ref int recordCount)
        {
            try
            {
                var results = new List<PersonSearchResult>();
                using (var connection = CreateConnection())
                {
                    using (var command = new SqlCommand("uspPersonSearch", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(new[]
                        {
                            new SqlParameter("@first_name", SqlDbType.VarChar, 50) {Value = criteria.FirstName},
                            new SqlParameter("@last_name", SqlDbType.VarChar, 50) {Value = criteria.LastName},
                            new SqlParameter("@state_id", SqlDbType.Int) {Value = criteria.StateId},
                            new SqlParameter("@gender", SqlDbType.Char, 1) {Value = criteria.Gender},
                            new SqlParameter("@dob", SqlDbType.DateTime) {Value = criteria.DateOfBirth},
                            new SqlParameter("@PageIndex", SqlDbType.Int) {Value = criteria.pageIndex},
                            new SqlParameter("@PageSize", SqlDbType.Int) {Value = criteria.pageSize},
                        });

                        command.Parameters.Add("@RecordCount", SqlDbType.Int, 4);
                        command.Parameters["@RecordCount"].Direction = ParameterDirection.Output;

                        connection.Open();
                        var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            results.Add(CreatePersonSearchResult(reader));
                        }
                        reader.Close();
                        recordCount = Convert.ToInt32(command.Parameters["@RecordCount"].Value);
                    }
                }

                return new ExecuteResult<List<PersonSearchResult>>
                {
                    Data = results,
                    Succeeded = true
                };
            }
            catch (Exception exception)
            {
                return new ExecuteResult<List<PersonSearchResult>>
                {
                    Succeeded = false,
                    Message = exception.Message
                };
            }
        }

        //private void GetCustomersPageWise(int pageIndex)
        //{
        //    string constring = ConfigurationManager.ConnectionStrings["constring"].ConnectionString;
        //    using (SqlConnection con = new SqlConnection(constring))
        //    {
        //        using (SqlCommand cmd = new SqlCommand("GetCustomersPageWise", con))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
        //            cmd.Parameters.AddWithValue("@PageSize", PageSize);
        //            cmd.Parameters.Add("@RecordCount", SqlDbType.Int, 4);
        //            cmd.Parameters["@RecordCount"].Direction = ParameterDirection.Output;
        //            con.Open();
        //            IDataReader idr = cmd.ExecuteReader();
        //            rptCustomers.DataSource = idr;
        //            rptCustomers.DataBind();
        //            idr.Close();
        //            con.Close();
        //            int recordCount = Convert.ToInt32(cmd.Parameters["@RecordCount"].Value);
        //            this.PopulatePager(recordCount, pageIndex);
        //        }
        //    }
        //}

        public ExecuteResult<Person> Save(Person person)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    using (var command = new SqlCommand("uspPersonUpsert", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(new[]
                        {
                            new SqlParameter("@person_id", SqlDbType.VarChar, 50) {Value = person.PersonId},
                            new SqlParameter("@first_name", SqlDbType.VarChar, 50) {Value = person.FirstName},
                            new SqlParameter("@last_name", SqlDbType.VarChar, 50) {Value = person.LastName},
                            new SqlParameter("@state_id", SqlDbType.Int) {Value = person.StateId},
                            new SqlParameter("@gender", SqlDbType.Char, 1) {Value = person.Gender},
                            new SqlParameter("@dob", SqlDbType.DateTime) {Value = person.DateOfBirth}
                        });
                        connection.Open();
                        var personId = Convert.ToInt32(command.ExecuteScalar());
                        person.PersonId = personId;
                    }
                }
                return new ExecuteResult<Person>
                {
                    Data = person,
                    Succeeded = true
                };

            }
            catch (Exception exception)
            {
                return new ExecuteResult<Person>
                {
                    Succeeded = false,
                    Message = exception.Message
                };
            }
        }
    }
}