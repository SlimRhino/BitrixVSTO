using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Bitrix.Structures
{
    // TODO значения по умолчанию
    
    public struct Company
    {
        [JsonProperty("ID")]
        public string ID { get; set; }

        [JsonProperty("TITLE")]
        public string Name { get; set; }

        [JsonProperty("COMPANY_TYPE")]
        public string CompanyType { get; set; }

        [JsonProperty("LOGO")]
        public string Logo { get; set; }

        [JsonProperty("ADDRESS")]
        public string Address { get; set; }
        /// <summary>
        /// Банковские реквизиты
        /// </summary>
        [JsonProperty("BANKING_DETAILS")]
        public string Banking { get; set; }
        /// <summary>
        /// Сфера деятельности
        /// </summary>
        [JsonProperty("INDUSTRY")]
        public string Industry { get; set; }
        /// <summary>
        /// Количество сотрудников
        /// </summary>
        [JsonProperty("EMPLOYEES")]
        public string Employees { get; set; }
        /// <summary>
        /// Валюта расчетов
        /// </summary>
        [JsonProperty("CURRENCY_ID")]
        public string Currency { get; set; }
        /// <summary>
        /// Годовой оборот
        /// </summary>
        [JsonProperty("REVENUE")]
        public string Revenue { get; set; }
        /// <summary>
        /// Флаг "Доступна для всех"
        /// </summary>
        [JsonProperty("OPENED")]
        public char Opened { get; set; }
        /// <summary>
        /// Комментарии
        /// </summary>
        [JsonProperty("COMMENTS")]
        public string Comments { get; set; }
        /// <summary>
        /// Идентификатор создавшего пользователя 
        /// </summary>
        [JsonProperty("CREATED_BY_ID")]
        public string CreatedBy { get; set; }
        /// <summary>
        /// Идентификатор последнего изменившего пользователя
        /// </summary>
        [JsonProperty("MODIFY_BY_ID")]
        public string ModifyBy { get; set; }

        [JsonProperty("EMAIL")]
        public List<Contacts> Emails { get; set; }

        [JsonProperty("PHONE")]
        public List<Contacts> Phones { get; set; }
        /// <summary>
        /// Идентификатор ответственного
        /// </summary>
        [JsonProperty("ASSIGNED_BY_ID")]
        public string AssignedBy { get; set; }
        /// <summary>
        /// Дата создания
        /// </summary>
        [JsonProperty("DATE_CREATE")]
        public string DateCreate { get; set; }
        /// <summary>
        /// Дата изменения
        /// </summary>
        [JsonProperty("DATE_MODIFY")]
        public string DateModify { get; set; }
        /// <summary>
        /// Идентификатор лида
        /// </summary>
        [JsonProperty("LEAD_ID")]
        public string LeadID { get; set; }
        /// <summary>
        /// Спискок мессенджеров
        /// </summary>
        [JsonProperty("IM")]
        public List<Contacts> Messengers { get; set; }
        /// <summary>
        /// Список сайтов
        /// </summary>
        [JsonProperty("WEB")]
        public List<Contacts> Sites { get; set; }
        /// <summary>
        /// Флаг незаполнености 
        /// </summary>
        public bool isNull;
    }

    public struct Contacts
    {
        /// <summary>
        /// Значение контакта
        /// </summary>
        [JsonProperty("VALUE")]
        public string Value { get; set; }
        /// <summary>
        /// Тип контакта
        /// </summary>
        [JsonProperty("VALUE_TYPE")]
        public string ValueType { get; set; }
    }
    
    // TODO адреса неработают
    public struct Address
    {
        /// <summary>
        /// Улица, дом, корпус, строение (фактический адрес)
        /// </summary>
        [JsonProperty("ADDRESS")]
        public string address1;
        /// <summary>
        /// Квартира / офис (фактический адрес)
        /// </summary>
        [JsonProperty("ADDRESS_2")]
        public string address2;
        /// <summary>
        /// Полный адрес   
        /// </summary>
        public string FullAddress
        {
            get { return address1 + " " + address2; }
        }

        [JsonProperty("ADDRESS_CITY")]
        public string City { get; set; }

        [JsonProperty("ADDRESS_REGION")]
        public string Region { get; set; }

        [JsonProperty("ADDRESS_PROVINCE")]
        public string Province { get; set; }

        [JsonProperty("ADDRESS_COUNTRY")]
        public string Country { get; set; }

        [JsonProperty("ADDRESS_COUNTRY_CODE")]
        public string CountryCode { get; set; }
        /// <summary>
        /// Почтовый индекс (фактический адрес)
        /// </summary>
        [JsonProperty("ADDRESS_POSTAL_CODE")]
        public string ZipCode { get; set; }

        [JsonProperty("REG_ADDRESS")]
        public string legalAddress1;
        /// <summary>
        /// Квартира / офис (фактический адрес)
        /// </summary>
        [JsonProperty("REG_ADDRESS_2")]
        public string legalAddress2;
        /// <summary>
        /// Полный юридический адрес   
        /// </summary>
        [JsonProperty("ADDRESS_LEGAL")]
        public string LegalFullAddress { get; set; }
        
        /// <summary>
        /// Почтовый индекс (юридический адрес)
        /// </summary>
        [JsonProperty("REG_ADDRESS_POSTAL_CODE")]
        public string ZipCodeLegal { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("REG_ADDRESS_CITY")]
        public string CityLegal { get; set; }

        [JsonProperty("REG_ADDRESS_REGION")]
        public string RegionLegal { get; set; }

        [JsonProperty("REG_ADDRESS_PROVINCE")]
        public string ProvinceLegal { get; set; }

        [JsonProperty("REG_ADDRESS_COUNTRY")]
        public string CountryLegal { get; set; }

        [JsonProperty("REG_ADDRESS_COUNTRY_CODE")]
        public string CountryCodeLegal { get; set; }
    }

    public struct Logo
    {
        /// <summary>
        /// Ссылка на скачивание картинки логотипа
        /// </summary>
        [JsonProperty("downloadUrl")]
        public string Download { get; set; }
        /// <summary>
        /// Ссылка на открытие в браузере
        /// </summary>
        [JsonProperty("showUrl")]
        public string Open { get; set; }
    }

    public struct ResultList
    {
        /// <summary>
        /// Результат полученный из страницы запроса. Макс. 50 штук
        /// </summary>
        [JsonProperty("result")]
        public List<Company> Result { get; set; }
        /// <summary>
        /// Колличество найденых по запросу объектов
        /// </summary>
        [JsonProperty("total")]
        public int Total { get; set; }
        /// <summary>
        /// Индекс для запроса следущей страницы результата
        /// </summary>
        [JsonProperty("next")]
        public int Next { get; set; }
    }
}
