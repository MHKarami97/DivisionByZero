using System;
using System.Collections.Generic;
using System.Globalization;

namespace Services.Services
{
    public class Date:IDate
    {
        public string ConvertDate(DateTime date)
        {
            var persianCalendar = new PersianCalendar();

            return persianCalendar.GetYear(date).ToString("0000/") + persianCalendar.GetMonth(date).ToString("00/") + persianCalendar.GetDayOfMonth(date).ToString("00");
        }

        public string PersianToEnglish(string persianStr)
        {
            var lettersDictionary = new Dictionary<char, char>
            {
                ['۰'] = '0',
                ['۱'] = '1',
                ['۲'] = '2',
                ['۳'] = '3',
                ['۴'] = '4',
                ['۵'] = '5',
                ['۶'] = '6',
                ['۷'] = '7',
                ['۸'] = '8',
                ['۹'] = '9',
                ['/'] = '/',
                [':'] = ':',
                [' '] = ' '
            };

            foreach (var item in persianStr)
            {
                persianStr = persianStr.Replace(item, lettersDictionary[item]);
            }

            return persianStr;
        }
    }
}