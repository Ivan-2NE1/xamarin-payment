using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Java.IO;
using QuickDate.Helpers.Utils;
using Console = System.Console;
using Exception = System.Exception;
using Uri = Android.Net.Uri;




namespace QuickDate.Helpers.CacheLoaders
{
    public enum ImageStyle
    {
        CenterCrop, CircleCrop
    }

    public enum ImagePlaceholders
    {
        Color, Drawable
    }

    public static class GlideImageLoader
    {
        public static RequestOptions DefaultOptions, CircleOptions;

        public static void InitImageLoader()
        {
            try
            {
                SetImageOption(); 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void SetImageOption()
        {
            try
            {
                DefaultOptions = new RequestOptions().Apply(RequestOptions.CenterCropTransform()
                    .CenterCrop() 
                    .InvokePriority(Priority.High).Override(200)
                    .Error(Resource.Drawable.ImagePlacholder)
                    .Placeholder(Resource.Drawable.ImagePlacholder));

                CircleOptions = new RequestOptions().Apply(RequestOptions.CircleCropTransform()
                    .CircleCrop() 
                    .InvokePriority(Priority.High).Override(200)
                    .Error(Resource.Drawable.ImagePlacholder_circle)
                    .Placeholder(Resource.Drawable.ImagePlacholder_circle));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="imageUri"></param>
        /// <param name="image"></param>
        /// <param name="style"></param>
        /// <param name="imagePlaceholders"></param>
        public static void LoadImage(Activity activity, string imageUri, ImageView image, ImageStyle style, ImagePlaceholders imagePlaceholders)
        {
            try
            {
                var newImage = Glide.With(activity);

                switch (imagePlaceholders)
                {
                    case ImagePlaceholders.Color:
                        var color = IMethods.Fun_String.RandomColor();
                        switch (style)
                        {
                            case ImageStyle.CircleCrop:
                                CircleOptions.Placeholder(new ColorDrawable(Color.ParseColor(color))).Fallback(new ColorDrawable(Color.ParseColor(color)));
                                break;
                            default:
                                DefaultOptions.Placeholder(new ColorDrawable(Color.ParseColor(color))).Fallback(new ColorDrawable(Color.ParseColor(color)));
                                break;
                        }
                        break;
                    case ImagePlaceholders.Drawable:
                        switch (style)
                        {
                            case ImageStyle.CircleCrop:
                                CircleOptions.Placeholder(Resource.Drawable.ImagePlacholder_circle).Fallback(Resource.Drawable.ImagePlacholder_circle);
                                break;
                            default:
                                DefaultOptions.Placeholder(Resource.Drawable.ImagePlacholder).Fallback(Resource.Drawable.ImagePlacholder);
                                break;
                        }
                        break;
                }

                if (imageUri.Contains("FirstImageOne") || imageUri.Contains("FirstImageTwo") || imageUri.Contains("no_profile_image") || imageUri.Contains("blackdefault") || imageUri.Contains("no_profile_image_circle")
                    || imageUri.Contains("ImagePlacholder") || imageUri.Contains("ImagePlacholder_circle"))
                {
                    if (style == ImageStyle.CircleCrop)
                    {
                        if (imageUri.Contains("FirstImageOne"))
                            newImage.Load(Resource.Drawable.FirstImageOne).Apply(CircleOptions).Into(image);
                        else if (imageUri.Contains("FirstImageTwo"))
                            newImage.Load(Resource.Drawable.FirstImageTwo).Apply(CircleOptions).Into(image);
                        else if (imageUri.Contains("no_profile_image_circle"))
                            newImage.Load(Resource.Drawable.no_profile_image_circle).Apply(CircleOptions).Into(image);
                        else if (imageUri.Contains("no_profile_image"))
                            newImage.Load(Resource.Drawable.no_profile_image).Apply(CircleOptions).Into(image);
                        else if (imageUri.Contains("ImagePlacholder"))
                            newImage.Load(Resource.Drawable.ImagePlacholder).Apply(CircleOptions).Into(image);
                        else if (imageUri.Contains("ImagePlacholder_circle"))
                            newImage.Load(Resource.Drawable.ImagePlacholder_circle).Apply(CircleOptions).Into(image);
                    }
                    else
                    {
                        if (imageUri.Contains("FirstImageOne"))
                            newImage.Load(Resource.Drawable.FirstImageOne).Apply(DefaultOptions).Into(image);
                        else if (imageUri.Contains("FirstImageTwo"))
                            newImage.Load(Resource.Drawable.FirstImageTwo).Apply(DefaultOptions).Into(image);
                        else if (imageUri.Contains("no_profile_image_circle"))
                            newImage.Load(Resource.Drawable.no_profile_image_circle).Apply(DefaultOptions).Into(image);
                        else if (imageUri.Contains("no_profile_image"))
                            newImage.Load(Resource.Drawable.no_profile_image).Apply(DefaultOptions).Into(image);
                        else if (imageUri.Contains("ImagePlacholder"))
                            newImage.Load(Resource.Drawable.ImagePlacholder).Apply(DefaultOptions).Into(image);
                        else if (imageUri.Contains("ImagePlacholder_circle"))
                            newImage.Load(Resource.Drawable.ImagePlacholder_circle).Apply(DefaultOptions).Into(image);
                    }
                }
                else if (!string.IsNullOrEmpty(imageUri) && imageUri.Contains("http"))
                {
                    newImage.Load(imageUri).Apply(style == ImageStyle.CircleCrop ? CircleOptions : DefaultOptions).Into(image);
                }
                else if (!string.IsNullOrEmpty(imageUri) && (imageUri.Contains("file://") || imageUri.Contains("content://") || imageUri.Contains("storage")))
                {
                    var file = Uri.FromFile(new File(imageUri));
                    newImage.Load(file.Path).Apply(style == ImageStyle.CircleCrop ? CircleOptions : DefaultOptions).Into(image);
                }
                else
                {
                    newImage.Load(Resource.Drawable.no_profile_image).Apply(style == ImageStyle.CircleCrop ? CircleOptions : DefaultOptions).Into(image);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            } 
        } 
    }
}