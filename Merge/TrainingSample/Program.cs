using System;
using Training;

namespace TrainingSample
{
    class Program
    {
        static void Main(string[] args)
        {
            LearningModel model = new LearningModel();

            double[] test = { 132, 260, 392, 784, 916 };
            int res = model.GetNote(test);
            Console.WriteLine(res);
            Console.ReadLine();
        }
    }
}
