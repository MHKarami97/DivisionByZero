using System;

namespace Services.Services
{
    public interface IDate
    {
        string ConvertDate(DateTime date);

        string PersianToEnglish(string persianStr);
    }
}