using System.Runtime.InteropServices;

unsafe partial class Libc {
    [DllImport("libc", EntryPoint = "printf", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    static extern int raw_printf(string format);

    [DllImport("libc", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void* malloc(int size);

    [DllImport("libc", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void* realloc(void* ptr, int size);

    [DllImport("libc", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void free(void* ptr);

	public static int printf(string format, params object[] args) {
		return raw_printf(String.Format(format, args));
	}
}

unsafe partial class RayLib {
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public unsafe struct Color {
		public byte r;
		public byte g;
		public byte b;
		public byte a;
	}

    [DllImport("libraylib", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	public static extern void InitWindow(int width, int height, String title);

    [DllImport("libraylib", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	public static extern void SetTargetFPS(int fps);

    [DllImport("libraylib", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	public static extern bool WindowShouldClose();

    [DllImport("libraylib", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	public static extern void BeginDrawing();

    [DllImport("libraylib", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	public static extern void ClearBackground(Color color);
	
    [DllImport("libraylib", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	public static extern void DrawText(String text, int posX, int posY, int fontSize, Color color);

    [DllImport("libraylib", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	public static extern void EndDrawing();

    [DllImport("libraylib", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	public static extern void CloseWindow();
}

unsafe struct Vector<T> {
	//
	// Can be joinable but for extensibility reason i split `capacity` and `cursor`
	//
	int capacity;
	public int cursor;
	T* ptr;

	public void New() {
		cursor = 0;
		capacity = 1;

		this.ptr = (T*) Libc.malloc(capacity * sizeof(T));
	}

	public void Push(T element) {
		if (ptr == null) {
			return;
		}

		T* new_ptr = null;

		if (cursor + 1 == capacity) {
			capacity++;
			new_ptr = (T*) Libc.realloc(this.ptr, capacity * sizeof(T));
		} else {
			new_ptr = this.ptr;
		}

		new_ptr += cursor;
		*new_ptr = element;

		cursor++;
	}

	public T? Get(int index) {
		if (index > this.capacity || this.ptr == null) {
			return default(T);
		}
		T* ptr_clone = ptr;
		ptr_clone += index;

		return *ptr_clone;
	}

	public void Pop() {
		capacity--;
		T* new_ptr = (T*) Libc.realloc(this.ptr, capacity * sizeof(T));

		cursor--;
	}

	public void Free() {
		Libc.free(this.ptr);
		this.ptr = null;
	}
}

[System.Runtime.CompilerServices.SkipLocalsInit]
unsafe class Program {
	public static void Main() {
		RayLib.InitWindow(800, 450, "Hello RayLib");
		RayLib.SetTargetFPS(60);

		while (!RayLib.WindowShouldClose()) {
			RayLib.BeginDrawing();

			RayLib.ClearBackground(
				new RayLib.Color {
					r = 255,
					g = 255,
					b = 255,
					a = 255,
				}
			);

            RayLib.DrawText("Congrats! You created your first window!", 190, 200, 20, new RayLib.Color { r = 200, g = 200, b = 200, a = 255} );

			RayLib.EndDrawing();
		}

		RayLib.CloseWindow();
	}
}
