using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitrix.Tasks
{
    class Tasks
    {
        #region Свойства
        private readonly Bitrix24 _bitrix;
        #endregion

        public Tasks(Bitrix24 bitrix)
        {
            _bitrix = bitrix;
        }

        #region Методы
        public void Add()
        {

        }

        public void Get(string taskId)
        {

        }

        public void Update()
        {

        }

        public void Delete(string taskId)
        {

        }

        public void GetFiles(string taskId)
        {

        }

        public void GetDescription(string taskId)
        {

        }

        public void GetDependson(string taskId)
        {

        }

        public void Delegate(string taskId, string userId)
        {

        }

        public void StartTask(string taskId)
        {

        }

        public void Defer(string taskId)
        {

        }

        public void Complete(string taskId)
        {

        }

        public void Renew(string taskId) //"не выполняется"
        {

        }

        public void Approve(string taskId)// "завершена"
        {

        }
        /// <summary>
        /// Добавить в избранное
        /// </summary>
        /// <param name="taskId">номер задачи</param>
        /// <param name="affectChildren">включать подзадачи в добавление. по умолчнию false</param>
        public void AddToFavorite(string taskId, bool affectChildren = false)
        {

        }
        /// <summary>
        /// Удалить из избранного
        /// </summary>
        /// <param name="taskId">номер задачи</param>
        /// <param name="affectChildren">включать подзадачи в добавление. по умолчнию true</param>
        public void DeleteOnFavorite(string taskId, bool affectChildren = true)
        {

        }

        public void AddFile() // только post запросы файлы в base64
        {

        }
        /// <summary>
        /// Открепляет файл от задачи
        /// </summary>
        /// <param name="taskId">Идентификатор задачи</param>
        /// <param name="fileId">Идентификатор прикрепленного файла</param>
        public void DeleteFile(string taskId, string fileId)
        {

        }
        #endregion

        #region Закрытые методы

        private void GetAllowedActions(string taskId) //task.item.getallowedtaskactionsasstrings
        {

        }

        private bool IsActionAllowed()
        {
            return true;
        }
        #endregion
    }
}
