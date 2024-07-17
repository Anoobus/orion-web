using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orion.Web.Util;

namespace Orion.Web.BLL.Core
{
    public interface IProduces<T>
    {
    }

    public interface IProcessResult<T>
    {
        public T Success { get; }
        public PresetError Error { get; }
        public IActionResult AsApiResult();
    }

    public class Processresult<T> : IProcessResult<T>
    {
        public Processresult(PresetError error)
        {
            Success = default;
            Error = error;
            _wasSuccess = false;
        }

        public Processresult(T success, int apiHttpStatusCode = StatusCodes.Status200OK)
        {
            Success = success;
            Error = null;
            _apiHttpStatusCode = apiHttpStatusCode;
            _wasSuccess = true;
        }

        public T Success { get; }
        public PresetError Error { get; }
        private readonly bool _wasSuccess;
        private readonly int _apiHttpStatusCode;

        public IActionResult AsApiResult()
        {
            if (_wasSuccess)
            {
                return new ObjectResult(Success)
                {
                    StatusCode = _apiHttpStatusCode
                };
            }

            return Error.AsActionResult();
        }

        public static implicit operator T(Processresult<T> d) => d.Success;

        public static implicit operator PresetError(Processresult<T> d) => d.Error;
    }

    public interface IOrchastrator<in TMessage, TResult>
        where TMessage : IProduces<TResult>
    {
        Task<IProcessResult<TResult>> Process(TMessage msg);
    }

    public abstract class Orchastrator<TMessage, TResult> : IOrchastrator<TMessage, TResult>
      where TMessage : IProduces<TResult>

    {
        private static readonly Serilog.ILogger _logger = Serilog.Log.Logger.ForContext(typeof(Orchastrator<,>));

        public async Task<IProcessResult<TResult>> Process(TMessage msg)
        {
            try
            {
                return await Handle(msg);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"An error occured trying to handle {msg.Dump()}");
                throw;
            }
        }

        protected abstract Task<IProcessResult<TResult>> Handle(TMessage msg);

        protected IProcessResult<TResult> Success(TResult result, int apiHttpStatusCode = StatusCodes.Status200OK)
        {
            return new Processresult<TResult>(result);
        }

        protected IProcessResult<TResult> Failure(PresetError err)
        {
            return new Processresult<TResult>(err);
        }
    }
}
