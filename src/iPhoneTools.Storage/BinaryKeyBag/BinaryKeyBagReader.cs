using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

/// <remarks>
/// keybag
///
/// A data structure used to store a collection of class keys.
/// Each type(user, device, system, backup, escrow, or iCloud Backup) has the same format:
/// • A header containing:
///   – Version(set to four in iOS 12 or later)
///   – Type(system, backup, escrow, or iCloud Backup)
///   – Keybag UUID
///   – An HMAC if the keybag is signed
///   – The method used for wrapping the class keys—tangling with the UID or PBKDF2, along with the salt and iteration count
/// • A list of class keys:
///   – Key UUID
///   – Class (which file or Keychain Data Protection class)
///   – Wrapping type (UID-derived key only; UID-derived key and passcode-derived key)
///   – Wrapped class key
///   – Public key for asymmetric classes
///
/// key wrapping
/// Encrypting one key with another. iOS uses NIST AES key wrapping, as per RFC 3394.
///  
/// tangling
/// The process by which a user’s passcode is turned into a cryptographic key and strengthened with the device’s UID.
/// This ensures that a brute-force attack must be performed on a given device, and thus is rate limited and can’t be performed in parallel.
/// The tangling algorithm is PBKDF2, which uses AES keyed with the device UID as the pseudorandom function (PRF) for each iteration.
/// </remarks>

namespace iPhoneTools
{
    public static class BinaryKeyBagReader
    {
        private enum ReaderState
        {
            Invalid,
            ReadHeader,
            ReadKeyBagEntry,
        }

        public static KeyBag Read(byte[] data)
        {
            KeyBag result = default;

            var state = ReaderState.Invalid;
            KeyBagEntry currentKey = default;
            var keys = new List<KeyBagEntry>();
            var length = data.Length;
            var position = 0;

            while (position < length)
            {
                var blockIdentifier = Encoding.ASCII.GetString(data, position, 4);
                position += 4;

                var blockLength = BinaryPrimitives.ReadInt32BigEndian(data.AsSpan(position, 4));
                position += 4;

                var value = data.AsSpan(position, blockLength);

                if (state == ReaderState.Invalid)
                {
                    state = ReaderState.ReadHeader;
                }
                if (state == ReaderState.ReadHeader)
                {
                    if (result is null)
                    {
                        result = new KeyBag();
                    }
                    if (blockIdentifier == KeyBagConstants.UuidTag && result.Uuid != Guid.Empty)
                    {
                        state = ReaderState.ReadKeyBagEntry;
                    }
                    else
                    {
                        result.SetValue(blockIdentifier, value);
                    }
                }
                if (state == ReaderState.ReadKeyBagEntry)
                {
                    if (blockIdentifier == KeyBagConstants.UuidTag)
                    {
                        if (currentKey != null)
                        {
                            keys.Add(currentKey);
                        }
                        currentKey = new KeyBagEntry();
                    }

                    currentKey.SetValue(blockIdentifier, value);
                }
                position += blockLength;
            }
            if (result != null)
            {
                if (currentKey != null)
                {
                    keys.Add(currentKey);
                }
                result.WrappedKeys = keys.ToArray();
            }

            return result;
        }
    }
}
