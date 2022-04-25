using System;
using Microsoft.AspNetCore.Mvc;

namespace orion.web.BLL
{
    public enum ErrorClassification
    {
        Unhandled,
        NotFound,
        BadRequest
    }
    public class PresetError
    {
        private readonly ErrorClassification classification;
        private readonly string title;
        private readonly string detail;
        private readonly string jsonPath;
        private readonly Exception exception;

        public PresetError(ErrorClassification classification, string title, string detail, string jsonPath = null, Exception exception = null)
        {
            this.classification = classification;
            this.title = title;
            this.detail = detail;
            this.jsonPath = jsonPath;
            this.exception = exception;
        }

        public ActionResult AsActionResult()
        {
            throw new NotImplementedException();
        }
    }

    public class ApiErrors
    {
        internal static PresetError UnHandledException(string message, Exception ex) => new PresetError(ErrorClassification.Unhandled, "General Error", message, null, ex);
        internal static PresetError NotFoundException(string message) => new PresetError(ErrorClassification.NotFound, "Supplied resource could not be found", message, null,null);          
    }
}