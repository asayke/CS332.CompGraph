#include <glad/glad.h>
#include <GLFW/glfw3.h>

#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>

#include "shaders.h"
#include "camera.h"
#include "model.h"

#include <iostream>
#include <list>
#include <deque>
#include <cstdlib> // for rand() and srand()
#include <ctime>   // for time() to seed the random number generator

// ���� ����������� stb_image.h, �� ��������:
// #define STB_IMAGE_IMPLEMENTATION
// #include "stb_image.h"

void framebuffer_size_callback(GLFWwindow* window, int width, int height);
void mouse_callback(GLFWwindow* window, double xpos, double ypos);
void scroll_callback(GLFWwindow* window, double xoffset, double yoffset);
void processInput(GLFWwindow* window);
unsigned int loadTexture(const char* path);
int last_pressed_key = GLFW_RELEASE;

//Directional light
class Light {
public:
    glm::vec3 position;
    glm::vec3 ambient;
    glm::vec3 diffuse;
    glm::vec3 specular;

    Light(glm::vec3 pos,
        glm::vec3 amb = glm::vec3(0.2f, 0.2f, 0.2f),
        glm::vec3 diff = glm::vec3(0.1f, 0.1f, 0.1f),
        glm::vec3 spec = glm::vec3(0.5f, 0.5f, 0.5f))
    {
        position = pos;
        ambient = amb;
        diffuse = diff;
        specular = spec;
    }
};

class PointLight : public Light {
public:
    float power;
    float coef;
    float linear;
    float quadratic;

    PointLight(glm::vec3 pos,
        float pow = 5.0f,
        float _coef = 1.0f,
        float _linear = 0.09f,
        float _quadratic = 0.032f,
        glm::vec3 amb = glm::vec3(0.2f, 0.2f, 0.2f),
        glm::vec3 diff = glm::vec3(0.5f, 0.5f, 0.5f),
        glm::vec3 spec = glm::vec3(1.0f, 1.0f, 1.0f))
        : Light(pos, amb, diff, spec)
    {
        power = pow;
        coef = _coef;
        linear = _linear;
        quadratic = _quadratic;
    }
};

class SpotLight : public PointLight {
public:
    float cutOff;     // ���� (� ��������) ���������� �������
    float outCutOff;  // ���� (� ��������) ������� �������
    glm::vec3 direction;

    SpotLight(glm::vec3 pos,
        float _cutOff = 15.0f,
        float _outCutOff = 18.0f,
        glm::vec3 _direction = glm::vec3(0, -1, 0),
        float pow = 1.0f,
        float _coef = 0.1f,
        float _linear = 0.09f,
        float _quadratic = 0.032f,
        glm::vec3 amb = glm::vec3(0.2f, 0.2f, 0.2f),
        glm::vec3 diff = glm::vec3(0.5f, 0.5f, 0.5f),
        glm::vec3 spec = glm::vec3(1.0f, 1.0f, 1.0f))
        : PointLight(pos, pow, _coef, _linear, _quadratic, amb, diff, spec)
    {
        cutOff = _cutOff;
        outCutOff = _outCutOff;
        direction = _direction;
    }
};


class gameObject
{
public:
    Model model;
    glm::mat4 transform;
    glm::vec3 velocity;
    glm::vec3 acceleration;
    float radius; // ��� �������� ������������ (���� ������� ������ ������)

    gameObject(const char* model_path,
        glm::mat4 t = glm::mat4(1.0f),
        glm::vec3 vel = glm::vec3(0),
        glm::vec3 accel = glm::vec3(0),
        float rad = 1.0f)
        : model(model_path), radius(rad)
    {
        transform = t;
        velocity = vel;
        acceleration = accel;
    }

    // ��������� ������� ������� (�� �������������� ����� ������� transform)
    glm::vec3 getPosition() const {
        return glm::vec3(transform[3]);
    }

    // ������� ���������� (�������� + ���������)
    void MakeStep(float deltaTime) {
        transform = glm::translate(transform, velocity * deltaTime);
        velocity += acceleration * deltaTime; // ������� ������
    }

    void Draw(Shader shader) {
        model.Draw(shader);
    }

    // ��������������� ������� ��� �������������
    void Rotate(float angle, glm::vec3 axis) {
        transform = glm::rotate(transform, glm::radians(angle), axis);
    }
    void Scale(glm::vec3 scaleFactor) {
        transform = glm::scale(transform, scaleFactor);
    }
    void Translate(glm::vec3 translation) {
        transform = glm::translate(transform, translation);
    }

    // ����� � ��������� ���������
    void Reset(glm::mat4 originalTransform = glm::mat4(1.0f), glm::vec3 originalVelocity = glm::vec3(0)) {
        transform = originalTransform;
        velocity = originalVelocity;
    }

    // ��������� ���� (���� �����)
    void ApplyForce(glm::vec3 force, float deltaTime) {
        glm::vec3 accelerationDueToForce = force; // �������, ��� ����� = 1
        velocity += accelerationDueToForce * deltaTime;
    }

    // ��������� ����������
    void ApplyGravity(float deltaTime) {
        const float gravity = -9.8f;  // ��������� ���������� �������
        if (transform[3].y > -0.5f) { // ������� ��� ��� y = -0.5
            velocity.y += gravity * deltaTime;
        }
        else {
            velocity.y = 0.0f;  // ������������� � �����
        }
    }
};

// ������� �������� ������������ (����������� �������������)
bool checkCollision(const gameObject& obj1, const gameObject& obj2) {
    glm::vec3 pos1 = obj1.getPosition();
    glm::vec3 pos2 = obj2.getPosition();

    float distance = glm::length(pos1 - pos2);  // ���������� ����� ��������
    return distance <= (obj1.radius + obj2.radius);
}

// ��������� ������
const unsigned int SCR_WIDTH = 800;
const unsigned int SCR_HEIGHT = 600;
bool blinnKeyPressed = false;
bool is_spotlight = true;

// ������
Camera camera(glm::vec3(0.0f, 3.0f, 6.0f));
float lastX = (float)SCR_WIDTH / 2.0;
float lastY = (float)SCR_HEIGHT / 2.0;
bool firstMouse = true;

// �������
float deltaTime = 0.0f;
float lastFrame = 0.0f;

// ���� � �������
const char* present_path = "present/presentBox.obj";
const char* house_path = "house/house.obj";
const char* light_path = "light/light.obj";
const char* sledge_path = "sledge/sledge.obj";

// �������� ��������� (zeppelin)
glm::vec3 zeppelin_speed = glm::vec3(-10, 0, -10);

// ������ "����"
float present_cd = 0;    // ������� ������ �������
int cnt = 0;             // ������� �����
bool is_dropped = false; // ����, ��� ���� �������� �������

void AddPoint() {
    cnt++;
    std::cout << "Add point" << std::endl;
    std::cout << "cnt: " << cnt << std::endl << std::endl;
}

int main()
{
    // ������������� GLFW
    glfwInit();
    glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
    glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
    glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);

    // �������� ����
    GLFWwindow* window = glfwCreateWindow(SCR_WIDTH, SCR_HEIGHT, "LearnOpenGL", NULL, NULL);
    if (window == NULL)
    {
        std::cout << "Failed to create GLFW window" << std::endl;
        glfwTerminate();
        return -1;
    }
    glfwMakeContextCurrent(window);

    // �������
    glfwSetFramebufferSizeCallback(window, framebuffer_size_callback);
    glfwSetCursorPosCallback(window, mouse_callback);
    glfwSetScrollCallback(window, scroll_callback);

    // ������ ����
    glfwSetInputMode(window, GLFW_CURSOR, GLFW_CURSOR_DISABLED);

    // ������������� GLAD
    if (!gladLoadGLLoader((GLADloadproc)glfwGetProcAddress))
    {
        std::cout << "Failed to initialize GLAD" << std::endl;
        return -1;
    }

    // ��������� OpenGL
    glEnable(GL_DEPTH_TEST);
    glEnable(GL_BLEND);
    glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

    // �������
    Shader shader("lighting_fong.vs", "multilighting.fs");

    // ������ �������
    gameObject zeppelin("zeppelin/zeppelin.obj");
    gameObject tree("tree/lp_xmas_tree_w_presents.obj");

    // �������� ����
    gameObject sledge(sledge_path);
    // ������� ������� ������� � ������� (����� ����� �������������)
    sledge.Scale(glm::vec3(0.1f, 0.1f, 0.1f));
    // ���� �������� ��, ����� ��� �� ���� ������ ���� (�� ��� ��������� �����, �� ����� ����������� � �����)
    sledge.Translate(glm::vec3(3.0f, -0.5f, 0.0f));

    // ��� ��� ��������
    std::deque<gameObject> presents;
    // ��� ��� �����
    std::deque<gameObject> houses;
    // ���� ��� ���������� �����
    std::deque<gameObject> lightsObjs;
    std::deque<PointLight> lights_sources;

    // ��������� (���)
    float planeVertices[] = {
        // positions            // normals         // texcoords
         10.0f, -0.5f,  10.0f,  0.0f, 1.0f, 0.0f,   1.0f,  0.0f,
        -10.0f, -0.5f,  10.0f,  0.0f, 1.0f, 0.0f,   0.0f,  0.0f,
        -10.0f, -0.5f, -10.0f,  0.0f, 1.0f, 0.0f,   0.0f,  1.0f,

         10.0f, -0.5f,  10.0f,  0.0f, 1.0f, 0.0f,   1.0f,  0.0f,
        -10.0f, -0.5f, -10.0f,  0.0f, 1.0f, 0.0f,   0.0f,  1.0f,
         10.0f, -0.5f, -10.0f,  0.0f, 1.0f, 0.0f,   1.0f,  1.0f
    };
    unsigned int planeVAO, planeVBO;
    glGenVertexArrays(1, &planeVAO);
    glGenBuffers(1, &planeVBO);
    glBindVertexArray(planeVAO);
    glBindBuffer(GL_ARRAY_BUFFER, planeVBO);
    glBufferData(GL_ARRAY_BUFFER, sizeof(planeVertices), planeVertices, GL_STATIC_DRAW);

    glEnableVertexAttribArray(0);
    glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(float), (void*)0);

    glEnableVertexAttribArray(1);
    glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(float), (void*)(3 * sizeof(float)));

    glEnableVertexAttribArray(2);
    glVertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, 8 * sizeof(float), (void*)(6 * sizeof(float)));

    glBindVertexArray(0);

    // ��������
    unsigned int woodTex = loadTexture("wood.png");
    unsigned int snowTex = loadTexture("snow.jpg");
    unsigned int treeTex = loadTexture("tree.png");

    // �������������� ��������
    zeppelin.Translate(glm::vec3(5.0f, 3.0f, 2.0f));
    zeppelin.Scale(glm::vec3(0.1f, 0.1f, 0.1f));
    zeppelin.velocity = zeppelin_speed;

    // ������ (����) � ��� � (0, -0.5, 0)
    tree.Translate(glm::vec3(0, -0.5f, 0.0f));

    // ��������� ��������� �����
    srand(static_cast<unsigned int>(time(0)));

    // ���������� ��������� �����
    int numHouses = 5;
    for (int i = 0; i < numHouses; ++i) {
        float randomX = static_cast<float>((rand() % 20) - 10);
        float randomZ = static_cast<float>((rand() % 20) - 10);

        gameObject house(house_path);
        house.Translate(glm::vec3(randomX, -0.5f, randomZ));
        house.Scale(glm::vec3(0.1f, 0.1f, 0.1f));
        houses.push_back(house);
    }

    // ���������� ��������� ���������� pointLight
    int numLights = 5;
    for (int i = 0; i < numLights; ++i) {
        float randomX = static_cast<float>((rand() % 20) - 10);
        float randomZ = static_cast<float>((rand() % 20) - 10);

        gameObject lightObj(light_path);
        PointLight l(glm::vec3(randomX, 0.0f, randomZ));

        lightObj.Translate(glm::vec3(randomX, -0.5f, randomZ));
        lightObj.Scale(glm::vec3(0.1f, 0.1f, 0.1f));

        lightsObjs.push_back(lightObj);
        lights_sources.push_back(l);
    }

    // ������������ ���� (Directional Light)
    Light dirLight = Light(glm::vec3(-0.2f, -1.0f, -0.3f));

    shader.use();
    // ������������� ��������� ��� ���. �����
    shader.setVec3("dirLight.direction", dirLight.position);
    shader.setVec3("dirLight.ambient", dirLight.ambient);
    shader.setVec3("dirLight.diffuse", dirLight.diffuse);
    shader.setVec3("dirLight.specular", dirLight.specular);

    // ��������� �� ���������
    SpotLight zeppelin_light = SpotLight(zeppelin.transform[3]);
    shader.setVec3("spotLight.position", zeppelin_light.position);
    shader.setVec3("spotLight.direction", zeppelin_light.direction);
    shader.setVec3("spotLight.ambient", zeppelin_light.ambient);
    shader.setVec3("spotLight.diffuse", zeppelin_light.diffuse);
    shader.setVec3("spotLight.specular", zeppelin_light.specular);
    shader.setFloat("spotLight.constant", zeppelin_light.coef);
    shader.setFloat("spotLight.linear", zeppelin_light.linear);
    shader.setFloat("spotLight.quadratic", zeppelin_light.quadratic);
    shader.setFloat("spotLight.cutOff", glm::cos(glm::radians(zeppelin_light.cutOff)));
    shader.setFloat("spotLight.outerCutOff", glm::cos(glm::radians(zeppelin_light.outCutOff)));

    // ������������� pointLights
    for (int i = 0; i < (int)lights_sources.size(); i++) {
        shader.setVec3("pointLights[" + std::to_string(i) + "].position", lights_sources[i].position);
        shader.setVec3("pointLights[" + std::to_string(i) + "].ambient", lights_sources[i].ambient);
        shader.setVec3("pointLights[" + std::to_string(i) + "].diffuse", lights_sources[i].diffuse);
        shader.setVec3("pointLights[" + std::to_string(i) + "].specular", lights_sources[i].specular);
        shader.setFloat("pointLights[" + std::to_string(i) + "].constant", lights_sources[i].coef);
        shader.setFloat("pointLights[" + std::to_string(i) + "].linear", lights_sources[i].linear);
        shader.setFloat("pointLights[" + std::to_string(i) + "].quadratic", lights_sources[i].quadratic);
    }

    // ���������� ��� �������� �����
    float sledgeAngle = 0.0f;

    // ���� �������
    while (!glfwWindowShouldClose(window))
    {
        // ���������� deltaTime
        float currentFrame = static_cast<float>(glfwGetTime());
        deltaTime = currentFrame - lastFrame;
        lastFrame = currentFrame;
        present_cd += deltaTime;

        // ��������� �����
        processInput(window);

        // ����� ������� � ��������� ������ 2 ������� (���� ������ SPACE)
        if (is_dropped && present_cd >= 2.0f) {
            is_dropped = false;
            present_cd = 0.0f;

            // ����� �������
            gameObject present(present_path);
            // ������ ������� ��� ����������
            present.Translate(glm::vec3(zeppelin.transform[3].x,
                zeppelin.transform[3].y,
                zeppelin.transform[3].z));
            present.Scale(glm::vec3(0.1f, 0.1f, 0.1f));
            presents.push_back(present);
        }

        // ��������� ������� (���������� � �������, ���� �������� ����)
        for (auto it = presents.begin(); it != presents.end(); ) {
            it->ApplyGravity(deltaTime);
            it->MakeStep(deltaTime);

            if (it->transform[3].y <= -0.5f) {
                it = presents.erase(it);
            }
            else {
                ++it;
            }
        }

        // �������� ������������ �������� � ������
        for (auto houseIter = houses.begin(); houseIter != houses.end();) {
            bool houseDeleted = false;

            for (auto presentIter = presents.begin(); presentIter != presents.end();) {
                if (checkCollision(*presentIter, *houseIter)) {
                    houseIter = houses.erase(houseIter);
                    presentIter = presents.erase(presentIter);
                    AddPoint();
                    houseDeleted = true;
                    break;
                }
                else {
                    ++presentIter;
                }
            }
            if (!houseDeleted) {
                ++houseIter;
            }
        }

        // ������� �����
        glClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

        // ������� ������
        glm::mat4 projection = glm::perspective(glm::radians(camera.Zoom),
            (float)SCR_WIDTH / (float)SCR_HEIGHT,
            0.1f, 100.0f);
        glm::mat4 view = camera.GetViewMatrix();

        // ��������� �������
        shader.use();
        shader.setMat4("projection", projection);
        shader.setMat4("view", view);
        shader.setVec3("viewPos", camera.Position);
        shader.setBool("is_spotlight", is_spotlight);

        // ��������� ��������� (������� = ������� ���������)
        shader.setVec3("spotLight.position", zeppelin.transform[3]);
        shader.setVec3("spotLight.direction", zeppelin_light.direction);

        // ������� ��������
        zeppelin.velocity = zeppelin_speed;
        zeppelin.MakeStep(deltaTime);
        shader.setMat4("model", zeppelin.transform);
        zeppelin.Draw(shader);

        // ������ ����
        shader.setMat4("model", tree.transform);
        tree.Draw(shader);

        // ��������� ���� ������ ����
        sledgeAngle += 1.0f * deltaTime; // �������� ��������
        float radius = 1.5f; // ������ ������ ������ ����
        float offsetX = cos(sledgeAngle) * radius;
        float offsetZ = sin(sledgeAngle) * radius;

        // �������� �������
        glm::mat4 sledgeTransform = glm::mat4(1.0f);
        // �������� ���� � ���������� ������ (0, -0.5, 0)
        sledgeTransform = glm::translate(sledgeTransform, glm::vec3(offsetX, -0.35f, offsetZ));
        // ������������ ��, ����� ���������� �� ����������� � ����������
        // (����� ����� sledgeAngle, ����� ��� ��������� �� ���� ��������)
        sledgeTransform = glm::rotate(sledgeTransform, -sledgeAngle, glm::vec3(0.0f, 0.01f, 0.0f));
        // ������� (����� ���� � �� �������, �� ��� ����������� ��������)
        sledgeTransform = glm::scale(sledgeTransform, glm::vec3(0.1f));
        // ���������� � ������
        sledge.transform = sledgeTransform;

        // ������ ����
        shader.setMat4("model", sledge.transform);
        sledge.Draw(shader);

        // ������ �������
        for (auto& present : presents) {
            shader.setMat4("model", present.transform);
            present.Draw(shader);
        }

        // ������ ����
        for (auto& house : houses) {
            shader.setMat4("model", house.transform);
            house.Draw(shader);
        }

        // ������ ������� "����" � ���� pointLights
        for (auto& lightObj : lightsObjs) {
            shader.setMat4("model", lightObj.transform);
            lightObj.Draw(shader);
        }

        // ������ ���
        glm::mat4 model = glm::mat4(1.0f);
        shader.setMat4("model", model);
        glBindVertexArray(planeVAO);
        glActiveTexture(GL_TEXTURE0);
        glBindTexture(GL_TEXTURE_2D, snowTex);
        glDrawArrays(GL_TRIANGLES, 0, 6);
        glBindTexture(GL_TEXTURE_2D, 0);

        // ����� ������� � ����� �������
        glfwSwapBuffers(window);
        glfwPollEvents();
    }

    // ����������� �������
    glDeleteVertexArrays(1, &planeVAO);
    glDeleteBuffers(1, &planeVBO);

    glfwTerminate();
    return 0;
}

// ��������� ������
void processInput(GLFWwindow* window)
{
    if (glfwGetKey(window, GLFW_KEY_ESCAPE) == GLFW_PRESS)
        glfwSetWindowShouldClose(window, true);

    // �������� ������
    if (glfwGetKey(window, GLFW_KEY_W) == GLFW_PRESS)
        camera.ProcessKeyboard(FORWARD, deltaTime);
    if (glfwGetKey(window, GLFW_KEY_S) == GLFW_PRESS)
        camera.ProcessKeyboard(BACKWARD, deltaTime);
    if (glfwGetKey(window, GLFW_KEY_A) == GLFW_PRESS)
        camera.ProcessKeyboard(LEFT, deltaTime);
    if (glfwGetKey(window, GLFW_KEY_D) == GLFW_PRESS)
        camera.ProcessKeyboard(RIGHT, deltaTime);

    // ���������� ����������
    if (glfwGetKey(window, GLFW_KEY_UP) == GLFW_PRESS && last_pressed_key != GLFW_KEY_UP) {
        zeppelin_speed.z = -zeppelin_speed.z;
        last_pressed_key = GLFW_KEY_UP;
    }
    if (glfwGetKey(window, GLFW_KEY_DOWN) == GLFW_PRESS && last_pressed_key != GLFW_KEY_DOWN) {
        zeppelin_speed.z = -zeppelin_speed.z;
        last_pressed_key = GLFW_KEY_DOWN;
    }
    if (glfwGetKey(window, GLFW_KEY_LEFT) == GLFW_PRESS && last_pressed_key != GLFW_KEY_LEFT) {
        zeppelin_speed.x = -zeppelin_speed.x;
        last_pressed_key = GLFW_KEY_LEFT;
    }
    if (glfwGetKey(window, GLFW_KEY_RIGHT) == GLFW_PRESS && last_pressed_key != GLFW_KEY_RIGHT) {
        zeppelin_speed.x = -zeppelin_speed.x;
        last_pressed_key = GLFW_KEY_RIGHT;
    }

    // Q/E - �� ���� ���� ��������� �������� �� Y
    if (glfwGetKey(window, GLFW_KEY_Q) == GLFW_PRESS) {
        zeppelin_speed.y = -zeppelin_speed.y;
    }
    if (glfwGetKey(window, GLFW_KEY_E) == GLFW_PRESS) {
        zeppelin_speed.y = -zeppelin_speed.y;
    }

    // ���������� ������� (������) � ���� ������� ������
    if (glfwGetKey(window, GLFW_KEY_SPACE) == GLFW_PRESS && present_cd >= 2.0f) {
        is_dropped = true;
    }

    // ���/���� ��������� (spotlight)
    if (glfwGetKey(window, GLFW_KEY_P) == GLFW_PRESS && last_pressed_key != GLFW_KEY_P) {
        is_spotlight = !is_spotlight;
        last_pressed_key = GLFW_KEY_P;
    }
}

// ������ ��������� �������� ����
void framebuffer_size_callback(GLFWwindow* window, int width, int height)
{
    glViewport(0, 0, width, height);
}

// ������ �������� ����
void mouse_callback(GLFWwindow* window, double xposIn, double yposIn)
{
    float xpos = static_cast<float>(xposIn);
    float ypos = static_cast<float>(yposIn);

    if (firstMouse)
    {
        lastX = xpos;
        lastY = ypos;
        firstMouse = false;
    }

    float xoffset = xpos - lastX;
    float yoffset = lastY - ypos;
    lastX = xpos;
    lastY = ypos;

    camera.ProcessMouseMovement(xoffset, yoffset);
}

// ������ �������� ����
void scroll_callback(GLFWwindow* window, double xoffset, double yoffset)
{
    camera.ProcessMouseScroll(static_cast<float>(yoffset));
}

// ������� �������� ��������
unsigned int loadTexture(char const* path)
{
    unsigned int textureID;
    glGenTextures(1, &textureID);

    int width, height, nrComponents;
    unsigned char* data = stbi_load(path, &width, &height, &nrComponents, 0);
    if (data)
    {
        GLenum format;
        if (nrComponents == 1)
            format = GL_RED;
        else if (nrComponents == 3)
            format = GL_RGB;
        else if (nrComponents == 4)
            format = GL_RGBA;

        glBindTexture(GL_TEXTURE_2D, textureID);
        glTexImage2D(GL_TEXTURE_2D, 0, format,
            width, height, 0,
            format, GL_UNSIGNED_BYTE, data);
        glGenerateMipmap(GL_TEXTURE_2D);

        // ��������� ��������
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S,
            format == GL_RGBA ? GL_CLAMP_TO_EDGE : GL_REPEAT);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T,
            format == GL_RGBA ? GL_CLAMP_TO_EDGE : GL_REPEAT);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER,
            GL_LINEAR_MIPMAP_LINEAR);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);

        stbi_image_free(data);
    }
    else
    {
        std::cout << "Texture failed to load at path: " << path << std::endl;
        stbi_image_free(data);
    }

    return textureID;
}
