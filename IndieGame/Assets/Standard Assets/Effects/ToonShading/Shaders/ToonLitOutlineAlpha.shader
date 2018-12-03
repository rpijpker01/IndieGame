Shader "Toon/Lit Outline-Alpha" {
	Properties{
		_Color("Main Color", Color) = (0.5,0.5,0.5,1)
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_Outline("Outline width", Range(.002, 0.03)) = .005
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Ramp("Toon Ramp (RGB)", 2D) = "gray" {}
		_AlphaMultiplier("Alpha Multiplier", range(0.0, 1.0)) = 0.6
	}

		SubShader{
			Tags { "RenderType" = "Opaque" "Queue" = "Transparent"}
			UsePass "Toon/Basic Outline-Alpha/OUTLINE"
			UsePass "Toon/Lit-Alpha/FORWARD"
		}

			Fallback "Toon/Lit"
}
