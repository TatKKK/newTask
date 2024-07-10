using Microsoft.AspNetCore.Identity.Data;
using newTask.Models;
using Role = newTask.Models.Role;

using Oracle.ManagedDataAccess.Client;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace newTask.Packages
{
    public interface IPKG_USERS
    {
        public void AddUser(User user);
     
        public void ChangePassword(int userId, User user);
      
        public void EditUser(User currentUser, int userId);
        public void DeleteUser(int userId);
        public List<User> GetUsers();

        public List<User> GetAssignees();

        public User GetUser(int userId);

        public User Authenticate(Login loginData);

        public List<UserReport> Report();
    }


    public class PKG_USERS : PKG_BASE, IPKG_USERS
    {
        IConfiguration config;

        public PKG_USERS(IConfiguration config) : base(config)
        {
            this.config = config;
        }

        public void AddUser(User user)
        {
            using (OracleConnection conn = new OracleConnection(ConnStr))
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand("c##tat.pkg_users.add_user", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;

                    cmd.Parameters.Add("p_companyId", OracleDbType.Int32).Value = user.CompanyId;
                    cmd.Parameters.Add("p_roleId", OracleDbType.Int32).Value = user.RoleId;
                    cmd.Parameters.Add("p_fname", OracleDbType.Varchar2).Value = user.Fname;
                    cmd.Parameters.Add("p_lname", OracleDbType.Varchar2).Value = user.Lname;
                    cmd.Parameters.Add("p_phone", OracleDbType.Varchar2).Value = user.Phone;
                    cmd.Parameters.Add("p_email", OracleDbType.Varchar2).Value = user.Email;
                    cmd.Parameters.Add("p_username", OracleDbType.Varchar2).Value = user.Username;
                    cmd.Parameters.Add("p_password", OracleDbType.Varchar2).Value = user.Password;
                    cmd.Parameters.Add("p_isActive", OracleDbType.Int32).Value = 1;

                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }

        }


        public List<User> GetUsers()
        {
            List<User> users = new List<User>();

            using (OracleConnection conn = new OracleConnection(ConnStr))
            {
                OracleCommand cmd = new OracleCommand("c##tat.pkg_users.get_users", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                conn.Open();
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        User user = new User()
                        {
                            UserId = reader["userId"] != DBNull.Value ? int.Parse(reader["userId"].ToString()) : 0,

                            CompanyId = reader["companyId"] != DBNull.Value ? int.Parse(reader["companyId"].ToString()) : 0,
                            Username = reader["username"] != DBNull.Value ? reader["username"].ToString() : "",
                            RoleId = reader["roleId"] != DBNull.Value ? int.Parse(reader["roleId"].ToString()) : 0,
                            Role = new Role
                            {
                                Id = Convert.ToInt32(reader["roleId"]),
                                Name = reader["roleName"].ToString()
                            }
                            ,
                            Fname = reader["fname"] != DBNull.Value ? reader["fname"].ToString() : "",
                            Lname = reader["lname"] != DBNull.Value ? reader["lname"].ToString() : "",
                            Phone = reader["phone"] != DBNull.Value ? reader["phone"].ToString() : "",
                            Email = reader["email"] != DBNull.Value ? reader["email"].ToString() : "",
                            IsActive = reader["isActive"] != DBNull.Value ? Convert.ToInt32(reader["isActive"].ToString()) == 1 : false,
                        };
                        users.Add(user);
                    }
                }
            }

            return users;
        }


        public List<User> GetAssignees()
        {
            List<User> assignees = new List<User>();

            using (OracleConnection conn = new OracleConnection(ConnStr))
            {
                OracleCommand cmd = new OracleCommand("c##tat.pkg_users.get_assignees", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                conn.Open();
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        User user = new User()
                        {
                            UserId = reader["userId"] != DBNull.Value ? int.Parse(reader["userId"].ToString()) : 0,

                            CompanyId = reader["companyId"] != DBNull.Value ? int.Parse(reader["companyId"].ToString()) : 0,
                            Username = reader["username"] != DBNull.Value ? reader["username"].ToString() : "",
                            RoleId = reader["roleId"] != DBNull.Value ? int.Parse(reader["roleId"].ToString()) : 0,
                            //Role = new Role
                            //{
                            //    Id = Convert.ToInt32(reader["roleId"]),
                            //    Name = reader["roleName"].ToString()
                            //}
                            //,
                            //Fname = reader["fname"] != DBNull.Value ? reader["fname"].ToString() : "",
                            //Lname = reader["lname"] != DBNull.Value ? reader["lname"].ToString() : "",
                            //Phone = reader["phone"] != DBNull.Value ? reader["phone"].ToString() : "",
                            //Email = reader["email"] != DBNull.Value ? reader["email"].ToString() : "",
                            //IsActive = reader["isActive"] != DBNull.Value ? Convert.ToInt32(reader["isActive"].ToString()) == 1 : false,
                        };
                        assignees.Add(user);
                    }
                }
            }

            return assignees;
        }

        public User GetUser(int userId)
        {
            User user = null;

            using (OracleConnection conn = new OracleConnection(ConnStr))
            {
                OracleCommand cmd = new OracleCommand("c##tat.pkg_users.get_user", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("p_userId", OracleDbType.Int32).Value = userId;
                cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                conn.Open();
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new User()
                        {
                            UserId = reader["userId"] != DBNull.Value ? int.Parse(reader["userId"].ToString()) : 0,

                            CompanyId = reader["companyId"] != DBNull.Value ? int.Parse(reader["companyId"].ToString()) : 0,
                            Username = reader["username"] != DBNull.Value ? reader["username"].ToString() : "",
                            RoleId = reader["roleId"] != DBNull.Value ? int.Parse(reader["roleId"].ToString()) : 0,
                            Role = new Role
                            {
                                Id = Convert.ToInt32(reader["roleId"]),
                                Name = reader["roleName"].ToString()
                            },
                            Fname = reader["fname"] != DBNull.Value ? reader["fname"].ToString() : "",
                            Lname = reader["lname"] != DBNull.Value ? reader["lname"].ToString() : "",
                            Phone = reader["phone"] != DBNull.Value ? reader["phone"].ToString() : "",
                            Email = reader["email"] != DBNull.Value ? reader["email"].ToString() : "",
                            IsActive = reader["isActive"] != DBNull.Value ? Convert.ToInt32(reader["isActive"].ToString()) == 1 : false,
                        };

                    }
                }
            }

            return user;
        }


        public void EditUser(User currentUser, int userId)
        {

            using (OracleConnection conn = new OracleConnection(ConnStr))
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand("c##tat.pkg_users.edit_user", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;

                    cmd.Parameters.Add("p_userId", OracleDbType.Int32).Value = userId;
                    cmd.Parameters.Add("p_roleId", OracleDbType.Int32).Value = currentUser.RoleId;
                    cmd.Parameters.Add("p_fname", OracleDbType.Varchar2).Value = currentUser.Fname;
                    cmd.Parameters.Add("p_lname", OracleDbType.Varchar2).Value = currentUser.Lname;
                    cmd.Parameters.Add("p_phone", OracleDbType.Varchar2).Value = currentUser.Phone;
                    cmd.Parameters.Add("p_email", OracleDbType.Varchar2).Value = currentUser.Email;
                    cmd.Parameters.Add("p_username", OracleDbType.Varchar2).Value = currentUser.Username;
                    cmd.Parameters.Add("p_isActive", OracleDbType.Int32).Value = currentUser.IsActive;

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        throw new Exception("User not found or no update needed.");
                    }
                    conn.Close();
                }
            }

        }

        public void ChangePassword(int userId, User user)
        {

            using (OracleConnection conn = new OracleConnection(ConnStr))
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand("c##tat.pkg_users.change_password", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("p_userId", OracleDbType.Int32).Value = userId;
                    cmd.Parameters.Add("p_password", OracleDbType.Varchar2).Value = user.Password;

                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }


        }
        public void DeleteUser(int userId)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnStr))
                {
                    conn.Open();

                    using (OracleCommand cmd = new OracleCommand("c##tat.pkg_users.delete_user", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_userId", OracleDbType.Int32).Value = userId;

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

        public User Authenticate(Login loginData)
        {
            OracleConnection conn = new OracleConnection();
            conn.ConnectionString = ConnStr;

            conn.Open();

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "c##tat.pkg_users.authenticate";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_username", OracleDbType.Varchar2).Value = loginData.Username;
            cmd.Parameters.Add("p_password", OracleDbType.Varchar2).Value = loginData.Password;
            cmd.Parameters.Add("p_result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            OracleDataReader reader = cmd.ExecuteReader();

            User user = null;

            if (reader.Read())
            {
                user = new User();

                user.UserId = reader["userId"] != DBNull.Value ? int.Parse(reader["userId"].ToString()) : 0;
                user.CompanyId = reader["companyId"] != DBNull.Value ? int.Parse(reader["companyId"].ToString()) : 0;
                user.Username = reader["username"] != DBNull.Value ? reader["username"].ToString() : string.Empty;

                user.RoleId = reader["roleId"] != DBNull.Value ? int.Parse(reader["roleId"].ToString()) : 0;

                user.Role = new Role
                {
                    Id = reader["roleId"] != DBNull.Value ? int.Parse(reader["roleId"].ToString()) : 0,
                    Name = reader["name"] != DBNull.Value ? reader["name"].ToString() : string.Empty
                };
            }

            conn.Close();
            return user;
        }

        public List<UserReport> Report()
        {
            List<UserReport> reports = new List<UserReport>();

            using (OracleConnection conn = new OracleConnection(ConnStr))
            {
                OracleCommand cmd = new OracleCommand("c##tat.pkg_users.report", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                conn.Open();
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        UserReport report = new UserReport
                        {
                            Fname = reader["fname"].ToString(),
                            Lname = reader["lname"].ToString(),
                            Username = reader["username"].ToString(),
                            CompletedTasks = Convert.ToInt32(reader["completedTasks"]),
                            NewTasks = Convert.ToInt32(reader["newTasks"]),
                            InProgressTasks = Convert.ToInt32(reader["inProgressTasks"]),
                            OverdueTasks = Convert.ToInt32(reader["overdueTasks"])
                        };
                        reports.Add(report);
                    }
                }
            }

            return reports;
        }

    }
}
