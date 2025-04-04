using Silk.NET.Core.Native;
using Silk.NET.Input;
using Silk.NET.GLFW;
using Silk.NET.Windowing;
using Silk.NET.Direct2D;
using Silk.NET.Direct3D11;
using Silk.NET.Windowing.Glfw;
using Silk.NET.Input.Glfw;
using Silk.NET.DXGI;
using Silk.NET.Direct3D.Compilers;
using Silk.NET.Maths;

namespace ATR.Host
{
    internal class Program
    {

        private static IWindow _window;
        private static DXGI _dxgi;
        private static D3D11 _d3d11;
        private static D2D _d2d;
        private static D3DCompiler _compiler;
        private static ComPtr<IDXGIFactory2> _factory = default;
        private static ComPtr<IDXGISwapChain1> _swapChain = default;
        private static ComPtr<ID3D11Device> _device = default;
        private static ComPtr<ID3D11DeviceContext> _context = default;
        private static ComPtr<ID2D1Factory1> _d2dFactory = default;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            GlfwWindowing.RegisterPlatform();
            GlfwInput.RegisterPlatform();

            WindowOptions options = WindowOptions.Default with
            {
                Size = new Silk.NET.Maths.Vector2D<int>(1024,1024),
                Title = "GSY Simulations ATR",
                API = GraphicsAPI.None
            };

            _window = Window.Create(options);

            _window.Load += OnLoad;
            _window.Update += OnUpdate;
            _window.Render += OnRender;
            _window.Closing += OnClose;
            _window.FramebufferResize += OnFramebufferResize;

            _window.Run();

            _factory.Dispose();
            _swapChain.Dispose();
            _device.Dispose();
            _context.Dispose();
            _d2dFactory.Dispose();
            _d3d11.Dispose();
            _d2d.Dispose();
            _compiler.Dispose();
            _dxgi.Dispose();
            _window.Dispose();

        }

        private static void OnLoad()
        {
            const bool forceDxvk = false;
            _dxgi = DXGI.GetApi(_window, forceDxvk);
            _d3d11 = D3D11.GetApi(_window, forceDxvk);
            _compiler = D3DCompiler.GetApi();

            var input = _window.CreateInput();
            foreach(var keyboard in input.Keyboards)
            {
                keyboard.KeyDown += KeyDown;
            }



            _window.Center();



        }

        private static void OnUpdate(double deltaTime)
        {

        }

        private static void OnRender(double deltaTime)
        {

        }

        private static void KeyDown(IKeyboard keyboard, Key key, int keyCode)
        {
            if (key == Key.Escape)
            {
                _window.Close();
            }
        }

        private static void OnFramebufferResize(Vector2D<int> newSize)
        {
            SilkMarshal.ThrowHResult(
                _swapChain.ResizeBuffers(0, (uint)newSize.X, (uint)newSize.Y, Format.FormatB8G8R8A8Unorm, 0)
                );
        }

        static void OnClose()
        {

        }
    }
}
