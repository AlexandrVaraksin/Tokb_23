using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace PassAuth
{
    public partial class Form2 : Form
    {
        public static key kl;

        public Form2(key Key)
        {
            InitializeComponent();
            kl = Key;
        }

        private int i = 3;

        private void button1_Click(object sender, EventArgs e)
        {
            /* string oldpas = textBox1.Text;
             string newpas = textBox2.Text;
             string pass;
             FileStream file = new FileStream("pass.txt", FileMode.Open);
             StreamReader reader = new StreamReader(file);
             pass = reader.ReadLine();
             reader.Close();
             oldpas = encode(oldpas, kl.Val);
             if (pass == oldpas)
             {
                 FileStream wfile = new FileStream("pass.txt", FileMode.Truncate);
                 StreamWriter writer = new StreamWriter(wfile);
                 Random rnd = new Random();
                 int i = 1 + rnd.Next(100);
                 newpas = encode(newpas, GenerateKeyWord(newpas.Length, i));
                 writer.WriteLine(newpas);
                 writer.Close();
                 MessageBox.Show("Your new key: " + GenerateKeyWord(newpas.Length, i));
                 kl.Val = GenerateKeyWord(newpas.Length, i);
             }
             else
             {
                 i--;
                 MessageBox.Show("Wrong password or key! You have " + i + " chances!");
             }*/

            string oldpas = textBox1.Text;
            string newpas = textBox2.Text;
            string drive = "\\\\.\\" + kl.Dir;
            string codpass = encode(oldpas, kl.Val); ;//кодирование пароля
            byte[] ByteBuffer = new byte[512];//задаем размер буфера
            byte[] temp = new byte[8];
            bool flag = true;
            FileReader fr = new FileReader();
            if (fr.OpenRead(drive)) //вызов для чтения
            {
                int count = fr.Read(ByteBuffer, 512);

                for (int i = 54; i < 62; i++)
                {
                    temp[i - 54] = ByteBuffer[i];
                }
                fr.Close();

                byte[] oldpass = new byte[8];
                for (int i = 0; i < codpass.Length; i++)
                {
                    oldpass[i] = (byte)codpass[i];
                }

                for (int i = 0; i < 8; i++)
                {
                    if (temp[i] != oldpass[i])
                        flag = false;
                }
                if (flag != false)
                {
                    if (fr.OpenWrite(drive))
                    {
                        string codnewpass;
                        codnewpass = null;
                        Random rnd = new Random();
                        int l = 1 + rnd.Next(100);
                        kl.Val = GenerateKeyWord(newpas.Length, l);
                        MessageBox.Show("Your key: " + kl.Val);
                        codnewpass = encode(newpas, kl.Val);
                        byte[] newpass = new byte[8];
                        for (int i = 0; i < codnewpass.Length; i++)
                        {
                            newpass[i] = (byte)codnewpass[i];
                        }
                        for (int i = 54; i < 62; i++)
                        {
                            ByteBuffer[i] = newpass[i - 54];
                        }
                        fr.Write(ByteBuffer, 0, 512);
                        this.Hide();
                        MessageBox.Show("Пароль сменен");

                        Application.Exit();
                    }
                }
                else
                {
                    MessageBox.Show("Попыток:" + i);
                    i--;
                    if (i < 1)
                        Application.Exit();
                    //    codpass = null;
                    //   temp = null;
                    //    oldpass = null;
                    textBox1.Clear();
                    textBox2.Clear();

                }
            }

        }

        public static char[] characters = new char[] {'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
           'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','X','X','0','1','2','3','4','5','6','7','8','9','_'};

        public int N = characters.Length;

        public static string encode(String pText, String pKey)
        {
            byte[] txt = Encoding.ASCII.GetBytes(pText);
            byte[] key = Encoding.ASCII.GetBytes(pKey);
            byte[] res = new byte[pText.Length];

            for (int i = 0; i < txt.Length; i++)
            {
                res[i] = (byte)(txt[i] ^ key[i % key.Length]);
            }
            string strMessage = Encoding.Default.GetString(res.Where(x => x != 0).ToArray());
            return strMessage;
        }

       

        public string GenerateKeyWord(int length, int startSeed)
        {
            Random rand = new Random(startSeed);

            string result = "";

            for (int i = 0; i < length; i++)
                result += characters[rand.Next(0, characters.Length)];

            return result;
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void Form2_Load(object sender, EventArgs e)
        {



        }
    }
}
