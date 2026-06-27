using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace CyberBot_POE.Services;

public class RespondingServices
{
    string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=TaskChat;Trusted_Connection=True;TrustServerCertificate=True";
    string step = "";
    string pendingTitle = "";
    string pendingDescription = "";
    string pendingReminder = "";
    string status = "";
    private string currentTopic;

    public string GetResponse(string message)
    {
        string msg = message;

        if (message.Contains("tell me more", StringComparison.OrdinalIgnoreCase) || message.Contains("give me more details", StringComparison.OrdinalIgnoreCase))
        {
            if (!string.IsNullOrEmpty(currentTopic))
            {
                return GetDetailedResponse(currentTopic);
            }
            else
            {
                ProcessMessage(message);
                return "You haven't asked about any security topics yet. What would you like to know about?";
            }
        }

        // Store the topic from the message
        string detectedTopic = StoreTopic(message);
        if (!string.IsNullOrEmpty(detectedTopic))
        {
            currentTopic = detectedTopic;
        }

        Dictionary<string, string> response = new Dictionary<string, string>
        {
            {"Hello","Hello how are you?"},
            {"I am good and yourself?", "i am also good, what can i assists you with?"},
            {"I am not good","I am sorry to hear that, is there anything i can do to help you?"},
            {"Thank you","You're welcome!" },
            {"bye","Goodbye! Stay safe online!" },
            {"Help","What can i assist you with?" },
            {"ask","You can ask me anything to keep you safe in the internet"},
            {"What can i ask you?","Anything i am your personal cyber security chatbot there to teach you about: Password, Phishing and how to safely Browse throw the internet"},
            {"Password","is a secret string of characters (letters, numbers, symbols) used to verify a person's identity when accessing a system,\n" +
             "Use strong passwords with symbols and numbers.\n" +
             "Example:\n" + "MyStr0ngP@ssw0rd!"},
            { "security", "Security protects your data and systems. Want me to elaborate on security best practices?" },
        };

        if (response.ContainsKey(message))
        {
            return response[message];
        }

        ProcessMessage(message);

        return "Sorry, I don't understand. Try asking about Password, Phishing, or Safe Browsing.";
    }

    private void ProcessMessage(string message)
    {
        string msg = message.ToLower();

        // Combined welcome message - ONE LINE!
        if (msg.Contains("welcome") || msg.Contains("start") || msg.Contains("hello") && string.IsNullOrEmpty(step))
        {
            Bot(@"Welcome to chatbot
Type: add task - task title
remind me to update password
to complete a task type: done 1 or complete 2
to delete a task, type delete 1 or remove 1
Type: show");
            return;
        }

        if (IsDeleteRequest(message))
        {
            Bot("deleted");
            int id = ExtractNumber(message);
            if (id == 0)
            {
                Bot("please type task number. e.g: delete 1");
                return;
            }
            DeleteTask(id);
            return;
        }

        if (IsCompleteRequest(message))
        {
            Bot("deleted");
            int id = ExtractNumber(message);
            if (id == 0)
            {
                Bot("please type task number. e.g: done 1");
                return;
            }
            CompleteTask(id);
            return;
        }

        if (step == "description")
        {
            pendingDescription = message;
            if (pendingDescription == "none")
            {
                pendingDescription = "no description";
            }
            step = "reminder";
            Bot("enter reminder or none");
            return;
        }

        if (step == "reminder")
        {
            pendingReminder = message;
            if (pendingReminder.ToLower() == "none")
            {
                pendingReminder = "none";
            }
            SaveTask(pendingTitle, pendingDescription, pendingReminder);
            ClearPendingTask();
            return;
        }

        if (msg.Contains("show") || msg.Contains("display") || msg.Contains("list") || msg.Contains("view"))
        {
            LoadTasks();
            Bot("here are ur tasks");
            return;
        }

        if (msg.Contains("quiz") || msg.Contains("question") || msg.Contains("test"))
        {
            Bot("quiz: 2fa in full");
            Bot("answer: two-factor authentication");
            return;
        }

        if (IsTaskRequest(message))
        {
            string title = ExtractTask(message);
            string description = ExtractDescription(message);
            string reminder = ExtractReminder(message);
            if (title == "")
            {
                Bot("please type the title");
                return;
            }
            if (description != "" && reminder != "")
            {
                SaveTask(title, description, reminder);
                return;
            }

            pendingTitle = title;
            if (description == "")
            {
                step = "description";
                Bot("enter the description");
                return;
            }
            pendingDescription = description;
            if (pendingReminder == "")
            {
                step = "reminder";
                Bot("enter reminder");
                return;
            }
            SaveTask(pendingTitle, pendingDescription, pendingReminder);
            ClearPendingTask();
            return;
        }
    }

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
            message, @"\b(delete|remove|erase|clear|cancel)\b", RegexOptions.IgnoreCase);
    }

    private bool IsCompleteRequest(string message)
    {
        return Regex.IsMatch(
            message, @"\b(done|finish|complete|completed)\b", RegexOptions.IgnoreCase);
    }

    private int ExtractNumber(string message)
    {
        Match match = Regex.Match(message, @"\d+");
        if (match.Success)
        {
            return int.Parse(match.Value);
        }
        return 0;
    }

    private string ExtractTask(string message)
    {
        string task = message.Trim();
        task = Regex.Replace(task, @"\bdescription\b.*", "", RegexOptions.IgnoreCase);
        task = Regex.Replace(task, @"\breminder\b.*", "", RegexOptions.IgnoreCase);
        task = Regex.Replace(task, @"^(please\s?(can you\s+)?(could you)\s+)?", "", RegexOptions.IgnoreCase);
        task = Regex.Replace(task, @"[?.!]+$", "");

        return task.Trim();
    }

    private string ExtractDescription(string message)
    {
        Match match = Regex.Match(message,
            @"\bdescription\b\s*[:\-]?\s*(.*?)(\breminder\b|$)",
            RegexOptions.IgnoreCase);
        if (match.Success)
        {
            return match.Groups[1].Value.Trim();
        }
        return "";
    }

    private string ExtractReminder(string message)
    {
        Match match = Regex.Match(message,
            @"\breminder\b\s*[:\-]?\s*(.*?)(\breminder\b|$)",
            RegexOptions.IgnoreCase);
        if (match.Success)
        {
            return match.Groups[1].Value.Trim();
        }
        return "";
    }

    private void SaveTask(string title, string description, string reminder)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"INSERT INTO dbo.[Task] (Title, Description, Reminder, IsCompleted)
                                 VALUES (@Title, @Description, @Reminder, 0)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Title", title);
                cmd.Parameters.AddWithValue("@Description", description);  // FIXED: was 'title'
                cmd.Parameters.AddWithValue("@Reminder", reminder);        // FIXED: was 'title'
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
                Bot(rows > 0 ? "Task completed" : id + ":task# does not exist");
            }
            LoadTasks();
        }
        catch
        {
            Bot("error:");
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
                Bot(rows > 0 ? "Task deleted" : id + ":task# does not exist");
            }
            LoadTasks();
        }
        catch
        {
            Bot("error:could not delete task");
        }
    }

    private void LoadTasks()
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT Id, Title, Description, Reminder, IsCompleted FROM dbo.[Task] ORDER BY Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                StringBuilder taskList = new StringBuilder();
                bool hasTasks = false;

                while (reader.Read())
                {
                    hasTasks = true;
                    string status = reader["IsCompleted"].ToString() == "True" ? " [COMPLETED]" : " [PENDING]";
                    taskList.AppendLine(reader["Id"] + ". " + reader["Title"] + " | " + reader["Description"] + " | " + reader["Reminder"] + status);
                }

                if (hasTasks)
                {
                    Bot(taskList.ToString());
                }
                else
                {
                    Bot("no tasks saved yet");
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

    public string StoreTopic(string message)
    {
        string[] keywords = { "password", "security", "phishing" };

        foreach (string keyword in keywords)
        {
            if (message.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            {
                currentTopic = keyword;
                return keyword;
            }
        }
        return null;
    }

    private string GetDetailedResponse(string keyword)
    {
        switch (keyword.ToLower())
        {
            case "password":
                return "🔐 **Password Security Tips:**\n" +
                       "• Use 12+ characters with uppercase, lowercase, numbers, and symbols\n" +
                       "• Never reuse passwords across different sites\n" +
                       "• Use a password manager like Bitwarden or LastPass\n" +
                       "• Enable 2-factor authentication (2FA) whenever possible\n" +
                       "• Change passwords every 3-6 months for critical accounts";

            case "security":
                return "🛡️ **Security Best Practices:**\n" +
                       "• Keep all software and apps updated\n" +
                       "• Use a VPN on public Wi-Fi\n" +
                       "• Enable firewall protection\n" +
                       "• Be cautious of suspicious links and attachments\n" +
                       "• Regularly backup important data";

            case "phishing":
                return "🎣 **How to Spot Phishing Attacks:**\n" +
                       "• Check sender's email address carefully\n" +
                       "• Hover over links before clicking to see the real URL\n" +
                       "• Look for poor grammar and spelling errors\n" +
                       "• Never share passwords or sensitive info via email\n" +
                       "• When in doubt, contact the company directly through official channels";

            default:
                return "I can provide more details on that topic. What specifically would you like to know?";
        }
    }
}