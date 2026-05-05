using System;
using System.Collections.Generic;

namespace VIOLET.Payloads
{
    /// <summary>
    /// Registry service that stores and retrieves typed, immutable payload entries per owner.
    /// </summary>
    public class PayloadRegistry
    {
        private readonly Dictionary<Type, object> _buckets = new Dictionary<Type, object>(); // Payload storage
        
        #region Public API

        /// <summary>
        /// Stores a payload entry for the given owner, replacing any existing entry of the same type.
        /// </summary>
        /// <typeparam name="TPayload">The payload struct type.</typeparam>
        /// <param name="owner">The object that owns this payload.</param>
        /// <param name="payload">The payload data to store.</param>
        /// <param name="nowSeconds">The current time in seconds, captured on the main thread via Time.time.</param>
        public void Set<TPayload>(object owner, TPayload payload, float nowSeconds) where TPayload : struct
        {
            var bucket = GetOrCreateBucket<TPayload>();
            var entry = new PayloadEntry<TPayload>(payload, nowSeconds);

            bucket[owner] = entry;
        }

        /// <summary>
        /// Attempts to retrieve the payload entry for the given owner.
        /// </summary>
        /// <typeparam name="TPayload">The payload struct type.</typeparam>
        /// <param name="owner">The object that owns this payload.</param>
        /// <param name="entry">The retrieved payload entry, if found.</param>
        /// <returns>TRUE if an entry was found</returns>
        public bool TryGet<TPayload>(object owner, out PayloadEntry<TPayload> entry) where TPayload : struct
        {
            if (TryGetBucket<TPayload>(out var bucket))
            {
                if (bucket.TryGetValue(owner, out var boxedPayload))
                {
                    // Unbox the payload
                    entry = (PayloadEntry<TPayload>)boxedPayload;
                    return true;
                }
            }
            
            entry = default;
            return false;
        }

        /// <summary>
        /// Removes all payload entries for the given owner across all payload types.
        /// </summary>
        /// <param name="owner">The object whose payloads should be removed.</param>
        public void Clear(object owner)
        {
            foreach (var pair in _buckets)
            {
                var bucket = (Dictionary<object, object>)pair.Value;
                bucket.Remove(owner);
            }
        }

        /// <summary>
        /// Removes the payload entry of the given type for the given owner.
        /// </summary>
        /// <typeparam name="TPayload">The payload struct type to remove.</typeparam>
        /// <param name="owner">The object whose payload should be removed.</param>
        public void Clear<TPayload>(object owner) where TPayload : struct
        {
            if (TryGetBucket<TPayload>(out var bucket))
            {
                bucket.Remove(owner);
            }
        }

        #endregion

        #region Helpers

        private Dictionary<object, object> GetOrCreateBucket<TPayload>() where TPayload : struct
        {
            var type = typeof(TPayload);

            if (_buckets.TryGetValue(type, out var existing))
            {
                return (Dictionary<object, object>)existing;
            }
            
            var bucket = new Dictionary<object, object>();
            _buckets[type] = bucket;
            return bucket;
        }

        private bool TryGetBucket<TPayload>(out Dictionary<object, object> bucket) where TPayload : struct
        {
            if (_buckets.TryGetValue(typeof(TPayload), out var data))
            {
                bucket = (Dictionary<object, object>)data;
                return true;
            }

            bucket = null;
            return false;
        }

        #endregion
    }
}