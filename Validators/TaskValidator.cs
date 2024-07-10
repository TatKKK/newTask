using newTask.Models;
using Task = newTask.Models.Task;

namespace newTask.Validators

{
    public static  class TaskValidator
    {
        public static bool Validate(Task task, out string errorMessage)
        {
            if (task == null)
            {
                errorMessage = "Task cannot be null.";
                return false;
            }

            if (string.IsNullOrEmpty(task.Name) || string.IsNullOrEmpty(task.Description) 
                || task.AssigneeId == 0 ||task.OwnerId == 0)
            {
                errorMessage = "Fields are required";
                return false;
            }

            if(task.Name.Length <4 || task.Description.Length < 4)
            {
                errorMessage = "Min 4 characters";
                return false;
            }

            if (task.DueDate.HasValue && task.DueDate.Value < DateTime.Now)
            {
                errorMessage = "Due date cannot be in the past.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

    }
}
