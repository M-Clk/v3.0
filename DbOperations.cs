using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;
using Otomasyon.Properties;
using Otomasyon.Resources;

namespace Otomasyon
{
    public class DbOperations
    {
        public static RegistryKey SqlConfiguration =
            Registry.CurrentUser.CreateSubKey("SOFTWARE\\Otomasyon\\SqlServerConfiguration");

        public string bilgiMessage;
        public SqlCommand cmd;

        public static SqlConnection Connection { get; set; } = new SqlConnection();

        public static bool IsDbCreated => Convert.ToBoolean(SqlConfiguration.GetValue("DbCreated", false));

        public SqlDataReader SqlTextReader(string command)
        {
            cmd = new SqlCommand(command, Connection);
            Application.UseWaitCursor = true; //Fare işaretçisini bekleyen işaretçi ile değiştir.
            SqlDataReader dataRead;
            try //Bağlantı açılmazsa hata fırlatır ona göre işlem yap
            {
                Connection.OpenSafe();
                //if(thread.ThreadState==ThreadState.Running)
                //thread.Abort();
                Connection.InfoMessage += delegate(object senderMessage, SqlInfoMessageEventArgs eMessage)
                {
                    bilgiMessage = eMessage.ToString();
                };
                dataRead = cmd.ExecuteReader();
                Application.UseWaitCursor = false;
                return dataRead;
            }
            catch (Exception e)

            {
                MessageBox.Show(
                    "Veritabanına bağlanılamadı. Veritabanı bağlantı sorunu varken hiçbir işlem yapılamaz. Sunucuya erişim sağlandığından emin olun.\n Hata Mesajı :  " +
                    e.Message, "Sunucu Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            cmd = new SqlCommand(command, Connection);
            cmd.CommandType = CommandType.StoredProcedure;
            if (prm != null && prm.Length > 0)
                cmd.Parameters.AddRange(prm);
            //Thread thread = new Thread(new ThreadStart(Bekle));
            //thread.Start();
            Application.UseWaitCursor = true; //Fare işaretçisini bekleyen işaretçi ile değiştir.
            SqlDataReader dataRead;
            try //Bağlantı açılmazsa hata fırlatır ona göre işlem yap
            {
                Connection.OpenSafe();
                //if(thread.ThreadState==ThreadState.Running)
                //thread.Abort();
                Connection.InfoMessage += delegate(object senderMessage, SqlInfoMessageEventArgs eMessage)
                {
                    bilgiMessage = eMessage.ToString();
                };
                dataRead = cmd.ExecuteReader();
                Application.UseWaitCursor = false;
                return dataRead;
            }
            catch (Exception e)

            {
                Application.UseWaitCursor = false;
                if (e.Message.IndexOf("ENCOKSATANLAR") > -1)
                    using (cmd = new SqlCommand(
                               "CREATE Procedure ENCOKSATANLAR @GoruntelenecekAdet int, @Tur tinyint AS BEGIN if(@Tur=0) Select Top(@GoruntelenecekAdet) ROW_NUMBER() OVER(Order By Urunler.Adi) AS 'No',Barkod_Kodu,Urunler.Adi,SUM(Miktar) AS ToplamMiktar, Birimler.Adi from Satis_Detayi,Urunler,Birimler  Where Bakod_kodu=Barkod_Kodu AND Iade=0 AND Bakod_kodu!='0' AND Birimler.Id=Urunler.Stok_birimi Group By Barkod_Kodu,Urunler.Adi,Birimler.Adi Order By (ToplamMiktar) Desc else if(@Tur=1) Select Top(@GoruntelenecekAdet) ROW_NUMBER() OVER(Order By Urunler.Adi) AS 'No', Barkod_Kodu,Urunler.Adi,SUM(Miktar) AS ToplamMiktar, Birimler.Adi  from Satis_Detayi,Urunler,Birimler  Where Bakod_kodu=Barkod_Kodu AND Iade=0 AND Bakod_kodu!='0' AND Birimler.Id=Urunler.Stok_birimi Group By Barkod_Kodu,Urunler.Adi,Birimler.Adi Order By ToplamMiktar else Select ROW_NUMBER() OVER(Order By Urunler.Adi) AS 'No', Bakod_kodu,Urunler.Adi, (Select 0) AS ToplamMiktar, Birimler.Adi from Urunler,Birimler Where Bakod_Kodu NOT IN(Select Barkod_Kodu from Satis_Detayi) AND Bakod_kodu!='0' AND Birimler.Id=Urunler.Stok_birimi END",
                               Connection))
                    {
                        cmd.CommandType = CommandType.Text;
                        Connection.OpenSafe();
                        cmd.ExecuteNonQuery();
                        Connection.Close();
                    }
                else
                    //if(thread.ThreadState==ThreadState.Running)
                    //thread.Abort();
                    MessageBox.Show(
                        "Veritabanına bağlanılamadı. Veritabanı bağlantı sorunu varken hiçbir işlem yapılamaz. Sunucuya erişim sağlandığından emin olun.\n Hata Mesajı :  " +
                        e.Message, "Sunucu Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);

                dataRead = null;
                return dataRead;
            }
            finally
            {
                Application.UseWaitCursor = false;
            }
        }

        public object
            OkuScalar(string scalarCommand, CommandType cmdtype,
                SqlParameter[] prm) //Veritabanından tek sonuç istendiğinde çalıştır
        {
            using (cmd = new SqlCommand(scalarCommand, Connection))
            {
                cmd.CommandType = cmdtype;
                if (cmdtype == CommandType.StoredProcedure)
                    for (var i = 0; i < prm.Length; i++)
                        cmd.Parameters.Add(prm[i]);

                Application.UseWaitCursor = true;
                Connection.OpenSafe();
                Connection.InfoMessage += delegate(object senderMessage, SqlInfoMessageEventArgs eMessage)
                {
                    bilgiMessage = eMessage.ToString();
                };
                Application.UseWaitCursor = false;
                //thread1.Abort();
                var snc = cmd.ExecuteScalar();
                if (prm.Length > 0)
                    cmd.Parameters.Clear();
                return snc;
            }
        }

        public string ScalarTextCommand(string command)
        {
            using (cmd = new SqlCommand(command, Connection))
            {
                cmd.CommandType = CommandType.Text;
                try
                {
                    Connection.OpenSafe();

                    return cmd.ExecuteScalar()?.ToString();
                }
                catch (Exception e)
                {
                    MessageBox.Show(
                        "Veritabanına bağlanılamadı. Veritabanı bağlantı sorunu varken hiçbir işlem yapılamaz. Sunucuya erişim sağlandığından emin olun. \n Hata Mesajı :  " +
                        e.Message, "Sunucu Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return "";
                }
                finally
                {
                    Application.UseWaitCursor = false;
                }
            }
        }

        public bool TextCommand(string command)
        {
            using (cmd = new SqlCommand(command, Connection))
            {
                cmd.CommandType = CommandType.Text;
                Application.UseWaitCursor = true; //Fare işaretçisini bekleyen işaretçi ile değiştir.

                try //Bağlantı açılmazsa hata fırlatır ona göre işlem yap
                {
                    Connection.OpenSafe();
                    //if(thread.ThreadState==ThreadState.Running)
                    //thread.Abort();
                    Application.UseWaitCursor = false;
                    if (cmd.ExecuteNonQuery() > 0)
                        return true;

                    else
                        return false;
                }

                catch (SqlException e)
                {
                    Application.UseWaitCursor = false;
                    if (e.Message.IndexOf("UNIQUE KEY") >= 0 && e.Message.IndexOf("Kullanicilar") >= 0)
                        MessageBox.Show(
                            "Bu kullanıcı adına sahip bir kullanıcı zaten var. Lütfen farklı bir kullanıcı adı deneyin.",
                            "Veri Girişi Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    else if (e.Message.IndexOf("UNIQUE KEY") >= 0 && e.Message.IndexOf("Musteriler") >= 0)
                        MessageBox.Show(
                            "Bu numarayı kullanan bir müşteri zaten var. İkinci bir müşteri adına kaydedilemez.",
                            "Veri Girişi Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        MessageBox.Show(
                            "Veritabanına bağlanılamadı. Veritabanı bağlantı sorunu varken hiçbir işlem yapılamaz. Sunucuya erişim sağlandığından emin olun. \n Hata Mesajı :  " +
                            e.Message, "Sunucu Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return false;
                }
                finally
                {
                    Application.UseWaitCursor = false;
                }
            }
        }

        public int GuncelleProcedure(string procName, SqlParameter[] parameterCollection)
        {
            using (cmd = new SqlCommand(procName, Connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                for (var i = 0; i < parameterCollection.Length; i++)
                    cmd.Parameters.Add(parameterCollection[i]);

                //Thread thread = new Thread(new ThreadStart(Bekle));
                //thread.Start();
                Application.UseWaitCursor = true; //Fare işaretçisini bekleyen işaretçi ile değiştir.

                try //Bağlantı açılmazsa hata fırlatır ona göre işlem yap
                {
                    Connection.OpenSafe();
                    Connection.InfoMessage += delegate(object senderMessage, SqlInfoMessageEventArgs eMessage)
                    {
                        bilgiMessage = eMessage.ToString();
                    };
                    //if(thread.ThreadState==ThreadState.Running)
                    //thread.Abort();
                    Application.UseWaitCursor = false;
                    if (cmd.ExecuteNonQuery() > 0)
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

                catch (SqlException e)

                {
                    //if(thread.ThreadState==ThreadState.Running)
                    //thread.Abort();

                    Application.UseWaitCursor = false;
                    if (e.Message.IndexOf("UNIQUE KEY") >= 0 && e.Message.IndexOf("Kullanicilar") >= 0)
                        MessageBox.Show(
                            "Bu kullanıcı adına sahip bir kullanıcı zaten var. Lütfen farklı bir kullanıcı adı deneyin.",
                            "Veri Girişi Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else if (e.Message.IndexOf("UNIQUE KEY") >= 0 && e.Message.IndexOf("Musteriler") >= 0)
                        MessageBox.Show(
                            "Bu numarayı kullanan bir müşteri zaten var. İkinci bir müşteri adına kaydedilemez.",
                            "Veri Girişi Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        MessageBox.Show(
                            "Veritabanına bağlanılamadı. Veritabanı bağlantı sorunu varken hiçbir işlem yapılamaz. Sunucuya erişim sağlandığından emin olun. \n Hata Mesajı :  " +
                            e.Message, "Sunucu Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return 0;
                }
                finally
                {
                    Application.UseWaitCursor = false;
                }
            }
        }

        public DataTable DisconnectedProcedure(string procName, SqlParameter[] parameterCollection)
        {
            using (var cmd = new SqlCommand(procName, Connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                for (var i = 0; i < parameterCollection.Length; i++) cmd.Parameters.Add(parameterCollection[i]);

                using (var dAdapter = new SqlDataAdapter(cmd))
                {
                    using (var table = new DataTable())
                    {
                        table.Clear();
                        dAdapter.Fill(table);
                        return table;
                    }
                }
            }
        }

        public List<string> CreateDb()
        {
            Connection.EnsureDbCreated();
            if (Settings.Default.IsClient)
                throw new InvalidOperationException(
                    "Bu bilgisayar bir istemci olarak ayarlanmış. Ancak SQL Server yüklü herhangi bir sunucu ile bağlantı için yapılandırılmamış. Lütfen Yapılandırma Merkezi sayfasından sunucu bilgisayar ile bağlantı kurun.");
            var listOfFailedCommands = new List<string>();
            using (cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.Connection = Connection;
                Connection.OpenSafe();
                var scripts = Regex.Split(FileResources.DbCreateScript, @"GO", RegexOptions.Multiline);
                foreach (var splitedScript in scripts)
                {
                    cmd.CommandText = splitedScript;
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        if (!splitedScript.Replace(Environment.NewLine, "")
                                .StartsWith("Drop", StringComparison.InvariantCultureIgnoreCase))
                            listOfFailedCommands.Add(splitedScript);
                    }
                }
            }

            SetDbCreatedFlag();
            return listOfFailedCommands;
        }

        public static void InitializeSqlConfiguration()
        {
            var serverName = (string)SqlConfiguration.GetValue("Server", "");
            var userName = (string)SqlConfiguration.GetValue("DBUserName", "");
            var password = (string)SqlConfiguration.GetValue("DBUserPassword", "");
            var timeout = (int)SqlConfiguration.GetValue("Timeout", 5);
            var dbName = IsDbCreated ? "OtomasyonDb" : "master";
            Connection.Close();
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
                Connection.ConnectionString =
                    @$"Server={serverName};DataBase={dbName};Trusted_Connection=True;Connection Timeout={timeout}";
            Connection.ConnectionString =
                @$"Server={serverName};Database={dbName};User ID={userName};Password ={password};Trusted_Connection=True;Connection Timeout={timeout}";
        }

        public static void SetSqlConfiguration(string server, string userName, string password, int timeout = 5)
        {
            SqlConfiguration.SetValue("Server", server);
            SqlConfiguration.SetValue("DBUserName", userName);
            SqlConfiguration.SetValue("DBUserPassword", password);
            SqlConfiguration.SetValue("Timeout", timeout);
            SetDbCreatedFlag();
        }

        private static void SetDbCreatedFlag()
        {
            SqlConfiguration.SetValue("DbCreated", Connection.IsDatabaseExist());
            InitializeSqlConfiguration();
        }
    }
}