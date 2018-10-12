using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bitrix.CRM;
using Bitrix.Structures;

namespace Bitrix
{
    //Создаем класс для работы с Битрикс24 из C#
   
    class Program
    {
        static void Main(string[] args)
        {
            Bitrix24 bx = new Bitrix24(username: "orehov@mag.travel", password: "c29_cwnk");
            bx.Initialization();
            //Company company = bx_logon.Companies.Get("1218");
            Dictionary<string, string> filters = new Dictionary<string, string>
            {
                {"ASSIGNED_BY_ID", "968" },
            };
            List<Company> companies = bx.Companies.List(filters);
            Console.ReadLine();
        }
    }
}
