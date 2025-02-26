using System;
using System.Collections.Generic;
 
using System.Text;
 

namespace CustomFunctions
{
    public class CustomFunctions
    {
        public static  int GetValueBasedOnRange(int value)
        {
           

            if (value >= 0 && value <= 350)
            {
                return 0;
            }
            else if (value >= 351 && value <= 700)
            {
                return value-350;
            }
            else if (value >= 701 && value <= 1050)
            {
                return value - 700;
            }
            else if (value >= 1051 && value <= 1400)
            {
                return value - 1050;
            }
            else if (value >= 1401 && value <= 1750)
            {
                return value - 1400;
            }
            else if (value >= 1751 && value <= 2100)
            {
                return value - 1750;
            }
            else if (value >= 2101 && value <= 2450)
            {
                return value - 2100;  // Тут я 
            }
            else if (value >= 2451 && value <= 2750)
            {
                return value - 2450;
            }
            else if (value >= 2751 && value <= 3100)
            {
                return value - 2750; // Тут я 
            }
            else if (value >= 3101 && value <= 3450)
            {
                return value - 3100;
            }
            else if (value >= 3450+1 && value <= 3800)
            {
                return value - 3450;
            }
            else if (value >= 3800 + 1 && value <= 4300)
            {
                return value - 3800;
            }
            else if (value >= 4300 + 1 && value <= 4800)
            {
                return value - 4300;
            }
            else if (value >= 4800 + 1 && value <= 5300)
            {
                return value - 4800;
            }
            else if (value >= 5300 + 1 && value <= 5900)
            {
                return value - 5300;
            }
            else if (value >= 5900 + 1 && value <= 6500)
            {
                return value - 5900;
            }
            else if (value >= 6500 + 1 && value <= 7100)
            {
                return value - 6500;
            }
            else if (value >= 7100 + 1 && value <= 7700)
            {
                return value - 7100;
            }
            else if (value >= 7700 + 1 && value <= 8300)
            {
                return value - 7700;
            }
            else if (value >= 8300 + 1 && value <= 9000)
            {
                return value - 8300;
            }
            else if (value >= 9000 + 1 && value <= 9700)
            {
                return value - 9000;
            }
            else if (value >= 9700 + 1 && value <= 10400)
            {
                return value - 9700;
            }
            else if (value >= 10400 + 1 && value <= 11100)
            {
                return value - 10400;
            }
            else if (value >= 11100 + 1 && value <= 11800)
            {
                return value - 11100;
            }
            else if (value >= 11800 + 1 && value <= 12500)
            {
                return value - 11800;
            }
            else if (value >= 12500 + 1 && value <= 15000)
            {
                return value - 12500;
            } 
            // Добавьте другие условия по необходимости
            return 0;
        }

        public static int GetValue214RPT(int value)
        {


            if (value >= 0 && value <= 300)
            {
                return 0;
            }
            else if (value >= 300 + 1 && value <= 600)
            {
                return value - 300;
            }
            else if (value >= 600 + 1 && value <= 1200)
            {
                return value - 600;
            }
            else if (value >= 1200 + 1 && value <= 1600)
            {
                return value - 1200;
            }
            else if (value >= 1600 + 1 && value <= 2000)
            {
                return value - 1600;
            }
            else if (value >= 2000 + 1 && value <= 2300)
            {
                return value - 2000;
            }
            else if (value >= 2300 + 1 && value <= 2600)
            {
                return value - 2300;
            }
            else if (value >= 2600 + 1 && value <= 2900)
            {
                return value - 2600;
            }
            else if (value >= 2900 + 1 && value <= 3200)
            {
                return value - 2900;
            }
            else if (value >= 3200 + 1 && value <= 3500)
            {
                return value - 3200;
            }
            else if (value >= 3500 + 1 && value <= 3900)
            {
                return value - 3500;
            }
            else if (value >= 3900 + 1 && value <= 4300)
            {
                return value - 3900;
            }
            else if (value >= 4300 + 1 && value <= 4800)
            {
                return value - 4300;
            }
            else if (value >= 4800 + 1 && value <= 5300)
            {
                return value - 4800;
            }
            else if (value >= 5300 + 1 && value <= 5900)
            {
                return value - 5300;
            }
            else if (value >= 5900 + 1 && value <= 6500)
            {
                return value - 5900;
            }
            else if (value >= 6500 + 1 && value <= 7100)
            {
                return value - 6500;
            }
            else if (value >= 7100 + 1 && value <= 7700)
            {
                return value - 7100;
            }
            else if (value >= 7700 + 1 && value <= 8300)
            {
                return value - 7700;
            }
            else if (value >= 8300 + 1 && value <= 8900)
            {
                return value - 8300;
            }
            else if (value >= 8900 + 1 && value <= 9500)
            {
                return value - 8900;
            }
            else if (value >= 9500 + 1 && value <= 10100)
            {
                return value - 9500;
            }
            else if (value >= 10100 + 1 && value <= 10700)
            {
                return value - 10100;
            }
            else if (value >= 10700 + 1 && value <= 11300)
            {
                return value - 10700;
            }
            else if (value >= 11300 + 1 && value <= 15000)
            {
                return value - 11300;
            }

            // Добавьте другие условия по необходимости
            return 0;
        }
    }
}
