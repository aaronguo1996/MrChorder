﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lyra.WaveParser;

namespace OnsetDetection
{
    public class OnsetDetector
    {
        private Audio audio;
        /// <summary>
        /// range of frequency in analysis
        /// </summary>
        public int bins { get; set; }
        /// <summary>
        /// number of frames in analysis
        /// </summary>
        public int M { get; set; }

        public double[] filterResult { get; set; }

        public double[] threshold { get; set; }
        public OnsetDetector(string filename)
        {
            audio = new Audio(filename);
            M = audio.data.Length / 256 - 7;
            if(M > 10000)
            {
                M = 10000;
            }
            bins = audio.fftLength / 2;
        }

        public double[][] Normalize(double[][] points)
        {
            double[][] normOutput = new double[M][];
            for (int i = 0; i < M; ++i)
            {
                normOutput[i] = new double[bins];
            }
            double max = 0, min = 0;
            for (int i = 0; i < M; ++i)
            {
                max = points[i][1];
                min = points[i][1];
                for (int j = 1; j < bins; ++j)
                {
                    if (max < points[i][j])
                    {
                        max = points[i][j];
                    }
                    if (min > points[i][j])
                    {
                        min = points[i][j];
                    }
                }
                for (int j = 1; j < bins; ++j)
                {
                    if (max - min == 0)
                    {
                        normOutput[i][j] = 0;
                    }
                    else
                    {
                        normOutput[i][j] = (points[i][j] - min) / (max - min);
                    }
                }
            }
            return normOutput;
        }
        public double[][] STFT()
        {
            double[][] output = new double[M][];
            for(int i = 0; i < M; ++i)
            {
                output[i] = new double[bins];
                output[i] = audio.GetFFTResult(i * 256);
            }
            output = Normalize(output);
            return output;
        }

        public double[] Filter()
        {
            double[][] output = new double[M][];
            double[][] result = new double[M][];
            double[][] differ = new double[M][];

            //data from experiments
            double alpha = 10, beta = 1;
            double T1 = 5, T2 = 70;
            for (int i = 0; i < M; ++i)
            {
                output[i] = new double[bins];
                result[i] = new double[bins];
                differ[i] = new double[bins];
            }
            output = STFT();
            //result = output;
            for(int i = 0; i < M; ++i)
            {
                for(int j = 0; j < bins; ++j)
                {
                    for (int k = 0; k <= i; ++k)
                    {
                        result[i][j] += (alpha * Math.Exp((k - i) / T1) + beta * Math.Exp((k - i) / T2)) * output[k][j];
                    }
                }
            }
            for(int i = 0; i < M; ++i)
            {
                for(int j = 1; j < bins; ++j)
                {
                    if (result[i][j] != 0)
                    {
                        result[i][j] = Math.Log10(result[i][j]);
                    }
                    else
                    {
                        result[i][j] = Math.Log10(0.001);
                    }
                }
            }
            for (int j = 1; j < bins; ++j)
            {
                for (int i = 1; i < M; ++i)
                {
                    differ[i-1][j] = result[i][j] - result[i - 1][j];// * audio.fs / audio.fftLength;
                    //differ[i][j] = Math.Abs(result[i][j] - result[i - 1][j] == 0 ? Math.Log10(Math.Abs(0.0001)) : Math.Log10(Math.Abs(result[i][j] - result[i - 1][j])));
                }
            }

            double[] sum = new double[M];
            for (int i = 1; i < M; ++i)
            {
                sum[i] = 0;
                for (int j = 1; j < bins; ++j)
                {
                    sum[i] += (differ[i][j] > 0 ? differ[i][j] : 0);
                }
            }
            return sum;
        }

        public int[] PeakPick()
        {
            filterResult = Filter();
            double[] smoothRes = new double[M];
            double[] sortedRes = new double[M];
            int[] onsetTime = new int[M];
            double alpha = 40, beta = 1;
            double T1 = 5, T2 = 70;
            for (int i = 0; i < M; ++i)
            {
                for(int j = 0; j < i; ++j)
                {
                    smoothRes[i] += filterResult[i] * (alpha * Math.Exp((j - i) / T1) + beta * Math.Exp((j - i) / T2));
                }
            }

            threshold = new double[M];
            double c = 1.0 / 150;
            int P = 10;
            
            for(int i = 0; i < M; ++i)
            {
                Array.Copy(smoothRes, sortedRes, smoothRes.Length);
                if (i < P)
                {
                    Array.Sort(sortedRes, 0, i + P);
                    threshold[i] = c * sortedRes[(i + P) / 2];
                }
                else if (i + P >= M)
                {
                    Array.Sort(sortedRes, i - P, M - i + P);
                    threshold[i] = c * sortedRes[(M - i + P) / 2 + (i - P)];
                }
                else
                {
                    Array.Sort(sortedRes, i - P, 2 * P + 1);
                    threshold[i] = c * sortedRes[i];
                }
            }
            double maxT = 0, maxF = 0;
            for(int i = 0; i < M; ++i)
            {
                if (threshold[i] > maxT)
                {
                    maxT = threshold[i];
                }
                if(filterResult[i] > maxF)
                {
                    maxF = filterResult[i];
                }
            }
            double max = maxT > maxF ? maxT : maxF;
            for (int i = 0; i < M; ++i)
            {
                threshold[i] /= max;
                filterResult[i] /= max;
            }
            int index = 0;
            for (int i = 1; i < M - 1; ++i)
            {
                if (filterResult[i] - filterResult[i - 1] > 0 && filterResult[i + 1] - filterResult[i] < 0 && filterResult[i] > threshold[i])
                {
                    onsetTime[index++] = i;
                }
                else if (filterResult[i] - filterResult[i - 1] < 0 && filterResult[i + 1] - filterResult[i] > 0 && filterResult[i] > threshold[i])
                {
                    onsetTime[index++] = -i;
                }
            }
            for(int i = 1; i < index-1; ++i)
            {
                if (onsetTime[i - 1] >= 0 && onsetTime[i] < 0 && onsetTime[i + 1] >= 0)
                {
                    onsetTime[i] = 0;
                    onsetTime[i + 1] = 0;
                    ++i;
                }
            }
            
            return onsetTime;
        }

        public float[] GenerateNotes()
        {
            int[] onsetTime = new int[M];
            int[] tmpTime = PeakPick();
            int tmp = 0;
            for (int i = 0; i < M; i++)
            {
                if (tmpTime[i] != 0)
                {
                    onsetTime[tmp++] = tmpTime[i] * 256;
                }
            }
            int[] tmps = audio.GetNotes(onsetTime, tmp);
            float[] notes = new float[tmp - 1];
            for(int i = 0; i < tmps.Length-1; ++i)
            {
                notes[i] = (float)tmps[i];
            }
            return notes;
        }
    }        
}