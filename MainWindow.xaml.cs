using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using System.Net.Mail;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.IO;


namespace testBT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            {
                //realizarea unui cod random
                string numere = "0123456789";
                Random random = new Random();
                string cod = string.Empty;
                for (int i = 0; i < 4; i++)
                {
                    int tempval = random.Next(0, numere.Length);
                    cod += tempval;
                }
                MessageBox.Show(cod);

                //nu am inteles bine cerinta,dar in cazul incare OTP trebuie sa contina si
                //date legate de ID, atunci ar merge si varianta de mai jos
                //cod = cod + textbox1.Text;
                //MessageBox.Show(cod);

                //adaugarea in baza de date a codului generat, cu data de creare
                var conn = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=testbt;Persist Security Info=True;User ID=artur;password=artur");
                conn.Open();
                var cmd = new SqlCommand("INSERT INTO tabela (util, data, cod,data_creare) VALUES(@util, @data, @cod,@data_creare);", conn);
                cmd.Parameters.Add("@util", textbox1.Text);
                cmd.Parameters.Add("@data", textbox2.Text);
                cmd.Parameters.Add("@cod", cod);
                cmd.Parameters.AddWithValue("@data_creare", DateTime.Now);
                cmd.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show(" Cod Generat ");

                /*
                // structura pentru trimiterea automata a codului generat pe email, pe moment este configurat sa trimita doar spre mailul personal.
                try
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("smtp.office365.com");
                    mail.From = new MailAddress("artur.ghidora@eurial.com.ro");
                    mail.To.Add("ghidoraartur@gmail.com");
                    //mail.CC.Add("");
                    mail.Subject = "OTP cod";
                    mail.Body = "<b>Buna ziua!</b> <p>Codul OTP este: " + cod + "</p><b>O zi buna!</b>";
                    mail.IsBodyHtml = true;
                    SmtpServer.Port = 25;
                    SmtpServer.Credentials = new System.Net.NetworkCredential("artur.ghidora@eurial.com.ro", "parolastearsa");
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Send(mail);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                MessageBox.Show(" Mail trimis ");

                //structura pentru trimitere automata SMS cu codul generat, pe moment se trimite doar pe numarul alocat, .
                try
                {
                    WebClient client = new WebClient();
                    Stream s = client.OpenRead(string.Format("https://platform.clickatell.com/messages/http/send?apiKey=jlJ8DRJ1RJKIPLuOKDK9aQ==&to=" + +40742435265 + "&content=Codul+OTP+este+ " + cod + " "));
                    StreamReader reader = new StreamReader(s);
                    string result = reader.ReadToEnd();
                    MessageBox.Show("Mesaj trimis");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare sms");
                    //ex.Message,"eroare",MessageBoxButton.OK
                }
                */
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //cautarea unor inregistrari in functie de ID si cod generat
            var scn = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=testbt;Persist Security Info=True;User ID=artur;password=artur");
            SqlCommand scmd = new SqlCommand("select count (*) as cnt from tabela where util=@util and cod=@cod", scn);
            scmd.Parameters.Clear();
            scmd.Parameters.AddWithValue("@util", textbox1.Text);
            scmd.Parameters.AddWithValue("@cod", textbox3.Text);
            scn.Open();

            string getValu = scmd.ExecuteScalar().ToString();
            int z = Int32.Parse(getValu);
            if (z == 0)
            {
                MessageBox.Show("Nu exista inregistrari");
            }
            else
            {
                //in cazul in care exista o inregistrare activa, in functie de cate secunde au trecut
                //de la generarea codului, va afisa un mesaj specific
                //am scazut direct din sql, data de creare a codului cu data actuala
                SqlCommand abc = new SqlCommand("select SUM(DATEDIFF(SECOND, data_creare, GETDATE() )) from tabela where util=@util and cod=@cod", scn);
                abc.Parameters.Clear();
                abc.Parameters.AddWithValue("@util", textbox1.Text);
                abc.Parameters.AddWithValue("@cod", textbox3.Text);

                string secunda = Convert.ToString(abc.ExecuteScalar());
                MessageBox.Show("Au trecut"+secunda+"secunde");
                int timp = Int32.Parse(secunda);
                  if (timp < 31)
                  {
                    MessageBox.Show("Cod corect");
                  }

                  else
                  {
                    MessageBox.Show("Cod invalid, mai genereaza alt cod");
                }
                scn.Close();
            }
        }
    }
}
