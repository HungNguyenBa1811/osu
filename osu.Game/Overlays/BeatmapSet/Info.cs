﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Online.Chat;
using osu.Game.Screens.Select;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Overlays.BeatmapSet
{
    public class Info : Container
    {
        private const float transition_duration = 250;
        private const float metadata_width = 225;
        private const float spacing = 20;

        private readonly Box successRateBackground;
        private readonly SuccessRate successRate;

        public readonly Bindable<BeatmapSetInfo> BeatmapSet = new Bindable<BeatmapSetInfo>();

        public BeatmapInfo Beatmap
        {
            get => successRate.Beatmap;
            set => successRate.Beatmap = value;
        }

        public Info()
        {
            MetadataSection source, tags, genre, language;
            RelativeSizeAxes = Axes.X;
            Height = 220;
            Masking = true;
            EdgeEffect = new EdgeEffectParameters
            {
                Colour = Color4.Black.Opacity(0.25f),
                Type = EdgeEffectType.Shadow,
                Radius = 3,
                Offset = new Vector2(0f, 1f),
            };

            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.White,
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Top = 15, Horizontal = BeatmapSetOverlay.X_PADDING },
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Right = metadata_width + BeatmapSetOverlay.RIGHT_WIDTH + spacing * 2 },
                            Child = new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Child = new MetadataSection(MetadataType.Description),
                            },
                        },
                        new Container
                        {
                            Anchor = Anchor.TopRight,
                            Origin = Anchor.TopRight,
                            RelativeSizeAxes = Axes.Y,
                            Width = metadata_width,
                            Padding = new MarginPadding { Horizontal = 10 },
                            Margin = new MarginPadding { Right = BeatmapSetOverlay.RIGHT_WIDTH + spacing },
                            Child = new FillFlowContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Direction = FillDirection.Full,
                                Children = new[]
                                {
                                    source = new MetadataSection(MetadataType.Source),
                                    genre = new MetadataSection(MetadataType.Genre) { Width = 0.5f },
                                    language = new MetadataSection(MetadataType.Language) { Width = 0.5f },
                                    tags = new MetadataSection(MetadataType.Tags),
                                },
                            },
                        },
                        new Container
                        {
                            Anchor = Anchor.TopRight,
                            Origin = Anchor.TopRight,
                            RelativeSizeAxes = Axes.Y,
                            Width = BeatmapSetOverlay.RIGHT_WIDTH,
                            Children = new Drawable[]
                            {
                                successRateBackground = new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                },
                                successRate = new SuccessRate
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Padding = new MarginPadding { Top = 20, Horizontal = 15 },
                                },
                            },
                        },
                    },
                },
            };

            BeatmapSet.ValueChanged += b =>
            {
                source.Text = b.NewValue?.Metadata.Source ?? string.Empty;
                tags.Text = b.NewValue?.Metadata.Tags ?? string.Empty;
                genre.Text = b.NewValue?.OnlineInfo?.Genre?.Name ?? string.Empty;
                language.Text = b.NewValue?.OnlineInfo?.Language?.Name ?? string.Empty;
            };
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            successRateBackground.Colour = colours.GrayE;
        }

        private class MetadataSection : FillFlowContainer
        {
            private readonly MetadataType type;
            private readonly OsuSpriteText header;
            private readonly LinkFlowContainer textFlow;

            public string Text
            {
                set
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        Hide();
                        return;
                    }

                    this.FadeIn(transition_duration);

                    textFlow.Clear();
                    static void format(SpriteText t) => t.Font = t.Font.With(size: 14);

                    switch(type)
                    {
                        case MetadataType.Tags:
                            string[] tags = value.Split(" ");
                            for (int i = 0; i <= tags.Length - 1; i++)
                            {
                                textFlow.AddLink(tags[i], LinkAction.SearchBeatmapSet, tags[i], null, format);

                                if (i != tags.Length - 1)
                                    textFlow.AddText(" ", format);
                            }
                            break;

                        case MetadataType.Source:
                            textFlow.AddLink(value, LinkAction.SearchBeatmapSet, value, null, format);
                            break;

                        default:
                            textFlow.AddText(value, format);
                            break;
                    }
                }
            }

            public MetadataSection(MetadataType type)
            {
                this.type = type;

                RelativeSizeAxes = Axes.X;
                AutoSizeAxes = Axes.Y;
                Spacing = new Vector2(5f);

                InternalChildren = new Drawable[]
                {
                    header = new OsuSpriteText
                    {
                        Text = this.type.ToString(),
                        Font = OsuFont.GetFont(size: 14, weight: FontWeight.Bold),
                        Shadow = false,
                        Margin = new MarginPadding { Top = 20 },
                    },
                    textFlow = new LinkFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                    },
                };
            }

            [BackgroundDependencyLoader]
            private void load(OsuColour colours)
            {
                header.Colour = textFlow.Colour = colours.Gray5;
            }
        }
    }
}
