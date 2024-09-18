import cv2
import numpy as np
import matplotlib.pyplot as plt
from matplotlib.widgets import Slider

def update(val):
    h = h_slider.val
    s = s_slider.val
    v = v_slider.val

    hsv_mod = hsv_image.copy()

    hsv_mod[:, :, 0] = (hsv_mod[:, :, 0].astype(np.float32) * h).astype(np.uint8) % 180  # Оттенок (0-180) умножаем значения оттенка на коэффициент `h`, преобразуем в 8-битный беззнаковый формат и берем остаток от деления на 180, чтобы получить значение в диапазоне 0-180.
    hsv_mod[:, :, 1] = np.clip(hsv_mod[:, :, 1].astype(np.float32) * s, 0, 255).astype(np.uint8)  # Насыщенность (0-255)
    hsv_mod[:, :, 2] = np.clip(hsv_mod[:, :, 2].astype(np.float32) * v, 0, 255).astype(np.uint8)  # Яркость (0-255)

    rgb_mod = cv2.cvtColor(hsv_mod, cv2.COLOR_HSV2RGB)

    ax.imshow(rgb_mod)
    fig.canvas.draw_idle()

image_path = 'ФРУКТЫ.jpg'
image = cv2.cvtColor(cv2.imread(image_path), cv2.COLOR_BGR2RGB)

hsv_image = cv2.cvtColor(image, cv2.COLOR_RGB2HSV)

fig, ax = plt.subplots(figsize=(8, 6))
plt.subplots_adjust(left=0.25, bottom=0.35)

ax.imshow(image)
ax.set_title('Adjust HSV Parameters')

ax_h = plt.axes([0.25, 0.25, 0.65, 0.03], facecolor='lightgray')
ax_s = plt.axes([0.25, 0.18, 0.65, 0.03], facecolor='lightgray')
ax_v = plt.axes([0.25, 0.11, 0.65, 0.03], facecolor='lightgray')

h_slider = Slider(ax_h, 'Hue', 0.0, 180.0, valinit=1.0)
s_slider = Slider(ax_s, 'Saturation', 0.0, 2.0, valinit=1.0)
v_slider = Slider(ax_v, 'Value', 0.0, 2.0, valinit=1.0)

h_slider.on_changed(update)
s_slider.on_changed(update)
v_slider.on_changed(update)

save_button = plt.axes([0.25, 0.02, 0.15, 0.04])
button = plt.Button(save_button, 'Save Image')

def save_image(event):
    h = h_slider.val
    s = s_slider.val
    v = v_slider.val

    hsv_mod = hsv_image.copy()
    hsv_mod[:, :, 0] = (hsv_mod[:, :, 0].astype(np.float32) * h).astype(np.uint8) % 180
    hsv_mod[:, :, 1] = np.clip(hsv_mod[:, :, 1].astype(np.float32) * s, 0, 255).astype(np.uint8)
    hsv_mod[:, :, 2] = np.clip(hsv_mod[:, :, 2].astype(np.float32) * v, 0, 255).astype(np.uint8)

    rgb_mod = cv2.cvtColor(hsv_mod, cv2.COLOR_HSV2RGB)
    cv2.imwrite('modified_image.jpg', cv2.cvtColor(rgb_mod, cv2.COLOR_RGB2BGR))
    print('Изображение сохранено как "modified_image.jpg"')

button.on_clicked(save_image)

plt.show()