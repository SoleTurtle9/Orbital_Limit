using UnityEngine;

public static class VfxEffects
{
    private static Material particleMaterial;

    public static void SpawnAsteroidExplosion(Vector3 position)
    {
        GameObject root = new GameObject("AsteroidExplosionVFX");
        root.transform.position = position;

        CreateParticleBurst(
            root.transform,
            "Rock Sparks",
            new Color(1f, 0.72f, 0.22f, 1f),
            new Color(1f, 0.22f, 0.05f, 0f),
            22,
            0.42f,
            4.8f,
            0.08f,
            0.18f
        );

        CreateParticleBurst(
            root.transform,
            "Dust Ring",
            new Color(0.7f, 0.55f, 0.42f, 0.8f),
            new Color(0.18f, 0.18f, 0.22f, 0f),
            14,
            0.55f,
            2.2f,
            0.18f,
            0.36f
        );

        Object.Destroy(root, 1.2f);
    }

    public static void SpawnBonusPickup(Vector3 position)
    {
        GameObject root = new GameObject("BonusPickupVFX");
        root.transform.position = position;

        CreateParticleBurst(
            root.transform,
            "Pickup Sparkles",
            new Color(0.25f, 1f, 0.9f, 1f),
            new Color(0.95f, 1f, 0.35f, 0f),
            18,
            0.48f,
            3.2f,
            0.07f,
            0.16f
        );

        CreateParticleBurst(
            root.transform,
            "Soft Glow",
            new Color(0.55f, 0.95f, 1f, 0.65f),
            new Color(0.2f, 0.55f, 1f, 0f),
            8,
            0.36f,
            1.2f,
            0.28f,
            0.5f
        );

        Object.Destroy(root, 1f);
    }

    private static void CreateParticleBurst(
        Transform parent,
        string name,
        Color startColor,
        Color endColor,
        short burstCount,
        float lifetime,
        float speed,
        float minSize,
        float maxSize
    )
    {
        GameObject effect = new GameObject(name);
        effect.transform.SetParent(parent);
        effect.transform.localPosition = Vector3.zero;

        ParticleSystem particles = effect.AddComponent<ParticleSystem>();
        ParticleSystemRenderer renderer = effect.GetComponent<ParticleSystemRenderer>();

        particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        renderer.sortingOrder = 20;
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.sharedMaterial = GetParticleMaterial();

        ParticleSystem.MainModule main = particles.main;
        main.loop = false;
        main.playOnAwake = false;
        main.duration = 0.12f;
        main.startLifetime = new ParticleSystem.MinMaxCurve(lifetime * 0.75f, lifetime);
        main.startSpeed = new ParticleSystem.MinMaxCurve(speed * 0.55f, speed);
        main.startSize = new ParticleSystem.MinMaxCurve(minSize, maxSize);
        main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 80;

        ParticleSystem.EmissionModule emission = particles.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[]
        {
            new ParticleSystem.Burst(0f, burstCount)
        });

        ParticleSystem.ShapeModule shape = particles.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.2f;
        shape.arc = 360f;

        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particles.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(startColor, 0f),
                new GradientColorKey(endColor, 1f)
            },
            new[]
            {
                new GradientAlphaKey(startColor.a, 0f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

        ParticleSystem.SizeOverLifetimeModule sizeOverLifetime = particles.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(
            1f,
            new AnimationCurve(
                new Keyframe(0f, 0.7f),
                new Keyframe(0.25f, 1.15f),
                new Keyframe(1f, 0f)
            )
        );

        particles.Play();
    }

    private static Material GetParticleMaterial()
    {
        if (particleMaterial != null)
            return particleMaterial;

        Shader shader =
            Shader.Find("Universal Render Pipeline/Particles/Unlit") ??
            Shader.Find("Particles/Standard Unlit") ??
            Shader.Find("Sprites/Default");

        particleMaterial = new Material(shader)
        {
            name = "Runtime Particle Material"
        };

        return particleMaterial;
    }
}
