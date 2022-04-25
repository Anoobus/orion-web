using System;
namespace orion.web.UI.Models
{
    public class EmptyResult
    {
        public static EmptyResult Instance => _instance;
        private static readonly EmptyResult _instance = new EmptyResult();
        
    }
}

