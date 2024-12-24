#version 330 core
out vec4 FragColor;

struct Light {
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    vec3 direction;
    float cutOff;
    float outerCutOff;
    bool is_SpotLight;
	
    float constant;
    float linear;
    float quadratic;
};

in VS_OUT {
    vec3 FragPos;
    vec3 Normal;
    vec2 TexCoords;
} fs_in;

uniform sampler2D floorTexture;
uniform vec4 lightPos;   // Position or Direction of the light (w is the trigger)
uniform vec3 viewPos;
uniform Light light;

void main()
{
    // Fetch the texture color
    vec3 color = texture(floorTexture, fs_in.TexCoords).rgb;

    // Ambient lighting (constant)
    vec3 ambient = 0.05 * color;

    // Prepare for Diffuse, Specular calculations
    vec3 lightDir;
    
    // Check if light is a Point Light (w == 1.0) or Directional Light (w == 0.0)
    if (lightPos.w == 1.0) {
        // Point Light: Calculate direction from fragment position to light position
        lightDir = normalize(lightPos.xyz - fs_in.FragPos);
        
    
    } else {
        // Directional Light: Use lightPos.xyz as direction (negate if light is coming from above)
        lightDir = normalize(-lightPos.xyz);
    }

    // Normal vector of the fragment
    vec3 normal = normalize(fs_in.Normal);

    // Diffuse lighting (Lambertian reflectance)
    float diff = max(dot(lightDir, normal), 0.0);
    vec3 diffuse = diff * color;

    // Specular lighting (Phong model)
    vec3 viewDir = normalize(viewPos - fs_in.FragPos);
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = 0.0;
    vec3 halfwayDir = normalize(lightDir + viewDir);
    spec = pow(max(dot(normal, halfwayDir), 0.0), 32.0); // Phong shininess exponent (32)

    vec3 specular = vec3(0.3) * spec; // assuming bright white light color
    if (lightPos.w == 1.0) {
        ambient *= light.ambient;
        diffuse *= light.diffuse;
        specular *= light.specular;
        float distance    = length(lightPos - fs_in.FragPos);
        float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));    
        ambient  *= attenuation;  
        diffuse   *= attenuation;
        specular *= attenuation; 
    } 

    // Final color = ambient + diffuse + specular
    float theta = dot(lightDir, normalize(-light.direction)); 
    if(light.is_SpotLight){
        if(theta > light.cutOff ) // remember that we're working with angles as cosines instead of degrees so a '>' is used.
            { 
            FragColor = vec4(ambient + diffuse + specular, 1.0);
            }else{
                FragColor = vec4((ambient).rgb, 1.0);
            }
        }
    else{
        FragColor = vec4(ambient + diffuse + specular, 1.0);
    }
}
    
