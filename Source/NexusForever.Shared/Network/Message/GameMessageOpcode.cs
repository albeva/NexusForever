namespace NexusForever.Shared.Network.Message
{
    public enum GameMessageOpcode
    {
        State                           = 0x0000,
        State2                          = 0x0001,
        ServerHello                     = 0x0003,
        ServerMaxCharacterLevelAchieved = 0x0036,
        ServerPlayerEnteredWorld        = 0x0061,
        ServerAuthEncrypted             = 0x0076,
        ServerCharacterLogoutStart      = 0x0092,
        ServerChangeWorld               = 0x00AD,
        ClientVendorPurchase            = 0x00BE,
        ClientCharacterLogout           = 0x00BF,
        ClientLogout                    = 0x00C0,
        ServerCharacterCreate           = 0x00DC,
        ServerChannelUpdateLoot         = 0x00DD,
        Server00F1                      = 0x00F1,
        Server0104                      = 0x0104, // Galactic Archive
        ServerCharacter                 = 0x010F, // single character
        ServerItemAdd                   = 0x0111,
        ServerCharacterList             = 0x0117,
        ServerMountUnlocked             = 0x0129,
        ServerItemDelete                = 0x0148,
        ClientItemDelete                = 0x0149,
        ServerCharacterSelectFail       = 0x0162,
        Server0169                      = 0x0169, // ability book related
        ClientItemSplit                 = 0x017D,
        ServerItemStackCountUpdate      = 0x017F,
        ClientItemMove                  = 0x0182,
        ClientEntitySelect              = 0x0185,
        ServerFlightPathUpdate          = 0x0188,
        ServerTitleSet                  = 0x0189,
        ServerTitleUpdate               = 0x018A,
        ServerTitles                    = 0x018B,
        ServerPlayerChanged             = 0x019B,
        Server019D                      = 0x019D, // action set related
        Server01A0                      = 0x01A0, // ability book related
        Server01A3                      = 0x01A3, // AMP
        ServerChatJoin                  = 0x01BC,
        ServerChatAccept                = 0x01C2,
        ClientChat                      = 0x01C3,
        ServerChat                      = 0x01C8,
        Server0237                      = 0x0237, // UI related, opens or closes different UI windows (bank, barber, ect...)
        ClientPing                      = 0x0241,
        ClientEncrypted                 = 0x0244,
        ServerCombatLog                 = 0x0247,
        ClientCharacterCreate           = 0x025B,
        ClientPacked                    = 0x025C, // the same as ClientEncrypted except the contents isn't encrypted?
        ServerPlayerCreate              = 0x025E,
        ServerEntityCreate              = 0x0262,
        ClientCharacterDelete           = 0x0352,
        ServerEntityDestory             = 0x0355,
        ClientEmote                     = 0x037E,
        Server03AA                      = 0x03AA, // friendship account related
        Server03BE                      = 0x03BE, // friendship related
        ServerRealmInfo                 = 0x03DB,
        ServerRealmEncrypted            = 0x03DC,
        ClientCheat                     = 0x03E0,
        Server0497                      = 0x0497, // guild info
        ServerItemSwap                  = 0x0568,
        ServerItemMove                  = 0x0569,
        ClientHelloRealm                = 0x058F,
        ServerAuthAccepted              = 0x0591,
        ClientHelloAuth                 = 0x0592,
        ServerMovementControl           = 0x0636,
        ServerClientLogout              = 0x0594,
        ClientEntityCommand             = 0x0637, // bidirectional? packet has both read and write handlers 
        ServerEntityCommand             = 0x0638, // bidirectional? packet has both read and write handlers
        ServerPathLog                   = 0x06BC,
        ServerRealmList                 = 0x0761, // bidirectional? packet has both read and write handlers
        ServerRealmMessages             = 0x0763,
        ClientTitleSet                  = 0x078E,
        ClientRealmList                 = 0x07A4,
        ClientCharacterList             = 0x07E0,
        ClientVendor                    = 0x07EA,
        ClientCharacterSelect           = 0x07DD,
        ClientStorefrontRequestCatalog  = 0x082D,
        Server0854                      = 0x0854, // crafting schematic
        Server0856                      = 0x0856, // tradeskills
        Server086F                      = 0x086F,
        Server08B3                      = 0x08B3,
        ServerVendor                    = 0x090B,
        ServerPlayerCurrencyChanged     = 0x0919,
        ServerItemVisualUpdate          = 0x0933,
        Server0934                      = 0x0934,
        ServerEmote                     = 0x093C,
        ClientWhoRequest                = 0x0959,
        ServerWhoResponse               = 0x095A,
        ServerAccountEntitlements       = 0x0968
    }
}
