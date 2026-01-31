using NSubstitute;
using NSubstitute.ReceivedExtensions;
using NUnit.Framework;
using SoulboundBackend.Client.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Screen = SoulboundBackend.Client.UI.Screens.Screen;

static class ScreenTestUtils {
	public static (Screen screen, IScreenObject obj) CreateScreen(Transform root) {
		Screen screen = Substitute.For<Screen>();
		IScreenObject obj = Substitute.For<IScreenObject>();
		obj.GetInstance().Returns(screen);
		screen.BuildObject(root).Returns(obj);
		return (screen, obj);
	}
}

public class ScreenManagerTests {
	[Test]
	public void PushScreen_HidesPreviousScreen() {
		Transform root = new GameObject("root").transform;
		var (screenA, objA) = ScreenTestUtils.CreateScreen(root);
		var (screenB, objB) = ScreenTestUtils.CreateScreen(root);
		ScreenManager manager = new(root);

		manager.PushScreen(screenA);
		manager.PushScreen(screenB);

		objA.Received(1).Hide();
		objB.Received(1).Show();
	}

	[Test]
	public void PushScreen_ShowsNewScreen() {
		Transform root = new GameObject("root").transform;
		ScreenManager manager = new(root);

		var (screen, obj) = ScreenTestUtils.CreateScreen(root);
		manager.PushScreen(screen);

		obj.Received(1).Show();
		obj.DidNotReceive().Hide();
	}

	[Test]
	public void PopScreen_DisposesTopScreen() {
		Transform root = new GameObject("root").transform;
		ScreenManager manager = new(root);

		var (screen, obj) = ScreenTestUtils.CreateScreen(root);
		manager.PushScreen(screen);

		manager.PopScreen();

		obj.Received(1).Hide();
		obj.Received(1).Dispose();
	}

	[Test]
	public void PopScreen_ShowsPreviousScreen() {
		Transform root = new GameObject("root").transform;
		ScreenManager manager = new(root);

		var (screenA, objA) = ScreenTestUtils.CreateScreen(root);
		var (screenB, _) = ScreenTestUtils.CreateScreen(root);

		manager.PushScreen(screenA);
		manager.PushScreen(screenB);

		manager.PopScreen();

		objA.Received(2).Show();
	}

	[Test]
	public void PopScreen_ReturnsFalse_WhenEmpty() {
		Transform root = new GameObject("root").transform;
		ScreenManager manager = new(root);

		bool popped = manager.PopScreen();
		Assert.IsFalse(popped);
	}

	[Test]
	public void ReplaceScreen_DisposesOldAndShowsNew() {
		Transform root = new GameObject("root").transform;
		ScreenManager manager = new(root);

		var (screenA, objA) = ScreenTestUtils.CreateScreen(root);
		var (screenB, objB) = ScreenTestUtils.CreateScreen(root);

		manager.PushScreen(screenA);
		manager.ReplaceScreen(screenB);

		objA.Received(1).Dispose();
		objB.Received(1).Show();
	}

	[Test]
	public void GetActiveScreen_ReturnsTopScreen() {
		Transform root = new GameObject("root").transform;
		ScreenManager manager = new(root);

		var (screenA, _) = ScreenTestUtils.CreateScreen(root);
		var (screenB, _) = ScreenTestUtils.CreateScreen(root);

		manager.PushScreen(screenA);
		manager.PushScreen(screenB);

		Assert.AreSame(screenB, manager.GetActiveScreen());
	}

	[Test]
	public void GetActiveScreen_ReturnsNull_WhenEmpty() {
		Transform root = new GameObject("root").transform;
		ScreenManager manager = new(root);

		Assert.IsNull(manager.GetActiveScreen());
	}

	[Test]
	public void Flush_DisposesAllScreens() {
		Transform root = new GameObject("root").transform;
		ScreenManager manager = new(root);

		var (screenA, objA) = ScreenTestUtils.CreateScreen(root);
		var (screenB, objB) = ScreenTestUtils.CreateScreen(root);

		manager.PushScreen(screenA);
		manager.PushScreen(screenB);

		manager.Flush();

		objA.Received(1).Dispose();
		objB.Received(1).Dispose();
	}

	[Test]
	public void Flush_LeavesManagerEmpty() {
		Transform root = new GameObject("root").transform;
		ScreenManager manager = new ScreenManager(root);

		manager.Flush();

		Assert.IsNull(manager.GetActiveScreen());
	}

}
