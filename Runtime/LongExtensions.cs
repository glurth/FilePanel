using UnityEngine;

namespace EyE.Unity.UI
{
    static public class LongExtensions
    {
        public static string FormatLargeNumberSI(this long num)
        {
            string[] modifiers = { " ", " Kilo", " Mega", " Giga", " Tera", " Peta", " Exa", " Zetta", " Yotta" };
            // Determine the appropriate modifier
            int i = 0;
            long workingNum = num;
            while (workingNum >= 1000 && i < modifiers.Length - 1)
            {
                workingNum /= 1000;
                i++;
            }
            // Format the number with modifier
            if (workingNum >= 100)
                return workingNum.ToString("F0") + modifiers[i];
            else
            {
                float floatNum = num / Mathf.Pow(1000, i);
                if(workingNum>=10)
                    return floatNum.ToString("F1") + modifiers[i];
                else
                    return floatNum.ToString("F2") + modifiers[i];
                //if (workingNum >= 10)
            }  

        }

    }
}