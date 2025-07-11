public static class APIEndpoints
    {
    // Base URLs (có thể thay đổi tùy môi trường)
    public static string BaseWalletURL = "http://localhost:3001";
    public static string BaseGameURL = "http://localhost:3000";

    // Các endpoint cụ thể
    public static string CreateWallet => $"{BaseWalletURL}/wallet/create";
    public static string UpdateWallet => $"{BaseGameURL}/update-wallet";
    public static string ImportWallet => $"{BaseWalletURL}/wallet/import";
    public static string WalletTransfer => $"{BaseWalletURL}/wallet/transfer";
    public static string GetWallet => $"{BaseGameURL}/get-wallet";

    // Game-specific APIs
    public static string GetTopAppleWritten => $"{BaseGameURL}/top-apple-written";
    public static string GetTopSwordCollected => $"{BaseGameURL}/top-sword-collected";
    public static string GetTopTransactionCount => $"{BaseGameURL}/top-tx-count";
    public static string GetAppleUnwritten => $"{BaseGameURL}/get-apple-unwritten";
    public static string GetAppleWrittenBase => $"{BaseGameURL}/get-apple-written";
    public static string GetSwordCollectedBase => $"{BaseGameURL}/get-sword-collected";
    public static string GetTransactionCountBase => $"{BaseGameURL}/get-tx-count";

    // Auth API
    public static string Login => $"{BaseGameURL}/login";
    public static string GetUserByMnemonic => $"{BaseGameURL}/get-user-by-mnemonic";
    public static string Signup => $"{BaseGameURL}/insert";

    // Nếu cần thêm endpoint, chỉ cần viết thêm:
    public static string GetPlayerData(string playerId) => $"{BaseGameURL}/player/{playerId}";
    }
