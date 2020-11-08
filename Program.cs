using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Transactions;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Apriori控制台程序
{
    //public enum itemType { milk, beer, diaper, bread, butter, cookie }
    
    class Apriori
    {
        private static int support; // 支持度(次数)
        private static double confidence; // 置信度阈值

        public Dictionary<int[], int> collections = new Dictionary<int[], int>();
        
        public static int[][] transList = new int[10][];//10条事务
        public Apriori(int min_sup, double min_conf) //构造函数
        {
            support = min_sup;
            confidence = min_conf;
            //牛奶=1，啤酒=2，尿布=3，面包=4，黄油=5，饼干=6
            transList[0] = new int[3] { 1, 2, 3 };
            transList[1] = new int[3] { 1, 4, 5 };
            transList[2] = new int[3] { 1, 3, 6 };
            transList[3] = new int[3] { 4, 5, 6 };
            transList[4] = new int[3] { 2, 6, 3 };
            transList[5] = new int[4] { 1, 3, 5, 4 };
            transList[6] = new int[3] { 4, 3, 5 };
            transList[7] = new int[2] { 3, 2 };
            transList[8] = new int[4] { 1, 3, 5, 4 };
            transList[9] = new int[2] { 2, 6 };

        }
        private Dictionary<int,int> get_L1()//获得一项强项集L1
        {
            
            int[] C1 = new int[6] { 1, 2, 3, 4, 5, 6 };
            //一项强项集
            int count;
            Dictionary<int,int> L1 = new Dictionary<int, int>();
            for (int i = 0; i < 6; i++) 
            {
                count = 0;
                for(int j = 0; j < 10; j++)
                { 
                    for(int k = 0; k < transList[j].Length; k++)
                    {
                        if (transList[j][k] == C1[i])
                        {
                            count++;
                            
                        }
                    }
                }
                if (count >= support)
                {
                    L1.Add(C1[i],count);
                    collections.Add(new int[] { C1[i] }, count);
                }
            }

            return L1;
        }
        private List<int[]> get_C2(Dictionary<int,int> L1)//获得二项候选集C2
        {
            List<int[]> C2_1 = new List<int[]>();
            List<int> L1_set = L1.Keys.ToList<int>();
            for(int i = 0; i < L1_set.Count; i++)
            {
                for(int j = i + 1; j < L1_set.Count; j++)
                {
                    C2_1.Add(new int[2] { i, j });
                }
            }
            return C2_1;
        }
        public bool checkHasNum(int passIn,int[] array)//检查一个数组中有无特定数字
        {
            bool hasNum = false;
            for(int i=0;i<array.Length;i++)
                if (passIn == array[i])
                {
                    hasNum = true;
                }
            return hasNum;
        }

        private Dictionary<int[],int> get_L2(List<int[]> C2)//获得二项强项集L2
        {
            Dictionary<int[],int> L2 = new Dictionary<int[], int>();
            
            int count;
            for(int i = 0; i < C2.Count; i++)
            {
                
                count = 0;
                for(int j = 0; j < 10; j++)
                {
                    if (checkHasNum(C2[i][0], transList[j]) && checkHasNum(C2[i][1], transList[j]))
                    {
                        count++;
                    }
                }
                if (count >= support)
                {
                    L2.Add(C2[i],count);
                    collections.Add(C2[i], count);
                }
            }
            return L2;
        }
                
        private List<int[]> get_C3(Dictionary<int[],int> L2)//由L2求C3
        {
            List<int[]> C3_1 = new List<int[]>();
            List<int[]> L2_set = L2.Keys.ToList();
            for (int i = 0; i < L2_set.Count; i++)
            {
                for (int j = i + 1; j < L2_set.Count; j++)
                {
                    if (L2_set[i].Intersect(L2_set[j]).ToArray().Length == 1)//找出有一个共同项的两组L2强项集
                    {
                        C3_1.Add(L2_set[i].Union(L2_set[j]).ToArray());//拼接,剔除重复项

                    }
                    
                }
            }
            for (int i = 0; i < C3_1.Count; i++)//去掉C3中的重复项
            {
                for (int j = C3_1.Count - 1; j > i; j--)  
                {

                    if (C3_1[i][0] == C3_1[j][0]&& C3_1[i][1] == C3_1[j][1]&& C3_1[i][2] == C3_1[j][2]||
                        C3_1[i][0] == C3_1[j][0] && C3_1[i][1] == C3_1[j][2] && C3_1[i][2] == C3_1[j][1]||
                        C3_1[i][0] == C3_1[j][1] && C3_1[i][1] == C3_1[j][0] && C3_1[i][2] == C3_1[j][2]||
                        C3_1[i][0] == C3_1[j][2] && C3_1[i][1] == C3_1[j][0] && C3_1[i][2] == C3_1[j][1]||
                        C3_1[i][0] == C3_1[j][2] && C3_1[i][1] == C3_1[j][1] && C3_1[i][2] == C3_1[j][0]||
                        C3_1[i][0] == C3_1[j][1] && C3_1[i][1] == C3_1[j][2] && C3_1[i][2] == C3_1[j][0])
                    {
                        C3_1.RemoveAt(j);
                    }
                    //if (C3_1[i][0] == C3_1[j][0] && C3_1[i][1] == C3_1[j][2] && C3_1[i][2] == C3_1[j][1])
                    //{
                    //    C3_1.RemoveAt(j);
                    //}
                    //if (C3_1[i][0] == C3_1[j][1] && C3_1[i][1] == C3_1[j][0] && C3_1[i][2] == C3_1[j][2])
                    //{
                    //    C3_1.RemoveAt(j);
                    //}
                    //if (C3_1[i][0] == C3_1[j][2] && C3_1[i][1] == C3_1[j][0] && C3_1[i][2] == C3_1[j][1])
                    //{
                    //    C3_1.RemoveAt(j);
                    //}
                    //if (C3_1[i][0] == C3_1[j][2] && C3_1[i][1] == C3_1[j][1] && C3_1[i][2] == C3_1[j][0])
                    //{
                    //    C3_1.RemoveAt(j);
                    //}
                    //if (C3_1[i][0] == C3_1[j][1] && C3_1[i][1] == C3_1[j][2] && C3_1[i][2] == C3_1[j][0])
                    //{
                    //    C3_1.RemoveAt(j);
                    //}

                }
            }
            
            return C3_1;
        }

        private Dictionary<int[],int> get_L3(List<int[]> C3)//获得三项强项集L3
        {
            Dictionary<int[], int> L3 = new Dictionary<int[], int>();
            int count;
            for (int i = 0; i < C3.Count; i++)
            {

                count = 0;
                for (int j = 0; j < 10; j++)
                {
                    if (checkHasNum(C3[i][0], transList[j]) && checkHasNum(C3[i][1], transList[j])&&checkHasNum(C3[i][2],transList[j]))
                    {
                        count++;
                    }
                }
                if (count >= support)
                {
                    L3.Add(C3[i], count);
                    collections.Add(C3[i], count);
                }
            }
            return L3;
        }
        //TODO:求置信概率
        private void Get_confidence(Dictionary<int[],int> L2,Dictionary<int,int> L1)//两项关联规则
        {
            int a ;
            foreach(KeyValuePair<int[],int> item in L2)
            {
                
                if (L1.TryGetValue(item.Key[0], out a))
                {
                    if ((double)(item.Value / (double)a) >= confidence)
                    {
                        Console.WriteLine(transferToString( item.Key[0]) + "->" + transferToString(item.Key[1]) + " :" + (double)(item.Value) / (double)a);
                    }

                }
                if (L1.TryGetValue(item.Key[1], out a))
                {
                    if ((double)(item.Value / (double)a) >= confidence)
                    {
                        Console.WriteLine(transferToString(item.Key[1]) + "->" + transferToString(item.Key[0]) + " :" + (double)(item.Value) / (double)a);
                    }

                }



            }
        }

        private void Get_confidence(Dictionary<int[],int> L3,Dictionary<int[], int> L2, Dictionary<int, int> L1)//三项关联规则
        {
            int a;
            //1推2
            foreach (KeyValuePair<int[], int> item in L3)
            {
                if (L1.TryGetValue(item.Key[0], out a))
                {
                    if ((double)(item.Value / (double)a) >= confidence)
                    {
                        Console.WriteLine(transferToString(item.Key[0]) + "->" + transferToString(item.Key[1])+" "+ transferToString(item.Key[2]) + " :" + (double)(item.Value) / (double)a);
                    }

                }
                if (L1.TryGetValue(item.Key[1], out a))
                {
                    if ((double)(item.Value / (double)a) >= confidence)
                    {
                        Console.WriteLine(transferToString(item.Key[1]) + "->" + transferToString(item.Key[0]) + " "+ transferToString(item.Key[2]) +" :"+ (double)(item.Value) / (double)a);
                    }

                }
                if (L1.TryGetValue(item.Key[2], out a))
                {
                    if ((double)(item.Value / (double)a) >= confidence)
                    {
                        Console.WriteLine(transferToString(item.Key[2]) + "->" + transferToString(item.Key[0]) + " "+ transferToString(item.Key[1]) +" :"+ (double)(item.Value) / (double)a);
                    }

                }
            }
            //2推1
            foreach (KeyValuePair<int[], int> item in L3)
            {
                if (L2.TryGetValue(new int[] { item.Key[0], item.Key[1] }, out a))
                {
                    if ((double)(item.Value / (double)a) >= confidence)
                    {
                        Console.WriteLine(transferToString(item.Key[0]) +" "+ transferToString(item.Key[1])+"->" + transferToString(item.Key[2]) + " :" + (double)(item.Value) / (double)a);
                    }
                }
                if (L2.TryGetValue(new int[] { item.Key[1], item.Key[2] }, out a))
                {
                    if ((double)(item.Value / (double)a) >= confidence)
                    {
                        Console.WriteLine(transferToString(item.Key[1]) + " " + transferToString(item.Key[2]) + "->" + transferToString(item.Key[0]) + " :" + (double)(item.Value) / (double)a);
                    }
                }
                if (L2.TryGetValue(new int[] { item.Key[0], item.Key[2] }, out a))
                {
                    if ((double)(item.Value / (double)a) >= confidence)
                    {
                        Console.WriteLine(transferToString(item.Key[0]) + " " + transferToString(item.Key[2]) + "->" + transferToString(item.Key[2]) + " :" + (double)(item.Value) / (double)a);
                    }
                }
            }
        }
        static void Main(string[] args)
        {
            int min_sup;//最小支持数
            double min_conf;//置信度
            Console.Write("请输入支持度最小值:（整数）");
            min_sup = int.Parse(Console.ReadLine());
            Console.Write("请输入置信度最小值:（小数）");
            min_conf = double.Parse(Console.ReadLine());
            Apriori apriori = new Apriori(min_sup, min_conf);
            Dictionary<int, int> L1_s = apriori.get_L1();//求L1
            Console.WriteLine("----------------一项强项集----------------");
            foreach (KeyValuePair<int, int> T in L1_s)
            {
                Console.WriteLine(apriori.transferToString(T.Key) + "  :" + T.Value);
            }//输出
            List<int[]> C2_s = apriori.get_C2(L1_s);//求C2
            Dictionary<int[], int> L2_s = apriori.get_L2(C2_s);//求L2
            Console.WriteLine("----------------二项强项集----------------");
            foreach (KeyValuePair<int[], int> T in L2_s)
            {
                Console.WriteLine(apriori.transferToString(T.Key[0]) + " 和 " + apriori.transferToString(T.Key[1]) + "  :" + T.Value);
            }//输出
            List<int[]> C3_s = apriori.get_C3(L2_s);//求C3
            Dictionary<int[], int> L3_s = apriori.get_L3(C3_s);//求L3
            Console.WriteLine("----------------三项强项集----------------");
            foreach (KeyValuePair<int[], int> T in L3_s)
            {
                Console.WriteLine(apriori.transferToString(T.Key[0]) + "、" + apriori.transferToString(T.Key[1]) + " 和 " + apriori.transferToString(T.Key[2]) + "  :" + T.Value);
            }//输出
            Console.WriteLine("----------------关联规则----------------");
            apriori.Get_confidence(L2_s, L1_s);//求关联规则
            apriori.Get_confidence(L3_s, L2_s, L1_s);
            Console.ReadKey();
        }

        public string transferToString(int a)
        {
            switch (a)
            {
                case 1:
                    return /*"milk"*/"牛奶";
                case 2:
                    return /*"beer"*/"啤酒";
                case 3:
                    return /*"diaper"*/"尿布";
                case 4:
                    return /*"bread"*/"面包";
                case 5:
                    return /*"butter"*/"黄油";
                case 6:
                    return /*"cookie"*/"饼干";
                    

            }
            return "null";
                
            
        }//转换成单词
        


        
    }


}
