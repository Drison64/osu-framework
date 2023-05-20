// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Platform;

namespace osu.Framework.Tests.Visual.Platform
{
    public partial class TestSceneWindowFlash : FrameworkTestScene
    {
        private IBindable<bool> isActive = null!;
        private IWindow? window;
        private SpriteText text = null!;
        private TextFlowContainer behaviourText = null!;
        private readonly Bindable<bool> flashUntilFocused = new BindableBool();

        [BackgroundDependencyLoader]
        private void load(GameHost gameHost)
        {
            isActive = gameHost.IsActive.GetBoundCopy();
            window = gameHost.Window;
            Child = new FillFlowContainer
            {
                Direction = FillDirection.Vertical,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new Drawable[]
                {
                    text = new SpriteText
                    {
                        Text = "This window will flash as soon as you unfocus it.",
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    },
                    behaviourText = new TextFlowContainer
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        AutoSizeAxes = Axes.Both
                    }
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            AddToggleStep("Flash until focused", a => flashUntilFocused.Value = a);
            flashUntilFocused.BindValueChanged(e =>
            {
                window?.CancelFlash();
                text.Text = "This window will flash " + (e.NewValue ? "until focused again" : "briefly")
                                                      + " as soon as it is unfocused.";
            }, true);
            isActive.BindValueChanged(e =>
            {
                if (!e.NewValue)
                    window?.Flash(flashUntilFocused.Value);
            }, true);
            behaviourText.AddParagraph("Behaviour is platform dependent (only for desktops):");
            behaviourText.AddParagraph("- Windows: icon flashes on the taskbar and raises it. Only once if briefly.");
            behaviourText.AddParagraph("- MacOS: icon jumps on the Dock (can be seen even hidden). Only once if briefly.");
            behaviourText.AddParagraph("- Linux: depends on DE/WM setup.");
        }
    }
}
