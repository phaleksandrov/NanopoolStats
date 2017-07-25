using System;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System.Xml;

namespace HttpRequest
{
    public partial class Form1 : Form
    {
        public Thread worker;
        public string address;
#if DEBUG
        public bool DEBUG = true;
#else
        public bool DEBUG = false;
#endif
        public volatile bool force_check = false;
        public decimal first_balance = 0;
        public DateTime started_time = DateTime.Now;

        public Form1()
        {
            InitializeComponent();
            if ( File.Exists("settings.xml") )
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("settings.xml");
                XmlNodeList elemList = doc.GetElementsByTagName("app");
                for (int i = 0; i < elemList.Count; i++)
                {
                    address = elemList[i].Attributes["address"].Value;
                    textBox1.Text = address;
                    numericUpDown1.Value = Decimal.Parse(elemList[i].Attributes["timeout"].Value);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (address == null)
            {
                return;
            }
            
            if(worker != null && worker.IsAlive )
            {
                worker.Abort();
                button1.Text = "Start";
            }
            else
            {
                worker = new Thread(GetInfoFromAPI);
                worker.Start();
                button1.Text = "Stop";
            }
        }

        public void GetInfoFromAPI()
        {
            while(true)
            {
                try
                {
                    string url = "https://api.nanopool.org/v1/eth/balance/";
                    if (address == null)
                        break;
                    url += address;
                    Handle hnd = new HttpRequest.Handle();
                    string json = hnd.GetData(url);
                    string show_balance;
                    show_balance = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                    show_balance += " |  Balance = ";

                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Balance));
                    Stream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
                    Balance user = (Balance)serializer.ReadObject(ms);
                    show_balance += user.data;
                    show_balance += "ETH  ->  ";
                    if( (first_balance == 0 ) || ( (user.data - first_balance) < 0 ) )
                    {
                        first_balance = user.data;
                    }

                    url = "https://api.nanopool.org/v1/eth/prices";
                    json = hnd.GetData(url);
                    DataContractJsonSerializer serializer_price = new DataContractJsonSerializer(typeof(Price));
                    Stream ms_price = new MemoryStream(Encoding.Unicode.GetBytes(json));
                    Price price = (Price)serializer_price.ReadObject(ms_price);
                    decimal total = price.data.price_usd * user.data;

                    show_balance += Math.Round(total, 2);
                    show_balance += "$";

                    show_balance += "  ETH Price = ";
                    show_balance += price.data.price_usd;
                    show_balance += "$";


                    url = "https://api.nanopool.org/v1/eth/avghashrate/";
                    url += address;
                    json = hnd.GetData(url);
                    DataContractJsonSerializer serializer_avghashrate = new DataContractJsonSerializer(typeof(AvgHashRate));
                    Stream ms_avghashrate = new MemoryStream(Encoding.Unicode.GetBytes(json));
                    AvgHashRate avghashrate = (AvgHashRate)serializer_avghashrate.ReadObject(ms_avghashrate);
                    show_balance += "(1h:";
                    show_balance += Math.Round(avghashrate.data.h1, 2);
                    show_balance += "Mh/s),";

                    show_balance += "(3h:";
                    show_balance += Math.Round(avghashrate.data.h3, 2);
                    show_balance += "Mh/s),";

                    show_balance += "(6h:";
                    show_balance += Math.Round(avghashrate.data.h6, 2);
                    show_balance += "Mh/s),";

                    show_balance += "(12h:";
                    show_balance += Math.Round(avghashrate.data.h12, 2);
                    show_balance += "Mh/s),";

                    show_balance += "(24h:";
                    show_balance += Math.Round(avghashrate.data.h24, 2);
                    show_balance += "Mh/s),";

                    string text2 = "";
                    if (first_balance != user.data)
                    {
                        string per_time = "" + Decimal.ToInt32((decimal) (DateTime.Now - started_time).TotalMinutes );
                        text2 += (user.data - first_balance);
                        text2 += " ETH per ";
                        text2 += per_time;
                        text2 += " min -> ";
                        text2 += Math.Round( ((user.data - first_balance) * price.data.price_usd), 3);
                        text2 += "$";

                        this.Invoke((MethodInvoker)delegate ()
                        {
                            textBox2.Text = text2;
                        });


                        text2 = "Total mining : ";
                        text2 += (user.data - first_balance);
                        text2 += " ETH";
                        this.Invoke((MethodInvoker)delegate ()
                        {
                            textBox3.Text = text2;
                        });
                    }

                    this.Invoke((MethodInvoker)delegate ()
                    {
                        richTextBox1.Text += show_balance;
                        richTextBox1.Text += "\n";

                        using (StreamWriter file = new StreamWriter("DB_ETH.txt", true))
                        {
                            file.WriteLine(show_balance);
                        }

                        richTextBox1.SelectionStart = richTextBox1.Text.Length;
                        // scroll it automatically
                        richTextBox1.ScrollToCaret();
                    });
                }
                catch
                {
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        richTextBox1.Text += "Some error occurred\n";
                        richTextBox1.SelectionStart = richTextBox1.Text.Length;
                        // scroll it automatically
                        richTextBox1.ScrollToCaret();
                    });
                }

                //richTextBox1.Text += result;
                decimal time_to_update = numericUpDown1.Value;

                for(int i = 0; i < 1000 * 60; i++ )
                {
                    if( force_check == true )
                    {
                        force_check = false;
                        break;
                    }
                    Thread.Sleep(Decimal.ToInt32(time_to_update) );
                }
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            address = textBox1.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            force_check = true;
        }
    }
}
