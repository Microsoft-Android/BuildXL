// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.ContractsLight;
using System.Linq;

namespace BuildXL.Cache.ContentStore.Hashing
{
    /// <summary>
    ///     Extension methods for HashType
    /// </summary>
    public static class HashTypeExtensions
    {
        private static readonly Dictionary<string, HashType> NameToValue =
            new Dictionary<string, HashType>(StringComparer.OrdinalIgnoreCase)
            {
                {"MD5", HashType.MD5},
                {"SHA1", HashType.SHA1},
                {"SHA256", HashType.SHA256},
                {"VSO0", HashType.Vso0},
                {"DEDUPCHUNK", HashType.DedupSingleChunk},
                {"DEDUPNODE", HashType.DedupNode},
                {"DEDUPNODEORCHUNK", HashType.Dedup64K}, // Leaving the DEDUPNODEORCHUNK moniker here for back-compat.
                {"DEDUP1024K", HashType.Dedup1024K},
                {"MURMUR", HashType.Murmur},
            };

        private static readonly Dictionary<HashType, string> ValueToName = NameToValue.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

        /// <summary>
        ///     Lookup a hash type by case-insensitive name string.
        /// </summary>
        public static HashType FindHashTypeByName(this string name)
        {
            if (!Enum.TryParse(name, true, out HashType hashType))
            {
                throw new ArgumentException($"HashType by name={name} is not recognized.");
            }

            return hashType;
        }

        /// <summary>
        ///     Serialize to string.
        /// </summary>
        public static string Serialize(this HashType hashType)
        {
            if (ValueToName.TryGetValue(hashType, out var result))
            {
                return result;
            }

            return hashType.ToString().ToUpperInvariant();
        }

        /// <summary>
        ///     Deserialize from string.
        /// </summary>
        public static bool Deserialize(this string value, out HashType hashType)
        {
            Contract.Requires(value != null);

            return NameToValue.TryGetValue(value, out hashType);
        }

         /// <summary>
        ///     IsValidDedupHashType - determine if the hash type is 'valid'.
        /// </summary>
        /// <param name="hashType">The given hash type.</param>
        /// <returns>True, if valid. False, otherwise.</returns>
        public static bool IsValidDedup(this HashType hashType)
        {
            switch (hashType)
            {
                case HashType.Dedup1024K:
                case HashType.Dedup64K:
                    return true;
                case HashType.Unknown:
                case HashType.SHA1:
                case HashType.SHA256:
                case HashType.MD5:
                case HashType.Vso0:
                case HashType.DedupSingleChunk:
                case HashType.DedupNode: // TODO: Chunk size optimization - remove this one entirely.
                case HashType.Murmur:
                case HashType.DeprecatedVso0:
                    return false;
                default:
                    throw new NotImplementedException($"Unsupported enum {hashType} of type {nameof(HashType)} encountered.");
            }
        }
    }
}
