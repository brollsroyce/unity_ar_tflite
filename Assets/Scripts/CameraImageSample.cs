using System;
using TensorFlowLite;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// This component tests getting the latest camera image
    /// and converting it to RGBA format. If successful,
    /// it displays the image on the screen as a RawImage
    /// and also displays information about the image.
    ///
    /// This is useful for computer vision applications where
    /// you need to access the raw pixels from camera image
    /// on the CPU.
    ///
    /// This is different from the ARCameraBackground component, which
    /// efficiently displays the camera image on the screen. If you
    /// just want to blit the camera texture to the screen, use
    /// the ARCameraBackground, or use Graphics.Blit to create
    /// a GPU-friendly RenderTexture.
    ///
    /// In this example, we get the camera image data on the CPU,
    /// convert it to an RGBA format, then display it on the screen
    /// as a RawImage texture to demonstrate it is working.
    /// This is done as an example; do not use this technique simply
    /// to render the camera image on screen.
    /// </summary>
    public class CameraImageSample : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The ARCameraManager which will produce frame events.")]
        ARCameraManager m_CameraManager;

        /// <summary>
        /// Get or set the <c>ARCameraManager</c>.
        /// </summary>
        public ARCameraManager cameraManager
        {
            get => m_CameraManager;
            set => m_CameraManager = value;
        }

        [SerializeField] SsdSpecific ssd;

        Texture2D m_CameraTexture;
        TextureResizer resizer;
        XRCpuImage.Transformation m_Transformation = XRCpuImage.Transformation.None;
        int rotation;

        void OnEnable()
        {
            rotation = Application.platform == RuntimePlatform.IPhonePlayer ? -90 : 90;
            resizer = new TextureResizer();
            if (m_CameraManager != null)
            {
                m_CameraManager.frameReceived += OnCameraFrameReceived;
            }
        }

        void OnDisable()
        {
            resizer?.Dispose();
            if (m_CameraManager != null)
            {
                m_CameraManager.frameReceived -= OnCameraFrameReceived;
            }
            Destroy(m_CameraTexture);
        }

        unsafe void UpdateCameraImage()
        {
            // Attempt to get the latest camera image. If this method succeeds,
            // it acquires a native resource that must be disposed (see below).
            if (!cameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
            {
                return;
            }
            
            // Display some information about the camera image
            /*m_ImageInfo.text = string.Format(
                "Image info:\n\twidth: {0}\n\theight: {1}\n\tplaneCount: {2}\n\ttimestamp: {3}\n\tformat: {4}",
                image.width, image.height, image.planeCount, image.timestamp, image.format);*/

            // Once we have a valid XRCpuImage, we can access the individual image "planes"
            // (the separate channels in the image). XRCpuImage.GetPlane provides
            // low-overhead access to this data. This could then be passed to a
            // computer vision algorithm. Here, we will convert the camera image
            // to an RGBA texture and draw it on the screen.

            // Choose an RGBA format.
            // See XRCpuImage.FormatSupported for a complete list of supported formats.
            var format = TextureFormat.RGBA32;

            if (m_CameraTexture == null || m_CameraTexture.width != image.width || m_CameraTexture.height != image.height)
            {
                m_CameraTexture = new Texture2D(image.width, image.height, format, false);
            }

            // Convert the image to format, flipping the image across the Y axis.
            // We can also get a sub rectangle, but we'll get the full image here.
            var conversionParams = new XRCpuImage.ConversionParams(image, format, m_Transformation);

            // Texture2D allows us write directly to the raw texture data
            // This allows us to do the conversion in-place without making any copies.
            var rawTextureData = m_CameraTexture.GetRawTextureData<byte>();
            try
            {
                image.Convert(conversionParams, new IntPtr(rawTextureData.GetUnsafePtr()), rawTextureData.Length);
            }
            finally
            {
                // We must dispose of the XRCpuImage after we're finished
                // with it to avoid leaking native resources.
                image.Dispose();
            }

            // Apply the updated texture data to our texture
            m_CameraTexture.Apply();
        }

        void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
        {
            UpdateCameraImage();
            ssd.Invoke(NormalizeWebcam(m_CameraTexture, m_CameraTexture.width, m_CameraTexture.height, false));
        }
        
        Texture NormalizeWebcam(Texture texture, int width, int height, bool isFrontFacing)
        {
            int cameraWidth = texture.width;
            int cameraHeight = texture.height;

            (cameraWidth, cameraHeight) = (cameraHeight, cameraWidth); // swap

            float cameraAspect = (float)cameraWidth / cameraHeight;
            float targetAspect = (float)width / height;

            int w, h;
            if (cameraAspect > targetAspect)
            {
                w = RoundToEven(cameraHeight * targetAspect);
                h = cameraHeight;
            }
            else
            {
                w = cameraWidth;
                h = RoundToEven(cameraWidth / targetAspect);
            }

            Matrix4x4 mtx;
            Vector4 uvRect;

            mtx = TextureResizer.GetVertTransform(rotation, texture, isFrontFacing);
            uvRect = TextureResizer.GetTextureST(targetAspect, cameraAspect, AspectMode.Fill);

            return resizer.Resize(texture, w, h, false, mtx, uvRect);
        }
        
        private static int RoundToEven(float n)
        {
            return Mathf.RoundToInt(n / 2) * 2;
        }        
    }
}