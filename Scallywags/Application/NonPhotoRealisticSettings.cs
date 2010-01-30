#region File Description
//-----------------------------------------------------------------------------
// NonPhotoRealisticSettings.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;

namespace Scallywags
{
    /// <summary>
    /// Class holds all the settings used to tweak the non-photorealistic rendering.
    /// </summary>
    public class NonPhotoRealisticSettings
    {
        #region Fields

        // Name of a preset setting, for display to the user.
        public readonly string Name;

        // Is the cartoon lighting shader enabled?
        public readonly bool EnableToonShading;

        // Settings for the edge detect filter.
        public readonly bool EnableEdgeDetect;
        public readonly float EdgeWidth;
        public readonly float EdgeIntensity;

        // Settings for the pencil sketch effect.
        public readonly bool EnableSketch;
        public readonly bool SketchInColor;
        public readonly float SketchThreshold;
        public readonly float SketchBrightness;
        public readonly float SketchJitterSpeed;

        #endregion


        /// <summary>
        /// Constructs a new non-photorealistic settings descriptor.
        /// </summary>
        public NonPhotoRealisticSettings(string name,
                                         bool enableEdgeDetect,
                                         float edgeWidth, float edgeIntensity,
                                         bool enableSketch, bool sketchInColor,
                                         float sketchThreshold, float sketchBrightness)
        {
            Name = name;
            EnableEdgeDetect = enableEdgeDetect;
            EdgeWidth = edgeWidth;
            EdgeIntensity = edgeIntensity;
            EnableSketch = enableSketch;
            SketchInColor = sketchInColor;
            SketchThreshold = sketchThreshold;
            SketchBrightness = sketchBrightness;
        }
        

        /// <summary>
        /// Table of preset settings, used by the sample program.
        /// </summary>
        public static NonPhotoRealisticSettings[] PresetSettings =
        {
            new NonPhotoRealisticSettings("Subtle Edge Enhancement",
                                          true, 0.5f, 0.5f,
                                          false, false, 0, 0),

                                          new NonPhotoRealisticSettings("Pencil",
                                          true, 0.5f, 0.5f,
                                          true, true, 0.1f, 0.3f),

                                          new NonPhotoRealisticSettings("Inverse",
                                          true, 0.5f, 0.5f,
                                          true, false, 0.1f, 0.3f),

                                          new NonPhotoRealisticSettings("Nothing Special",
                                          false, 0, 0,
                                          false, false, 0, 0),

            //new NonPhotoRealisticSettings("Cartoon", true,
            //                              true, 0.75f, 0.75f,
            //                              false, false, 0, 0, 0),

            

            //new NonPhotoRealisticSettings("Chunky Monochrome", true,
            //                              true, 1.5f, 0.5f,
            //                              true, false, 0, 0.35f, 0),

            
        };
    }
}