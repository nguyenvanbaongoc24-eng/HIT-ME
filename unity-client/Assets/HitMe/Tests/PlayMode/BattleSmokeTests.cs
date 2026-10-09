using System.Collections;
using System.IO;
using System.Reflection;
using HitMe.Core;
using HitMe.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace HitMe.Tests
{
    public sealed class BattleSmokeTests
    {
        [UnityTest]
        public IEnumerator RimPlacementKeepsHudAndPlayerPresentationSeparate()
        {
            SceneManager.LoadScene("Battle"); yield return null; yield return null;
            int[,] sizes = { {360, 800}, {390, 844}, {412, 915}, {430, 932} };
            foreach (int i in new[] { 0, 1, 2, 3 })
            {
                SetGameViewSize(sizes[i, 0], sizes[i, 1]); yield return null; yield return null;
                var view = Object.FindAnyObjectByType<BattleView>();
                view.SafeAreaOverride = new Rect(0, 34, Screen.width, Screen.height - 78); yield return null; yield return null;
                view.StartPlacement();
                foreach (Point position in new[] { new Point(0, 1660), new Point(0, -1660), new Point(910, 0), new Point(-910, 0) })
                {
                    Point p = view.Viewport.WorldToScreen(position);
                    view.Place(new Vector2((float)(p.X * Screen.width / 390.0), (float)(Screen.height - p.Y * Screen.width / 390.0)));
                    yield return null;
                    Transform actor = GameObject.Find("ActorFeet0").transform;
                    foreach (string part in new[] { "CharacterSpritePlaceholder", "NameHealth", "FeetHitbox" })
                    {
                        Rect bounds = ScreenBounds(actor.Find(part).GetComponent<RectTransform>());
                        Assert.That(bounds.xMin, Is.GreaterThanOrEqualTo(-1), part);
                        Assert.That(bounds.xMax, Is.LessThanOrEqualTo(Screen.width + 1), part);
                        foreach (string widget in new[] { "Round", "Timer", "Ready", "Chat", "Weapon", "Settings", "Status", "PortraitPlaceholder0", "PortraitPlaceholder1", "PortraitPlaceholder2" })
                            Assert.IsFalse(bounds.Overlaps(ScreenBounds(GameObject.Find(widget).GetComponent<RectTransform>())), part + " overlaps " + widget);
                    }
                    Assert.IsTrue(Geometry.ValidPlacement(view.Session.Position, 1000, 1750, 90));
                }
            }
            LogAssert.NoUnexpectedReceived();
        }
        static Rect ScreenBounds(RectTransform r)
        {
            var corners = new Vector3[4]; r.GetWorldCorners(corners);
            Vector2 min = RectTransformUtility.WorldToScreenPoint(null, corners[0]);
            Vector2 max = RectTransformUtility.WorldToScreenPoint(null, corners[2]);
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }
        [UnityTest]
        public IEnumerator PlacementPointerInputLocksOnlyPlayerAndRevealsAtDeadline()
        {
            SceneManager.LoadScene("Battle"); yield return null; yield return null;
            SetGameViewSize(390, 844); yield return null; yield return null;
            var view = Object.FindAnyObjectByType<BattleView>();
            GameObject.Find("Ready").GetComponent<Button>().onClick.Invoke();
            Assert.AreEqual(0, view.VisibleActorCount);
            var floor = GameObject.Find("ArenaSandPlaceholder");
            var e = new PointerEventData(EventSystem.current) { pointerId = 0, position = new Vector2(195, 350) };
            ExecuteEvents.Execute(floor, e, ExecuteEvents.pointerDownHandler);
            e.position = new Vector2(265, 380); ExecuteEvents.Execute(floor, e, ExecuteEvents.dragHandler);
            ExecuteEvents.Execute(floor, e, ExecuteEvents.pointerUpHandler);
            Assert.IsTrue(view.Session.HasPosition); Assert.IsTrue(view.Session.HasAim);
            Assert.IsTrue(Geometry.ValidPlacement(view.Session.Position, 1000, 1750, 90));
            yield return null;
            Assert.AreEqual(1, view.VisibleActorCount);
            GameObject.Find("Ready").GetComponent<Button>().onClick.Invoke();
            Assert.IsTrue(view.Session.Locked); Assert.AreEqual(BattlePhase.Placement, view.Session.Phase);
            Assert.IsNull(GameObject.Find("ActorFeet1")); Assert.IsNull(GameObject.Find("ActorFeet2"));
            var screenshot = ScreenCapture.CaptureScreenshotAsTexture();
            string output = Path.GetFullPath(Path.Combine(Application.dataPath, "../../docs/screenshots")); Directory.CreateDirectory(output);
            File.WriteAllBytes(Path.Combine(output, "Battle-placement-390x844.png"), screenshot.EncodeToPNG()); Object.Destroy(screenshot);
            yield return new WaitForSecondsRealtime(5.4f);
            Assert.AreEqual(BattlePhase.AwaitingRules, view.Session.Phase);
            Assert.AreEqual(3, view.VisibleActorCount);
            Assert.AreEqual("♥♥♥", GameObject.Find("Hearts").GetComponent<Text>().text, "No inferred damage resolution.");
            LogAssert.NoUnexpectedReceived();
        }
        [UnityTest]
        public IEnumerator BattleLoadsHidesOpponentsAndCapturesFourSizes()
        {
            SceneManager.LoadScene("Battle"); yield return null; yield return null;
            BattleView view = Object.FindAnyObjectByType<BattleView>(); Assert.IsNotNull(view);
            Assert.AreEqual(3, view.VisibleActorCount);
            int[,] sizes = { {360, 800}, {390, 844}, {412, 915}, {430, 932} };
            for (int i = 0; i < sizes.GetLength(0); i++)
            {
                SetGameViewSize(sizes[i, 0], sizes[i, 1]);
                yield return null; yield return null; yield return null;
                Assert.AreEqual(sizes[i, 0], Screen.width, "Game View width");
                Assert.AreEqual(sizes[i, 1], Screen.height, "Game View height");
                RectTransform safe = GameObject.Find("SafeAreaHUD").GetComponent<RectTransform>();
                foreach (string name in new[] { "Ready", "Chat", "Weapon", "Settings", "Round" })
                {
                    RectTransform r = GameObject.Find(name).GetComponent<RectTransform>();
                    Vector3[] corners = new Vector3[4]; r.GetWorldCorners(corners);
                    foreach (Vector3 corner in corners)
                    {
                        Vector2 p = RectTransformUtility.WorldToScreenPoint(null, corner);
                        Assert.That(p.x, Is.InRange(Screen.safeArea.xMin - 1, Screen.safeArea.xMax + 1), name);
                        Assert.That(p.y, Is.InRange(Screen.safeArea.yMin - 1, Screen.safeArea.yMax + 1), name);
                    }
                }
                foreach (Text t in Object.FindObjectsByType<Text>())
                    Assert.LessOrEqual(t.preferredHeight, t.rectTransform.rect.height + 2, "Clipped text: " + t.name + " / " + t.text);
                string output = Path.GetFullPath(Path.Combine(Application.dataPath, "../../docs/screenshots")); Directory.CreateDirectory(output);
                // WaitForEndOfFrame is not supported in Editor batchmode; capture the last rendered Game View frame.
                yield return null;
                Texture2D capture = ScreenCapture.CaptureScreenshotAsTexture();
                // The GPU can still be presenting a blank frame immediately after a fixed-size switch.
                // Wait for a real rendered frame, with a bounded timeout; do not manufacture image pixels.
                for (int attempt = 0; attempt < 20; attempt++)
                {
                    Color pixel = capture.GetPixel(Screen.width / 2, Screen.height / 2);
                    if (pixel.r > .85f && pixel.g > .4f) break;
                    Object.Destroy(capture);
                    yield return new WaitForSecondsRealtime(.1f);
                    capture = ScreenCapture.CaptureScreenshotAsTexture();
                }
                Assert.IsNotNull(capture); Assert.AreEqual(Screen.width, capture.width); Assert.AreEqual(Screen.height, capture.height);
                Color sand = capture.GetPixel(Screen.width / 2, Screen.height / 2);
                Assert.That(sand.r, Is.GreaterThan(.85f), "Arena mesh must actually render, not just exist in hierarchy.");
                Assert.That(sand.g, Is.GreaterThan(.4f), "Visible terracotta floor at arena center.");
                File.WriteAllBytes(Path.Combine(output, "Battle-" + Screen.width + "x" + Screen.height + ".png"), capture.EncodeToPNG());
                Object.Destroy(capture);
            }
            view.StartPlacement(); yield return null;
            Assert.AreEqual(BattlePhase.Placement, view.Session.Phase);
            Assert.AreEqual(0, view.VisibleActorCount);
            Assert.IsFalse(GameObject.Find("ActorFeet1") != null);
            Assert.IsFalse(GameObject.Find("ActorFeet2") != null);
            view.SafeAreaOverride = new Rect(0, 34, Screen.width, Screen.height - 78);
            yield return null; yield return null;
            foreach (string name in new[] { "Ready", "Chat", "Weapon", "Settings", "Round" })
            {
                var r = GameObject.Find(name).GetComponent<RectTransform>();
                Vector3[] corners = new Vector3[4]; r.GetWorldCorners(corners);
                foreach (Vector3 corner in corners)
                {
                    Vector2 p = RectTransformUtility.WorldToScreenPoint(null, corner);
                    Assert.That(p.y, Is.InRange(33, Screen.height - 43), "Simulated notch: " + name);
                }
            }
            Assert.AreEqual(0, view.VisibleActorCount, "Resize must not reveal opponents.");
            LogAssert.NoUnexpectedReceived();
        }
        static void SetGameViewSize(int width, int height)
        {
#if UNITY_EDITOR
            // Editor-only test utility; reflection is restricted to the internal Game View sizing API.
            var assembly = typeof(UnityEditor.EditorWindow).Assembly;
            var sizesType = assembly.GetType("UnityEditor.GameViewSizes");
            var singleton = typeof(UnityEditor.ScriptableSingleton<>).MakeGenericType(sizesType);
            var instance = singleton.GetProperty("instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);
            var groupType = sizesType.GetProperty("currentGroupType").GetValue(instance);
            var group = sizesType.GetMethod("GetGroup").Invoke(instance, new[] { groupType });
            var sizeType = assembly.GetType("UnityEditor.GameViewSize");
            var fixedType = System.Enum.Parse(assembly.GetType("UnityEditor.GameViewSizeType"), "FixedResolution");
            var size = System.Activator.CreateInstance(sizeType, new object[] { fixedType, width, height, "HIT ME Test" });
            group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { size });
            int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null) - 1;
            var gameView = UnityEditor.EditorWindow.GetWindow(assembly.GetType("UnityEditor.GameView"));
            gameView.GetType().GetProperty("selectedSizeIndex", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(gameView, index);
            gameView.Repaint();
#else
            Screen.SetResolution(width, height, false);
#endif
        }
    }
}
