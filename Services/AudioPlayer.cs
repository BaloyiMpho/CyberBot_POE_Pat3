using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Media;
using System.Windows;

namespace CyberBot_POE.Services;

public class AudioPlayer
{
     public void Greetings(){
        try{
            SoundPlayer player = new SoundPlayer("Assets/Greeting.wav");
             player.Play();
        }catch(Exception ex){

          MessageBox.Show($"Erro Paying the sound:{ex.Message}");
        }
     }
}