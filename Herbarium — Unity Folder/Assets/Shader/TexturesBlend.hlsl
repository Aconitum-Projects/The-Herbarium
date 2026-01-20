void TexturesBlend_float(
float3 Base,
float3 SecondColor,     // couleur unie
float Mask,             // mask 0-1
float Strength,
float Mode,               // 0 = Normal, 1 = Multiply, 2 = Add
out float3 Out
)
{
    // Normal / Blend
    float3 blendResult = lerp(Base, SecondColor, Mask);

    // Multiply masqué
    float3 multiplyResult = lerp(Base, Base * SecondColor, Mask);

    // Add masqué
    float3 addResult = lerp(Base, Base + SecondColor, Mask);

    // Sélection du mode
    float3 result = blendResult;
    if (Mode == 1)
        result = multiplyResult;
    else if (Mode == 2)
        result = addResult;

    // Strength globale
    Out = lerp(Base, result, saturate(Strength));
}
