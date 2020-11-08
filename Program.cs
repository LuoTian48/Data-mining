using System;
using System.Collections.Generic;

namespace k_means聚类算法
{
    class watermelon
    {
        public int ID;//编号
        public double density;//密度
        public double sweetness;//甜度

        public watermelon(int id, double den, double sw)//构造
        {
            ID = id;
            density = den;
            sweetness = sw;
        }
    }
    class kmeans
    {
        int k;//分成几组
        
        public watermelon[] watermelons = new watermelon[30];//30个西瓜初始化
        public static List<List<watermelon>> groups = new List<List<watermelon>>();
        public kmeans(int k)
        {
            watermelons[0] = new watermelon(1, 0.697, 0.460); watermelons[10] = new watermelon(11, 0.245, 0.057); watermelons[20] = new watermelon(21, 0.748, 0.232);
            watermelons[1] = new watermelon(2, 0.774, 0.376); watermelons[11] = new watermelon(12, 0.343, 0.099); watermelons[21] = new watermelon(22, 0.714, 0.346);
            watermelons[2] = new watermelon(3, 0.634, 0.264); watermelons[12] = new watermelon(13, 0.639, 0.161); watermelons[22] = new watermelon(23, 0.483, 0.312);
            watermelons[3] = new watermelon(4, 0.608, 0.308); watermelons[13] = new watermelon(14, 0.657, 0.198); watermelons[23] = new watermelon(24, 0.478, 0.437);
            watermelons[4] = new watermelon(5, 0.556, 0.215); watermelons[14] = new watermelon(15, 0.360, 0.370); watermelons[24] = new watermelon(25, 0.525, 0.369);
            watermelons[5] = new watermelon(6, 0.403, 0.237); watermelons[15] = new watermelon(16, 0.593, 0.042); watermelons[25] = new watermelon(26, 0.751, 0.489);
            watermelons[6] = new watermelon(7, 0.481, 0.149); watermelons[16] = new watermelon(17, 0.719, 0.103); watermelons[26] = new watermelon(27, 0.532, 0.472);
            watermelons[7] = new watermelon(8, 0.437, 0.211); watermelons[17] = new watermelon(18, 0.359, 0.188); watermelons[27] = new watermelon(28, 0.473, 0.376);
            watermelons[8] = new watermelon(9, 0.666, 0.091); watermelons[18] = new watermelon(19, 0.339, 0.241); watermelons[28] = new watermelon(29, 0.725, 0.445);
            watermelons[9] = new watermelon(10, 0.243, 0.267); watermelons[19] = new watermelon(20, 0.282, 0.257); watermelons[29] = new watermelon(30, 0.446, 0.459);

            this.k = k;
            for(int i=0;i<k; i++)
            {
                groups.Add( new List<watermelon>());
            }
        }
        public double distance(double x,double y,double x_mean,double y_mean)//计算欧式距离
        {
            return System.Math.Sqrt((x-x_mean)* (x - x_mean) + (y - y_mean)* (y - y_mean));
        }
        public int Smallest(int k,double[,] center,watermelon melon)
        {
            int num = 0;
            double dis = 1000;
            for(int i = 0; i < k; i++)
            {
                if (distance(melon.density, melon.sweetness, center[i, 0], center[i, 1]) <= dis)
                {
                    num = i;
                    dis = distance(melon.density, melon.sweetness, center[i, 0], center[i, 1]);
                }
            }
            return num;

        }
        public double[] GetMean(List<watermelon> group)
        {
            double sum1 = 0;
            double sum2 = 0;
            for(int i = 0; i < group.Count; i++)
            {
                sum1 += group[i].density;
                sum2 += group[i].sweetness;
            }
            return new double[] { sum1/group.Count, sum2/group.Count };
        }
        public double Cal_E( double[,] center)
        {
            double result=0;
            for(int i = 0; i < groups.Count; i++)
            {
                for(int j = 0; j < groups[i].Count; j++)
                {
                    result += distance(groups[i][j].density, groups[i][j].sweetness, center[i, 0], center[i, 1]);
                }
                
            }
            return result;
        }
        public void ClearList()
        {
            for (int i = 0; i < groups.Count; i++)
            {
                groups[i].Clear();
                
            }
        }
        public void PrintList(double[,] center)
        {
            for (int i = 0; i < groups.Count; i++)
            {
                Console.WriteLine("第{0}组:",i+1);
                Console.Write("[");
                for (int j = 0; j < groups[i].Count; j++)
                {
                    Console.Write(" {0} ",groups[i][j].ID);
                }
                Console.Write("]\n");
            }
            Console.WriteLine("E={0}", Cal_E(center));
            Console.WriteLine("--------------------------------------------------------------");
        }
        public void GetProcess() 
        {
            double[,] center = new double[k, 2];
            double E;
            double E2 = 0;
            int index;
            //把前三个值作为初始聚类中心
            for (int i = 0; i < k; i++)
            {
                center[i, 0] = watermelons[i].density;
                center[i, 1] = watermelons[i].sweetness;
            }
            //第一轮分类
            for(int i = 0; i < 30; i++)
            {
                index = Smallest(k, center, watermelons[i]);
                groups[index].Add(watermelons[i]);
            }
            for (int i = 0; i < k; i++)//更新平均值
            {
                center[i, 0] = GetMean(groups[i])[0];
                center[i, 1] = GetMean(groups[i])[1];
            }
            E = Cal_E(center);
            PrintList(center);
            //第一轮结束----------------------------------------------------------------------------//
            
            while (E2 != E)
            {
                ClearList();
                for (int i = 0; i < 30; i++)//再分类
                {
                    index = Smallest(k, center, watermelons[i]);
                    groups[index].Add(watermelons[i]);
                }
                for (int i = 0; i < k; i++)//更新平均值
                {
                    center[i, 0] = GetMean(groups[i])[0];
                    center[i, 1] = GetMean(groups[i])[1];
                }
                E2 = E;
                E = Cal_E(center);
                PrintList(center);
            }
        }
        
        static void Main(string[] args)
        {
            int k;            
            Console.Write("请输入聚类个数k：");
            k = int.Parse(Console.ReadLine());
            if (k <= 30)
            {                
                kmeans source = new kmeans(k);
                source.GetProcess();
            }
            else
            {
                Console.WriteLine("聚类太多");
            }
            Console.ReadKey();
        }
    }
}
