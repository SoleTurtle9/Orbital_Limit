using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace OrbitalLimit.Tests.PlayMode
{
    // Reflect Assembly-CSharp so adding tests does not move or rename runtime assemblies.
    public class GameplayRegressionTests
    {
        private const BindingFlags Members = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private Component player;
        private Component score;
        private Component manager;
        private Component hud;
        private Component asteroidSpawner;
        private Component bonusSpawner;
        private int originalHighScore;
        private bool hadHighScore;
        private bool hadMusicVolume;
        private float originalMusicVolume;
        private readonly List<string> fontWarnings = new List<string>();

        [OneTimeSetUp]
        public void SavePreferences()
        {
            hadHighScore = PlayerPrefs.HasKey("HighScore");
            originalHighScore = PlayerPrefs.GetInt("HighScore", 0);
            hadMusicVolume = PlayerPrefs.HasKey("MusicVolume");
            originalMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            Application.logMessageReceived += CaptureFontWarnings;
        }

        [UnitySetUp]
        public IEnumerator LoadGame()
        {
            Time.timeScale = 1f;
            fontWarnings.Clear();
            yield return SceneManager.LoadSceneAsync("Game");
            yield return null;
            player = Find("PlayerController");
            score = Find("ScoreManager");
            manager = Find("GameManager");
            hud = Find("HUDController");
            asteroidSpawner = Find("AsteroidSpawner");
            bonusSpawner = Find("BonusSpawner");
            ((Behaviour)asteroidSpawner).enabled = false;
            ((Behaviour)bonusSpawner).enabled = false;
            ((Behaviour)player).enabled = false;
            player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            foreach (AudioSource source in Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
                source.mute = true;
        }

        [UnityTearDown]
        public IEnumerator ResetTime()
        {
            Time.timeScale = 1f;
            Assert.That(fontWarnings, Is.Empty, "UI must not request missing characters or ellipsis glyphs.");
            yield return null;
        }

        [OneTimeTearDown]
        public void RestorePreferences()
        {
            Application.logMessageReceived -= CaptureFontWarnings;
            if (hadHighScore) PlayerPrefs.SetInt("HighScore", originalHighScore);
            else PlayerPrefs.DeleteKey("HighScore");
            if (hadMusicVolume) PlayerPrefs.SetFloat("MusicVolume", originalMusicVolume);
            else PlayerPrefs.DeleteKey("MusicVolume");
            PlayerPrefs.Save();
            Time.timeScale = 1f;
        }

        [UnityTest]
        public IEnumerator SolidLaserControl_ReproducesOriginalPushback()
        {
            Rigidbody2D body = player.GetComponent<Rigidbody2D>();
            Vector2 before = body.position;
            Transform point = Field<Transform>(player, "firePoint");
            GameObject laser = Object.Instantiate(Field<GameObject>(player, "laserPrefab"), point.position, point.rotation);
            ((Behaviour)laser.GetComponent(RuntimeType("Bullet"))).enabled = false;
            foreach (Collider2D collider in laser.GetComponentsInChildren<Collider2D>()) collider.isTrigger = false;
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            Vector2 displacement = body.position - before;
            Debug.Log("Solid laser control displacement: " + displacement);
            Assert.That(displacement.magnitude, Is.GreaterThan(0.01f), "Positive control must reproduce physical displacement from the old solid collider.");
        }

        [UnityTest]
        public IEnumerator ContinuousShooting_DoesNotPushShip()
        {
            Rigidbody2D body = player.GetComponent<Rigidbody2D>();
            Vector2 before = body.position;
            for (int i = 0; i < 60; i++)
            {
                Invoke(player, "Shoot");
                yield return new WaitForFixedUpdate();
            }
            Assert.That(Vector2.Distance(body.position, before), Is.LessThan(0.001f));
            Assert.That(body.linearVelocity.sqrMagnitude, Is.LessThan(0.000001f));
        }

        [UnityTest]
        public IEnumerator Shot_UsesTriggerAndIgnoresBothShipColliders()
        {
            Invoke(player, "Shoot");
            yield return null;
            Component bullet = Find("Bullet");
            foreach (Collider2D laserCollider in bullet.GetComponentsInChildren<Collider2D>())
            {
                Assert.That(laserCollider.isTrigger, Is.True);
                foreach (Collider2D shipCollider in player.GetComponentsInChildren<Collider2D>())
                    Assert.That(Physics2D.GetIgnoreCollision(laserCollider, shipCollider), Is.True);
            }
            Assert.That(player.GetComponents<Collider2D>().Length, Is.EqualTo(2));
        }

        [UnityTest]
        public IEnumerator HealthPickup_TwoShipColliders_HealsOnce()
        {
            Invoke(player, "TakeDamage", 60);
            GameObject pickup = SpawnPickup("healthPickupPrefab");
            yield return new WaitForFixedUpdate();
            yield return null;
            Assert.That(Property<int>(player, "CurrentHealth"), Is.EqualTo(65));
            Assert.That(pickup == null, Is.True);
        }

        [UnityTest]
        public IEnumerator FullHealthPickup_RemainsAvailableAndClampsHealing()
        {
            GameObject pickup = SpawnPickup("healthPickupPrefab");
            yield return new WaitForFixedUpdate();
            yield return null;
            Assert.That(pickup != null, Is.True);
            pickup.transform.position += Vector3.right * 4f;
            yield return new WaitForFixedUpdate();
            Invoke(player, "TakeDamage", 10);
            pickup.transform.position = PickupPosition();
            yield return new WaitForFixedUpdate();
            yield return null;
            Assert.That(Property<int>(player, "CurrentHealth"), Is.EqualTo(100));
            Assert.That(pickup == null, Is.True);
            Assert.That(Text(Field<Component>(hud, "notificationText")), Does.Contain("+10 HP"));
        }

        [UnityTest]
        public IEnumerator FireRatePickup_ActivatesOnceAndExpiresAtSixSeconds()
        {
            GameObject pickup = SpawnPickup("fireRatePickupPrefab");
            float before = Time.time;
            yield return new WaitForFixedUpdate();
            yield return null;
            Assert.That(pickup == null, Is.True);
            Assert.That(Field<float>(player, "currentShootInterval"), Is.EqualTo(0.12f).Within(0.0001f));
            float endsAt = Field<float>(player, "fireRateBoostEndTime");
            Assert.That(endsAt, Is.InRange(before + 6f, Time.time + 6f));
            Assert.That(Object.FindObjectsByType<Transform>(FindObjectsSortMode.None).Count(t => t.name == "BonusPickupVFX"), Is.EqualTo(1));
            yield return new WaitForSeconds(6.1f);
            Invoke(player, "UpdateFireRateBoost");
            Assert.That(Field<float>(player, "currentShootInterval"), Is.EqualTo(0.35f).Within(0.0001f));
        }

        [UnityTest]
        public IEnumerator Asteroid_TwoShipColliders_InflictsOneDamageEvent()
        {
            GameObject asteroid = SpawnHeavy(PickupPosition());
            yield return new WaitForFixedUpdate();
            yield return null;
            Assert.That(Property<int>(player, "CurrentHealth"), Is.EqualTo(65));
            Assert.That(asteroid == null, Is.True);
            Assert.That((int)Invoke(score, "GetScore"), Is.Zero);
        }

        [UnityTest]
        public IEnumerator MultipleBullets_OneHeavyAsteroid_AwardsThreePointsOnce()
        {
            GameObject asteroid = SpawnHeavy(new Vector3(4f, 0f, 0f));
            for (int i = 0; i < 5; i++) SpawnLaser(asteroid.transform.position);
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return null;
            Assert.That(asteroid == null, Is.True);
            Assert.That((int)Invoke(score, "GetScore"), Is.EqualTo(3));
        }

        [UnityTest]
        public IEnumerator OneBullet_OverlappingAsteroids_DamagesOneTarget()
        {
            GameObject first = SpawnHeavy(new Vector3(4f, 0f, 0f));
            GameObject second = SpawnHeavy(new Vector3(4.2f, 0f, 0f));
            SpawnLaser(new Vector3(4.1f, 0f, 0f));
            yield return new WaitForFixedUpdate();
            yield return null;
            int remaining = Field<int>(first.GetComponent(RuntimeType("Asteroid")), "currentHealth") +
                Field<int>(second.GetComponent(RuntimeType("Asteroid")), "currentHealth");
            Assert.That(remaining, Is.EqualTo(5));
            Assert.That((int)Invoke(score, "GetScore"), Is.Zero);
        }

        [UnityTest]
        public IEnumerator Pause_BlocksShootingAndResumeRestoresIt()
        {
            Component pause = Find("PauseMenuController");
            Invoke(pause, "PauseGame");
            Invoke(player, "Shoot");
            yield return null;
            Assert.That(Time.timeScale, Is.Zero);
            Assert.That(FindAll("Bullet").Length, Is.Zero);
            Invoke(pause, "ResumeGame");
            Invoke(player, "Shoot");
            yield return null;
            Assert.That(Time.timeScale, Is.EqualTo(1f));
            Assert.That(FindAll("Bullet").Length, Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator GameOver_HidesAllHudAndBlocksShooting()
        {
            Invoke(hud, "ShowBonusNotification", "Проверка уведомления");
            yield return null;
            Invoke(player, "TakeDamage", 100);
            Invoke(player, "Shoot");
            Invoke(manager, "GameOver");
            yield return null;
            RectTransform root = Field<RectTransform>(hud, "hudRoot");
            Assert.That(root.gameObject.activeInHierarchy, Is.False);
            foreach (CanvasRenderer renderer in root.GetComponentsInChildren<CanvasRenderer>(true))
                Assert.That(renderer.gameObject.activeInHierarchy, Is.False);
            GameObject panel = Field<GameObject>(manager, "gameOverPanel");
            Assert.That(panel.activeInHierarchy, Is.True);
            Assert.That(panel.transform.IsChildOf(root), Is.False);
            Assert.That(FindAll("Bullet").Length, Is.Zero);
            Assert.That(Property<int>(player, "CurrentHealth"), Is.Zero);
            Assert.That(Time.timeScale, Is.Zero);
        }

        [UnityTest]
        public IEnumerator Restart_AfterGameOver_ResetsHealthScoreWaveAndHud()
        {
            Invoke(score, "AddScore", 12);
            Invoke(player, "TakeDamage", 100);
            Assert.That(PlayerPrefs.GetInt("HighScore"), Is.GreaterThanOrEqualTo(12));
            Invoke(manager, "RestartGame");
            yield return null;
            yield return null;
            Component restartedPlayer = Find("PlayerController");
            Component restartedManager = Find("GameManager");
            Assert.That(Property<int>(restartedPlayer, "CurrentHealth"), Is.EqualTo(100));
            Assert.That((int)Invoke(Find("ScoreManager"), "GetScore"), Is.Zero);
            Assert.That(Property<int>(Find("AsteroidSpawner"), "CurrentWave"), Is.EqualTo(1));
            Assert.That(Property<bool>(restartedManager, "IsGameOver"), Is.False);
            Assert.That(Field<RectTransform>(Find("HUDController"), "hudRoot").gameObject.activeInHierarchy, Is.True);
            Assert.That(Time.timeScale, Is.EqualTo(1f));
        }

        [UnityTest]
        public IEnumerator HealthBar_TracksActualHealthWithoutFontOrSpriteDependency()
        {
            Invoke(player, "TakeDamage", 50);
            yield return null;
            Canvas.ForceUpdateCanvases();
            Component fill = Field<Component>(hud, "healthFill");
            RectTransform rect = fill.GetComponent<RectTransform>();
            Assert.That(rect.anchorMax.x, Is.EqualTo(0.5f).Within(0.0001f));
            Assert.That(rect.rect.width, Is.EqualTo(((RectTransform)rect.parent).rect.width * 0.5f).Within(0.1f));
            Assert.That(Text(Field<Component>(hud, "healthValueText")), Does.Contain("50 / 100"));
        }

        [TestCase(1, 2f)]
        [TestCase(3, 1.8f)]
        [TestCase(5, 1.6f)]
        [TestCase(12, 0.9f)]
        [TestCase(100, 0.9f)]
        public void WaveSpawnInterval_PreservesBalance(int wave, float expected)
        {
            SetField(asteroidSpawner, "currentWave", wave);
            Assert.That((float)Invoke(asteroidSpawner, "GetCurrentSpawnInterval"), Is.EqualTo(expected).Within(0.0001f));
            Assert.That(Field<float>(asteroidSpawner, "waveDuration"), Is.EqualTo(20f));
        }

        [TestCase(1, false, false)]
        [TestCase(3, true, false)]
        [TestCase(5, true, true)]
        public void AsteroidTypes_UnlockAtConfiguredWaves(int wave, bool allowFast, bool allowHeavy)
        {
            SetField(asteroidSpawner, "currentWave", wave);
            GameObject fast = Field<GameObject>(asteroidSpawner, "fastAsteroidPrefab");
            GameObject heavy = Field<GameObject>(asteroidSpawner, "heavyAsteroidPrefab");
            var selected = new HashSet<GameObject>();
            UnityEngine.Random.InitState(12345);
            for (int i = 0; i < 128; i++) selected.Add((GameObject)Invoke(asteroidSpawner, "ChooseAsteroidPrefab"));
            Assert.That(selected.Contains(fast), Is.EqualTo(allowFast));
            Assert.That(selected.Contains(heavy), Is.EqualTo(allowHeavy));
        }

        [Test]
        public void BonusPrefabsAndAudioClips_AreAssigned()
        {
            Assert.That(Field<GameObject>(bonusSpawner, "healthPickupPrefab"), Is.Not.Null);
            Assert.That(Field<GameObject>(bonusSpawner, "fireRatePickupPrefab"), Is.Not.Null);
            Assert.That(Field<float>(bonusSpawner, "minSpawnDelay"), Is.EqualTo(10f));
            Assert.That(Field<float>(bonusSpawner, "maxSpawnDelay"), Is.EqualTo(16f));
            Component audio = Find("AudioManager");
            Assert.That(Field<AudioSource>(audio, "audioSource"), Is.Not.Null);
            foreach (string clip in new[] { "laserSound", "hitSound", "gameOverSound", "pickupSound" })
                Assert.That(Field<AudioClip>(audio, clip), Is.Not.Null);
        }

        [Test]
        public void InterfaceFonts_RenderRussianAndLatinInBothWeights()
        {
            TMP_FontAsset regular = Resources.Load<TMP_FontAsset>("Fonts/OrbitalUI");
            TMP_FontAsset bold = Resources.Load<TMP_FontAsset>("Fonts/OrbitalUI-Bold");
            Assert.That(regular, Is.Not.Null);
            Assert.That(bold, Is.Not.Null);
            Assert.That(regular.atlasPopulationMode, Is.EqualTo(AtlasPopulationMode.Static));
            Assert.That(bold.atlasPopulationMode, Is.EqualTo(AtlasPopulationMode.Static));
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789" +
                "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя…№—–/:%>";
            foreach (char value in alphabet)
            {
                bool alternative;
                var normalGlyph = TMP_FontAssetUtilities.GetCharacterFromFontAsset(value, regular, false,
                    FontStyles.Normal, FontWeight.Regular, out alternative);
                var boldGlyph = TMP_FontAssetUtilities.GetCharacterFromFontAsset(value, regular, false,
                    FontStyles.Bold, FontWeight.Bold, out alternative);
                Assert.That(normalGlyph, Is.Not.Null, "Regular: " + value);
                Assert.That(boldGlyph, Is.Not.Null, "Bold: " + value);
                Assert.That(normalGlyph.textAsset, Is.EqualTo(regular));
                Assert.That(boldGlyph.textAsset, Is.EqualTo(bold));
            }

            TMP_Text[] labels = Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            Assert.That(labels.Length, Is.GreaterThan(15));
            foreach (TMP_Text label in labels)
            {
                Assert.That(label.font, Is.EqualTo(regular), label.name);
                label.ForceMeshUpdate(true, true);
                for (int i = 0; i < label.textInfo.characterCount; i++)
                {
                    TMP_CharacterInfo glyph = label.textInfo.characterInfo[i];
                    if (!glyph.isVisible) continue;
                    Assert.That(glyph.fontAsset == regular || glyph.fontAsset == bold, Is.True, label.name);
                }
            }
        }

        [UnityTest]
        public IEnumerator GameMusic_PauseSettingsFreezePlaybackAndResumeAtSamePosition()
        {
            AudioSource music = Field<AudioSource>(Find("AudioManager"), "musicSource");
            Assert.That(music.clip.name, Is.EqualTo("space"));
            Assert.That(music.loop, Is.True);
            yield return new WaitForSecondsRealtime(0.2f);
            Assert.That(music.isPlaying, Is.True);
            Assert.That(music.timeSamples, Is.GreaterThan(0));

            Component pause = Find("PauseMenuController");
            Invoke(pause, "PauseGame");
            int pausedSample = music.timeSamples;
            yield return new WaitForSecondsRealtime(0.15f);
            Assert.That(music.isPlaying, Is.False);
            Assert.That(music.timeSamples, Is.EqualTo(pausedSample));
            GameObject pauseCard = Field<GameObject>(pause, "pauseCard");
            pauseCard.transform.Find("SettingsButton").GetComponent<Button>().onClick.Invoke();
            GameObject settings = Field<GameObject>(pause, "settingsCard");
            Assert.That(settings.activeInHierarchy, Is.True);
            Slider slider = settings.GetComponentsInChildren<Slider>().Single(s => s.transform.parent.name == "МУЗЫКАRow");
            slider.value = 0.42f;
            Assert.That(music.volume, Is.EqualTo(0.42f).Within(0.001f));
            Assert.That(music.isPlaying, Is.False);
            settings.transform.Find("BackButton").GetComponent<Button>().onClick.Invoke();
            Assert.That(pauseCard.activeInHierarchy, Is.True);
            pauseCard.transform.Find("ResumeButton").GetComponent<Button>().onClick.Invoke();
            yield return new WaitForSecondsRealtime(0.15f);
            Assert.That(Time.timeScale, Is.EqualTo(1f));
            Assert.That(music.isPlaying, Is.True);
            Assert.That(music.timeSamples, Is.GreaterThan(pausedSample));
        }

        [UnityTest]
        public IEnumerator MenuMusic_SettingsAndStartSwitchTracksWithoutOverlap()
        {
            PlayerPrefs.SetFloat("MusicVolume", 0.37f);
            yield return SceneManager.LoadSceneAsync("Main_menu");
            yield return null;
            AudioSource menuMusic = Field<AudioSource>(Find("AudioManager"), "musicSource");
            menuMusic.mute = true;
            Assert.That(menuMusic.clip.name, Is.EqualTo("Menu"));
            Assert.That(menuMusic.loop, Is.True);
            Assert.That(menuMusic.isPlaying, Is.True);
            Assert.That(menuMusic.volume, Is.EqualTo(0.37f).Within(0.001f));
            Component menu = Find("MainMenuController");
            GameObject menuCard = Field<GameObject>(menu, "menuCard");
            menuCard.transform.Find("SettingsButton").GetComponent<Button>().onClick.Invoke();
            GameObject settings = Field<GameObject>(menu, "settingsCard");
            Assert.That(settings.activeInHierarchy, Is.True);
            Slider slider = settings.GetComponentsInChildren<Slider>().Single(s => s.transform.parent.name == "МУЗЫКАRow");
            slider.value = 0.62f;
            Assert.That(menuMusic.volume, Is.EqualTo(0.62f).Within(0.001f));
            Assert.That(PlayerPrefs.GetFloat("MusicVolume"), Is.EqualTo(0.62f).Within(0.001f));
            settings.transform.Find("BackButton").GetComponent<Button>().onClick.Invoke();
            menuCard.transform.Find("StartButton").GetComponent<Button>().onClick.Invoke();
            yield return null;
            yield return null;
            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo("Game"));
            Assert.That(menuMusic == null, Is.True, "Menu music must be destroyed on scene transition.");
            AudioSource gameMusic = Field<AudioSource>(Find("AudioManager"), "musicSource");
            gameMusic.mute = true;
            Assert.That(gameMusic.clip.name, Is.EqualTo("space"));
            Assert.That(gameMusic.isPlaying, Is.True);
            Assert.That(gameMusic.volume, Is.EqualTo(0.62f).Within(0.001f));
            Assert.That(Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None)
                .Any(source => source.clip != null && source.clip.name == "Menu"), Is.False);
        }

        private GameObject SpawnPickup(string field)
        {
            GameObject pickup = Object.Instantiate(Field<GameObject>(bonusSpawner, field), PickupPosition(), Quaternion.identity);
            Component script = pickup.GetComponent(RuntimeType(field == "healthPickupPrefab" ? "HealthPickup" : "FireRatePickup"));
            SetField(script, "fallSpeed", 0f);
            SetField(script, "rotationSpeed", 0f);
            return pickup;
        }

        private Vector3 PickupPosition()
        {
            return player.transform.position + new Vector3(0f, -0.1f, 0f);
        }

        private GameObject SpawnHeavy(Vector3 position)
        {
            GameObject asteroid = Object.Instantiate(Field<GameObject>(asteroidSpawner, "heavyAsteroidPrefab"), position, Quaternion.identity);
            Component script = asteroid.GetComponent(RuntimeType("Asteroid"));
            SetField(script, "speed", 0f);
            SetField(script, "speedVariation", 0f);
            SetField(script, "speedIncreasePerScore", 0f);
            return asteroid;
        }

        private void SpawnLaser(Vector3 position)
        {
            Object.Instantiate(Field<GameObject>(player, "laserPrefab"), position, Quaternion.identity);
        }

        private void CaptureFontWarnings(string message, string stackTrace, LogType type)
        {
            if (type == LogType.Warning && (message.Contains("font asset") || message.Contains("Ellipsis") ||
                message.Contains("Unicode character"))) fontWarnings.Add(message);
        }

        private static Type RuntimeType(string name) { return Type.GetType(name + ", Assembly-CSharp", true); }
        private static Component[] FindAll(string name)
        {
            Type type = RuntimeType(name);
            return Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Where(component => component.GetType() == type).Cast<Component>().ToArray();
        }
        private static Component Find(string name)
        {
            Component[] matches = FindAll(name);
            Assert.That(matches.Length, Is.EqualTo(1), name + " must have one scene instance");
            return matches[0];
        }
        private static object Invoke(Component target, string method, params object[] args) { return target.GetType().GetMethod(method, Members).Invoke(target, args); }
        private static T Field<T>(Component target, string name) { return (T)target.GetType().GetField(name, Members).GetValue(target); }
        private static void SetField(Component target, string name, object value) { target.GetType().GetField(name, Members).SetValue(target, value); }
        private static T Property<T>(Component target, string name) { return (T)target.GetType().GetProperty(name, Members).GetValue(target); }
        private static string Text(Component target) { return Property<string>(target, "text"); }
    }
}
