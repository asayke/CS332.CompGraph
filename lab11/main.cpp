#include <SFML/Graphics.hpp>
#include <GL/glew.h>
#include <iostream>
#include <cstdlib>
#include <vector>

// ID ��������� ���������
GLuint Program;
// ID ���������
GLint Attrib_vertex, Attrib_color;
// ID uniform-���������� ��� �����
GLint Uniform_color;
// ID Vertex Buffer Object
GLuint VBO, VBO_colors;

struct Vertex
{
    GLfloat x;
    GLfloat y;
};

int current_vertex_count = 3;
int shape_type = 0;
int color_mode = 0;
std::vector<Vertex> figure;
std::vector<GLfloat> colors; // ������ ������ ��� ������

// ���� ������ �������� ����������
const char* VertexShaderSource = R"(
 #version 330 core
 in vec2 coord;
 void main() {
    gl_Position = vec4(coord, 0.0, 1.0);
 }
)";

const char* FragShaderSource = R"(
 #version 330 core
 out vec4 color;
 void main() {
    color = vec4(1, 1, 1, 1);
 }
)";

// ���� �������� uniform-����������
const char* VertexShaderSourceUniform = R"(
 #version 330 core
 in vec2 coord;
 void main() {
    gl_Position = vec4(coord, 0.0, 1.0);
 }
)";

const char* FragShaderSourceUniform = R"(
 #version 330 core
 uniform vec4 color;
 out vec4 fragColor;
 void main() {
     fragColor = color;
 }
)";

// ��������
const char* VertexShaderSourceGrad = R"(
 #version 330 core
 in vec2 coord;
 in vec3 color;
 out vec3 vertex_color;
 void main() {
     gl_Position = vec4(coord, 0.0, 1.0);
     vertex_color = color; // �������� ���� � ����������� ������
 }
)";

const char* FragShaderSourceGrad = R"(
 #version 330 core
 in vec3 vertex_color;
 out vec4 color;
 void main() {
     color = vec4(vertex_color, 1.0); // ����������� ������� ������ �� �������
 }
)";

void ShaderLog(unsigned int shader)
{
    int infologLen = 0;
    glGetShaderiv(shader, GL_INFO_LOG_LENGTH, &infologLen);

    if (infologLen > 1)
    {
        int charsWritten = 0;
        std::vector<char> infoLog(infologLen);
        glGetShaderInfoLog(shader, infologLen, &charsWritten, infoLog.data());
        std::cout << "InfoLog: " << infoLog.data() << std::endl;
    }
}

void InitShader()
{
    // ������� ��������� ������
    GLuint vShader = glCreateShader(GL_VERTEX_SHADER);

    // ������� ����������� ������
    GLuint fShader = glCreateShader(GL_FRAGMENT_SHADER);

    // �������� �������� ���
    switch (color_mode)
    {
    case 0:
        glShaderSource(vShader, 1, &VertexShaderSource, NULL);
        glShaderSource(fShader, 1, &FragShaderSource, NULL);
        break;
    case 1:
        glShaderSource(vShader, 1, &VertexShaderSourceUniform, NULL);
        glShaderSource(fShader, 1, &FragShaderSourceUniform, NULL);
        break;
    case 2:
        glShaderSource(vShader, 1, &VertexShaderSourceGrad, NULL);
        glShaderSource(fShader, 1, &FragShaderSourceGrad, NULL);
        break;
    }

    // ����������� ������
    glCompileShader(vShader);
    std::cout << "vertex shader \n";

    // ������� ������ ���� �������
    ShaderLog(vShader);

    // ����������� ������
    glCompileShader(fShader);
    std::cout << "fragment shader \n";

    // ������� ������ ���� �������
    ShaderLog(fShader);

    // ������� ��������� � ����������� ������� � ���
    Program = glCreateProgram();
    glAttachShader(Program, vShader);
    glAttachShader(Program, fShader);

    // ������� ��������� ���������
    glLinkProgram(Program);

    // ��������� ������ ������
    int link_ok;
    glGetProgramiv(Program, GL_LINK_STATUS, &link_ok);

    if (!link_ok)
    {
        std::cout << "error attach shaders \n";
        return;
    }

    // ���������� ID ��������� �� ��������� ���������
    Attrib_vertex = glGetAttribLocation(Program, "coord");
    if (color_mode == 2)
        Attrib_color = glGetAttribLocation(Program, "color");

    if (Attrib_vertex == -1 || Attrib_color == -1)
    {
        std::cout << "could not bind attribs" << std::endl;
        return;
    }

    if (color_mode == 1)
    {
        Uniform_color = glGetUniformLocation(Program, "color");
        if (Uniform_color == -1)
        {
            std::cout << "could not bind uniform 'color'" << std::endl;
            return;
        }
    }
}

void InitVBO()
{
    glGenBuffers(1, &VBO);
    glGenBuffers(1, &VBO_colors);

    figure.clear();
    colors.clear();

    switch (shape_type)
    {
    case 0: // �����������
        figure = {
            {-0.8f, -0.8f},
            {0.0f, 0.8f},
            {0.8f, -0.8f}
        };
        current_vertex_count = 3;
        break;
    case 1: // �������������
        figure = {
            {-0.8f, -0.8f},
            {-0.8f, 0.8f},
            {0.8f, 0.8f},
            {0.8f, -0.8f}
        };
        current_vertex_count = 4;
        break;
    case 2: // ����
        figure = {
            {0.0, -0.8},
            {-0.8, 0.4},
            {-0.5, 0.6},
            {0.0, 0.8},
            {0.5, 0.6},
            {0.8, 0.4},
        };
        current_vertex_count = 6;
        break;
    case 3: // ������������
        figure = {
            {0.0, 0.9},
            {0.951, 0.209},
            {0.588, -0.909},
            {-0.588, -0.909 },
            {-0.951, 0.209 }
        };
        current_vertex_count = 5;
        break;
    }

    // �������� ������ � VBO ��� ������
    glBindBuffer(GL_ARRAY_BUFFER, VBO);
    glBufferData(GL_ARRAY_BUFFER, figure.size() * sizeof(Vertex), figure.data(), GL_STATIC_DRAW);


    // ��������� ��������� ������ ��� ������ �������
    if (color_mode == 2)
    {
        for (int i = 0; i < current_vertex_count; ++i)
        {
            colors.push_back(static_cast<float>(rand()) / static_cast<float>(RAND_MAX));
            colors.push_back(static_cast<float>(rand()) / static_cast<float>(RAND_MAX));
            colors.push_back(static_cast<float>(rand()) / static_cast<float>(RAND_MAX));
        }
        // �������� ������ � VBO ��� ������
        glBindBuffer(GL_ARRAY_BUFFER, VBO_colors);
        glBufferData(GL_ARRAY_BUFFER, colors.size() * sizeof(GLfloat), colors.data(), GL_STATIC_DRAW);
    }
}

void Init()
{
    // ������������� �������� � �������
    InitShader();
    InitVBO();
}

void Draw()
{
    glUseProgram(Program); // ������������� ��������� ��������� �������
    glEnableVertexAttribArray(Attrib_vertex); // �������� ������� ������
    glBindBuffer(GL_ARRAY_BUFFER, VBO); // ���������� VBO ��� ��������� ������
    glVertexAttribPointer(Attrib_vertex, 2, GL_FLOAT, GL_FALSE, 0, 0); // �������� ���������� ������

    if (color_mode == 2)
    {
        glEnableVertexAttribArray(Attrib_color); // �������� ������� ��� �����
        glBindBuffer(GL_ARRAY_BUFFER, VBO_colors); // ���������� VBO ��� ������
        glVertexAttribPointer(Attrib_color, 3, GL_FLOAT, GL_FALSE, 0, 0); // �������� ����� ��� ������ �������
    }

    glBindBuffer(GL_ARRAY_BUFFER, 0); // ��������� VBO

    switch (shape_type)
    {
    case 0:
        glUniform4f(Uniform_color, 1.0f, 0.0f, 0.0f, 1.0f); // �������
        break;
    case 1:
        glUniform4f(Uniform_color, 0.0f, 1.0f, 0.0f, 1.0f); // �������
        break;
    case 2:
        glUniform4f(Uniform_color, 0.0f, 0.0f, 1.0f, 1.0f); // �����
        break;
    case 3:
        glUniform4f(Uniform_color, 1.0f, 1.0f, 0.0f, 1.0f); // ������
        break;
    }

    glDrawArrays(GL_TRIANGLE_FAN, 0, current_vertex_count); // ������ ������

    glDisableVertexAttribArray(Attrib_vertex); // ��������� ������� ������

    if (color_mode == 2)
        glDisableVertexAttribArray(Attrib_color); // ��������� ������� �����

    glUseProgram(0); // ��������� ��������� ���������
}

// ������������ ������
void ReleaseVBO()
{
    glBindBuffer(GL_ARRAY_BUFFER, 0);
    glDeleteBuffers(1, &VBO);
    glDeleteBuffers(1, &VBO_colors);
}

// ������������ ��������
void ReleaseShader()
{
    glUseProgram(0); // ��������� ��������� ���������
    glDeleteProgram(Program); // ������� ���������
}

void Release()
{
    // ������������ ��������
    ReleaseShader();
    ReleaseVBO();
}

int main()
{
    sf::Window window(sf::VideoMode(600, 600), "Color figures", sf::Style::Default, sf::ContextSettings(24));
    window.setVerticalSyncEnabled(true);
    window.setActive(true);
    glewInit();

    Init();

    while (window.isOpen())
    {
        sf::Event event;

        while (window.pollEvent(event))
        {
            if (event.type == sf::Event::Closed)
                window.close();
            else if (event.type == sf::Event::Resized)
                glViewport(0, 0, event.size.width, event.size.height);
            if (event.type == sf::Event::KeyPressed)
            {
                switch (event.key.code)
                {
                case sf::Keyboard::Num1:
                    shape_type = 0;
                    InitVBO();
                    break;
                case sf::Keyboard::Num2:
                    shape_type = 1;
                    InitVBO();
                    break;
                case sf::Keyboard::Num3:
                    shape_type = 2;
                    InitVBO();
                    break;
                case sf::Keyboard::Num4:
                    shape_type = 3;
                    InitVBO();
                    break;
                case sf::Keyboard::Num8:
                    color_mode = 0;
                    InitShader();
                    break;
                case sf::Keyboard::Num9:
                    color_mode = 1;
                    InitShader();
                    break;
                case sf::Keyboard::Num0:
                    color_mode = 2;
                    InitVBO();
                    InitShader();
                    break;
                }
            }
        }

        glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

        Draw();

        window.display();
    }

    Release();
    return 0;
}
