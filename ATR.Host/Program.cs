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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Runtime.CompilerServices;
using System.Drawing;

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

        private static float[] _backgroundColor = [0.392f, 0.584f, 0.929f, 1.0f];

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

            _window = Silk.NET.Windowing.Window.Create(options);

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

        private static unsafe void OnLoad()
        {
            _window.Center();

            const bool forceDxvk = false;
            _dxgi = DXGI.GetApi(_window, forceDxvk);
            _d3d11 = D3D11.GetApi(_window, forceDxvk);
            _compiler = D3DCompiler.GetApi();

            var input = _window.CreateInput();
            foreach(var keyboard in input.Keyboards)
            {
                keyboard.KeyDown += KeyDown;
            }

            SilkMarshal.ThrowHResult
            (
                _d3d11.CreateDevice
                (
                    default(ComPtr<IDXGIAdapter>),
                    D3DDriverType.Hardware,
                    Software: default,
                    (uint)CreateDeviceFlag.Debug,
                    null,
                    0,
                    D3D11.SdkVersion,
                    ref _device,
                    null, 
                    ref _context
                    )
            );

            if (OperatingSystem.IsWindows())
            {
                #if DEBUG
                _device.SetInfoQueueCallback(msg => Console.WriteLine(SilkMarshal.PtrToString((nint)msg.PDescription)));
                #endif
            }

            // Create our swapchain.
            var swapChainDesc = new SwapChainDesc1
            {
                BufferCount = 2, // double buffered
                Format = Format.FormatB8G8R8A8Unorm,
                BufferUsage = DXGI.UsageRenderTargetOutput,
                SwapEffect = SwapEffect.FlipDiscard,
                SampleDesc = new SampleDesc(1, 0)
            };

            _factory = _dxgi.CreateDXGIFactory<IDXGIFactory2>();

            // Create the swapchain.
            SilkMarshal.ThrowHResult
            (
                _factory.CreateSwapChainForHwnd
                (
                    _device,
                    _window.Native!.DXHandle!.Value,
                    in swapChainDesc,
                    null,
                    ref Unsafe.NullRef<IDXGIOutput>(),
                    ref _swapChain
                )
            );
        }

        private static void OnUpdate(double deltaTime)
        {

        }

        private static unsafe void OnRender(double deltaTime)
        {
            using var framebuffer = _swapChain.GetBuffer<ID3D11Texture2D>(0);

            // Create a view over the render target.
            ComPtr<ID3D11RenderTargetView> renderTargetView = default;
            SilkMarshal.ThrowHResult(_device.CreateRenderTargetView(framebuffer, null, ref renderTargetView));

            // Clear the render target to be all black ahead of rendering.
            _context.ClearRenderTargetView(renderTargetView, ref _backgroundColor[0]);

            // Update the rasterizer state with the current viewport.
            var viewport = new Viewport(0, 0, _window.FramebufferSize.X, _window.FramebufferSize.Y, 0, 1);
            _context.RSSetViewports(1, in viewport);

            // Tell the output merger about our render target view.
            _context.OMSetRenderTargets(1, ref renderTargetView, ref Unsafe.NullRef<ID3D11DepthStencilView>());

            // Present the drawn image.
            _swapChain.Present(1, 0);

            // Clean up any resources created in this method.
            renderTargetView.Dispose();
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
