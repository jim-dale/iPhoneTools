
namespace iPhoneTools
{
    public enum ProtectionClass
    {
        NotSet = 0,

        NSFileProtectionComplete = 1,
        NSFileProtectionCompleteUnlessOpen = 2,
        NSFileProtectionCompleteUntilFirstUserAuthentication = 3,
        NSFileProtectionNone = 4,
        NSFileProtectionRecovery = 5,

        kSecAttrAccessibleWhenUnlocked = 6,
        kSecAttrAccessibleAfterFirstUnlock = 7,
        kSecAttrAccessibleAlways = 8,
        kSecAttrAccessibleWhenUnlockedThisDeviceOnly = 9,
        kSecAttrAccessibleAfterFirstUnlockThisDeviceOnly = 10,
        kSecAttrAccessibleAlwaysThisDeviceOnly = 11,
    }
}
