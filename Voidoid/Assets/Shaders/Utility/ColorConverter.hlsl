float3 HSVtoRGB(float3 hsv) {
    float h = hsv.x;  // 色相（0〜1）
    float s = hsv.y;  // 彩度（0〜1）
    float v = hsv.z;  // 明度（0〜1）

    h = frac(h) * 6.0;  // 色相を 0〜6 にスケーリング
    float c = v * s;    // 色の強さ（彩度）
    float x = c * (1 - abs(fmod(h, 2.0) - 1.0));
    float3 rgb;

    if (0.0 <= h && h < 1.0) {
        rgb = float3(c, x, 0);
    } else if (1.0 <= h && h < 2.0) {
        rgb = float3(x, c, 0);
    } else if (2.0 <= h && h < 3.0) {
        rgb = float3(0, c, x);
    } else if (3.0 <= h && h < 4.0) {
        rgb = float3(0, x, c);
    } else if (4.0 <= h && h < 5.0) {
        rgb = float3(x, 0, c);
    } else { // 5.0 <= h < 6.0
        rgb = float3(c, 0, x);
    }

    float m = v - c;
    return rgb + m;
}