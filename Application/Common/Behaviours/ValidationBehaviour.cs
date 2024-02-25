using Application.Common.Exceptions;
using FluentValidation;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Behaviours
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {

            if (_validators.Any())
            {

                var errors = new Dictionary<string, string>();

                var context = new ValidationContext<TRequest>(request);
                var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

                if (failures.Any())
                {
                    int errorCount = 1;

                    if (failures.Count > 1)
                    {
                        foreach (var error in failures)
                        {
                            var errorTitle = string.Empty;

                            if (string.IsNullOrEmpty(error.PropertyName))
                            {
                                errorTitle = $"Error #{errorCount++}";
                            }
                            else
                            {
                                errorTitle = error.PropertyName.ToLower().Replace("dto.", "");
                            }

                            if (errors.ContainsKey(errorTitle))
                            {
                                errorTitle = $"{errorTitle} #{errors.Count(a => a.Key.Contains(errorTitle)) + 1}";
                            }

                            errors.Add(errorTitle, error.ErrorMessage.ToLower().Replace("dto. ", ""));
                        }

                        throw new ResponseException(HttpStatusCode.BadRequest, "Validation errors occurred", errors);
                    }
                    else
                    {
                        throw new ResponseException(HttpStatusCode.BadRequest, failures.FirstOrDefault().ErrorMessage.ToLower().Replace("dto. ", ""), null);
                    }

                }

            }
            return await next();
        }
    }
}
