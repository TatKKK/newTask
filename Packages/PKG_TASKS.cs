using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.VisualBasic;
using newTask.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;
using Task = newTask.Models.Task;
using TaskStatus = newTask.Models.TaskStatus;



namespace newTask.Packages
{
    public interface IPKG_TASKS
    {
        public void AddTask(Task task);
        public void EditTask(Task task, int taskId);
        public void DeleteTask(int taskId);
        public void CompleteTask(int taskId, TaskStatus status);

        public void StartTask(int taskId, TaskStatus status);
        public List<Task> GetTasks(int companyId);
        public List<Task> GetTasksByAssignee(int assigneeId);
        public List<Task> GetTasksByOwner(int ownerId);
        public Task GetTask(int taskId);
        public List<Task> TasksCompletedByUser(int assigneeId);
        public List<Task> TasksInProgressByUser(int assigneeId);
        public List<Task> TasksOverdueByUser(int assigneeId);
        public List<Task> TasksCompleted(int companyId);
        public List<Task> TasksNew(int companyId);
        public List<Task> TasksInProgress(int companyId);
        public List<Task> TasksOverdue(int companyId);

    }
    public class PKG_TASKS : PKG_BASE, IPKG_TASKS
    {
        IConfiguration config;
       

        public PKG_TASKS(IConfiguration config) : base(config)
        {
            this.config = config;
        }

        public void AddTask(Task task)
        {
            {
                using (OracleConnection conn = new OracleConnection(ConnStr))
                {
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand("c##tat.pkg_tasks.add_task", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.BindByName = true;

                        cmd.Parameters.Add("p_name", OracleDbType.Varchar2).Value = task.Name;
                        cmd.Parameters.Add("p_description", OracleDbType.Varchar2).Value = task.Description;
                        cmd.Parameters.Add("p_create", OracleDbType.Date).Value = DateTime.Now;
                        cmd.Parameters.Add("p_duedate", OracleDbType.TimeStamp).Value = task.DueDate;
                        cmd.Parameters.Add("p_taskLevel", OracleDbType.Int32).Value = task.Level;
                        //cmd.Parameters.Add("p_status", OracleDbType.Int32).Value = 1;
                        cmd.Parameters.Add("p_userId", OracleDbType.Int32).Value = task.AssigneeId;
                        cmd.Parameters.Add("p_ownerId", OracleDbType.Int32).Value = task.OwnerId;

                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }

            }

        }

        public void EditTask(Task task, int taskId)
        {
            using (OracleConnection conn = new OracleConnection(ConnStr))
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand("c##tat.pkg_tasks.edit_task", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;

                    cmd.Parameters.Add("p_taskId", OracleDbType.Int32).Value = taskId;
                    cmd.Parameters.Add("p_name", OracleDbType.Varchar2).Value = (object)task.Name ?? DBNull.Value;
                    cmd.Parameters.Add("p_description", OracleDbType.Varchar2).Value = (object)task.Description ?? DBNull.Value;
                    cmd.Parameters.Add("p_duedate", OracleDbType.TimeStamp).Value = (object)task.DueDate ?? DBNull.Value;
                    cmd.Parameters.Add("p_taskLevel", OracleDbType.Int32).Value = (object)task.Level?? DBNull.Value;


                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }



        //public void CompleteTask(int taskId)
        //{
        //    using (OracleConnection conn = new OracleConnection(ConnStr))
        //    {
        //        using (OracleCommand cmd = new OracleCommand("c##tat.pkg_tasks.complete_task", conn))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.Add("p_taskId", OracleDbType.Int32).Value = taskId;
        //            cmd.Parameters.Add("p_taskStatus", OracleDbType.Varchar2).Value = TaskStatus.Completed.ToString();

        //            conn.Open();
        //            cmd.ExecuteNonQuery();
        //        }
        //    }
        //}

        public void DeleteTask(int taskId)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnStr))
                {
                    conn.Open();

                    using (OracleCommand cmd = new OracleCommand("c##tat.pkg_tasks.delete_task", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_taskId", OracleDbType.Int32).Value = taskId;

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

        public void StartTask(int taskId, TaskStatus status)
        {
            using (OracleConnection conn = new OracleConnection(ConnStr))
            {
                using (OracleCommand cmd = new OracleCommand("c##tat.pkg_tasks.start_task", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_taskId", OracleDbType.Int32).Value = taskId;
                    //cmd.Parameters.Add("p_taskStatus", OracleDbType.Varchar2).Value = TaskStatus.InProgress.ToString();
                    cmd.Parameters.Add("p_taskStatus", OracleDbType.Varchar2).Value = status.ToString();

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void CompleteTask(int taskId, TaskStatus status)
        {
            using (OracleConnection conn = new OracleConnection(ConnStr))
            {
                using (OracleCommand cmd = new OracleCommand("c##tat.pkg_tasks.complete_task", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_taskId", OracleDbType.Int32).Value = taskId;
                    cmd.Parameters.Add("p_taskStatus", OracleDbType.Varchar2).Value = status.ToString();

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void UpdateTaskStatus(int taskId, TaskStatus status)
        {
            using (OracleConnection conn = new OracleConnection(ConnStr))
            {
                using (OracleCommand cmd = new OracleCommand("c##tat.pkg_tasks.update_task_status", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_taskId", OracleDbType.Int32).Value = taskId;
                    cmd.Parameters.Add("p_taskStatus", OracleDbType.Varchar2).Value = status.ToString();

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public List<Task> GetTasks(int companyId)
        {
            List<Task> tasks = new List<Task>();

            using (OracleConnection conn = new OracleConnection(ConnStr))
            {
                OracleCommand cmd = new OracleCommand("c##tat.pkg_tasks.get_tasks", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("p_companyId", OracleDbType.Int32).Value = companyId;
                cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                conn.Open();
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Task task = new Task()
                        {
                            Id = reader["taskId"] != DBNull.Value ? int.Parse(reader["taskId"].ToString()) : 0,
                            AssigneeId = reader["userId"] != DBNull.Value ? int.Parse(reader["userId"].ToString()) : 0,
                            User = new User
                            {
                                UserId = Convert.ToInt32(reader["userId"]),
                                Fname = reader["fname"] != DBNull.Value ? reader["fname"].ToString() : "",
                                Lname = reader["lname"] != DBNull.Value ? reader["lname"].ToString() : "",
                                Username = reader["username"] != DBNull.Value ? reader["username"].ToString() : "",
                                CompanyId = reader["companyId"] != DBNull.Value ? int.Parse(reader["companyId"].ToString()) : 0,
                            },
                            OwnerId = reader["ownerId"] != DBNull.Value ? int.Parse(reader["ownerId"].ToString()) : 0,
                            Name = reader["name"] != DBNull.Value ? reader["name"].ToString() : "",
                            Description = reader["description"] != DBNull.Value ? reader["description"].ToString() : "",
                            Status = Enum.TryParse(reader["taskStatus"].ToString(), out TaskStatus taskStatus) ? taskStatus : null,
                            CreateDate = reader["createDate"] != DBNull.Value ? Convert.ToDateTime(reader["createDate"]) : DateTime.Now,
                            DueDate = reader["duedate"] != DBNull.Value ? Convert.ToDateTime(reader["duedate"]) : DateTime.Now,
                            ModifyDate = reader["modifyDate"] != DBNull.Value ? Convert.ToDateTime(reader["modifyDate"]) : DateTime.Now,
                            Level = Enum.TryParse(reader["taskLevel"].ToString(), out TaskLevel taskLevel) ? taskLevel : null
                        };

                        if (task.DueDate.HasValue && task.DueDate.Value < DateTime.Now && task.Status != TaskStatus.Completed)
                        {
                            task.Status = TaskStatus.Overdue;
                            UpdateTaskStatus(task.Id.Value, TaskStatus.Overdue);
                        }
                        tasks.Add(task);
                    }
                }
            }

            return tasks;
        }

        public List<Task> GetTasksByAssignee(int assigneeId)
        {
            List<Task> tasks = new List<Task>();

            using (OracleConnection conn = new OracleConnection(ConnStr))
            {
                OracleCommand cmd = new OracleCommand("c##tat.pkg_tasks.tasks_by_assignee", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("p_userId", OracleDbType.Int32).Value = assigneeId;
                cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                conn.Open();
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Task task = new Task()
                        {
                            Id = reader["taskId"] != DBNull.Value ? int.Parse(reader["taskId"].ToString()) : 0,
                            AssigneeId = reader["userId"] != DBNull.Value ? int.Parse(reader["userId"].ToString()) : 0,
                            OwnerId = reader["ownerId"] != DBNull.Value ? int.Parse(reader["ownerId"].ToString()) : 0,
                            Name = reader["name"] != DBNull.Value ? reader["name"].ToString() : "",
                            Description = reader["description"] != DBNull.Value ? reader["description"].ToString() : "",
                            Status = Enum.TryParse(reader["taskStatus"].ToString(), out TaskStatus taskStatus) ? taskStatus : null,
                            CreateDate = reader["createDate"] != DBNull.Value ? Convert.ToDateTime(reader["createDate"]) : DateTime.Now,
                            DueDate = reader["duedate"] != DBNull.Value ? Convert.ToDateTime(reader["duedate"]) : DateTime.Now,
                            ModifyDate = reader["modifyDate"] != DBNull.Value ? Convert.ToDateTime(reader["modifyDate"]) : DateTime.Now,
                            Level = Enum.TryParse(reader["taskLevel"].ToString(), out TaskLevel taskLevel) ? taskLevel : null
                        };
                        tasks.Add(task);
                    }
                }
            }

            return tasks;
        }

        public List<Task> GetTasksByOwner(int ownerId)
        {
            List<Task> tasks = new List<Task>();

            using (OracleConnection conn = new OracleConnection(ConnStr))
            {
                OracleCommand cmd = new OracleCommand("c##tat.pkg_tasks.tasks_by_owner", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("p_ownerId", OracleDbType.Int32).Value = ownerId;
                cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                conn.Open();
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Task task = new Task()
                        {
                            Id = reader["taskId"] != DBNull.Value ? int.Parse(reader["taskId"].ToString()) : 0,
                            AssigneeId = reader["userId"] != DBNull.Value ? int.Parse(reader["userId"].ToString()) : 0,
                            OwnerId = reader["ownerId"] != DBNull.Value ? int.Parse(reader["ownerId"].ToString()) : 0,
                            Name = reader["name"] != DBNull.Value ? reader["name"].ToString() : "",
                            Description = reader["description"] != DBNull.Value ? reader["description"].ToString() : "",
                            Status = Enum.TryParse(reader["taskStatus"].ToString(), out TaskStatus taskStatus) ? taskStatus : null,
                            CreateDate = reader["createDate"] != DBNull.Value ? Convert.ToDateTime(reader["createDate"]) : DateTime.Now,
                            DueDate = reader["duedate"] != DBNull.Value ? Convert.ToDateTime(reader["duedate"]) : DateTime.Now,
                            ModifyDate = reader["modifyDate"] != DBNull.Value ? Convert.ToDateTime(reader["modifyDate"]) : DateTime.Now,
                            Level = Enum.TryParse(reader["taskLevel"].ToString(), out TaskLevel taskLevel) ? taskLevel : null
                        };
                        tasks.Add(task);
                    }
                }
            }

            return tasks;
        }
        public Task GetTask(int taskId)
        {
            Task task = null;

            using (OracleConnection conn = new OracleConnection(ConnStr))
            {
                OracleCommand cmd = new OracleCommand("c##tat.pkg_tasks.get_task", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("p_taskId", OracleDbType.Int32).Value = taskId;

                cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                conn.Open();
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        task = new Task()
                        {
                            Id = reader["taskId"] != DBNull.Value ? int.Parse(reader["taskId"].ToString()) : 0,
                            AssigneeId = reader["userId"] != DBNull.Value ? int.Parse(reader["userId"].ToString()) : 0,
                            User = new User
                            {
                                UserId = Convert.ToInt32(reader["userId"]),
                                Fname = reader["fname"].ToString(),
                                Lname = reader["lname"].ToString(),
                                Username = reader["username"].ToString()
                            },
                            OwnerId = reader["ownerId"] != DBNull.Value ? int.Parse(reader["ownerId"].ToString()) : 0,
                            Name = reader["name"] != DBNull.Value ? reader["name"].ToString() : "",
                            Description = reader["description"] != DBNull.Value ? reader["description"].ToString() : "",
                            Status = Enum.TryParse(reader["taskStatus"].ToString(), out TaskStatus taskStatus) ? taskStatus : null,
                            CreateDate = reader["createDate"] != DBNull.Value ? Convert.ToDateTime(reader["createDate"]) : DateTime.Now,
                            DueDate = reader["duedate"] != DBNull.Value ? Convert.ToDateTime(reader["duedate"]) : DateTime.Now,
                            ModifyDate = reader["modifyDate"] != DBNull.Value ? Convert.ToDateTime(reader["modifyDate"]) : DateTime.Now,
                            Level = Enum.TryParse(reader["taskLevel"].ToString(), out TaskLevel taskLevel) ? taskLevel : null
                        };

                    }
                }
            }

            return task;
        }

        public List<Task> TasksCompletedByUser(int assigneeId)
        {
            List<Task> tasks = new List<Task>();

            using (OracleConnection conn = new OracleConnection(ConnStr))
            {
                OracleCommand cmd = new OracleCommand("c##tat.pkg_tasks.tasksCompletedByUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("p_userId", OracleDbType.Int32).Value = assigneeId;
                cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                conn.Open();
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Task task = new Task()
                        {
                            Id = reader["taskId"] != DBNull.Value ? int.Parse(reader["taskId"].ToString()) : 0,
                            AssigneeId = reader["userId"] != DBNull.Value ? int.Parse(reader["userId"].ToString()) : 0,
                            OwnerId = reader["ownerId"] != DBNull.Value ? int.Parse(reader["ownerId"].ToString()) : 0,
                            Name = reader["name"] != DBNull.Value ? reader["name"].ToString() : "",
                            Description = reader["description"] != DBNull.Value ? reader["description"].ToString() : "",
                            Status = Enum.TryParse(reader["taskStatus"].ToString(), out TaskStatus taskStatus) ? taskStatus : null,
                            CreateDate = reader["createDate"] != DBNull.Value ? Convert.ToDateTime(reader["createDate"]) : DateTime.Now,
                            DueDate = reader["duedate"] != DBNull.Value ? Convert.ToDateTime(reader["duedate"]) : DateTime.Now,
                            ModifyDate = reader["modifyDate"] != DBNull.Value ? Convert.ToDateTime(reader["modifyDate"]) : DateTime.Now,
                            Level = Enum.TryParse(reader["taskLevel"].ToString(), out TaskLevel taskLevel) ? taskLevel : null,
                            User = new User()
                            {
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
                            }
                        };
                        tasks.Add(task);
                    }
                }
            }

            return tasks;
        }


        public List<Task> TasksCompleted(int companyId)
        {
            List<Task> tasks = new List<Task>();

            using (OracleConnection conn = new OracleConnection(ConnStr))
            {
                OracleCommand cmd = new OracleCommand("c##tat.pkg_tasks.tasksCompleted", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                conn.Open();
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Task task = new Task()
                        {
                            Id = reader["taskId"] != DBNull.Value ? int.Parse(reader["taskId"].ToString()) : 0,
                            AssigneeId = reader["userId"] != DBNull.Value ? int.Parse(reader["userId"].ToString()) : 0,
                            OwnerId = reader["ownerId"] != DBNull.Value ? int.Parse(reader["ownerId"].ToString()) : 0,
                            Name = reader["name"] != DBNull.Value ? reader["name"].ToString() : "",
                            Description = reader["description"] != DBNull.Value ? reader["description"].ToString() : "",
                            Status = Enum.TryParse(reader["taskStatus"].ToString(), out TaskStatus taskStatus) ? taskStatus : null,
                            CreateDate = reader["createDate"] != DBNull.Value ? Convert.ToDateTime(reader["createDate"]) : DateTime.Now,
                            DueDate = reader["duedate"] != DBNull.Value ? Convert.ToDateTime(reader["duedate"]) : DateTime.Now,
                            ModifyDate = reader["modifyDate"] != DBNull.Value ? Convert.ToDateTime(reader["modifyDate"]) : DateTime.Now,
                            Level = Enum.TryParse(reader["taskLevel"].ToString(), out TaskLevel taskLevel) ? taskLevel : null,
                            User = new User()
                            {
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
                            }
                        };
                        tasks.Add(task);
                    }
                }
            }

            return tasks;
        }


        public List<Task> TasksNew(int companyId)
        {
            List<Task> tasks = new List<Task>();

            using (OracleConnection conn = new OracleConnection(ConnStr))
            {
                OracleCommand cmd = new OracleCommand("c##tat.pkg_tasks.tasksNew", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("p_companyId", OracleDbType.Int32).Value = companyId;
                cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                conn.Open();
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Task task = new Task()
                        {
                            Id = reader["taskId"] != DBNull.Value ? int.Parse(reader["taskId"].ToString()) : 0,
                            AssigneeId = reader["userId"] != DBNull.Value ? int.Parse(reader["userId"].ToString()) : 0,
                            OwnerId = reader["ownerId"] != DBNull.Value ? int.Parse(reader["ownerId"].ToString()) : 0,
                            Name = reader["name"] != DBNull.Value ? reader["name"].ToString() : "",
                            Description = reader["description"] != DBNull.Value ? reader["description"].ToString() : "",
                            Status = Enum.TryParse(reader["taskStatus"].ToString(), out TaskStatus taskStatus) ? taskStatus : null,
                            CreateDate = reader["createDate"] != DBNull.Value ? Convert.ToDateTime(reader["createDate"]) : DateTime.Now,
                            DueDate = reader["duedate"] != DBNull.Value ? Convert.ToDateTime(reader["duedate"]) : DateTime.Now,
                            ModifyDate = reader["modifyDate"] != DBNull.Value ? Convert.ToDateTime(reader["modifyDate"]) : DateTime.Now,
                            Level = Enum.TryParse(reader["taskLevel"].ToString(), out TaskLevel taskLevel) ? taskLevel : null,
                            User = new User()
                            {
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
                            }
                        };
                        tasks.Add(task);
                    }
                }
            }

            return tasks;
        }

        public List<Task> TasksInProgressByUser(int assigneeId)
        {
            List<Task> tasks = new List<Task>();

            using (OracleConnection conn = new OracleConnection(ConnStr))
            {
                OracleCommand cmd = new OracleCommand("c##tat.pkg_tasks.tasksInProgressByUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("p_userId", OracleDbType.Int32).Value = assigneeId;
                cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                conn.Open();
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Task task = new Task()
                        {
                            Id = reader["taskId"] != DBNull.Value ? int.Parse(reader["taskId"].ToString()) : 0,
                            AssigneeId = reader["userId"] != DBNull.Value ? int.Parse(reader["userId"].ToString()) : 0,
                            OwnerId = reader["ownerId"] != DBNull.Value ? int.Parse(reader["ownerId"].ToString()) : 0,
                            Name = reader["name"] != DBNull.Value ? reader["name"].ToString() : "",
                            Description = reader["description"] != DBNull.Value ? reader["description"].ToString() : "",
                            Status = Enum.TryParse(reader["taskStatus"].ToString(), out TaskStatus taskStatus) ? taskStatus : null,
                            CreateDate = reader["createDate"] != DBNull.Value ? Convert.ToDateTime(reader["createDate"]) : DateTime.Now,
                            DueDate = reader["duedate"] != DBNull.Value ? Convert.ToDateTime(reader["duedate"]) : DateTime.Now,
                            ModifyDate = reader["modifyDate"] != DBNull.Value ? Convert.ToDateTime(reader["modifyDate"]) : DateTime.Now,
                            Level = Enum.TryParse(reader["taskLevel"].ToString(), out TaskLevel taskLevel) ? taskLevel : null,
                            User = new User()
                            {
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
                            }
                        };
                        tasks.Add(task);
                    }
                }
            }

            return tasks;
        }

        public List<Task> TasksInProgress(int companyId)
        {
            List<Task> tasks = new List<Task>();

            using (OracleConnection conn = new OracleConnection(ConnStr))
            {
                OracleCommand cmd = new OracleCommand("c##tat.pkg_tasks.tasksNew", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("p_companyId", OracleDbType.Int32).Value = companyId;
                cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                conn.Open();
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Task task = new Task()
                        {
                            Id = reader["taskId"] != DBNull.Value ? int.Parse(reader["taskId"].ToString()) : 0,
                            AssigneeId = reader["userId"] != DBNull.Value ? int.Parse(reader["userId"].ToString()) : 0,
                            OwnerId = reader["ownerId"] != DBNull.Value ? int.Parse(reader["ownerId"].ToString()) : 0,
                            Name = reader["name"] != DBNull.Value ? reader["name"].ToString() : "",
                            Description = reader["description"] != DBNull.Value ? reader["description"].ToString() : "",
                            Status = Enum.TryParse(reader["taskStatus"].ToString(), out TaskStatus taskStatus) ? taskStatus : null,
                            CreateDate = reader["createDate"] != DBNull.Value ? Convert.ToDateTime(reader["createDate"]) : DateTime.Now,
                            DueDate = reader["duedate"] != DBNull.Value ? Convert.ToDateTime(reader["duedate"]) : DateTime.Now,
                            ModifyDate = reader["modifyDate"] != DBNull.Value ? Convert.ToDateTime(reader["modifyDate"]) : DateTime.Now,
                            Level = Enum.TryParse(reader["taskLevel"].ToString(), out TaskLevel taskLevel) ? taskLevel : null,
                            User = new User()
                            {
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
                            }
                        };
                        tasks.Add(task);
                    }
                }
            }

            return tasks;
        }

        public List<Task> TasksOverdueByUser(int assigneeId)
        {
            List<Task> tasks = new List<Task>();

            using (OracleConnection conn = new OracleConnection(ConnStr))
            {
                OracleCommand cmd = new OracleCommand("c##tat.pkg_tasks.tasksOverdueByUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("p_userId", OracleDbType.Int32).Value = assigneeId;
                cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                conn.Open();
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Task task = new Task()
                        {
                            Id = reader["taskId"] != DBNull.Value ? int.Parse(reader["taskId"].ToString()) : 0,
                            AssigneeId = reader["userId"] != DBNull.Value ? int.Parse(reader["userId"].ToString()) : 0,
                            OwnerId = reader["ownerId"] != DBNull.Value ? int.Parse(reader["ownerId"].ToString()) : 0,
                            Name = reader["name"] != DBNull.Value ? reader["name"].ToString() : "",
                            Description = reader["description"] != DBNull.Value ? reader["description"].ToString() : "",
                            Status = Enum.TryParse(reader["taskStatus"].ToString(), out TaskStatus taskStatus) ? taskStatus : null,
                            CreateDate = reader["createDate"] != DBNull.Value ? Convert.ToDateTime(reader["createDate"]) : DateTime.Now,
                            DueDate = reader["duedate"] != DBNull.Value ? Convert.ToDateTime(reader["duedate"]) : DateTime.Now,
                            ModifyDate = reader["modifyDate"] != DBNull.Value ? Convert.ToDateTime(reader["modifyDate"]) : DateTime.Now,
                            Level = Enum.TryParse(reader["taskLevel"].ToString(), out TaskLevel taskLevel) ? taskLevel : null,
                            User = new User()
                            {
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
                            }
                        };
                        tasks.Add(task);
                    }
                }
            }

            return tasks;
        }

        public List<Task> TasksOverdue(int companyId)
        {
            List<Task> tasks = new List<Task>();

            using (OracleConnection conn = new OracleConnection(ConnStr))
            {
                OracleCommand cmd = new OracleCommand("c##tat.pkg_tasks.tasksOverdue", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("p_companyId", OracleDbType.Int32).Value = companyId;
                cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                conn.Open();
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Task task = new Task()
                        {
                            Id = reader["taskId"] != DBNull.Value ? int.Parse(reader["taskId"].ToString()) : 0,
                            AssigneeId = reader["userId"] != DBNull.Value ? int.Parse(reader["userId"].ToString()) : 0,
                            OwnerId = reader["ownerId"] != DBNull.Value ? int.Parse(reader["ownerId"].ToString()) : 0,
                            Name = reader["name"] != DBNull.Value ? reader["name"].ToString() : "",
                            Description = reader["description"] != DBNull.Value ? reader["description"].ToString() : "",
                            Status = Enum.TryParse(reader["taskStatus"].ToString(), out TaskStatus taskStatus) ? taskStatus : null,
                            CreateDate = reader["createDate"] != DBNull.Value ? Convert.ToDateTime(reader["createDate"]) : DateTime.Now,
                            DueDate = reader["duedate"] != DBNull.Value ? Convert.ToDateTime(reader["duedate"]) : DateTime.Now,
                            ModifyDate = reader["modifyDate"] != DBNull.Value ? Convert.ToDateTime(reader["modifyDate"]) : DateTime.Now,
                            Level = Enum.TryParse(reader["taskLevel"].ToString(), out TaskLevel taskLevel) ? taskLevel : null,
                            User = new User()
                            {
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
                            }
                        };
                        tasks.Add(task);
                    }
                }
            }

            return tasks;
        }
    }
}
