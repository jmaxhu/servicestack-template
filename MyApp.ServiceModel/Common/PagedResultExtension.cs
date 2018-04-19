using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyApp.ServiceModel.Common
{
    public static class PagedResultExtension
    {
        public static async Task<PagedResult<TTo>> ToResDtos<TFrom, TTo>(
            this PagedResult<TFrom> result,
            Func<TFrom, TTo> converterFunc) where TFrom : class where TTo : class
        {
            var resDtos = new List<TTo>(result.Results.Count);

            result.Results.ForEach(item => resDtos.Add(converterFunc(item)));

            return await Task.FromResult(new PagedResult<TTo>
            {
                PageIndex = result.PageIndex,
                PageSize = result.PageSize,
                Total = result.Total,
                Results = resDtos
            });
        }
    }
}