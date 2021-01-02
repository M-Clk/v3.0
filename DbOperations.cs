using Microsoft.Win32;
using Otomasyon.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Web.Management;
using System.Windows.Forms;

namespace Otomasyon
{
    class DbOperations
    {
        public DbOperations()
        { }
        public string bilgiMessage;
        public SqlConnection con;
        public SqlCommand cmd;
        public static bool BaglantiKontrol(string conString)
        {
            using(SqlConnection connect = new SqlConnection(conString))
                return connect.IsAvailable();
        }
        public SqlDataReader SqlTextReader(string command)
        {
            con = new SqlConnection(Settings.Default.ConnectionString);//Veritabanı bağlantısı yap ve ürün sorgusunu yapan stored proc çalıştır
            cmd = new SqlCommand(command, con);
            Application.UseWaitCursor = true;//Fare işaretçisini bekleyen işaretçi ile değiştir.
            SqlDataReader dataRead;
            try //Bağlantı açılmazsa hata fırlatır ona göre işlem yap
            {
                con.Open();
                //if(thread.ThreadState==ThreadState.Running)
                //thread.Abort();
                con.InfoMessage += delegate (object senderMessage, SqlInfoMessageEventArgs eMessage)
                {
                    bilgiMessage = eMessage.ToString();
                };
                dataRead = cmd.ExecuteReader();
                Application.UseWaitCursor = false;
                return dataRead;
            }
            catch(Exception e)

            {
                MessageBox.Show("Veritabanına bağlanılamadı. Veritabanı bağlantı sorunu varken hiçbir işlem yapılamaz. Sunucuya erişim sağlandığından emin olun.\n Hata Mesajı :  " + e.Message, "Sunucu Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dataRead = null;
                return dataRead;
            }
            finally
            {
                Application.UseWaitCursor = false;
            }
        }
        public SqlDataReader OkuProcedure(string command, SqlParameter[] prm)
        {
            con = new SqlConnection(Settings.Default.ConnectionString);//Veritabanı bağlantısı yap ve ürün sorgusunu yapan stored proc çalıştır

            cmd = new SqlCommand(command, con);
            cmd.CommandType = CommandType.StoredProcedure;
            if(prm != null && prm.Length > 0)
                cmd.Parameters.AddRange(prm);
            //Thread thread = new Thread(new ThreadStart(Bekle));
            //thread.Start();
            Application.UseWaitCursor = true;//Fare işaretçisini bekleyen işaretçi ile değiştir.
            SqlDataReader dataRead;
            try //Bağlantı açılmazsa hata fırlatır ona göre işlem yap
            {
                con.Open();
                //if(thread.ThreadState==ThreadState.Running)
                //thread.Abort();
                con.InfoMessage += delegate (object senderMessage, SqlInfoMessageEventArgs eMessage)
                {
                    bilgiMessage = eMessage.ToString();
                };
                dataRead = cmd.ExecuteReader();
                Application.UseWaitCursor = false;
                return dataRead;
            }
            catch(Exception e)

            {
                Application.UseWaitCursor = false;
                if(e.Message.IndexOf("ENCOKSATANLAR") > -1)
                {

                    using(con = new SqlConnection(Settings.Default.ConnectionString))
                    {
                        using(cmd = new SqlCommand("CREATE Procedure ENCOKSATANLAR @GoruntelenecekAdet int, @Tur tinyint AS BEGIN if(@Tur=0) Select Top(@GoruntelenecekAdet) ROW_NUMBER() OVER(Order By Urunler.Adi) AS 'No',Barkod_Kodu,Urunler.Adi,SUM(Miktar) AS ToplamMiktar, Birimler.Adi from Satis_Detayi,Urunler,Birimler  Where Bakod_kodu=Barkod_Kodu AND Iade=0 AND Bakod_kodu!='0' AND Birimler.Id=Urunler.Stok_birimi Group By Barkod_Kodu,Urunler.Adi,Birimler.Adi Order By (ToplamMiktar) Desc else if(@Tur=1) Select Top(@GoruntelenecekAdet) ROW_NUMBER() OVER(Order By Urunler.Adi) AS 'No', Barkod_Kodu,Urunler.Adi,SUM(Miktar) AS ToplamMiktar, Birimler.Adi  from Satis_Detayi,Urunler,Birimler  Where Bakod_kodu=Barkod_Kodu AND Iade=0 AND Bakod_kodu!='0' AND Birimler.Id=Urunler.Stok_birimi Group By Barkod_Kodu,Urunler.Adi,Birimler.Adi Order By ToplamMiktar else Select ROW_NUMBER() OVER(Order By Urunler.Adi) AS 'No', Bakod_kodu,Urunler.Adi, (Select 0) AS ToplamMiktar, Birimler.Adi from Urunler,Birimler Where Bakod_Kodu NOT IN(Select Barkod_Kodu from Satis_Detayi) AND Bakod_kodu!='0' AND Birimler.Id=Urunler.Stok_birimi END", con))
                        {
                            cmd.CommandType = CommandType.Text;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
                else
                {
                    //if(thread.ThreadState==ThreadState.Running)
                    //thread.Abort();
                    MessageBox.Show("Veritabanına bağlanılamadı. Veritabanı bağlantı sorunu varken hiçbir işlem yapılamaz. Sunucuya erişim sağlandığından emin olun.\n Hata Mesajı :  " + e.Message, "Sunucu Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                dataRead = null;
                return dataRead;
            }
            finally
            {
                Application.UseWaitCursor = false;
            }

        }
        public object OkuScalar(string scalarCommand, CommandType cmdtype, SqlParameter[] prm)//Veritabanından tek sonuç istendiğinde çalıştır
        {
            using(con = new SqlConnection(Settings.Default.ConnectionString))
            {

                using(cmd = new SqlCommand(scalarCommand, con))
                {
                    cmd.CommandType = cmdtype;
                    if(cmdtype == CommandType.StoredProcedure)
                    {
                        for(int i = 0; i < prm.Length; i++)
                        {
                            cmd.Parameters.Add(prm[i]);
                        }

                    }
                    //Thread thread1 = new Thread(new ThreadStart(Bekle));
                    //thread1.Start();
                    Application.UseWaitCursor = true;
                    //try //Bağlantı açılmazsa hata fırlatır ona göre işlem yap
                    //{
                    con.Open();
                    con.InfoMessage += delegate (object senderMessage, SqlInfoMessageEventArgs eMessage)
                    {
                        bilgiMessage = eMessage.ToString();
                    };
                    Application.UseWaitCursor = false;
                    //thread1.Abort();
                    object snc = cmd.ExecuteScalar();
                    if(prm.Length > 0)
                        cmd.Parameters.Clear();
                    return snc;
                    //}
                    //catch (SqlException e)

                    //{
                    //Application.UseWaitCursor = false;
                    ////thread1.Abort();
                    //MessageBox.Show("Veritabanına bağlanılamadı. Veritabanı bağlantı sorunu varken hiçbir işlem yapılamaz. Sunucuya erişim sağlandığından emin olun. " + e.Message, "Sunucu Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //cmd.Parameters.Clear();
                    //return null;
                    //}
                }
            }
        }
        public string ScalarTextCommand(string command)
        {
            using(con = new SqlConnection(Settings.Default.ConnectionString))
            {
                using(cmd = new SqlCommand(command, con))
                {
                    cmd.CommandType = CommandType.Text;
                    try
                    {
                        con.Open();

                        return cmd.ExecuteScalar().ToString();
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show("Veritabanına bağlanılamadı. Veritabanı bağlantı sorunu varken hiçbir işlem yapılamaz. Sunucuya erişim sağlandığından emin olun. \n Hata Mesajı :  " + e.Message, "Sunucu Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return "";
                    }
                    finally
                    {
                        Application.UseWaitCursor = false;
                    }
                }
            }
        }
        public bool TextCommand(string command)
        {
            using(con = new SqlConnection(Settings.Default.ConnectionString)) //Veritabanı bağlantısı yap ve ürün sorgusunu yapan stored proc çalıştır
            {
                using(cmd = new SqlCommand(command, con))
                {
                    cmd.CommandType = CommandType.Text;
                    Application.UseWaitCursor = true;//Fare işaretçisini bekleyen işaretçi ile değiştir.

                    try //Bağlantı açılmazsa hata fırlatır ona göre işlem yap
                    {
                        con.Open();
                        //if(thread.ThreadState==ThreadState.Running)
                        //thread.Abort();
                        Application.UseWaitCursor = false;
                        if(cmd.ExecuteNonQuery() > 0)
                        {
                            return true;
                        }

                        else
                        {
                            return false;
                        }
                    }

                    catch(SqlException e)

                    {
                        //if(thread.ThreadState==ThreadState.Running)
                        //thread.Abort();

                        Application.UseWaitCursor = false;
                        if(e.Message.IndexOf("UNIQUE KEY") >= 0 && e.Message.IndexOf("Kullanicilar") >= 0)
                            MessageBox.Show("Bu kullanıcı adına sahip bir kullanıcı zaten var. Lütfen farklı bir kullanıcı adı deneyin.", "Veri Girişi Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        else if(e.Message.IndexOf("UNIQUE KEY") >= 0 && e.Message.IndexOf("Musteriler") >= 0)
                            MessageBox.Show("Bu numarayı kullanan bir müşteri zaten var. İkinci bir müşteri adına kaydedilemez.", "Veri Girişi Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else
                            MessageBox.Show("Veritabanına bağlanılamadı. Veritabanı bağlantı sorunu varken hiçbir işlem yapılamaz. Sunucuya erişim sağlandığından emin olun. \n Hata Mesajı :  " + e.Message, "Sunucu Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return false;
                    }
                    finally
                    {
                        Application.UseWaitCursor = false;
                    }
                }
            }
        }
        public int GuncelleProcedure(string procName, SqlParameter[] parameterCollection)
        {

            using(con = new SqlConnection(Settings.Default.ConnectionString)) //Veritabanı bağlantısı yap ve ürün sorgusunu yapan stored proc çalıştır
            {
                using(cmd = new SqlCommand(procName, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    for(int i = 0; i < parameterCollection.Length; i++)
                    {
                        cmd.Parameters.Add(parameterCollection[i]);
                    }

                    //Thread thread = new Thread(new ThreadStart(Bekle));
                    //thread.Start();
                    Application.UseWaitCursor = true;//Fare işaretçisini bekleyen işaretçi ile değiştir.

                    try //Bağlantı açılmazsa hata fırlatır ona göre işlem yap
                    {
                        con.Open();
                        con.InfoMessage += delegate (object senderMessage, SqlInfoMessageEventArgs eMessage)
                        {
                            bilgiMessage = eMessage.ToString();
                        };
                        //if(thread.ThreadState==ThreadState.Running)
                        //thread.Abort();
                        Application.UseWaitCursor = false;
                        if(cmd.ExecuteNonQuery() > 0)
                        {
                            cmd.Parameters.Clear();
                            return 1;
                        }

                        else
                        {
                            cmd.Parameters.Clear();
                            return -1;
                        }
                    }

                    catch(SqlException e)

                    {
                        //if(thread.ThreadState==ThreadState.Running)
                        //thread.Abort();

                        Application.UseWaitCursor = false;
                        if(e.Message.IndexOf("UNIQUE KEY") >= 0 && e.Message.IndexOf("Kullanicilar") >= 0)
                            MessageBox.Show("Bu kullanıcı adına sahip bir kullanıcı zaten var. Lütfen farklı bir kullanıcı adı deneyin.", "Veri Girişi Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        else if(e.Message.IndexOf("UNIQUE KEY") >= 0 && e.Message.IndexOf("Musteriler") >= 0)
                            MessageBox.Show("Bu numarayı kullanan bir müşteri zaten var. İkinci bir müşteri adına kaydedilemez.", "Veri Girişi Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else
                            MessageBox.Show("Veritabanına bağlanılamadı. Veritabanı bağlantı sorunu varken hiçbir işlem yapılamaz. Sunucuya erişim sağlandığından emin olun. \n Hata Mesajı :  " + e.Message, "Sunucu Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return 0;
                    }
                    finally
                    {
                        Application.UseWaitCursor = false;
                    }
                }
            }
        }
        public DataTable DisconnectedProcedure(string procName, SqlParameter[] parameterCollection)

        {
            using(SqlConnection con = new SqlConnection(Settings.Default.ConnectionString))
            {
                using(SqlCommand cmd = new SqlCommand(procName, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    for(int i = 0; i < parameterCollection.Length; i++)
                    {
                        cmd.Parameters.Add(parameterCollection[i]);
                    }

                    using(SqlDataAdapter dAdapter = new SqlDataAdapter(cmd))
                    {
                        using(DataTable table = new DataTable())
                        {
                            table.Clear();
                            dAdapter.Fill(table);
                            return table;
                        }
                    }
                }
            }

        }
        public string FirstConnection()
        {
            string serverName = null;
            //string dbName = "OtomasyonDB";

            //ServiceController[] controllers = ServiceController.GetServices();
            //foreach(var item in controllers)
            //{
            //    if(item.DisplayName.Contains("SQL Server ("))
            //    {
            //        serverName = item.DisplayName.Remove(item.DisplayName.IndexOf(')'));
            //        serverName = serverName.Remove(0, serverName.IndexOf('(') + 1);
            //    }
            //}
            //if(serverName == null)
            serverName = "LOCALHOST";
            //else
            //    serverName = $"LOCALHOST\\{serverName}";
            return serverName;
        }
        public static bool CheckSqlConnection() => new SqlConnection(Settings.Default.ConnectionString).IsAvailable();
        public void SetSqlConnetion(string server = null)
        {
            Settings.Default.ConnectionString = @$"Server={server ?? Settings.Default.LocalSqlServerName};database = {Settings.Default.DbName}; Integrated Security=SSPI; Connection Timeout={Settings.Default.SqlConnectionTimeOut}";
            Settings.Default.SaveChanges();
        }

        public void CreateDb()
        {
            if(Settings.Default.IsClient)
                throw new InvalidOperationException("Bu bilgisayar bir istemci olarak ayarlanmış. Ancak SQL Server yüklü herhangi bir sunucu ile bağlantı için yapılandırılmamış. Lütfen Yapılandırma Merkezi sayfasından sunucu bilgisayar ile bağlantı kurun.");
            LoadLocalInstance();
            using(con = new SqlConnection(@$"Server={Settings.Default.LocalSqlServerName};database = master; Integrated Security=SSPI;"))
            {
                using(cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    con.Open();
                    cmd.CommandText = $"IF (EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE ('[' + name + ']' =  N'{Settings.Default.DbName}' OR name =  N'{Settings.Default.DbName}'))) Select 1 ;ELSE Select 0;";
                    if(Convert.ToBoolean(cmd.ExecuteScalar()))
                    {
                        Settings.Default.IsDbCreated = true;
                        Settings.Default.SaveChanges();
                        return;
                    }
                    var scripts = Regex.Split(Resources.FileResources.DbCreateScript, @"GO", RegexOptions.Multiline);
                    foreach(var splitScript in scripts)
                    {
                        cmd.CommandText = splitScript;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            Settings.Default.IsDbCreated = true;
            Settings.Default.SaveChanges();
            SetSqlConnetion();
        }
        void LoadLocalInstance()
        {
            var instanceName = GetLocalSqlServerInstanceNames().FirstOrDefault() ?? throw new SqlExecutionException("Bilgisayarınızda kurulu herhangi bir SQL Server sürümü bulunamadı. Bu uygulama SQL Server üzerinden verileri depolayıp size sunmaktadır. Lütfen 2005 üstü versiyonuna sahip bir SQL Server kurun.");
            var serverName = $"{instanceName}\\{Environment.MachineName}";
            Settings.Default.LocalSqlServerName = serverName;
            Settings.Default.SaveChanges();
        }
        private static IList<string> GetLocalSqlServerInstanceNames()
        {
            RegistryValueDataReader registryValueDataReader = new RegistryValueDataReader();

            string[] instances64Bit = registryValueDataReader.ReadRegistryValueData(RegistryHive.Wow64,
                                                                                    Registry.LocalMachine,
                                                                                    @"SOFTWARE\Microsoft\Microsoft SQL Server",
                                                                                    "InstalledInstances");

            string[] instances32Bit = registryValueDataReader.ReadRegistryValueData(RegistryHive.Wow6432,
                                                                                    Registry.LocalMachine,
                                                                                    @"SOFTWARE\Microsoft\Microsoft SQL Server",
                                                                                    "InstalledInstances");

            IList<string> localInstanceNames = new List<string>(instances64Bit);

            localInstanceNames = localInstanceNames.Union(instances32Bit).ToList();

            return localInstanceNames;
        }
    }
}
