float2 fade(float2 t) {
    return t * t * t * (t * (t * 6 - 15) + 10);
}

float2 hash(float2 p) {
    // ランダムなグラデーションベクトル
    float h = frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
    return normalize(float2(cos(h * 6.28318), sin(h * 6.28318)));
}

float perlinNoise2D(float2 p) {
    float2 i = floor(p);
    float2 f = frac(p);

    // 4つの角のグラデーションベクトル
    float2 g00 = hash(i + float2(0.0, 0.0));
    float2 g10 = hash(i + float2(1.0, 0.0));
    float2 g01 = hash(i + float2(0.0, 1.0));
    float2 g11 = hash(i + float2(1.0, 1.0));

    // それぞれの方向ベクトル
    float2 d00 = f - float2(0.0, 0.0);
    float2 d10 = f - float2(1.0, 0.0);
    float2 d01 = f - float2(0.0, 1.0);
    float2 d11 = f - float2(1.0, 1.0);

    // 各ドット積
    float v00 = dot(g00, d00);
    float v10 = dot(g10, d10);
    float v01 = dot(g01, d01);
    float v11 = dot(g11, d11);

    // 補間
    float2 u = fade(f);
    float x1 = lerp(v00, v10, u.x);
    float x2 = lerp(v01, v11, u.x);
    float value = lerp(x1, x2, u.y);

    // [-1, 1] → [0, 1] に変換
    return 0.5 + 0.5 * value;
}