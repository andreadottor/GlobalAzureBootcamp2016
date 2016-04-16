using System;

namespace Dottor.gab16pn.Web.Models
{
   
    public class ErrorModel
    {
        public ErrorModel()
        {

        }

        public ErrorModel(Exception ex)
        {
            this.Message = ex.Message;
            this.StackTrace = ex.StackTrace;
        }
        
        public string Message { get; set; }

        public string StackTrace { get; set; }
    }
}
