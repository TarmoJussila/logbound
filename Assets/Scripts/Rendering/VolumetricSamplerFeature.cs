using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Logbound.Rendering
{
    public class VolumetricSamplerFeature : ScriptableRendererFeature
    {
        private class CustomRenderPass : ScriptableRenderPass
        {
            private Material _material;

            private RTHandle _dstHandle;
            private string _globalTextureName;

            public CustomRenderPass(RenderPassEvent e, Material mat, RenderTexture tex, string globalTextureName)
            {
                renderPassEvent = e;
                _material = mat;
                requiresIntermediateTexture = true;
                _globalTextureName = globalTextureName;
            }

            // This class stores the data needed by the RenderGraph pass.
            // It is passed as a parameter to the delegate function that executes the RenderGraph pass.
            private class PassData
            {
            }

            // This static method is passed as the RenderFunc delegate to the RenderGraph render pass.
            // It is used to execute draw commands.
            private static void ExecutePass(PassData data, RasterGraphContext context)
            {
            }

            // RecordRenderGraph is where the RenderGraph handle can be accessed, through which render passes can be added to the graph.
            // FrameData is a context container through which URP resources can be accessed and managed.
            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                const string passName = "VolumetricSamplerFeature";

                // UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
                // UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
                //
                // if (resourceData.isActiveTargetBackBuffer)
                // {
                //     return;
                // }
                //
                // RenderGraphUtils.BlitMaterialParameters parameters = new();
                // TextureHandle rtHandle = renderGraph.ImportTexture(_dstHandle);
                //
                // var src = resourceData.activeColorTexture;RenderTextureDescriptor textureProperties = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.Default, 0);
                //
                // TextureHandle destination = UniversalRenderer.CreateRenderGraphTexture(renderGraph,
                //     textureProperties, _globalTextureName, false);
                //
                // parameters.material = _material;
                // parameters.source = src;
                // parameters.destination = rtHandle;
                //
                // renderGraph.AddBlitPass(parameters, passName);

                //This adds a raster render pass to the graph, specifying the name and the data type that will be passed to the ExecutePass function.
                using (var builder = renderGraph.AddRasterRenderPass<PassData>(passName, out var passData))
                {
                    // Use this scope to set the required inputs and outputs of the pass and to
                    // setup the passData with the required properties needed at pass execution time.

                    // Make use of frameData to access resources and camera data through the dedicated containers.
                    // Eg:
                    var cameraData = frameData.Get<UniversalCameraData>();
                    var resourceData = frameData.Get<UniversalResourceData>();

                    if (resourceData.isActiveTargetBackBuffer)
                    {
                        return;
                    }

                    // Setup pass inputs and outputs through the builder interface.
                    // Eg:
                    // builder.UseTexture(sourceTexture);
                    var textureProperties = new RenderTextureDescriptor(cameraData.cameraTargetDescriptor.width, cameraData.cameraTargetDescriptor.height, RenderTextureFormat.Default, 0);

                    var destination = UniversalRenderer.CreateRenderGraphTexture(renderGraph,
                        textureProperties, _globalTextureName, false);

                    // This sets the render target of the pass to the active color texture. Change it to your own render target as needed.


                    builder.AllowGlobalStateModification(true);
                    builder.SetRenderAttachment(destination, 0);
                    builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture);
                    builder.AllowPassCulling(false);
                    //builder.UseGlobalTexture(Shader.PropertyToID(_globalTextureName), AccessFlags.ReadWrite);
                    builder.SetGlobalTextureAfterPass(destination, Shader.PropertyToID(_globalTextureName));

                    // Assigns the ExecutePass function to the render pass delegate. This will be called by the render graph when executing the pass.
                    builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecutePass(data, context));
                    builder.Dispose();
                }
            }

            // NOTE: This method is part of the compatibility rendering path, please use the Render Graph API above instead.
            // Cleanup any allocated resources that were created during the execution of this render pass.
            public override void OnCameraCleanup(CommandBuffer cmd)
            {
            }
        }

        [SerializeField] private RenderPassEvent m_InjectionPoint = RenderPassEvent.AfterRenderingPrePasses;
        [SerializeField] private Material m_Material;
        [SerializeField] private RenderTexture m_RenderTexture;
        [SerializeField] private string m_GlobalTextureName;

        private CustomRenderPass m_ScriptablePass;

        /// <inheritdoc/>
        public override void Create()
        {
            m_ScriptablePass = new CustomRenderPass(m_InjectionPoint, m_Material, m_RenderTexture, m_GlobalTextureName);
        }

        // Here you can inject one or multiple render passes in the renderer.
        // This method is called when setting up the renderer once per-camera.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(m_ScriptablePass);
        }
    }
}