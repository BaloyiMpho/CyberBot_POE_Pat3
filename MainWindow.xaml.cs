using CyberBot_POE.Models;
using CyberBot_POE.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static System.Net.Mime.MediaTypeNames;



namespace CyberBot_POE;

public partial class MainWindow : Window
{
    private RespondingServices _respondingServices;
    private AudioPlayer _audioPlayer;
    private UserProfile _userProfile;


    public MainWindow()
    {
        InitializeComponent();
        _userProfile = new UserProfile();
        _respondingServices = new RespondingServices();
        _audioPlayer = new AudioPlayer();
        AddMessage(_userProfile.GetArt(), false);
        AddMessage(_Bot( "Welcome to chatbot\nType: add task - task title\nremind me to update password\nto complete a task type: done 1 or complete 2\nto delete a task, type delete 1 or remove 1\nType: show"
), false);
        _audioPlayer.Greetings();

    }

    private string _Bot(string text)
    {
        return _respondingServices.GetResponse(text);
    }

    private async void Button2_Click(object sender, RoutedEventArgs e)
    {

        {
            string userMessage = inputMessage.Text.Trim();

            if (string.IsNullOrWhiteSpace(userMessage))
                return;


            AddMessage(userMessage, true);


            AddMessage("Thinking...", false);

            await Task.Delay(1500);


            if (chatPanel.Children.Count > 0)
                chatPanel.Children.RemoveAt(chatPanel.Children.Count - 1);

            string botReply = _respondingServices.GetResponse(userMessage);

          
             AddMessage(botReply, false);
            inputMessage.Clear();

        }

    }
    public void AddMessage(string message, bool isUser)
    {
        Border bubble = new Border
        {
            Background = isUser ? System.Windows.Media.Brushes.LightBlue : System.Windows.Media.Brushes.LightGray,
            CornerRadius = new CornerRadius(12),
            Padding = new Thickness(10),
            Margin = new Thickness(0, 5, 0, 5),
            HorizontalAlignment = isUser ? HorizontalAlignment.Right : HorizontalAlignment.Left,
            MaxWidth = 250
        };
        TextBlock text = new TextBlock
        {
            Text = message,
            TextWrapping = TextWrapping.Wrap
        };
        bubble.Child = text;
        chatPanel.Children.Add(bubble);
    }
    

    private void Game_Click(object sender, RoutedEventArgs e)
    {

        GamePage gamePage = new GamePage();
        this.Content = gamePage;
    }

}

