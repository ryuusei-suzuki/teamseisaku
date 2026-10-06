using UnityEngine;

// 属性(AttributeType)ごとの表示色をまとめるユーティリティ
// スキル名ラベルの色分けと、攻撃エフェクトの色分けの両方から使う
public static class AttributeColorUtility
{
    // 炎:赤 水:青 風:緑 それ以外(無属性):白
    public static Color GetColor(AttributeType attribute)
    {
        switch (attribute)
        {
            case AttributeType.Fire:
                return Color.red;
            case AttributeType.Water:
                return Color.blue;
            case AttributeType.Wind:
                return Color.green;
            default:
                return Color.white;
        }
    }

    // 生成したエフェクト(とその子オブジェクト)に含まれる全てのParticleSystemの色を
    // 指定した属性の色に変更する(元の透明度(アルファ)は維持する)
    public static void ApplyAttributeColor(GameObject effectInstance, AttributeType attribute)
    {
        if (effectInstance == null) return;

        Color tint = GetColor(attribute);
        ParticleSystem[] particleSystems = effectInstance.GetComponentsInChildren<ParticleSystem>(true);

        foreach (ParticleSystem particle in particleSystems)
        {
            ParticleSystem.MainModule main = particle.main;

            float alpha = 1f;
            if (main.startColor.mode == ParticleSystemGradientMode.Color)
            {
                alpha = main.startColor.color.a;
            }

            main.startColor = new Color(tint.r, tint.g, tint.b, alpha);
        }
    }
}
