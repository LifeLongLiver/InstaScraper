﻿
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading;
using System.Net.Http;
using System.Threading.Tasks;
using Dasync.Collections;
using QuickType;
using System.Web.UI.WebControls.Expressions;
using System.Reflection;

namespace Instagram_Email_Grabber
{


    public partial class Form1 : Form
    {

        string fileName;
        int count = 0;
        decimal threadCount;
        string path;
        string proxPath;
        int threadC;
        string[] proxies;
        string[] fileInfo;
        readonly string pattern = @"(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@" + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\." + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|" + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})";
        bool stopBtn = false;

        
        public Form1()
        {
            InitializeComponent();
        }
        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {

            threadCount = threadCountUD.Value;
            threadC = Decimal.ToInt32(threadCount);
        }




        private void StartBtn_Click(object sender, EventArgs e)
        {
         
            stopBtn = false;
            MainWork();
            
            
        }



        private async void MainWork()
        {
            if (proxPath == null)
            {
                try
                {
                    fileName = Path.GetTempFileName();
                    FileInfo fileStuff = new FileInfo(fileName);
                    fileStuff.Attributes = FileAttributes.Temporary;
                    StreamWriter streamWriter = File.AppendText(fileName);
                    streamWriter.WriteLine("Filler");
                    streamWriter.Flush();
                    streamWriter.Close();
                    proxPath = fileName;
                }
                catch (Exception ex)
                {
                   
                }
            }
            proxies = File.ReadAllLines(proxPath);
            fileInfo = File.ReadAllLines(path);
            var proxAndNames = proxies.Zip(fileInfo, (n, w) => new { Proxies = n, InstaName = w });
            

            await proxAndNames.ParallelForEachAsync(

         async var =>
         {

             bool repeat = true;
             while (repeat)
             {
                 if (stopBtn is true)
                 {
                     break;
                 }

                 bool entryFound = false;

                 var cookies = new CookieContainer();

                 var cts = new CancellationTokenSource();

                 var httpClientHandler = new HttpClientHandler
                 {
                     Proxy = new WebProxy(var.Proxies),
                     AllowAutoRedirect = true,
                     CookieContainer = cookies,
                     UseCookies = false,
                     UseDefaultCredentials = false,
                     AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                     UseProxy = true

                 };

                 using (var client = new HttpClient(httpClientHandler, false))
                 {
                     if (!checkBox1.Checked)
                     {
                       httpClientHandler.UseProxy = false;
                     }
                     count++;
                     TriedCount.Invoke(new Action(() => TriedCount.Text = count.ToString()));
                     client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:70.0) Gecko/20100101 Firefox/70.0");
                     client.BaseAddress = new Uri("https://www.instagram.com/");


                     try
                     {

                         var response = await client.GetAsync(var.InstaName + "/?__a=1").ConfigureAwait(true);
                         response.EnsureSuccessStatusCode();

                         if (response.IsSuccessStatusCode)
                         {
                             var grabbed = await client.GetStringAsync(var.InstaName + "/?__a=1").ConfigureAwait(true);
                             var welcome = Temperatures.FromJson(grabbed);
                             Match match = Regex.Match(grabbed, pattern, RegexOptions.None);
                             
                           /*  if (welcome.Graphql.User.BusinessCategoryName is null)
                             {
                                 welcome.Graphql.User.BusinessCategoryName = "";
                             }
                             */
                             foreach (DataGridViewRow row in dataGridView1.Rows)
                             {
                                 object val1 = row.Cells[1].Value;

                                 if (val1.ToString().Contains(var.InstaName))
                                 {

                                     entryFound = true;
                                     break;
                                 }
                             }

                             if (entryFound == false)
                             {
                                 dataGridView1.Invoke(new Action(() => dataGridView1.Rows.Add(welcome.Graphql.User.Id, var.InstaName, welcome.Graphql.User.FullName, match.Value, welcome.Graphql.User.EdgeFollowedBy.Count, welcome.Graphql.User.EdgeFollow.Count, welcome.Graphql.User.IsBusinessAccount, welcome.Graphql.User.BusinessCategoryName)));
                             }

                             if (!proxBox.Text.Contains(var.Proxies))
                             {
                                 proxBox.Invoke(new Action(() => proxBox.Text += var.Proxies + System.Environment.NewLine));
                             }

                             match = match.NextMatch();

                         }
                     }

                     catch (TaskCanceledException)
                     {


                     }
                     catch (TargetInvocationException)
                     {
                
                     }

                     catch (HttpRequestException ex)

                     {
                       
                     }
                     catch (ArgumentNullException) 
                     {

                     }
                     
                     catch (Exception)
                     {

                     }
                     finally
                     {

                         client.Dispose();
                         cts.Dispose();

                     }
                 }


             }



         }, maxDegreeOfParallelism: threadC);


        }





        private void Button1_Click_1(object sender, EventArgs e)
        {

            stopBtn = true;

        }

        public void Button3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                try
                {
                    //Opens file selection screen and grabs the path
                    openFileDialog.InitialDirectory = "c:\\";
                    openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                    openFileDialog.FilterIndex = 2;
                    openFileDialog.RestoreDirectory = true;
                    DialogResult result = openFileDialog.ShowDialog();
                    path = openFileDialog.FileName;
                    var ulineCount = File.ReadLines(path).Count();
                    uLineLbl.Text += ulineCount.ToString();

                }
                catch (Exception)
                {

                }
                
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                try
                {
                    //Opens file selection screen and grabs the path
                    openFileDialog.InitialDirectory = "c:\\";
                    openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                    openFileDialog.FilterIndex = 2;
                    openFileDialog.RestoreDirectory = true;
                    DialogResult result = openFileDialog.ShowDialog();
                    proxPath = openFileDialog.FileName;
                    var plineCount = File.ReadLines(proxPath).Count();
                    pLineLbl.Text += plineCount.ToString();

                }

                catch (ArgumentException)
                {



                }
            }
        }

        
    }


}








