namespace Microsoft.AspNetCore.Mvc.ModelBinding
{
    public static class ModelStateExtensions
    {
        public static Dictionary<string, string[]> ToDictionary(this ModelStateDictionary modelState)
        {
            var result = new Dictionary<string, string[]>();
            foreach (var kv in modelState)
            {
                var value = kv.Value;
                var arr = value.Errors.Select(e => e.ErrorMessage).ToArray();
                result.Add(kv.Key, arr);
            }

            return result; // modelState.ToDictionary(x => x.Key, x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray());
        }
    }
}
