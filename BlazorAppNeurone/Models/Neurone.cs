using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlazorAppNeurone.Models
{
    internal class Neurone
    {
        static int[,] matriceInput = { { 0, 0, 1, 1 }, { 0, 1, 0, 1 } };
        static int[] vettoreTarget = { 0, 0, 0, 1 };

        byte nInput;
        decimal bias;
        decimal[] vettorePesi;
        decimal learningRate;
        int epoca;
        bool converge;

        public Neurone(byte NInput, decimal Bias, decimal LearningRate)
        {
            nInput = NInput;
            bias = Bias;
            learningRate = LearningRate;
            epoca = 0;
            converge = false;

            Random rnd = new Random();
            vettorePesi = new decimal[NInput + 1];
            for (int i = 0; i < NInput + 1; i++)
            {
                vettorePesi[i] = rnd.Next(-5, 5) + (decimal)Math.Round(rnd.NextDouble(), 1);
            }
        }

        public Neurone(byte NInput, decimal Bias, decimal LearningRate, decimal[] Pesi)
        {
            nInput = NInput;
            bias = Bias;
            learningRate = LearningRate;
            epoca = 0;
            converge = false;
            vettorePesi = Pesi;
        }

        public int NuovaEpoca()
        {
            int nRighe = matriceInput.GetLength(1);
            int erroreTotale = 0;
            for (int i = 0; i < nRighe; i++)
            {
                byte attivazione = Attiva((byte)i);
                erroreTotale += Math.Abs(vettoreTarget[i] - attivazione);
                for (int j = 0; j < nInput + 1; j++)
                {
                    if(j == 0)
                    {
                        vettorePesi[j] += learningRate * bias * (vettoreTarget[i] - attivazione);
                    }
                    else
                    {
                        vettorePesi[j] += learningRate * matriceInput[j - 1, i] * (vettoreTarget[i] - attivazione);
                    }
                }
            }
            converge = (erroreTotale == 0) ? true : false;

            epoca++;
            return epoca;
        }

        public byte Attiva(byte NTrainingRow)
        {
            decimal sommaPesata = 0;
            for (int i = 0; i < vettorePesi.Length; i++)
            {
                if(i > 0)
                {
                    sommaPesata += matriceInput[i - 1, NTrainingRow] * vettorePesi[i];
                }
                else
                {
                    sommaPesata += bias * vettorePesi[i];
                }
            }

            return (byte)((sommaPesata > 0) ? 1 : 0);
        }

        public static void SerializzaTestSet()
        {
            string serMatriceInput = JsonConvert.SerializeObject(matriceInput);
            string serVettoreTarget = JsonConvert.SerializeObject(vettoreTarget);
            string serializedTestSet = "{matriceInput:" + serMatriceInput + ", vettoreTarget:" + serVettoreTarget + "}";
            Console.WriteLine(serializedTestSet);
            File.WriteAllText("../../../testSet.json", serializedTestSet);
        }

        public static void DeserializzaTestSet()
        {
            JObject testSet = JObject.Parse(File.ReadAllText("../../../testSet.json"));
            matriceInput = testSet["matriceInput"].ToObject<int[,]>();
            vettoreTarget = testSet["vettoreTarget"].ToObject<int[]>();
        }

        public decimal[] Pesi { get => vettorePesi; }
        public bool Converge { get => converge; }
    }
}
