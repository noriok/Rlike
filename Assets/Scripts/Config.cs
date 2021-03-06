﻿public static class Config {
    public const float ChipSize = 40.0f / 100.0f; // チップ画像サイズ/PixelsPerUnit

    // プレイヤーとカメラの Y 座標のオフセット
    public const float CameraOffsetY = -0.5f;

    public const float MinimapOffsetX = -1.3f;
    public const float MinimapOffsetY = 0.9f;

    // フロアに配置可能な敵の最大数
    public const int EnemyMaxCount = 25;

    // TODO:デバッグでスピード調整できるように
    public const float WalkDuration = 0.27f;

    public const float ItemThrowSpeed = 4.5f;
    public const float ItemThrowDiagonalSpeed = ItemThrowSpeed  * 1.2f;

    public const float MoveSpeed = 4.5f;
    public const float DiagonalMoveSpeed = MoveSpeed * 1.2f;

    public const float SpotlightAlpha = 0.55f;
}
