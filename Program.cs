// ---------------------------------------------------------------------------
// File name:Project2.cs
// Project name: Project 2
// ---------------------------------------------------------------------------
// Creator’s name and email: Stan Seiferth zsjs19@goldmail.etsu.edu					
// Course-Section:CSCI-3230-001
//	Creation Date:	10/15/2015		
// ---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Diagnostics;


namespace Project2
{
    /**
    * Class Name: UInt <br>
    * Class Purpose:Manages very large unsigned integers <br>
    *
    * <hr>
    * Date created: 10/15/2015 <br>
    * @author Stan Seiferth
    */
    class UInt
    {
        public const uint max = 1000000000;
        public uint[] x = new uint[1000000];
        public int count = 0;

        public UInt()
        {
            x[0] = 0;
            count = 1;
        }//UInt()

        public UInt(uint i)
        {
            x[0] = i;
            count = 1;
        }//UInt(uint)

        public UInt(uint i, uint p)
        {
            x[0] = i;
            x[1] = p;
            count = 2;
        }//UInt(uint)

        public UInt(uint[] p)
        {
            count = p.Length;
            for (int i = 0; i < p.Length; i++)
                x[i] = p[i];
        }//UInt(uint)

        public UInt(UInt uIn)
        {
            count = uIn.getCount();
            for (int i = 0; i < count + 1; i++)
                x[i] = uIn.getX()[i];
        }//UInt(UInt)

        public uint[] getX()
        {
            return x;
        }//getX

        public void setX(int i, uint n)
        {
            x[i] = n;
        }


        public int getCount()
        {
            return count;
        }//getCount

        public void setCount(int n)
        {
            count = n;
        }//setCount

        public void incCount(int n)
        {
            count += n;
        }//incCount


        /**
          * Method Name: Add <br>
          * Method Purpose: Adds very large UInt's <br>
          *
          * <hr>
          * Date created: 10/15/2015 <br>
          *
          * <hr>
          * Notes on specifications, special algorithms, and assumptions:
          *   Adds numbers together and accounts for overflow
          *
          * <hr>
          *   @param UInt that is being added to sum  
          *   @return None
          */
        public void Add(UInt z)
        {
            long temp;
            uint quot = 0;
            int length = z.getCount();                  //finds number of elements implemented
            for (int i = 0; i < length; i++)        //adds VL and z elements together
            {
                if (z.getX()[i] == 0)
                    continue;
                temp = x[i] + z.getX()[i];
                if (temp >= max)                    //checks for overflow                 
                {
                    quot = (uint)(temp / max);            //removes carry and adds it to next element over
                    x[i + 1] += quot;
                    x[i] = (uint)(temp - (quot * max));
                    if (count <= i)
                        count = i+1;
                }//if
                else
                {
                    x[i] = (uint)temp;                    //no overflow
                    if (count <= i)
                        count = i + 1;
                }
            }//for

            if (x[length] > max)                    //checks that carry doesn't cause next element to overflow
            {
                quot = (x[length] / max);
                x[length + 1] += quot;
                x[length] -= (quot * max);
                if (count <= length)
                    count = length+1;
            }//if
            //}//else
        }//Add(uint)

        /**
         * Method Name: Mult <br>
         * Method Purpose: multiplies very large UInt's <br>
         *
         * <hr>
         * Date created: 10/15/2015 <br>
         *
         * <hr>
         * Notes on specifications, special algorithms, and assumptions:
         *   multiplies numbers together and accounts for overflow
         *
         * <hr>
         *   @param UInt that is to be multiplied by  
         *   @return None
         */
        public void Mult(UInt z)
        {
            UInt[] toAdd = new UInt[count * 200];
            long l;
            long zeros = 0;
            int index = 0;
            for (int i = 0; i < z.getCount(); i++)
            {
                for (int ii = 0; ii < 9; ii++)
                {
                    l = UInt32.Parse(z.getX()[i].ToString("D9").Substring(8 - ii, 1));

                    if (l == 0)
                    {
                        zeros += 1;
                        continue;
                    }
                    else
                    {
                        toAdd[index] = new UInt(Seperate(l, zeros)); //(long)(l * Math.Pow(10, zeros)
                        index += 1;
                    }
                    zeros += 1;
                }
            }
            for (int i = 0; i < count + 1; i++)
                setX(i, 0);
            for (int i = 0; i < index; i++)
            {
                Add(toAdd[i]);
            }

        }//Mult(uint)

        /**
          * Method Name: Mult <br>
          * Method Purpose: multiplies very large UInt's <br>
          *
          * <hr>
          * Date created: 10/15/2015 <br>
          *
          * <hr>
          * Notes on specifications, special algorithms, and assumptions:
          *   multiplies numbers together and accounts for overflow
          *
          * <hr>
          *   @param long that UInt is multiplied by  
          *   @return None
          */
        public void Mult(long l)
        {
            uint[] carries = new uint[count + 2];
            UInt Z;
            long product;
            for (int i = 0; i < count + 1; i++)
            {
                if (x[i] == 0)
                    continue;
                product = checked(l * x[i]);                                                                //mulitiplies numbers together and checks for overflow
                if (product < max)
                    x[i] = (uint)product;
                else
                {
                    x[i] = UInt32.Parse(product.ToString().Substring(product.ToString().Length - 9, 9));    //finds last 9 digits and sets to x[i]
                    product = UInt32.Parse(product.ToString().Substring(0, product.ToString().Length - 9)); //finds remaining digits and sets it to product
                    carries[i + 1] = (uint)product;                                                           //stores carry for adding to next slot
                    if (count < i + 1)                                                                      //checks to see count is correct and sets it accordingly
                        count = i + 1;
                }
            }
            Z = new UInt(carries);                                                                          //moves carries into another UInt
            Add(Z);                                                                                         //adds UInt to main UInt

        }//Mult(long)

        /**
          * Method Name: Mult <br>
          * Method Purpose: multiplies very large UInt's <br>
          *
          * <hr>
          * Date created: 10/15/2015 <br>
          *
          * <hr>
          * Notes on specifications, special algorithms, and assumptions:
          *   multiplies numbers together and accounts for overflow
          *
          * <hr>
          *   @param long that UInt is multiplied by  
          *   @return None
          */
        public UInt Seperate(long n, long zeros)
        {
            long product;
            UInt output = new UInt();
            for (int i = 0; i < count; i++)
            {
                UInt tempUInt = new UInt();
                long tempZeros = zeros;
                int index = i; 
                while (tempZeros > 8)
                {
                    output.setX(i, 0);
                    tempZeros -= 9;
                    index += 1;
                }
                if (x[i] == 0)
                {
                    continue;
                }
                if (tempZeros > 0)
                    product = (long)checked(n * x[i] * Math.Pow(10, tempZeros));
                else
                    product = checked(n * x[i]);
                if (product < max)
                {
                    tempUInt.setX(index, (uint)product);
                    tempUInt.setCount(index + 1);
                }//if
                else
                {
                    tempUInt.setX(index, UInt32.Parse(product.ToString().Substring(product.ToString().Length - 9, 9)));    //finds last 9 digits and sets to x[i]
                    product = UInt32.Parse(product.ToString().Substring(0, product.ToString().Length - 9)); //finds remaining digits and sets it to product
                    if (product > max)
                    {
                        tempUInt.setX(index + 1, UInt32.Parse(product.ToString().Substring(product.ToString().Length - 9, 9)));
                        product = UInt32.Parse(product.ToString().Substring(0, product.ToString().Length - 9));
                        tempUInt.setX(index + 2, (uint)product);
                        if (tempUInt.getCount() < index + 3)                                                                      //checks to see count is correct and sets it accordingly
                            tempUInt.setCount(index + 3);
                    }//if
                    else
                    {
                        tempUInt.setX(index + 1, (uint)product);                                                           //stores carry for adding to next slot
                        if (tempUInt.getCount() < index + 2)                                                                      //checks to see count is correct and sets it accordingly
                            tempUInt.setCount(index + 2);
                    }//else
                 }//else
                
                output.Add(tempUInt);
            }
            return output;
        }//Mult(long)

        /**
         * Method Name: toString <br>
         * Method Purpose: formats and prints UInt to screen <br>
         *
         * <hr>
         * Date created: 10/15/2015 <br>
         *
         * <hr>
         * Notes on specifications, special algorithms, and assumptions:
         *   zeros are padded to left of all non-leading numbers
         *
         * <hr>
         *   @param none
         *   @return None
         */
        public void toString()
        {
            Console.Write(x[count-1].ToString());
            for (int i = count - 2; i > -1; i--)
            {
                Console.Write(x[i].ToString("D9"));
            }

        }

    }//VLInt

    /**
    * Class Name: Program <br>
    * Class Purpose: Main entry point for Project 2 <br>
    *
    * <hr>
    * Date created: 10/15/2015 <br>
    * @author Stan Seiferth
    */

    class Program
    {
        public static UInt[] UIntArr;
        public static UInt VL;
        public static bool firstAdd = true;
        /**
         * Method Name: Factorial <br>
         * Method Purpose: Uses Mult to find factorial of number <br>
         *
         * <hr>
         * Date created: 10/15/2015 <br>
         *
         * <hr>
         * Notes on specifications, special algorithms, and assumptions:
         *   feeds Mult a series of numbers to multiply UInt by in order to find factorial
         *
         * <hr>
         *   @param uint number to find factorial of  
         *   @return None
         */
        public static void Factorial(uint n)
        {
            //for(int i = 2; i < n; i++)
            //{
            //    VL.Mult(i);
            //}
            UIntArr = new UInt[n / 2];
            uint[] uintArr = new uint[n / 2];
            uint ii = ((n / 2) + 1);
            for (uint i = 0; i < n / 2; i++)
            {
                uintArr[i] = ii;
                ii++;
            }//for
            ii = ((n / 2) - 1);
            for (uint i = 0; i < n / 2; i++)
            {
                UIntArr[i] = new UInt(i + 1);
                UIntArr[i].Mult(uintArr[ii]);
                ii--;
            }//for
            int end = (int)(n / 2 - 1);
            if (n % 2 == 0)
            {
                VL = new UInt(UIntArr[end]);
                firstAdd = false;
                DnC((end - 1));
            }
            else
            {
                VL.Add(UIntArr[end - 1]);
                firstAdd = false;
                DnC((end - 2));
            }

        }//factorial(uint)

        public static void DnC(int end)            //divide and conquer
        {
            uint endIndex = (uint)end;
            if (end == 2)
            {
                VL.Mult(UIntArr[2]);
                UIntArr[0].Mult(UIntArr[1]);
                VL.Mult(UIntArr[0]);
                return;
            }
            if (end < 2)
            {
                UIntArr[0].Mult(UIntArr[1]);
                VL.Mult(UIntArr[0]);
                return;
            }
            for (int i = 0; i < endIndex; i++)
            {
                UIntArr[i].Mult(UIntArr[endIndex]);
                endIndex--;
            }
            if (end % 2 == 1)
            {
                end = (end / 2);
                //if (!firstAdd)
                //{
                //    VL.Mult(UIntArr[end + 1]);
                //    firstAdd = false;
                //}
                //else
                //    VL.Add(UIntArr[end+1]);
            }
            else
            {
                end = (end / 2 - 1);
                if (!firstAdd)
                {
                    if (end >= 1)
                        VL.Mult(UIntArr[end + 1]);
                }
                else
                {
                    if (end >= 1)
                    {
                        VL.Add(UIntArr[end + 1]);
                        firstAdd = false;
                    }
                }
            }
            DnC(end);
        }//dnc(uint)

        /**
            * Method Name: main <br>
            * Method Purpose:driver for project 2 <br>
            *
            * <hr>
            * Date created: 10/15/2015 <br>
            *
            * <hr>
            * Notes on specifications, special algorithms, and assumptions:
            *   is the main entry point for and accepts user input
            *
            * <hr>
            *   @param string[] args
            *   @return void
            */
        static void Main(string[] args)
        {
            int input = 0;
            string strInput;
            bool loop = true;

            while (loop)
            {
                Console.WriteLine("Please enter any whole number: ");
                strInput = Console.ReadLine();
                if (Int32.TryParse(strInput, out input))
                    loop = false;
            }
            UInt Z = new UInt((uint)input);
            //VL = new UInt(999999999);
            //VL.Mult(10);
            Stopwatch st = new Stopwatch();
            st.Start();
           // VL.Mult(Z);
            Factorial((uint)input);
            st.Stop();
            Console.WriteLine("Time used: {0} secs", st.Elapsed.TotalMilliseconds / 1000);
            VL.toString();
            Console.ReadLine();
        }//main
    }//program
}
