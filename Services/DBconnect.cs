using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Data.SqlClient;


namespace CyberBot_POE.Services
{

    class DBconnect
    {

        string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=taskDatabase;Trusted_Connection=True;TrustServerCertificate=True";
        string step = "";
        string pendingTitle = "";
        string pendingDescription = "";
        string pendingReminder = "";
        string status = "";
       private string Task { get; set; }
    private bool IsTaskRequest(string message)
        {
            return Regex.IsMatch(
                message,
                @"\b(add|create|make)\s+(a\s+)?(new\s+)?(task|reminder|todo)\b|" + @"\bset\s+(a\s+)?reminder\b|" + @"\bremind\s+me\b|" + @"\bi\s+need\s+to\b",
                RegexOptions.IgnoreCase);
        }
        private bool IsDeleteRequest(string message)
        {
            return Regex.IsMatch(
                message, @"\b(delete|remove|erase|clear|cancel)\b", RegexOptions.IgnoreCase
                );
        }
        private bool IsCompleteRequest(string message)
        {
            return Regex.IsMatch(
                message, @"\b(done|finish|complete|completed)\b", RegexOptions.IgnoreCase
                );
        }
    

    private void SaveTask(string title, string description, string reminder)
{
    try
    {

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();

            string query = @"INSERT INTO dbo.[Task] (Title, Description, Reminder, IsCompleted)
                                     VALUES (@Title, @Description', @Reminder , 0)";

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Title", title);
            cmd.Parameters.AddWithValue("@Description", title);
            cmd.Parameters.AddWithValue("@Reminder", title);
            cmd.ExecuteNonQuery();
        }

        Bot("Task saved:" + title);
        Bot("description:" + description);
        Bot("Reminder:" + reminder);
        LoadTasks();
    }
    catch (Exception ex)
    {
        Bot("Error: task not saved");
        MessageBox.Show(ex.Message);
    }
}
private void CompleteTask(int id)
{
    try
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string query = "UPDATE dbo.Task SET IsCompleted=1 WHERE Id =@Id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            int rows = cmd.ExecuteNonQuery();
            Bot(rows > 0 ? "Task completed" : +id + ":task# does not exist");
        }
        LoadTasks();
    }
    catch
    {
        Bot("error:");
        //Message()
    }
}

private void DeleteTask(int id)
{
    try
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string query = "DELETE FROM dbo.Task WHERE Id =@Id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            int rows = cmd.ExecuteNonQuery();
            Bot(rows > 0 ? "Task completed" : +id + ":task# does not exist");
        }
        //LoadTasks();
    }
    catch
    {
        Bot("error:could not delete task");
        //Message()
    }
}
        public void LoadTasks()
        {
           

           
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT Id, Title,Description, Reminder,IsCompleted FROM dbo.[Task] ORDER BY Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Task =(reader["Id"] + ". " + reader["Title"] + " | " + reader["Description"] + "|" + reader["Reminder"] + status);
                    }
                    if (Task.Length == 0)
                    {
                        Task = "no tasks saved yet";
                    }
                }
            }
            catch (Exception ex)
            {
                Bot("Error: failed to load tasks");
                MessageBox.Show(ex.Message);
            }
        }
        private void ClearPendingTask()
        {
            step = "";
            pendingTitle = "";
            pendingDescription = "";
            pendingReminder = "";
        }

        private string Bot(string text)
        {
            return text;
        }
    }
}
