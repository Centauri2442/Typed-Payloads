
namespace VIOLET.Payloads
{
    /// <summary>
    /// Wraps an immutable payload value with the timestamp at which it was recorded.
    /// </summary>
    /// <typeparam name="TPayload">The payload struct type.</typeparam>
    public readonly struct PayloadEntry<TPayload> where TPayload : struct
    {
        /// <summary>
        /// The payload data.
        /// </summary>
        public TPayload Data { get; }

        /// <summary>
        /// The time in seconds at which this payload was recorded, as provided by the caller.
        /// </summary>
        public float Timestamp { get; }

        /// <summary>
        /// Initializes a new PayloadEntry with the given data and timestamp.
        /// </summary>
        /// <param name="data">The payload data.</param>
        /// <param name="nowSeconds">The time in seconds at which this payload was recorded.</param>
        public PayloadEntry(TPayload data, float nowSeconds)
        {
            Data = data;
            Timestamp = nowSeconds;
        }
    }
}