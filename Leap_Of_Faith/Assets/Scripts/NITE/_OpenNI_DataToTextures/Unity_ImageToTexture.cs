// Unity_ImageToTexture
// Author: Noisecrime
// Date:   18.06.11

// Summary
// Alternative class to OpenNIImagemapViewer - has NO ONGUI and stores texture in material.

// Display Kinect RGB Image using Unity's native methods (SetPixels) to copy and resize the source data into a texture.
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
using System.IO;
using System.Runtime.InteropServices;
using OpenNI;

public class Unity_ImageToTexture : MonoBehaviour 
{
	private OpenNIContext 	Context;
	
	public	int 			desiredFactor 	= 4;		// User determined scaling of source (kinect) image.
	public	bool			forcePowerOfTwo = true;		// Default: True (faster), False is much slower.
	public	bool			useMipmaps 		= false;	// Default: False (faster), True is slower, but lets you scale texture.
	public	Material		targetMaterial;
	
	[NonSerialized]
	public	Texture2D 		imageMapTexture;			// Unity Texture for displaying Kinect image.
	public  Texture2D		imageOutputTexture;
	
	private	Color[] 		imageMapColors;				// Unity colors array for kinect image.
	private	byte[]			imageMapRaw;				// Byte array to hold Kinect image source. RGB Format
	
	//custom image to output
	private Color[]			imageMapOutput;
	
	private int				actualFactor = 4;			// User determined scaled forced to power-of-two, i.e. 1,2,4,8 etc
	private	int 			rawWidth;					// Width of kinect source image  in pixels.
	private	int 			rawHeight;					// Height of kinect source image in pixels.
	private	int 			potWidth;					// Width of Unity Texture as next Power-of-two.
	private	int 			potHeight;					// Height of Unity Texture as next Power-of-two.
	private	int 			dstWidth;					// Width of scaled Image in Unity.
	private	int 			dstHeight;					// Height of scaled Image in Unity.
	
	void Start () 
	{
		Context = OpenNIContext.Instance;
		
		// Force Factor to a power of two 1,2,4,8 etc
		actualFactor 		= getNextPowerOfTwo(desiredFactor);

		// Init texture - getting image size from openNI, then determining factor
		MapOutputMode mom 	= Context.Image.MapOutputMode;
		rawWidth			= mom.XRes;
		rawHeight 			= mom.YRes;
		dstWidth			= rawWidth/actualFactor;
		dstHeight			= rawHeight/actualFactor;
			
		// Force Texture to be Power-Of-Two to avid Unity making two copies
		potWidth 			= forcePowerOfTwo ? getNextPowerOfTwo( dstWidth  ) : dstWidth ;
        potHeight           = forcePowerOfTwo ? getNextPowerOfTwo( dstHeight ) : dstHeight;
		imageMapTexture 	= new Texture2D(potWidth, potHeight, TextureFormat.RGB24 , useMipmaps);
		imageOutputTexture 	= new Texture2D(rawWidth, rawHeight, TextureFormat.RGB24 , useMipmaps);
        
        // Force filter mode to Bilinear to avoid (trilinear) mipmaps		
        imageMapTexture.filterMode =  useMipmaps ? FilterMode.Trilinear : FilterMode.Bilinear;		

		// Imagemap data, bytes = width*height*RGB
		imageMapRaw 		= new byte[rawWidth * rawHeight * 3];
		imageMapColors		= new Color[dstWidth*dstHeight];
		imageMapOutput 		= new Color[rawWidth * rawHeight];
		        
		// Set up texture in material
		targetMaterial.mainTexture = imageMapTexture;
		
		// Flip and scale texture
		float uScale =  dstWidth/(float)potWidth;
		float vScale =  dstHeight/(float)potHeight;		
		targetMaterial.SetTextureScale ("_MainTex", new Vector2 (uScale, vScale) ); 
		targetMaterial.SetTextureOffset("_MainTex", new Vector2 (0.0f,   0.0f) );
		
		/*
        // Pre-fill Unity Texture to use no alpha. To hide the area not used in the POT texture.
        Color[] txColorArray = imageMapTexture.GetPixels();
		
		for(int i=0; i< txColorArray.Length;i++)
			txColorArray[i].a = 0.0f;
			
		imageMapTexture.SetPixels(txColorArray);
		
		// Pre-fill Unity color array to use full alpha.
		for (int i=0; i<(dstWidth*dstHeight); i++)		
        {
			imageMapColors[i].a = 1.0f;
		}	
*/

		print(	"Image: Factor: " + actualFactor + " (" + desiredFactor+ ") - Src Width: " + rawWidth + "   Src Height: " + rawHeight + 
				"  Dst Width: " + dstWidth + "   Dst Height: " + dstHeight + "  Tx Width: " + potWidth + "   Tx Height: " + potHeight +
				"     Filter: " + imageMapTexture.filterMode + "  Format: " + imageMapTexture.format + "  MipMaps:" +imageMapTexture.mipmapCount);
		
		}
	

	void Update () 
	{
		Context.Update();
		// Copy data and update Texture
		Marshal.Copy(Context.Image.ImageMapPtr, imageMapRaw, 0, imageMapRaw.Length);
		UpdateImageMapTexture();
	}
	
	void UpdateImageMapTexture()
    {
		// flip the depthmap as we create the texture	
		int i 			= dstWidth*dstHeight-1;
		int index 		= 0;
		float modifier 	= 1.0f/255.0f;
		
		for (int y = 0; y < dstHeight; ++y)
		{
			for (int x = 0; x < dstWidth; ++x, --i, index += actualFactor)
			{				
				imageMapColors[i].r = imageMapRaw[(index*3)]   * modifier;	
				imageMapColors[i].g = imageMapRaw[(index*3)+1] * modifier;
				imageMapColors[i].b = imageMapRaw[(index*3)+2] * modifier;						
			}
			index += (actualFactor-1)*rawWidth; // Skip lines
		}
		
		imageMapTexture.SetPixels(0, 0, dstWidth, dstHeight, imageMapColors, 0);
        imageMapTexture.Apply( useMipmaps );

#pragma warning disable 0219
		int j 			= rawWidth*rawHeight-1;
#pragma warning restore 0219

		int jIndex 		= 0;

#pragma warning disable 0219
		float jModifier 	= 1.0f/255.0f;
#pragma warning restore 0219

		for (int y = 0; y < rawHeight; ++y)
		{
			for (int x = 0; x < rawWidth; ++x, --i, jIndex += actualFactor)
			{				
				imageMapOutput[i].r = imageMapRaw[(jIndex*3)]   * modifier;	
				imageMapOutput[i].g = imageMapRaw[(jIndex*3)+1] * modifier;
				imageMapOutput[i].b = imageMapRaw[(jIndex*3)+2] * modifier;						
			}
			jIndex += (actualFactor-1)*rawWidth; // Skip lines
		}
		
		imageOutputTexture.SetPixels(0, 0, rawWidth, rawHeight, imageMapOutput, 0);
		imageOutputTexture.Apply( useMipmaps );	
		
		byte[] output = imageMapTexture.EncodeToPNG();
		File.WriteAllBytes(Application.dataPath + "/../SavedScreentx.png", output);
		
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
