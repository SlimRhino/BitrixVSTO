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

namespace Bitrix
{

    public class Bitrix24
    {
        //Объявляем константы
        private const string BX_ClientID = "local"; //Ваш Код приложения (client_id)
        private const string BX_ClientSecret = "N0IZEbcYYU6"; //Ваш Ключ приложения (client_secret)
        private const string BX_Portal = "https://magtravel.bitrix24.ru"; //Адрес вашего портала\сайта в Битрикс24
        private const string BX_OAuthSite = "https://oauth.bitrix.info"; //Этот адрес не изменяйте

        //Объявляем приватные служебные поля
        private string AccessToken;
        private string RefreshToken;
        private DateTime RefreshTime;
        private string Code;
        private string Cookie;
        private string _username;
        private string _password;



        //Создаем конструктор с вызовом подключения к Битрикс24 при создании экземпляра данного класса  
        public Bitrix24(string username, string password)
        {
            _username = username;
            _password = password;
        }

        //Создаем закрытый метод для начального подключения к Битрикс24
        //Укажите Ваши логин и пароль пользователя (админа) вашего портала в Битрикс24, под которым будут выполнять REST запросы к Битрикс24.
        public bool Connect()
        {
            //Создаем HTTP подключение, здесь ничего не меняем
            string BX_URI = BX_Portal + "/oauth/authorize/?client_id=" + BX_ClientID;
            HttpWebRequest requestLogonBitrix24 = (HttpWebRequest)WebRequest.Create(BX_URI);

            //Настраиваем подключение, ничего не меняем
            string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(_username + ":" + _password));
            requestLogonBitrix24.Headers.Add("Authorization", "Basic " + svcCredentials);
            requestLogonBitrix24.AllowAutoRedirect = false; // Это обязательное условие, чтобы нас автоматически не переадресовывали на другую страницу
            requestLogonBitrix24.Method = "POST";

            //Подключаемся (отправляем запрос)
            HttpWebResponse responseLogonBitrix24 = (HttpWebResponse)requestLogonBitrix24.GetResponse();

            //Проверяем что статус-код доджен быть 302, нам должны предложить переадресацию, иначе авторизация не требуется, мы и так авторизированы
            if (responseLogonBitrix24.StatusCode == HttpStatusCode.Found)
            {
                //Ничего не меняем, здесь получаем из заголовков ответа Куки и параметры адреса переадресации (из поля "Location") парамер Code
                Uri locationURI = new Uri(responseLogonBitrix24.Headers["Location"]);
                // Ловко парсим URL-адрес с помощью HttpUtility, подключите "System.Web" через пакеты NuGet
                var locationParams = System.Web.HttpUtility.ParseQueryString(locationURI.Query);
                Cookie = responseLogonBitrix24.Headers["Set-Cookie"];
                Code = locationParams["Code"];

                //Вызываем исключение, если Код мы не смогли получить, без него далее ни как.
                if (String.IsNullOrEmpty(Code)) return false;
                //{
                //   throw new FormatException("CodeNotFound");
                //}

                //Закрываем подключение
                responseLogonBitrix24.Close();

                //Если код успешно получили, то формируем новый HTTP запрос для получения Токенов авторизации
                string BX_OAuth_URI = BX_OAuthSite + "/oauth/token" + "/?" + "grant_type=authorization_code" + "&" +
                "client_id=" + BX_ClientID + "&" +
                "client_secret=" + BX_ClientSecret + "&" +
                "code=" + Code;
                SetToken(BX_OAuth_URI);
            }
            return true;
        }

        //Закрытый метод для получения и записи Токенов авторизации
        private void SetToken(string BX_OAuth_URI)
        {
            //Формируем новый HTTP запрос для получения Токенов авторизации
            HttpWebRequest requestLogonBitrixOAuth = (HttpWebRequest)WebRequest.Create(BX_OAuth_URI);
            requestLogonBitrixOAuth.Method = "POST";
            requestLogonBitrixOAuth.Headers["Cookie"] = Cookie; //Используем Куки полученный в предыдущем запросе авторизации

            //Подключаемся (отправляем запрос)
            HttpWebResponse responseLogonBitrixOAuth = (HttpWebResponse)requestLogonBitrixOAuth.GetResponse();

            //Если в ответ получаем статус-код отличный от 200, то это ошибка, вызываем исключение
            if (responseLogonBitrixOAuth.StatusCode != HttpStatusCode.OK)
            {
                throw new FormatException("ErrorLogonBitrixOAuth");
            }
            else
            {
                //Читаем тело ответа
                Stream dataStreamLogonBitrixOAuth = responseLogonBitrixOAuth.GetResponseStream();
                var readerLogonBitrixOAuth = new StreamReader(dataStreamLogonBitrixOAuth);
                string stringLogonBitrixOAuth = readerLogonBitrixOAuth.ReadToEnd();

                //Обязательно закрываем подключения и потоки
                readerLogonBitrixOAuth.Close();
                responseLogonBitrixOAuth.Close();

                //Ловко преобразуем тело ответа в формате JSON в .Net объект с помощью Newtonsoft.Json, не забудьте подключить Newtonsoft.Json через NuGet
                var converter = new ExpandoObjectConverter();
                dynamic objLogonBitrixOAuth = JsonConvert.DeserializeObject<ExpandoObject>(stringLogonBitrixOAuth, converter);

                //Записывем Токены авторизации в поля нашего класса из динамического объекта
                AccessToken = objLogonBitrixOAuth.access_token;
                RefreshToken = objLogonBitrixOAuth.refresh_token;
                RefreshTime = DateTime.Now.AddSeconds(objLogonBitrixOAuth.expires_in); //Добавляем к текущей дате количество секунд действия токена, обычно это плюс один час

                //Закрываем поток
                dataStreamLogonBitrixOAuth.Close();
            }
        }

        //Закрытый метод для обновления Токенов авторизации, если истекло время их действия
        private void RefreshTokens()
        {
            if (RefreshTime == DateTime.MinValue) // Если RefreshTime пустая
            {
                //Тогда вызываем авторизацию по полной программе
                Connect();
                return; //Тогда дальше не идём
            }

            //Проверяем, если истекло время действия Токена авторизации, то обновляем его
            if (RefreshTime.AddSeconds(-5) < DateTime.Now)
            {
                //Формируем новый HTTP запрос для обновления Токена авторизации, здесь Code уже не нужен
                string BX_OAuth_URI = BX_OAuthSite + "/oauth/token" + "/?" + "grant_type=refresh_token" + "&" +
                "client_id=" + BX_ClientID + "&" +
                "client_secret=" + BX_ClientSecret + "&" +
                "refresh_token=" + RefreshToken;

                SetToken(BX_OAuth_URI);
            }
        }

        //метод для отправки REST-запросов в Битрикс24
        internal string SendCommand(string Command, string Params = "", string Body = "")
        {
            //Проверяем и обновлем Токены авторизации
            RefreshTokens();

            //Проверяем возможное указание параметров
            string BX_REST_URI = "";
            if (String.IsNullOrEmpty(Params))
                BX_REST_URI = BX_Portal + "/rest/" + Command + "?auth=" + AccessToken;
            else
                BX_REST_URI = BX_Portal + "/rest/" + Command + "?auth=" + AccessToken + "&" + Params;
            //Создаем новое HTTP подключение для отправки REST-запроса в Битрикс24
            HttpWebRequest requestBitrixREST = (HttpWebRequest)WebRequest.Create(BX_REST_URI);
            requestBitrixREST.Method = "POST";
            requestBitrixREST.Headers["Cookie"] = Cookie; //Используем Куки полученный в запросе авторизации

            //Готовим тело запроса и вставляем его в тело POST-запроса
            byte[] byteArrayBody = Encoding.UTF8.GetBytes(Body);
            requestBitrixREST.ContentType = "application/x-www-form-urlencoded";
            requestBitrixREST.ContentLength = byteArrayBody.Length;
            Stream dataBodyStream = requestBitrixREST.GetRequestStream();
            dataBodyStream.Write(byteArrayBody, 0, byteArrayBody.Length);
            dataBodyStream.Close();

            //Отправляем данные в Битрикс24
            HttpWebResponse responseBitrixREST = (HttpWebResponse)requestBitrixREST.GetResponse();

            //Читаем тело ответа от Битрикс24
            Stream dataStreamBitrixREST = responseBitrixREST.GetResponseStream();
            StreamReader readerBitrixREST = new StreamReader(dataStreamBitrixREST);
            string stringBitrixREST = readerBitrixREST.ReadToEnd();

            

            //Закрываем все подключения и потоки
            readerBitrixREST.Close();
            dataStreamBitrixREST.Close();
            responseBitrixREST.Close();
            //Возвращаем строку ответа в формате JSON
            return stringBitrixREST;
        }

        private Task<string> SendCommandAsync(string Command, string Params = "", string Body = "")
        {
            return Task.Run(() => SendCommand(Command, Params, Body));
        }

        #region Открытые методы

        public CRM.Companies Companies { get; set; }

        public void Initialization()
        {
            Companies = new CRM.Companies(this);
        }
        /*
        public async Task<Company> GetCompanyById(string id)
        {
            //Делаем запрос на список компаний с пользовательским полем ID
            string json = await SendCommandAsync("crm.company.list", "filter[UF_CRM_1473837528]=" + id + "&select[0]=ID")
                .ConfigureAwait(false);
            Console.WriteLine("Send 1 done");
            dynamic dResult = JsonConvert.DeserializeObject<ExpandoObject>(json);
            id = (string)dResult.result[0].ID;
            if (dResult.result.Count == 0) throw new ArgumentException("Компании с таким id не существует");

            // Берем из ответа объект компании и его поле системного ID и по нему ищим всю инфу
            json = await SendCommandAsync("crm.company.get", "ID=" + id).ConfigureAwait(false);
            Console.WriteLine("Send 2 done");
            Answer answer = JsonConvert.DeserializeObject<Answer>(json);
            Company requestCompany = answer.RequestCompany;
            return requestCompany;
        }
        







        public async Task<Company> GetCompanyByID(string id)
        {
            string json = await SendCommandAsync("crm.company.get", "ID=" + id).ConfigureAwait(false);
            Answer answer = JsonConvert.DeserializeObject<Answer>(json);
            Company requestCompany = answer.RequestCompany;
            return requestCompany;
        }

        public void DeleteCompany(string id)
        {
            SendCommand("crm.company.delete", "ID=" + id);
        }

        public async Task<Company> GetCompanyBy(string mail)
        {
            string json = string.Empty;
            try
            {
                json = await SendCommandAsync("crm.company.list", "FILTER[EMAIL]=" + mail + "&PARAMS[]=&SELECT[]=")
                    .ConfigureAwait(false);
            }
            catch(WebException)
            {
                throw new ArgumentException("Компании с таким email не найдено");
            }
            Answer answer = JsonConvert.DeserializeObject<Answer>(json);
            Company requestCompany = answer.RequestCompany;
            return requestCompany;
        }

        public async Task<Dictionary<string, Company>> GetTasksAsync(string managerId)
        {
            
                if (!UserExist(managerId)) throw new UserNotFoundException("Пользователь с таким id не найден");
                //Берем список задач с фильтром по группе и менеджеру    "ORDER[]=&FILTER[]=&SELECT[]=*"
                string TaskListByJSON = this.SendCommand("task.item.list",
                    "ORDER[STATUS]=&FILTER[RESPONSIBLE_ID]=" + managerId + "&FILTER[REAL_STATUS]=2&FILTER[GROUP_ID]=1&PARAMS[]=&SELECT[]=");
                ExpandoObjectConverter converter = new ExpandoObjectConverter();
                dynamic dResult = JsonConvert.DeserializeObject<ExpandoObject>(TaskListByJSON, converter);
                List<dynamic> tasks = dResult.result;
                //Ищем в найденых задачах те, что на стадии "письмо" (92)
                Dictionary<string, Company> curTasks = new Dictionary<string, Company>();
                foreach (dynamic task in tasks)
                {
                    string taskJSON = this.SendCommand("task.item.getdata", "TASKID=" + task.ID);
                    dResult = JsonConvert.DeserializeObject<ExpandoObject>(taskJSON, converter);
                    if (dResult.result.STAGE_ID == "92" || dResult.result.STAGE_ID == "58")
                    {
                        //фомируем список словаря с задачей и компанией 
                        List<object> crmCompanys = dResult.result.UF_CRM_TASK;
                        string companyId = (string)crmCompanys[0];
                        Company company = new Company();
                        company = await GetCompanyByID(companyId.Remove(0, 3));
                        curTasks.Add(task.ID, company);
                    }
                }
                return curTasks.Count == 0 ? null : curTasks;
            
        }

        public Task UpdateTask(string taskID)
        {
            return Task.Run(() =>
            {
                string checkList = this.SendCommand("task.checklistitem.getlist", "TASKID=" + taskID);
                dynamic dList = JsonConvert.DeserializeObject<ExpandoObject>(checkList);
                List<dynamic> list = dList.result;
                this.SendCommand("task.checklistitem.complete", "TASKID=" + taskID + "&ITEMID=" + list[0].ID);
                this.SendCommand("task.item.startexecution", "TASKID=" + taskID);
                this.SendCommand("task.stages.movetask", "id=" + taskID + "&stageId=94");
            });
        }

        public bool UserExist(string id)
        {
            string user = SendCommand("user.get", "ID=" + id);
            dynamic dList = JsonConvert.DeserializeObject<ExpandoObject>(user);
            return dList.result.Count == 0 ? false : true;
        }
        public string UserName(string id)
        {
            string user = SendCommand("user.get", "ID=" + id);
            dynamic dList = JsonConvert.DeserializeObject<ExpandoObject>(user);
            List<dynamic> users = dList.result;
            user = users[0].LAST_NAME + " " + users[0].NAME;
            return user;
        }
    }
    */
        #endregion

        class UserNotFoundException : Exception
        {
            public UserNotFoundException(string message)
                : base(message)
            { }
        }





    }
}
