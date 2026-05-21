using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SkyboxFader : MonoBehaviour
{
    [Header("Directional Light")]
    public Light directionalLight;

    [Header("Light A")]
    public Color lightColorA = Color.white;
    public float lightIntensityA = 1f;

    [Header("Light B")]
    public Color lightColorB = new Color(1f, 0.5f, 0f); // orange par exemple
    public float lightIntensityB = 0.5f;
    
    [Header("Fog A")]
    public Color fogColorA = Color.gray;
    public float fogDensityA = 0.01f;

    [Header("Fog B")]
    public Color fogColorB = Color.blue;
    public float fogDensityB = 0.03f;
    
    [Header("Ambient Gradient A")]
    public Color skyColorA = new Color(0.2f, 0.2f, 0.3f);
    public Color equatorColorA = new Color(0.1f, 0.1f, 0.1f);
    public Color groundColorA = new Color(0.05f, 0.05f, 0.05f);

    [Header("Ambient Gradient B")]
    public Color skyColorB = new Color(0.4f, 0.3f, 0.1f);
    public Color equatorColorB = new Color(0.2f, 0.15f, 0.05f);
    public Color groundColorB = new Color(0.1f, 0.08f, 0.02f);
    
    [Header("Material avec shader Custom/SkyboxBlend")]
    public Material skyboxBlendMaterial;
    public float fadeDuration = 2f;
    public string playerTag = "Player";

    private bool isFading = false;
    private bool isOnB = false;

    void Start()
    {
        GetComponent<Collider>().isTrigger = true;
        RenderSettings.skybox = skyboxBlendMaterial;
        skyboxBlendMaterial.SetFloat("_Blend", 0f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag) || isFading) return;
        StartCoroutine(FadeCoroutine(isOnB ? 0f : 1f));
    }

    IEnumerator FadeCoroutine(float target)
{
    isFading = true;
    float start = skyboxBlendMaterial.GetFloat("_Blend");

    // Ambient
    Color startSky     = RenderSettings.ambientSkyColor;
    Color startEquator = RenderSettings.ambientEquatorColor;
    Color startGround  = RenderSettings.ambientGroundColor;

    Color targetSky     = isOnB ? skyColorA     : skyColorB;
    Color targetEquator = isOnB ? equatorColorA : equatorColorB;
    Color targetGround  = isOnB ? groundColorA  : groundColorB;

    // Fog
    Color startFogColor   = RenderSettings.fogColor;
    float startFogDensity = RenderSettings.fogDensity;

    Color targetFogColor   = isOnB ? fogColorA   : fogColorB;
    float targetFogDensity = isOnB ? fogDensityA : fogDensityB;

    //Ground
    Color startLightColor     = directionalLight.color;
    float startLightIntensity = directionalLight.intensity;

    Color targetLightColor     = isOnB ? lightColorA : lightColorB;
    float targetLightIntensity = isOnB ? lightIntensityA : lightIntensityB;
    
    float elapsed = 0f;

    while (elapsed < fadeDuration)
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / fadeDuration);
        float smooth = t * t * (3f - 2f * t);

        skyboxBlendMaterial.SetFloat("_Blend", Mathf.Lerp(start, target, smooth));

        // Ambient gradient
        RenderSettings.ambientSkyColor     = Color.Lerp(startSky,     targetSky,     smooth);
        RenderSettings.ambientEquatorColor = Color.Lerp(startEquator, targetEquator, smooth);
        RenderSettings.ambientGroundColor  = Color.Lerp(startGround,  targetGround,  smooth);

        // Fog
        RenderSettings.fogColor   = Color.Lerp(startFogColor,   targetFogColor,   smooth);
        RenderSettings.fogDensity = Mathf.Lerp(startFogDensity, targetFogDensity, smooth);

        //Ground
        directionalLight.color     = Color.Lerp(startLightColor, targetLightColor, smooth);
        directionalLight.intensity = Mathf.Lerp(startLightIntensity, targetLightIntensity, smooth);

        DynamicGI.UpdateEnvironment();
        yield return null;
    }

    skyboxBlendMaterial.SetFloat("_Blend", target);
    RenderSettings.ambientSkyColor     = targetSky;
    RenderSettings.ambientEquatorColor = targetEquator;
    RenderSettings.ambientGroundColor  = targetGround;
    RenderSettings.fogColor            = targetFogColor;
    RenderSettings.fogDensity          = targetFogDensity;
    directionalLight.color     = targetLightColor;
    directionalLight.intensity = targetLightIntensity;

    isOnB = !isOnB;
    isFading = false;
}
}