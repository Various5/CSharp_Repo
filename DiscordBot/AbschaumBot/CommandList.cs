using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbschaumBot
{
    public class CommandList
    {
        public void Writetest() 
        {
            MessageBox.Show("You really clicked Writetest?");
            // Send the Enter key
            //  SendKeys.Send("H");

            // Wait for a brief period to ensure that the game has received the Enter key
            //  System.Threading.Thread.Sleep(100);
        }
    }
}
