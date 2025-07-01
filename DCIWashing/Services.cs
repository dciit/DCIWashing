using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using DCIWashing.Props;
using System.Windows.Forms;

namespace EKBPartStock
{
    internal class Services
    {
        SqlConnectDB dbSCM = new SqlConnectDB("dbSCM");
        SoundPlayer pcs = new SoundPlayer(@"audio\pcs.wav");
        SoundPlayer increase = new SoundPlayer(@"audio\increase.wav");
        internal void PlaySoundPartName(string PartName, int qty)
        {
            increase.PlaySync();
            if (PartName == "BODY ASSY" || PartName == "BODY")
            {
                SoundPlayer body = new SoundPlayer(@"audio\body.wav");
                body.PlaySync();
            }

            else if (PartName == "CASING BOTTOM ASSY" || PartName == "BOTTOM")
            {
                SoundPlayer bottom = new SoundPlayer(@"audio\bottom.wav");
                bottom.PlaySync();
            }



            else if (PartName == "CRANK SHAFT")
            {
                SoundPlayer cs = new SoundPlayer(@"audio\cs.wav");
                cs.PlaySync();
            }



            else if (PartName == "FIXED SCROLL TYPICAL DWG." || PartName == "FS")
            {
                SoundPlayer fix = new SoundPlayer(@"audio\fix.wav");
                fix.PlaySync();
            }



            else if (PartName == "FIXED SCROLL SEMI-ASSY" || PartName == "FS")
            {
                SoundPlayer fix2 = new SoundPlayer(@"audio\fix.wav");
                fix2.PlaySync();
            }


            else if (PartName == "HOUSING TYPICAL DWG." || PartName == "HOUSING SCROLL")
            {
                SoundPlayer hs = new SoundPlayer(@"audio\hs.wav");
                hs.PlaySync();
            }



            else if (PartName == "LOWER B/R HOLDER" || PartName == "LW" || PartName == "LOWER MAIN BEARING METAL")
            {
                SoundPlayer lw = new SoundPlayer(@"audio\lw.wav");
                lw.PlaySync();
            }


            else if (PartName == "ORBITING SCROLL" || PartName == "OS")
            {
                SoundPlayer ob = new SoundPlayer(@"audio\ob.wav");
                ob.PlaySync();
            }


            else if (PartName == "CASING TOP ASSY" || PartName == "TOP")
            {
                SoundPlayer top = new SoundPlayer(@"audio\TOP.wav");
                top.PlaySync();
            }

            else if (PartName == "CYLINDER" || PartName == "CY")
            {
                SoundPlayer cy = new SoundPlayer(@"audio\CY.wav");
                cy.PlaySync();

            }

            else if (PartName == "LOWER CYLINDER")
            {
                SoundPlayer cy = new SoundPlayer(@"audio\CY.wav");
                cy.PlaySync();

                SoundPlayer lw = new SoundPlayer(@"audio\lw.wav");
                lw.PlaySync();
            }

            else if (PartName == "UPPER CYLINDER")
            {
                SoundPlayer cy = new SoundPlayer(@"audio\CY.wav");
                cy.PlaySync();

                SoundPlayer up = new SoundPlayer(@"audio\upper.wav");
                up.PlaySync();
            }


            else if (PartName == "FRONT HEAD" || PartName == "FH")
            {
                SoundPlayer fh = new SoundPlayer(@"audio\FH.wav");
                fh.PlaySync();
            }


            else if (PartName == "PISTON" || PartName == "PI")
            {
                SoundPlayer ps = new SoundPlayer(@"audio\PS.wav");
                ps.PlaySync();
            }

            else if (PartName == "REAR HEAD" || PartName == "RH")
            {
                SoundPlayer rh = new SoundPlayer(@"audio\RH.wav");
                rh.PlaySync();
            }



            ConvertToSound(qty);

            pcs.PlaySync();
        }
        public void ConvertToSound(int Qty)
        {

            if (Qty >= 1 && Qty <= 9) // 1-9
            {
                SoundPlayer sound = new SoundPlayer(@"audio\" + Qty + ".wav");
                sound.PlaySync();
            }

            else if (Qty >= 10 && Qty <= 99) // 10-99
            {
                if (Qty % 10 == 1) // 11,21,31,41,51,61,71,81,91
                {
                    if (Qty == 11) // 11
                    {
                        SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");
                        SoundPlayer oneone = new SoundPlayer(@"audio\oneone.wav");
                        ten.PlaySync();

                        oneone.PlaySync();


                    }

                    else if (Qty == 21) //21
                    {
                        SoundPlayer twotwo = new SoundPlayer(@"audio\twotwo.wav");
                        SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");
                        SoundPlayer oneone = new SoundPlayer(@"audio\oneone.wav");

                        twotwo.PlaySync();

                        ten.PlaySync();

                        oneone.PlaySync();

                    }
                    else
                    {
                        SoundPlayer sound = new SoundPlayer(@"audio\" + Qty / 10 + ".wav");
                        SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");
                        SoundPlayer oneone = new SoundPlayer(@"audio\oneone.wav");

                        sound.PlaySync();

                        ten.PlaySync();

                        oneone.PlaySync();
                    }

                }
                else if (Qty % 10 == 0) // 10,20,30,40,50,60,70,80,90
                {

                    if (Qty == 10) //10
                    {
                        SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");


                        ten.PlaySync();

                    }
                    else if (Qty == 20)
                    {
                        SoundPlayer twotwo = new SoundPlayer(@"audio\twotwo.wav");
                        SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");

                        twotwo.PlaySync();

                        ten.PlaySync();
                    }
                    else
                    {

                        SoundPlayer sound = new SoundPlayer(@"audio\" + Qty / 10 + ".wav");
                        SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");

                        sound.PlaySync();

                        ten.PlaySync();



                    }


                }
                else if (Qty / 10 < 2)
                {
                    SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");
                    SoundPlayer unit = new SoundPlayer(@"audio\" + Qty % 10 + ".wav");

                    ten.PlaySync();

                    unit.PlaySync();
                }
                else if (Qty / 10 < 3)
                {
                    SoundPlayer twotwo = new SoundPlayer(@"audio\twotwo.wav");
                    SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");
                    SoundPlayer unit = new SoundPlayer(@"audio\" + Qty % 10 + ".wav");

                    twotwo.PlaySync();

                    ten.PlaySync();

                    unit.PlaySync();

                }
                else
                {

                    SoundPlayer sound = new SoundPlayer(@"audio\" + Qty / 10 + ".wav");
                    SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");
                    SoundPlayer unit = new SoundPlayer(@"audio\" + Qty % 10 + ".wav");

                    sound.PlaySync();

                    ten.PlaySync();

                    unit.PlaySync();




                }
            }


            else if (Qty >= 100 && Qty <= 999) // 100-999
            {
                if (Qty % 100 == 0) // 100,200,300,400,500,600,700,800,900
                {
                    SoundPlayer sound = new SoundPlayer(@"audio\" + Qty / 100 + ".wav");
                    SoundPlayer hundred = new SoundPlayer(@"audio\hundred.wav");
                    sound.PlaySync();

                    hundred.PlaySync();
                }

                else
                {
                    SoundPlayer sound = new SoundPlayer(@"audio\" + Qty / 100 + ".wav");
                    SoundPlayer hundred = new SoundPlayer(@"audio\hundred.wav");
                    sound.PlaySync();

                    hundred.PlaySync();


                    int result = Qty % 100;

                    if (result >= 1 && result <= 9)
                    {
                        SoundPlayer unit = new SoundPlayer(@"audio\" + result + ".wav");
                        unit.PlaySync();
                    }


                    else if (result % 10 == 1) // 11,21,31,41,51,61,71,81,91
                    {


                        if (result == 11) // 11
                        {
                            SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");
                            SoundPlayer oneone = new SoundPlayer(@"audio\oneone.wav");
                            ten.PlaySync();

                            oneone.PlaySync();
                        }

                        else if (result == 21) //21
                        {
                            SoundPlayer twotwo = new SoundPlayer(@"audio\twotwo.wav");
                            SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");
                            SoundPlayer oneone = new SoundPlayer(@"audio\oneone.wav");

                            twotwo.PlaySync();

                            ten.PlaySync();

                            oneone.PlaySync();

                        }
                        else
                        {
                            SoundPlayer soundUnit = new SoundPlayer(@"audio\" + result / 10 + ".wav");
                            SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");
                            SoundPlayer oneone = new SoundPlayer(@"audio\oneone.wav");

                            soundUnit.PlaySync();

                            ten.PlaySync();

                            oneone.PlaySync();
                        }

                    }
                    else if (result % 10 == 0) // 10,20,30,40,50,60,70,80,90
                    {

                        if (result == 10) //10
                        {
                            SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");


                            ten.PlaySync();

                        }
                        else if (result == 20)
                        {
                            SoundPlayer twotwo = new SoundPlayer(@"audio\twotwo.wav");
                            SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");

                            twotwo.PlaySync();

                            ten.PlaySync();
                        }
                        else
                        {
                            SoundPlayer soundUnit = new SoundPlayer(@"audio\" + result / 10 + ".wav");
                            SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");

                            soundUnit.PlaySync();

                            ten.PlaySync();

                        }



                    }
                    else if (result / 10 < 2)
                    {
                        SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");
                        SoundPlayer unit = new SoundPlayer(@"audio\" + result % 10 + ".wav");

                        ten.PlaySync();

                        unit.PlaySync();
                    }
                    else if (result / 10 < 3)
                    {
                        SoundPlayer twotwo = new SoundPlayer(@"audio\twotwo.wav");
                        SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");
                        SoundPlayer unit = new SoundPlayer(@"audio\" + result % 10 + ".wav");

                        twotwo.PlaySync();

                        ten.PlaySync();

                        unit.PlaySync();

                    }
                    else
                    {

                        SoundPlayer soundTen = new SoundPlayer(@"audio\" + result / 10 + ".wav");
                        SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");
                        SoundPlayer unit = new SoundPlayer(@"audio\" + result % 10 + ".wav");

                        soundTen.PlaySync();

                        ten.PlaySync();

                        unit.PlaySync();




                    }

                }
            }



            else if (Qty >= 1000 && Qty <= 9999) // 1000 - 9999
            {

                int result = Qty % 1000;

                if (Qty % 1000 == 0)
                {
                    SoundPlayer sound = new SoundPlayer(@"audio\" + Qty / 1000 + ".wav");
                    SoundPlayer thousand = new SoundPlayer(@"audio\thousand.wav");
                    sound.PlaySync();
                    thousand.PlaySync();
                }



                else
                {
                    SoundPlayer soundstart = new SoundPlayer(@"audio\" + Qty / 1000 + ".wav");
                    SoundPlayer thousand = new SoundPlayer(@"audio\thousand.wav");
                    soundstart.PlaySync();
                    thousand.PlaySync();

                    if (result >= 1 && result <= 9) // 1-9
                    {
                        SoundPlayer sound = new SoundPlayer(@"audio\" + result + ".wav");
                        sound.PlaySync();
                    }

                    else if (result >= 10 && result <= 99) // 10-99
                    {
                        if (result % 10 == 1) // 11,21,31,41,51,61,71,81,91
                        {


                            if (result == 11) // 11
                            {
                                SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");
                                SoundPlayer oneone = new SoundPlayer(@"audio\oneone.wav");
                                ten.PlaySync();

                                oneone.PlaySync();


                            }

                            else if (result == 21) //21
                            {
                                SoundPlayer twotwo = new SoundPlayer(@"audio\twotwo.wav");
                                SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");
                                SoundPlayer oneone = new SoundPlayer(@"audio\oneone.wav");

                                twotwo.PlaySync();

                                ten.PlaySync();

                                oneone.PlaySync();

                            }
                            else
                            {
                                SoundPlayer sound = new SoundPlayer(@"audio\" + result / 10 + ".wav");
                                SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");
                                SoundPlayer oneone = new SoundPlayer(@"audio\oneone.wav");

                                sound.PlaySync();

                                ten.PlaySync();

                                oneone.PlaySync();
                            }

                        }
                        else if (result % 10 == 0) // 10,20,30,40,50,60,70,80,90
                        {

                            if (result == 10) //10
                            {
                                SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");


                                ten.PlaySync();

                            }
                            else if (result == 20)
                            {
                                SoundPlayer twotwo = new SoundPlayer(@"audio\twotwo.wav");
                                SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");

                                twotwo.PlaySync();

                                ten.PlaySync();
                            }
                            else
                            {

                                SoundPlayer sound = new SoundPlayer(@"audio\" + result / 10 + ".wav");
                                SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");

                                sound.PlaySync();

                                ten.PlaySync();



                            }


                        }
                        else if (result / 10 < 2)
                        {
                            SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");
                            SoundPlayer unit = new SoundPlayer(@"audio\" + result % 10 + ".wav");

                            ten.PlaySync();

                            unit.PlaySync();
                        }
                        else if (result / 10 < 3)
                        {
                            SoundPlayer twotwo = new SoundPlayer(@"audio\twotwo.wav");
                            SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");
                            SoundPlayer unit = new SoundPlayer(@"audio\" + result % 10 + ".wav");

                            twotwo.PlaySync();

                            ten.PlaySync();

                            unit.PlaySync();

                        }
                        else
                        {

                            SoundPlayer sound = new SoundPlayer(@"audio\" + result / 10 + ".wav");
                            SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");
                            SoundPlayer unit = new SoundPlayer(@"audio\" + result % 10 + ".wav");

                            sound.PlaySync();

                            ten.PlaySync();

                            unit.PlaySync();




                        }
                    }

                    else if (result >= 100 && result <= 999) // 100-999
                    {
                        if (result % 100 == 0) // 100,200,300,400,500,600,700,800,900
                        {
                            SoundPlayer sound = new SoundPlayer(@"audio\" + result / 100 + ".wav");
                            SoundPlayer hundred = new SoundPlayer(@"audio\hundred.wav");
                            sound.PlaySync();

                            hundred.PlaySync();
                        }

                        else
                        {
                            SoundPlayer sound = new SoundPlayer(@"audio\" + result / 100 + ".wav");
                            SoundPlayer hundred = new SoundPlayer(@"audio\hundred.wav");
                            sound.PlaySync();

                            hundred.PlaySync();


                            int result2 = result % 100;

                            if (result2 >= 1 && result2 <= 9)
                            {
                                SoundPlayer unit = new SoundPlayer(@"audio\" + result2 + ".wav");
                                unit.PlaySync();
                            }


                            else if (result2 % 10 == 1) // 11,21,31,41,51,61,71,81,91
                            {


                                if (result2 == 11) // 11
                                {
                                    SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");
                                    SoundPlayer oneone = new SoundPlayer(@"audio\oneone.wav");
                                    ten.PlaySync();

                                    oneone.PlaySync();
                                }

                                else if (result2 == 21) //21
                                {
                                    SoundPlayer twotwo = new SoundPlayer(@"audio\twotwo.wav");
                                    SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");
                                    SoundPlayer oneone = new SoundPlayer(@"audio\oneone.wav");

                                    twotwo.PlaySync();

                                    ten.PlaySync();

                                    oneone.PlaySync();

                                }
                                else
                                {
                                    SoundPlayer soundUnit = new SoundPlayer(@"audio\" + result2 / 10 + ".wav");
                                    SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");
                                    SoundPlayer oneone = new SoundPlayer(@"audio\oneone.wav");

                                    soundUnit.PlaySync();

                                    ten.PlaySync();

                                    oneone.PlaySync();
                                }

                            }
                            else if (result2 % 10 == 0) // 10,20,30,40,50,60,70,80,90
                            {

                                if (result2 == 10) //10
                                {
                                    SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");


                                    ten.PlaySync();

                                }
                                else if (result2 == 20)
                                {
                                    SoundPlayer twotwo = new SoundPlayer(@"audio\twotwo.wav");
                                    SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");

                                    twotwo.PlaySync();

                                    ten.PlaySync();
                                }
                                else
                                {
                                    SoundPlayer soundUnit = new SoundPlayer(@"audio\" + result2 / 10 + ".wav");
                                    SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");

                                    soundUnit.PlaySync();

                                    ten.PlaySync();

                                }



                            }
                            else if (result2 / 10 < 2)
                            {
                                SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");
                                SoundPlayer unit = new SoundPlayer(@"audio\" + result2 % 10 + ".wav");

                                ten.PlaySync();

                                unit.PlaySync();
                            }
                            else if (result2 / 10 < 3)
                            {
                                SoundPlayer twotwo = new SoundPlayer(@"audio\twotwo.wav");
                                SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");
                                SoundPlayer unit = new SoundPlayer(@"audio\" + result2 % 10 + ".wav");

                                twotwo.PlaySync();

                                ten.PlaySync();

                                unit.PlaySync();

                            }
                            else
                            {

                                SoundPlayer soundTen = new SoundPlayer(@"audio\" + result2 / 10 + ".wav");
                                SoundPlayer ten = new SoundPlayer(@"audio\ten.wav");
                                SoundPlayer unit = new SoundPlayer(@"audio\" + result2 % 10 + ".wav");

                                soundTen.PlaySync();

                                ten.PlaySync();

                                unit.PlaySync();




                            }

                        }
                    }
                }
            }
        }
        internal List<PropTransaction> GetTransactionOfDay(string configWcno, DateTime value, string shift)
        {
            string ymd = value.ToString("yyyyMMdd");
            string mechaWCNO = ("8" + configWcno.Substring(1, 2));
            List<PropTransaction> TransectionList = new List<PropTransaction>();
            SqlCommand sql = new SqlCommand();
            string str = $@"select FORMAT(CreateDate,'dd/MM/yyyy HH:mm:ss') Time,TRANS.PARTNO,TRANS.CM,TRANS.TransType,TRANS.TransQty,TRANS.CreateBy,COMMON.MODEL_COMMON,COMMON.PART_DESC from EKB_WIP_PART_STOCK_TRANSACTION TRANS 
LEFT JOIN vi_APS_PartInfo COMMON
ON COMMON.WCNO = '" + configWcno + "' AND COMMON.PARTNO = TRANS.PARTNO AND COMMON.CM = TRANS.CM where  TRANS.WCNO IN ('" + mechaWCNO + "','" + configWcno + "')  AND RefNo = 'WASHING-SYSTEM' AND YMD = '" + ymd + "' AND RefNo = 'WASHING-SYSTEM' ORDER BY  FORMAT(CreateDate,'dd/MM/yyyy HH:mm:ss') DESC";
            sql.CommandText = str;
            DataTable dt = dbSCM.Query(sql);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow drow in dt.Rows)
                {
                    PropTransaction Transection = new PropTransaction();
                    Transection.partno = drow["PARTNO"].ToString();
                    Transection.cm = drow["CM"].ToString();
                    //Transection.desc = drow["PART_DESC"].ToString();
                    //Transection.model = drow["MODEL_COMMON"].ToString();
                    Transection.time = drow["Time"].ToString();
                    Transection.type = drow["TransType"].ToString();
                    Transection.qty = Convert.ToDecimal(drow["TransQty"].ToString());
                    //Transection.CREATEBY = drow["CreateBy"].ToString();
                    TransectionList.Add(Transection);
                }

            }
            return TransectionList;
        }

       
    }

}
