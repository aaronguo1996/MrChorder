using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;
using Accord.Controls;
using Accord.IO;
using Accord.MachineLearning.Bayes;
using Accord.Statistics.Distributions.Univariate;
using System.Data;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;
using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;

namespace AforgeTest
{
    class Program
    {
        static void Main(string[] args)
        {
            DataTable table = new ExcelReader("C:/Users/t-yaxie/Desktop/Fun/MrChorder/AforgeTest/examples2.xls").GetWorksheet("Classification - Yin Yang");
            double[][] inputs = table.ToArray<double>("X", "Y");
            int[] outputs = table.Columns["G"].ToArray<int>();
            

            // In our problem, we have 2 classes (samples can be either
            // positive or negative), and 2 continuous-valued inputs.
            DecisionTree tree = new DecisionTree(
                            inputs: new List<DecisionVariable>
                                {
                        DecisionVariable.Continuous("X"),
                        DecisionVariable.Continuous("Y")
                                },
                            classes: 4);

            C45Learning teacher = new C45Learning(tree);

            // The C4.5 algorithm expects the class labels to
            // range from 0 to k, so we convert -1 to be zero:
            //
            outputs = outputs.Apply(x => x < 0 ? 0 : x);

            double error = teacher.Run(inputs, outputs);

            // Classify the samples using the model
            int[] answers = inputs.Apply(tree.Compute);

            int y = tree.Compute(inputs[50]);

            // Plot the results
            //ScatterplotBox.Show("Expected", inputs, outputs);
            //ScatterplotBox.Show("Results", inputs, answers);
            Console.ReadLine();
        }
    }
}
