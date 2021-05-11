using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class GeneralMessages
    {

        public string _entityName { get; set; }

        public GeneralMessages(string entityName)
        {
            _entityName = entityName;
        }

        public string msgInsert { get { return string.Format(ConfigurationManager.AppSettings["msgInsert"].ToString(), _entityName); } }
        public string msgUpdate { get { return string.Format(ConfigurationManager.AppSettings["msgUpdate"].ToString(), _entityName); } }
        public string msgEntryExists { get { return string.Format(ConfigurationManager.AppSettings["msgEntryExists"].ToString(), _entityName); } }
        public string msgError { get { return string.Format(ConfigurationManager.AppSettings["msgError"].ToString(), _entityName); } }
        public string msgDelete { get { return string.Format(ConfigurationManager.AppSettings["msgDelete"].ToString(), _entityName); } }
        public string msgChangeStatus { get { return string.Format(ConfigurationManager.AppSettings["msgChangeStatus"].ToString(), _entityName); } }
        public string msgStatusError { get { return string.Format(ConfigurationManager.AppSettings["msgStatusError"].ToString(), _entityName); } }
        public string msgParentExists { get { return string.Format(ConfigurationManager.AppSettings["msgParentExists"].ToString(), _entityName); } }
        public string msgAccessDenied { get { return string.Format(ConfigurationManager.AppSettings["msgAccessDenied"].ToString(), _entityName); } }
        public string msgCancel { get { return string.Format(ConfigurationManager.AppSettings["msgCancel"].ToString(), _entityName); } }
        public string msgApproved { get { return string.Format(ConfigurationManager.AppSettings["msgApproved"].ToString(), _entityName); } }
        public string msgDisApproved { get { return string.Format(ConfigurationManager.AppSettings["msgDisApproved"].ToString(), _entityName); } }
        public string msgApplied { get { return string.Format(ConfigurationManager.AppSettings["msgApplied"].ToString(), _entityName); } }
        public string msgHold { get { return string.Format(ConfigurationManager.AppSettings["msgHold"].ToString(), _entityName); } }
        public string msgUnHold { get { return string.Format(ConfigurationManager.AppSettings["msgUnHold"].ToString(), _entityName); } }
        public string msgChangePassword { get { return string.Format(ConfigurationManager.AppSettings["msgChangePassword"].ToString(), _entityName); } }
    }
}