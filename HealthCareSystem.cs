using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.Numerics;

namespace HealthCareSystem
{
    public class Contract1 : SmartContract
    {
        public static readonly byte[] Owner = "AMmdFAutFquNwvhFWUdSxnbn4bwMnwvkq8".ToScriptHash();

        private struct Patient
        {
            public BigInteger Date;
            public byte[] Id_Number;
            public byte[] Patient_Name;
            public byte[] Patient_Age;
            public byte[] Patient_Address;
            public byte[] Disease;
            public byte[] Status ;
            public BigInteger NoD;
           
        }



        private static Patient NewPatient(BigInteger date,byte[] id_Number,
            byte[] patient_Name,byte[] patient_Age,byte[] patient_Address,byte[] disease,byte[] status,BigInteger noD)
        {
            Runtime.Notify("AP");
            
            return new Patient
            {
                Date = date,
                Id_Number = id_Number,
                Patient_Name = patient_Name,
                Patient_Age = patient_Age,
                Patient_Address = patient_Address,
                Disease = disease,
                Status = status,
                NoD = noD,
    };
        }

        private static readonly byte[] Empty = { };
        private static readonly byte[] Admit = "Admit".AsByteArray();
        private static readonly byte[] Discharge = "Discharge".AsByteArray();

        public static object Main(string operation, params object[] args)
        {

            if (Runtime.CheckWitness(Owner))
            {
                if (operation == "AddPatient")
                {
                    BigInteger Now = Runtime.Time;
                    byte[] Date = (byte[])args[0];

                    var entry = NewPatient(
                        Now,
                         GetId().AsByteArray(), (byte[])args[1],
                        (byte[])args[2], (byte[])args[3], (byte[])args[4],
                        Admit, 0
                    );

                    Runtime.Notify("Entered:", entry);
                    return AddPatient(Date,entry);
                }




                if (operation == "DischargePatient")
                {
                    byte[] Date = (byte[])args[0];
                    byte[] Id = (byte[])args[1];
                    byte[] name = (byte[])args[2];

                    byte[] entry = Storage.Get(Storage.CurrentContext, Id.Concat(name));

              
                    Patient st = (Patient)entry.Deserialize();
                   
                  
                    
                  
                    Storage.Put(Storage.CurrentContext, Admit.Concat(Id), 0);
                    Storage.Put(Storage.CurrentContext, Discharge.Concat(Id), 1);

                    

                    bool ss = StatusEntry(Date,st);

                    Runtime.Notify("DP..");

                    return ss;

                }

            }


            if (operation == "getTodaysNumberofAdmit")
            {

                return  Get_Todays_Number_of_Admit((byte[])args[0]);
               
            }

            if (operation == "getTotalNumberofAdmit")
            {

                return Get_Total_Number_of_Admit();

            }

            if (operation == "getTotalNumberofDischarge")
            {

                return Get_Total_Number_of_Discharge();

            }

            if (operation == "getTodaysNumberofDischarge")
            {

                return Get_Todays_Number_of_Discharge((byte[]) args[0]);

            }

            if (operation == "getPatientDetails")
            {

                return GetPatientDetails((byte[])args[0],(byte[])args[1]);

            }



            if (operation == "getTotalNumberOfPatient")
            {

                return GetTotalNumberOfPatient();

            }

            if (operation == "getForList")
            {

                return GetForList((byte[])args[0]);

            }



            if (operation == "getPatientList") return GetPatientList((byte[])args[0]);

            if (operation == "getPatientList1") return GetPatientList1((byte[])args[0]);

            if (operation == "getPatientList2") return GetPatientList2((byte[])args[0]);


            if (operation == "ppatientdetails") return ppatientdetails((byte[])args[0]);

            if (operation == "ppatientdetailsd") return ppatientdetailsd((byte[])args[0]);

            if (operation == "ppatientdetailsd1") return ppatientdetailsd1((byte[])args[0], (byte[])args[1]);

            if (operation == "getPD") return GetPD((byte[])args[0]);

            return true;
        }



        private static bool AddPatient(byte[] Date,Patient entry)
        {
           
            Storage.Put(Storage.CurrentContext, (entry.Id_Number).Concat(entry.Patient_Name), entry.Serialize());
            Storage.Put(Storage.CurrentContext, entry.Id_Number, entry.Serialize());
            Storage.Put(Storage.CurrentContext, ("New_Patient".AsByteArray()).Concat(entry.Id_Number),
                 entry.Id_Number.Concat(("-").AsByteArray()).Concat(entry.Patient_Name).Concat(("-").AsByteArray()).Concat(entry.Patient_Age).Concat(("-").AsByteArray()).Concat(entry.Patient_Address).Concat(("-").AsByteArray()).Concat(entry.Disease).Concat(("-").AsByteArray()).Concat(entry.Status).Concat(("-").AsByteArray()).Concat((entry.NoD).AsByteArray()));
            Storage.Put(Storage.CurrentContext, Admit.Concat(entry.Id_Number), 1);
            bool k = StatusEntry(Date,entry);

            Runtime.Notify(" Successfully");
            return k;

        }


        private static Patient GetPatientDetails(byte[] id, byte[] name)
        {
            byte[] k = Storage.Get(Storage.CurrentContext, id.Concat(name));
            Patient pd = (Patient)k.Deserialize();
            Runtime.Notify("P D : ", pd);
            return pd;
        }

            private static BigInteger GetId()
        {
            BigInteger Id = Storage.Get(Storage.CurrentContext, "Id_Number").AsBigInteger() + 1;
            BigInteger Number = Storage.Get(Storage.CurrentContext, "Number_of_Admitted").AsBigInteger() + 1;
            Storage.Put(Storage.CurrentContext, "Id_Number", Id);
            Storage.Put(Storage.CurrentContext, "Number_of_Admitted", Number);

            Runtime.Notify("Id",Id);
            return Id;
        }


        private static byte[] GetStatus(byte[] Id,byte[] name)
        {
            byte[] Concat_Status = Admit.Concat(Id);
            BigInteger Status = (Storage.Get(Storage.CurrentContext, Concat_Status)).AsBigInteger();

            Runtime.Notify("Status",Status);

            if (Status==1)
            {
                
                return Admit;

            }
            else 
            {
                return Discharge;
            }

        }

        private static bool StatusEntry(byte[] Date, Patient detail)
        {
            byte[] sts = GetStatus(detail.Id_Number, detail.Patient_Name);

            if (sts == Admit)
            {
                byte[] Concat_Date = ("Number_of_Admitted".AsByteArray()).Concat((Date));
                BigInteger Today_Admission = Storage.Get(Storage.CurrentContext, Concat_Date).AsBigInteger() + 1;
                Storage.Put(Storage.CurrentContext, Concat_Date, Today_Admission);
                Runtime.Notify("A......");
                return true;
            }

            else if (sts == Discharge)
            {
                detail.Status = Discharge;

                detail.NoD = (((detail.Date - Runtime.Time) / 60) / 60) / 24;


                Storage.Put(Storage.CurrentContext, (detail.Id_Number).Concat(detail.Patient_Name), detail.Serialize());
                byte[] Concat_Date_1 = ("Number_of_Discharged".AsByteArray()).Concat((Date));
                BigInteger Today_Discharge = Storage.Get(Storage.CurrentContext, Concat_Date_1).AsBigInteger() + 1;
                Storage.Put(Storage.CurrentContext, Concat_Date_1, Today_Discharge);

                Storage.Put(Storage.CurrentContext, detail.Id_Number, detail.Serialize());
                Storage.Put(Storage.CurrentContext, ("New_Patient".AsByteArray()).Concat(detail.Id_Number), 
                 detail.Id_Number.Concat(("-").AsByteArray()).Concat(detail.Patient_Name).Concat(("-").AsByteArray()).Concat(detail.Patient_Age).Concat(("-").AsByteArray()).Concat(detail.Patient_Address).Concat(("-").AsByteArray()).Concat(detail.Disease).Concat(("-").AsByteArray()).Concat(detail.Status).Concat(("-").AsByteArray()).Concat((detail.NoD).AsByteArray()));

                BigInteger Decrease_Admission = Storage.Get(Storage.CurrentContext, "Number_of_Admitted").AsBigInteger() - 1;
                Storage.Put(Storage.CurrentContext, "Number_of_Admitted", Decrease_Admission);

                BigInteger Increase_Discharge = Storage.Get(Storage.CurrentContext, "Number_of_Discharged").AsBigInteger() + 1;
                Storage.Put(Storage.CurrentContext, "Number_of_Discharged", Increase_Discharge);
                Runtime.Notify("D.....");
                return true;
            }
            return false;

          
        }

        private static byte[] Get_Todays_Number_of_Admit(byte[] Date)
        {            


            byte[] Concat_Date1 = ("Number_of_Admitted".AsByteArray()).Concat((Date));
            Runtime.Notify("T N o A : ", Storage.Get(Storage.CurrentContext, Concat_Date1));
            return (Storage.Get(Storage.CurrentContext, Concat_Date1));

            
        }

        private static byte[] Get_Total_Number_of_Admit()
        {
           Runtime.Notify("Tt N O A : ", Storage.Get(Storage.CurrentContext, "Number_of_Admitted".AsByteArray()));


            return (Storage.Get(Storage.CurrentContext, "Number_of_Admitted".AsByteArray()));
        }
        private static byte[] Get_Total_Number_of_Discharge()
        {
            Runtime.Notify("Tt N O D : ", Storage.Get(Storage.CurrentContext, "Number_of_Discharged".AsByteArray()));
            return (Storage.Get(Storage.CurrentContext, "Number_of_Discharged".AsByteArray()));
        }

        private static byte[] Get_Todays_Number_of_Discharge(byte[] Date)
        {

            byte[] Concat_Date1 = ("Number_of_Discharged".AsByteArray()).Concat((Date));
            Runtime.Notify("T N O D : ", Storage.Get(Storage.CurrentContext, Concat_Date1));
            return (Storage.Get(Storage.CurrentContext, Concat_Date1));
        }




        private static BigInteger GetTotalNumberOfPatient()
        {
          
            return (Storage.Get(Storage.CurrentContext, "Id_Number").AsBigInteger());
        }



        private static byte[] GetPD(byte[] ID)
        {

            return (Storage.Get(Storage.CurrentContext, ("New_Patient".AsByteArray()).Concat(ID)));
        }


        private static Patient GetForList(byte[] Id)
        {

            
            byte[] k = Storage.Get(Storage.CurrentContext, Id);
            
            Patient s = (Patient) k.Deserialize();

            return s;

        }






        private static Patient[] GetPatientList(byte[] Id)
        {
            var result = new Patient[30];

            var it = Storage.Find(Storage.CurrentContext, Id);

          

            var i = 0;
           while (it.Next() && i < 30)
            {
                var value = it.Value;
                var bytes = value.Deserialize();
                var offer = (Patient)bytes;
                result[i] = offer;
                i++;
            }

            return result;
        }




        private static Patient GetPatientList1(byte[] Id)
        {
          

            var it = Storage.Get(Storage.CurrentContext, Id);
                    
            
                var bytes = it.Deserialize();
                var offer = (Patient)bytes;          
                
           

            return offer;
        }




        private static object GetPatientList2(byte[] Id)
        {


            var it = Storage.Get(Storage.CurrentContext, Id);




            return it.Deserialize();
        }






        //change


        private static Patient ppatientdetails(byte[] id)
        {
            byte[] patdetails = Storage.Get(Storage.CurrentContext, id);
            if (patdetails.Length == 0) return new Patient();

            return (Patient)patdetails.Deserialize();
        }

        private static Patient[] ppatientdetailsd(byte[] id)
        {
            var result = new Patient[50];

            var it = Storage.Find(Storage.CurrentContext, id);

              

            var i = 0;
            while (it.Next() && i < 50)
            {
                var value = it.Value;
                var bytes = value.Deserialize();
                var offer = (Patient)bytes;
                result[i] = offer;
                i++;
            }

            return result;
        }

        private static Patient[] ppatientdetailsd1(byte[] id, byte[] offset)
        {
            var result = new Patient[50];

            var it = Storage.Find(Storage.CurrentContext, id);

            if (offset != Empty)
            {
                while (it.Next())
                {
                    if (it.Value == offset) break;
                }
            }

            var i = 0;
            while (it.Next() && i < 50)
            {
                var value = it.Value;
                var bytes = value.Deserialize();
                var offer = (Patient)bytes;
                result[i] = offer;
                i++;
            }

            return result;
        }



    }
}
