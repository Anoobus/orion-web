using System;
namespace Orion.Web.UI.Models
{
    public class EmptyResult
    {
        public static EmptyResult Instance => _instance;
        private static readonly EmptyResult _instance = new EmptyResult();
    }
}
