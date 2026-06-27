using CyberBot_POE.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace CyberBot_POE
{
    public partial class GamePage : Page, INotifyPropertyChanged
    {
        private string _question;
        private string _option1;
        private string _option2;
        private string _option3;
        private string _option4;
        private int _score;

        private int _currentIndex = 0;
        private List<Question> _questions;

        public GamePage()
        {
            InitializeComponent();
            DataContext = this;

            LoadQuestions();
            LoadQuizData(0);
        }

        // ===== BINDABLE PROPERTIES =====

        public string Question
        {
            get => _question;
            set { _question = value; OnPropertyChanged(); }
        }

        public string Option1
        {
            get => _option1;
            set { _option1 = value; OnPropertyChanged(); }
        }

        public string Option2
        {
            get => _option2;
            set { _option2 = value; OnPropertyChanged(); }
        }

        public string Option3
        {
            get => _option3;
            set { _option3 = value; OnPropertyChanged(); }
        }

        public string Option4
        {
            get => _option4;
            set { _option4 = value; OnPropertyChanged(); }
        }

        public string ScoreString => $"Score: {_score}";

        // ===== LOAD QUESTIONS =====

        private void LoadQuestions()
        {
            _questions = new List<Question>
            {
                new Question
                {
                    QuestionText = "What does 2FA stand for?",
                    Options = new[] { "Two-Factor Authentication", "Two-Factor Authorization", "Two-Factor Access", "Two-Factor Approval" },
                    CorrectAnswer = 0
                },
                new Question
                {
                    QuestionText = "What is phishing?",
                    Options = new[] { "Fishing", "Cyber attack", "App", "Software" },
                    CorrectAnswer = 1
                }
            };
        }

        // ===== LOAD UI =====

        private void LoadQuizData(int index)
        {
            var q = _questions[index];

            Question = q.QuestionText;
            Option1 = q.Options[0];
            Option2 = q.Options[1];
            Option3 = q.Options[2];
            Option4 = q.Options[3];

            OnPropertyChanged(nameof(ScoreString));
        }

        // ===== SUBMIT =====

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            int selected = -1;

            if (opt1.IsChecked == true) selected = 0;
            else if (opt2.IsChecked == true) selected = 1;
            else if (opt3.IsChecked == true) selected = 2;
            else if (opt4.IsChecked == true) selected = 3;

            if (selected == -1)
            {
                MessageBox.Show("Select an answer first!");
                return;
            }

            var q = _questions[_currentIndex];

            if (selected == q.CorrectAnswer)
            {
                _score++;
                MessageBox.Show("Correct!");
            }
            else
            {
                MessageBox.Show("Wrong!");
            }

            _currentIndex++;

            if (_currentIndex < _questions.Count)
            {
                LoadQuizData(_currentIndex);
            }
            else
            {
                MessageBox.Show($"Quiz Finished! Score: {_score}");
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

       

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}