#version 330 core
precision highp float;  // Use high precision for floats

out vec4 FragColor;

struct Light {
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    vec3 direction;   // Direction of the spotlight
    float cutOff;     // Spotlight cutoff angle (in radians)
    bool is_SpotLight;
	
    float constant;   // Point light constant attenuation
    float linear;     // Point light linear attenuation
    float quadratic;  // Point light quadratic attenuation
};

uniform Light light;

in VS_OUT {
    vec3 FragPos;
    vec3 Normal;
    vec2 TexCoords;
} fs_in;

uniform sampler2D floorTexture;
uniform vec4 lightPos;
uniform vec3 viewPos;
const float k = 0.8;

void main()
{
    // Normalized vectors for light and view direction
    vec3 normal = normalize(fs_in.Normal);
    vec3 lightDir;
    float distance = 0.0;
    float attenuation = 1.0;
    
    // Check if light is a point light or directional light
    if (lightPos.w == 1.0) {
        // Point Light: Calculate direction from fragment position to light position
        lightDir = normalize(lightPos.xyz - fs_in.FragPos);
        // Calculate the distance from the fragment to the light
        distance = length(lightPos.xyz - fs_in.FragPos);
        
        // Apply attenuation based on the distance
        attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));
    } else {
        // Directional Light: Use lightPos.xyz as direction (negate if light is coming from above)
        lightDir = normalize(-lightPos.xyz);
    }

    vec3 viewDir = normalize(viewPos - fs_in.FragPos);
    
    // Calculate the Minnaert diffuse lighting
    float d1 = pow(max(dot(normal, lightDir), 0.0), 1.0 + k);  // Light direction
    float d2 = pow(1.0 - dot(normal, viewDir), 1.0 - k);        // View direction

    // Apply attenuation to diffuse lighting
    d1 *= attenuation;

    // Fetch the texture color
    vec4 textureColor = texture(floorTexture, fs_in.TexCoords);
    
    // Spotlight cutoff logic
    float theta = dot(lightDir, normalize(-light.direction)); 
    
    if (light.is_SpotLight) {
        if (theta > light.cutOff) { // If the fragment is within the spotlight cone
            // Apply the Minnaert lighting model for the spotlight area
            FragColor = textureColor * d1 * d2;
            FragColor.a = 1.0;  // Ensure the fragment is fully opaque
        } else {
            // Outside the spotlight cone, apply only ambient light
            FragColor = vec4(0,0,0, 1.0);  // Only ambient light
        }
    } else {
        // If it's not a spotlight, apply the Minnaert lighting model
        FragColor = textureColor * d1 * d2;
        FragColor.a = 1.0;  // Ensure the fragment is fully opaque
    }

    // Final adjustment: Apply the attenuation to the entire lighting result
    FragColor.rgb *= attenuation;
}
