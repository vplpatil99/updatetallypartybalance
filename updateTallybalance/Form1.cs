using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

namespace updateTallybalance
{
    public partial class Form1 : Form
    {
        SqlConnection online,onlineTallyDB;
        SqlConnection local;
        SqlDataReader SourceReader, DestinationReader;
        String connectionString,city_id;


        SqlCommand SourceCmdRetrive, DestinationCmdRetrive, DestinationCmdUpdate, DestinationCmdInsert;


        public Form1()
        {

            InitializeComponent();
            String line;
            using (StreamReader reader = new StreamReader("ServerConfig.txt"))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("connectionString="))
                    {
                        connectionString = line.Substring(line.IndexOf("=") + 1);
                    }
                    else if (line.Contains("city_id="))
                    {
                        city_id = line.Substring(line.IndexOf("=") + 1);
                    }
                }

                reader.Close();
            }            
            online = new SqlConnection(ConfigurationManager.ConnectionStrings["tallyserver175"].ConnectionString);
            onlineTallyDB = new SqlConnection(ConfigurationManager.ConnectionStrings["tallyserverTallyDB"].ConnectionString);
            local = new SqlConnection(connectionString);
            //Destination = new SqlConnection(ConfigurationManager.ConnectionStrings["local"].ConnectionString);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //BulkCopy();
            //onlineTallyDB.Open();

            //DestinationCmdRetrive = new SqlCommand("select * from logger where day(log_date)=day(getdate()) and month(log_date)=month(getdate()) and year(log_date)=year(getdate()) and city_id="+city_id, onlineTallyDB);
            //DestinationReader = DestinationCmdRetrive.ExecuteReader();
            //if (DestinationReader.Read())
            //{
            //    onlineTallyDB.Close();
            //    DestinationReader.Close();
            //}
            //else
            //{
            //    onlineTallyDB.Close();
            //    DestinationReader.Close();
                
            //    onlineTallyDB.Open();

            //    DestinationCmdInsert = new SqlCommand("insert into logger values(getdate(),"+city_id+")", onlineTallyDB);
            //    DestinationCmdInsert.ExecuteNonQuery();

            //    onlineTallyDB.Close();
            //    DestinationReader.Close();
            //    UpdatePartyCode();

            //}

            updatebalance();
            updateOnlineFlag();
            updateLocalFlag();
            Application.Exit();
        }


        private void updateOnlineFlag()
        {
            try
            {
                SourceCmdRetrive = new SqlCommand("select party_code,party_name,updatedate,activatedyn,creditpolicyapplicable,maxcrperiodindays,isnull(mobilemail,0),isnull(mobilemailaddress,0) from partydetails", local);
                online.Open();
                local.Open();
                SourceReader = SourceCmdRetrive.ExecuteReader();

                while (SourceReader.Read())
                {
                    if (SourceReader.GetValue(0) != null && SourceReader.GetValue(1) != null)
                    {
                        string party_code = SourceReader.GetValue(0).ToString();
                        string party_name = SourceReader.GetValue(1).ToString();
                        DateTime updatedate = Convert.ToDateTime(SourceReader.GetValue(2).ToString());
                        string activatedyn = SourceReader.GetValue(3).ToString();
                        string creditpolicyapplicable = SourceReader.GetValue(4).ToString();
                        string maxcrperiodindays = SourceReader.GetValue(5).ToString();
                        string mobilemail = SourceReader.GetValue(6).ToString();
                        string mobilemailaddress = SourceReader.GetValue(7).ToString();



                        DestinationCmdRetrive = new SqlCommand("select * from partydetails where party_code='" + party_code + "' and updatedate<'" + updatedate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' and city_id=" + city_id + " and party_name='"+party_name+"'", online);

                        DestinationReader = DestinationCmdRetrive.ExecuteReader();
                        if (DestinationReader.Read())
                        {
                            //MessageBox.Show("Updated");
                            DestinationReader.Close();
                            DestinationCmdUpdate = new SqlCommand("Update partydetails set updatedate='" + updatedate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "',activatedyn='" + activatedyn + "',creditpolicyapplicable='" + creditpolicyapplicable + "',maxcrperiodindays='" + maxcrperiodindays + "',mobilemail='" + mobilemail + "',mobilemailaddress='" + mobilemailaddress + "' where party_code='" + party_code + "' and party_name='" + party_name + "' and city_id='" + city_id + "'", online);
                            DestinationCmdUpdate.ExecuteNonQuery();
                        }
                        else
                        {
                            //MessageBox.Show(" not Updated");
                            DestinationReader.Close();
                            /*DestinationCmdInsert = new SqlCommand("insert into TallyPartyBalance (tallypartyname,balamt,updatedate,updatetime) values('" + tallypartyname + "','" + balance + "','" + DateTime.Now.ToString("yyyyMMdd") + "','" + DateTime.Now.ToString("yyyyMMdd") + "')", Destination);
                            DestinationCmdInsert.ExecuteNonQuery();
                            DestinationReader.Close();*/
                        }

                        //DestinationCmdRetrive.Dispose();


                    }
                }
                SourceReader.Close();
                SourceCmdRetrive.Dispose();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                online.Close();
                local.Close();
            }

        }
        private void updateLocalFlag()
        {
            try
            {
                SourceCmdRetrive = new SqlCommand("select party_code,party_name,updatedate,activatedyn,creditpolicyapplicable,maxcrperiodindays,isnull(mobilemail,0),isnull(mobilemailaddress,0) from partydetails where city_id=" + city_id, online);
                online.Open();
                local.Open();
                SourceReader = SourceCmdRetrive.ExecuteReader();

                while (SourceReader.Read())
                {
                    if (SourceReader.GetValue(0) != null && SourceReader.GetValue(1) != null)
                    {
                        string party_code = SourceReader.GetValue(0).ToString();
                        string party_name = SourceReader.GetValue(1).ToString();
                        DateTime updatedate = Convert.ToDateTime(SourceReader.GetValue(2).ToString());
                        string activatedyn = SourceReader.GetValue(3).ToString();
                        string creditpolicyapplicable = SourceReader.GetValue(4).ToString();
                        string maxcrperiodindays = SourceReader.GetValue(5).ToString();
                        string mobilemail = SourceReader.GetValue(6).ToString();
                        string mobilemailaddress = SourceReader.GetValue(7).ToString();



                        DestinationCmdRetrive = new SqlCommand("select * from partydetails where party_code='" + party_code + "' and updatedate<'" + updatedate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' and party_name='"+party_name+"'", local);
                        

                        DestinationReader = DestinationCmdRetrive.ExecuteReader();
                        if (DestinationReader.Read())
                        {
                            //MessageBox.Show("Updated");
                            DestinationReader.Close();
                            DestinationCmdUpdate = new SqlCommand("Update partydetails set updatedate='" + updatedate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "',activatedyn='" + activatedyn + "',creditpolicyapplicable='" + creditpolicyapplicable + "',maxcrperiodindays='" + maxcrperiodindays + "',mobilemail='" + mobilemail + "',mobilemailaddress='" + mobilemailaddress + "' where party_code='" + party_code + "' and party_name='" + party_name + "'", local);
                            DestinationCmdUpdate.ExecuteNonQuery();
                        }
                        else
                        {
                            //MessageBox.Show(" not Updated");
                            DestinationReader.Close();
                            /*DestinationCmdInsert = new SqlCommand("insert into TallyPartyBalance (tallypartyname,balamt,updatedate,updatetime) values('" + tallypartyname + "','" + balance + "','" + DateTime.Now.ToString("yyyyMMdd") + "','" + DateTime.Now.ToString("yyyyMMdd") + "')", Destination);
                            DestinationCmdInsert.ExecuteNonQuery();
                            DestinationReader.Close();*/
                        }

                        //DestinationCmdRetrive.Dispose();


                    }
                }
                SourceReader.Close();
                SourceCmdRetrive.Dispose();
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
            }
            finally
            {
                online.Close();
                local.Close();
            }

        }
        private void updatebalance()
        {
            try
            {
                SourceCmdRetrive = new SqlCommand("SELECT party_code,party_name,tallypartyname,balamt,collection,updatedate,updatetime FROM tallypartybalance where city_id=" + city_id.Trim(), onlineTallyDB);
                onlineTallyDB.Open();
                local.Open();
                SourceReader = SourceCmdRetrive.ExecuteReader();

                while (SourceReader.Read())
                {
                    if (SourceReader.GetValue(2) != null && SourceReader.GetValue(3) != null)
                    {
                        double balance = Convert.ToDouble(SourceReader.GetValue(3));
                        string tallypartyname = SourceReader.GetValue(2)+"";


                        DestinationCmdRetrive = new SqlCommand("select * from tallypartybalance where tallypartyname='" + tallypartyname + "'", local);

                        DestinationReader = DestinationCmdRetrive.ExecuteReader();
                        if (DestinationReader.Read())
                        {
                            //MessageBox.Show("updated");
                            DestinationReader.Close();
                            DestinationCmdUpdate = new SqlCommand("Update tallypartybalance set balamt='" + balance + "',updatedate='" + DateTime.Now.ToString("yyyyMMdd") + "',updatetime='" + DateTime.Now.ToString("yyyyMMdd") + "' where tallypartyname='" + tallypartyname + "'", local);
                            DestinationCmdUpdate.ExecuteNonQuery();
                        }
                        else
                        {
                            DestinationReader.Close();
                            
                            DestinationCmdInsert = new SqlCommand("insert into TallyPartyBalance (tallypartyname,balamt,updatedate,updatetime) values('" + tallypartyname + "','" + balance + "','" + DateTime.Now.ToString("yyyyMMdd") + "','" + DateTime.Now.ToString("yyyyMMdd") + "')", local);
                            DestinationCmdInsert.ExecuteNonQuery();
                            DestinationReader.Close();
                        }

                        //DestinationCmdRetrive.Dispose();


                    }
                   // MessageBox.Show("ledger downloaded");
                }
                SourceReader.Close();
                SourceCmdRetrive.Dispose();
            }
            catch (Exception e)
            {
               // MessageBox.Show(e.Message);
            }
            finally
            {
                local.Close();
                onlineTallyDB.Close();
            }

        }
        private void UpdatePartyCode()
        {
            try
            {
                SourceCmdRetrive = new SqlCommand("SELECT party_code,party_name,tallypartyname,balamt,collection,updatedate,updatetime FROM tallypartybalance where party_code is not null", local);
                onlineTallyDB.Open();
                local.Open();
                SourceReader = SourceCmdRetrive.ExecuteReader();

                while (SourceReader.Read())
                {
                    if (SourceReader.GetValue(2) != null && SourceReader.GetValue(3) != null)
                    {
                        double balance = Convert.ToDouble(SourceReader.GetValue(3));
                        string tallypartyname = SourceReader.GetValue(2).ToString();
                        string party_code = SourceReader.GetValue(0).ToString();


                        DestinationCmdRetrive = new SqlCommand("select * from tallypartybalance where tallypartyname='" + tallypartyname + "' and city_id=" + city_id.Trim(), onlineTallyDB);

                        DestinationReader = DestinationCmdRetrive.ExecuteReader();
                        if (DestinationReader.Read())
                        {
                           // MessageBox.Show("party code updated");
                            DestinationReader.Close();
                            DestinationCmdUpdate = new SqlCommand("Update tallypartybalance set party_code='" + party_code + "',updatedate='" + DateTime.Now.ToString("yyyyMMdd") + "',updatetime='" + DateTime.Now.ToString("yyyyMMdd") + "' where tallypartyname='" + tallypartyname + "' and city_id=" + city_id.Trim(), onlineTallyDB);
                            DestinationCmdUpdate.ExecuteNonQuery();
                        }
                        else
                        {
                            DestinationReader.Close();
                            /*DestinationCmdInsert = new SqlCommand("insert into TallyPartyBalance (tallypartyname,balamt,updatedate,updatetime) values('" + tallypartyname + "','" + balance + "','" + DateTime.Now.ToString("yyyyMMdd") + "','" + DateTime.Now.ToString("yyyyMMdd") + "')", local);
                            DestinationCmdInsert.ExecuteNonQuery();
                            DestinationReader.Close();*/
                        }

                        //DestinationCmdRetrive.Dispose();


                    }
                }
                SourceReader.Close();
                SourceCmdRetrive.Dispose();
               // MessageBox.Show("party_code updated");
            }
            catch (Exception e)
            {
               // MessageBox.Show(e.Message);
            }
            finally
            {
                local.Close();
                onlineTallyDB.Close();
            }

        }

    }
}
