using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Controls;
using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;
using System.Data;
using Accord.IO;
using Accord.Math;

namespace Training
{
    class Program
    {
        static void Main(string[] args)
        {
            DataTable table = new ExcelReader("C:/Users/t-yaxie/Desktop/Fun/single_note_result/ttt.xls").GetWorksheet("Sheet1");
            //[index][features] featrues: [p1 p2 p3 p4]
            double[][] inputs = table.ToArray<double>("freq1", "freq2", "freq3");
            //double[][] inputs = table.ToArray<double>("freq2", "freq3");
            //[outputs]
            int[] outputs = table.Columns["note"].ToArray<int>();

            DecisionTree tree = new DecisionTree(
                inputs: new List<DecisionVariable>
                {
                    DecisionVariable.Continuous("freq1"),
                    DecisionVariable.Continuous("freq2"),
                    DecisionVariable.Continuous("freq3")
                    //...
                },
                classes: 8);
            C45Learning teacher = new C45Learning(tree);

            //C45 results should be 0 to k;

            //train
            double error = teacher.Run(inputs, outputs);

            //predict
            int[] answers = inputs.Apply(tree.Compute);
            double[] test = new double[3] { 640, 704, 802 };
            int res = tree.Compute(test);

            Console.ReadLine();
        }
    }
}
