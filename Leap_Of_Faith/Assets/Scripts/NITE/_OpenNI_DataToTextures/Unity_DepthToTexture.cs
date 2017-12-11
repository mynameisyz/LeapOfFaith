// Unity_DepthToTexture
// Author: Noisecrime
// Date:   18.06.11

// Summary
// Alternative class to OpenNIDepthmapViewer - has NO ONGUI and stores texture in material.

// Display Kinect Depth image using Unity's native methods (SetPixels) to copy and resize the source data into a texture.
// Scaling image is acheived by skipping rows/cols, so it is either fullsize, half-size, quater-size etc.

// Implementation:
// Mipmaps are disable as default to avoid re-calculating them each time the texture is updated.
// Only enable if you really need mipmaps as their creation adveserly affects performance.

// Texture for the image is forced to next power-of-two as Unity does not natively support non-pot textures.
// If you use a non-pot dimension then internally Unity will create two versions which is in-efficient!
// By forcing to use the next power of two we ensure Unity only has one copy of the texture internally and
// gain 30% performance, sometimes more.

// Further performance can be gained if you set the texture format to RGB24 instead of ARGB32.
// This will have the downside that the texture will not have an alpha and therefore any border due to POT will be visible.

// Further Optimisations
// Could try building a POT color array and avoid cropped SetPixels() function?


using UnityEngine;
using System;
using System.Runtime.InteropServices;
using OpenNI;

public class Unity_DepthToTexture : MonoBehaviour 
{
	private OpenNIContext 	Context;
		
    public int              desiredFactor   = 4;	        // User determined scaling of source (kinect) image.
	public	bool			forcePowerOfTwo = true;			// Default: True (faster), False is much slower.
	public	bool			useMipmaps 		= false;		// Default: False (faster), True is slower, but lets you scale texture.
	public	bool			SetAltViewPoint = false;		// Default: False - if true maps depth/label image into RGB space.
	public Color 			depthColor      = Color.yellow;
	public	Material		targetMaterial;
	
	private	Texture2D 		depthMapTexture;	            // Unity Texture for displaying Kinect depth.
	private	Color[] 		depthMapColors;		            // Unity colors array for kinect depth.
	private	short[]			depthMapRaw;		            // Array of shorts to hold Kinect depth source. 
	private	float[] 		depthHistogramMap;
		
	private int				actualFactor = 4;	            // User determined scaled forced to power-of-two, i.e. 1,2,4,8 etc	
	private	int 			rawWidth;			            // Width of kinect source image in pixels.
	private	int 			rawHeight;			            // Height of kinect source image in pixels.
	private	int 			potWidth;			            // Width of Power-of-two Unity Texture.
	private	int 			potHeight;			            // Height of Power-of-two Unity Texture.
	private	int 			dstWidth;			            // Width of scaled Image in Unity.
	private	int 			dstHeight;			            // Height of scaled Image in Unity.

	void Start () 
	{
		Context = OpenNIContext.Instance;
		
		// Transform depthmap into RGB image space?
		if (SetAltViewPoint) Context.Depth.AlternativeViewpointCapability.SetViewpoint(Context.Image);
		
		// Force Factor to a power of two 1,2,4,8 etc
		actualFactor 		= getNextPowerOfTwo(desiredFactor);

		// Init texture - getting image size from openNI, then determining factor
		MapOutputMode mom 	= Context.Depth.MapOutputMode;
		rawWidth			= mom.XRes;
		rawHeight 			= mom.YRes;
		dstWidth			= rawWidth/actualFactor;
		dstHeight			= rawHeight/actualFactor;
			
		// Force Texture to be Power-Of-Two to avid Unity making two copies
        potWidth            = forcePowerOfTwo ? getNextPowerOfTwo(dstWidth) : dstWidth;
        potHeight           = forcePowerOfTwo ? getNextPowerOfTwo(dstHeight) : dstHeight;
		depthMapTexture 	= new Texture2D(potWidth, potHeight, TextureFormat.RGB24 , useMipmaps);

        // Force filter mode to Bilinear to avoid (trilinear) mipmaps		
        depthMapTexture.filterMode =  useMipmaps ? FilterMode.Trilinear : FilterMode.Bilinear;

		// depthmap data
		depthMapRaw 		= new short[rawWidth * rawHeight];
		depthMapColors 		= new Color[dstWidth*dstHeight];
		
		// histogram stuff
		int maxDepth = (int)Context.Depth.DeviceMaxDepth;
		depthHistogramMap = new float[maxDepth];
		
		// Set up texture in material
		targetMaterial.mainTexture = depthMapTexture;
		
		// Flip and scale texture
		float uScale =  dstWidth/(float)potWidth;
		float vScale =  dstHeight/(float)potHeight;		
		targetMaterial.SetTextureScale ("_MainTex", new Vector2 (uScale, vScale) ); 
		targetMaterial.SetTextureOffset("_MainTex", new Vector2 (0.0f,   0.0f) );
		
		/*
		 // Pre-fill Unity Texture to use no alpha. To hide the area not used in the POT texture.
        Color[] txColorArray = depthMapTexture.GetPixels();
		
		for(int i=0; i< txColorArray.Length;i++)
			txColorArray[i].a = 0.0f;
			
		depthMapTexture.SetPixels(txColorArray);
		
		// Pre-fill Unity color array to use full alpha.
		for (int i=0; i<(dstWidth*dstHeight); i++)		
        {
			depthMapColors[i].a = 1.0f;
		}
		*/
			
		print(	"Depth: Factor: " + actualFactor + " (" + desiredFactor+ ") - Src Width: " + rawWidth + "   Src Height: " + rawHeight + 
				"  Dst Width: " + dstWidth + "   Dst Height: " + dstHeight + "  Tx Width: " + potWidth + "   Tx Height: " + potHeight +
				"     Filter: " + depthMapTexture.filterMode + "  Format: " + depthMapTexture.format + "  MipMaps:" + depthMapTexture.mipmapCount);
	}
	
	void Update () 
    {
		Context.Update();
		
		Marshal.Copy(Context.Depth.DepthMapPtr, depthMapRaw, 0, depthMapRaw.Length);
		UpdateHistogram();
		UpdateDepthmapTexture();
	}
	
	void UpdateHistogram()
	{
		int i, numOfPoints = 0;	
		int depthIndex 	= 0;
		
		Array.Clear(depthHistogramMap, 0, depthHistogramMap.Length);
				
		for (int y = 0; y < dstHeight; ++y)
		{
			for (int x = 0; x < dstWidth; ++x, depthIndex += actualFactor)
			{
				if (depthMapRaw[depthIndex] != 0)
				{
					depthHistogramMap[depthMapRaw[depthIndex]]++;
					numOfPoints++;
				}
			}
			depthIndex += (actualFactor-1)*rawWidth; // Skip lines
		}

        if (numOfPoints > 0)
        {
            for (i = 1; i < depthHistogramMap.Length; i++)
	        {   
		        depthHistogramMap[i] += depthHistogramMap[i-1];
	        }
            for (i = 0; i < depthHistogramMap.Length; i++)
	        {
                depthHistogramMap[i] = 1.0f - (depthHistogramMap[i] / numOfPoints);
	        }
        }
	}
	
	void UpdateDepthmapTexture()
    {
		// flip the depthmap as we create the texture	
		int i 			= dstWidth*dstHeight-1;
		int depthIndex 	= 0;
		
		depthHistogramMap[0] = 0; //Force rawdepth = 0 to black in histogram, avoids conditional check in loop.
		
		for (int y = 0; y < dstHeight; ++y)
		{
			for (int x = 0; x < dstWidth; ++x, --i, depthIndex += actualFactor)
			{			
				// Fast Method - 39 fps
				float depthValue = depthHistogramMap[depthMapRaw[depthIndex]];
				depthMapColors[i].r = depthColor.r * depthValue;	
				depthMapColors[i].g = depthColor.g * depthValue;
				depthMapColors[i].b = depthColor.b * depthValue;
				// depthMapColors[i].a = 1.0f;
						
				/*
				// Slower Method - 31 fps				
				short pixel = depthMapRaw[depthIndex];
				Color c = new Color(depthHistogramMap[pixel], depthHistogramMap[pixel], depthHistogramMap[pixel], 1.0f);
				depthMapColors[i] = depthColor * c;	
				*/							
			}
			depthIndex += (actualFactor-1)*rawWidth; // Skip lines
		}
		
		depthMapTexture.SetPixels(0, 0, dstWidth, dstHeight, depthMapColors, 0);
        depthMapTexture.Apply( useMipmaps );
   }
   

   
   
	// Support function: Finds the next HIGHEST POT
	int getNextPowerOfTwo (int val)
	{
		val--;
		val = (val >> 1) | val;
		val = (val >> 2) | val;
		val = (val >> 4) | val;
		val = (val >> 8) | val;
		val = (val >> 16) | val;
		val++; // Val is now the next highest power of 2.
		return (val);
	}
}
