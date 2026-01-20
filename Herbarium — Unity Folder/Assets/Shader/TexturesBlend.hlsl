#ifndef TEXTURES_BLEND_INCLUDED
#define TEXTURES_BLEND_INCLUDED

void TexturesBlend_float(
    float3 Base,
    float3 SecondTextureColor,
    float Strength,
    int Mode,           // 0 = Normal, 1 = Multiply, 2 = Add
    out float3 Out
)
{
    // Mask implicite : noir = 0, couleur = 1
    float mask = max(max(SecondTextureColor.r, SecondTextureColor.g), SecondTextureColor.b);

    // Normal / Blend
    float3 blendResult = Base * (1.0 - mask) + SecondTextureColor;

    // Multiply masqué
    float3 multiplyResult = lerp(Base, Base * SecondTextureColor, mask);

    // Add masqué
    float3 addResult = lerp(Base, Base + SecondTextureColor, mask);

    // Sélection du mode
    float3 result = blendResult;

    if (Mode == 1)
        result = multiplyResult;
    else if (Mode == 2)
        result = addResult;

    // Strength globale
    Out = lerp(Base, result, saturate(Strength));
}

#endif
