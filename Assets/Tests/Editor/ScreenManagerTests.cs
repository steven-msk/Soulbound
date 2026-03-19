using NSubstitute;
using NSubstitute.ReceivedExtensions;
using NUnit.Framework;
using SoulboundBackend.Client.UI.Screens;
using UnityEngine;
using Screen = SoulboundBackend.Client.UI.Screens.Screen;

static class ScreenTestUtils {
	public static (Screen screen, IScreenObject obj) CreateScreen(IScreenObjectFactory objectFactory) {
		Screen screen = Substitute.For<Screen>();
		IScreenObject obj = Substitute.For<IScreenObject>();
		obj.GetInstance().Returns(screen);
		screen.BuildObject(objectFactory).Returns(obj);
		return (screen, obj);
	}
}

public class ScreenManagerTests {
	private ScreenManager manager;

	private class FakeScreenRoot : IScreenRoot {
		public void AttachScreenObject(GameObject screenObject) {
		}
	}

	[SetUp]
	public void Setup() {
		manager = new ScreenManager(new FakeScreenRoot());
	}

	[Test]
	public void PushScreenMethod_HidesPreviousScreen() {
		var (screenA, objA) = ScreenTestUtils.CreateScreen(manager);
		var (screenB, objB) = ScreenTestUtils.CreateScreen(manager);

		manager.PushScreen(screenA);
		manager.PushScreen(screenB);

		objA.Received(1).Hide();
		objB.Received(1).Show();
	}

	[Test]
	public void PushScreenMethod_ShowsNewScreen() {
		var (screen, obj) = ScreenTestUtils.CreateScreen(manager);
		manager.PushScreen(screen);

		obj.Received(1).Show();
		obj.DidNotReceive().Hide();
	}

	[Test]
	public void PopScreenMethod_DisposesTopScreen() {
		var (screen, obj) = ScreenTestUtils.CreateScreen(manager);
		manager.PushScreen(screen);

		manager.PopScreen();

		obj.Received(1).Hide();
		obj.Received(1).Dispose();
	}

	[Test]
	public void PopScreenMethod_ShowsPreviousScreen() {
		var (screenA, objA) = ScreenTestUtils.CreateScreen(manager);
		var (screenB, _) = ScreenTestUtils.CreateScreen(manager);

		manager.PushScreen(screenA);
		manager.PushScreen(screenB);

		manager.PopScreen();

		objA.Received(2).Show();
	}

	[Test]
	public void PopScreenMethod_ReturnsFalse_WhenEmpty() {
		bool popped = manager.PopScreen();
		Assert.IsFalse(popped);
	}

	[Test]
	public void ReplaceScreenMethod_DisposesOldAndShowsNew() {
		var (screenA, objA) = ScreenTestUtils.CreateScreen(manager);
		var (screenB, objB) = ScreenTestUtils.CreateScreen(manager);

		manager.PushScreen(screenA);
		manager.ReplaceScreen(screenB);

		objA.Received(1).Dispose();
		objB.Received(1).Show();
	}

	[Test]
	public void GetActiveScreenMethod_ReturnsTopScreen() {
		var (screenA, _) = ScreenTestUtils.CreateScreen(manager);
		var (screenB, _) = ScreenTestUtils.CreateScreen(manager);

		manager.PushScreen(screenA);
		manager.PushScreen(screenB);

		Assert.That(manager.GetActiveScreen(), Is.SameAs(screenB));
	}

	[Test]
	public void GetActiveScreenMethod_ReturnsNull_WhenEmpty() {
		Assert.That(manager.GetActiveScreen(), Is.Null);
	}

	[Test]
	public void FlushMethod_DisposesAllScreens() {
		var (screenA, objA) = ScreenTestUtils.CreateScreen(manager);
		var (screenB, objB) = ScreenTestUtils.CreateScreen(manager);

		manager.PushScreen(screenA);
		manager.PushScreen(screenB);

		manager.Flush();

		objA.Received(1).Dispose();
		objB.Received(1).Dispose();
	}

	[Test]
	public void FlushMethod_LeavesManagerEmpty() {
		manager.Flush();

		Assert.That(manager.GetActiveScreen(), Is.Null);
	}

}
