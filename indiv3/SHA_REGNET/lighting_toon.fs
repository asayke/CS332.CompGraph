#version 330 core

out vec4 FragColor;

struct Light {
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    vec3 direction;
    float cutOff;  // Spotlight's cutoff angle in radians
    bool is_SpotLight;
	
    float constant;
    float linear;
    float quadratic;
};

uniform Light light;

in VS_OUT {
    vec3 FragPos;
    vec3 Normal;
    vec2 TexCoords;
} fs_in;

uniform sampler2D floorTexture;
uniform vec4 lightPos;   // The position and type of the light (w component as the trigger)
uniform vec3 viewPos;

void main()
{   
    // Normalize the normal vector
    vec3 norm = normalize(fs_in.Normal);
    vec3 lightDir;
    float distance = 0.0;
    float attenuation = 1.0;
    float spotFactor = 1.0;  // For spotlight attenuation based on angle

    if (lightPos.w == 1.0) {
        // Point Light: Calculate direction from fragment position to light position
        lightDir = normalize(lightPos.xyz - fs_in.FragPos);
        // Calculate the distance from the fragment to the light
        distance = length(lightPos.xyz - fs_in.FragPos);
        
        // Spotlight: Attenuate based on spotlight cone (cutoff angle)
        if (light.is_SpotLight) {
            // Normalize the direction of the spotlight
            lightDir = normalize(lightPos.xyz - fs_in.FragPos);
            float theta = dot(lightDir, normalize(-light.direction));
            
            // Check if the fragment is within the spotlight cone
            if (theta > light.cutOff) {
                // Calculate the attenuation based on the angle
                float epsilon = light.cutOff - 0.1;  // Add a small epsilon for smooth transition
                spotFactor = (theta - epsilon) / (1.0 - epsilon); // Smooth falloff
                spotFactor = clamp(spotFactor, 0.0, 1.0);
            } else {
                spotFactor = 0.0;  // Outside spotlight cone
            }

            // Calculate the distance-based attenuation
            distance = length(viewPos - fs_in.FragPos);
        }
        
        // Apply attenuation based on the distance
        attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));
    } else {
        // Directional Light: Use lightPos.xyz as direction (no attenuation)
        lightDir = normalize(-lightPos.xyz); // Direction is simply the negated light position
    }

    // Compute the diffuse lighting intensity (dot product between normal and light direction)
    float diff = max(dot(norm, lightDir), 0.0);
    
    // Toon shading quantization: We'll break the light intensity into 3 bands
    float toonShading = 0.0;
    if (diff > 0.8) {
        toonShading = 1.0; // Lightest band
    } else if (diff > 0.4) {
        toonShading = 0.7; // Mid band
    } else if (diff > 0.1) {
        toonShading = 0.4; // Dark band
    } else {
        toonShading = 0.2; // Very dark band
    }

    // Apply toon shading to diffuse lighting and apply attenuation (if point light or spotlight)
    vec3 diffuse = toonShading * light.diffuse * attenuation * spotFactor;

    // Specular lighting (simplified for toon shading)
    vec3 viewDir = normalize(viewPos - fs_in.FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = 0.0;

    // Quantize specular component (sharp specular highlight)
    float specStrength = max(dot(viewDir, reflectDir), 0.0);
    if (specStrength > 0.8) {
        spec = 1.0; // Sharp specular highlight
    } else if (specStrength > 0.4) {
        spec = 0.7; // Medium specular highlight
    } else if (specStrength > 0.1) {
        spec = 0.4; // Low specular highlight
    } else {
        spec = 0.2; // No specular highlight
    }

    // Apply specular shading and attenuate it if it's a point light or spotlight
    vec3 specular = spec * light.specular * attenuation * spotFactor;

    // Sample the texture color
    vec3 texColor = texture(floorTexture, fs_in.TexCoords).rgb;

    // Combine the lighting components: ambient, diffuse, specular, and texture
    vec3 finalColor = texColor * (light.ambient + diffuse + specular);

    
    FragColor = vec4(finalColor, 1.0);
}
