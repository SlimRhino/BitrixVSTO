using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitrix.CRM;

using Bitrix.Structures;

namespace Bitrix.CRM
{
    
    public partial class Companies  : Interfaces.ICompanies
    {
        #region Свойства
        private readonly Bitrix24 _bitrix;
        #endregion

        public Companies(Bitrix24 bitrix)
        {
            _bitrix = bitrix;
        }

        #region Методы

        public Company Get(string id)
        {
            string json = _bitrix.SendCommand("crm.company.get", "ID=" + id).Remove(0, 10);
            // вырезается часть ответа с данными по времени запроса, так как не придумал зачем они?
            int removeIndex = json.IndexOf(@",""time");
            json = json.Remove(removeIndex, json.Length - removeIndex);
            return JsonConvert.DeserializeObject<Company>(json);
        }

        public void Delete( string id)
        {
            string json = _bitrix.SendCommand("crm.company.delete", "ID=" + id);
        }
        
        public List<Company> List(IDictionary<string,string> filters)
        {
            string FILTER = string.Empty;
            foreach (KeyValuePair<string,string> filter in filters)
            {
                FILTER += "&FILTER["+filter.Key+"]="+filter.Value;
            }
            string json = _bitrix.SendCommand("crm.company.list", "ORDER[]=" + FILTER + "&SELECT[]=*");
            ResultList companyList = JsonConvert.DeserializeObject<ResultList>(json);
            if (companyList.Total <= 50) return companyList.Result;
            List<Company> companies = companyList.Result;
            while (companyList.Next != 0)
            {
                json = _bitrix.SendCommand("crm.company.list", "ORDER[]=" + FILTER + "&SELECT[]=*&start=" + companyList.Next);
                companyList = JsonConvert.DeserializeObject<ResultList>(json);
                companies.AddRange(companyList.Result);
            }
            return companies;
        }

        public void Add(ref Company company, bool registerEvent = false)
        {

        }

        public void Update(ref Company company)
        {

        }

        #endregion

        

    }
}
