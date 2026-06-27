using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberBot_POE.Services;

public class RespondingServices
{
    private string currentTopic;

    public string GetResponse(string message){
        string msg = message;

        // Check for "tell me more" request FIRST
        if (message.Contains("tell me more", StringComparison.OrdinalIgnoreCase)||message.Contains("give me more details", StringComparison.OrdinalIgnoreCase))
        {
            if (!string.IsNullOrEmpty(currentTopic))
            {
                return GetDetailedResponse(currentTopic);
            }
            else
            {
                return "You haven't asked about any security topics yet. What would you like to know about?";
            }
        }

        // Store the topic from the message (THIS WAS MISSING!)
        string detectedTopic = StoreTopic(message);
        if (!string.IsNullOrEmpty(detectedTopic))
        {
            currentTopic = detectedTopic;
        }

        Dictionary<string, string> response = new Dictionary< string, string>
        {


        {"Hello","Hello how are you?"}, {"I am good and yourself?", "i am also good, what can i assists you with?"},
            {"I am not good","I am sorry to hear that, is there anything i can do to help you?"},
            {"Thank you","You're welcome!" },{"bye","Goodbye! Stay safe online!" },
            {"Help","What can i assist you with?" },{"ask","You can ask me anything to keep you safe in the internet"},
        {"What can i ask you?","Anything i am your personal cyber  security chatbot there  to teach you about: Password, Phishing and how to safely Browse throw the internet"},
        {"Password","is a secret string of characters (letters, numbers, symbols) used to verify a person’s identity when accessing a system,\n"+
         "Use strong passwords with symbols and numbers.\n"+
         "Example:\n"+"MyStr0ngP@ssw0rd!"},
            { "security", "Security protects your data and systems. Want me to elaborate on security best practices?" },
        };
         if (response.ContainsKey(message))
    {
        return response[message];
    }

    
    return "Sorry, I don’t understand. Try asking about Password, Phishing, or Safe Browsing.";

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