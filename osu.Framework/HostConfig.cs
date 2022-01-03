// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Framework
{
    /// <summary>
    /// Various configuration properties for a <see cref="Host"/>.
    /// </summary>
    public struct HostConfig
    {
        /// <summary>
        /// Name of the game or application.
        /// </summary>
        public string GameName { get; set; }

        /// <summary>
        /// Whether to bind the IPC port.
        /// </summary>
        public bool BindIPC { get; set; }

        /// <summary>
        /// Whether this is a portable installation.
        /// </summary>
        public bool PortableInstallation { get; set; }

        /// <summary>
        /// Whether to bypass the compositor.
        /// </summary>
        /// <remarks>
        /// On Linux, the compositor re-buffers the application to apply various window effects,
        /// increasing latency in the process. Thus it is a good idea for games to bypass it,
        /// though normal applications would generally benefit from letting the window effects untouched. <br/>
        /// If the SDL_VIDEO_X11_NET_WM_BYPASS_COMPOSITOR environment variable is set, this property will have no effect.
        /// </remarks>
        public bool BypassCompositor { get; set; }

        public static HostConfig GameConfig(string gameName, bool bindIPC = false, bool portableInstallation = false)
        {
            return new HostConfig
            {
                GameName = gameName,
                BindIPC = bindIPC,
                PortableInstallation = portableInstallation,
                BypassCompositor = true,
            };
        }

        public static HostConfig ApplicationConfig(string gameName, bool bindIPC = false, bool portableInstallation = false)
        {
            return new HostConfig
            {
                GameName = gameName,
                BindIPC = bindIPC,
                PortableInstallation = portableInstallation,
                BypassCompositor = false,
            };
        }
    }
}
