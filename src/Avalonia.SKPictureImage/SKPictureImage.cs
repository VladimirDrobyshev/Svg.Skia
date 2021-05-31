﻿#nullable disable
using System;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Visuals.Media.Imaging;

namespace Avalonia.SKPictureImage
{
    /// <summary>
    /// An <see cref="IImage"/> that uses a <see cref="SkiaSharp.SKPicture"/> for content.
    /// </summary>
    public class SKPictureImage : AvaloniaObject, IImage, IAffectsRender
    {
        /// <summary>
        /// Defines the <see cref="Source"/> property.
        /// </summary>
        public static readonly StyledProperty<SkiaSharp.SKPicture> SourceProperty =
            AvaloniaProperty.Register<SKPictureImage, SkiaSharp.SKPicture>(nameof(Source));

        public event EventHandler Invalidated;

        /// <summary>
        /// Gets or sets the <see cref="SkiaSharp.SKPicture"/> content.
        /// </summary>
        [Content]
        public SkiaSharp.SKPicture Source
        {
            get => GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        /// <inheritdoc/>
        public Size Size => Source is { } ? new Size(Source.CullRect.Width, Source.CullRect.Height) : default;

        /// <inheritdoc/>
        void IImage.Draw(DrawingContext context, Rect sourceRect, Rect destRect, BitmapInterpolationMode bitmapInterpolationMode)
        {
            var source = Source;
            if (source is null)
            {
                return;
            }
            var bounds = source.CullRect;
            var scaleMatrix = Matrix.CreateScale(destRect.Width / sourceRect.Width, destRect.Height / sourceRect.Height);
            var translateMatrix = Matrix.CreateTranslation(-sourceRect.X + destRect.X - bounds.Top, -sourceRect.Y + destRect.Y - bounds.Left);
            using (context.PushClip(destRect))
            using (context.PushPreTransform(translateMatrix * scaleMatrix))
            {
                context.Custom(new SKPictureDrawOperation(new Rect(0, 0, bounds.Width, bounds.Height), source));
            }
        }

        /// <inheritdoc/>
        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);
            if (change.Property == SourceProperty)
            {
                RaiseInvalidated(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the <see cref="Invalidated"/> event.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected void RaiseInvalidated(EventArgs e) => Invalidated?.Invoke(this, e);
    }
}
