using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using orion.web.Util;

namespace orion.web.BLL
{
    public interface IResult
    {
        ActionResult AsActionResult();
    }
    public interface IMessage<TResult> where TResult : IResult
    {

    }

    public abstract class HandleBase<T, U>
        where T : IMessage<U>
        where U : IResult

    {
         private static readonly Serilog.ILogger _logger = Serilog.Log.Logger.ForContext(typeof(HandleBase<,>));
      
        public async Task<IActionResult> Process(T msg)
        {
            try
            {
                return (await Handle(msg)).AsActionResult();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"An error occured trying to handle {msg.Dump()}");
                return ApiErrors.UnHandledException(ex.Message,ex).AsActionResult();
            }
        }
        protected abstract Task<IResult> Handle(T msg);

    }
}

