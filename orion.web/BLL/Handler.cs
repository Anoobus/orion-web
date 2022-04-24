using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using orion.web.BLL;
using orion.web.Util;

namespace orion.web.BLL
{
    public interface IResult
    {
        ActionResult AsActionResult();
    }

    public interface IMessageHandler<in TMessage>
    {
        Task<IActionResult> Process(TMessage msg);
    }
    public interface IMessage<out TResult>  where TResult : IResult
    {
      
    }
    public interface IOnProcessEventEmitter
    {
          Action<IResult> OnProcessComplete { get; set; }
    }
    public interface IHandler<in T, out U> : IMessageHandler<T>
        where T : IMessage<U>
        where U : IResult
    
    {
      
    }

    
    
    public abstract class HandleBase<T, U> : IHandler<T, U>, IOnProcessEventEmitter
        where T : IMessage<U>
        where U : IResult
    {
        
        private static readonly Serilog.ILogger _logger = Serilog.Log.Logger.ForContext(typeof(HandleBase<,>));

        public Action<IResult> OnProcessComplete { get; set; }

        public async Task<IActionResult> Process(T msg)
        {
            try
            {
                var res = await Handle(msg);
                OnProcessComplete?.Invoke(res);
                return res.AsActionResult();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"An error occured trying to handle {msg.Dump()}");
                var res = ApiErrors.UnHandledException(ex.Message, ex);
                OnProcessComplete?.Invoke(res);
                return res.AsActionResult();
            }
        }

        protected abstract Task<IResult> Handle(T msg);

    }
}

