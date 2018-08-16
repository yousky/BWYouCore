using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace BWYouCore.Web.MVC.ViewModels
{
    public class ErrorResultViewModel
    {
        public WebStatusMessageBody Error { get; set; }
        public List<ValidationObjectError> ModelValidResult { get; set; }
        public dynamic Etc { get; set; }

        public ErrorResultViewModel()
        {

        }

        public ErrorResultViewModel(int httpStatusCode, string message, dynamic etc = null)
        {
            Error = new WebStatusMessageBody()
            {
                Status = httpStatusCode,
                Code = "E" + string.Format("{0:D3}", httpStatusCode),
                Message = message,
                Link = "",
                DeveloperMessage = ""
            };
            Etc = etc;
        }

        //TODO ExceptionHandlerContext 어떻게 바뀐걸까.
        public ErrorResultViewModel(int httpStatusCode, Exception ex, dynamic etc = null)
        {
            Error = new WebStatusMessageBody()
            {
                Status = httpStatusCode,
                Code = "E" + string.Format("{0:D3}", httpStatusCode),
                Message = ex.Message,
                Link = "",
#if (!DEBUG)
                        DeveloperMessage = ""
#else
                DeveloperMessage = ex.ToString()
#endif
            };
            Etc = etc;
        }

        public ErrorResultViewModel(int httpStatusCode, ModelStateDictionary modelState, dynamic etc = null)
        {
            Error = new WebStatusMessageBody()
            {
                Status = httpStatusCode,
                Code = "E400",
                Message = "Validation Fail",
                Link = "",
#if(!DEBUG)
                DeveloperMessage = ""
#else
                DeveloperMessage = "Validation Fail"
#endif
            };
            ModelValidResult = GetValidationObjectErrors(modelState);
            Etc = etc;
        }

        protected List<ValidationObjectError> GetValidationObjectErrors(ModelStateDictionary modelState)
        {
            List<ValidationObjectError> validationObjectErrors = new List<ValidationObjectError>();
            foreach (var key in modelState.Keys)
            {
                foreach (var e in modelState[key].Errors)
                {
                    validationObjectErrors.Add(new ValidationObjectError(key, e.ErrorMessage));
                }
            }

            return validationObjectErrors;
        }
    }
}
