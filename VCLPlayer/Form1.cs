using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using VCLPlayer;

namespace VCLPlayer
{
    public partial class Form1 : Form
    {
        VlcPlayer player;
        VlcPlayerList playerList;
        public Form1()
        {
            InitializeComponent();
            player = new VlcPlayer("./plugins");
            playerList = new VlcPlayerList("./plugins");
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            try
            {
                this.Width = 1080;
                this.Height = 960;
                this.Top = 960;
                this.Left = 0;

                //player.SetRenderWindow((int)this.Handle);
                //player.SetFullScreen(true);
               // player.PlayFile("./1.flv");
                string[] files = { "./1.flv","./2.flv"};
                playerList.SetRenderWindow((int)this.Handle);
                playerList.PlayFiles(files,(int)this.Handle );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
           
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            player.Pause();
        }
    }
}
