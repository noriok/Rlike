public static class Config {
    public const float ChipSize = 40.0f / 100.0f; // チップ画像サイズ/PixelsPerUnit

    // プレイヤーとカメラの Y 座標のオフセット
    public const float CameraOffsetY = -0.5f;

    public const float MinimapOffsetX = -1.3f;
    public const float MinimapOffsetY = 0.9f;

    // フロアに配置可能な敵の最大数
    public const int EnemyMaxCount = 25;

    // TODO:デバッグでスピード調整できるように
    public const float WalkDuration = 0.27f;

    public const float ThrowItemSpeed = 4.5f;
}
