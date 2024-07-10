using newTask.Models;
using Oracle.ManagedDataAccess.Client;
using System.ComponentModel.Design;
using System.Data;
using System.Numerics;
using System.Threading.Tasks;

namespace newTask.Packages
{
    public interface IPKG_COMPANIES
    {
        public void AddCompanyAndUser(Registration companyAndUser);

        public List<Registration> GetAll();
        public Registration GetCompany(int id);
        public void DeleteCompany(int id);
        public int GetCompanyUsersCount(int companyId);
    }
    public class PKG_COMPANIES : PKG_BASE, IPKG_COMPANIES
    {
        IConfiguration config;

        public PKG_COMPANIES(IConfiguration config) : base(config)
        {
            this.config = config;
        }
        public void AddCompanyAndUser(Registration companyAndUser)
        {
            using (OracleConnection conn = new OracleConnection(ConnStr))
            {
                conn.Open();
                OracleTransaction txn = conn.BeginTransaction();

                try
                {
                    OracleCommand addCompanyCmd = new OracleCommand("c##tat.pkg_companies.add_company", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    addCompanyCmd.Parameters.Add("p_name", OracleDbType.Varchar2).Value = companyAndUser.Company.Name;
                    addCompanyCmd.Parameters.Add("p_taxCode", OracleDbType.Varchar2).Value = companyAndUser.Company.TaxCode;
                    addCompanyCmd.Parameters.Add("p_address", OracleDbType.Varchar2).Value = companyAndUser.Company.Address;
                    var p_companyId = new OracleParameter("p_companyId", OracleDbType.Int32, ParameterDirection.Output);
                    addCompanyCmd.Parameters.Add(p_companyId);

                    addCompanyCmd.ExecuteNonQuery();

                    int companyId = int.Parse(p_companyId.Value.ToString());

                    OracleCommand addUserCmd = new OracleCommand("c##tat.pkg_users.add_user", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    addUserCmd.Parameters.Add("p_companyId", OracleDbType.Int32).Value = p_companyId.Value;
                    addUserCmd.Parameters.Add("p_roleId", OracleDbType.Int32).Value = companyAndUser.AdminUser.RoleId;
                    addUserCmd.Parameters.Add("p_fname", OracleDbType.Varchar2).Value = companyAndUser.AdminUser.Fname;
                    addUserCmd.Parameters.Add("p_lname", OracleDbType.Varchar2).Value = companyAndUser.AdminUser.Lname;
                    addUserCmd.Parameters.Add("p_phone", OracleDbType.Varchar2).Value = companyAndUser.AdminUser.Phone;
                    addUserCmd.Parameters.Add("p_email", OracleDbType.Varchar2).Value = companyAndUser.AdminUser.Email;
                    addUserCmd.Parameters.Add("p_username", OracleDbType.Varchar2).Value = companyAndUser.AdminUser.Username;
                    addUserCmd.Parameters.Add("p_password", OracleDbType.Varchar2).Value = companyAndUser.AdminUser.Password;
                    addUserCmd.Parameters.Add("p_isActive", OracleDbType.Int32).Value = 1;

                    addUserCmd.ExecuteNonQuery();

                    txn.Commit();
                }
                catch (Exception ex)
                {
                    txn.Rollback();
                    Console.WriteLine("Error: " + ex.Message);
                    throw;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public List<Registration> GetAll()
        {
            List<Registration> companyList = new List<Registration>();
            using (OracleConnection con = new OracleConnection(base.ConnStr))
            {
                con.Open();
                OracleCommand cmd = new OracleCommand("c##tat.pkg_companies.get_companies", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int companyId = Convert.ToInt32(reader["companyId"]);
                        if (!companyList.Any(r => r.Company.CompanyId == companyId))
                        {
                            companyList.Add(new Registration
                            {
                                Company = new Company
                                {
                                    CompanyId = companyId,
                                    Name = reader["name"].ToString(),
                                    TaxCode = reader["taxCode"].ToString(),
                                    Address = reader["address"].ToString(),
                                   
                                }
                            });
                        }

                        var currentCompany = companyList.First(r => r.Company.CompanyId == companyId);

                      
                        int userCount = Convert.ToInt32(reader["userCount"]);
                       
                        currentCompany.Company.UserCount = userCount;
                    }
                }
            }
            return companyList;
        }



        public Registration GetCompany(int id)
        {
            Registration company = null;
            using (OracleConnection con = new OracleConnection(ConnStr))
            {
                con.Open();
                OracleCommand cmd = new OracleCommand("c##tat.pkg_companies.get_company", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("p_id", OracleDbType.Int32).Value = id;
                cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (company == null)
                        {
                            company = new Registration
                            {
                                Company = new Company
                                {
                                    CompanyId = id,
                                    Name = reader["name"].ToString(),
                                    TaxCode = reader["taxCode"].ToString(),
                                    Address = reader["address"].ToString(),
                                    Users = new List<User>()
                                }
                            };
                        }
                        User user = new User
                        {
                            Username = reader["username"] != DBNull.Value ? reader["username"].ToString() : "",
                            Fname = reader["fname"] != DBNull.Value ? reader["fname"].ToString() : "",
                            Lname = reader["lname"] != DBNull.Value ? reader["lname"].ToString() : "",
                            Phone = reader["phone"] != DBNull.Value ? reader["phone"].ToString() : ""
                        };
                        company.Company.Users.Add(user);
                    }
                }
            }
            return company;
        }

        public void DeleteCompany(int id)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnStr))
                {
                    conn.Open();

                    using (OracleCommand cmd = new OracleCommand("c##tat.pkg_companies.delete_company", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_taskId", OracleDbType.Int32).Value = id;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (OracleException ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");

                throw;
            }
        }


        public int GetCompanyUsersCount(int companyId)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnStr))
                {
                    conn.Open();

                    using (OracleCommand cmd = new OracleCommand("c##tat.pkg_companies.count_users", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_companyId", OracleDbType.Int32).Value = companyId;
                        cmd.Parameters.Add("p_count_users", OracleDbType.Int32, ParameterDirection.Output);

                        cmd.ExecuteNonQuery();

                        int userCount = Convert.ToInt32(cmd.Parameters["p_count_users"].Value);
                        return userCount;
                    }
                }
            }
            catch (OracleException ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");

                throw;
            }
        }


    }
}