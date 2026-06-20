using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGManagement.MobileApp.Services;

public interface IApiService
{
    Task<TResponse?> GetAsync<TResponse>(string endpoint);
    Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest body);
    Task<HttpResponseMessage> PostRawAsync<TRequest>(string endpoint, TRequest body);
}
