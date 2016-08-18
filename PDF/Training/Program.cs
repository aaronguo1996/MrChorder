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
        static DecisionTree tree;

        static void Main(string[] args)
        {
<<<<<<< HEAD
            TreeTraining();

            double[] test = { 132, 260, 392, 784, 916 };
            int res = GetNote(test);

            Console.ReadLine();
        }

        static void TreeTraining()
        {
            DataTable table = new ExcelReader("C:/Users/t-yaxie/Desktop/Fun/single_note_result/result.xls").GetWorksheet("Sheet1");
            DataTable test_table = new ExcelReader("C:/Users/t-yaxie/Desktop/Fun/single_note_result/result_star_50_bigger_20.xls").GetWorksheet("Sheet1");
=======
            DataTable table = new ExcelReader("F:/Microsoft/Mrchorder/data/result.xls").GetWorksheet("Sheet1");
            DataTable test_table = new ExcelReader("F:/Microsoft/Mrchorder/data/test_star.xls").GetWorksheet("Sheet1");
>>>>>>> d52c8c2d2a9a61b8135f6917fcc8eec87f496d44
            //[index][features] featrues: [p1 p2 p3 p4]
            double[][] inputs = table.ToArray<double>("freq1", "freq2", "freq3", "freq4", "freq5");
            double[][] test_inputs = test_table.ToArray<double>("freq1", "freq2", "freq3", "freq4", "freq5");
            //[outputs]
            int[] outputs = table.Columns["note"].ToArray<int>();

            tree = new DecisionTree(
                inputs: new List<DecisionVariable>
                {
                    DecisionVariable.Continuous("freq1"),
                    DecisionVariable.Continuous("freq2"),
                    DecisionVariable.Continuous("freq3"),
                    DecisionVariable.Continuous("freq4"),
                    DecisionVariable.Continuous("freq5")
                    //...
                },
                classes: 9);
            C45Learning teacher = new C45Learning(tree);

            //C45 results should be 0 to k;

            //train
            double error = teacher.Run(inputs, outputs);

            //predict
            int[] answers = inputs.Apply(tree.Compute);
            //double[] test = new double[3] { 640, 704, 802 };
            //int res = tree.Compute(test);
            int[] test_result = test_inputs.Apply(tree.Compute);
        }

        static int GetNote(double[] input)
        {
            return tree.Compute(input);
        }
    }
}
