using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

namespace SoulboundEngine.Client.Input {
	public static partial class InputTokens {
		public static class Keyboard {
			public static InputToken ESC, DELETE;
			public static InputToken F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12;
			public static InputToken keypad_1, keypad_2, keypad_3, keypad_4, keypad_5, keypad_6, keypad_7, keypad_8, keypad_9, keypad_0;
			public static InputToken BACK_QUOTE, MINUS, EQUALS, BACKSPACE;
			public static InputToken Q, W, E, R, T, Y, U, I, O, P,
									  A, S, D, F, G, H, J, K, L,
									   Z, X, C, V, B, N, M;
			public static InputToken TAB, BRACKET_LEFT, BRACKET_RIGHT, BACKSLASH;
			public static InputToken CAPS_LOCK, SEMICOLON, QUOTE, ENTER;
			public static InputToken SHIFT, COMMA, PERIOD, SLASH, RIGHT_SHIFT;
			public static InputToken CTRL, ALT, SPACE, RIGHT_ALT;
			public static InputToken ARROW_LEFT, ARROW_RIGHT, ARROW_UP, ARROW_DOWN;
			public static InputToken HOME, END, PAGE_UP, PAGE_DOWN, PRINTSCREEN;
			public static InputToken ANY;

			public static void Register(InputActionAsset asset) {
				ESC = Create(asset, "Keyboard/ESC");
				DELETE = Create(asset, "Keyboard/DELETE");
				F1 = Create(asset, "Keyboard/F1");
				F2 = Create(asset, "Keyboard/F2");
				F3 = Create(asset, "Keyboard/F3");
				F4 = Create(asset, "Keyboard/F4");
				F5 = Create(asset, "Keyboard/F5");
				F6 = Create(asset, "Keyboard/F6");
				F7 = Create(asset, "Keyboard/F7");
				F8 = Create(asset, "Keyboard/F8");
				F9 = Create(asset, "Keyboard/F9");
				F10 = Create(asset, "Keyboard/F10");
				F11 = Create(asset, "Keyboard/F11");
				F12 = Create(asset, "Keyboard/F12");
				keypad_0 = Create(asset, "Keyboard/keypad_0");
				keypad_1 = Create(asset, "Keyboard/keypad_1");
				keypad_2 = Create(asset, "Keyboard/keypad_2");
				keypad_3 = Create(asset, "Keyboard/keypad_3");
				keypad_4 = Create(asset, "Keyboard/keypad_4");
				keypad_5 = Create(asset, "Keyboard/keypad_5");
				keypad_6 = Create(asset, "Keyboard/keypad_6");
				keypad_7 = Create(asset, "Keyboard/keypad_7");
				keypad_8 = Create(asset, "Keyboard/keypad_8");
				keypad_9 = Create(asset, "Keyboard/keypad_9");
				BACK_QUOTE = Create(asset, "Keyboard/BACK_QUOTE");
				MINUS = Create(asset, "Keyboard/MINUS");
				EQUALS = Create(asset, "Keyboard/EQUALS");
				BACKSPACE = Create(asset, "Keyboard/BACKSPACE");
				Q = Create(asset, "Keyboard/Q");
				W = Create(asset, "Keyboard/W");
				E = Create(asset, "Keyboard/E");
				R = Create(asset, "Keyboard/R");
				T = Create(asset, "Keyboard/T");
				Y = Create(asset, "Keyboard/Y");
				U = Create(asset, "Keyboard/U");
				I = Create(asset, "Keyboard/I");
				O = Create(asset, "Keyboard/O");
				P = Create(asset, "Keyboard/P");
				A = Create(asset, "Keyboard/A");
				S = Create(asset, "Keyboard/S");
				D = Create(asset, "Keyboard/D");
				F = Create(asset, "Keyboard/F");
				G = Create(asset, "Keyboard/G");
				H = Create(asset, "Keyboard/H");
				J = Create(asset, "Keyboard/J");
				K = Create(asset, "Keyboard/K");
				L = Create(asset, "Keyboard/L");
				Z = Create(asset, "Keyboard/Z");
				X = Create(asset, "Keyboard/X");
				C = Create(asset, "Keyboard/C");
				V = Create(asset, "Keyboard/V");
				B = Create(asset, "Keyboard/B");
				N = Create(asset, "Keyboard/N");
				M = Create(asset, "Keyboard/M");
				TAB = Create(asset, "Keyboard/TAB");
				BRACKET_LEFT = Create(asset, "Keyboard/BRACKET_LEFT");
				BRACKET_RIGHT = Create(asset, "Keyboard/BRACKET_RIGHT");
				BACKSLASH = Create(asset, "Keyboard/BACKSLASH");
				CAPS_LOCK = Create(asset, "Keyboard/CAPS_LOCK");
				SEMICOLON = Create(asset, "Keyboard/SEMICOLON");
				QUOTE = Create(asset, "Keyboard/QUOTE");
				ENTER = Create(asset, "Keyboard/ENTER");
				SHIFT = Create(asset, "Keyboard/SHIFT");
				COMMA = Create(asset, "Keyboard/COMMA");
				PERIOD = Create(asset, "Keyboard/PERIOD");
				SLASH = Create(asset, "Keyboard/SLASH");
				RIGHT_SHIFT = Create(asset, "Keyboard/RIGHT_SHIFT");
				CTRL = Create(asset, "Keyboard/CTRL");
				ALT = Create(asset, "Keyboard/ALT");
				RIGHT_ALT = Create(asset, "Keyboard/RIGHT_ALT");
				ARROW_LEFT = Create(asset, "Keyboard/ARROW_LEFT");
				ARROW_RIGHT = Create(asset, "Keyboard/ARROW_RIGHT");
				ARROW_UP = Create(asset, "Keyboard/ARROW_UP");
				ARROW_DOWN = Create(asset, "Keyboard/ARROW_DOWN");
				HOME = Create(asset, "Keyboard/HOME");
				END = Create(asset, "Keyboard/END");
				PAGE_UP = Create(asset, "Keyboard/PAGE_UP");
				PAGE_DOWN = Create(asset, "Keyboard/PAGE_DOWN");
				PRINTSCREEN = Create(asset, "Keyboard/PRINTSCREEN");
				ANY = Create(asset, "Keyboard/ANY");
			}
		}
	}
}
