using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace RTS_Laba_3
{
    public partial class Form1 : Form
    {
        Process console;
        StreamWriter writer;

        public Form1()
        {
            InitializeComponent();

            console = Process.Start(new ProcessStartInfo("cmd.exe") 
            {
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });

            // Задание обработчика вывода
            console.OutputDataReceived += OutputHandler;

            // Начать перехват вывода в обработчик
            console.BeginOutputReadLine();

            // поток ввода
            writer = console.StandardInput;
            writer.AutoFlush = true;
        }

        private void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                textBox2.Invoke(new Action(() =>
                {
                    if (outLine.Data == "\f")
                    {
                        textBox2.Text = "";
                        return;
                    }

                    // получаем байты строки из текущей кодировки (win1251) (которая cp866) 
                    var bytes = Encoding.Default.GetBytes(outLine.Data);

                    // декодируем байты в CP866 кодировке
                    var text = Encoding.GetEncoding("CP866").GetString(bytes);

                    textBox2.Text += text + "\r\n";
                }));
            }
        }

        private void EnterCommand()
        {
            // пишем в поток ввода дочернего процесса
            writer.WriteLine(textBox1.Text + "\r\n");
            textBox1.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EnterCommand();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                EnterCommand();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            console.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            writer.WriteLine("cls\r\n");
        }
    }
}
