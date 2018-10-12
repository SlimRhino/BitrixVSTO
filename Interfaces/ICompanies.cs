using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitrix.CRM;
using Bitrix.Structures; 

namespace Bitrix.Interfaces
{
    public interface ICompanies
    {
        Company Get(string id);

        void Add(ref Company company, bool registerEvent);

        void Delete(string id);

        List<Company> List(IDictionary<string, string> filters);

        void Update(ref Company company);
    }
}
