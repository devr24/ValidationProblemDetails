using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;
using System.Text.Json.Serialization;

namespace ValidationProblemDetails.Models
{
    public class ValidationProblemInfo : HttpValidationProblemDetails
    {
        public ValidationProblemInfo(ControllerContext context, HttpStatusCode statusCode) : base(context.ModelState.ToDictionary())
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
            Title = "One or more model validation errors occurred.";
            Status = (int)statusCode;
            Detail = "See the errors property for details";
            Instance = context.HttpContext.Request.Path;
        }

        public ValidationProblemInfo(ModelStateDictionary modelState) : base(modelState.ToDictionary()) { }

        /// <summary>
        /// Gets the validation errors associated with this instance of <see cref="ValidationProblemDetails"/>.
        /// </summary>
        [JsonIgnore]
        public new IDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>(StringComparer.Ordinal);

        /// <summary>
        /// Gets the validation errors associated with this instance of <see cref="ValidationProblemDetails"/>.
        /// </summary>
        [JsonPropertyName("errors")]
        [JsonPropertyOrder(1)]
        public IDictionary<string, ErrorItem[]> ErrorItems => GetErrors();

        private IDictionary<string, ErrorItem[]> GetErrors()
        {
            var returnItems = new Dictionary<string, ErrorItem[]>();
            foreach (var err in base.Errors)
            {
                returnItems.Add(err.Key, err.Value.Select(e => ErrorItem.Parse(err.Key, e)).ToArray());
            }

            return returnItems;
        }

    }

    public class ErrorItem
    {
        public ErrorItem(string code, string message)
        {
            Code = code;
            Message = message;
        }

        public string Code { get; }
        public string Message { get; }

        public static ErrorItem Parse(string key, string parseMessage)
        {
            var parts = parseMessage.Split('|');
            if (int.TryParse(parts[0], out _))
            {
                var message = parts.Length > 1 ? string.Join('|', parts.Skip(1)) : $"{key} error occurred";
                return new ErrorItem(parts[0], message);
            }

            return new ErrorItem("0000", parseMessage);
        }
    }
}