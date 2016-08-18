using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;
using OnsetDetection;

namespace DisplayWindow
{
    public class DisplayWindow
    {
        static double[][] points;
        static double[][] normOutput;
        static OnsetDetector od;
        static void Main()
        {
            od = new OnsetDetector("t5.wav");
            points = new double[od.M][];
            normOutput = new double[od.M][];
            for (int i = 0; i < od.M; ++i)
            {
                points[i] = new double[od.bins];
                normOutput[i] = new double[od.bins];
            }
            normOutput[0] = od.peakPick();
            points[0] = od.filterResult;
            //normalize();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var form = new Form();
            form.SetBounds(100, 100, 1200, 800);
            form.Paint += new PaintEventHandler(paintHandler);
            Application.Run(form);
        }

        public static void paintHandler(Object sender, PaintEventArgs e)
        {

            Pen blackPen = new Pen(Color.Black, 1);
            Pen redPen = new Pen(Color.Red, 1);
            Pen greenPen = new Pen(Color.Green, 1);

            if (points != null)
            {
                int M = points.Length;
                int bin = points[0].Length;
                Point prev = new Point(20,750);
                Point curr = new Point(20,750);
                for (int i = 0; i < M; ++i)
                {
                    /*for (int j = 1; j < bin; ++j)
                    {
                        SolidBrush b = new SolidBrush(Color.Black);
                        if (points[i][j] > 0.75)
                        {
                            b.Color = Color.FromArgb((int)(255 - points[i][j] * 255), (int)(255 - points[i][j] * 255), (int)(255 - points[i][j] * 255));
                        }
                        else
                        {
                            b.Color = Color.FromArgb(255, 255, 255);
                        }
                        e.Graphics.FillEllipse(b, new Rectangle(i * 3, 750 - j, 3, 3));
                    }*/
                    curr = new Point(20 + i*3, 790 - (int)(points[0][i] * 750));
                    e.Graphics.DrawLine(blackPen, prev, curr);
                    prev = curr;
                }
                
                for(int i = 0; i < M; ++i)
                {
                    if (normOutput[0][i] != 0)
                    {
                        e.Graphics.DrawLine(redPen, new Point(20 + (int)normOutput[0][i] * 3, 790), new Point(20 + (int)normOutput[0][i] * 3, 0));
                    }
                }

                //draw threshold
                prev = new Point(20, 750);
                for (int i = 0; i < M; ++i)
                {
                    curr = new Point(20 + i * 3, 790 - (int)(od.threshold[i] * 750));
                    e.Graphics.DrawLine(greenPen, prev, curr);
                    prev = curr;
                }
            }
        }
    }
}