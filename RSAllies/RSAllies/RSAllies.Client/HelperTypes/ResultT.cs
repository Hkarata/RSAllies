using Newtonsoft.Json;

namespace RSAllies.Client.HelperTypes
{
    public class Result<TValue> : Result
    {
        private readonly TValue? _value;

        [JsonConstructor]
        protected internal Result(TValue? value, bool isSuccess, Error error)
            : base(isSuccess, error) =>
            _value = value;

        public List? Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException("The value of a failure result can not be accessed.");

        public static implicit operator Result<TValue>(TValue? value) => Create(value);
    }
}
